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
using Seec.Marketing.NetTalk.WebService.Client;

public partial class include_client_dom_pg_order_edit_goods_block : System.Web.UI.UserControl
{
    const string USER_CONTROL_LOAD_SEQ = "UserControlLoadSeq";
    List<ASP.include_client_dom_pg_order_edit_goods_item_block_ascx> _blockGoodsItemList = new List<ASP.include_client_dom_pg_order_edit_goods_item_block_ascx>();

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
                            var blockGoodsItem = (ASP.include_client_dom_pg_order_edit_goods_item_block_ascx)this.LoadControl("~/include/client/dom/pg_order/edit/goods_item_block.ascx");
                            blockGoodsItem.Remove += new include_client_dom_pg_order_edit_goods_item_block.RemoveEventHandler(GoodsItem_OnRemove);
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
        this.divGoodsBlock.CssClass = string.Format("dev-goods-block-{0}", this.GetPgAttr(PgAttrDefine.SId));
        WebUtilBox.AddAttribute(this.lnkAdd, "onclick", string.Format("orderItemHelper.openWindow('{0}');", this.GetPgAttr(PgAttrDefine.SId)));
    }

    #region 設定資訊
    public void SetInfo(PGOrderHelper.GoodsEditInfo editInfo)
    {
        if (editInfo == null)
        {
            this.Visible = false;
            return;
        }

        this.litTitle.Text = editInfo.Title;
        this.SetPgAttr(PgAttrDefine.QuoteNumber, editInfo.QuoteNumber);

        if (!string.IsNullOrWhiteSpace(editInfo.QuoteNumber))
        {
            //設定專案報價錨點.
            this.litAnchor.Text = string.Format("<a name='GoodsBlock_{0}'></a>", editInfo.QuoteNumber);
            //顯示「最大值」
            this.phMaxQty.Visible = true;
        }
    }
    #endregion

    #region 取得輸入資訊
    public PGOrderHelper.GoodsEditInfo GetInfo()
    {
        var editInfo = new PGOrderHelper.GoodsEditInfo()
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

    #region 新增區塊
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        var seledGoodsItems = WebUtilBox.FindControl<HiddenField>(this.Page, "hidSeledGoodsItems").Value.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);
        this.Add(seledGoodsItems);
    }

    public void Add(string[] seledGoodsItems)
    {
        if (seledGoodsItems.Length == 0)
        {
            return;
        }

        Returner returner = null;
        try
        {
            //已存在的品項暫存
            var existedGoodsItems = new List<string>();
            existedGoodsItems.AddRange(this.hidExistedGoodsItems.Value.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries));

            if (!string.IsNullOrWhiteSpace(this.GetPgAttr(PgAttrDefine.QuoteNumber)))
            {
                #region 專案報價
                ProjQuote entityProjQuote = new ProjQuote(SystemDefine.ConnInfo);

                var conds = new ProjQuote.InfoViewConds
                    (
                       ConvertLib.ToStrs(this.GetPgAttr(PgAttrDefine.QuoteNumber)),
                       seledGoodsItems,
                       DefVal.Str
                    );

                SqlOrder sorting = SqlOrder.Default;
                returner = entityProjQuote.GetInfoView(conds, Int32.MaxValue, SqlOrder.Default);
                if (returner.IsCompletedAndContinue)
                {
                    var infos = ProjQuote.InfoView.Binding(returner.DataSet.Tables[0]);

                    foreach (var info in infos)
                    {
                        //已存在的就不再加入
                        if (existedGoodsItems.Contains(info.QuoteItemId))
                        {
                            continue;
                        }

                        //加入已存在的品項暫存
                        existedGoodsItems.Add(info.QuoteItemId);

                        var block = (ASP.include_client_dom_pg_order_edit_goods_item_block_ascx)this.LoadControl("~/include/client/dom/pg_order/edit/goods_item_block.ascx");
                        block.Remove += new include_client_dom_pg_order_edit_goods_item_block.RemoveEventHandler(GoodsItem_OnRemove);
                        this._blockGoodsItemList.Add(block);
                        this.phGoodsItemList.Controls.Add(block);

                        int seqNo;
                        if (!this.IsPostBack)
                        {
                            //第一次載入時, 取總數.
                            seqNo = this._blockGoodsItemList.Count;
                        }
                        else
                        {
                            //post back 因先加入再算序號, 故而不用 + 1.
                            seqNo = this._blockGoodsItemList.Where(q => q.Visible).Count();
                        }

                        var editInfo = new PGOrderHelper.GoodsItemEditInfo()
                        {
                            SeqNo = seqNo,
                            PartNo = info.ProductId,
                            Summary = info.Summary,
                            QuoteNumber = info.QuoteNumber,
                            QuoteItemId = info.QuoteItemId,
                            MaxQty = info.Quantity - info.DomOrderUseQty - info.PGOrderUseQty
                        };

                        block.SetInfo(editInfo);

                        ViewState[USER_CONTROL_LOAD_SEQ] += "[GoodsItem]";
                    }
                }
                #endregion
            }
            else
            {
                #region 一般品項
                ErpInv entityErpInv = new ErpInv(SystemDefine.ConnInfo);

                var conds = new ErpInv.InfoConds
                     (
                        DefVal.SIds,
                        DefVal.Longs,
                        seledGoodsItems,
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

                    foreach (var info in infos)
                    {
                        //已存在的就不再加入
                        if (existedGoodsItems.Contains(info.Item))
                        {
                            continue;
                        }

                        //加入已存在的品項暫存
                        existedGoodsItems.Add(info.Item);

                        var block = (ASP.include_client_dom_pg_order_edit_goods_item_block_ascx)this.LoadControl("~/include/client/dom/pg_order/edit/goods_item_block.ascx");
                        block.Remove += new include_client_dom_pg_order_edit_goods_item_block.RemoveEventHandler(GoodsItem_OnRemove);
                        this._blockGoodsItemList.Add(block);
                        this.phGoodsItemList.Controls.Add(block);

                        int seqNo;
                        if (!this.IsPostBack)
                        {
                            //第一次載入時, 取總數.
                            seqNo = this._blockGoodsItemList.Count;
                        }
                        else
                        {
                            //post back 因先加入再算序號, 故而不用 + 1.
                            seqNo = this._blockGoodsItemList.Where(q => q.Visible).Count();
                        }

                        var editInfo = new PGOrderHelper.GoodsItemEditInfo()
                        {
                            SeqNo = seqNo,
                            PartNo = info.Item,
                            Summary = info.Description
                        };

                        block.SetInfo(editInfo);

                        ViewState[USER_CONTROL_LOAD_SEQ] += "[GoodsItem]";
                    }
                }
                #endregion
            }

            //重整已存在的品項暫存
            this.hidExistedGoodsItems.Value = string.Join(",", existedGoodsItems.GroupBy(q => q).Select(q => q.Key).ToArray());
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

        var itemEditInfo = ((ASP.include_client_dom_pg_order_edit_goods_item_block_ascx)sender).GetInfo();
        if (!string.IsNullOrWhiteSpace(itemEditInfo.QuoteNumber))
        {
            //專案報價品項
            existedGoodsItems.Remove(itemEditInfo.QuoteItemId);
        }
        else
        {
            //一般品項
            existedGoodsItems.Remove(itemEditInfo.PartNo);
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
    }
}