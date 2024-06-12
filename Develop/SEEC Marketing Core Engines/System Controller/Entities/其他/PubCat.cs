using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Globalization;

using EzCoding;
using EzCoding.DB;
using EzCoding.SystemEngines;
using EzCoding.SystemEngines.EntityController;

namespace Seec.Marketing.SystemEngines
{
    public partial class DBTableDefine
    {
        /// <summary>
        /// 通用分類。
        /// </summary>
        public const string PUB_CAT = "PUB_CAT";
    }

    /// <summary>
    /// 通用分類的類別實作。
    /// </summary>
    public class PubCat : SystemHandler
    {
        /// <summary>
        /// 初始化 Seec.Marketing.SystemEngines.PubCat 類別的新執行個體。
        /// 預設使用的 Primary Key 欄位名稱為「SID」及 EzCoding.SystemIdVerifier 系統代號驗證物件。
        /// </summary>
        /// <param name="connInfo">資料庫連線資訊。</param>
        public PubCat(string connInfo)
            : base(EntityHelper.GetEntity(connInfo, DBTableDefine.PUB_CAT))
        {
            SqlOrder sorting = new SqlOrder();
            sorting.Add("SORT", Sort.Descending);

            base.DefaultSqlOrder = sorting;
        }

        #region 表格資料
        /// <summary>
        /// 表格資料。
        /// </summary>
        public class Info : InfoBase
        {
            /// <summary>
            /// 系統代號。
            /// </summary>
            [SchemaMapping(Name = "SID", Type = ReturnType.SId)]
            public ISystemId SId { get; set; }
            /// <summary>
            /// 建立日期時間。
            /// </summary>
            [SchemaMapping(Name = "CDT", Type = ReturnType.DateTime)]
            public DateTime Cdt { get; set; }
            /// <summary>
            /// 修改日期時間。
            /// </summary>
            [SchemaMapping(Name = "MDT", Type = ReturnType.DateTime)]
            public DateTime Mdt { get; set; }
            /// <summary>
            /// 建立人。
            /// </summary>
            [SchemaMapping(Name = "CSID", Type = ReturnType.SId)]
            public ISystemId CSId { get; set; }
            /// <summary>
            /// 修改人。
            /// </summary>
            [SchemaMapping(Name = "MSID", Type = ReturnType.SId)]
            public ISystemId MSId { get; set; }
            /// <summary>
            /// 刪除標記。
            /// </summary>
            [SchemaMapping(Name = "MDELED", Type = ReturnType.Bool)]
            public bool MDeled { get; set; }
            /// <summary>
            /// 啟用。
            /// </summary>
            [SchemaMapping(Name = "ENABLED", Type = ReturnType.Bool)]
            public bool Enabled { get; set; }
            /// <summary>
            /// 是否為預設資料。
            /// </summary>
            [SchemaMapping(Name = "IS_DEF", Type = ReturnType.Bool)]
            public bool IsDef { get; set; }
            /// <summary>
            /// 使用對象（1:內銷地區 2:內銷貨運方式 3:外銷貨運方式）。
            /// </summary>
            [SchemaMapping(Name = "USE_TGT", Type = ReturnType.Int)]
            public int UseTgt { get; set; }
            /// <summary>
            /// 父單元系統代號。
            /// </summary>
            [SchemaMapping(Name = "PUNIT_SID", Type = ReturnType.SId, AllowNull = true)]
            public ISystemId PunitSId { get; set; }
            /// <summary>
            /// 上層節點系統代號。
            /// </summary>
            [SchemaMapping(Name = "PARENT_SID", Type = ReturnType.SId, AllowNull = true)]
            public ISystemId ParentSId { get; set; }
            /// <summary>
            /// 代碼。
            /// </summary>
            [SchemaMapping(Name = "CODE", Type = ReturnType.String)]
            public string Code { get; set; }
            /// <summary>
            /// 名稱。
            /// </summary>
            [SchemaMapping(Name = "NAME", Type = ReturnType.String)]
            public string Name { get; set; }
            /// <summary>
            /// 內容標題。
            /// </summary>
            [SchemaMapping(Name = "CONT_TITLE", Type = ReturnType.String)]
            public string ContTitle { get; set; }
            /// <summary>
            /// 內容。
            /// </summary>
            [SchemaMapping(Name = "CONT", Type = ReturnType.String)]
            public string Cont { get; set; }
            /// <summary>
            /// 說明。
            /// </summary>
            [SchemaMapping(Name = "DESC", Type = ReturnType.String)]
            public string Desc { get; set; }
            /// <summary>
            /// 最大階層數。
            /// </summary>
            [SchemaMapping(Name = "MAX_LAYER", Type = ReturnType.Int, AllowNull = true)]
            public int? MaxLayer { get; set; }
            /// <summary>
            /// 自訂欄位。
            /// </summary>
            [SchemaMapping(Name = "CUST_FIELD", Type = ReturnType.String)]
            public string CustField { get; set; }
            /// <summary>
            /// 排序。
            /// </summary>
            [SchemaMapping(Name = "SORT", Type = ReturnType.Int)]
            public int Sort { get; set; }

            /// <summary>
            /// 以 1 為起的分類階層。
            /// </summary>
            [SchemaMapping(Name = "LAYER", Type = ReturnType.Int)]
            public int Layer { get; set; }

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

            /// <summary>
            /// 繫結資料。
            /// </summary>
            public static Info Binding(InputInfo input)
            {
                return DBUtilBox.BindingInput<Info>(new Info(), input);
            }

