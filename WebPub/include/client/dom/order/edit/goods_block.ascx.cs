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

public partial class include_client_dom_order_edit_goods_block : System.Web.UI.UserControl
{
    const string USER_CONTROL_LOAD_SEQ = "UserControlLoadSeq";
    List<ASP.include_client_dom_order_edit_goods_item_block_ascx> _blockGoodsItemList = new List<ASP.include_client_dom_order_edit_goods_item_block_ascx>();

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
        /// 報價單號碼。
        /// </summary>
        public const string QuoteNumber = "QuoteNumber";
        /// <summary>
        /// 是否為預設區塊。
        /// </summary>
        public const string IsDefault = "IsDefault";
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
                            var blockGoodsItem = (ASP.include_client_dom_order_edit_goods_item_block_ascx)this.LoadControl("~/include/client/dom/order/edit/goods_item_block.ascx");
                            blockGoodsItem.Remove += new include_client_dom_order_edit_goods_item_block.RemoveEventHandler(GoodsItem_OnRemove);
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

    #region 設定資訊
    public void SetInfo(DomOrderHelper.GoodsEditInfo editInfo)
    {
        this.SetInfo(editInfo, false);
    }

    public void SetInfo(DomOrderHelper.GoodsEditInfo editInfo, bool addMode)
    {
        if (editInfo == null)
        {
            this.Visible = false;
            return;
        }

        this.litTitle.Text = editInfo.Title;
        this.SetPgAttr(PgAttrDefine.QuoteNumber, editInfo.QuoteNumber);
        this.hidSubtotal.Value = editInfo.Items.Sum(q => ConvertLib.ToSingle(q.PaidAmt, 0)).ToString();

        if (!string.IsNullOrWhiteSpace(editInfo.QuoteNumber))
        {
            //設定專案報價錨點.
            this.litAnchor.Text = string.Format("<a name='GoodsBlock_{0}'></a>", editInfo.QuoteNumber);
        }

        #region 初始品項清單
        if (editInfo.Items != null && editInfo.Items.Count > 0)
        {
            //已存在的品項暫存
            var existedGoodsItems = new List<string>();

            if (!string.IsNullOrWhiteSpace(this.GetPgAttr(PgAttrDefine.QuoteNumber)))
            {
                //直接來自於專案報價品項暫存的是「報價單明細項次」
                existedGoodsItems.AddRange(editInfo.Items.Where(q => q.PGOrderDetSId == null).Select(q => q.QuoteItemId).ToArray());
                //來自於備貨單的專案報價品項暫存的是「報價單明細項次[#]備貨單明細系統代號」
                existedGoodsItems.AddRange(editInfo.Items.Where(q => q.PGOrderDetSId != null).Select(q => string.Format("{1}{0}{2}", SystemDefine.JoinSeparator, q.QuoteItemId, q.PGOrderDetSId)).ToArray());
            }
            else
            {
                //一般品項暫存的是「料號」
                existedGoodsItems.AddRange(editInfo.Items.Where(q => q.PGOrderDetSId == null).Select(q => q.PartNo).ToArray());
                //來自於備貨單一般品項暫存的是「料號[#]備貨單明細系統代號」
                existedGoodsItems.AddRange(editInfo.Items.Where(q => q.PGOrderDetSId != null).Select(q => string.Format("{1}{0}{2}", SystemDefine.JoinSeparator, q.PartNo, q.PGOrderDetSId)).ToArray());
            }

            foreach (var itemEditInfo in editInfo.Items)
            {
                var block = (ASP.include_client_dom_order_edit_goods_item_block_ascx)this.LoadControl("~/include/client/dom/order/edit/goods_item_block.ascx");
                block.Remove += new include_client_dom_order_edit_goods_item_block.RemoveEventHandler(GoodsItem_OnRemove);
                this._blockGoodsItemList.Add(block);
                this.phGoodsItemList.Controls.Add(block);
                block.SetInfo(itemEditInfo);

                if (addMode)
                {
                    //不是初始載入的, 是選擇專案報價後預設載入全部專案報價品項用的.
                    ViewState[USER_CONTROL_LOAD_SEQ] += "[GoodsItem]";
                }
            }

            this.hidExistedGoodsItems.Value = string.Join(",", existedGoodsItems.GroupBy(q => q).Select(q => q.Key).ToArray());
        }
        #endregion
    }
    #endregion

