using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Globalization;

using EzCoding;
using EzCoding.Collections;
using EzCoding.SystemEngines;
using EzCoding.DB;
using EzCoding.SystemEngines.EntityController;

namespace Seec.Marketing.SystemEngines
{
    public partial class DBTableDefine
    {
        /// <summary>
        /// 通用文稿。
        /// </summary>
        public const string PUB_DOC = "PUB_DOC";
    }

    /// <summary>
    /// 通用文稿的類別實作。
    /// </summary>
    public class PubDoc : SystemHandler
    {
        /// <summary>
        /// 初始化 Seec.Marketing.SystemEngines.PubDoc 類別的新執行個體。
        /// 預設使用的 Primary Key 欄位名稱為「SID」及 EzCoding.SystemIdVerifier 系統代號驗證物件。
        /// </summary>
        /// <param name="connInfo">資料庫連線資訊。</param>
        public PubDoc(string connInfo)
            : base(EntityHelper.GetEntity(connInfo, DBTableDefine.PUB_DOC))
        {

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
            /// 單元代碼。
            /// </summary>
            [SchemaMapping(Name = "UNIT_CODE", Type = ReturnType.Int)]
            public int UnitCode { get; set; }
            /// <summary>
            /// 名稱。
            /// </summary>
            [SchemaMapping(Name = "NAME", Type = ReturnType.String)]
            public string Name { get; set; }
            /// <summary>
            /// 簡介。
            /// </summary>
            [SchemaMapping(Name = "SUMMARY", Type = ReturnType.String)]
            public string Summary { get; set; }
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
            /// 發佈日期。
            /// </summary>
            [SchemaMapping(Name = "RELEASE_DATE", Type = ReturnType.DateTime, AllowNull = true)]
            public DateTime? ReleaseDate { get; set; }
            /// <summary>
            /// 上架日期。
            /// </summary>
            [SchemaMapping(Name = "UPPER_DATE", Type = ReturnType.DateTime, AllowNull = true)]
            public DateTime? UpperDate { get; set; }
            /// <summary>
            /// 下架日期。
            /// </summary>
            [SchemaMapping(Name = "LOWER_DATE", Type = ReturnType.DateTime, AllowNull = true)]
            public DateTime? LowerDate { get; set; }
            /// <summary>
            /// 是否顯示於首頁。
            /// </summary>
            [SchemaMapping(Name = "DISP_HOME", Type = ReturnType.Bool)]
            public bool DispHome { get; set; }
            /// <summary>
            /// 自訂欄位。
            /// </summary>
            [SchemaMapping(Name = "CUST_FIELD", Type = ReturnType.String)]
            public string CustField { get; set; }
            /// <summary>
            /// 狀態（0:草稿 1:上線）。
            /// </summary>
            [SchemaMapping(Name = "STATUS", Type = ReturnType.Int)]
            public int Status { get; set; }
            /// <summary>
            /// 權重。
            /// </summary>
            [SchemaMapping(Name = "WEIGHT", Type = ReturnType.Int)]
            public int Weight { get; set; }
            /// <summary>
            /// 排序。
            /// </summary>
            [SchemaMapping(Name = "SORT", Type = ReturnType.Int)]
            public int Sort { get; set; }

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
            /// 單元代碼。
            /// </summary>
            public int? UnitCode { get; set; }
            /// <summary>
            /// 名稱。
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// 簡介。
            /// </summary>
            public string Summary { get; set; }
            /// <summary>
            /// 內容標題。
            /// </summary>
            public string ContTitle { get; set; }
            /// <summary>
            /// 內容。
            /// </summary>
            public string Cont { get; set; }
            /// <summary>
            /// 發佈日期。
            /// </summary>
            public DateTime? ReleaseDate { get; set; }
            /// <summary>
            /// 上架日期。
            /// </summary>
            public DateTime? UpperDate { get; set; }
            /// <summary>
            /// 下架日期。
            /// </summary>
            public DateTime? LowerDate { get; set; }
            /// <summary>
            /// 是否顯示於首頁。
            /// </summary>
            public bool? DispHome { get; set; }
            /// <summary>
            /// 自訂欄位。
            /// </summary>
            public string CustField { get; set; }
            /// <summary>
            /// 狀態（0:草稿 1:上線）。
            /// </summary>
            public int? Status { get; set; }
            /// <summary>
            /// 權重。
            /// </summary>
            public int? Weight { get; set; }
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
            #region AboutConnPro
            /// <summary>
            /// AboutConnPro 欄位定義。
            /// </summary>
            public static class AboutConnPro
            {
                /// <summary>
                /// 示意圖說明。
                /// </summary>
                public const string IllustratedDesc = "IllustratedDesc";
            }
            #endregion

            #region AboutSuperworld
            /// <summary>
            /// AboutSuperworld 欄位定義。
            /// </summary>
            public static class AboutSuperworld
            {
                /// <summary>
                /// 示意圖說明。
                /// </summary>
                public const string IllustratedDesc = "IllustratedDesc";
            }
            #endregion

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
        }
        #endregion