            /// <summary>
            /// 繫結資料。
            /// </summary>
            public static Info[] Binding(InputInfo[] inputs)
            {
                List<Info> infos = new List<Info>();
                foreach (var input in inputs)
                {
                    infos.Add(Info.Binding(input));
                }

                return infos.ToArray();
            }
            #endregion
        }
        #endregion

        #region 輸入資訊（驗證用）
        /// <summary>
        /// 輸入資訊（驗證用）。
        /// </summary>
        public class InputInfo
        {
            /// <summary>
            /// 系統代號。
            /// </summary>
            public ISystemId SId { get; set; }
            /// <summary>
            /// 建立日期時間。
            /// </summary>
            public DateTime? Cdt { get; set; }
            /// <summary>
            /// 修改日期時間。
            /// </summary>
            public DateTime? Mdt { get; set; }
            /// <summary>
            /// 建立人。
            /// </summary>
            public ISystemId CSId { get; set; }
            /// <summary>
            /// 修改人。
            /// </summary>
            public ISystemId MSId { get; set; }
            /// <summary>
            /// 刪除標記。
            /// </summary>
            public bool? MDeled { get; set; }
            /// <summary>
            /// 啟用。
            /// </summary>
            public bool? Enabled { get; set; }
            /// <summary>
            /// 是否為預設資料。
            /// </summary>
            public bool? IsDef { get; set; }
            /// <summary>
            /// 使用對象（1:內銷地區 2:內銷貨運方式 3:外銷貨運方式）。
            /// </summary>
            public int? UseTgt { get; set; }
            /// <summary>
            /// 父單元系統代號。
            /// </summary>
            public ISystemId PunitSId { get; set; }
            /// <summary>
            /// 上層節點系統代號。
            /// </summary>
            public ISystemId ParentSId { get; set; }
            /// <summary>
            /// 代碼。
            /// </summary>
            public string Code { get; set; }
            /// <summary>
            /// 名稱。
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// 內容標題。
            /// </summary>
            public string ContTitle { get; set; }
            /// <summary>
            /// 內容。
            /// </summary>
            public string Cont { get; set; }
            /// <summary>
            /// 說明。
            /// </summary>
            public string Desc { get; set; }
            /// <summary>
            /// 最大階層數。
            /// </summary>
            public int? MaxLayer { get; set; }
            /// <summary>
            /// 自訂欄位。
            /// </summary>
            public string CustField { get; set; }
            /// <summary>
            /// 排序。
            /// </summary>
            public int? Sort { get; set; }
        }
        #endregion

        #region 自訂欄位
        /// <summary>
        /// 自訂欄位。
        /// </summary>
        public class CustField
        {
            #region 內銷地區
            /// <summary>
            /// 內銷地區。
            /// </summary>
            public static class DomDist
            {
                /// <summary>
                /// 公司名稱。
                /// </summary>
                public const string CompName = "CompName";
                /// <summary>
                /// 電話。
                /// </summary>
                public const string Tel = "Tel";
                /// <summary>
                /// 傳真。
                /// </summary>
                public const string Fax = "Fax";
                /// <summary>
                /// 地址。
                /// </summary>
                public const string Addr = "Addr";
            }
            #endregion

            #region CustField
            /// <summary>
            /// 初始化 CustField 類別的新執行個體。
            /// </summary>
            public CustField()
            {
                this.Name = string.Empty;
                this.Value = string.Empty;
            }

            /// <summary>
            /// 初始化 CustField 類別的新執行個體。
            /// </summary>
            /// <param name="name">欄位名稱。</param>
            /// <param name="value">欄位值。</param>
            public CustField(string name, string value)
            {
                this.Name = name;
                this.Value = value;
            }

            /// <summary>
            /// 欄位名稱。
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// 欄位值。
            /// </summary>
            public string Value { get; set; }
            #endregion
        }
        #endregion

        #region 使用對象列舉
        /// <summary>
        /// 使用對象列舉。
        /// </summary>
        public enum UseTgtOpt
        {
            /// <summary>
            /// 內銷地區。
            /// </summary>
            DomDist = 1,
            /// <summary>
            /// 內銷貨運方式。
            /// </summary>
            DomFreightWay = 2,
            /// <summary>
            /// 外銷貨運方式。
            /// </summary>
            ExtFreightWay = 3
        }
        #endregion

