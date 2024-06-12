using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Globalization;

using EzCoding;
using EzCoding.SystemEngines;
using EzCoding.DB;
using EzCoding.SystemEngines.EntityController;
using Seec.Marketing.Erp;

namespace Seec.Marketing.SystemEngines
{
    public partial class DBTableDefine
    {
        /// <summary>
        /// 外銷商品。
        /// </summary>
        public const string EXT_ITEM_DETAILS = "EXT_ITEM_DETAILS";
    }
    #region Michelle 修改註記 2016.5.13
    // 原 CreatedBy,LastUpdatedBy 欄位型態寫為long,因為原本沒用到,但外銷型號維護會用到,會記錄使用者的sid,所以都改為ISystemId
    #endregion

    /// <summary>
    /// 外銷商品的類別實作。
    /// </summary>
    public class ExtItemDetails : SystemBase
    {
        /// <summary>
        /// 初始化 Seec.Marketing.SystemEngines.ExtItemDetails 類別的新執行個體。
        /// </summary>
        /// <param name="connInfo">資料庫連線資訊。</param>
        public ExtItemDetails(string connInfo)
            : base(EntityHelper.GetEntityBase(connInfo, DBTableDefine.EXT_ITEM_DETAILS))
        {
            SqlOrder sorting = new SqlOrder();
            sorting.Add("ERP_ITEM", Sort.Ascending);

            base.DefaultSqlOrder = sorting;
        }

        #region 表格資料
        /// <summary>
        /// 表格資料。
        /// </summary>
        public class Info : InfoBase, IExtItemCats
        {
            /// <summary>
            /// 外銷型號。
            /// </summary>
            [SchemaMapping(Name = "EXPORT_ITEM", Type = ReturnType.String)]
            public string ExportItem { get; set; }
            /// <summary>
            /// 料號 ID。
            /// </summary>
            [SchemaMapping(Name = "ERP_ITEM_ID", Type = ReturnType.Long, AllowNull = true)]
            public long? ErpItemId { get; set; }
            /// <summary>
            /// 料號。
            /// </summary>
            [SchemaMapping(Name = "ERP_ITEM", Type = ReturnType.String)]
            public string ErpItem { get; set; }
            /// <summary>
            /// 產品別。
            /// </summary>
            [SchemaMapping(Name = "EXPORT_ITEM_TYPE", Type = ReturnType.String)]
            public string ExportItemType { get; set; }
            /// <summary>
            /// 分類 1。
            /// </summary>
            [SchemaMapping(Name = "CATEGORY1", Type = ReturnType.String)]
            public string Category1 { get; set; }
            /// <summary>
            /// 分類 2。
            /// </summary>
            [SchemaMapping(Name = "CATEGORY2", Type = ReturnType.String)]
            public string Category2 { get; set; }
            /// <summary>
            /// 分類 3。
            /// </summary>
            [SchemaMapping(Name = "CATEGORY3", Type = ReturnType.String)]
            public string Category3 { get; set; }
            /// <summary>
            /// 分類 4。
            /// </summary>
            [SchemaMapping(Name = "CATEGORY4", Type = ReturnType.String)]
            public string Category4 { get; set; }
            /// <summary>
            /// 分類 5。
            /// </summary>
            [SchemaMapping(Name = "CATEGORY5", Type = ReturnType.String)]
            public string Category5 { get; set; }
            /// <summary>
            /// 分類 6。
            /// </summary>
            [SchemaMapping(Name = "CATEGORY6", Type = ReturnType.String)]
            public string Category6 { get; set; }
            /// <summary>
            /// 分類 7。
            /// </summary>
            [SchemaMapping(Name = "CATEGORY7", Type = ReturnType.String)]
            public string Category7 { get; set; }
            /// <summary>
            /// 分類 8。
            /// </summary>
            [SchemaMapping(Name = "CATEGORY8", Type = ReturnType.String)]
            public string Category8 { get; set; }
            /// <summary>
            /// 分類 9。
            /// </summary>
            [SchemaMapping(Name = "CATEGORY9", Type = ReturnType.String)]
            public string Category9 { get; set; }
            /// <summary>
            /// 分類 10。
            /// </summary>
            [SchemaMapping(Name = "CATEGORY10", Type = ReturnType.String)]
            public string Category10 { get; set; }
            /// <summary>
            /// 分類 11。
            /// </summary>
            [SchemaMapping(Name = "CATEGORY11", Type = ReturnType.String)]
            public string Category11 { get; set; }
            /// <summary>
            /// 分類 12。
            /// </summary>
            [SchemaMapping(Name = "CATEGORY12", Type = ReturnType.String)]
            public string Category12 { get; set; }
            /// <summary>
            /// 分類 13。
            /// </summary>
            [SchemaMapping(Name = "CATEGORY13", Type = ReturnType.String)]
            public string Category13 { get; set; }
            /// <summary>
            /// 分類 14。
            /// </summary>
            [SchemaMapping(Name = "CATEGORY14", Type = ReturnType.String)]
            public string Category14 { get; set; }
            /// <summary>
            /// 分類 15。
            /// </summary>
            [SchemaMapping(Name = "CATEGORY15", Type = ReturnType.String)]
            public string Category15 { get; set; }
            /// <summary>
            /// 是否作用中。
            /// </summary>
            [SchemaMapping(Name = "ACTIVE_FLAG", Type = ReturnType.String)]
            public string ActiveFlag { get; set; }
            /// <summary>
            /// 建立時間。
            /// </summary>
            [SchemaMapping(Name = "CREATEION_DATE", Type = ReturnType.DateTime)]
            public DateTime CreateionDate { get; set; }
            /// <summary>
            /// 建立人。
            /// </summary>
            [SchemaMapping(Name = "CREATED_BY", Type = ReturnType.SId)]
            public ISystemId CreatedBy { get; set; }
            /// <summary>
            /// 修改時間。
            /// </summary>
            [SchemaMapping(Name = "LAST_UPDATED_DATE", Type = ReturnType.DateTime, AllowNull = true)]
            public DateTime? LastUpdatedDate { get; set; }
            /// <summary>
            /// 修改人。
            /// </summary>
            [SchemaMapping(Name = "LAST_UPDATED_BY", Type = ReturnType.SId)]
            public ISystemId LastUpdatedBy { get; set; }