        #region 異動
        #region Add
        /// <summary>
        /// 依指定的參數，新增一筆資料。
        /// </summary>
        /// <param name="actorSId">操作人系統代號。為客戶端在操作時的操作人系統代號。</param> 
        /// <param name="pubDocSId">指定的通用文稿系統代號。</param>
        /// <param name="enabled">資料的啟用狀態。</param>
        /// <param name="unitCode">單元代碼。</param>
        /// <param name="name">名稱（null 或 empty 將自動略過操作）。</param>
        /// <param name="summary">簡介（null 或 empty 將自動略過操作）。</param>
        /// <param name="contTitle">內容標題（null 或 empty 將自動略過操作）。</param>
        /// <param name="cont">內容（null 或 empty 將自動略過操作）。</param>
        /// <param name="releaseDate">發佈日期（null 將自動略過操作）。</param>
        /// <param name="upperDate">上架日期（null 將自動略過操作）。</param>
        /// <param name="lowerDate">下架日期（null 將自動略過操作）。</param>
        /// <param name="dispHome">是否顯示於首頁。</param>
        /// <param name="custField">自訂欄位（null 或 empty 將自動略過操作）。</param>
        /// <param name="status">狀態（0:草稿 1:上線）。</param>
        /// <param name="sort">排序。</param>
        /// <returns>EzCoding.Returner。若新增成功則回傳時包含著一個「NEW_SID」的 DataTable 和「TABLE_NAME」、「SID」兩個欄位。</returns>
        /// <exception cref="System.ArgumentNullException">參數 actorSId 不允許為 null 值。</exception>
        /// <exception cref="System.ArgumentNullException">參數 pubDocSId 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        public Returner Add(ISystemId actorSId, ISystemId pubDocSId, bool enabled, int unitCode, string name, string summary, string contTitle, string cont, DateTime? releaseDate, DateTime? upperDate, DateTime? lowerDate, bool dispHome, string custField, int status, int sort)
        {
            if (actorSId == null) { throw new ArgumentNullException("actorSId"); }
            if (pubDocSId == null) { throw new ArgumentNullException("pubDocSId"); }
            string[] exceptionSIds = this.SystemIdVerifier.CheckValid(actorSId, pubDocSId);
            if (exceptionSIds.Length > 0)
            {
                throw new LibraryException(string.Format("invaild ISystemId ({0}) format.", string.Join("、", exceptionSIds)));
            }

            Returner returner = new Returner();

            DataSet dSet = null;
            DataTableMaker dataTableMaker = null;

            try
            {
                #region 交易緩衝區
                List<object> columnsValue = new List<object>();
                dataTableMaker = new DataTableMaker();
                Handier.AddDataTableMaker("SID", pubDocSId.ToString(), dataTableMaker, columnsValue, false);
                Handier.AddDataTableMaker("CSID", actorSId.ToString(), dataTableMaker, columnsValue, false);
                Handier.AddDataTableMaker("MSID", actorSId.ToString(), dataTableMaker, columnsValue, false);
                Handier.AddDataTableMaker("ENABLED", enabled ? "Y" : "N", dataTableMaker, columnsValue, false);
                Handier.AddDataTableMaker("UNIT_CODE", unitCode, dataTableMaker, columnsValue);
                Handier.AddDataTableMaker("NAME", name, dataTableMaker, columnsValue, true);
                Handier.AddDataTableMaker("SUMMARY", summary, dataTableMaker, columnsValue, true);
                Handier.AddDataTableMaker("CONT_TITLE", contTitle, dataTableMaker, columnsValue, true);
                Handier.AddDataTableMaker("CONT", cont, dataTableMaker, columnsValue, true);
                Handier.AddDataTableMaker("RELEASE_DATE", releaseDate, "yyyyMMdd", dataTableMaker, columnsValue);
                Handier.AddDataTableMaker("UPPER_DATE", upperDate, "yyyyMMdd", dataTableMaker, columnsValue);
                Handier.AddDataTableMaker("LOWER_DATE", lowerDate, "yyyyMMdd", dataTableMaker, columnsValue);
                Handier.AddDataTableMaker("DISP_HOME", dispHome ? "Y" : "N", dataTableMaker, columnsValue, false);
                Handier.AddDataTableMaker("CUST_FIELD", custField, dataTableMaker, columnsValue, true);
                Handier.AddDataTableMaker("STATUS", status, dataTableMaker, columnsValue);
                Handier.AddDataTableMaker("SORT", sort, dataTableMaker, columnsValue);

                if (columnsValue.Count == 0) { return returner; }
                dataTableMaker.AddColumnsValue(columnsValue.ToArray());

                dSet = new DataSet();
                dSet.Locale = CultureInfo.InvariantCulture;
                dSet.Tables.Add(dataTableMaker.DataTable);
                returner.ChangeInto(base.Add(dSet, true));
                #endregion

                return returner;
            }
            finally
            {
                if (dataTableMaker != null) { dataTableMaker.Dispose(); }
            }
        }
        #endregion

