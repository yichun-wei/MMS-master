//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Data;
//using System.Globalization;

//using EzCoding;
//using EzCoding.SystemEngines;
//using EzCoding.DB;
//using EzCoding.SystemEngines.EntityController;
//using Seec.Marketing.Erp;

//namespace Seec.Marketing.SystemEngines
//{
//    public partial class DBTableDefine
//    {
//        /// <summary>
//        /// ERP 倉庫。
//        /// </summary>
//        public const string ERP_WHSE = "ERP_WHSE";
//    }

//    /// <summary>
//    /// ERP 倉庫的類別實作。
//    /// </summary>
//    public class ErpWhse : SystemHandler
//    {
//        /// <summary>
//        /// 初始化 Seec.Marketing.SystemEngines.ErpWhse 類別的新執行個體。
//        /// 預設使用的 Primary Key 欄位名稱為「SID」及 EzCoding.SystemIdVerifier 系統代號驗證物件。
//        /// </summary>
//        /// <param name="connInfo">資料庫連線資訊。</param>
//        public ErpWhse(string connInfo)
//            : base(EntityHelper.GetEntity(connInfo, DBTableDefine.ERP_WHSE))
//        {
//            SqlOrder sorting = new SqlOrder();
//            sorting.Add("SECONDARY_INVENTORY_NAME", Sort.Ascending);

//            base.DefaultSqlOrder = sorting;
//        }

//        #region 表格資料
//        /// <summary>
//        /// 表格資料。
//        /// </summary>
//        public class Info : InfoBase, IErpWhse
//        {
//            /// <summary>
//            /// 系統代號。
//            /// </summary>
//            [SchemaMapping(Name = "SID", Type = ReturnType.SId)]
//            public ISystemId SId { get; set; }
//            /// <summary>
//            /// 建立日期時間。
//            /// </summary>
//            [SchemaMapping(Name = "CDT", Type = ReturnType.DateTime)]
//            public DateTime Cdt { get; set; }
//            /// <summary>
//            /// 修改日期時間。
//            /// </summary>
//            [SchemaMapping(Name = "MDT", Type = ReturnType.DateTime)]
//            public DateTime Mdt { get; set; }
//            /// <summary>
//            /// 建立人。
//            /// </summary>
//            [SchemaMapping(Name = "CSID", Type = ReturnType.SId)]
//            public ISystemId CSId { get; set; }
//            /// <summary>
//            /// 修改人。
//            /// </summary>
//            [SchemaMapping(Name = "MSID", Type = ReturnType.SId)]
//            public ISystemId MSId { get; set; }
//            /// <summary>
//            /// 刪除標記。
//            /// </summary>
//            [SchemaMapping(Name = "MDELED", Type = ReturnType.Bool)]
//            public bool MDeled { get; set; }
//            /// <summary>
//            /// 啟用。
//            /// </summary>
//            [SchemaMapping(Name = "ENABLED", Type = ReturnType.Bool)]
//            public bool Enabled { get; set; }
//            /// <summary>
//            /// 名稱。
//            /// </summary>
//            [SchemaMapping(Name = "SECONDARY_INVENTORY_NAME", Type = ReturnType.String)]
//            public string SecondaryInventoryName { get; set; }
//            /// <summary>
//            /// 倉庫 ID。
//            /// </summary>
//            [SchemaMapping(Name = "ORGANIZATION_ID", Type = ReturnType.Int)]
//            public int OrganizationId { get; set; }
//            /// <summary>
//            /// 屬性 15。
//            /// </summary>
//            [SchemaMapping(Name = "ATTRIBUTE15", Type = ReturnType.String)]
//            public string Attribute15 { get; set; }
//            /// <summary>
//            /// 市場範圍（1:內銷 2:外銷）。
//            /// </summary>
//            [SchemaMapping(Name = "MKTG_RANGE", Type = ReturnType.Int)]
//            public int MktgRange { get; set; }

//            #region 繫結資料
//            /// <summary>
//            /// 繫結資料。
//            /// </summary>
//            public static Info Binding(DataRow row)
//            {
//                return DBUtilBox.BindingRow<Info>(new Info(), row);
//            }

//            /// <summary>
//            /// 繫結資料。
//            /// </summary>
//            public static Info[] Binding(DataTable dTable)
//            {
//                List<Info> infos = new List<Info>();
//                foreach (DataRow row in dTable.Rows)
//                {
//                    infos.Add(Info.Binding(row));
//                }