        #region 異動
        #region Add
        /// <summary>
        /// 依指定的參數，新增一筆資料。
        /// </summary>
        /// <param name="actorSId">操作人系統代號。為客戶端在操作時的操作人系統代號。</param> 
        /// <param name="pubCatSId">指定的通用分類系統代號。</param>
        /// <param name="enabled">資料的啟用狀態。</param>
        /// <param name="useTgt">使用對象（1:內銷地區 2:內銷貨運方式 3:外銷貨運方式）。</param>
        /// <param name="punitSId">父單元系統代號（null 將自動略過操作）。</param>
        /// <param name="parentSId">上層節點系統代號（null 表示最上層節點）。</param>
        /// <param name="code">代碼（null 或 empty 將自動略過操作）。</param>
        /// <param name="name">名稱。</param>
        /// <param name="contTitle">內容標題（null 或 empty 將自動略過操作）。</param>
        /// <param name="cont">內容（null 或 empty 將自動略過操作）。</param>
        /// <param name="desc">說明（null 或 empty 將自動略過操作）。</param>
        /// <param name="maxLayer">最大階層數（null 將自動略過操作）。</param>
        /// <param name="custField">自訂欄位（null 或 empty 將自動略過操作）。</param>
        /// <param name="sort">排序。</param>
        /// <returns>EzCoding.Returner。若新增成功則回傳時包含著一個「NEW_SID」的 DataTable 和「TABLE_NAME」、「SID」兩個欄位。</returns>
        /// <exception cref="System.ArgumentNullException">參數 actorSId 不允許為 null 值。</exception>
        /// <exception cref="System.ArgumentNullException">參數 pubCatSId 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        public Returner Add(ISystemId actorSId, ISystemId pubCatSId, bool enabled, int useTgt, ISystemId punitSId, ISystemId parentSId, string code, string name, string contTitle, string cont, string desc, int? maxLayer, string custField, int sort)
        {
            if (actorSId == null) { throw new ArgumentNullException("actorSId"); }
            if (pubCatSId == null) { throw new ArgumentNullException("pubCatSId"); }
            string[] exceptionSIds = this.SystemIdVerifier.CheckValid(actorSId, pubCatSId);
            if (exceptionSIds.Length > 0)
            {
                throw new LibraryException(string.Format("invaild ISystemId ({0}) format.", string.Join("、", exceptionSIds)));
            }

            Returner returner = new Returner();
            try
            {
                #region 交易緩衝區
                TransColValSet transSet = new TransColValSet();
                transSet.SmartAdd("SID", pubCatSId.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("CSID", actorSId.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("MSID", actorSId.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("ENABLED", enabled ? "Y" : "N", GenericDBType.Char, false);
                transSet.SmartAdd("USE_TGT", useTgt, GenericDBType.Int);
                transSet.SmartAdd("PUNIT_SID", punitSId, base.SystemIdVerifier, GenericDBType.Char, true);
                transSet.SmartAdd("PARENT_SID", parentSId, base.SystemIdVerifier, GenericDBType.Char, true);
                transSet.SmartAdd("CODE", code, GenericDBType.VarChar, true);
                transSet.SmartAdd("NAME", name, GenericDBType.NVarChar, false);
                transSet.SmartAdd("CONT_TITLE", contTitle, GenericDBType.NVarChar, true);
                transSet.SmartAdd("CONT", cont, GenericDBType.NVarChar, true);
                transSet.SmartAdd("DESC", desc, GenericDBType.NVarChar, true);
                transSet.SmartAdd("MAX_LAYER", maxLayer, GenericDBType.Int);
                transSet.SmartAdd("CUST_FIELD", custField, GenericDBType.NVarChar, true);
                transSet.SmartAdd("SORT", sort, GenericDBType.Int);

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
        /// 依指定的參數，修改一筆通用分類。
        /// </summary>
        /// <param name="actorSId">操作人系統代號。為客戶端在操作時的操作人系統代號。</param>
        /// <param name="pubCatSId">通用分類系統代號。</param>
        /// <param name="enabled">資料的啟用狀態。</param>
        /// <param name="parentSId">上層節點系統代號（null 表示移動到最上層節點）。</param>
        /// <param name="code">代碼（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="name">名稱。</param>
        /// <param name="contTitle">內容標題（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="cont">內容（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="desc">說明（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="maxLayer">最大階層數（null 則直接設為 DBNull）。</param>
        /// <param name="custField">自訂欄位（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="sort">排序。</param>
        /// <returns>EzCoding.Returner。</returns>
        /// <exception cref="System.ArgumentNullException">參數 actorSId 不允許為 null 值。</exception>
        /// <exception cref="System.ArgumentNullException">參數 pubCatSId 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        public Returner Modify(ISystemId actorSId, ISystemId pubCatSId, bool enabled, ISystemId parentSId, string code, string name, string contTitle, string cont, string desc, int? maxLayer, string custField, int sort)
        {
            if (actorSId == null) { throw new ArgumentNullException("actorSId"); }
            if (pubCatSId == null) { throw new ArgumentNullException("pubCatSId"); }
            string[] exceptionSIds = this.SystemIdVerifier.CheckValid(actorSId, pubCatSId);
            if (exceptionSIds.Length > 0)
            {
                throw new LibraryException(string.Format("invaild ISystemId ({0}) format.", string.Join("、", exceptionSIds)));
            }

            Returner returner = new Returner();
            try
            {
                #region 交易緩衝區
                TransColValSet transSet = new TransColValSet();
                transSet.SmartAdd("MDT", DateTime.Now, "yyyyMMddHHmmss", GenericDBType.Char);
                transSet.SmartAdd("MSID", actorSId.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("ENABLED", enabled ? "Y" : "N", GenericDBType.Char, false);
                transSet.SmartAdd("PARENT_SID", parentSId, base.SystemIdVerifier, GenericDBType.Char, true, true);
                transSet.SmartAdd("CODE", code, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("NAME", name, GenericDBType.NVarChar, false);
                transSet.SmartAdd("CONT_TITLE", contTitle, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("CONT", cont, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("DESC", desc, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("MAX_LAYER", maxLayer, GenericDBType.Int, true);
                transSet.SmartAdd("CUST_FIELD", custField, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("SORT", sort, GenericDBType.Int);

                SqlCondsSet condsMain = new SqlCondsSet();
                condsMain.Add(base.PrimaryKey, SqlCond.EqualTo, pubCatSId.ToString(), GenericDBType.Char, string.Empty);

                returner.ChangeInto(base.Modify(transSet, condsMain, true));
                #endregion

                return returner;
            }
            finally
            {
            }
        }
        #endregion

        #region UpdateCustSort
        /// <summary>
        /// 更新自訂排序。
        /// </summary>
        /// <param name="pubCatSId">通用分類系統代號。</param> 
        /// <param name="val">排序。</param>
        /// <returns>EzCoding.Returner。</returns>
        /// <exception cref="System.ArgumentNullException">參數 pubCatSId 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        public Returner UpdateCustSort(ISystemId pubCatSId, int val)
        {
            if (pubCatSId == null) { throw new ArgumentNullException("pubCatSId"); }
            string[] exceptionSIds = this.SystemIdVerifier.CheckValid(pubCatSId);
            if (exceptionSIds.Length > 0)
            {
                throw new LibraryException(string.Format("invaild ISystemId ({0}) format.", string.Join("、", exceptionSIds)));
            }

            Returner returner = new Returner();
            try
            {
                #region 交易緩衝區
                TransColValSet transSet = new TransColValSet();
                transSet.SmartAdd("SORT", val, GenericDBType.Int);

                SqlCondsSet condsMain = new SqlCondsSet();
                condsMain.Add(base.PrimaryKey, SqlCond.EqualTo, pubCatSId.ToString(), GenericDBType.Char, string.Empty);

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
            /// <param name="pubCatSIds">通用分類系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param> 
            /// <param name="useTgts">使用對象陣列集合（1:內銷地區 2:內銷貨運方式 3:外銷貨運方式; 若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="punitSId">父單元系統代號（若為 null 則略過條件檢查）。</param>
            /// <param name="parentSId">上層節點系統代號（若為 null 則略過條件檢查）。</param>
            /// <param name="code">代碼（若為 null 或 empty 則略過條件檢查）。</param>
            public InfoConds(ISystemId[] pubCatSIds, int[] useTgts, ISystemId punitSId, ISystemId parentSId, string code)
            {
                this.PubCatSIds = pubCatSIds;
                this.UseTgts = useTgts;
                this.PunitSId = punitSId;
                this.ParentSId = parentSId;
                this.Code = code;
            }

            /// <summary>
            /// 通用分類系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public ISystemId[] PubCatSIds { get; set; }
            /// <summary>
            /// 使用對象陣列集合（1:內銷地區 2:內銷貨運方式 3:外銷貨運方式; 若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public int[] UseTgts { get; set; }
            /// <summary>
            /// 父單元系統代號（若為 null 則略過條件檢查）。
            /// </summary>
            public ISystemId PunitSId { get; set; }
            /// <summary>
            /// 上層節點系統代號（若為 null 則略過條件檢查）。
            /// </summary>
            public ISystemId ParentSId { get; set; }
            /// <summary>
            /// 代碼（若為 null 或 empty 則略過條件檢查）。
            /// </summary>
            public string Code { get; set; }
        }
        #endregion

        #region GetInfo
        /// <summary> 
        /// 依指定的參數取得資料。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <param name="size">指定的前 N 筆資料（若為 Int32.MaxValue 則表示全部）。</param>
        /// <param name="sqlOrder">SQL 排序（Order By）的組合類別（若為 null 則使用預設排序）。</param>
        /// <param name="includeScope">資料取得所包含的範圍（是否註解刪除或啟用中）。</param>
        /// <param name="inquireColumns">含有零或多個指定查詢時所要使用的欄位名稱 System.String 陣列。若陣列為零，則使用預設開放的欄位名稱。</param>
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetInfo(InfoConds qConds, int size, SqlOrder sqlOrder, IncludeScope includeScope, params string[] inquireColumns)
        {
            Returner returner = new Returner();

            SqlCondsSet condsMain = Handier.MergeDefaultCondsSet(includeScope);
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
        /// <param name="includeScope">資料取得所包含的範圍（是否註解刪除或啟用中）。</param>
        /// <param name="inquireColumns">含有零或多個指定查詢時所要使用的欄位名稱 System.String 陣列。若陣列為零，則使用預設開放的欄位名稱。</param>
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetInfo(InfoConds qConds, PagingFlipper flipper, SqlOrder sqlOrder, IncludeScope includeScope, params string[] inquireColumns)
        {
            Returner returner = new Returner();

            SqlCondsSet condsMain = Handier.MergeDefaultCondsSet(includeScope);
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
        /// <param name="includeScope">資料取得所包含的範圍（是否註解刪除或啟用中）。</param> 
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetInfoCount(InfoConds qConds, IncludeScope includeScope)
        {
            Returner returner = new Returner();

            SqlCondsSet condsMain = Handier.MergeDefaultCondsSet(includeScope);
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
        /// <param name="includeScope">資料取得所包含的範圍（是否註解刪除或啟用中）。</param>
        /// <param name="inquireColumns">含有零或多個指定查詢時所要使用的欄位名稱 System.String 陣列。若陣列為零，則使用預設開放的欄位名稱。</param>
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetInfoByCompoundSearch(InfoConds qConds, string[] columnsName, string keyword, bool isAbsolute, PagingFlipper flipper, SqlOrder sqlOrder, IncludeScope includeScope, params string[] inquireColumns)
        {
            Returner returner = new Returner();

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.GetInfoByCompoundSearch(columnsName, keyword, EntityHelper.KEYWORD_SEPARATOR, isAbsolute, flipper, sqlOrder, GetInfoCondsSet(qConds), includeScope, inquireColumns));
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
        /// <param name="includeScope">資料取得所包含的範圍（是否註解刪除或啟用中）。</param> 
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetInfoByCompoundSearchCount(InfoConds qConds, string[] columnsName, string keyword, bool isAbsolute, IncludeScope includeScope)
        {
            Returner returner = new Returner();

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.GetInfoByCompoundSearchCount(columnsName, keyword, EntityHelper.KEYWORD_SEPARATOR, isAbsolute, GetInfoCondsSet(qConds), includeScope));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetInfoCondsSet
        /// <summary> 
        /// 依指定的條件取得 SqlCondsSet。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <returns>EzCoding.Collections.SqlCondsSet。</returns> 
        private SqlCondsSet GetInfoCondsSet(InfoConds qConds)
        {
            #region 自訂表格
            var custEntity = new SqlSyntaxBuilder(base.Entity, new SqlSyntax());

            custEntity.Append("SELECT [PUB_CAT].* ");
            custEntity.Append("FROM [PUB_CAT] WITH (NOLOCK) ");

            var sqlConds = new List<string>();

            if (qConds.PubCatSIds != null && qConds.PubCatSIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "SID", SqlOperator.EqualTo, SystemId.ToString(qConds.PubCatSIds), GenericDBType.Char));
            }

            if (qConds.UseTgts != null && qConds.UseTgts.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "USE_TGT", SqlOperator.EqualTo, qConds.UseTgts.Select(q => (object)q).ToArray(), GenericDBType.Int));
            }

            if (qConds.PunitSId != null)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "PUNIT_SID", SqlOperator.EqualTo, qConds.PunitSId.Value, GenericDBType.Char));
            }

            if (qConds.ParentSId != null)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "PARENT_SID", SqlOperator.EqualTo, qConds.ParentSId.Value, GenericDBType.Char));
            }

            if (!string.IsNullOrWhiteSpace(qConds.Code))
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "CODE", SqlOperator.EqualTo, qConds.Code, GenericDBType.VarChar));
            }

            custEntity.Append(SqlUtilBox.ToSqlConds(sqlConds.Where(q => !string.IsNullOrWhiteSpace(q)).ToArray(), " WHERE ", string.Empty));
            base.Entity.CustomEntitySqlSyntax = custEntity.GetSqlSyntax();
            #endregion

            SqlCondsSet condsMain = new SqlCondsSet();

            return condsMain;
        }
        #endregion

        #region CheckEditModeForCode
        /// <summary>
        /// 檢查在編輯時，是否允許異動代碼。
        /// </summary>
        /// <param name="pubCatSId">通用分類系統代號（若為 null 則表示為新增模式）。</param> 
        /// <param name="useTgt">使用對象（1:內銷地區 2:內銷貨運方式 3:外銷貨運方式）。</param>
        /// <param name="codeNew">異動的代碼。</param>
        /// <param name="includeScope">資料取得所包含的範圍（是否註解刪除或啟用中）。</param>
        /// <returns>EzCoding.Returner。</returns>
        public bool CheckEditModeForCode(ISystemId pubCatSId, int useTgt, string codeNew, IncludeScope includeScope)
        {
            string codeOld = string.Empty;

            if (pubCatSId != null)
            {
                using (Returner returner = base.GetInfo(new ISystemId[] { pubCatSId }, includeScope, new string[] { "CODE" }))
                {
                    if (returner.IsCompletedAndContinue)
                    {
                        codeOld = returner.DataSet.Tables[0].Rows[0]["CODE"].ToString();
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            if (pubCatSId != null && string.Compare(codeOld, codeNew, StringComparison.OrdinalIgnoreCase) == 0)
            {
                //修改時與原值相同
                return true;
            }
            else
            {
                SqlCondsSet condsMain = new SqlCondsSet();
                condsMain.Add("USE_TGT", SqlCond.EqualTo, useTgt, GenericDBType.Int, SqlCondsSet.And);
                condsMain.Add("CODE", SqlCond.EqualTo, codeNew, GenericDBType.VarChar, SqlCondsSet.And);

                //新增時或修改時新舊值不相同
                return Convert.ToInt32(base.GetInfoCount(condsMain, includeScope).DataSet.Tables[0].Rows[0][0], CultureInfo.InvariantCulture) == 0;
            }
        }
        #endregion
        #endregion

        #region 最上層節點查詢
        #region 查詢條件
        /// <summary>
        /// 查詢條件。
        /// </summary>
        public class TopInfoConds
        {
            ///// <summary>
            ///// 初始化 TopInfoConds 類別的新執行個體。
            ///// 預設略過所有條件。
            ///// </summary>
            //public TopInfoConds()
            //{

            //}

            /// <summary>
            /// 初始化 TopInfoConds 類別的新執行個體。
            /// </summary>
            /// <param name="pubCatSIds">通用分類系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param> 
            /// <param name="useTgt">使用對象（1:內銷地區 2:內銷貨運方式 3:外銷貨運方式）。</param>
            /// <param name="punitSId">父單元系統代號（若為 null 則略過條件檢查）。</param>
            public TopInfoConds(ISystemId[] pubCatSIds, int useTgt, ISystemId punitSId)
            {
                this.PubCatSIds = pubCatSIds;
                this.UseTgt = useTgt;
                this.PunitSId = punitSId;
            }

            /// <summary>
            /// 通用分類系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public ISystemId[] PubCatSIds { get; set; }
            /// <summary>
            /// 使用對象（1:內銷地區 2:內銷貨運方式 3:外銷貨運方式）。
            /// </summary>
            public int UseTgt { get; set; }
            /// <summary>
            /// 父單元系統代號（若為 null 則略過條件檢查）。
            /// </summary>
            public ISystemId PunitSId { get; set; }
        }
        #endregion

        #region GetTopInfo
        /// <summary> 
        /// 依指定的參數取得資料。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <param name="size">指定的前 N 筆資料（若為 Int32.MaxValue 則表示全部）。</param>
        /// <param name="sqlOrder">SQL 排序（Order By）的組合類別（若為 null 則使用預設排序）。</param>
        /// <param name="includeScope">資料取得所包含的範圍（是否註解刪除或啟用中）。</param>
        /// <param name="inquireColumns">含有零或多個指定查詢時所要使用的欄位名稱 System.String 陣列。若陣列為零，則使用預設開放的欄位名稱。</param>
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetTopInfo(TopInfoConds qConds, int size, SqlOrder sqlOrder, IncludeScope includeScope, params string[] inquireColumns)
        {
            Returner returner = new Returner();

            SqlCondsSet condsMain = Handier.MergeDefaultCondsSet(includeScope);
            condsMain.Add(GetTopInfoCondsSet(qConds));

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
        /// <param name="includeScope">資料取得所包含的範圍（是否註解刪除或啟用中）。</param>
        /// <param name="inquireColumns">含有零或多個指定查詢時所要使用的欄位名稱 System.String 陣列。若陣列為零，則使用預設開放的欄位名稱。</param>
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetTopInfo(TopInfoConds qConds, PagingFlipper flipper, SqlOrder sqlOrder, IncludeScope includeScope, params string[] inquireColumns)
        {
            Returner returner = new Returner();

            SqlCondsSet condsMain = Handier.MergeDefaultCondsSet(includeScope);
            condsMain.Add(GetTopInfoCondsSet(qConds));

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.Entity.GetPagingBy(flipper, sqlOrder.UseDefault ? base.DefaultSqlOrder : sqlOrder, condsMain, inquireColumns));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetTopInfoCount
        /// <summary> 
        /// 依指定的參數取得資料總數。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <param name="includeScope">資料取得所包含的範圍（是否註解刪除或啟用中）。</param> 
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetTopInfoCount(TopInfoConds qConds, IncludeScope includeScope)
        {
            Returner returner = new Returner();

            SqlCondsSet condsMain = Handier.MergeDefaultCondsSet(includeScope);
            condsMain.Add(GetTopInfoCondsSet(qConds));

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.Entity.GetCountBy(condsMain));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetTopInfoByCompoundSearch
        /// <summary> 
        /// 依指定的參數取得資料（複合式查詢）。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <param name="columnsName">欄位名稱陣列。</param>
        /// <param name="keyword">關鍵字。</param>
        /// <param name="isAbsolute">是否為絕對查詢。若 false 則使用 like 查詢。</param>
        /// <param name="flipper">分頁操作參數集合。</param>
        /// <param name="sqlOrder">SQL 排序（Order By）的組合類別（若為 null 則使用預設排序）。</param>
        /// <param name="includeScope">資料取得所包含的範圍（是否註解刪除或啟用中）。</param>
        /// <param name="inquireColumns">含有零或多個指定查詢時所要使用的欄位名稱 System.String 陣列。若陣列為零，則使用預設開放的欄位名稱。</param>
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetTopInfoByCompoundSearch(TopInfoConds qConds, string[] columnsName, string keyword, bool isAbsolute, PagingFlipper flipper, SqlOrder sqlOrder, IncludeScope includeScope, params string[] inquireColumns)
        {
            Returner returner = new Returner();

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.GetInfoByCompoundSearch(columnsName, keyword, EntityHelper.KEYWORD_SEPARATOR, isAbsolute, flipper, sqlOrder, GetTopInfoCondsSet(qConds), includeScope, inquireColumns));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetTopInfoByCompoundSearchCount
        /// <summary> 
        /// 依指定的參數取得資料總數（複合式查詢）。 
        /// 若欄位名稱陣列長度等於0，則會略過指定欄位名稱查詢動作。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <param name="columnsName">欄位名稱陣列。</param> 
        /// <param name="keyword">關鍵字。</param> 
        /// <param name="isAbsolute">是否為絕對查詢。若 false 則使用 like 查詢。</param> 
        /// <param name="includeScope">資料取得所包含的範圍（是否註解刪除或啟用中）。</param> 
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetTopInfoByCompoundSearchCount(TopInfoConds qConds, string[] columnsName, string keyword, bool isAbsolute, IncludeScope includeScope)
        {
            Returner returner = new Returner();

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.GetInfoByCompoundSearchCount(columnsName, keyword, EntityHelper.KEYWORD_SEPARATOR, isAbsolute, GetTopInfoCondsSet(qConds), includeScope));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetTopInfoCondsSet
        /// <summary> 
        /// 依指定的條件取得 SqlCondsSet。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <returns>EzCoding.Collections.SqlCondsSet。</returns> 
        private SqlCondsSet GetTopInfoCondsSet(TopInfoConds qConds)
        {
            #region 自訂表格
            var custEntity = new SqlSyntaxBuilder(base.Entity, new SqlSyntax());

            custEntity.Append("SELECT [PUB_CAT].* ");
            custEntity.Append("FROM [PUB_CAT] WITH (NOLOCK) ");
            custEntity.Append("WHERE [PUB_CAT].[PARENT_SID] IS NULL ");

            var sqlConds = new List<string>();

            if (qConds.PubCatSIds != null && qConds.PubCatSIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "SID", SqlOperator.EqualTo, SystemId.ToString(qConds.PubCatSIds), GenericDBType.Char));
            }

            sqlConds.Add(custEntity.BuildConds(string.Empty, "USE_TGT", SqlOperator.EqualTo, qConds.UseTgt, GenericDBType.Int));

            if (qConds.PunitSId != null)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "PUNIT_SID", SqlOperator.EqualTo, qConds.PunitSId.Value, GenericDBType.Char));
            }

            custEntity.Append(SqlUtilBox.ToSqlConds(sqlConds.Where(q => !string.IsNullOrWhiteSpace(q)).ToArray(), " AND ", string.Empty));
            base.Entity.CustomEntitySqlSyntax = custEntity.GetSqlSyntax();
            #endregion

            SqlCondsSet condsMain = new SqlCondsSet();

            return condsMain;
        }
        #endregion
        #endregion

        #region 分類樹查詢
        #region 查詢條件
        /// <summary>
        /// 查詢條件。
        /// </summary>
        public class TreeInfoConds
        {
            ///// <summary>
            ///// 初始化 TreeInfoConds 類別的新執行個體。
            ///// 預設略過所有條件。
            ///// </summary>
            //public TreeInfoConds()
            //{

            //}

            /// <summary>
            /// 初始化 TreeInfoConds 類別的新執行個體。
            /// </summary>
            /// <param name="pubCatSId">通用分類系統代號（若為 null 則略過條件檢查）。</param> 
            /// <param name="useTgt">使用對象（1:內銷地區 2:內銷貨運方式 3:外銷貨運方式）。</param>
            /// <param name="punitSId">父單元系統代號（若為 null 則略過條件檢查）。</param>
            public TreeInfoConds(ISystemId pubCatSId, int useTgt, ISystemId punitSId)
            {
                this.PubCatSId = pubCatSId;
                this.UseTgt = useTgt;
                this.PunitSId = punitSId;
            }

            /// <summary>
            /// 通用分類系統代號（若為 null 則略過條件檢查）。
            /// </summary>
            public ISystemId PubCatSId { get; set; }
            /// <summary>
            /// 使用對象（1:內銷地區 2:內銷貨運方式 3:外銷貨運方式）。
            /// </summary>
            public int UseTgt { get; set; }
            /// <summary>
            /// 父單元系統代號（若為 null 則略過條件檢查）。
            /// </summary>
            public ISystemId PunitSId { get; set; }
        }
        #endregion

        #region GetTreeInfo
        /// <summary> 
        /// 依指定的參數取得資料。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <param name="size">指定的前 N 筆資料（若為 Int32.MaxValue 則表示全部）。</param>
        /// <param name="sqlOrder">SQL 排序（Order By）的組合類別（若為 null 則使用預設排序）。</param>
        /// <param name="includeScope">資料取得所包含的範圍（是否註解刪除或啟用中）。</param>
        /// <param name="inquireColumns">含有零或多個指定查詢時所要使用的欄位名稱 System.String 陣列。若陣列為零，則使用預設開放的欄位名稱。</param>
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetTreeInfo(TreeInfoConds qConds, int size, SqlOrder sqlOrder, IncludeScope includeScope, params string[] inquireColumns)
        {
            Returner returner = new Returner();

            SqlCondsSet condsMain = Handier.MergeDefaultCondsSet(includeScope);
            condsMain.Add(GetTreeInfoCondsSet(qConds));

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
        /// <param name="includeScope">資料取得所包含的範圍（是否註解刪除或啟用中）。</param>
        /// <param name="inquireColumns">含有零或多個指定查詢時所要使用的欄位名稱 System.String 陣列。若陣列為零，則使用預設開放的欄位名稱。</param>
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetTreeInfo(TreeInfoConds qConds, PagingFlipper flipper, SqlOrder sqlOrder, IncludeScope includeScope, params string[] inquireColumns)
        {
            Returner returner = new Returner();

            SqlCondsSet condsMain = Handier.MergeDefaultCondsSet(includeScope);
            condsMain.Add(GetTreeInfoCondsSet(qConds));

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.Entity.GetPagingBy(flipper, sqlOrder.UseDefault ? base.DefaultSqlOrder : sqlOrder, condsMain, inquireColumns));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetTreeInfoCount
        /// <summary> 
        /// 依指定的參數取得資料總數。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <param name="includeScope">資料取得所包含的範圍（是否註解刪除或啟用中）。</param> 
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetTreeInfoCount(TreeInfoConds qConds, IncludeScope includeScope)
        {
            Returner returner = new Returner();

            SqlCondsSet condsMain = Handier.MergeDefaultCondsSet(includeScope);
            condsMain.Add(GetTreeInfoCondsSet(qConds));

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.Entity.GetCountBy(condsMain));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetTreeInfoByCompoundSearch
        /// <summary> 
        /// 依指定的參數取得資料（複合式查詢）。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <param name="columnsName">欄位名稱陣列。</param>
        /// <param name="keyword">關鍵字。</param>
        /// <param name="isAbsolute">是否為絕對查詢。若 false 則使用 like 查詢。</param>
        /// <param name="flipper">分頁操作參數集合。</param>
        /// <param name="sqlOrder">SQL 排序（Order By）的組合類別（若為 null 則使用預設排序）。</param>
        /// <param name="includeScope">資料取得所包含的範圍（是否註解刪除或啟用中）。</param>
        /// <param name="inquireColumns">含有零或多個指定查詢時所要使用的欄位名稱 System.String 陣列。若陣列為零，則使用預設開放的欄位名稱。</param>
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetTreeInfoByCompoundSearch(TreeInfoConds qConds, string[] columnsName, string keyword, bool isAbsolute, PagingFlipper flipper, SqlOrder sqlOrder, IncludeScope includeScope, params string[] inquireColumns)
        {
            Returner returner = new Returner();

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.GetInfoByCompoundSearch(columnsName, keyword, EntityHelper.KEYWORD_SEPARATOR, isAbsolute, flipper, sqlOrder, GetTreeInfoCondsSet(qConds), includeScope, inquireColumns));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetTreeInfoByCompoundSearchCount
        /// <summary> 
        /// 依指定的參數取得資料總數（複合式查詢）。 
        /// 若欄位名稱陣列長度等於0，則會略過指定欄位名稱查詢動作。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <param name="columnsName">欄位名稱陣列。</param> 
        /// <param name="keyword">關鍵字。</param> 
        /// <param name="isAbsolute">是否為絕對查詢。若 false 則使用 like 查詢。</param> 
        /// <param name="includeScope">資料取得所包含的範圍（是否註解刪除或啟用中）。</param> 
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetTreeInfoByCompoundSearchCount(TreeInfoConds qConds, string[] columnsName, string keyword, bool isAbsolute, IncludeScope includeScope)
        {
            Returner returner = new Returner();

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.GetInfoByCompoundSearchCount(columnsName, keyword, EntityHelper.KEYWORD_SEPARATOR, isAbsolute, GetTreeInfoCondsSet(qConds), includeScope));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetTreeInfoCondsSet
        /// <summary> 
        /// 依指定的條件取得 SqlCondsSet。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <returns>EzCoding.Collections.SqlCondsSet。</returns> 
        private SqlCondsSet GetTreeInfoCondsSet(TreeInfoConds qConds)
        {
            #region 自訂表格
            var custEntity = new SqlSyntaxBuilder(base.Entity, new SqlSyntax());

            List<string> sqlConds;

            #region PreEntity
            StringBuilder builderPreSqlSyntax = new StringBuilder();
            sqlConds = new List<string>();

            if (base.SystemIdVerifier.IsSystemId(qConds.PubCatSId))
            {
                sqlConds.Add(string.Format("[SID] = '{0}'", qConds.PubCatSId));
            }
            else
            {
                sqlConds.Add("[PARENT_SID] IS NULL");
            }

            sqlConds.Add(string.Format("[USE_TGT] = {0}", qConds.UseTgt));

            if (base.SystemIdVerifier.IsSystemId(qConds.PunitSId))
            {
                sqlConds.Add(string.Format("[PUNIT_SID] = '{0}'", qConds.PunitSId));
            }

            //建立遞迴分類樹
            //因為只有第一階會記錄 [MAX_LAYER], 所以所有子階的 [MAX_LAYER] 都要等於上一階的 [MAX_LAYER].
            builderPreSqlSyntax.AppendFormat("WITH [PUB_CAT_TREE] ([SID], [CDT], [MDT], [CSID], [MSID], [MDELED], [ENABLED], [IS_DEF], [USE_TGT], [PUNIT_SID], [PARENT_SID], [CODE], [NAME], [DESC], [CUST_FIELD], [MAX_LAYER], [SORT], [LAYER]) AS (SELECT [SID], [CDT], [MDT], [CSID], [MSID], [MDELED], [ENABLED], [IS_DEF], [USE_TGT], [PUNIT_SID], [PARENT_SID], [CODE], [NAME], [DESC], [CUST_FIELD], [MAX_LAYER], [SORT], 1 AS [LAYER] FROM [PUB_CAT] WITH (NOLOCK) {0} UNION ALL SELECT [T1].[SID], [T1].[CDT], [T1].[MDT], [T1].[CSID], [T1].[MSID], [T1].[MDELED], [T1].[ENABLED], [T1].[IS_DEF], [T1].[USE_TGT], [T1].[PUNIT_SID], [T1].[PARENT_SID], [T1].[CODE], [T1].[NAME], [T1].[DESC], [T1].[CUST_FIELD], [T2].[MAX_LAYER], [T1].[SORT], [LAYER] + 1 FROM [PUB_CAT] [T1] WITH (NOLOCK) INNER JOIN [PUB_CAT_TREE] [T2] ON [T1].[PARENT_SID] = [T2].[SID]) ", SqlUtilBox.ToSqlConds(sqlConds.ToArray(), "WHERE", string.Empty));
            #endregion

            custEntity.Append("SELECT [PUB_CAT_TREE].* ");
            custEntity.Append("FROM [PUB_CAT_TREE] WITH (NOLOCK) ");

            sqlConds = new List<string>();

            custEntity.Append(SqlUtilBox.ToSqlConds(sqlConds.Where(q => !string.IsNullOrWhiteSpace(q)).ToArray(), " WHERE ", string.Empty));
            base.Entity.CustomEntitySqlSyntax = custEntity.GetSqlSyntax();
            base.Entity.CustomEntityPreSqlSyntax = builderPreSqlSyntax.ToString();
            #endregion

            SqlCondsSet condsMain = new SqlCondsSet();

            return condsMain;
        }
        #endregion
        #endregion

        #region GetDomDistUseInfo
        /// <summary> 
        /// 取得「內銷地區」已使用中的資訊。 
        /// </summary> 
        /// <param name="pubCatSId">通用分類系統代號。</param> 
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetDomDistUseInfo(ISystemId pubCatSId)
        {
            List<string> unions = new List<string>();

            //系統使用者/內銷地區 關聯表
            unions.Add(string.Format("SELECT 1 AS [USE_BY], (SELECT COUNT(*) AS [CNT] FROM [REL_TAB] WHERE [REL_CODE] = 2 AND [TGT_SID] = '{0}') AS [USE_CNT]", pubCatSId));

            //內銷訂單
            unions.Add(string.Format("SELECT 2 AS [USE_BY], (SELECT COUNT(*) AS [CNT] FROM [DOM_ORDER] WHERE [DOM_DIST_SID] = '{0}') AS [USE_CNT]", pubCatSId));

            return base.Entity.UnsafeExecuteInquire(string.Join(" UNION ALL ", unions.ToArray()));
        }
        #endregion

        #region GetFreightWayUseInfo
        /// <summary> 
        /// 取得「貨運方式」已使用中的資訊。 
        /// </summary> 
        /// <param name="pubCatSId">通用分類系統代號。</param> 
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetFreightWayUseInfo(ISystemId pubCatSId)
        {
            List<string> unions = new List<string>();

            //內銷訂單
            unions.Add(string.Format("SELECT 1 AS [USE_BY], (SELECT COUNT(*) AS [CNT] FROM [DOM_ORDER] WHERE [FREIGHT_WAY_SID] = '{0}') AS [USE_CNT]", pubCatSId));

            return base.Entity.UnsafeExecuteInquire(string.Join(" UNION ALL ", unions.ToArray()));
        }
        #endregion
    }
}