        #region Modify
        /// <summary>
        /// 依指定的參數，修改一筆通用文稿。
        /// </summary>
        /// <param name="actorSId">操作人系統代號。為客戶端在操作時的操作人系統代號。</param>
        /// <param name="pubDocSId">通用文稿系統代號。</param>
        /// <param name="enabled">資料的啟用狀態。</param>
        /// <param name="name">名稱（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="summary">簡介（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="contTitle">內容標題（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="cont">內容（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="releaseDate">發佈日期（null 則直接設為 DBNull）。</param>
        /// <param name="upperDate">上架日期（null 則直接設為 DBNull）。</param>
        /// <param name="lowerDate">下架日期（null 則直接設為 DBNull）。</param>
        /// <param name="dispHome">是否顯示於首頁。</param>
        /// <param name="custField">自訂欄位（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="status">狀態（0:草稿 1:上線）。</param>
        /// <param name="sort">排序。</param>
        /// <returns>EzCoding.Returner。</returns>
        /// <exception cref="System.ArgumentNullException">參數 actorSId 不允許為 null 值。</exception>
        /// <exception cref="System.ArgumentNullException">參數 pubDocSId 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        public Returner Modify(ISystemId actorSId, ISystemId pubDocSId, bool enabled, string name, string summary, string contTitle, string cont, DateTime? releaseDate, DateTime? upperDate, DateTime? lowerDate, bool dispHome, string custField, int status, int sort)
        {
            if (actorSId == null) { throw new ArgumentNullException("actorSId"); }
            if (pubDocSId == null) { throw new ArgumentNullException("pubDocSId"); }
            string[] exceptionSIds = this.SystemIdVerifier.CheckValid(actorSId, pubDocSId);
            if (exceptionSIds.Length > 0)
            {
                throw new LibraryException(string.Format("invaild ISystemId ({0}) format.", string.Join("、", exceptionSIds)));
            }

            Returner returner = new Returner();

            DataSet dSet = null;
            DataTableMaker dataTableMaker = null;

            try
            {
                #region 交易緩衝區
                List<object> columnsValue = new List<object>();
                dataTableMaker = new DataTableMaker();
                Handier.AddDataTableMaker("MDT", DateTime.Now, "yyyyMMddHHmmss", dataTableMaker, columnsValue, false);
                Handier.AddDataTableMaker("MSID", actorSId.ToString(), dataTableMaker, columnsValue, false);
                Handier.AddDataTableMaker("ENABLED", enabled ? "Y" : "N", dataTableMaker, columnsValue, false);
                Handier.AddDataTableMaker("NAME", name, dataTableMaker, columnsValue, true, true);
                Handier.AddDataTableMaker("SUMMARY", summary, dataTableMaker, columnsValue, true, true);
                Handier.AddDataTableMaker("CONT_TITLE", contTitle, dataTableMaker, columnsValue, true, true);
                Handier.AddDataTableMaker("CONT", cont, dataTableMaker, columnsValue, true, true);
                Handier.AddDataTableMaker("RELEASE_DATE", releaseDate, "yyyyMMdd", dataTableMaker, columnsValue);
                Handier.AddDataTableMaker("UPPER_DATE", upperDate, "yyyyMMdd", dataTableMaker, columnsValue, true);
                Handier.AddDataTableMaker("LOWER_DATE", lowerDate, "yyyyMMdd", dataTableMaker, columnsValue, true);
                Handier.AddDataTableMaker("DISP_HOME", dispHome ? "Y" : "N", dataTableMaker, columnsValue, false);
                Handier.AddDataTableMaker("CUST_FIELD", custField, dataTableMaker, columnsValue, true, true);
                Handier.AddDataTableMaker("STATUS", status, dataTableMaker, columnsValue);
                Handier.AddDataTableMaker("SORT", sort, dataTableMaker, columnsValue);

                if (columnsValue.Count == 0) { return returner; }
                dataTableMaker.AddColumnsValue(columnsValue.ToArray());

                dSet = new DataSet();
                dSet.Locale = CultureInfo.InvariantCulture;
                dSet.Tables.Add(dataTableMaker.DataTable);

                SqlCondsSet condsMain = new SqlCondsSet();
                condsMain.Add(base.PrimaryKey, SqlCond.EqualTo, pubDocSId.ToString(), GenericDBType.Char, string.Empty);

                returner.ChangeInto(base.Modify(dSet, condsMain, true));
                #endregion

                return returner;
            }
            finally
            {
                if (dataTableMaker != null) { dataTableMaker.Dispose(); }
            }
        }
        #endregion

