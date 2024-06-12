using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using System.Transactions;

using EzCoding;
using EzCoding.Collections;
using EzCoding.Web.UI;
using EzCoding.DB;
using Seec.Marketing;
using Seec.Marketing.SystemEngines;
using Seec.Marketing.Erp;

public partial class include_client_ext_order_edit_manual_goods_item_block : System.Web.UI.UserControl
{
    #region 網頁屬性
    /// <summary>
    /// 屬性常數定義。
    /// </summary>
    struct PgAttrDefine
    {
        /// <summary>
        /// 系統代號。
        /// </summary>
        public const string SId = "SId";
        /// <summary>
        /// 商品來源（1:一般品項 2:手動新增）。
        /// </summary>
        public const string GoodsSrc = "GoodsSrc";
        /// <summary>
        /// 牌價。
        /// 若有值，表示已檢查過料號是否存在價目表。
        /// </summary>
        public const string ListPrice = "ListPrice";
    }

    string GetPgAttr(string key)
    {
        return this.InitVals.Attributes[key];
    }

    void SetPgAttr(string key, string value)
    {
        this.InitVals.Attributes[key] = value;
    }
    #endregion

    protected void Page_Init(object sender, EventArgs e)
    {

    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        WebUtilBox.AddAttribute(this.htmlTr, "class", string.Format("dev-goods-item dev-goods-item-{0}", this.GetPgAttr(PgAttrDefine.SId)));
        WebUtilBox.AddAttribute(this.lnkSearchMapping, "onclick", string.Format("orderItemHelper.openWindow('{0}');", this.GetPgAttr(PgAttrDefine.SId)));
    }

    /// <summary>
    /// 序號。
    /// </summary>
    public int SeqNo
    {
        get { return Convert.ToInt32(this.litSeqNo.Text); }
        set { this.litSeqNo.Text = value.ToString(); }
    }

    #region 設定資訊
    public void SetInfo(ExtOrderHelper.GoodsItemEditInfo editInfo)
    {
        if (editInfo == null)
        {
            this.Visible = false;
            return;
        }

        //商品來源（1:一般品項 2:手動新增）
        this.SetPgAttr(PgAttrDefine.GoodsSrc, "2");

        this.SetPgAttr(PgAttrDefine.SId, editInfo.SId.Value);
        this.litSeqNo.Text = editInfo.SeqNo.ToString();
        this.litModel.Text = editInfo.Model;
        this.litPartNo.Text = editInfo.PartNo;
        this.lblQty.Text = ConvertLib.ToAccounting(editInfo.Qty, string.Empty);
        this.SetPgAttr(PgAttrDefine.ListPrice, ConvertLib.ToStr(editInfo.ListPrice, string.Empty));

        if (editInfo.UnitPrice.HasValue)
        {
            this.lblUnitPrice.Text = editInfo.UnitPrice.ToString();
        }
        else if (editInfo.ListPrice.HasValue)
        {
            //若無單價, 則牌價 * 折扣.
            this.lblUnitPrice.Text = MathLib.Round(editInfo.ListPrice.Value * ConvertLib.ToSingle(editInfo.Discount, 1), 4).ToString();
        }

        if (editInfo.Discount.HasValue)
        {
            this.lblDiscount.Text = MathLib.Round(editInfo.Discount.Value * 100, 2).ToString();
        }
        else
        {
            var unitPrice = ConvertLib.ToSingle(this.lblUnitPrice.Text, DefVal.Float);
            if (editInfo.ListPrice.HasValue && unitPrice.HasValue)
            {
                this.lblDiscount.Text = MathLib.Round(unitPrice.Value / editInfo.ListPrice.Value * 100, 2).ToString();
            }
        }

        this.lblPaidAmt.Text = ConvertLib.ToAccounting(editInfo.PaidAmt, string.Empty);

        this.lblRmk.Text = editInfo.Rmk;

        if (editInfo.ListPrice == null)
        {
            //若料號比對不到價格, 要秀出錯誤訊息提示
            WebUtilBox.AddAttribute(this.htmlTr, "class", "error");
        }
    }
    #endregion