//                return infos.ToArray();
//            }
//            #endregion
//        }
//        #endregion

//        #region 異動
//        #region Add
//        /// <summary>
//        /// 依指定的參數，新增一筆資料。
//        /// </summary>
//        /// <param name="actorSId">操作人系統代號。為客戶端在操作時的操作人系統代號。</param> 
//        /// <param name="erpWhseSId">指定的 ERP 倉庫系統代號。</param>
//        /// <param name="enabled">資料的啟用狀態。</param>
//        /// <param name="secondaryInventoryName">名稱。</param>
//        /// <param name="organizationId">倉庫 ID。</param>
//        /// <param name="attribute15">屬性 15。</param>
//        /// <param name="mktgRange">市場範圍（1:內銷 2:外銷）。</param>
//        /// <returns>EzCoding.Returner。若新增成功則回傳時包含著一個「NEW_SID」的 DataTable 和「TABLE_NAME」、「SID」兩個欄位。</returns>
//        /// <exception cref="System.ArgumentNullException">參數 actorSId 不允許為 null 值。</exception>
//        /// <exception cref="System.ArgumentNullException">參數 erpWhseSId 不允許為 null 值。</exception>
//        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
//        public Returner Add(ISystemId actorSId, ISystemId erpWhseSId, bool enabled, string secondaryInventoryName, int organizationId, string attribute15, int mktgRange)
//        {
//            if (erpWhseSId == null) { throw new ArgumentNullException("erpWhseSId"); }
//            string[] exceptionSIds = this.SystemIdVerifier.CheckValid(erpWhseSId);
//            if (exceptionSIds.Length > 0)
//            {
//                throw new LibraryException(string.Format("invaild ISystemId ({0}) format.", string.Join("、", exceptionSIds)));
//            }

//            Returner returner = new Returner();
//            try
//            {
//                #region 交易緩衝區
//                TransColValSet transSet = new TransColValSet();
//                transSet.SmartAdd("SID", erpWhseSId.ToString(), GenericDBType.Char, false);
//                transSet.SmartAdd("CSID", actorSId.ToString(), GenericDBType.Char, false);
//                transSet.SmartAdd("MSID", actorSId.ToString(), GenericDBType.Char, false);
//                transSet.SmartAdd("ENABLED", enabled ? "Y" : "N", GenericDBType.Char, false);
//                transSet.SmartAdd("SECONDARY_INVENTORY_NAME", secondaryInventoryName, GenericDBType.VarChar, false);
//                transSet.SmartAdd("ORGANIZATION_ID", organizationId, GenericDBType.Int);
//                transSet.SmartAdd("ATTRIBUTE15", attribute15, GenericDBType.VarChar, false);
//                transSet.SmartAdd("MKTG_RANGE", mktgRange, GenericDBType.Int);

//                returner.ChangeInto(base.Add(transSet, true));
//                #endregion

//                return returner;
//            }
//            finally
//            {
//            }
//        }
//        #endregion

//        #region Modify (僅匯入用)
//        /// <summary>
//        /// 依指定的參數，修改一筆 ERP 倉庫。
//        /// </summary>
//        /// <param name="actorSId">操作人系統代號。為客戶端在操作時的操作人系統代號。</param>
//        /// <param name="secondaryInventoryName">名稱。</param>
//        /// <param name="organizationId">倉庫 ID。</param>
//        /// <param name="enabled">資料的啟用狀態。</param>
//        /// <param name="attribute15">屬性 15。</param>
//        /// <param name="mktgRange">市場範圍（1:內銷 2:外銷）。</param>
//        /// <returns>EzCoding.Returner。</returns>
//        /// <exception cref="System.ArgumentNullException">參數 actorSId 不允許為 null 值。</exception>
//        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
//        public Returner Modify(ISystemId actorSId, string secondaryInventoryName, int organizationId, bool enabled, string attribute15, int mktgRange)
//        {
//            if (actorSId == null) { throw new ArgumentNullException("actorSId"); }
//            string[] exceptionSIds = this.SystemIdVerifier.CheckValid(actorSId);
//            if (exceptionSIds.Length > 0)
//            {
//                throw new LibraryException(string.Format("invaild ISystemId ({0}) format.", string.Join("、", exceptionSIds)));
//            }

