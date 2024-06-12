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
        /// 系統組態。
        /// </summary>
        public const string SYS_CONFIG = "SYS_CONFIG";
    }

    /// <summary>
    /// 系統組態的類別實作。
    /// </summary>
    public class SysConfig : SystemHandler
    {
        /// <summary>
        /// 初始化 Seec.Marketing.SystemEngines.SysConfig 類別的新執行個體。
        /// 預設使用的 Primary Key 欄位名稱為「SID」及 EzCoding.SystemIdVerifier 系統代號驗證物件。
        /// </summary>
        /// <param name="connInfo">資料庫連線資訊。</param>
        public SysConfig(string connInfo)
            : base(EntityHelper.GetEntity(connInfo, DBTableDefine.SYS_CONFIG))
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
            /// 組態類別（1:靜態組態 2:{for 單元組態}）。
            /// </summary>
            [SchemaMapping(Name = "CONFIG_TYPE", Type = ReturnType.Int)]
            public int ConfigType { get; set; }
            /// <summary>
            /// 組態代碼。
            /// </summary>
            [SchemaMapping(Name = "CONFIG_CODE", Type = ReturnType.String)]
            public string ConfigCode { get; set; }
            /// <summary>
            /// 組態名稱。
            /// </summary>
            [SchemaMapping(Name = "CONFIG_NAME", Type = ReturnType.String)]
            public string ConfigName { get; set; }
            /// <summary>
            /// 組態參數。
            /// </summary>
            [SchemaMapping(Name = "CONFIG_PARAM", Type = ReturnType.String)]
            public string ConfigParam { get; set; }

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
            /// 組態類別（1:靜態組態 2:{for 單元組態}）。
            /// </summary>
            public int? ConfigType { get; set; }
            /// <summary>
            /// 組態代碼。
            /// </summary>
            public string ConfigCode { get; set; }
            /// <summary>
            /// 組態名稱。
            /// </summary>
            public string ConfigName { get; set; }
            /// <summary>
            /// 組態參數。
            /// </summary>
            public string ConfigParam { get; set; }
        }
        #endregion

        #region 異動
        #region Add
        /// <summary>
        /// 依指定的參數，新增一筆資料。
        /// </summary>
        /// <param name="actorSId">操作人系統代號。為客戶端在操作時的操作人系統代號。</param> 
        /// <param name="configType">組態類別。</param>
        /// <param name="configCode">組態代碼。</param>
        /// <param name="configName">組態名稱。</param>
        /// <param name="configParam">組態參數。</param>
        /// <returns>EzCoding.Returner。若新增成功則回傳時包含著一個「NEW_SID」的 DataTable 和「TABLE_NAME」、「SID」兩個欄位。</returns>
        /// <exception cref="System.ArgumentNullException">參數 actorSId 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        public Returner Add(ISystemId actorSId, int configType, string configCode, string configName, string configParam)
        {
            if (actorSId == null) { throw new ArgumentNullException("actorSId"); }
            string[] exceptionSIds = this.SystemIdVerifier.CheckValid(actorSId);
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
                Handier.AddDataTableMaker("CSID", actorSId.ToString(), dataTableMaker, columnsValue, false);
                Handier.AddDataTableMaker("MSID", actorSId.ToString(), dataTableMaker, columnsValue, false);
                Handier.AddDataTableMaker("CONFIG_TYPE", configType, dataTableMaker, columnsValue);
                Handier.AddDataTableMaker("CONFIG_CODE", configCode, dataTableMaker, columnsValue, false);
                Handier.AddDataTableMaker("CONFIG_NAME", configName, dataTableMaker, columnsValue, false);
                Handier.AddDataTableMaker("CONFIG_PARAM", configParam, dataTableMaker, columnsValue, false);

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
        /// 依指定的參數，修改一筆系統組態。
        /// </summary>
        /// <param name="actorSId">操作人系統代號。為客戶端在操作時的操作人系統代號。</param>
        /// <param name="configType">組態類別。</param>
        /// <param name="configCode">組態代碼。</param>
        /// <param name="configParam">組態參數。</param>
        /// <returns>EzCoding.Returner。</returns>
        /// <exception cref="System.ArgumentNullException">參數 actorSId 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        public Returner Modify(ISystemId actorSId, int configType, string configCode, string configParam)
        {
            if (actorSId == null) { throw new ArgumentNullException("actorSId"); }
            string[] exceptionSIds = this.SystemIdVerifier.CheckValid(actorSId);
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
                Handier.AddDataTableMaker("CONFIG_PARAM", configParam, dataTableMaker, columnsValue, false);

                if (columnsValue.Count == 0) { return returner; }
                dataTableMaker.AddColumnsValue(columnsValue.ToArray());

                dSet = new DataSet();
                dSet.Locale = CultureInfo.InvariantCulture;
                dSet.Tables.Add(dataTableMaker.DataTable);

                SqlCondsSet condsMain = new SqlCondsSet();
                condsMain.Add("CONFIG_TYPE", SqlCond.EqualTo, configType, GenericDBType.Int, SqlCondsSet.And);
                condsMain.Add("CONFIG_CODE", SqlCond.EqualTo, configCode, GenericDBType.VarChar, SqlCondsSet.And);

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

        #region 查詢
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
            /// <param name="configType">組態類別。</param>
            /// <param name="configCode">組態代碼（若為 null 或 empty 則略過條件檢查）。</param>
            public InfoConds(int configType, string configCode)
            {
                this.ConfigType = configType;
                this.ConfigCode = configCode;
            }

            /// <summary>
            /// 組態類別。
            /// </summary>
            public int ConfigType { get; set; }
            /// <summary>
            /// 組態代碼（若為 null 或 empty 則略過條件檢查）。
            /// </summary>
            public string ConfigCode { get; set; }
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
            condsMain.Add(this.GetInfoCondsSet(qConds));

            returner.ChangeInto(base.Entity.GetInfoBy(size, sqlOrder.UseDefault ? base.DefaultSqlOrder : sqlOrder, condsMain, inquireColumns));
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
            condsMain.Add(this.GetInfoCondsSet(qConds));

            returner.ChangeInto(base.Entity.GetPagingBy(flipper, sqlOrder.UseDefault ? base.DefaultSqlOrder : sqlOrder, condsMain, inquireColumns));
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
            condsMain.Add(this.GetInfoCondsSet(qConds));

            returner.ChangeInto(base.Entity.GetCountBy(condsMain));
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
        public Returner GetInfoByCompoundSearch(InfoConds qConds, string[] columnsName, string keyword, bool isAbsolute, PagingFlipper flipper, SqlOrder sqlOrder,
        IncludeScope includeScope, params string[] inquireColumns)
        {
            Returner returner = new Returner();

            returner.ChangeInto(base.GetInfoByCompoundSearch(columnsName, keyword, EntityHelper.KEYWORD_SEPARATOR, isAbsolute, flipper, sqlOrder, this.GetInfoCondsSet(qConds), includeScope, inquireColumns
            ));
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

            returner.ChangeInto(base.GetInfoByCompoundSearchCount(columnsName, keyword, EntityHelper.KEYWORD_SEPARATOR, isAbsolute, this.GetInfoCondsSet(qConds), includeScope));
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
            SqlCondsSet condsMain = new SqlCondsSet();

            condsMain.Add("CONFIG_TYPE", SqlCond.EqualTo, qConds.ConfigType, GenericDBType.Int, SqlCondsSet.And);

            if (!string.IsNullOrEmpty(qConds.ConfigCode))
            {
                condsMain.Add("CONFIG_CODE", SqlCond.EqualTo, qConds.ConfigCode, GenericDBType.VarChar, SqlCondsSet.And);
            }

            return condsMain;
        }
        #endregion
        #endregion
    }
}