    #region 取得輸入資訊
    public ExtOrderHelper.GoodsItemEditInfo GetInfo()
    {
        return new ExtOrderHelper.GoodsItemEditInfo()
        {
            SId = ConvertLib.ToSId(this.GetPgAttr(PgAttrDefine.SId)),
            Source = Convert.ToInt32(this.GetPgAttr(PgAttrDefine.GoodsSrc)),
            SeqNo = Convert.ToInt32(this.litSeqNo.Text),
            Model = this.litModel.Text,
            PartNo = this.litPartNo.Text,
            //ErpOnHand = ConvertLib.ToInt(this.lblErpOnHand.Text, DefVal.Int),
            Qty = ConvertLib.ToInt(this.lblQty.Text, DefVal.Int),
            ListPrice = ConvertLib.ToSingle(this.GetPgAttr(PgAttrDefine.ListPrice), DefVal.Float),
            UnitPrice = ConvertLib.ToSingle(this.lblUnitPrice.Text, DefVal.Float),
            Discount = ConvertLib.ToSingle(this.lblDiscount.Text, DefVal.Float),
            PaidAmt = ConvertLib.ToSingle(this.lblPaidAmt.Text, DefVal.Float),
            Rmk = this.lblRmk.Text
        };
    }
    #endregion

    /// <summary>
    /// 選擇的外銷商品。
    /// </summary>
    class SeledExtGoods
    {
        public string Model { get; set; }
        public string PartNo { get; set; }
    }

