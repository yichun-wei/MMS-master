using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;

using EzCoding;
using EzCoding.Collections;
using EzCoding.Web.UI;
using EzCoding.DB;
using Seec.Marketing;
using Seec.Marketing.SystemEngines;

public partial class popup_ext_goods_2 : System.Web.UI.Page
{
    #region 網頁屬性
    /// <summary>
    /// 主版。
    /// </summary>
    IClientPopupMaster MainPage { get; set; }
    /// <summary>
    /// 網頁的控制操作。
    /// </summary>
    WebPg WebPg { get; set; }
    /// <summary>
    /// 網頁標題。
    /// </summary>
    string PageTitle { get; set; }
    /// <summary>
    /// 是否重新執行查詢動作。
    /// </summary>
    bool DoSearch { get; set; }
    /// <summary>
    /// 網頁是否已完成初始。
    /// </summary>
    bool HasInitial { get; set; }
    #endregion

    public bool IsSingleMode { get; set; }
    public ISystemId ContainerSId { get; set; }

    #region 網頁初始的一連貫操作
    protected void Page_Init(object sender, EventArgs e)
    {
        if (!this.InitVital()) { return; }
        if (!this.InitPage()) { return; }
        if (!this.LoadIncludePage()) { return; }

        this.HasInitial = true;
    }

    #region 初始化的必要操作
    /// <summary>
    /// 初始化的必要操作。
    /// </summary>
    private bool InitVital()
    {
        this.WebPg = new WebPg(this, true, OperPosition.GeneralClient);
        this.MainPage = (IClientPopupMaster)this.Master;
        if (this.MainPage.ActorInfoSet == null)
        {
            return false;
        }

        this.IsSingleMode = ConvertLib.ToBoolean(Request.QueryString["single"]);
        this.ContainerSId = ConvertLib.ToSId(Request.QueryString["cntrSId"]);

        if (this.ContainerSId == null)
        {
            JSBuilder.ClosePage(this);
            return false;
        }

        return true;
    }
    #endregion

    #region 初始化頁面
    /// <summary>
    /// 初始化頁面。
    /// </summary>
    private bool InitPage()
    {
        this.observerPaging.Register(this.firstPaging, this.prevPaging, this.numberPaging, this.nextPaging, this.lastPaging, this.textboxPaging);

        this.PageTitle = "品項清單";

        Returner returner = null;
        try
        {
            if (!this.IsPostBack)
            {
                var extItemCatConds = ExtItemHelper.GetExtItemCatConds(true);
                foreach (var info in extItemCatConds.ExtItemTypes)
                {
                    this.lstItemType.Items.Add(new ListItem(info.ExportItemType, info.ExportItemType));
                }

                this.phSingleModeJS.Visible = this.IsSingleMode;
                this.phMultiModeJS.Visible = !this.IsSingleMode;
                this.phMultiAdd.Visible = !this.IsSingleMode;
            }
        }
        finally
        {
            if (returner != null) { returner.Dispose(); }
        }

        return true;
    }
    #endregion

    #region 載入使用者控制項
    /// <summary>
    /// 載入使用者控制項。
    /// </summary>
    private bool LoadIncludePage()
    {
        return true;
    }
    #endregion
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        //網頁抬頭
        this.WebPg.SetClientPageTitle(this.PageTitle);

