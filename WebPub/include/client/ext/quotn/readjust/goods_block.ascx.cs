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

public partial class include_client_ext_quotn_readjust_goods_block : System.Web.UI.UserControl
{
    const string USER_CONTROL_LOAD_SEQ = "UserControlLoadSeq";
    List<ASP.include_client_ext_quotn_readjust_goods_item_block_ascx> _blockGoodsItemList = new List<ASP.include_client_ext_quotn_readjust_goods_item_block_ascx>();

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
                            var blockGoodsItem = (ASP.include_client_ext_quotn_readjust_goods_item_block_ascx)this.LoadControl("~/include/client/ext/quotn/readjust/goods_item_block.ascx");
                            blockGoodsItem.Remove += new include_client_ext_quotn_readjust_goods_item_block.RemoveEventHandler(GoodsItem_OnRemove);
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
        WebUtilBox.AddAttribute(this.lnkInputAdd, "onclick", string.Format("orderItemHelper.showInputAdd('{0}');", this.GetPgAttr(PgAttrDefine.SId)));

        this.hidAutoCompleteClientID.Value = this.acGoodsKeyword.ClientID;

        this.acGoodsKeyword.OnClientItemSelected = string.Format("orderItemHelper.onAutoCompleteGoodsSeled");

        WebUtilBox.AddAttribute(this.txtGoodsKeyword, "onkeyup", string.Format("orderItemHelper.regCntrSId('{0}');", this.GetPgAttr(PgAttrDefine.SId)));
        WebUtilBox.AddAttribute(this.txtGoodsKeyword, "onkeyup", string.Format("orderItemHelper.setAutoCompleteContextKey();"));

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
    public void SetInfo(ExtOrderHelper.GoodsEditInfo editInfo)
    {
        if (editInfo == null)
        {
            this.Visible = false;
            return;
        }

        this.hidSubtotal.Value = editInfo.Items.Sum(q => ConvertLib.ToSingle(q.PaidAmt, 0)).ToString();

        #region 初始品項清單
        if (editInfo.Items != null && editInfo.Items.Count > 0)
        {
            //已存在的品項暫存
            var existedGoodsItems = new List<string>();

            ////一般品項暫存的是「料號」
            //existedGoodsItems.AddRange(editInfo.Items.Select(q => q.PartNo).ToArray());
            //[by fan] 加入型號「型號[#]料號」
            existedGoodsItems.AddRange(editInfo.Items.Select(q => string.Format("{1}{0}{2}", SystemDefine.JoinSeparator, ConvertLib.ToBase64Encoding(q.Model), q.PartNo)).ToArray());

            foreach (var itemEditInfo in editInfo.Items)
            {
                var block = (ASP.include_client_ext_quotn_readjust_goods_item_block_ascx)this.LoadControl("~/include/client/ext/quotn/readjust/goods_item_block.ascx");
                block.Remove += new include_client_ext_quotn_readjust_goods_item_block.RemoveEventHandler(GoodsItem_OnRemove);
                this._blockGoodsItemList.Add(block);
                this.phGoodsItemList.Controls.Add(block);
                block.SetInfo(itemEditInfo);
            }

            this.hidExistedGoodsItems.Value = string.Join(",", existedGoodsItems.GroupBy(q => q).Select(q => q.Key).ToArray());
        }
        #endregion
    }
    #endregion