            #region 繫結資料
            /// <summary>
            /// 繫結資料。
            /// </summary>
            public static Info Binding(DataRow row)
            {
                return DBUtilBox.BindingRow<Info>(new Info(), row);
            }

            /// <summary>
            /// 繫結資料。
            /// </summary>
            public static Info[] Binding(DataTable dTable)
            {
                List<Info> infos = new List<Info>();
                foreach (DataRow row in dTable.Rows)
                {
                    infos.Add(Info.Binding(row));
                }

                return infos.ToArray();
            }
            #endregion
        }
        #endregion

        #region 異動
        #region Add
        /// <summary>
        /// 依指定的參數，新增一筆資料。
        /// </summary>
        /// <param name="exportItem">外銷型號。</param>
        /// <param name="erpItemId">料號 ID（null 將自動略過操作）。</param>
        /// <param name="erpItem">料號（null 或 empty 將自動略過操作）。</param>
        /// <param name="exportItemType">產品別（null 或 empty 將自動略過操作）。</param>
        /// <param name="category1">分類 1（null 或 empty 將自動略過操作）。</param>
        /// <param name="category2">分類 2（null 或 empty 將自動略過操作）。</param>
        /// <param name="category3">分類 3（null 或 empty 將自動略過操作）。</param>
        /// <param name="category4">分類 4（null 或 empty 將自動略過操作）。</param>
        /// <param name="category5">分類 5（null 或 empty 將自動略過操作）。</param>
        /// <param name="category6">分類 6（null 或 empty 將自動略過操作）。</param>
        /// <param name="category7">分類 7（null 或 empty 將自動略過操作）。</param>
        /// <param name="category8">分類 8（null 或 empty 將自動略過操作）。</param>
        /// <param name="category9">分類 9（null 或 empty 將自動略過操作）。</param>
        /// <param name="category10">分類 10（null 或 empty 將自動略過操作）。</param>
        /// <param name="category11">分類 11（null 或 empty 將自動略過操作）。</param>
        /// <param name="category12">分類 12（null 或 empty 將自動略過操作）。</param>
        /// <param name="category13">分類 13（null 或 empty 將自動略過操作）。</param>
        /// <param name="category14">分類 14（null 或 empty 將自動略過操作）。</param>
        /// <param name="category15">分類 15（null 或 empty 將自動略過操作）。</param>
        /// <param name="activeFlag">是否作用中。</param>
        /// <param name="createionDate">建立時間。</param>
        /// <param name="createdBy">建立人。</param>
        /// <param name="lastUpdatedDate">修改時間（null 將自動略過操作）。</param>
        /// <param name="lastUpdatedBy">修改人（null 將自動略過操作）。</param>
        /// <returns>EzCoding.Returner。</returns>
        /// <exception cref="System.ArgumentNullException">參數 exportItem 不允許為 null 值。</exception>
        public Returner Add(string exportItem, long? erpItemId, string erpItem, string exportItemType, string category1, string category2, string category3, string category4, string category5, string category6, string category7, string category8, string category9, string category10, string category11, string category12, string category13, string category14, string category15, string activeFlag, DateTime createionDate, ISystemId createdBy, DateTime? lastUpdatedDate, ISystemId lastUpdatedBy)
        {
            if (exportItem == null) { throw new ArgumentNullException("exportItem"); }

            Returner returner = new Returner();
            try
            {
                #region 交易緩衝區
                TransColValSet transSet = new TransColValSet();
                transSet.SmartAdd("EXPORT_ITEM", exportItem, GenericDBType.NVarChar, false);
                transSet.SmartAdd("ERP_ITEM_ID", erpItemId, GenericDBType.BigInt);
                transSet.SmartAdd("ERP_ITEM", erpItem, GenericDBType.NVarChar, true);
                transSet.SmartAdd("EXPORT_ITEM_TYPE", exportItemType, GenericDBType.NVarChar, true);
                transSet.SmartAdd("CATEGORY1", category1, GenericDBType.NVarChar, true);
                transSet.SmartAdd("CATEGORY2", category2, GenericDBType.NVarChar, true);
                transSet.SmartAdd("CATEGORY3", category3, GenericDBType.NVarChar, true);
                transSet.SmartAdd("CATEGORY4", category4, GenericDBType.NVarChar, true);
                transSet.SmartAdd("CATEGORY5", category5, GenericDBType.NVarChar, true);
                transSet.SmartAdd("CATEGORY6", category6, GenericDBType.NVarChar, true);
                transSet.SmartAdd("CATEGORY7", category7, GenericDBType.NVarChar, true);
                transSet.SmartAdd("CATEGORY8", category8, GenericDBType.NVarChar, true);
                transSet.SmartAdd("CATEGORY9", category9, GenericDBType.NVarChar, true);
                transSet.SmartAdd("CATEGORY10", category10, GenericDBType.NVarChar, true);
                transSet.SmartAdd("CATEGORY11", category11, GenericDBType.NVarChar, true);
                transSet.SmartAdd("CATEGORY12", category12, GenericDBType.NVarChar, true);
                transSet.SmartAdd("CATEGORY13", category13, GenericDBType.NVarChar, true);
                transSet.SmartAdd("CATEGORY14", category14, GenericDBType.NVarChar, true);
                transSet.SmartAdd("CATEGORY15", category15, GenericDBType.NVarChar, true);
                transSet.SmartAdd("ACTIVE_FLAG", activeFlag, GenericDBType.Char, false);
                transSet.SmartAdd("CREATEION_DATE", createionDate, "yyyyMMddHHmmss", GenericDBType.Char);
                transSet.SmartAdd("CREATED_BY", createdBy.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("LAST_UPDATED_DATE", lastUpdatedDate, "yyyyMMddHHmmss", GenericDBType.Char);
                transSet.SmartAdd("LAST_UPDATED_BY", lastUpdatedBy.ToString(), GenericDBType.Char, false);

                returner.ChangeInto(base.Add(transSet, true));
                #endregion

                return returner;
            }
            finally
            {
            }
        }
        #endregion

        #region Modify
        /// <summary>
        /// 依指定的參數，修改一筆 外銷商品。
        /// </summary>
        /// <param name="exportItem">外銷型號。</param>
        /// <param name="erpItemId">料號 ID（null 則直接設為 DBNull）。</param>
        /// <param name="erpItem">料號（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="exportItemType">產品別（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="category1">分類 1（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="category2">分類 2（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="category3">分類 3（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="category4">分類 4（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="category5">分類 5（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="category6">分類 6（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="category7">分類 7（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="category8">分類 8（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="category9">分類 9（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="category10">分類 10（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="category11">分類 11（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="category12">分類 12（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="category13">分類 13（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="category14">分類 14（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="category15">分類 15（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="activeFlag">是否作用中。</param>
        /// <param name="lastUpdatedDate">修改時間（null 則直接設為 DBNull）。</param>
        /// <param name="lastUpdatedBy">修改人（null 則直接設為 DBNull）。</param>
        /// <returns>EzCoding.Returner。</returns>
        /// <exception cref="System.ArgumentNullException">參數 exportItem 不允許為 null 值。</exception>
        public Returner Modify(string exportItem, long? erpItemId, string erpItem, string exportItemType, string category1, string category2, string category3, string category4, string category5, string category6, string category7, string category8, string category9, string category10, string category11, string category12, string category13, string category14, string category15, string activeFlag, DateTime? lastUpdatedDate, ISystemId lastUpdatedBy)
        {
            if (exportItem == null) { throw new ArgumentNullException("exportItem"); }

            Returner returner = new Returner();
            try
            {
                #region 交易緩衝區
                TransColValSet transSet = new TransColValSet();
                transSet.SmartAdd("EXPORT_ITEM", exportItem, GenericDBType.NVarChar, false);
                transSet.SmartAdd("ERP_ITEM_ID", erpItemId, GenericDBType.BigInt, true);
                transSet.SmartAdd("ERP_ITEM", erpItem, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("EXPORT_ITEM_TYPE", exportItemType, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("CATEGORY1", category1, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("CATEGORY2", category2, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("CATEGORY3", category3, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("CATEGORY4", category4, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("CATEGORY5", category5, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("CATEGORY6", category6, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("CATEGORY7", category7, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("CATEGORY8", category8, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("CATEGORY9", category9, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("CATEGORY10", category10, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("CATEGORY11", category11, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("CATEGORY12", category12, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("CATEGORY13", category13, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("CATEGORY14", category14, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("CATEGORY15", category15, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("ACTIVE_FLAG", activeFlag, GenericDBType.Char, false);
                transSet.SmartAdd("LAST_UPDATED_DATE", lastUpdatedDate, "yyyyMMddHHmmss", GenericDBType.Char, true);
                transSet.SmartAdd("LAST_UPDATED_BY", lastUpdatedBy.ToString(), GenericDBType.Char, true);

                SqlCondsSet condsMain = new SqlCondsSet();
                condsMain.Add("EXPORT_ITEM", SqlCond.EqualTo, exportItem, GenericDBType.NVarChar, SqlCondsSet.And);

                returner.ChangeInto(base.Modify(transSet, condsMain, true));
                #endregion

                return returner;
            }
            finally
            {
            }
        }
        #endregion
        #endregion

        #region 一般查詢
    
        #region 查詢條件
        /// <summary>
        /// 查詢條件。
        /// </summary>
        public class InfoConds
        {
            ///// <summary>
            ///// 初始化 InfoConds 類別的新執行個體。
            ///// 預設略過所有條件。
            ///// </summary>
            //public InfoConds()
            //{

            //}

            /// <summary>
            /// 初始化 InfoConds 類別的新執行個體。
            /// </summary>
            /// <param name="exportItems">外銷型號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="exportItemType">產品別（若為 null 或 empty 則略過條件檢查）。</param>
            /// <param name="category1">分類 1（若為 null 或 empty 則略過條件檢查）。</param>
            /// <param name="category2">分類 2（若為 null 或 empty 則略過條件檢查）。</param>
            /// <param name="category3">分類 3（若為 null 或 empty 則略過條件檢查）。</param>
            /// <param name="category4">分類 4（若為 null 或 empty 則略過條件檢查）。</param>
            /// <param name="category5">分類 5（若為 null 或 empty 則略過條件檢查）。</param>
            /// <param name="activeFlag">是否作用中（若為 null 則略過條件檢查）。</param>
            /// <param name="reqErpItem">料號是否必須有值（若為 null 則略過條件檢查）。</param>
            public InfoConds(string[] exportItems, string exportItemType, string category1, string category2, string category3, string category4, string category5, bool? activeFlag, bool? reqErpItem)
            {
                this.ExportItems = exportItems;
                this.ExportItemType = exportItemType;
                this.Category1 = category1;
                this.Category2 = category2;
                this.Category3 = category3;
                this.Category4 = category4;
                this.Category5 = category5;
                this.ActiveFlag = activeFlag;
                this.ReqErpItem = reqErpItem;
            }

 
            /// <summary>
            /// 外銷型號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public string[] ExportItems { get; set; }
            /// <summary>
            /// 產品別（若為 null 或 empty 則略過條件檢查）。
            /// </summary>
            public string ExportItemType { get; set; }
            /// <summary>
            /// 分類 1（若為 null 或 empty 則略過條件檢查）。
            /// </summary>
            public string Category1 { get; set; }
            /// <summary>
            /// 分類 2（若為 null 或 empty 則略過條件檢查）。
            /// </summary>
            public string Category2 { get; set; }
            /// <summary>
            /// 分類 3（若為 null 或 empty 則略過條件檢查）。
            /// </summary>
            public string Category3 { get; set; }
            /// <summary>
            /// 分類 4（若為 null 或 empty 則略過條件檢查）。
            /// </summary>
            public string Category4 { get; set; }
            /// <summary>
            /// 分類 5（若為 null 或 empty 則略過條件檢查）。
            /// </summary>
            public string Category5 { get; set; }
            /// <summary>
            /// 是否作用中（若為 null 則略過條件檢查）。
            /// </summary>
            public bool? ActiveFlag { get; set; }
            /// <summary>
            /// 料號是否必須有值（若為 null 則略過條件檢查）。
            /// </summary>
            public bool? ReqErpItem { get; set; }

        }
        #endregion

        #region GetInfo
        /// <summary> 
        /// 依指定的參數取得資料。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <param name="size">指定的前 N 筆資料（若為 Int32.MaxValue 則表示全部）。</param>
        /// <param name="sqlOrder">SQL 排序（Order By）的組合類別（若為 null 則使用預設排序）。</param>
        /// <param name="inquireColumns">含有零或多個指定查詢時所要使用的欄位名稱 System.String 陣列。若陣列為零，則使用預設開放的欄位名稱。</param>
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetInfo(InfoConds qConds, int size, SqlOrder sqlOrder, params string[] inquireColumns)
        {
            Returner returner = new Returner();

            SqlCondsSet condsMain = new SqlCondsSet();
            condsMain.Add(GetInfoCondsSet(qConds));

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.Entity.GetInfoBy(size, sqlOrder.UseDefault ? base.DefaultSqlOrder : sqlOrder, condsMain, inquireColumns));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }

        /// <summary> 
        /// 依指定的參數取得資料。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <param name="flipper">分頁操作參數集合。</param>
        /// <param name="sqlOrder">SQL 排序（Order By）的組合類別（若為 null 則使用預設排序）。</param>
        /// <param name="inquireColumns">含有零或多個指定查詢時所要使用的欄位名稱 System.String 陣列。若陣列為零，則使用預設開放的欄位名稱。</param>
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetInfo(InfoConds qConds, PagingFlipper flipper, SqlOrder sqlOrder, params string[] inquireColumns)
        {
            Returner returner = new Returner();

            SqlCondsSet condsMain = new SqlCondsSet();
            condsMain.Add(GetInfoCondsSet(qConds));

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.Entity.GetPagingBy(flipper, sqlOrder.UseDefault ? base.DefaultSqlOrder : sqlOrder, condsMain, inquireColumns));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetInfoCount
        /// <summary> 
        /// 依指定的參數取得資料總數。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetInfoCount(InfoConds qConds)
        {
            Returner returner = new Returner();

            SqlCondsSet condsMain = new SqlCondsSet();
            condsMain.Add(GetInfoCondsSet(qConds));

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.Entity.GetCountBy(condsMain));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetInfoByCompoundSearch
        /// <summary> 
        /// 依指定的參數取得資料（複合式查詢）。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <param name="columnsName">欄位名稱陣列。</param>
        /// <param name="keyword">關鍵字。</param>
        /// <param name="sqlLikeMode">T-SQL 語法中的 LIKE 查詢模式。</param>
        /// <param name="sqlLikeOperator">T-SQL 語法中的 LIKE 運算式。</param>
        /// <param name="size">指定的前 N 筆資料（若為 Int32.MaxValue 則表示全部）。</param>
        /// <param name="sqlOrder">SQL 排序（Order By）的組合類別（若為 null 則使用預設排序）。</param>
        /// <param name="includeScope">資料取得所包含的範圍（是否註解刪除或啟用中）。</param>
        /// <param name="inquireColumns">含有零或多個指定查詢時所要使用的欄位名稱 System.String 陣列。若陣列為零，則使用預設開放的欄位名稱。</param>
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetInfoByCompoundSearch(InfoConds qConds, string[] columnsName, string keyword, SqlLikeMode sqlLikeMode, SqlLikeOperator sqlLikeOperator, int size, SqlOrder sqlOrder, IncludeScope includeScope, params string[] inquireColumns)
        {
            Returner returner = new Returner();

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.GetInfoByCompoundSearch(columnsName, keyword, EntityHelper.KEYWORD_SEPARATOR, sqlLikeMode, sqlLikeOperator, size, sqlOrder, GetInfoCondsSet(qConds), inquireColumns));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }

        /// <summary> 
        /// 依指定的參數取得資料（複合式查詢）。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <param name="columnsName">欄位名稱陣列。</param>
        /// <param name="keyword">關鍵字。</param>
        /// <param name="isAbsolute">是否為絕對查詢。若 false 則使用 like 查詢。</param>
        /// <param name="flipper">分頁操作參數集合。</param>
        /// <param name="sqlOrder">SQL 排序（Order By）的組合類別（若為 null 則使用預設排序）。</param>
        /// <param name="inquireColumns">含有零或多個指定查詢時所要使用的欄位名稱 System.String 陣列。若陣列為零，則使用預設開放的欄位名稱。</param>
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetInfoByCompoundSearch(InfoConds qConds, string[] columnsName, string keyword, bool isAbsolute, PagingFlipper flipper, SqlOrder sqlOrder, params string[] inquireColumns)
        {
            Returner returner = new Returner();

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.GetInfoByCompoundSearch(columnsName, keyword, EntityHelper.KEYWORD_SEPARATOR, isAbsolute, flipper, sqlOrder, GetInfoCondsSet(qConds), inquireColumns));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetInfoByCompoundSearchCount
        /// <summary> 
        /// 依指定的參數取得資料總數（複合式查詢）。 
        /// 若欄位名稱陣列長度等於0，則會略過指定欄位名稱查詢動作。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <param name="columnsName">欄位名稱陣列。</param> 
        /// <param name="keyword">關鍵字。</param> 
        /// <param name="isAbsolute">是否為絕對查詢。若 false 則使用 like 查詢。</param> 
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetInfoByCompoundSearchCount(InfoConds qConds, string[] columnsName, string keyword, bool isAbsolute)
        {
            Returner returner = new Returner();

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.GetInfoByCompoundSearchCount(columnsName, keyword, EntityHelper.KEYWORD_SEPARATOR, isAbsolute, GetInfoCondsSet(qConds)));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetGroupInfo
        /// <summary> 
        /// 依指定的欄位取得群組資料。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <param name="fieldNames">欄位名稱陣列集合。</param>
        /// <param name="sort">排序方式。</param>
        /// <returns>EzCoding.Returner。</returns>
        public Returner GetGroupInfo(InfoConds qConds, string[] fieldNames, Sort sort)
        {
            SqlCondsSet condsMain = new SqlCondsSet();
            condsMain.Add(GetInfoCondsSet(qConds));

            SqlGroup grouping = new SqlGroup();
            SqlOrder sorting = new SqlOrder();

            for (int i = 0; i < fieldNames.Length; i++)
            {
                grouping.Add(fieldNames[i]);
                sorting.Add(fieldNames[i], sort);
            }

            try
            {
                base.Entity.EnableCustomEntity = true;
                return base.Entity.GetGroupBy(grouping, sorting, condsMain);
            }
            finally
            {
                base.Entity.EnableCustomEntity = false;
            }
        }
        #endregion

        #region GetInfoCondsSet
        /// <summary> 
        /// 依指定的條件取得 SqlCondsSet。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <returns>EzCoding.Collections.SqlCondsSet。</returns> 
        SqlCondsSet GetInfoCondsSet(InfoConds qConds)
        {
            #region 自訂表格
            var custEntity = new SqlSyntaxBuilder(base.Entity, new SqlSyntax());

            custEntity.Append("SELECT [EXT_ITEM_DETAILS].* ");
            custEntity.Append("FROM [EXT_ITEM_DETAILS] ");

            var sqlConds = new List<string>();

            if (qConds.ExportItems != null && qConds.ExportItems.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "EXPORT_ITEM", SqlOperator.EqualTo, qConds.ExportItems, GenericDBType.NVarChar));
            }

            if (!string.IsNullOrWhiteSpace(qConds.ExportItemType))
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "EXPORT_ITEM_TYPE", SqlOperator.EqualTo, qConds.ExportItemType, GenericDBType.NVarChar));
            }

            #region 分類值改成允許空字串 - 已註解
            //if (!string.IsNullOrWhiteSpace(qConds.Category1))
            //{
            //    sqlConds.Add(custEntity.BuildConds(string.Empty, "CATEGORY1", SqlOperator.EqualTo, qConds.Category1, GenericDBType.NVarChar));
            //}

            //if (!string.IsNullOrWhiteSpace(qConds.Category2))
            //{
            //    sqlConds.Add(custEntity.BuildConds(string.Empty, "CATEGORY2", SqlOperator.EqualTo, qConds.Category2, GenericDBType.NVarChar));
            //}

            //if (!string.IsNullOrWhiteSpace(qConds.Category3))
            //{
            //    sqlConds.Add(custEntity.BuildConds(string.Empty, "CATEGORY3", SqlOperator.EqualTo, qConds.Category3, GenericDBType.NVarChar));
            //}

            //if (!string.IsNullOrWhiteSpace(qConds.Category4))
            //{
            //    sqlConds.Add(custEntity.BuildConds(string.Empty, "CATEGORY4", SqlOperator.EqualTo, qConds.Category4, GenericDBType.NVarChar));
            //}

            //if (!string.IsNullOrWhiteSpace(qConds.Category5))
            //{
            //    sqlConds.Add(custEntity.BuildConds(string.Empty, "CATEGORY5", SqlOperator.EqualTo, qConds.Category5, GenericDBType.NVarChar));
            //}
            #endregion

            #region 分類值改成允許空字串
            string EMPTY_ITEM_VALUE = "[---]";

            if (!string.IsNullOrWhiteSpace(qConds.Category1))
            {
                if (qConds.Category1 == EMPTY_ITEM_VALUE)
                {
                    sqlConds.Add(custEntity.BuildConds(string.Empty, "CATEGORY1", SqlOperator.IsNull, string.Empty, GenericDBType.NVarChar));
                }
                else
                {
                    sqlConds.Add(custEntity.BuildConds(string.Empty, "CATEGORY1", SqlOperator.EqualTo, qConds.Category1, GenericDBType.NVarChar));
                }
            }

            if (!string.IsNullOrWhiteSpace(qConds.Category2))
            {
                if (qConds.Category2 == EMPTY_ITEM_VALUE)
                {
                    sqlConds.Add(custEntity.BuildConds(string.Empty, "CATEGORY2", SqlOperator.IsNull, string.Empty, GenericDBType.NVarChar));
                }
                else
                {
                    sqlConds.Add(custEntity.BuildConds(string.Empty, "CATEGORY2", SqlOperator.EqualTo, qConds.Category2, GenericDBType.NVarChar));
                }
            }

            if (!string.IsNullOrWhiteSpace(qConds.Category3))
            {
                if (qConds.Category3 == EMPTY_ITEM_VALUE)
                {
                    sqlConds.Add(custEntity.BuildConds(string.Empty, "CATEGORY3", SqlOperator.IsNull, string.Empty, GenericDBType.NVarChar));
                }
                else
                {
                    sqlConds.Add(custEntity.BuildConds(string.Empty, "CATEGORY3", SqlOperator.EqualTo, qConds.Category3, GenericDBType.NVarChar));
                }
            }

            if (!string.IsNullOrWhiteSpace(qConds.Category4))
            {
                if (qConds.Category4 == EMPTY_ITEM_VALUE)
                {
                    sqlConds.Add(custEntity.BuildConds(string.Empty, "CATEGORY4", SqlOperator.IsNull, string.Empty, GenericDBType.NVarChar));
                }
                else
                {
                    sqlConds.Add(custEntity.BuildConds(string.Empty, "CATEGORY4", SqlOperator.EqualTo, qConds.Category4, GenericDBType.NVarChar));
                }
            }

            if (!string.IsNullOrWhiteSpace(qConds.Category5))
            {
                if (qConds.Category5 == EMPTY_ITEM_VALUE)
                {
                    sqlConds.Add(custEntity.BuildConds(string.Empty, "CATEGORY5", SqlOperator.IsNull, string.Empty, GenericDBType.NVarChar));
                }
                else
                {
                    sqlConds.Add(custEntity.BuildConds(string.Empty, "CATEGORY5", SqlOperator.EqualTo, qConds.Category5, GenericDBType.NVarChar));
                }
            }
            #endregion

            if (qConds.ActiveFlag.HasValue)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "ACTIVE_FLAG", SqlOperator.EqualTo, qConds.ActiveFlag.Value ? "Y" : "N", GenericDBType.Char));
            }

            if (qConds.ReqErpItem.HasValue)
            {
                sqlConds.Add("[ERP_ITEM_ID] IS NOT NULL");
            }

            custEntity.Append(SqlUtilBox.ToSqlConds(sqlConds.Where(q => !string.IsNullOrWhiteSpace(q)).ToArray(), " WHERE ", string.Empty));
            base.Entity.CustomEntitySqlSyntax = custEntity.GetSqlSyntax();
            #endregion

            SqlCondsSet condsMain = new SqlCondsSet();

            return condsMain;
        }
        #endregion
        #endregion

        #region 檢視查詢
        #region GetGroupCatInfo
        /// <summary> 
        /// 取得群組分類清單。 
        /// </summary> 
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetGroupCatInfo()
        {
            #region 自訂表格
            var custEntity = new SqlSyntaxBuilder(base.Entity, new SqlSyntax());

            custEntity.Append(@"
                SELECT [EXPORT_ITEM_TYPE], [CATEGORY1], [CATEGORY2], [CATEGORY3], [CATEGORY4], [CATEGORY5]
                FROM [EXT_ITEM_DETAILS] WITH (NOLOCK)
                WHERE [ACTIVE_FLAG] = 'Y'
                GROUP BY [EXPORT_ITEM_TYPE], [CATEGORY1], [CATEGORY2], [CATEGORY3], [CATEGORY4], [CATEGORY5]
            ");

            var sqlConds = new List<string>();

            custEntity.Append(SqlUtilBox.ToSqlConds(sqlConds.Where(q => !string.IsNullOrWhiteSpace(q)).ToArray(), " WHERE ", string.Empty));
            base.Entity.CustomEntitySqlSyntax = custEntity.GetSqlSyntax();
            #endregion

            try
            {
                base.Entity.EnableCustomEntity = true;
                return base.Entity.GetInfoBy(Int32.MaxValue, SqlOrder.Clear, new SqlCondsSet());
            }
            finally
            {
                base.Entity.EnableCustomEntity = false;
            }
        }
        #endregion
        #endregion

        #region 檢視查詢
        #region GetGroupCatInfo
        /// <summary> 
        /// 取得ERP料號資訊。 2016.5.9 新增 by Michelle
        /// </summary> 
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetErpInfo(string ErpItem)
        {
            #region 自訂表格
            var custEntity = new SqlSyntaxBuilder(base.Entity, new SqlSyntax());

            custEntity.Append("SELECT [EXT_ITEM_DETAILS].* ");
            custEntity.Append("FROM [EXT_ITEM_DETAILS] ");

            var sqlConds = new List<string>();
            if (ErpItem != null && ErpItem.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "ERP_ITEM", SqlOperator.EqualTo, ErpItem, GenericDBType.NVarChar));
            }


            custEntity.Append(SqlUtilBox.ToSqlConds(sqlConds.Where(q => !string.IsNullOrWhiteSpace(q)).ToArray(), " WHERE ", string.Empty));
            base.Entity.CustomEntitySqlSyntax = custEntity.GetSqlSyntax();
            #endregion

            try
            {
                base.Entity.EnableCustomEntity = true;
                return base.Entity.GetInfoBy(Int32.MaxValue, SqlOrder.Clear, new SqlCondsSet());
            }
            finally
            {
                base.Entity.EnableCustomEntity = false;
            }
        }
        #endregion
        #endregion
    }
}