        #region UpdateWeight
        /// <summary>
        /// 更新權重。
        /// </summary>
        /// <param name="pubDocSId">通用文稿系統代號。</param> 
        /// <param name="val">權重。</param>
        /// <returns>EzCoding.Returner。</returns>
        /// <exception cref="System.ArgumentNullException">參數 pubDocSId 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        public Returner UpdateWeight(ISystemId pubDocSId, int val)
        {
            if (pubDocSId == null) { throw new ArgumentNullException("pubDocSId"); }
            string[] exceptionSIds = this.SystemIdVerifier.CheckValid(pubDocSId);
            if (exceptionSIds.Length > 0)
            {
                throw new LibraryException(string.Format("invaild ISystemId ({0}) format.", string.Join("、", exceptionSIds)));
            }

            Returner returner = new Returner();

            DataSet dSet = null;
            DataTableMaker dataTableMaker = null;

            try
            {
                #region 交易緩衝區
                dataTableMaker = new DataTableMaker();
                dataTableMaker.AddDataColumn("WEIGHT");
                dataTableMaker.AddColumnsValue(new object[] { val });

                dSet = new DataSet();
                dSet.Locale = CultureInfo.InvariantCulture;
                dSet.Tables.Add(dataTableMaker.DataTable);

                SqlCondsSet condsMain = new SqlCondsSet();
                condsMain.Add(base.PrimaryKey, SqlCond.EqualTo, pubDocSId.ToString(), GenericDBType.Char, string.Empty);

                returner.ChangeInto(base.Modify(dSet, condsMain, true));
                #endregion

                return returner;
            }
            finally
            {
                if (dataTableMaker != null) { dataTableMaker.Dispose(); }
            }
        }
        #endregion