    protected void btnSearchMapping_Click(object sender, EventArgs e)
    {
        var seledGoodsItems = WebUtilBox.FindControl<HiddenField>(this.Page, "hidSeledGoodsItems").Value.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
        if (seledGoodsItems.Length == 0)
        {
            return;
        }

        ////倉庫
        //var whse = WebUtilBox.FindControl<DropDownList>(this.Page, "lstWhseList").SelectedValue;
        var priceListId = ConvertLib.ToLong(WebUtilBox.FindControl<HiddenField>(this.Page, "hidPriceListId").Value, DefVal.Long);

        var hidExistedGoodsItems = WebUtilBox.FindControl<HiddenField>(this.Page, "hidExistedGoodsItems");

        Returner returner = null;
        try
        {
            //已存在的品項暫存
            var existedGoodsItems = new List<string>();
            existedGoodsItems.AddRange(hidExistedGoodsItems.Value.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries));

            //選擇的外銷商品陣列集合
            List<SeledExtGoods> seledExtGoodsList = new List<SeledExtGoods>();

            #region 一般品項
            do
            {
                //[by fan] 加入型號「型號[#]料號」
                foreach (var item in seledGoodsItems)
                {
                    var split = item.Split(new string[] { SystemDefine.JoinSeparator }, StringSplitOptions.None);
                    seledExtGoodsList.Add(new SeledExtGoods() { Model = ConvertLib.ToBase64Decoding(split[0]), PartNo = split[1] });
                }

                //[by fan] 加入型號「型號[#]料號」
                //var goodsItems = seledGoodsItems.Where(q => q.IndexOf(SystemDefine.JoinSeparator) == -1).ToArray();
                //僅取得有料號的品項
                var goodsItems = seledExtGoodsList.Where(q => !string.IsNullOrWhiteSpace(q.PartNo)).Select(q => q.PartNo).ToArray();
                if (goodsItems.Length == 0)
                {
                    break;
                }

                //只會有一筆
                //var goodsItem = seledGoodsItems[0];
                //[by fan] 加入型號「型號[#]料號」
                var goodsItem = seledExtGoodsList[0].PartNo;

                ErpInv entityErpInv = new ErpInv(SystemDefine.ConnInfo);

                var conds = new ErpInv.InfoConds
                     (
                        DefVal.SIds,
                        DefVal.Longs,
                        ConvertLib.ToStrs(goodsItem),
                        DefVal.Str,
                        DefVal.Str,
                        DefVal.Str,
                        DefVal.Bool
                     );

                SqlOrder sorting = SqlOrder.Default;
                returner = entityErpInv.GetInfo(conds, 1, SqlOrder.Default, IncludeScope.All);
                if (returner.IsCompletedAndContinue)
                {
                    var info = ErpInv.Info.Binding(returner.DataSet.Tables[0].Rows[0]);

                    //不顯示在手量
                    ////先取得所有品項的在手量
                    //var onHandInfos = ErpHelper.GetOnHandInfo(infos.Where(q => !string.IsNullOrWhiteSpace(q.Item)).Select(q => q.Item).ToArray());

                    //先取得所有品項的價目表
                    var priceBookInfos = new ErpHelper.PriceBookInfo[0];
                    if (priceListId.HasValue)
                    {
                        priceBookInfos = ErpHelper.GetPriceBookInfo(priceListId.Value, ConvertLib.ToStrs(info.Item));
                    }

                    //[by fan] 改為使用外銷商品的型號
                    var model = seledExtGoodsList.Where(q => info.Item.Equals(q.PartNo)).Single().Model;
                    var existedGoodsItem = string.Format("{1}{0}{2}", SystemDefine.JoinSeparator, ConvertLib.ToBase64Encoding(model), info.Item);

                    //已存在的就不再加入
                    if (existedGoodsItems.Contains(existedGoodsItem))
                    {
                        break;
                    }

                    //移除先前的品項暫存
                    existedGoodsItems = existedGoodsItems.Where(q => q != string.Format("{1}{0}{2}", SystemDefine.JoinSeparator, ConvertLib.ToBase64Encoding(this.litModel.Text), this.litPartNo.Text)).ToList();

                    //加入已存在的品項暫存
                    existedGoodsItems.Add(existedGoodsItem);

                    var qty = Convert.ToSingle(this.lblQty.Text);
                    var unitPrice = Convert.ToSingle(this.lblUnitPrice.Text);
                    var discount = ConvertLib.ToSingle(this.lblDiscount.Text, 100);
                    var listPrice = ErpHelper.GetPriceBookListPrice(priceBookInfos, info.Item);

                    //[by fan] 改為使用外銷商品的型號
                    //this.litModel.Text = info.Model;
                    this.litModel.Text = model;
                    
                    this.litPartNo.Text = info.Item;
                    //this.lblQty.Text = ConvertLib.ToAccounting(editInfo.Qty, string.Empty);
                    this.SetPgAttr(PgAttrDefine.ListPrice, ConvertLib.ToStr(listPrice, string.Empty));

                    #region 單價不變
                    //單價不變的情況下, 折扣有可能破 100%.
                    //以牌價、單價重新計算折扣
                    if (listPrice.HasValue)
                    {
                        discount = (float)MathLib.Round(unitPrice / listPrice.Value * 100, 2);
                        this.lblDiscount.Text = discount.ToString();
                    }
                    #endregion

                    #region 折扣不變
                    ////以牌價、折扣重新計算單價
                    //if (listPrice.HasValue)
                    //{
                    //    unitPrice = (float)MathLib.Round(listPrice.Value * (discount / 100), 4);
                    //    this.lblUnitPrice.Text = unitPrice.ToString();
                    //}
                    #endregion

                    this.lblPaidAmt.Text = ConvertLib.ToStr(unitPrice * qty, string.Empty);

                    if (listPrice == null)
                    {
                        //若料號比對不到價格, 要秀出錯誤訊息提示
                        WebUtilBox.AddAttribute(this.htmlTr, "class", "error");
                    }
                    else
                    {
                        WebUtilBox.RemoveAttribute(this.htmlTr, "class", "error");
                    }
                }
            } while (false);
            #endregion

            //重整已存在的品項暫存
            hidExistedGoodsItems.Value = string.Join(",", existedGoodsItems.GroupBy(q => q).Select(q => q.Key).ToArray());

            //計算整張訂單金額
            WebUtilBox.RegisterScript(this.Page, "orderItemHelper.calcOrderAmt();");
        }
        finally
        {
            if (returner != null) { returner.Dispose(); }
        }
    }
}