        if (!this.IsPostBack)
        {
            //第一次載入時, 要先從 opener 取得已存在的品項, 載入後品得後再建立列表.
            WebPg.RegisterScript("initPageInfo();");
        }
        else
        {
            var scriptManager = ScriptManager.GetCurrent(this);
            if (!scriptManager.IsInAsyncPostBack)
            {
                this.ExtItemDetailsList();
            }
        }
    }

    protected void btnInitPageInfo_Click(object sender, EventArgs e)
    {

    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        this.Page_DoSearch(null, EventArgs.Empty);
    }

    protected void Page_DoSearch(object sender, EventArgs e)
    {
        this.DoSearch = true;
        this.hidSearchConds.Value = string.Empty;
    }

    #region 取得外銷型號分類資訊 - 已註解
    ///// <summary>
    ///// 取得外銷型號分類資訊。
    ///// </summary>
    ///// <param name="renew">是否重新取得。</param>
    //ExtItemType.Info[] GetExtItemTypeInfos(bool renew)
    //{
    //    /********************************************************************************
    //     * 避免每次都重取資料庫, 使用快取, 記得使用者的 Session.
    //     * 不放在 Application 的原因為不知士電何時更新, 不能一直快取, 沒有觸發點知道士電更新資料庫而清空快取.
    //     * 故而每次第一次開啟查詢頁時, 皆重新取得.
    //    ********************************************************************************/
    //    ExtItemType.Info[] extItemTypeInfos;

    //    extItemTypeInfos = Session[SessionDefine.ExtItemCatConds] as ExtItemType.Info[];
    //    if (extItemTypeInfos == null || !renew)
    //    {
    //        if (extItemTypeInfos != null)
    //        {
    //            return extItemTypeInfos;
    //        }
    //    }

    //    Returner returner = null;
    //    try
    //    {
    //        ExtItemType entityExtItemType = new ExtItemType(SystemDefine.ConnInfo);
    //        returner = entityExtItemType.GetInfo(new ExtItemType.InfoConds(DefVal.Strs, true), Int32.MaxValue, SqlOrder.Default, ConvertLib.ToStrs("EXPORT_ITEM_TYPE", "CATEGORY_DESC1", "CATEGORY_DESC2", "CATEGORY_DESC3", "CATEGORY_DESC4", "CATEGORY_DESC5"));
    //        if (returner.IsCompletedAndContinue)
    //        {
    //            extItemTypeInfos = ExtItemType.Info.Binding(returner.DataSet.Tables[0]);
    //        }
    //        else
    //        {
    //            extItemTypeInfos = new ExtItemType.Info[0];
    //        }

    //        Session[SessionDefine.ExtItemCatConds] = extItemTypeInfos;
    //        return extItemTypeInfos;
    //    }
    //    finally
    //    {
    //        if (returner != null) { returner.Dispose(); }
    //    }
    //}
    #endregion

    #region 分類切換
    protected void lstItemType_SelectedIndexChanged(object sender, EventArgs e)
    {
        var seled = this.lstItemType.SelectedValue;
        IExtItemCats[] filterCats = null;

        this.lstCat_1.Items.Clear();
        this.lstCat_2.Items.Clear();
        this.lstCat_3.Items.Clear();
        this.lstCat_4.Items.Clear();
        this.lstCat_5.Items.Clear();

        if (!string.IsNullOrWhiteSpace(seled))
        {
            var extItemCatConds = ExtItemHelper.GetExtItemCatConds(false);
            filterCats = extItemCatConds.ExtItemCats.Where(q => seled.Equals(q.ExportItemType)).ToArray();

            #region 重置分類
            var extItemTypeInfo = extItemCatConds.ExtItemTypes.Where(q => seled.Equals(q.ExportItemType)).Single();

            this.lstCat_1.Visible = !string.IsNullOrWhiteSpace(extItemTypeInfo.CategoryDesc1);
            if (this.lstCat_1.Visible)
            {
                this.lstCat_1.Items.Add(new ListItem(string.Format("[{0}]", extItemTypeInfo.CategoryDesc1), string.Empty));
            }

            this.lstCat_2.Visible = !string.IsNullOrWhiteSpace(extItemTypeInfo.CategoryDesc2);
            if (this.lstCat_2.Visible)
            {
                this.lstCat_2.Items.Add(new ListItem(string.Format("[{0}]", extItemTypeInfo.CategoryDesc2), string.Empty));
            }

            this.lstCat_3.Visible = !string.IsNullOrWhiteSpace(extItemTypeInfo.CategoryDesc3);
            if (this.lstCat_3.Visible)
            {
                this.lstCat_3.Items.Add(new ListItem(string.Format("[{0}]", extItemTypeInfo.CategoryDesc3), string.Empty));
            }

            this.lstCat_4.Visible = !string.IsNullOrWhiteSpace(extItemTypeInfo.CategoryDesc4);
            if (this.lstCat_4.Visible)
            {
                this.lstCat_4.Items.Add(new ListItem(string.Format("[{0}]", extItemTypeInfo.CategoryDesc4), string.Empty));
            }

            this.lstCat_5.Visible = !string.IsNullOrWhiteSpace(extItemTypeInfo.CategoryDesc5);
            if (this.lstCat_5.Visible)
            {
                this.lstCat_5.Items.Add(new ListItem(string.Format("[{0}]", extItemTypeInfo.CategoryDesc5), string.Empty));
            }
            #endregion

            this.ResetCat_1_List(filterCats, DefVal.Str);
        }
    }

    protected void lstCat_1_SelectedIndexChanged(object sender, EventArgs e)
    {
        var seleds = ConvertLib.ToStrs(this.lstItemType.SelectedValue, this.lstCat_1.SelectedValue);
        IExtItemCats[] filterCats = null;

        if (seleds.Length == 2)
        {
            var extItemCatConds = ExtItemHelper.GetExtItemCatConds(false);
            filterCats = extItemCatConds.ExtItemCats.Where(q => seleds[0].Equals(q.ExportItemType) && seleds[1].Equals(q.Category1)).ToArray();
        }

        this.ResetCat_2_List(filterCats, DefVal.Str);
    }

    #region ResetCat_1_List
    IExtItemCats[] ResetCat_1_List(IExtItemCats[] baseCats, string specSeled)
    {
        IExtItemCats[] filterCats = null;
        var curCat = this.lstCat_1;
        //是否顯示在切換產品別即已決定
        if (!curCat.Visible) { return filterCats; }
        //若已賦值, 則保留提示項目外, 其他清空.
        if (curCat.Items.Count > 1)
        {
            var promptItem = new ListItem(curCat.Items[0].Text, curCat.Items[0].Value);
            curCat.Items.Clear();
            curCat.Items.Add(promptItem);
        }

        if (baseCats != null && baseCats.Length > 0)
        {
            var grpExtItemCats = baseCats.Where(q => !string.IsNullOrWhiteSpace(q.Category1)).GroupBy(q => q.Category1).Select(q => new { Key = q.Key, List = q.ToArray() });
            foreach (var grpInfo in grpExtItemCats)
            {
                //分類值改成允許空字串
                //var item = new ListItem(grpInfo.Key, grpInfo.Key);
                var item = new ListItem(grpInfo.Key.Replace(ExtItemHelper.EMPTY_ITEM_VALUE, "(空值)"), grpInfo.Key);
                item.Selected = !string.IsNullOrWhiteSpace(specSeled) && specSeled.Equals(grpInfo.Key);
                curCat.Items.Add(item);

                if (item.Selected)
                {
                    filterCats = grpInfo.List;
                }
            }
        }

        //若有指定選擇值, 則由叫用端自行控制.
        if (string.IsNullOrWhiteSpace(specSeled))
        {
            this.ResetCat_2_List(filterCats, DefVal.Str);
        }

        return filterCats;
    }
    #endregion

    protected void lstCat_2_SelectedIndexChanged(object sender, EventArgs e)
    {
        var seleds = ConvertLib.ToStrs(this.lstItemType.SelectedValue, this.lstCat_1.SelectedValue, this.lstCat_2.SelectedValue);
        IExtItemCats[] filterCats = null;

        if (seleds.Length == 3)
        {
            var extItemCatConds = ExtItemHelper.GetExtItemCatConds(false);
            filterCats = extItemCatConds.ExtItemCats.Where(q => seleds[0].Equals(q.ExportItemType) && seleds[1].Equals(q.Category1) && seleds[2].Equals(q.Category2)).ToArray();
        }

        this.ResetCat_3_List(filterCats, DefVal.Str);
    }

    #region ResetCat_2_List
    IExtItemCats[] ResetCat_2_List(IExtItemCats[] baseCats, string specSeled)
    {
        IExtItemCats[] filterCats = null;
        var curCat = this.lstCat_2;
        //是否顯示在切換產品別即已決定
        if (!curCat.Visible) { return filterCats; }
        //若已賦值, 則保留提示項目外, 其他清空.
        if (curCat.Items.Count > 1)
        {
            var promptItem = new ListItem(curCat.Items[0].Text, curCat.Items[0].Value);
            curCat.Items.Clear();
            curCat.Items.Add(promptItem);
        }

        if (baseCats != null && baseCats.Length > 0)
        {
            var grpExtItemCats = baseCats.Where(q => !string.IsNullOrWhiteSpace(q.Category2)).GroupBy(q => q.Category2).Select(q => new { Key = q.Key, List = q.ToArray() });
            foreach (var grpInfo in grpExtItemCats)
            {
                //分類值改成允許空字串
                //var item = new ListItem(grpInfo.Key, grpInfo.Key);
                var item = new ListItem(grpInfo.Key.Replace(ExtItemHelper.EMPTY_ITEM_VALUE, "(空值)"), grpInfo.Key);
                item.Selected = !string.IsNullOrWhiteSpace(specSeled) && specSeled.Equals(grpInfo.Key);
                curCat.Items.Add(item);

                if (item.Selected)
                {
                    filterCats = grpInfo.List;
                }
            }
        }

        //若有指定選擇值, 則由叫用端自行控制.
        if (string.IsNullOrWhiteSpace(specSeled))
        {
            this.ResetCat_3_List(filterCats, DefVal.Str);
        }

        return filterCats;
    }
    #endregion

    protected void lstCat_3_SelectedIndexChanged(object sender, EventArgs e)
    {
        var seleds = ConvertLib.ToStrs(this.lstItemType.SelectedValue, this.lstCat_1.SelectedValue, this.lstCat_2.SelectedValue, this.lstCat_3.SelectedValue);
        IExtItemCats[] filterCats = null;

        if (seleds.Length == 4)
        {
            var extItemCatConds = ExtItemHelper.GetExtItemCatConds(false);
            filterCats = extItemCatConds.ExtItemCats.Where(q => seleds[0].Equals(q.ExportItemType) && seleds[1].Equals(q.Category1) && seleds[2].Equals(q.Category2) && seleds[3].Equals(q.Category3)).ToArray();
        }

        this.ResetCat_4_List(filterCats, DefVal.Str);
    }

    #region ResetCat_3_List
    IExtItemCats[] ResetCat_3_List(IExtItemCats[] baseCats, string specSeled)
    {
        IExtItemCats[] filterCats = null;
        var curCat = this.lstCat_3;
        //是否顯示在切換產品別即已決定
        if (!curCat.Visible) { return filterCats; }
        //若已賦值, 則保留提示項目外, 其他清空.
        if (curCat.Items.Count > 1)
        {
            var promptItem = new ListItem(curCat.Items[0].Text, curCat.Items[0].Value);
            curCat.Items.Clear();
            curCat.Items.Add(promptItem);
        }

        if (baseCats != null && baseCats.Length > 0)
        {
            var grpExtItemCats = baseCats.Where(q => !string.IsNullOrWhiteSpace(q.Category3)).GroupBy(q => q.Category3).Select(q => new { Key = q.Key, List = q.ToArray() });
            foreach (var grpInfo in grpExtItemCats)
            {
                //分類值改成允許空字串
                //var item = new ListItem(grpInfo.Key, grpInfo.Key);
                var item = new ListItem(grpInfo.Key.Replace(ExtItemHelper.EMPTY_ITEM_VALUE, "(空值)"), grpInfo.Key);
                item.Selected = !string.IsNullOrWhiteSpace(specSeled) && specSeled.Equals(grpInfo.Key);
                curCat.Items.Add(item);

                if (item.Selected)
                {
                    filterCats = grpInfo.List;
                }
            }
        }

        //若有指定選擇值, 則由叫用端自行控制.
        if (string.IsNullOrWhiteSpace(specSeled))
        {
            this.ResetCat_4_List(filterCats, DefVal.Str);
        }

        return filterCats;
    }
    #endregion

    protected void lstCat_4_SelectedIndexChanged(object sender, EventArgs e)
    {
        var seleds = ConvertLib.ToStrs(this.lstItemType.SelectedValue, this.lstCat_1.SelectedValue, this.lstCat_2.SelectedValue, this.lstCat_3.SelectedValue, this.lstCat_4.SelectedValue);
        IExtItemCats[] filterCats = null;

        if (seleds.Length == 5)
        {
            var extItemCatConds = ExtItemHelper.GetExtItemCatConds(false);
            filterCats = extItemCatConds.ExtItemCats.Where(q => seleds[0].Equals(q.ExportItemType) && seleds[1].Equals(q.Category1) && seleds[2].Equals(q.Category2) && seleds[3].Equals(q.Category3) && seleds[4].Equals(q.Category4)).ToArray();
        }

        this.ResetCat_5_List(filterCats, DefVal.Str);
    }

    #region ResetCat_4_List
    IExtItemCats[] ResetCat_4_List(IExtItemCats[] baseCats, string specSeled)
    {
        IExtItemCats[] filterCats = null;
        var curCat = this.lstCat_4;
        //是否顯示在切換產品別即已決定
        if (!curCat.Visible) { return filterCats; }
        //若已賦值, 則保留提示項目外, 其他清空.
        if (curCat.Items.Count > 1)
        {
            var promptItem = new ListItem(curCat.Items[0].Text, curCat.Items[0].Value);
            curCat.Items.Clear();
            curCat.Items.Add(promptItem);
        }

        if (baseCats != null && baseCats.Length > 0)
        {
            var grpExtItemCats = baseCats.Where(q => !string.IsNullOrWhiteSpace(q.Category4)).GroupBy(q => q.Category4).Select(q => new { Key = q.Key, List = q.ToArray() });
            foreach (var grpInfo in grpExtItemCats)
            {
                //分類值改成允許空字串
                //var item = new ListItem(grpInfo.Key, grpInfo.Key);
                var item = new ListItem(grpInfo.Key.Replace(ExtItemHelper.EMPTY_ITEM_VALUE, "(空值)"), grpInfo.Key);
                item.Selected = !string.IsNullOrWhiteSpace(specSeled) && specSeled.Equals(grpInfo.Key);
                curCat.Items.Add(item);

                if (item.Selected)
                {
                    filterCats = grpInfo.List;
                }
            }
        }

        //若有指定選擇值, 則由叫用端自行控制.
        if (string.IsNullOrWhiteSpace(specSeled))
        {
            this.ResetCat_5_List(filterCats, DefVal.Str);
        }

        return filterCats;
    }
    #endregion

    #region ResetCat_5_List
    IExtItemCats[] ResetCat_5_List(IExtItemCats[] baseCats, string specSeled)
    {
        IExtItemCats[] filterCats = null;
        var curCat = this.lstCat_5;
        //是否顯示在切換產品別即已決定
        if (!curCat.Visible) { return filterCats; }
        //若已賦值, 則保留提示項目外, 其他清空.
        if (curCat.Items.Count > 1)
        {
            var promptItem = new ListItem(curCat.Items[0].Text, curCat.Items[0].Value);
            curCat.Items.Clear();
            curCat.Items.Add(promptItem);
        }

        if (baseCats != null && baseCats.Length > 0)
        {
            var grpExtItemCats = baseCats.Where(q => !string.IsNullOrWhiteSpace(q.Category5)).GroupBy(q => q.Category5).Select(q => new { Key = q.Key, List = q.ToArray() });
            foreach (var grpInfo in grpExtItemCats)
            {
                //分類值改成允許空字串
                //var item = new ListItem(grpInfo.Key, grpInfo.Key);
                var item = new ListItem(grpInfo.Key.Replace(ExtItemHelper.EMPTY_ITEM_VALUE, "(空值)"), grpInfo.Key);
                item.Selected = !string.IsNullOrWhiteSpace(specSeled) && specSeled.Equals(grpInfo.Key);
                curCat.Items.Add(item);

                if (item.Selected)
                {
                    filterCats = grpInfo.List;
                }
            }
        }

        curCat.Visible = curCat.Items.Count > 0;
        return filterCats;
    }
    #endregion
    #endregion

    #region 取得查詢條件
    #region 查詢條件
    class SearchConds
    {
        public SearchConds()
        {
            this.KeywordCols = new List<string>();
        }

        public ExtItemDetails.InfoConds ExtItemDetailsConds { get; set; }
        public List<string> KeywordCols { get; set; }
        public string Keyword { get; set; }
    }
    #endregion

    SearchConds GetSearchConds()
    {
        SearchConds conds;
        if (string.IsNullOrWhiteSpace(this.hidSearchConds.Value))
        {
            conds = new SearchConds();

            //conds.Keyword = this.txtKeyword.Text;
            //conds.KeywordCols.Add("ITEM");
            //conds.KeywordCols.Add("DESCRIPTION");

            conds.ExtItemDetailsConds = new ExtItemDetails.InfoConds
                (
                   DefVal.Strs,
                   this.lstItemType.SelectedValue,
                   this.lstCat_1.SelectedValue,
                   this.lstCat_2.SelectedValue,
                   this.lstCat_3.SelectedValue,
                   this.lstCat_4.SelectedValue,
                   this.lstCat_5.SelectedValue,
                   true,
                   this.IsSingleMode ? true : DefVal.Bool
                );

            this.hidSearchConds.Value = CustJson.SerializeObject(conds);
        }
        else
        {
            conds = CustJson.DeserializeObject<SearchConds>(this.hidSearchConds.Value);

            #region 還原查詢條件
            #region 外銷型號分類
            //是否有異動過外銷型號分類. 若為 true, 則全部恢復.
            bool hasChgCatConds =
                conds.ExtItemDetailsConds.ExportItemType != this.lstItemType.SelectedValue
                || conds.ExtItemDetailsConds.Category1 != this.lstCat_1.SelectedValue
                || conds.ExtItemDetailsConds.Category2 != this.lstCat_2.SelectedValue
                || conds.ExtItemDetailsConds.Category3 != this.lstCat_3.SelectedValue
                || conds.ExtItemDetailsConds.Category4 != this.lstCat_4.SelectedValue
                || conds.ExtItemDetailsConds.Category5 != this.lstCat_5.SelectedValue;

            if (hasChgCatConds)
            {
                var extItemCatConds = ExtItemHelper.GetExtItemCatConds(true);
                IExtItemCats[] filterCats = extItemCatConds.ExtItemCats.Where(q => conds.ExtItemDetailsConds.ExportItemType.Equals(q.ExportItemType)).ToArray();

                #region 產品別
                if (string.IsNullOrWhiteSpace(conds.ExtItemDetailsConds.ExportItemType))
                {
                    //之後的連續清空
                    WebUtilBox.SetListControlSelected(string.Empty, this.lstItemType);
                    filterCats = null;
                    this.ResetCat_1_List(filterCats, DefVal.Str);
                }
                else
                {
                    if (conds.ExtItemDetailsConds.ExportItemType != this.lstItemType.SelectedValue)
                    {
                        #region 重置分類
                        this.lstCat_1.Items.Clear();
                        this.lstCat_2.Items.Clear();
                        this.lstCat_3.Items.Clear();
                        this.lstCat_4.Items.Clear();
                        this.lstCat_5.Items.Clear();

                        var extItemTypeInfo = extItemCatConds.ExtItemTypes.Where(q => conds.ExtItemDetailsConds.ExportItemType.Equals(q.ExportItemType)).Single();

                        this.lstCat_1.Visible = !string.IsNullOrWhiteSpace(extItemTypeInfo.CategoryDesc1);
                        if (this.lstCat_1.Visible)
                        {
                            this.lstCat_1.Items.Add(new ListItem(string.Format("[{0}]", extItemTypeInfo.CategoryDesc1), string.Empty));
                        }

                        this.lstCat_2.Visible = !string.IsNullOrWhiteSpace(extItemTypeInfo.CategoryDesc2);
                        if (this.lstCat_2.Visible)
                        {
                            this.lstCat_2.Items.Add(new ListItem(string.Format("[{0}]", extItemTypeInfo.CategoryDesc2), string.Empty));
                        }

                        this.lstCat_3.Visible = !string.IsNullOrWhiteSpace(extItemTypeInfo.CategoryDesc3);
                        if (this.lstCat_3.Visible)
                        {
                            this.lstCat_3.Items.Add(new ListItem(string.Format("[{0}]", extItemTypeInfo.CategoryDesc3), string.Empty));
                        }

                        this.lstCat_4.Visible = !string.IsNullOrWhiteSpace(extItemTypeInfo.CategoryDesc4);
                        if (this.lstCat_4.Visible)
                        {
                            this.lstCat_4.Items.Add(new ListItem(string.Format("[{0}]", extItemTypeInfo.CategoryDesc4), string.Empty));
                        }

                        this.lstCat_5.Visible = !string.IsNullOrWhiteSpace(extItemTypeInfo.CategoryDesc5);
                        if (this.lstCat_5.Visible)
                        {
                            this.lstCat_5.Items.Add(new ListItem(string.Format("[{0}]", extItemTypeInfo.CategoryDesc5), string.Empty));
                        }
                        #endregion
                    }

                    if (!string.IsNullOrWhiteSpace(WebUtilBox.SetListControlSelected(conds.ExtItemDetailsConds.ExportItemType, this.lstItemType)))
                    {
                        //若無下階值, 則下下階之後的會直接連續清空.
                        filterCats = this.ResetCat_1_List(filterCats, conds.ExtItemDetailsConds.Category1);
                    }
                    else
                    {
                        //若指定後仍是找不到, 或下階為空值, 則之後的連續清空.
                        filterCats = null;
                        this.ResetCat_1_List(filterCats, DefVal.Str);
                    }
                }
                #endregion

                #region 分類 1
                if (filterCats != null && filterCats.Length > 0)
                {
                    if (string.IsNullOrWhiteSpace(conds.ExtItemDetailsConds.Category1))
                    {
                        //之後的連續清空
                        WebUtilBox.SetListControlSelected(string.Empty, this.lstCat_1);
                        filterCats = null;
                        this.ResetCat_2_List(filterCats, DefVal.Str);
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(WebUtilBox.SetListControlSelected(conds.ExtItemDetailsConds.Category1, this.lstCat_1)))
                        {
                            //若無下階值, 則下下階之後的會直接連續清空.
                            filterCats = this.ResetCat_2_List(filterCats, conds.ExtItemDetailsConds.Category2);
                        }
                        else
                        {
                            //若指定後仍是找不到, 或下階為空值, 則之後的連續清空.
                            filterCats = null;
                            this.ResetCat_2_List(filterCats, DefVal.Str);
                        }
                    }
                }
                #endregion

                #region 分類 2
                if (filterCats != null && filterCats.Length > 0)
                {
                    if (string.IsNullOrWhiteSpace(conds.ExtItemDetailsConds.Category2))
                    {
                        //之後的連續清空
                        WebUtilBox.SetListControlSelected(string.Empty, this.lstCat_2);
                        filterCats = null;
                        this.ResetCat_3_List(filterCats, DefVal.Str);
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(WebUtilBox.SetListControlSelected(conds.ExtItemDetailsConds.Category2, this.lstCat_2)))
                        {
                            //若無下階值, 則下下階之後的會直接連續清空.
                            filterCats = this.ResetCat_3_List(filterCats, conds.ExtItemDetailsConds.Category3);
                        }
                        else
                        {
                            //若指定後仍是找不到, 或下階為空值, 則之後的連續清空.
                            filterCats = null;
                            this.ResetCat_3_List(filterCats, DefVal.Str);
                        }
                    }
                }
                #endregion

                #region 分類 3
                if (filterCats != null && filterCats.Length > 0)
                {
                    if (string.IsNullOrWhiteSpace(conds.ExtItemDetailsConds.Category3))
                    {
                        //之後的連續清空
                        WebUtilBox.SetListControlSelected(string.Empty, this.lstCat_3);
                        filterCats = null;
                        this.ResetCat_4_List(filterCats, DefVal.Str);
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(WebUtilBox.SetListControlSelected(conds.ExtItemDetailsConds.Category3, this.lstCat_3)))
                        {
                            //若無下階值, 則下下階之後的會直接連續清空.
                            filterCats = this.ResetCat_4_List(filterCats, conds.ExtItemDetailsConds.Category4);
                        }
                        else
                        {
                            //若指定後仍是找不到, 或下階為空值, 則之後的連續清空.
                            filterCats = null;
                            this.ResetCat_4_List(filterCats, DefVal.Str);
                        }
                    }
                }
                #endregion

                #region 分類 4
                if (filterCats != null && filterCats.Length > 0)
                {
                    if (string.IsNullOrWhiteSpace(conds.ExtItemDetailsConds.Category4))
                    {
                        //之後的連續清空
                        WebUtilBox.SetListControlSelected(string.Empty, this.lstCat_4);
                        filterCats = null;
                        this.ResetCat_5_List(filterCats, DefVal.Str);
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(WebUtilBox.SetListControlSelected(conds.ExtItemDetailsConds.Category4, this.lstCat_4)))
                        {
                            //若無下階值, 則下下階之後的會直接連續清空.
                            filterCats = this.ResetCat_5_List(filterCats, conds.ExtItemDetailsConds.Category5);
                        }
                        else
                        {
                            //若指定後仍是找不到, 或下階為空值, 則之後的連續清空.
                            filterCats = null;
                            this.ResetCat_5_List(filterCats, DefVal.Str);
                        }
                    }
                }
                #endregion

                #region 分類 5
                if (filterCats != null && filterCats.Length > 0)
                {
                    if (string.IsNullOrWhiteSpace(conds.ExtItemDetailsConds.Category5))
                    {
                        WebUtilBox.SetListControlSelected(string.Empty, this.lstCat_5);
                    }
                    else
                    {
                        WebUtilBox.SetListControlSelected(conds.ExtItemDetailsConds.Category5, this.lstCat_5);
                    }
                }
                #endregion
            }
            #endregion

            //this.txtKeyword.Text = conds.Keyword;
            #endregion
        }

        return conds;
    }
    #endregion

    #region 外銷商品列表
    /// <summary> 
    /// 外銷商品列表。 
    /// </summary> 
    void ExtItemDetailsList()
    {
        Returner returner = null;
        try
        {
            TableRow htmlTr = null;
            TableHeaderCell htmlTh;
            TableCell htmlTd;

            #region 標題
            Table htmlTable = new Table();
            htmlTable.CellPadding = 0;
            htmlTable.CellSpacing = 0;
            htmlTable.CssClass = "ListTable";
            this.phPageList.Controls.Add(htmlTable);

            htmlTr = new TableRow();
            htmlTable.Rows.Add(htmlTr);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "型號";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "料號";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "分類";
            htmlTr.Cells.Add(htmlTh);

            htmlTh = new TableHeaderCell();
            htmlTh.Text = "選擇";
            htmlTr.Cells.Add(htmlTh);
            #endregion

            //網頁未完成初始時不執行
            if (!this.HasInitial) { return; }

            ExtItemDetails entityExtItemDetails = new ExtItemDetails(SystemDefine.ConnInfo);

            var conds = this.GetSearchConds();

            returner = entityExtItemDetails.GetInfoCount(conds.ExtItemDetailsConds);

            //分頁初始
            PagingFlipper flipper = new PagingFlipper()
            {
                Size = Convert.ToInt32(this.listCountOfPage.SelectedValue),
                Total = Convert.ToInt32(returner.DataSet.Tables[0].Rows[0][0])
            };

            this.observerPaging.TotalPageCount = this.observerPaging.CalculateTotalPageCount(flipper.Size, flipper.Total);
            this.observerPaging.PagingInit();

            if (this.DoSearch)
            {
                this.observerPaging.CurrentPageNumber = 1;
            }

            this.observerPaging.PagingActing();
            this.phPaging.Visible = this.observerPaging.TotalPageCount > 1;

            flipper.Page = this.observerPaging.CurrentPageNumber;
            if (flipper.Total == 0)
            {
                //this.litCurrentPageNumber.Text = "0";
                this.litTotalPageNumber.Text = "0";
                this.litSearchResulted.Text = "0";
            }
            else
            {
                //this.litCurrentPageNumber.Text = this.observerPaging.CurrentPageNumber.ToString();
                this.litTotalPageNumber.Text = this.observerPaging.TotalPageCount.ToString();
                this.litSearchResulted.Text = flipper.Total.ToString();
            }

            SqlOrder sorting = SqlOrder.Default;
            returner = entityExtItemDetails.GetInfo(conds.ExtItemDetailsConds, flipper, sorting);

            if (returner.IsCompletedAndContinue)
            {
                var infos = ExtItemDetails.Info.Binding(returner.DataSet.Tables[0]);

                //已存在的品項暫存
                var existedGoodsItems = new List<string>();
                existedGoodsItems.AddRange(this.hidExistedGoodsItems.Value.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries));

                for (int i = 0; i < infos.Length; i++)
                {
                    var info = infos[i];

                    htmlTr = new TableRow();
                    htmlTr.CssClass = "dev-sel-row";
                    htmlTable.Rows.Add(htmlTr);

                    string titleOfToolTip = string.Format("{0}", info.ErpItem);

                    htmlTd = new TableCell();
                    htmlTd.Text = info.ExportItem;
                    htmlTr.Cells.Add(htmlTd);

                    htmlTd = new TableCell();
                    htmlTd.Text = info.ErpItem;
                    htmlTr.Cells.Add(htmlTd);

                    htmlTd = new TableCell();
                    htmlTd.Text = string.Join(" / ", ConvertLib.ToStrs(info.ExportItemType, info.Category1, info.Category2, info.Category3, info.Category4, info.Category5));
                    htmlTr.Cells.Add(htmlTd);

                    //編碼型號 (因為有逗號)
                    var encodeExportItem = ConvertLib.ToBase64Encoding(info.ExportItem);

                    htmlTd = new TableCell();
                    htmlTd.CssClass = "dev-sel-cell";
                    if (this.IsSingleMode)
                    {
                        //用 javascript 控制, 點 row 即選擇, 若在 button 中也加 onlick, 會執行兩次.
                        //htmlTd.Text = string.Format("<input type='button' value='選擇' class='dev-sel' customerid='{0}' onclick='returnSingleInfo({0}); return false;' />", info.ErpItem);
                        htmlTd.Text = existedGoodsItems.Contains(string.Format("{1}{0}{2}", SystemDefine.JoinSeparator, encodeExportItem, info.ErpItem)) ? string.Empty : string.Format("<input type='button' value='選擇' class='dev-sel' partno='{1}{0}{2}' />", SystemDefine.JoinSeparator, encodeExportItem, info.ErpItem);
                    }
                    else
                    {
                        htmlTd.Text = existedGoodsItems.Contains(string.Format("{1}{0}{2}", SystemDefine.JoinSeparator, encodeExportItem, info.ErpItem)) ? string.Empty : string.Format("<input type='checkbox' class='dev-sel' value='{1}{0}{2}' />", SystemDefine.JoinSeparator, encodeExportItem, info.ErpItem);
                    }
                    htmlTr.Cells.Add(htmlTd);
                }
            }
        }
        finally
        {
            if (returner != null) returner.Dispose();
        }
    }
    #endregion
}