//            Returner returner = new Returner();
//            try
//            {
//                #region 交易緩衝區
//                TransColValSet transSet = new TransColValSet();
//                transSet.SmartAdd("MDT", DateTime.Now, "yyyyMMddHHmmss", GenericDBType.Char);
//                transSet.SmartAdd("MSID", actorSId.ToString(), GenericDBType.Char, false);
//                transSet.SmartAdd("ENABLED", enabled ? "Y" : "N", GenericDBType.Char, false);
//                transSet.SmartAdd("ATTRIBUTE15", attribute15, GenericDBType.VarChar, false);
//                transSet.SmartAdd("MKTG_RANGE", mktgRange, GenericDBType.Int);

//                SqlCondsSet condsMain = new SqlCondsSet();
//                condsMain.Add("SECONDARY_INVENTORY_NAME", SqlCond.EqualTo, secondaryInventoryName, GenericDBType.VarChar, SqlCondsSet.And);
//                condsMain.Add("ORGANIZATION_ID", SqlCond.EqualTo, organizationId, GenericDBType.Int, SqlCondsSet.And);

//                returner.ChangeInto(base.Modify(transSet, condsMain, true));
//                #endregion

//                return returner;
//            }
//            finally
//            {
//            }
//        }
//        #endregion
//        #endregion

//        #region 一般查詢
//        #region 查詢條件
//        /// <summary>
//        /// 查詢條件。
//        /// </summary>
//        public class InfoConds
//        {
//            ///// <summary>
//            ///// 初始化 InfoConds 類別的新執行個體。
//            ///// 預設略過所有條件。
//            ///// </summary>
//            //public InfoConds()
//            //{

//            //}

//            /// <summary>
//            /// 初始化 InfoConds 類別的新執行個體。
//            /// </summary>
//            /// <param name="erpWhseSIds">ERP 倉庫系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
//            /// <param name="secondaryInventoryNames">名稱陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
//            /// <param name="mktgRanges">市場範圍陣列集合（1:內銷 2:外銷; 若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
//            public InfoConds(ISystemId[] erpWhseSIds, string[] secondaryInventoryNames, int[] mktgRanges)
//            {
//                this.ErpWhseSIds = erpWhseSIds;
//                this.SecondaryInventoryNames = secondaryInventoryNames;
//                this.MktgRanges = mktgRanges;
//            }

//            /// <summary>
//            /// ERP 倉庫系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
//            /// </summary>
//            public ISystemId[] ErpWhseSIds { get; set; }
//            /// <summary>
//            /// 名稱陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
//            /// </summary>
//            public string[] SecondaryInventoryNames { get; set; }
//            /// <summary>
//            /// 市場範圍陣列集合（1:內銷 2:外銷; 若為 null 或陣列長度等於 0 則略過條件檢查）。
//            /// </summary>
//            public int[] MktgRanges { get; set; }
//        }
//        #endregion

//        #region GetInfo
//        /// <summary> 
//        /// 依指定的參數取得資料。 
//        /// </summary> 
//        /// <param name="qConds">查詢條件。</param> 
//        /// <param name="size">指定的前 N 筆資料（若為 Int32.MaxValue 則表示全部）。</param>
//        /// <param name="sqlOrder">SQL 排序（Order By）的組合類別（若為 null 則使用預設排序）。</param>
//        /// <param name="includeScope">資料取得所包含的範圍（是否註解刪除或啟用中）。</param>
//        /// <param name="inquireColumns">含有零或多個指定查詢時所要使用的欄位名稱 System.String 陣列。若陣列為零，則使用預設開放的欄位名稱。</param>
//        /// <returns>EzCoding.Returner。</returns> 
//        public Returner GetInfo(InfoConds qConds, int size, SqlOrder sqlOrder, IncludeScope includeScope, params string[] inquireColumns)
//        {
//            Returner returner = new Returner();

//            SqlCondsSet condsMain = Handier.MergeDefaultCondsSet(includeScope);
//            condsMain.Add(GetInfoCondsSet(qConds));

//            base.Entity.EnableCustomEntity = true;
//            returner.ChangeInto(base.Entity.GetInfoBy(size, sqlOrder.UseDefault ? base.DefaultSqlOrder : sqlOrder, condsMain, inquireColumns));
//            base.Entity.EnableCustomEntity = false;
//            return returner;
//        }