        #region UpdateCustSort
        /// <summary>
        /// 更新自訂排序。
        /// </summary>
        /// <param name="pubDocSId">通用文稿系統代號。</param> 
        /// <param name="val">排序。</param>
        /// <returns>EzCoding.Returner。</returns>
        /// <exception cref="System.ArgumentNullException">參數 pubDocSId 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        public Returner UpdateCustSort(ISystemId pubDocSId, int val)
        {
            if (pubDocSId == null) { throw new ArgumentNullException("pubDocSId"); }
            string[] exceptionSIds = this.SystemIdVerifier.CheckValid(pubDocSId);
            if (exceptionSIds.Length > 0)
            {
                throw new LibraryException(string.Format("invaild ISystemId ({0}) format.", string.Join("、", exceptionSIds)));
            }

            Returner returner = new Returner();

            DataSet dSet = null;
            DataTableMaker dataTableMaker = null;

            try
            {
                #region 交易緩衝區
                dataTableMaker = new DataTableMaker();
                dataTableMaker.AddDataColumn("SORT");
                dataTableMaker.AddColumnsValue(new object[] { val });

                dSet = new DataSet();
                dSet.Locale = CultureInfo.InvariantCulture;
                dSet.Tables.Add(dataTableMaker.DataTable);

                SqlCondsSet condsMain = new SqlCondsSet();
                condsMain.Add(base.PrimaryKey, SqlCond.EqualTo, pubDocSId.ToString(), GenericDBType.Char, string.Empty);

                returner.ChangeInto(base.Modify(dSet, condsMain, true));
                #endregion

                return returner;
            }
            finally
            {
                if (dataTableMaker != null) { dataTableMaker.Dispose(); }
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
            /// <param name="pubDocSIds">通用文稿系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param> 
            /// <param name="unitCode">單元代碼。</param>
            /// <param name="statuses">狀態（0:草稿 1:上線; 若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="beginReleaseDateByRange">範圍查詢的起始發佈日期（若為 null 則略過條件檢查）。</param>
            /// <param name="endReleaseDateByRange">範圍查詢的結束發佈日期（若為 null 則略過條件檢查）。</param>
            /// <param name="dispHome">否顯示於首頁（若為 null 則略過條件檢查）。</param>
            /// <param name="ulsStatus">上下架狀態。</param>
            public InfoConds(ISystemId[] pubDocSIds, int unitCode, int[] statuses, DateTime? beginReleaseDateByRange, DateTime? endReleaseDateByRange, bool? dispHome, ULShelfStatus ulsStatus)
            {
                this.PubDocSIds = pubDocSIds;
                this.UnitCode = unitCode;
                this.Statuses = statuses;
                this.BeginReleaseDateByRange = beginReleaseDateByRange;
                this.EndReleaseDateByRange = endReleaseDateByRange;
                this.DispHome = dispHome;
                this.UlsStatus = ulsStatus;
            }

            /// <summary>
            /// 通用文稿系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public ISystemId[] PubDocSIds { get; set; }
            /// <summary>
            /// 單元代碼。
            /// </summary>
            public int UnitCode { get; set; }
            /// <summary>
            /// 狀態（0:草稿 1:上線; 若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public int[] Statuses { get; set; }
            /// <summary>
            /// 範圍查詢的起始發佈日期（若為 null 則略過條件檢查）。
            /// </summary>
            public DateTime? BeginReleaseDateByRange { get; set; }
            /// <summary>
            /// 範圍查詢的結束發佈日期（若為 null 則略過條件檢查）。
            /// </summary>
            public DateTime? EndReleaseDateByRange { get; set; }
            /// <summary>
            /// 是否顯示於首頁（若為 null 則略過條件檢查）。
            /// </summary>
            public bool? DispHome { get; set; }
            /// <summary>
            /// 上下架狀態。
            /// </summary>
            public ULShelfStatus UlsStatus { get; set; }
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
        SqlCondsSet GetInfoCondsSet(InfoConds qConds)
        {
            #region 自訂表格
            var custEntity = new SqlSyntaxBuilder(base.Entity, new SqlSyntax());

            custEntity.Append("SELECT [PUB_DOC].* ");
            custEntity.Append("FROM [PUB_DOC] WITH (NOLOCK) ");

            var sqlConds = new List<string>();

            if (qConds.PubDocSIds != null && qConds.PubDocSIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "SID", SqlOperator.EqualTo, SystemId.ToString(qConds.PubDocSIds), GenericDBType.Char));
            }

            sqlConds.Add(custEntity.BuildConds(string.Empty, "UNIT_CODE", SqlOperator.EqualTo, qConds.UnitCode, GenericDBType.Int));

            if (qConds.DispHome.HasValue)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "DISP_HOME", SqlOperator.EqualTo, qConds.DispHome.Value ? "Y" : "N", GenericDBType.Char));
            }

            if (qConds.Statuses != null && qConds.Statuses.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "STATUS", SqlOperator.EqualTo, qConds.Statuses.Select(q => (object)q).ToArray(), GenericDBType.Int));
            }

            if (qConds.BeginReleaseDateByRange.HasValue)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "RELEASE_DATE", SqlOperator.GreaterEqualThan, qConds.BeginReleaseDateByRange.Value.ToString("yyyyMMdd"), GenericDBType.Char));
            }

            if (qConds.EndReleaseDateByRange.HasValue)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "RELEASE_DATE", SqlOperator.LessEqualThan, qConds.EndReleaseDateByRange.Value.ToString("yyyyMMdd"), GenericDBType.Char));
            }

            custEntity.Append(SqlUtilBox.ToSqlConds(sqlConds.Where(q => !string.IsNullOrWhiteSpace(q)).ToArray(), " WHERE ", string.Empty));
            base.Entity.CustomEntitySqlSyntax = custEntity.GetSqlSyntax();
            #endregion

            SqlCondsSet condsMain = new SqlCondsSet();

            string today = DateTime.Today.ToString("yyyyMMdd", CultureInfo.InvariantCulture);

            if (qConds.UlsStatus == ULShelfStatus.Normal)
            {
                #region 已上架未下架
                SqlCondsSet condsULShelf = new SqlCondsSet();
                SqlCondsSet condsUpper = new SqlCondsSet();
                condsUpper.Add("UPPER_DATE", SqlCond.LessEqualThan, today, GenericDBType.Char, SqlCondsSet.Or);
                condsUpper.Add("UPPER_DATE", SqlCond.IsNull, GenericDBType.Char, string.Empty);

                SqlCondsSet condsLower = new SqlCondsSet();
                condsLower.Add("LOWER_DATE", SqlCond.GreaterEqualThan, today, GenericDBType.Char, SqlCondsSet.Or);
                condsLower.Add("LOWER_DATE", SqlCond.IsNull, GenericDBType.Char, string.Empty);

                condsULShelf.Add(condsUpper, SqlCondsSet.And);
                condsULShelf.Add(condsLower);

                condsMain.Add(condsULShelf, SqlCondsSet.And);
                #endregion
            }
            else if (qConds.UlsStatus == ULShelfStatus.HasUpper)
            {
                #region 已上架但不論是否已下架
                SqlCondsSet condsUpper = new SqlCondsSet();
                condsUpper.Add("UPPER_DATE", SqlCond.LessEqualThan, today, GenericDBType.Char, SqlCondsSet.Or);
                condsUpper.Add("UPPER_DATE", SqlCond.IsNull, GenericDBType.Char, string.Empty);

                condsMain.Add(condsUpper, SqlCondsSet.And);
                #endregion
            }
            else if (qConds.UlsStatus == ULShelfStatus.OnlyLower)
            {
                #region 僅已下架
                condsMain.Add("LOWER_DATE", SqlCond.LessThan, today, GenericDBType.Char, SqlCondsSet.And);
                #endregion
            }

            return condsMain;
        }
        #endregion
        #endregion

        #region 最新消息查詢
        #region 表格資料
        /// <summary>
        /// 表格資料。
        /// </summary>
        public class NewsInfo : Info
        {
            /// <summary>
            /// 發佈日期-年。
            /// </summary>
            [SchemaMapping(Name = "RELEASE_YEAR", Type = ReturnType.String)]
            public string ReleaseYear { get; set; }

            #region 繫結資料
            /// <summary>
            /// 繫結資料。
            /// </summary>
            new public static NewsInfo Binding(DataRow row)
            {
                return DBUtilBox.BindingRow<NewsInfo>(new NewsInfo(), row);
            }

            /// <summary>
            /// 繫結資料。
            /// </summary>
            new public static NewsInfo[] Binding(DataTable dTable)
            {
                List<NewsInfo> infos = new List<NewsInfo>();
                foreach (DataRow row in dTable.Rows)
                {
                    infos.Add(NewsInfo.Binding(row));
                }

                return infos.ToArray();
            }
            #endregion
        }
        #endregion

        #region 查詢條件
        /// <summary>
        /// 查詢條件。
        /// </summary>
        public class NewsInfoConds
        {
            ///// <summary>
            ///// 初始化 NewsInfoConds 類別的新執行個體。
            ///// 預設略過所有條件。
            ///// </summary>
            //public NewsInfoConds()
            //{

            //}

            /// <summary>
            /// 初始化 NewsInfoConds 類別的新執行個體。
            /// </summary>
            /// <param name="pubDocSIds">通用文稿系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param> 
            /// <param name="statuses">狀態（0:草稿 1:上線; 若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="beginReleaseDateByRange">範圍查詢的起始發佈日期（若為 null 則略過條件檢查）。</param>
            /// <param name="endReleaseDateByRange">範圍查詢的結束發佈日期（若為 null 則略過條件檢查）。</param>
            /// <param name="dispHome">否顯示於首頁（若為 null 則略過條件檢查）。</param>
            /// <param name="ulsStatus">上下架狀態。</param>
            /// <param name="releaseYear">發佈日期-年（若為 null 則略過條件檢查）。</param>
            public NewsInfoConds(ISystemId[] pubDocSIds, int[] statuses, DateTime? beginReleaseDateByRange, DateTime? endReleaseDateByRange, bool? dispHome, ULShelfStatus ulsStatus, int? releaseYear)
            {
                this.PubDocSIds = pubDocSIds;
                this.Statuses = statuses;
                this.BeginReleaseDateByRange = beginReleaseDateByRange;
                this.EndReleaseDateByRange = endReleaseDateByRange;
                this.DispHome = dispHome;
                this.UlsStatus = ulsStatus;
                this.ReleaseYear = releaseYear;
            }

            /// <summary>
            /// 通用文稿系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public ISystemId[] PubDocSIds { get; set; }
            /// <summary>
            /// 狀態（0:草稿 1:上線; 若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public int[] Statuses { get; set; }
            /// <summary>
            /// 範圍查詢的起始發佈日期（若為 null 則略過條件檢查）。
            /// </summary>
            public DateTime? BeginReleaseDateByRange { get; set; }
            /// <summary>
            /// 範圍查詢的結束發佈日期（若為 null 則略過條件檢查）。
            /// </summary>
            public DateTime? EndReleaseDateByRange { get; set; }
            /// <summary>
            /// 是否顯示於首頁（若為 null 則略過條件檢查）。
            /// </summary>
            public bool? DispHome { get; set; }
            /// <summary>
            /// 上下架狀態。
            /// </summary>
            public ULShelfStatus UlsStatus { get; set; }
            /// <summary>
            /// 發佈日期-年（若為 null 則略過條件檢查）。
            /// </summary>
            public int? ReleaseYear { get; set; }
        }
        #endregion

        #region GetNewsInfo
        /// <summary> 
        /// 依指定的參數取得資料。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <param name="size">指定的前 N 筆資料（若為 Int32.MaxValue 則表示全部）。</param>
        /// <param name="sqlOrder">SQL 排序（Order By）的組合類別（若為 null 則使用預設排序）。</param>
        /// <param name="includeScope">資料取得所包含的範圍（是否註解刪除或啟用中）。</param>
        /// <param name="inquireColumns">含有零或多個指定查詢時所要使用的欄位名稱 System.String 陣列。若陣列為零，則使用預設開放的欄位名稱。</param>
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetNewsInfo(NewsInfoConds qConds, int size, SqlOrder sqlOrder, IncludeScope includeScope, params string[] inquireColumns)
        {
            Returner returner = new Returner();

            SqlCondsSet condsMain = Handier.MergeDefaultCondsSet(includeScope);
            condsMain.Add(GetNewsInfoCondsSet(qConds));

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
        public Returner GetNewsInfo(NewsInfoConds qConds, PagingFlipper flipper, SqlOrder sqlOrder, IncludeScope includeScope, params string[] inquireColumns)
        {
            Returner returner = new Returner();

            SqlCondsSet condsMain = Handier.MergeDefaultCondsSet(includeScope);
            condsMain.Add(GetNewsInfoCondsSet(qConds));

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.Entity.GetPagingBy(flipper, sqlOrder.UseDefault ? base.DefaultSqlOrder : sqlOrder, condsMain, inquireColumns));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetNewsInfoCount
        /// <summary> 
        /// 依指定的參數取得資料總數。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <param name="includeScope">資料取得所包含的範圍（是否註解刪除或啟用中）。</param> 
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetNewsInfoCount(NewsInfoConds qConds, IncludeScope includeScope)
        {
            Returner returner = new Returner();

            SqlCondsSet condsMain = Handier.MergeDefaultCondsSet(includeScope);
            condsMain.Add(GetNewsInfoCondsSet(qConds));

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.Entity.GetCountBy(condsMain));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetNewsInfoByCompoundSearch
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
        public Returner GetNewsInfoByCompoundSearch(NewsInfoConds qConds, string[] columnsName, string keyword, bool isAbsolute, PagingFlipper flipper, SqlOrder sqlOrder, IncludeScope includeScope, params string[] inquireColumns)
        {
            Returner returner = new Returner();

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.GetInfoByCompoundSearch(columnsName, keyword, EntityHelper.KEYWORD_SEPARATOR, isAbsolute, flipper, sqlOrder, GetNewsInfoCondsSet(qConds), includeScope, inquireColumns));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetNewsInfoByCompoundSearchCount
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
        public Returner GetNewsInfoByCompoundSearchCount(NewsInfoConds qConds, string[] columnsName, string keyword, bool isAbsolute, IncludeScope includeScope)
        {
            Returner returner = new Returner();

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.GetInfoByCompoundSearchCount(columnsName, keyword, EntityHelper.KEYWORD_SEPARATOR, isAbsolute, GetNewsInfoCondsSet(qConds), includeScope));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetGroupNewsInfo
        /// <summary> 
        /// 依指定的欄位取得群組資料。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <param name="fieldNames">欄位名稱陣列集合。</param>
        /// <param name="sort">排序方式。</param>
        /// <param name="includeScope">資料取得所包含的範圍（是否註解刪除或啟用中）。</param>
        /// <returns>EzCoding.Returner。</returns>
        public Returner GetGroupNewsInfo(NewsInfoConds qConds, string[] fieldNames, Sort sort, IncludeScope includeScope)
        {
            SqlCondsSet condsMain = Handier.MergeDefaultCondsSet(includeScope);
            condsMain.Add(this.GetNewsInfoCondsSet(qConds));

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

        /// <summary> 
        /// 依指定的欄位取得群組資料。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <param name="flipper">分頁操作參數集合。</param>
        /// <param name="fieldNames">欄位名稱陣列集合。</param>
        /// <param name="sort">排序方式。</param>
        /// <param name="includeScope">資料取得所包含的範圍（是否註解刪除或啟用中）。</param>
        /// <returns>EzCoding.Returner。</returns>
        public Returner GetGroupNewsInfo(NewsInfoConds qConds, PagingFlipper flipper, string[] fieldNames, Sort sort, IncludeScope includeScope)
        {
            SqlCondsSet condsMain = Handier.MergeDefaultCondsSet(includeScope);
            condsMain.Add(this.GetNewsInfoCondsSet(qConds));

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
                return base.Entity.GetGroupPagingBy(flipper, grouping, sorting, condsMain);
            }
            finally
            {
                base.Entity.EnableCustomEntity = false;
            }
        }
        #endregion

        #region GetGroupNewsInfoCount
        /// <summary> 
        /// 依指定的欄位取得群組資料總數。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <param name="fieldNames">欄位名稱陣列集合。</param>
        /// <param name="includeScope">資料取得所包含的範圍（是否註解刪除或啟用中）。</param>
        /// <returns>EzCoding.Returner。</returns>
        public Returner GetGroupNewsInfoCount(NewsInfoConds qConds, string[] fieldNames, IncludeScope includeScope)
        {
            SqlCondsSet condsMain = Handier.MergeDefaultCondsSet(includeScope);
            condsMain.Add(this.GetNewsInfoCondsSet(qConds));

            SqlGroup grouping = new SqlGroup();

            for (int i = 0; i < fieldNames.Length; i++)
            {
                grouping.Add(fieldNames[i]);
            }

            try
            {
                base.Entity.EnableCustomEntity = true;
                return base.Entity.GetGroupCountBy(grouping, condsMain);
            }
            finally
            {
                base.Entity.EnableCustomEntity = false;
            }
        }
        #endregion

        #region GetNewsInfoCondsSet
        /// <summary> 
        /// 依指定的條件取得 SqlCondsSet。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <returns>EzCoding.Collections.SqlCondsSet。</returns> 
        SqlCondsSet GetNewsInfoCondsSet(NewsInfoConds qConds)
        {
            #region 自訂表格
            var custEntity = new SqlSyntaxBuilder(base.Entity, new SqlSyntax());

            custEntity.Append("SELECT [PUB_DOC].*, ");
            custEntity.Append("       SUBSTRING([RELEASE_DATE], 1, 4) AS [RELEASE_YEAR] ");
            custEntity.Append("FROM [PUB_DOC] WITH (NOLOCK) ");
            custEntity.Append("WHERE [UNIT_CODE] = 6 ");

            var sqlConds = new List<string>();

            if (qConds.PubDocSIds != null && qConds.PubDocSIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "SID", SqlOperator.EqualTo, SystemId.ToString(qConds.PubDocSIds), GenericDBType.Char));
            }

            if (qConds.DispHome.HasValue)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "DISP_HOME", SqlOperator.EqualTo, qConds.DispHome.Value ? "Y" : "N", GenericDBType.Char));
            }

            if (qConds.Statuses != null && qConds.Statuses.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "STATUS", SqlOperator.EqualTo, qConds.Statuses.Select(q => (object)q).ToArray(), GenericDBType.Int));
            }

            if (qConds.BeginReleaseDateByRange.HasValue)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "RELEASE_DATE", SqlOperator.GreaterEqualThan, qConds.BeginReleaseDateByRange.Value.ToString("yyyyMMdd"), GenericDBType.Char));
            }

            if (qConds.EndReleaseDateByRange.HasValue)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "RELEASE_DATE", SqlOperator.LessEqualThan, qConds.EndReleaseDateByRange.Value.ToString("yyyyMMdd"), GenericDBType.Char));
            }

            custEntity.Append(SqlUtilBox.ToSqlConds(sqlConds.Where(q => !string.IsNullOrWhiteSpace(q)).ToArray(), " AND ", string.Empty));
            base.Entity.CustomEntitySqlSyntax = custEntity.GetSqlSyntax();
            #endregion

            SqlCondsSet condsMain = new SqlCondsSet();

            if (qConds.ReleaseYear.HasValue)
            {
                condsMain.Add("RELEASE_YEAR", SqlCond.EqualTo, qConds.ReleaseYear.Value.ToString(), GenericDBType.VarChar, SqlCondsSet.And);
            }

            string today = DateTime.Today.ToString("yyyyMMdd", CultureInfo.InvariantCulture);

            if (qConds.UlsStatus == ULShelfStatus.Normal)
            {
                #region 已上架未下架
                SqlCondsSet condsULShelf = new SqlCondsSet();
                SqlCondsSet condsUpper = new SqlCondsSet();
                condsUpper.Add("UPPER_DATE", SqlCond.LessEqualThan, today, GenericDBType.Char, SqlCondsSet.Or);
                condsUpper.Add("UPPER_DATE", SqlCond.IsNull, GenericDBType.Char, string.Empty);

                SqlCondsSet condsLower = new SqlCondsSet();
                condsLower.Add("LOWER_DATE", SqlCond.GreaterEqualThan, today, GenericDBType.Char, SqlCondsSet.Or);
                condsLower.Add("LOWER_DATE", SqlCond.IsNull, GenericDBType.Char, string.Empty);

                condsULShelf.Add(condsUpper, SqlCondsSet.And);
                condsULShelf.Add(condsLower);

                condsMain.Add(condsULShelf, SqlCondsSet.And);
                #endregion
            }
            else if (qConds.UlsStatus == ULShelfStatus.HasUpper)
            {
                #region 已上架但不論是否已下架
                SqlCondsSet condsUpper = new SqlCondsSet();
                condsUpper.Add("UPPER_DATE", SqlCond.LessEqualThan, today, GenericDBType.Char, SqlCondsSet.Or);
                condsUpper.Add("UPPER_DATE", SqlCond.IsNull, GenericDBType.Char, string.Empty);

                condsMain.Add(condsUpper, SqlCondsSet.And);
                #endregion
            }
            else if (qConds.UlsStatus == ULShelfStatus.OnlyLower)
            {
                #region 僅已下架
                condsMain.Add("LOWER_DATE", SqlCond.LessThan, today, GenericDBType.Char, SqlCondsSet.And);
                #endregion
            }

            return condsMain;
        }
        #endregion
        #endregion
    }
}