    #region 取得輸入資訊
    public DomOrderHelper.GoodsEditInfo GetInfo()
    {
        var editInfo = new DomOrderHelper.GoodsEditInfo()
        {
            Title = this.litTitle.Text,
            QuoteNumber = this.GetPgAttr(PgAttrDefine.QuoteNumber)
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

        //倉庫
        var whse = WebUtilBox.FindControl<DropDownList>(this.Page, "lstWhseList").SelectedValue;
        var priceListId = ConvertLib.ToLong(WebUtilBox.FindControl<HiddenField>(this.Page, "hidPriceListId").Value, DefVal.Long);

        Returner returner = null;
        try
        {
            //已存在的品項暫存
            var existedGoodsItems = new List<string>();
            existedGoodsItems.AddRange(this.hidExistedGoodsItems.Value.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries));

            if (!string.IsNullOrWhiteSpace(this.GetPgAttr(PgAttrDefine.QuoteNumber)))
            {
                #region 直接來自於專案報價品項
                do
                {
                    var goodsItems = seledGoodsItems.Where(q => q.IndexOf(SystemDefine.JoinSeparator) == -1).ToArray();
                    if (goodsItems.Length == 0)
                    {
                        break;
                    }

                    ProjQuote entityProjQuote = new ProjQuote(SystemDefine.ConnInfo);

                    var conds = new ProjQuote.InfoViewConds
                        (
                           ConvertLib.ToStrs(this.GetPgAttr(PgAttrDefine.QuoteNumber)),
                           goodsItems,
                           DefVal.Str
                        );

                    SqlOrder sorting = SqlOrder.Default;
                    returner = entityProjQuote.GetInfoView(conds, Int32.MaxValue, SqlOrder.Default);
                    if (returner.IsCompletedAndContinue)
                    {
                        var infos = ProjQuote.InfoView.Binding(returner.DataSet.Tables[0]);

                        //不顯示在手量 (先恢復試速度)
                        //先取得所有品項的在手量
                        //var onHandInfos = ErpHelper.GetOnHandInfo(infos.Where(q => !string.IsNullOrWhiteSpace(q.ProductId)).Select(q => q.ProductId).ToArray(), whse);
                        //改為取得所有倉庫的在手量
                        //先取得所有倉庫
                        var erpWhseInfos = ErpHelper.GetErpWhseInfo(ConvertLib.ToInts(1));
                        var onHandInfos = ErpHelper.GetOnHandInfo(infos.Where(q => !string.IsNullOrWhiteSpace(q.ProductId)).Select(q => q.ProductId).ToArray(), DefVal.Str);

                        foreach (var info in infos)
                        {
                            //已存在的就不再加入
                            if (existedGoodsItems.Contains(info.QuoteItemId))
                            {
                                continue;
                            }

                            //加入已存在的品項暫存
                            existedGoodsItems.Add(info.QuoteItemId);

                            var block = (ASP.include_client_dom_order_edit_goods_item_block_ascx)this.LoadControl("~/include/client/dom/order/edit/goods_item_block.ascx");
                            block.Remove += new include_client_dom_order_edit_goods_item_block.RemoveEventHandler(GoodsItem_OnRemove);
                            this._blockGoodsItemList.Add(block);
                            this.phGoodsItemList.Controls.Add(block);

                            var editInfo = new DomOrderHelper.GoodsItemEditInfo()
                            {
                                SeqNo = this._blockGoodsItemList.Where(q => q.Visible).Count(), //先加入再算序號, 故而不用 + 1.
                                PartNo = info.ProductId,
                                QuoteNumber = info.QuoteNumber,
                                QuoteItemId = info.QuoteItemId,
                                MaxQty = info.Quantity - info.DomOrderUseQty - info.PGOrderUseQty,
                                ListPrice = info.UnitPrice,
                                Discount = info.Discount
                            };

                            //不顯示在手量 (先恢復試速度)
                            //改為取得所有倉庫的在手量
                            //editInfo.ErpOnHand = ErpHelper.GetOnHandQty(onHandInfos, whse, info.ProductId);
                            editInfo.ErpOnHand = onHandInfos.Where(q => q.SubinventoryCode == whse && q.Segment1 == info.ProductId).DefaultIfEmpty(new ErpHelper.OnHandInfo()).SingleOrDefault().OnhandQty;
                            editInfo.ErpWhseOnHands = ErpHelper.GetPerWhseOnHandQty(onHandInfos, erpWhseInfos, info.ProductId);

                            block.SetInfo(editInfo);

                            ViewState[USER_CONTROL_LOAD_SEQ] += "[GoodsItem]";
                        }
                    }
                } while (false);
                #endregion

                #region 來自於備貨單的專案報價品項
                do
                {
                    var goodsItems = seledGoodsItems.Where(q => q.IndexOf(SystemDefine.JoinSeparator) != -1).ToArray();
                    if (goodsItems.Length == 0)
                    {
                        break;
                    }

                    var pgOrderDetSIds = ConvertLib.ToSIds(goodsItems.Select(q => ConvertLib.ToSId(q.Split(new string[] { SystemDefine.JoinSeparator }, StringSplitOptions.None)[1])).ToArray());
                    if (pgOrderDetSIds.Length == 0)
                    {
                        break;
                    }

                    PGOrderDet entityPGOrderDet = new PGOrderDet(SystemDefine.ConnInfo);

                    //為了效能, 依「備貨單明細系統代號」直接取出, 驗證待儲存時再處理.
                    var conds = new PGOrderDet.InfoViewConds
                        (
                           pgOrderDetSIds,
                           DefVal.SId,
                           DefVal.Str,
                           DefVal.Int,
                           DefVal.Str,
                           DefVal.Str,
                           DefVal.Str,
                           DefVal.Long,
                           DefVal.Bool,
                           false,
                           IncludeScope.All
                        );

                    SqlOrder sorting = SqlOrder.Default;
                    returner = entityPGOrderDet.GetInfoView(conds, Int32.MaxValue, SqlOrder.Default);
                    if (returner.IsCompletedAndContinue)
                    {
                        var infos = PGOrderDet.InfoView.Binding(returner.DataSet.Tables[0]);

                        //不顯示在手量 (先恢復試速度)
                        //先取得所有品項的在手量
                        //var onHandInfos = ErpHelper.GetOnHandInfo(infos.Where(q => !string.IsNullOrWhiteSpace(q.PartNo)).Select(q => q.PartNo).ToArray(), whse);
                        //改為取得所有倉庫的在手量
                        var erpWhseInfos = ErpHelper.GetErpWhseInfo(ConvertLib.ToInts(1));
                        var onHandInfos = ErpHelper.GetOnHandInfo(infos.Where(q => !string.IsNullOrWhiteSpace(q.PartNo)).Select(q => q.PartNo).ToArray(), DefVal.Str);

                        foreach (var info in infos)
                        {
                            //已存在的就不再加入
                            if (existedGoodsItems.Contains(string.Format("{1}{0}{2}", SystemDefine.JoinSeparator, info.QuoteItemId, info.SId)))
                            {
                                continue;
                            }

                            //加入已存在的品項暫存
                            existedGoodsItems.Add(string.Format("{1}{0}{2}", SystemDefine.JoinSeparator, info.QuoteItemId, info.SId));

                            var block = (ASP.include_client_dom_order_edit_goods_item_block_ascx)this.LoadControl("~/include/client/dom/order/edit/goods_item_block.ascx");
                            block.Remove += new include_client_dom_order_edit_goods_item_block.RemoveEventHandler(GoodsItem_OnRemove);
                            this._blockGoodsItemList.Add(block);
                            this.phGoodsItemList.Controls.Add(block);

                            var editInfo = new DomOrderHelper.GoodsItemEditInfo()
                            {
                                SeqNo = this._blockGoodsItemList.Where(q => q.Visible).Count(), //先加入再算序號, 故而不用 + 1.
                                PartNo = info.PartNo,
                                PGOrderOdrNo = info.PGOrderOdrNo,
                                PGOrderDetSId = info.SId,
                                QuoteNumber = info.QuoteNumber,
                                QuoteItemId = info.QuoteItemId,
                                MaxQty = info.Qty - info.DomOrderUseQty,
                                ListPrice = info.ProjQuoteUnitPrice,
                                Discount = info.Discount
                            };

                            //不顯示在手量 (先恢復試速度)
                            //改為取得所有倉庫的在手量
                            //editInfo.ErpOnHand = ErpHelper.GetOnHandQty(onHandInfos, whse, info.PartNo);
                            editInfo.ErpOnHand = onHandInfos.Where(q => q.SubinventoryCode == whse && q.Segment1 == info.PartNo).DefaultIfEmpty(new ErpHelper.OnHandInfo()).SingleOrDefault().OnhandQty;
                            editInfo.ErpWhseOnHands = ErpHelper.GetPerWhseOnHandQty(onHandInfos, erpWhseInfos, info.PartNo);

                            block.SetInfo(editInfo);

                            ViewState[USER_CONTROL_LOAD_SEQ] += "[GoodsItem]";
                        }
                    }
                } while (false);
                #endregion
            }
            else
            {
                #region 一般品項
                do
                {
                    var goodsItems = seledGoodsItems.Where(q => q.IndexOf(SystemDefine.JoinSeparator) == -1).ToArray();
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

                        //不顯示在手量 (先恢復試速度)
                        //先取得所有品項的在手量
                        //var onHandInfos = ErpHelper.GetOnHandInfo(infos.Where(q => !string.IsNullOrWhiteSpace(q.Item)).Select(q => q.Item).ToArray(), whse);
                        //改為取得所有倉庫的在手量
                        //先取得所有倉庫
                        var erpWhseInfos = ErpHelper.GetErpWhseInfo(ConvertLib.ToInts(1));
                        var onHandInfos = ErpHelper.GetOnHandInfo(infos.Where(q => !string.IsNullOrWhiteSpace(q.Item)).Select(q => q.Item).ToArray(), DefVal.Str);

                        //先取得所有品項的價目表
                        var priceBookInfos = new ErpHelper.PriceBookInfo[0];
                        if (priceListId.HasValue)
                        {
                            priceBookInfos = ErpHelper.GetPriceBookInfo(priceListId.Value, infos.Select(q => q.Item).ToArray());
                        }

                        foreach (var info in infos)
                        {
                            //已存在的就不再加入
                            if (existedGoodsItems.Contains(info.Item))
                            {
                                continue;
                            }

                            //加入已存在的品項暫存
                            existedGoodsItems.Add(info.Item);

                            var block = (ASP.include_client_dom_order_edit_goods_item_block_ascx)this.LoadControl("~/include/client/dom/order/edit/goods_item_block.ascx");
                            block.Remove += new include_client_dom_order_edit_goods_item_block.RemoveEventHandler(GoodsItem_OnRemove);
                            this._blockGoodsItemList.Add(block);
                            this.phGoodsItemList.Controls.Add(block);

                            var editInfo = new DomOrderHelper.GoodsItemEditInfo()
                            {
                                SeqNo = this._blockGoodsItemList.Where(q => q.Visible).Count(), //先加入再算序號, 故而不用 + 1.
                                PartNo = info.Item,
                                DefDct = info.Discount,
                                Discount = ConvertLib.ToSingle(info.Discount, 100) / 100
                            };

                            //不顯示在手量 (先恢復試速度)
                            //改為取得所有倉庫的在手量
                            //editInfo.ErpOnHand = ErpHelper.GetOnHandQty(onHandInfos, whse, info.Item);
                            editInfo.ErpOnHand = onHandInfos.Where(q => q.SubinventoryCode == whse && q.Segment1 == info.Item).DefaultIfEmpty(new ErpHelper.OnHandInfo()).SingleOrDefault().OnhandQty;
                            editInfo.ErpWhseOnHands = ErpHelper.GetPerWhseOnHandQty(onHandInfos, erpWhseInfos, info.Item);

                            editInfo.ListPrice = ErpHelper.GetPriceBookListPrice(priceBookInfos, info.Item);

                            block.SetInfo(editInfo);

                            ViewState[USER_CONTROL_LOAD_SEQ] += "[GoodsItem]";
                        }
                    }
                } while (false);
                #endregion

                #region 來自於備貨單一般品項
                do
                {
                    var goodsItems = seledGoodsItems.Where(q => q.IndexOf(SystemDefine.JoinSeparator) != -1).ToArray();
                    if (goodsItems.Length == 0)
                    {
                        break;
                    }

                    var pgOrderDetSIds = ConvertLib.ToSIds(goodsItems.Select(q => ConvertLib.ToSId(q.Split(new string[] { SystemDefine.JoinSeparator }, StringSplitOptions.None)[1])).ToArray());
                    if (pgOrderDetSIds.Length == 0)
                    {
                        break;
                    }

                    PGOrderDet entityPGOrderDet = new PGOrderDet(SystemDefine.ConnInfo);

                    //為了效能, 依「備貨單明細系統代號」直接取出, 驗證待儲存時再處理.
                    var conds = new PGOrderDet.InfoViewConds
                        (
                           pgOrderDetSIds,
                           DefVal.SId,
                           DefVal.Str,
                           DefVal.Int,
                           DefVal.Str,
                           DefVal.Str,
                           DefVal.Str,
                           DefVal.Long,
                           DefVal.Bool,
                           false,
                           IncludeScope.All
                        );

                    SqlOrder sorting = SqlOrder.Default;
                    returner = entityPGOrderDet.GetInfoView(conds, Int32.MaxValue, SqlOrder.Default);
                    if (returner.IsCompletedAndContinue)
                    {
                        var infos = PGOrderDet.InfoView.Binding(returner.DataSet.Tables[0]);

                        //不顯示在手量 (先恢復試速度)
                        //先取得所有品項的在手量
                        //var onHandInfos = ErpHelper.GetOnHandInfo(infos.Where(q => !string.IsNullOrWhiteSpace(q.PartNo)).Select(q => q.PartNo).ToArray(), whse);
                        //改為取得所有倉庫的在手量
                        //先取得所有倉庫
                        var erpWhseInfos = ErpHelper.GetErpWhseInfo(ConvertLib.ToInts(1));
                        var onHandInfos = ErpHelper.GetOnHandInfo(infos.Where(q => !string.IsNullOrWhiteSpace(q.PartNo)).Select(q => q.PartNo).ToArray(), DefVal.Str);

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

                            var block = (ASP.include_client_dom_order_edit_goods_item_block_ascx)this.LoadControl("~/include/client/dom/order/edit/goods_item_block.ascx");
                            block.Remove += new include_client_dom_order_edit_goods_item_block.RemoveEventHandler(GoodsItem_OnRemove);
                            this._blockGoodsItemList.Add(block);
                            this.phGoodsItemList.Controls.Add(block);

                            var editInfo = new DomOrderHelper.GoodsItemEditInfo()
                            {
                                SeqNo = this._blockGoodsItemList.Where(q => q.Visible).Count(), //先加入再算序號, 故而不用 + 1.
                                PartNo = info.PartNo,
                                PGOrderOdrNo = info.PGOrderOdrNo,
                                PGOrderDetSId = info.SId,
                                MaxQty = info.Qty - info.DomOrderUseQty,
                                DefDct = info.DefDct,
                                Discount = ConvertLib.ToSingle(info.Discount, 100) / 100
                            };

                            //不顯示在手量 (先恢復試速度)
                            //改為取得所有倉庫的在手量
                            //editInfo.ErpOnHand = ErpHelper.GetOnHandQty(onHandInfos, whse, info.PartNo);
                            editInfo.ErpOnHand = onHandInfos.Where(q => q.SubinventoryCode == whse && q.Segment1 == info.PartNo).DefaultIfEmpty(new ErpHelper.OnHandInfo()).SingleOrDefault().OnhandQty;
                            editInfo.ErpWhseOnHands = ErpHelper.GetPerWhseOnHandQty(onHandInfos, erpWhseInfos, info.PartNo);

                            editInfo.ListPrice = ErpHelper.GetPriceBookListPrice(priceBookInfos, info.PartNo);

                            block.SetInfo(editInfo);

                            ViewState[USER_CONTROL_LOAD_SEQ] += "[GoodsItem]";
                        }
                    }
                } while (false);
                #endregion
            }

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

