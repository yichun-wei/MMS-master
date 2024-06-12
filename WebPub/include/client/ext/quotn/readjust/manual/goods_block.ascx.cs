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

public partial class include_client_ext_quotn_readjust_manual_goods_block : System.Web.UI.UserControl
{
    const string USER_CONTROL_LOAD_SEQ = "UserControlLoadSeq";
    List<ASP.include_client_ext_quotn_readjust_manual_goods_item_block_ascx> _blockGoodsItemList = new List<ASP.include_client_ext_quotn_readjust_manual_goods_item_block_ascx>();

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
                            var blockGoodsItem = (ASP.include_client_ext_quotn_readjust_manual_goods_item_block_ascx)this.LoadControl("~/include/client/ext/quotn/readjust/manual/goods_item_block.ascx");
                            blockGoodsItem.Remove += new include_client_ext_quotn_readjust_manual_goods_item_block.RemoveEventHandler(GoodsItem_OnRemove);
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
            foreach (var itemEditInfo in editInfo.Items)
            {
                var block = (ASP.include_client_ext_quotn_readjust_manual_goods_item_block_ascx)this.LoadControl("~/include/client/ext/quotn/readjust/manual/goods_item_block.ascx");
                block.Remove += new include_client_ext_quotn_readjust_manual_goods_item_block.RemoveEventHandler(GoodsItem_OnRemove);
                this._blockGoodsItemList.Add(block);
                this.phGoodsItemList.Controls.Add(block);
                block.SetInfo(itemEditInfo);
            }
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

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        //var block = (ASP.include_client_ext_quotn_readjust_manual_goods_item_block_ascx)this.LoadControl("~/include/client/ext/quotn/readjust/manual/goods_item_block.ascx");
        //block.Remove += new include_client_ext_quotn_readjust_manual_goods_item_block.RemoveEventHandler(GoodsItem_OnRemove);
        //this._blockGoodsItemList.Add(block);
        //this.phGoodsItemList.Controls.Add(block);

        //var editInfo = new ExtOrderHelper.GoodsItemEditInfo()
        //{
        //    SeqNo = this._blockGoodsItemList.Where(q => q.Visible).Count() //先加入再算序號, 故而不用 + 1.
        //};

        //block.SetInfo(editInfo);

        //ViewState[USER_CONTROL_LOAD_SEQ] += "[GoodsItem]";

        this.Add(new ExtOrderHelper.GoodsItemEditInfo[] { new ExtOrderHelper.GoodsItemEditInfo() });

        //計算整張訂單金額
        WebUtilBox.RegisterScript(this.Page, "orderItemHelper.calcOrderAmt();");
    }

    public void Add(ExtOrderHelper.GoodsItemEditInfo[] editInfos)
    {
        foreach (var editInfo in editInfos)
        {
            var block = (ASP.include_client_ext_quotn_readjust_manual_goods_item_block_ascx)this.LoadControl("~/include/client/ext/quotn/readjust/manual/goods_item_block.ascx");
            block.Remove += new include_client_ext_quotn_readjust_manual_goods_item_block.RemoveEventHandler(GoodsItem_OnRemove);
            this._blockGoodsItemList.Add(block);
            this.phGoodsItemList.Controls.Add(block);

            editInfo.SeqNo = this._blockGoodsItemList.Where(q => q.Visible).Count(); //先加入再算序號, 故而不用 + 1.

            block.SetInfo(editInfo);

            ViewState[USER_CONTROL_LOAD_SEQ] += "[GoodsItem]";
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

        //計算整張訂單金額
        WebUtilBox.RegisterScript(this.Page, "orderItemHelper.calcOrderAmt();");
    }
}