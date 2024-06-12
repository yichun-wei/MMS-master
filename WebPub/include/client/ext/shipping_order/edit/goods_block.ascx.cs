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
using Seec.Marketing.NetTalk.WebService.Client;

public partial class include_client_ext_shipping_order_edit_goods_block : System.Web.UI.UserControl
{
    const string USER_CONTROL_LOAD_SEQ = "UserControlLoadSeq";
    List<ASP.include_client_ext_shipping_order_edit_goods_item_block_ascx> _blockGoodsItemList = new List<ASP.include_client_ext_shipping_order_edit_goods_item_block_ascx>();

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
        /// 外銷訂單系統代號。
        /// </summary>
        public const string ExtOrderSId = "ExtOrderSId";
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

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!SystemId.MinValue.IsSystemId(this.GetPgAttr(PgAttrDefine.SId)))
        {
            this.SetPgAttr(PgAttrDefine.SId, new SystemId().Value);
        }

        #region 重載附件清單
        if (this.IsPostBack)
        {
            if (ViewState[USER_CONTROL_LOAD_SEQ] != null)
            {
                string[] loadSeq = StringLib.SplitSurrounded(ViewState[USER_CONTROL_LOAD_SEQ].ToString(), new LeftRightPair<char, char>('[', ']'));
                for (int i = 0; i < loadSeq.Length; i++)
                {
                    switch (loadSeq[i])
                    {
                        case "GoodsItem":
                            var blockGoodsItem = (ASP.include_client_ext_shipping_order_edit_goods_item_block_ascx)this.LoadControl("~/include/client/ext/shipping_order/edit/goods_item_block.ascx");
                            blockGoodsItem.Remove += new include_client_ext_shipping_order_edit_goods_item_block.RemoveEventHandler(GoodsItem_OnRemove);
                            this._blockGoodsItemList.Add(blockGoodsItem);
                            this.phGoodsItemList.Controls.Add(blockGoodsItem);
                            break;
                    }
                }
            }
        }
        #endregion
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        this.divGoodsBlock.CssClass = string.Format("dev-goods-block dev-goods-block-{0}", this.GetPgAttr(PgAttrDefine.SId));
        WebUtilBox.AddAttribute(this.lnkSearchAdd, "onclick", string.Format("orderItemHelper.openWindow('{0}');", this.GetPgAttr(PgAttrDefine.SId)));

        //還原值
        this.lblSubtotal.Text = ConvertLib.ToAccounting(ConvertLib.ToSingle(this.hidSubtotal.Value, DefVal.Float), string.Empty);
    }

    /// <summary>
    /// 系統代號。
    /// </summary>
    public ISystemId SId
    {
        get { return ConvertLib.ToSId(this.GetPgAttr(PgAttrDefine.SId)); }
    }

    /// <summary>
    /// 區塊標題。
    /// </summary>
    public string Title
    {
        get { return this.litTitle.Text; }
        set { this.litTitle.Text = value; }
    }

    #region 設定資訊
    public void SetInfo(ExtShippingOrderHelper.GoodsEditInfo_OdrBase editInfo)
    {
        this.SetInfo(editInfo, false);
    }

    public void SetInfo(ExtShippingOrderHelper.GoodsEditInfo_OdrBase editInfo, bool addMode)
    {
        if (editInfo == null)
        {
            this.Visible = false;
            return;
        }

        this.litTitle.Text = editInfo.Title;
        this.SetPgAttr(PgAttrDefine.ExtOrderSId, editInfo.ExtOrderSId.Value);
        this.hidSubtotal.Value = editInfo.Items.Sum(q => ConvertLib.ToSingle(q.PaidAmt, 0)).ToString();

        if (editInfo.ExtOrderSId != null)
        {
            //設定外銷訂單錨點.
            this.litAnchor.Text = string.Format("<a name='GoodsBlock_{0}'></a>", editInfo.ExtOrderSId);
        }

        #region 初始品項清單
        if (editInfo.Items != null && editInfo.Items.Count > 0)
        {
            //已存在的品項暫存
            var existedGoodsItems = new List<string>();

            //一般品項暫存的是「料號-外銷訂單明細系統代號」
            existedGoodsItems.AddRange(editInfo.Items.Select(q => string.Format("{1}{0}{2}", SystemDefine.JoinSeparator, q.PartNo, q.ExtOrderDetSId)).ToArray());

            foreach (var itemEditInfo in editInfo.Items)
            {
                var block = (ASP.include_client_ext_shipping_order_edit_goods_item_block_ascx)this.LoadControl("~/include/client/ext/shipping_order/edit/goods_item_block.ascx");
                block.Remove += new include_client_ext_shipping_order_edit_goods_item_block.RemoveEventHandler(GoodsItem_OnRemove);
                this._blockGoodsItemList.Add(block);
                this.phGoodsItemList.Controls.Add(block);
                block.SetInfo(itemEditInfo);

                if (addMode)
                {
                    //不是初始載入的, 是選擇外銷訂單後預設載入全部外銷訂單品項用的.
                    ViewState[USER_CONTROL_LOAD_SEQ] += "[GoodsItem]";
                }
            }

            this.hidExistedGoodsItems.Value = string.Join(",", existedGoodsItems.GroupBy(q => q).Select(q => q.Key).ToArray());
        }
        #endregion
    }
    #endregion

    #region 取得輸入資訊
    public ExtShippingOrderHelper.GoodsEditInfo_OdrBase GetInfo()
    {
        var editInfo = new ExtShippingOrderHelper.GoodsEditInfo_OdrBase()
        {
            Title = this.litTitle.Text,
            ExtOrderSId = ConvertLib.ToSId(this.GetPgAttr(PgAttrDefine.ExtOrderSId))
        };

        foreach (var block in this._blockGoodsItemList)
        {
            if (block.Visible)
            {
                editInfo.Items.Add(block.GetInfo());
            }
        }

        return editInfo;
    }
    #endregion

    #region 查詢新增區塊
    protected void btnSearchAdd_Click(object sender, EventArgs e)
    {
        var seledGoodsItems = WebUtilBox.FindControl<HiddenField>(this.Page, "hidSeledGoodsItems").Value.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
        if (seledGoodsItems.Length == 0)
        {
            return;
        }

        ////倉庫
        //var whse = WebUtilBox.FindControl<DropDownList>(this.Page, "lstWhseList").SelectedValue;
        var priceListId = ConvertLib.ToLong(WebUtilBox.FindControl<HiddenField>(this.Page, "hidPriceListId").Value, DefVal.Long);
        //客戶 ID (一定有值, 若沒值, 則取不到或出錯唄.)
        var customerId = Convert.ToInt64(WebUtilBox.FindControl<HiddenField>(this.Page, "hidCustomerId").Value);

        Returner returner = null;
        try
        {
            //已存在的品項暫存
            var existedGoodsItems = new List<string>();
            existedGoodsItems.AddRange(this.hidExistedGoodsItems.Value.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries));

            #region 一般品項
            do
            {
                var goodsItems = seledGoodsItems.Where(q => q.IndexOf(SystemDefine.JoinSeparator) != -1).ToArray();
                if (goodsItems.Length == 0)
                {
                    break;
                }

                var extOrderDetSIds = ConvertLib.ToSIds(goodsItems.Select(q => ConvertLib.ToSId(q.Split(new string[] { SystemDefine.JoinSeparator }, StringSplitOptions.None)[1])).ToArray());
                if (extOrderDetSIds.Length == 0)
                {
                    break;
                }

                ExtOrderDet entityExtOrderDet = new ExtOrderDet(SystemDefine.ConnInfo);

                //為了效能, 依「外銷訂單明細系統代號」直接取出, 驗證待儲存時再處理.
                var conds = new ExtOrderDet.InfoViewConds
                    (
                       extOrderDetSIds,
                       customerId,
                       ConvertLib.ToSIds(this.GetPgAttr(PgAttrDefine.ExtOrderSId)),
                       DefVal.Int,
                       DefVal.Str,
                       DefVal.Bool,
                       false,
                       IncludeScope.OnlyNotMarkDeleted
                    );

                SqlOrder sorting = SqlOrder.Default;
                returner = entityExtOrderDet.GetInfoView(conds, Int32.MaxValue, new SqlOrder("SORT", Sort.Ascending));
                if (returner.IsCompletedAndContinue)
                {
                    var infos = ExtOrderDet.InfoView.Binding(returner.DataSet.Tables[0]);

                    //不顯示在手量
                    ////先取得所有品項的在手量
                    //var onHandInfos = ErpHelper.GetOnHandInfo(infos.Where(q => !string.IsNullOrWhiteSpace(q.Item)).Select(q => q.Item).ToArray());

                    //先取得所有品項的價目表
                    var priceBookInfos = new ErpHelper.PriceBookInfo[0];
                    if (priceListId.HasValue)
                    {
                        priceBookInfos = ErpHelper.GetPriceBookInfo(priceListId.Value, infos.Select(q => q.PartNo).ToArray());
                    }

                    foreach (var info in infos)
                    {
                        //已存在的就不再加入
                        if (existedGoodsItems.Contains(string.Format("{1}{0}{2}", SystemDefine.JoinSeparator, info.PartNo, info.SId)))
                        {
                            continue;
                        }

                        //加入已存在的品項暫存
                        existedGoodsItems.Add(string.Format("{1}{0}{2}", SystemDefine.JoinSeparator, info.PartNo, info.SId));

                        var block = (ASP.include_client_ext_shipping_order_edit_goods_item_block_ascx)this.LoadControl("~/include/client/ext/shipping_order/edit/goods_item_block.ascx");
                        block.Remove += new include_client_ext_shipping_order_edit_goods_item_block.RemoveEventHandler(GoodsItem_OnRemove);
                        this._blockGoodsItemList.Add(block);
                        this.phGoodsItemList.Controls.Add(block);

                        var editInfo = new ExtShippingOrderHelper.GoodsItemEditInfo_OdrBase()
                        {
                            SeqNo = this._blockGoodsItemList.Where(q => q.Visible).Count(), //先加入再算序號, 故而不用 + 1.
                            ExtOrderDetSId = info.SId,
                            Model = info.Model,
                            PartNo = info.PartNo,
                            MaxQty = info.Qty - info.ShipOdrUseQty,
                            Discount = info.Discount,
                            Rmk = info.Rmk
                        };

                        //不顯示在手量
                        //editInfo.ErpOnHand = ErpHelper.GetOnHandQty(onHandInfos, whse, info.Item);
                        editInfo.ListPrice = ErpHelper.GetPriceBookListPrice(priceBookInfos, info.PartNo);

                        block.SetInfo(editInfo);

                        ViewState[USER_CONTROL_LOAD_SEQ] += "[GoodsItem]";
                    }
                }
            } while (false);
            #endregion

            //重整已存在的品項暫存
            this.hidExistedGoodsItems.Value = string.Join(",", existedGoodsItems.GroupBy(q => q).Select(q => q.Key).ToArray());

            //計算整張訂單金額
            WebUtilBox.RegisterScript(this.Page, "orderItemHelper.calcOrderAmt();");
        }
        finally
        {
            if (returner != null) { returner.Dispose(); }
        }
    }
    #endregion

    protected void GoodsItem_OnRemove(object sender, EventArgs e)
    {
        //重整排序
        int seqNo = 0;
        foreach (var block in this._blockGoodsItemList)
        {
            if (block.Visible)
            {
                block.SeqNo = ++seqNo;
            }
        }

        #region 品項暫存移除
        //已存在的品項暫存
        var existedGoodsItems = new List<string>();
        existedGoodsItems.AddRange(this.hidExistedGoodsItems.Value.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries));

        var itemEditInfo = ((ASP.include_client_ext_shipping_order_edit_goods_item_block_ascx)sender).GetInfo();
        //一般品項
        existedGoodsItems.Remove(string.Format("{1}{0}{2}", SystemDefine.JoinSeparator, itemEditInfo.PartNo, itemEditInfo.ExtOrderDetSId));

        //重整已存在的品項暫存
        this.hidExistedGoodsItems.Value = string.Join(",", existedGoodsItems.GroupBy(q => q).Select(q => q.Key).ToArray());
        #endregion

        if (seqNo == 0)
        {
            //若已沒有任何項目, 則移除.
            WebUtilBox.RegisterScript(this.Page, string.Format("extOrderHelper.removeExistedExtOrders('{0}');", this.GetPgAttr(PgAttrDefine.ExtOrderSId)));
            this.Visible = false;
        }

        //計算整張訂單金額
        WebUtilBox.RegisterScript(this.Page, "orderItemHelper.calcOrderAmt();");
    }
}