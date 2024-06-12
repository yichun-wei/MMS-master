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
        /// 外銷型號分類。
        /// </summary>
        public const string EXT_ITEM_TYPE = "EXT_ITEM_TYPE";
    }

    #region Michelle 修改註記 2016.5.13
    // 原 CreatedBy,LastUpdatedBy 欄位型態寫為long,因為原本沒用到,但外銷型號維護會用到,會記錄使用者的sid,所以都改為ISystemId
    #endregion

    /// <summary>
    /// 外銷型號分類的類別實作。
    /// </summary>
    public class ExtItemType : SystemBase
    {
        /// <summary>
        /// 初始化 Seec.Marketing.SystemEngines.ExtItemType 類別的新執行個體。
        /// </summary>
        /// <param name="connInfo">資料庫連線資訊。</param>
        public ExtItemType(string connInfo)
            : base(EntityHelper.GetEntityBase(connInfo, DBTableDefine.EXT_ITEM_TYPE))
        {
            SqlOrder sorting = new SqlOrder();
            sorting.Add("EXPORT_ITEM_TYPE", Sort.Ascending);

            base.DefaultSqlOrder = sorting;
        }

        #region 表格資料
        /// <summary>
        /// 表格資料。
        /// </summary>
        public class Info : InfoBase
        {
            /// <summary>
            /// 產品別。
            /// </summary>
            [SchemaMapping(Name = "EXPORT_ITEM_TYPE", Type = ReturnType.String)]
            public string ExportItemType { get; set; }
            /// <summary>
            /// 分類 1。
            /// </summary>
            [SchemaMapping(Name = "CATEGORY_DESC1", Type = ReturnType.String)]
            public string CategoryDesc1 { get; set; }
            /// <summary>
            /// 分類 2。
            /// </summary>
            [SchemaMapping(Name = "CATEGORY_DESC2", Type = ReturnType.String)]
            public string CategoryDesc2 { get; set; }
            /// <summary>
            /// 分類 3。
            /// </summary>
            [SchemaMapping(Name = "CATEGORY_DESC3", Type = ReturnType.String)]
            public string CategoryDesc3 { get; set; }
            /// <summary>
            /// 分類 4。
            /// </summary>
            [SchemaMapping(Name = "CATEGORY_DESC4", Type = ReturnType.String)]
            public string CategoryDesc4 { get; set; }
            /// <summary>
            /// 分類 5。
            /// </summary>
            [SchemaMapping(Name = "CATEGORY_DESC5", Type = ReturnType.String)]
            public string CategoryDesc5 { get; set; }
            /// <summary>
            /// 分類 6。
            /// </summary>
            [SchemaMapping(Name = "CATEGORY_DESC6", Type = ReturnType.String)]
            public string CategoryDesc6 { get; set; }
            /// <summary>
            /// 分類 7。
            /// </summary>
            [SchemaMapping(Name = "CATEGORY_DESC7", Type = ReturnType.String)]
            public string CategoryDesc7 { get; set; }
            /// <summary>
            /// 分類 8。
            /// </summary>
            [SchemaMapping(Name = "CATEGORY_DESC8", Type = ReturnType.String)]
            public string CategoryDesc8 { get; set; }
            /// <summary>
            /// 分類 9。
            /// </summary>
            [SchemaMapping(Name = "CATEGORY_DESC9", Type = ReturnType.String)]
            public string CategoryDesc9 { get; set; }
            /// <summary>
            /// 分類 10。
            /// </summary>
            [SchemaMapping(Name = "CATEGORY_DESC10", Type = ReturnType.String)]
            public string CategoryDesc10 { get; set; }
            /// <summary>
            /// 分類 11。
            /// </summary>
            [SchemaMapping(Name = "CATEGORY_DESC11", Type = ReturnType.String)]
            public string CategoryDesc11 { get; set; }
            /// <summary>
            /// 分類 12。
            /// </summary>
            [SchemaMapping(Name = "CATEGORY_DESC12", Type = ReturnType.String)]
            public string CategoryDesc12 { get; set; }
            /// <summary>
            /// 分類 13。
            /// </summary>
            [SchemaMapping(Name = "CATEGORY_DESC13", Type = ReturnType.String)]
            public string CategoryDesc13 { get; set; }
            /// <summary>
            /// 分類 14。
            /// </summary>
            [SchemaMapping(Name = "CATEGORY_DESC14", Type = ReturnType.String)]
            public string CategoryDesc14 { get; set; }
            /// <summary>
            /// 分類 15。
            /// </summary>
            [SchemaMapping(Name = "CATEGORY_DESC15", Type = ReturnType.String)]
            public string CategoryDesc15 { get; set; }
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
        /// <param name="exportItemType">產品別。</param>
        /// <param name="categoryDesc1">分類 1（null 或 empty 將自動略過操作）。</param>
        /// <param name="categoryDesc2">分類 2（null 或 empty 將自動略過操作）。</param>
        /// <param name="categoryDesc3">分類 3（null 或 empty 將自動略過操作）。</param>
        /// <param name="categoryDesc4">分類 4（null 或 empty 將自動略過操作）。</param>
        /// <param name="categoryDesc5">分類 5（null 或 empty 將自動略過操作）。</param>
        /// <param name="categoryDesc6">分類 6（null 或 empty 將自動略過操作）。</param>
        /// <param name="categoryDesc7">分類 7（null 或 empty 將自動略過操作）。</param>
        /// <param name="categoryDesc8">分類 8（null 或 empty 將自動略過操作）。</param>
        /// <param name="categoryDesc9">分類 9（null 或 empty 將自動略過操作）。</param>
        /// <param name="categoryDesc10">分類 10（null 或 empty 將自動略過操作）。</param>
        /// <param name="categoryDesc11">分類 11（null 或 empty 將自動略過操作）。</param>
        /// <param name="categoryDesc12">分類 12（null 或 empty 將自動略過操作）。</param>
        /// <param name="categoryDesc13">分類 13（null 或 empty 將自動略過操作）。</param>
        /// <param name="categoryDesc14">分類 14（null 或 empty 將自動略過操作）。</param>
        /// <param name="categoryDesc15">分類 15（null 或 empty 將自動略過操作）。</param>
        /// <param name="activeFlag">是否作用中。</param>
        /// <param name="createionDate">建立時間。</param>
        /// <param name="createdBy">建立人。</param>
        /// <param name="lastUpdatedDate">修改時間（null 將自動略過操作）。</param>
        /// <param name="lastUpdatedBy">修改人（null 將自動略過操作）。</param>
        /// <returns>EzCoding.Returner。</returns>
        /// <exception cref="System.ArgumentNullException">參數 exportItemType 不允許為 null 值。</exception>
        public Returner Add(string exportItemType, string categoryDesc1, string categoryDesc2, string categoryDesc3, string categoryDesc4, string categoryDesc5, string categoryDesc6, string categoryDesc7, string categoryDesc8, string categoryDesc9, string categoryDesc10, string categoryDesc11, string categoryDesc12, string categoryDesc13, string categoryDesc14, string categoryDesc15, string activeFlag, DateTime createionDate, ISystemId createdBy, DateTime? lastUpdatedDate, ISystemId lastUpdatedBy)
        {
            if (exportItemType == null) { throw new ArgumentNullException("exportItemType"); }

            Returner returner = new Returner();
            try
            {
                #region 交易緩衝區
                TransColValSet transSet = new TransColValSet();
                transSet.SmartAdd("EXPORT_ITEM_TYPE", exportItemType, GenericDBType.NVarChar, false);
                transSet.SmartAdd("CATEGORY_DESC1", categoryDesc1, GenericDBType.NVarChar, true);
                transSet.SmartAdd("CATEGORY_DESC2", categoryDesc2, GenericDBType.NVarChar, true);
                transSet.SmartAdd("CATEGORY_DESC3", categoryDesc3, GenericDBType.NVarChar, true);
                transSet.SmartAdd("CATEGORY_DESC4", categoryDesc4, GenericDBType.NVarChar, true);
                transSet.SmartAdd("CATEGORY_DESC5", categoryDesc5, GenericDBType.NVarChar, true);
                transSet.SmartAdd("CATEGORY_DESC6", categoryDesc6, GenericDBType.NVarChar, true);
                transSet.SmartAdd("CATEGORY_DESC7", categoryDesc7, GenericDBType.NVarChar, true);
                transSet.SmartAdd("CATEGORY_DESC8", categoryDesc8, GenericDBType.NVarChar, true);
                transSet.SmartAdd("CATEGORY_DESC9", categoryDesc9, GenericDBType.NVarChar, true);
                transSet.SmartAdd("CATEGORY_DESC10", categoryDesc10, GenericDBType.NVarChar, true);
                transSet.SmartAdd("CATEGORY_DESC11", categoryDesc11, GenericDBType.NVarChar, true);
                transSet.SmartAdd("CATEGORY_DESC12", categoryDesc12, GenericDBType.NVarChar, true);
                transSet.SmartAdd("CATEGORY_DESC13", categoryDesc13, GenericDBType.NVarChar, true);
                transSet.SmartAdd("CATEGORY_DESC14", categoryDesc14, GenericDBType.NVarChar, true);
                transSet.SmartAdd("CATEGORY_DESC15", categoryDesc15, GenericDBType.NVarChar, true);
                transSet.SmartAdd("ACTIVE_FLAG", activeFlag, GenericDBType.Char, false);
                transSet.SmartAdd("CREATEION_DATE", createionDate, "yyyyMMddHHmmss", GenericDBType.Char);
                transSet.SmartAdd("CREATED_BY", createdBy.ToString() , GenericDBType.Char,false);
                transSet.SmartAdd("LAST_UPDATED_DATE", lastUpdatedDate, "yyyyMMddHHmmss", GenericDBType.Char);
                transSet.SmartAdd("LAST_UPDATED_BY", lastUpdatedBy.ToString(), GenericDBType.Char,false);

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
        /// 依指定的參數，修改一筆 外銷型號分類。
        /// </summary>
        /// <param name="exportItemType">產品別。</param>
        /// <param name="categoryDesc1">分類 1（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="categoryDesc2">分類 2（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="categoryDesc3">分類 3（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="categoryDesc4">分類 4（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="categoryDesc5">分類 5（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="categoryDesc6">分類 6（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="categoryDesc7">分類 7（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="categoryDesc8">分類 8（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="categoryDesc9">分類 9（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="categoryDesc10">分類 10（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="categoryDesc11">分類 11（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="categoryDesc12">分類 12（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="categoryDesc13">分類 13（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="categoryDesc14">分類 14（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="categoryDesc15">分類 15（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="activeFlag">是否作用中。</param>
        /// <param name="createionDate">建立時間。</param>
        /// <param name="createdBy">建立人。</param>
        /// <param name="lastUpdatedDate">修改時間（null 則直接設為 DBNull）。</param>
        /// <param name="lastUpdatedBy">修改人（null 則直接設為 DBNull）。</param>
        /// <returns>EzCoding.Returner。</returns>
        /// <exception cref="System.ArgumentNullException">參數 exportItemType 不允許為 null 值。</exception>
        public Returner Modify(string exportItemType, string categoryDesc1, string categoryDesc2, string categoryDesc3, string categoryDesc4, string categoryDesc5, string categoryDesc6, string categoryDesc7, string categoryDesc8, string categoryDesc9, string categoryDesc10, string categoryDesc11, string categoryDesc12, string categoryDesc13, string categoryDesc14, string categoryDesc15, string activeFlag, DateTime createionDate, ISystemId createdBy, DateTime? lastUpdatedDate, ISystemId lastUpdatedBy)
        {
            if (exportItemType == null) { throw new ArgumentNullException("exportItemType"); }

            Returner returner = new Returner();
            try
            {
                #region 交易緩衝區
                TransColValSet transSet = new TransColValSet();
                transSet.SmartAdd("CATEGORY_DESC1", categoryDesc1, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("CATEGORY_DESC2", categoryDesc2, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("CATEGORY_DESC3", categoryDesc3, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("CATEGORY_DESC4", categoryDesc4, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("CATEGORY_DESC5", categoryDesc5, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("CATEGORY_DESC6", categoryDesc6, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("CATEGORY_DESC7", categoryDesc7, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("CATEGORY_DESC8", categoryDesc8, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("CATEGORY_DESC9", categoryDesc9, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("CATEGORY_DESC10", categoryDesc10, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("CATEGORY_DESC11", categoryDesc11, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("CATEGORY_DESC12", categoryDesc12, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("CATEGORY_DESC13", categoryDesc13, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("CATEGORY_DESC14", categoryDesc14, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("CATEGORY_DESC15", categoryDesc15, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("ACTIVE_FLAG", activeFlag, GenericDBType.Char, false);
                transSet.SmartAdd("CREATEION_DATE", createionDate, "yyyyMMddHHmmss", GenericDBType.Char);
                transSet.SmartAdd("CREATED_BY", createdBy.ToString(), GenericDBType.Char,false);
                transSet.SmartAdd("LAST_UPDATED_DATE", lastUpdatedDate, "yyyyMMddHHmmss", GenericDBType.Char, true);
                transSet.SmartAdd("LAST_UPDATED_BY", lastUpdatedBy.ToString(), GenericDBType.Char, false);

                SqlCondsSet condsMain = new SqlCondsSet();
                condsMain.Add("EXPORT_ITEM_TYPE", SqlCond.EqualTo, exportItemType, GenericDBType.NVarChar, SqlCondsSet.And);

                returner.ChangeInto(base.Modify(transSet, condsMain, true));
                #endregion

                return returner;
            }
            finally
            {
            }
        }
        #endregion

        #region Modify-for 產品別更新
        /// <summary>
        /// 依指定的參數，修改一筆 外銷型號分類。
        /// </summary>
        /// <param name="exportItemType">產品別。</param>
        /// <param name="categoryDesc1">分類 1（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="categoryDesc2">分類 2（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="categoryDesc3">分類 3（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="categoryDesc4">分類 4（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="categoryDesc5">分類 5（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="categoryDesc6">分類 6（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="categoryDesc7">分類 7（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="categoryDesc8">分類 8（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="categoryDesc9">分類 9（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="categoryDesc10">分類 10（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="categoryDesc11">分類 11（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="categoryDesc12">分類 12（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="activeFlag">是否作用中。</param>
        /// <param name="lastUpdatedDate">修改時間（null 則直接設為 DBNull）。</param>
        /// <param name="lastUpdatedBy">修改人（null 則直接設為 DBNull）。</param>
        /// <returns>EzCoding.Returner。</returns>
        public Returner Modify(string exportItemType, string categoryDesc1, string categoryDesc2, string categoryDesc3, string categoryDesc4, string categoryDesc5, string categoryDesc6, string categoryDesc7, string categoryDesc8, string categoryDesc9, string categoryDesc10, string categoryDesc11, string categoryDesc12, string activeFlag,DateTime lastUpdatedDate, ISystemId lastUpdatedBy)
        {
            Returner returner = new Returner();
            try
            {
                #region 交易緩衝區
                TransColValSet transSet = new TransColValSet();
                transSet.SmartAdd("CATEGORY_DESC1", categoryDesc1, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("CATEGORY_DESC2", categoryDesc2, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("CATEGORY_DESC3", categoryDesc3, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("CATEGORY_DESC4", categoryDesc4, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("CATEGORY_DESC5", categoryDesc5, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("CATEGORY_DESC6", categoryDesc6, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("CATEGORY_DESC7", categoryDesc7, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("CATEGORY_DESC8", categoryDesc8, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("CATEGORY_DESC9", categoryDesc9, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("CATEGORY_DESC10", categoryDesc10, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("CATEGORY_DESC11", categoryDesc11, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("CATEGORY_DESC12", categoryDesc12, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("ACTIVE_FLAG", activeFlag, GenericDBType.Char, false);
                transSet.SmartAdd("LAST_UPDATED_DATE", lastUpdatedDate, "yyyyMMddHHmmss", GenericDBType.Char, true);
                transSet.SmartAdd("LAST_UPDATED_BY", lastUpdatedBy.ToString(), GenericDBType.Char, false);

                SqlCondsSet condsMain = new SqlCondsSet();
                condsMain.Add("EXPORT_ITEM_TYPE", SqlCond.EqualTo, exportItemType, GenericDBType.NVarChar, SqlCondsSet.And);

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
            /// <param name="exportItemTypes">產品別陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="activeFlag">是否作用中（若為 null 則略過條件檢查）。</param>
            public InfoConds(string[] exportItemTypes, bool? activeFlag)
            {
                this.ExportItemTypes = exportItemTypes;
                this.ActiveFlag = activeFlag;
            }

            /// <summary>
            /// 產品別陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public string[] ExportItemTypes { get; set; }
            /// <summary>
            /// 是否作用中（若為 null 則略過條件檢查）。
            /// </summary>
            public bool? ActiveFlag { get; set; }
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

            custEntity.Append("SELECT [EXT_ITEM_TYPE].* ");
            custEntity.Append("FROM [EXT_ITEM_TYPE] ");

            var sqlConds = new List<string>();

            if (qConds.ExportItemTypes != null && qConds.ExportItemTypes.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "EXPORT_ITEM_TYPE", SqlOperator.EqualTo, qConds.ExportItemTypes, GenericDBType.NVarChar));
            }

            if (qConds.ActiveFlag.HasValue)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "ACTIVE_FLAG", SqlOperator.EqualTo, qConds.ActiveFlag.Value ? "Y" : "N", GenericDBType.Char));
            }

            custEntity.Append(SqlUtilBox.ToSqlConds(sqlConds.Where(q => !string.IsNullOrWhiteSpace(q)).ToArray(), " WHERE ", string.Empty));
            base.Entity.CustomEntitySqlSyntax = custEntity.GetSqlSyntax();
            #endregion

            SqlCondsSet condsMain = new SqlCondsSet();

            return condsMain;
        }
        #endregion
        #endregion
    }
}