//        /// <summary> 
//        /// 依指定的參數取得資料。 
//        /// </summary> 
//        /// <param name="qConds">查詢條件。</param> 
//        /// <param name="flipper">分頁操作參數集合。</param>
//        /// <param name="sqlOrder">SQL 排序（Order By）的組合類別（若為 null 則使用預設排序）。</param>
//        /// <param name="includeScope">資料取得所包含的範圍（是否註解刪除或啟用中）。</param>
//        /// <param name="inquireColumns">含有零或多個指定查詢時所要使用的欄位名稱 System.String 陣列。若陣列為零，則使用預設開放的欄位名稱。</param>
//        /// <returns>EzCoding.Returner。</returns> 
//        public Returner GetInfo(InfoConds qConds, PagingFlipper flipper, SqlOrder sqlOrder, IncludeScope includeScope, params string[] inquireColumns)
//        {
//            Returner returner = new Returner();

//            SqlCondsSet condsMain = Handier.MergeDefaultCondsSet(includeScope);
//            condsMain.Add(GetInfoCondsSet(qConds));

//            base.Entity.EnableCustomEntity = true;
//            returner.ChangeInto(base.Entity.GetPagingBy(flipper, sqlOrder.UseDefault ? base.DefaultSqlOrder : sqlOrder, condsMain, inquireColumns));
//            base.Entity.EnableCustomEntity = false;
//            return returner;
//        }
//        #endregion

//        #region GetInfoCount
//        /// <summary> 
//        /// 依指定的參數取得資料總數。 
//        /// </summary> 
//        /// <param name="qConds">查詢條件。</param> 
//        /// <param name="includeScope">資料取得所包含的範圍（是否註解刪除或啟用中）。</param> 
//        /// <returns>EzCoding.Returner。</returns> 
//        public Returner GetInfoCount(InfoConds qConds, IncludeScope includeScope)
//        {
//            Returner returner = new Returner();

//            SqlCondsSet condsMain = Handier.MergeDefaultCondsSet(includeScope);
//            condsMain.Add(GetInfoCondsSet(qConds));

//            base.Entity.EnableCustomEntity = true;
//            returner.ChangeInto(base.Entity.GetCountBy(condsMain));
//            base.Entity.EnableCustomEntity = false;
//            return returner;
//        }
//        #endregion

//        #region GetInfoByCompoundSearch
//        /// <summary> 
//        /// 依指定的參數取得資料（複合式查詢）。 
//        /// </summary> 
//        /// <param name="qConds">查詢條件。</param> 
//        /// <param name="columnsName">欄位名稱陣列。</param>
//        /// <param name="keyword">關鍵字。</param>
//        /// <param name="isAbsolute">是否為絕對查詢。若 false 則使用 like 查詢。</param>
//        /// <param name="flipper">分頁操作參數集合。</param>
//        /// <param name="sqlOrder">SQL 排序（Order By）的組合類別（若為 null 則使用預設排序）。</param>
//        /// <param name="includeScope">資料取得所包含的範圍（是否註解刪除或啟用中）。</param>
//        /// <param name="inquireColumns">含有零或多個指定查詢時所要使用的欄位名稱 System.String 陣列。若陣列為零，則使用預設開放的欄位名稱。</param>
//        /// <returns>EzCoding.Returner。</returns> 
//        public Returner GetInfoByCompoundSearch(InfoConds qConds, string[] columnsName, string keyword, bool isAbsolute, PagingFlipper flipper, SqlOrder sqlOrder, IncludeScope includeScope, params string[] inquireColumns)
//        {
//            Returner returner = new Returner();

//            base.Entity.EnableCustomEntity = true;
//            returner.ChangeInto(base.GetInfoByCompoundSearch(columnsName, keyword, EntityHelper.KEYWORD_SEPARATOR, isAbsolute, flipper, sqlOrder, GetInfoCondsSet(qConds), includeScope, inquireColumns));
//            base.Entity.EnableCustomEntity = false;
//            return returner;
//        }
//        #endregion