    /// <summary>
    /// 是否為預設區塊。
    /// </summary>
    public bool IsDefault
    {
        get { return "Y".Equals(this.GetPgAttr(PgAttrDefine.IsDefault)); }
        set { this.SetPgAttr(PgAttrDefine.IsDefault, value ? "Y" : "N"); }
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

        var itemEditInfo = ((ASP.include_client_dom_order_edit_goods_item_block_ascx)sender).GetInfo();
        if (!string.IsNullOrWhiteSpace(itemEditInfo.QuoteNumber))
        {
            if (itemEditInfo.PGOrderDetSId == null)
            {
                //直接來自於專案報價品項
                existedGoodsItems.Remove(itemEditInfo.QuoteItemId);
            }
            else
            {
                //來自於備貨單的專案報價品項
                existedGoodsItems.Remove(string.Format("{1}{0}{2}", SystemDefine.JoinSeparator, itemEditInfo.QuoteItemId, itemEditInfo.PGOrderDetSId));
            }
        }
        else
        {
            if (itemEditInfo.PGOrderDetSId == null)
            {
                //一般品項
                existedGoodsItems.Remove(itemEditInfo.PartNo);
            }
            else
            {
                //來自於備貨單一般品項
                existedGoodsItems.Remove(string.Format("{1}{0}{2}", SystemDefine.JoinSeparator, itemEditInfo.PartNo, itemEditInfo.PGOrderDetSId));
            }
        }

        //重整已存在的品項暫存
        this.hidExistedGoodsItems.Value = string.Join(",", existedGoodsItems.GroupBy(q => q).Select(q => q.Key).ToArray());
        #endregion

        if (seqNo == 0 && !this.IsDefault)
        {
            //若已沒有任何項目, 則移除.
            WebUtilBox.RegisterScript(this.Page, string.Format("projQuoteHelper.removeExistedProjQuotes('{0}');", this.GetPgAttr(PgAttrDefine.QuoteNumber)));
            this.Visible = false;
        }

        //計算整張訂單金額
        WebUtilBox.RegisterScript(this.Page, "orderItemHelper.calcOrderAmt();");
    }
}