    #region 取得輸入資訊
    public ExtOrderHelper.GoodsEditInfo GetInfo()
    {
        var editInfo = new ExtOrderHelper.GoodsEditInfo()
        {
            Title = this.litTitle.Text
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

    /// <summary>
    /// 選擇的外銷商品。
    /// </summary>
    class SeledExtGoods
    {
        public string Model { get; set; }
        public string PartNo { get; set; }
    }

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

        Returner returner = null;
        try
        {
            //已存在的品項暫存
            var existedGoodsItems = new List<string>();
            existedGoodsItems.AddRange(this.hidExistedGoodsItems.Value.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries));

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

                ErpInv entityErpInv = new ErpInv(SystemDefine.ConnInfo);

                var conds = new ErpInv.InfoConds
                     (
                        DefVal.SIds,
                        DefVal.Longs,
                        goodsItems,
                        DefVal.Str,
                        DefVal.Str,
                        DefVal.Str,
                        DefVal.Bool
                     );

                SqlOrder sorting = SqlOrder.Default;
                returner = entityErpInv.GetInfo(conds, Int32.MaxValue, SqlOrder.Default, IncludeScope.All);
                if (returner.IsCompletedAndContinue)
                {
                    var infos = ErpInv.Info.Binding(returner.DataSet.Tables[0]);

                    //不顯示在手量
                    ////先取得所有品項的在手量
                    //var onHandInfos = ErpHelper.GetOnHandInfo(infos.Where(q => !string.IsNullOrWhiteSpace(q.Item)).Select(q => q.Item).ToArray());

                    //先取得所有品項的價目表
                    var priceBookInfos = new ErpHelper.PriceBookInfo[0];
                    if (priceListId.HasValue)
                    {
                        priceBookInfos = ErpHelper.GetPriceBookInfo(priceListId.Value, infos.Select(q => q.Item).ToArray());
                    }

                    foreach (var info in infos)
                    {
                        //[by fan] 改為使用外銷商品的型號
                        var model = seledExtGoodsList.Where(q => info.Item.Equals(q.PartNo)).Single().Model;
                        var existedGoodsItem = string.Format("{1}{0}{2}", SystemDefine.JoinSeparator, ConvertLib.ToBase64Encoding(model), info.Item);

                        //已存在的就不再加入
                        if (existedGoodsItems.Contains(existedGoodsItem))
                        {
                            continue;
                        }

                        //加入已存在的品項暫存
                        existedGoodsItems.Add(existedGoodsItem);

                        var block = (ASP.include_client_ext_quotn_readjust_goods_item_block_ascx)this.LoadControl("~/include/client/ext/quotn/readjust/goods_item_block.ascx");
                        block.Remove += new include_client_ext_quotn_readjust_goods_item_block.RemoveEventHandler(GoodsItem_OnRemove);
                        this._blockGoodsItemList.Add(block);
                        this.phGoodsItemList.Controls.Add(block);

                        var editInfo = new ExtOrderHelper.GoodsItemEditInfo()
                        {
                            SeqNo = this._blockGoodsItemList.Where(q => q.Visible).Count(), //先加入再算序號, 故而不用 + 1.
                            //[by fan] 改為使用外銷商品的型號
                            //Model = info.Model,
                            Model = model,
                            PartNo = info.Item,
                            Discount = 1
                        };

                        //不顯示在手量
                        //editInfo.ErpOnHand = ErpHelper.GetOnHandQty(onHandInfos, whse, info.Item);
                        editInfo.ListPrice = ErpHelper.GetPriceBookListPrice(priceBookInfos, info.Item);

                        block.SetInfo(editInfo);

                        ViewState[USER_CONTROL_LOAD_SEQ] += "[GoodsItem]";
                    }
                }
            } while (false);
            #endregion

            //重整已存在的品項暫存
            this.hidExistedGoodsItems.Value = string.Join(",", existedGoodsItems.GroupBy(q => q).Select(q => q.Key).ToArray());

            #region 觸發事件
            if (this.AddComplete != null)
            {
                //只保留沒有料號的商品. for 搜尋新增, 若選擇的品項沒有料號時, 則加到手動新增去.
                this.AddComplete(sender, new CustEventArgs() { Tag = seledExtGoodsList.Where(q => string.IsNullOrWhiteSpace(q.PartNo)).Select(q => q.Model).ToArray() });
            }
            #endregion

            //計算整張訂單金額
            WebUtilBox.RegisterScript(this.Page, "orderItemHelper.calcOrderAmt();");
        }
        finally
        {
            if (returner != null) { returner.Dispose(); }
        }
    }
    #endregion

    #region 移除指定的品項
    /// <summary>
    /// 移除指定的品項。
    /// </summary>
    public void RemoveSeledItems()
    {
        foreach (var block in this._blockGoodsItemList)
        {
            if (block.Seled)
            {
                block.Visible = false;
            }
        }

        //已存在的品項暫存
        var existedGoodsItems = new List<string>();
        existedGoodsItems.AddRange(this.hidExistedGoodsItems.Value.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries));

        //重整排序
        int seqNo = 0;
        foreach (var block in this._blockGoodsItemList)
        {
            if (block.Visible)
            {
                block.SeqNo = ++seqNo;

                #region 品項暫存移除
                var itemEditInfo = block.GetInfo();
                //一般品項
                //existedGoodsItems.Remove(itemEditInfo.PartNo);
                //[by fan] 加入型號「型號[#]料號」
                existedGoodsItems.Remove(string.Format("{1}{0}{2}", SystemDefine.JoinSeparator, ConvertLib.ToBase64Encoding(itemEditInfo.Model), itemEditInfo.PartNo));
                #endregion
            }
        }

        //重整已存在的品項暫存
        this.hidExistedGoodsItems.Value = string.Join(",", existedGoodsItems.GroupBy(q => q).Select(q => q.Key).ToArray());

        //計算整張訂單金額
        WebUtilBox.RegisterScript(this.Page, "orderItemHelper.calcOrderAmt();");
    }
    #endregion

    public delegate void AddCompleteEventHandler(object sender, CustEventArgs e);
    public event AddCompleteEventHandler AddComplete;
    void OnAddComplete(object sender, AddCompleteEventHandler e)
    {
        if (this.AddComplete != null)
        {
            this.AddComplete(sender, new CustEventArgs());
        }
    }

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

        var itemEditInfo = ((ASP.include_client_ext_quotn_readjust_goods_item_block_ascx)sender).GetInfo();
        //一般品項
        //existedGoodsItems.Remove(itemEditInfo.PartNo);
        //[by fan] 加入型號「型號[#]料號」
        existedGoodsItems.Remove(string.Format("{1}{0}{2}", SystemDefine.JoinSeparator, ConvertLib.ToBase64Encoding(itemEditInfo.Model), itemEditInfo.PartNo));

        //重整已存在的品項暫存
        this.hidExistedGoodsItems.Value = string.Join(",", existedGoodsItems.GroupBy(q => q).Select(q => q.Key).ToArray());
        #endregion

        //計算整張訂單金額
        WebUtilBox.RegisterScript(this.Page, "orderItemHelper.calcOrderAmt();");
    }
}