//        #region GetInfoByCompoundSearchCount
//        /// <summary> 
//        /// 依指定的參數取得資料總數（複合式查詢）。 
//        /// 若欄位名稱陣列長度等於0，則會略過指定欄位名稱查詢動作。 
//        /// </summary> 
//        /// <param name="qConds">查詢條件。</param> 
//        /// <param name="columnsName">欄位名稱陣列。</param> 
//        /// <param name="keyword">關鍵字。</param> 
//        /// <param name="isAbsolute">是否為絕對查詢。若 false 則使用 like 查詢。</param> 
//        /// <param name="includeScope">資料取得所包含的範圍（是否註解刪除或啟用中）。</param> 
//        /// <returns>EzCoding.Returner。</returns> 
//        public Returner GetInfoByCompoundSearchCount(InfoConds qConds, string[] columnsName, string keyword, bool isAbsolute, IncludeScope includeScope)
//        {
//            Returner returner = new Returner();

//            base.Entity.EnableCustomEntity = true;
//            returner.ChangeInto(base.GetInfoByCompoundSearchCount(columnsName, keyword, EntityHelper.KEYWORD_SEPARATOR, isAbsolute, GetInfoCondsSet(qConds), includeScope));
//            base.Entity.EnableCustomEntity = false;
//            return returner;
//        }
//        #endregion

//        #region GetGroupInfo
//        /// <summary> 
//        /// 依指定的欄位取得群組資料。 
//        /// </summary> 
//        /// <param name="qConds">查詢條件。</param> 
//        /// <param name="fieldNames">欄位名稱陣列集合。</param>
//        /// <param name="sort">排序方式。</param>
//        /// <returns>EzCoding.Returner。</returns>
//        public Returner GetGroupInfo(InfoConds qConds, string[] fieldNames, Sort sort)
//        {
//            SqlCondsSet condsMain = new SqlCondsSet();
//            condsMain.Add(GetInfoCondsSet(qConds));

//            SqlGroup grouping = new SqlGroup();
//            SqlOrder sorting = new SqlOrder();

//            for (int i = 0; i < fieldNames.Length; i++)
//            {
//                grouping.Add(fieldNames[i]);
//                sorting.Add(fieldNames[i], sort);
//            }

//            try
//            {
//                base.Entity.EnableCustomEntity = true;
//                return base.Entity.GetGroupBy(grouping, sorting, condsMain);
//            }
//            finally
//            {
//                base.Entity.EnableCustomEntity = false;
//            }
//        }
//        #endregion

//        #region GetInfoCondsSet
//        /// <summary> 
//        /// 依指定的條件取得 SqlCondsSet。 
//        /// </summary> 
//        /// <param name="qConds">查詢條件。</param> 
//        /// <returns>EzCoding.Collections.SqlCondsSet。</returns> 
//        SqlCondsSet GetInfoCondsSet(InfoConds qConds)
//        {
//            #region 自訂表格
//            var custEntity = new SqlSyntaxBuilder(base.Entity, new SqlSyntax());

//            custEntity.Append("SELECT [ERP_WHSE].* ");
//            custEntity.Append("FROM [ERP_WHSE] ");

//            var sqlConds = new List<string>();

//            if (qConds.ErpWhseSIds != null && qConds.ErpWhseSIds.Length > 0)
//            {
//                sqlConds.Add(custEntity.BuildConds(string.Empty, "SID", SqlOperator.EqualTo, SystemId.ToString(qConds.ErpWhseSIds), GenericDBType.Char));
//            }

//            if (qConds.SecondaryInventoryNames != null && qConds.SecondaryInventoryNames.Length > 0)
//            {
//                sqlConds.Add(custEntity.BuildConds(string.Empty, "SECONDARY_INVENTORY_NAME", SqlOperator.EqualTo, qConds.SecondaryInventoryNames, GenericDBType.VarChar));
//            }

//            if (qConds.MktgRanges != null && qConds.MktgRanges.Length > 0)
//            {
//                sqlConds.Add(custEntity.BuildConds(string.Empty, "MKTG_RANGE", SqlOperator.EqualTo, qConds.MktgRanges.Select(q => (object)q).ToArray(), GenericDBType.Int));
//            }

//            custEntity.Append(SqlUtilBox.ToSqlConds(sqlConds.Where(q => !string.IsNullOrWhiteSpace(q)).ToArray(), " WHERE ", string.Empty));
//            base.Entity.CustomEntitySqlSyntax = custEntity.GetSqlSyntax();
//            #endregion

//            SqlCondsSet condsMain = new SqlCondsSet();

//            return condsMain;
//        }
//        #endregion
//        #endregion
//    }
//}
