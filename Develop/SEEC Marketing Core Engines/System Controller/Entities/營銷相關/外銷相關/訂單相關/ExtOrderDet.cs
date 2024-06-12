using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Transactions;
using System.Globalization;

using EzCoding;
using EzCoding.SystemEngines;
using EzCoding.DB;
using EzCoding.SystemEngines.EntityController;

namespace Seec.Marketing.SystemEngines
{
    public partial class DBTableDefine
    {
        /// <summary>
        /// 外銷訂單明細。
        /// </summary>
        public const string EXT_ORDER_DET = "EXT_ORDER_DET";
    }

    /// <summary>
    /// 外銷訂單明細的類別實作。
    /// </summary>
    public class ExtOrderDet : SystemHandler
    {
        /// <summary>
        /// 初始化 Seec.Marketing.SystemEngines.UserGrp 類別的新執行個體。
        /// 預設使用的 Primary Key 欄位名稱為「SID」及 EzCoding.SystemIdVerifier 系統代號驗證物件。
        /// </summary>
        /// <param name="connInfo">資料庫連線資訊。</param>
        public ExtOrderDet(string connInfo)
            : base(EntityHelper.GetEntity(connInfo, DBTableDefine.EXT_ORDER_DET))
        {
            base.DefaultSqlOrder = new SqlOrder("SORT", Sort.Ascending);
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
            /// 外銷訂單系統代號。
            /// </summary>
            [SchemaMapping(Name = "EXT_ORDER_SID", Type = ReturnType.SId)]
            public ISystemId ExtOrderSId { get; set; }
            /// <summary>
            /// 來源（1:一般品項 2:手動新增）。
            /// </summary>
            [SchemaMapping(Name = "SOURCE", Type = ReturnType.Int)]
            public int Source { get; set; }
            /// <summary>
            /// 型號。
            /// </summary>
            [SchemaMapping(Name = "MODEL", Type = ReturnType.String)]
            public string Model { get; set; }
            /// <summary>
            /// 料號。
            /// </summary>
            [SchemaMapping(Name = "PART_NO", Type = ReturnType.String)]
            public string PartNo { get; set; }
            /// <summary>
            /// 數量。
            /// </summary>
            [SchemaMapping(Name = "QTY", Type = ReturnType.Int)]
            public int Qty { get; set; }
            /// <summary>
            /// 牌價。
            /// </summary>
            [SchemaMapping(Name = "LIST_PRICE", Type = ReturnType.Float, AllowNull = true)]
            public float? ListPrice { get; set; }
            /// <summary>
            /// 單價。
            /// </summary>
            [SchemaMapping(Name = "UNIT_PRICE", Type = ReturnType.Float)]
            public float UnitPrice { get; set; }
            /// <summary>
            /// 折扣。
            /// </summary>
            [SchemaMapping(Name = "DISCOUNT", Type = ReturnType.Float, AllowNull = true)]
            public float? Discount { get; set; }
            /// <summary>
            /// 實付金額。
            /// </summary>
            [SchemaMapping(Name = "PAID_AMT", Type = ReturnType.Float)]
            public float PaidAmt { get; set; }
            /// <summary>
            /// 備註。
            /// </summary>
            [SchemaMapping(Name = "RMK", Type = ReturnType.String)]
            public string Rmk { get; set; }
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
            /// 外銷訂單系統代號。
            /// </summary>
            public ISystemId ExtOrderSId { get; set; }
            /// <summary>
            /// 來源（1:一般品項 2:手動新增）。
            /// </summary>
            public int? Source { get; set; }
            /// <summary>
            /// 型號。
            /// </summary>
            public string Model { get; set; }
            /// <summary>
            /// 料號。
            /// </summary>
            public string PartNo { get; set; }
            /// <summary>
            /// 數量。
            /// </summary>
            public int? Qty { get; set; }
            /// <summary>
            /// 牌價。
            /// </summary>
            public float? ListPrice { get; set; }
            /// <summary>
            /// 單價。
            /// </summary>
            public float? UnitPrice { get; set; }
            /// <summary>
            /// 折扣。
            /// </summary>
            public float? Discount { get; set; }
            /// <summary>
            /// 實付金額。
            /// </summary>
            public float? PaidAmt { get; set; }
            /// <summary>
            /// 備註。
            /// </summary>
            public string Rmk { get; set; }
            /// <summary>
            /// 排序。
            /// </summary>
            public int? Sort { get; set; }
        }
        #endregion

        #region 異動
        #region Add
        /// <summary>
        /// 依指定的參數，新增一筆資料。
        /// </summary>
        /// <param name="actorSId">操作人系統代號。為客戶端在操作時的操作人系統代號。</param> 
        /// <param name="extOrderDetSId">指定的外銷訂單明細系統代號。</param>
        /// <param name="extOrderSId">外銷訂單系統代號。</param>
        /// <param name="source">來源（1:一般品項 2:手動新增）。</param>
        /// <param name="model">型號（null 或 empty 將自動略過操作）。</param>
        /// <param name="partNo">料號。</param>
        /// <param name="qty">數量。</param>
        /// <param name="listPrice">牌價（null 將自動略過操作）。</param>
        /// <param name="unitPrice">單價。</param>
        /// <param name="discount">折扣（null 將自動略過操作）。</param>
        /// <param name="paidAmt">實付金額。</param>
        /// <param name="rmk">備註（null 或 empty 將自動略過操作）。</param>
        /// <param name="sort">排序。</param>
        /// <returns>EzCoding.Returner。若新增成功則回傳時包含著一個「NEW_SID」的 DataTable 和「TABLE_NAME」、「SID」兩個欄位。</returns>
        /// <exception cref="System.ArgumentNullException">參數 actorSId 不允許為 null 值。</exception>
        /// <exception cref="System.ArgumentNullException">參數 extOrderDetSId 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        public Returner Add(ISystemId actorSId, ISystemId extOrderDetSId, ISystemId extOrderSId, int source, string model, string partNo, int qty, float? listPrice, float unitPrice, float? discount, float paidAmt, string rmk, int sort)
        {
            if (actorSId == null) { throw new ArgumentNullException("actorSId"); }
            if (extOrderDetSId == null) { throw new ArgumentNullException("extOrderDetSId"); }
            string[] exceptionSIds = this.SystemIdVerifier.CheckValid(actorSId, extOrderDetSId);
            if (exceptionSIds.Length > 0)
            {
                throw new LibraryException(string.Format("invaild ISystemId ({0}) format.", string.Join("、", exceptionSIds)));
            }

            Returner returner = new Returner();
            try
            {
                #region 交易緩衝區
                TransColValSet transSet = new TransColValSet();
                transSet.SmartAdd("SID", extOrderDetSId.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("CSID", actorSId.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("MSID", actorSId.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("EXT_ORDER_SID", extOrderSId.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("SOURCE", source, GenericDBType.Int);
                transSet.SmartAdd("MODEL", model, GenericDBType.NVarChar, true);
                transSet.SmartAdd("PART_NO", partNo, GenericDBType.VarChar, true);
                transSet.SmartAdd("QTY", qty, GenericDBType.Int);
                transSet.SmartAdd("LIST_PRICE", listPrice, GenericDBType.Float);
                transSet.SmartAdd("UNIT_PRICE", unitPrice, GenericDBType.Float);
                transSet.SmartAdd("DISCOUNT", discount, GenericDBType.Float);
                transSet.SmartAdd("PAID_AMT", paidAmt, GenericDBType.Float);
                transSet.SmartAdd("RMK", rmk, GenericDBType.NVarChar, true);
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
        /// 依指定的參數，修改一筆外銷訂單明細。
        /// </summary>
        /// <param name="actorSId">操作人系統代號。為客戶端在操作時的操作人系統代號。</param>
        /// <param name="extOrderDetSId">外銷訂單明細系統代號。</param>
        /// <param name="source">來源（1:一般品項 2:手動新增）。</param>
        /// <param name="model">型號（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="partNo">料號。</param>
        /// <param name="qty">數量。</param>
        /// <param name="listPrice">牌價（null 則直接設為 DBNull）。</param>
        /// <param name="unitPrice">單價。</param>
        /// <param name="discount">折扣（null 則直接設為 DBNull）。</param>
        /// <param name="paidAmt">實付金額。</param>
        /// <param name="rmk">備註（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="sort">排序。</param>
        /// <returns>EzCoding.Returner。</returns>
        /// <exception cref="System.ArgumentNullException">參數 actorSId 不允許為 null 值。</exception>
        /// <exception cref="System.ArgumentNullException">參數 extOrderDetSId 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        public Returner Modify(ISystemId actorSId, ISystemId extOrderDetSId, int source, string model, string partNo, int qty, float? listPrice, float unitPrice, float? discount, float paidAmt, string rmk, int sort)
        {
            if (actorSId == null) { throw new ArgumentNullException("actorSId"); }
            if (extOrderDetSId == null) { throw new ArgumentNullException("extOrderDetSId"); }
            string[] exceptionSIds = this.SystemIdVerifier.CheckValid(actorSId, extOrderDetSId);
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
                transSet.SmartAdd("SOURCE", source, GenericDBType.Int);
                transSet.SmartAdd("MODEL", model, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("PART_NO", partNo, GenericDBType.VarChar, true);
                transSet.SmartAdd("QTY", qty, GenericDBType.Int);
                transSet.SmartAdd("LIST_PRICE", listPrice, GenericDBType.Float, true);
                transSet.SmartAdd("UNIT_PRICE", unitPrice, GenericDBType.Float);
                transSet.SmartAdd("DISCOUNT", discount, GenericDBType.Float, true);
                transSet.SmartAdd("PAID_AMT", paidAmt, GenericDBType.Float);
                transSet.SmartAdd("RMK", rmk, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("SORT", sort, GenericDBType.Int);

                SqlCondsSet condsMain = new SqlCondsSet();
                condsMain.Add(base.PrimaryKey, SqlCond.EqualTo, extOrderDetSId.ToString(), GenericDBType.Char, string.Empty);

                returner.ChangeInto(base.Modify(transSet, condsMain, true));
                #endregion

                return returner;
            }
            finally
            {
            }
        }
        #endregion

        #region DeleteByExtOrderSId
        /// <summary>
        /// 依指定的外銷訂單系統代號刪除資料。
        /// </summary>
        /// <param name="extOrderSIds">外銷訂單系統代號陣列集合。</param> 
        /// <returns>EzCoding.Returner。</returns>
        public Returner DeleteByExtOrderSId(ISystemId[] extOrderSIds)
        {
            if (extOrderSIds == null || extOrderSIds.Length == 0)
            {
                return new Returner();
            }

            Returner returner = new Returner();

            try
            {
                SqlCondsSet condsMain = new SqlCondsSet();
                condsMain.Add("EXT_ORDER_SID", true, SystemId.ToString(extOrderSIds), GenericDBType.Char, SqlCondsSet.And);

                returner.ChangeInto(base.Delete(condsMain, true));

                return returner;
            }
            finally
            {
                if (returner != null) { returner.Dispose(); }
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
            /// <param name="extOrderDetSIds">外銷訂單明細系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="extOrderSId">外銷訂單系統代號（若為 null 則略過條件檢查）。</param>
            /// <param name="source">來源（1:一般品項 2:手動新增; 若為 null 則略過條件檢查）。</param>
            /// <param name="partNo">料號（若為 null 或 empty 則略過條件檢查）。</param>
            public InfoConds(ISystemId[] extOrderDetSIds, ISystemId extOrderSId, int? source, string partNo)
            {
                this.ExtQuotnDetSIds = extOrderDetSIds;
                this.ExtOrderSId = extOrderSId;
                this.Source = source;
                this.PartNo = partNo;
            }

            /// <summary>
            /// 外銷訂單明細系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public ISystemId[] ExtQuotnDetSIds { get; set; }
            /// <summary>
            /// 外銷訂單系統代號（若為 null 則略過條件檢查）。
            /// </summary>
            public ISystemId ExtOrderSId { get; set; }
            /// <summary>
            /// 來源（1:一般品項 2:手動新增; 若為 null 則略過條件檢查）。
            /// </summary>
            public int? Source { get; set; }
            /// <summary>
            /// 料號（若為 null 或 empty 則略過條件檢查）。
            /// </summary>
            public string PartNo { get; set; }
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

            custEntity.Append("SELECT [EXT_ORDER_DET].* ");
            custEntity.Append("FROM [EXT_ORDER_DET] ");

            var sqlConds = new List<string>();

            if (qConds.ExtQuotnDetSIds != null && qConds.ExtQuotnDetSIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "SID", SqlOperator.EqualTo, SystemId.ToString(qConds.ExtQuotnDetSIds), GenericDBType.Char));
            }

            if (qConds.ExtOrderSId != null)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "EXT_ORDER_SID", SqlOperator.EqualTo, qConds.ExtOrderSId.Value, GenericDBType.Char));
            }

            if (qConds.Source.HasValue)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "SOURCE", SqlOperator.EqualTo, qConds.Source.Value, GenericDBType.Int));
            }

            if (!string.IsNullOrWhiteSpace(qConds.PartNo))
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "PART_NO", SqlOperator.EqualTo, qConds.PartNo, GenericDBType.VarChar));
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
        #region 表格資料
        /// <summary>
        /// 表格資料。
        /// </summary>
        public class InfoView : Info
        {
            /// <summary>
            /// 報價單編號。
            /// </summary>
            [SchemaMapping(Name = "ODR_NO", Type = ReturnType.String)]
            public string OdrNo { get; set; }
            /// <summary>
            /// 是否已取消。
            /// </summary>
            [SchemaMapping(Name = "IS_CANCEL", Type = ReturnType.Bool)]
            public bool IsCancel { get; set; }
            /// <summary>
            /// 料號 ID。
            /// </summary>
            [SchemaMapping(Name = "INVENTORY_ITEM_ID", Type = ReturnType.Long, AllowNull = true)]
            public long? InventoryItemId { get; set; }
            /// <summary>
            /// 料號摘要。
            /// </summary>
            [SchemaMapping(Name = "SUMMARY", Type = ReturnType.String)]
            public string Summary { get; set; }
            /// <summary>
            /// 料號重量。
            /// </summary>
            [SchemaMapping(Name = "UNIT_WEIGHT", Type = ReturnType.Float, AllowNull = true)]
            public float? UnitWeight { get; set; }
            /// <summary>
            /// 重量單位。
            /// </summary>
            [SchemaMapping(Name = "WEIGHT_UOM_CODE", Type = ReturnType.String)]
            public string WeightUomCode { get; set; }
            /// <summary>
            /// 料號所屬製造組織代碼。
            /// </summary>
            [SchemaMapping(Name = "ORGANIZATION_CODE", Type = ReturnType.String)]
            public string OrganizationCode { get; set; }
            /// <summary>
            /// 累計生產量。
            /// 同報價單目前所有訂單版本的已確認生產單同品項加總。
            /// </summary>
            [SchemaMapping(Name = "CUM_PROD_QTY", Type = ReturnType.Int)]
            public int CumProdQty { get; set; }
            /// <summary>
            /// 外銷出貨單使用量。
            /// </summary>
            [SchemaMapping(Name = "SHIP_ODR_USE_QTY", Type = ReturnType.Int)]
            public int ShipOdrUseQty { get; set; }

            #region 繫結資料
            /// <summary>
            /// 繫結資料。
            /// </summary>
            new public static InfoView Binding(DataRow row)
            {
                return DBUtilBox.BindingRow<InfoView>(new InfoView(), row);
            }

            /// <summary>
            /// 繫結資料。
            /// </summary>
            new public static InfoView[] Binding(DataTable dTable)
            {
                List<InfoView> infos = new List<InfoView>();
                foreach (DataRow row in dTable.Rows)
                {
                    infos.Add(InfoView.Binding(row));
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
        public class InfoViewConds
        {
            ///// <summary>
            ///// 初始化 InfoViewConds 類別的新執行個體。
            ///// 預設略過所有條件。
            ///// </summary>
            //public InfoViewConds()
            //{

            //}

            /// <summary>
            /// 初始化 InfoViewConds 類別的新執行個體。
            /// </summary>
            /// <param name="extOrderDetSIds">外銷訂單明細系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="customerId">客戶 ID（若為 null 則略過條件檢查）。</param>
            /// <param name="extOrderSIds">外銷訂單系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="source">來源（1:一般品項 2:手動新增; 若為 null 則略過條件檢查）。</param>
            /// <param name="partNo">料號（若為 null 或 empty 則略過條件檢查）。</param>
            /// <param name="isCancel">是否已取消（若為 null 則略過條件檢查）。</param>
            /// <param name="onlyAvailable">是否僅取得尚有可用數量的外銷訂單，若否則取得全部。</param>
            /// <param name="includeScope">資料取得所包含的範圍（是否註解刪除或啟用中）。</param>
            public InfoViewConds(ISystemId[] extOrderDetSIds, long? customerId, ISystemId[] extOrderSIds, int? source, string partNo, bool? isCancel, bool onlyAvailable, IncludeScope includeScope)
            {
                this.ExtQuotnDetSIds = extOrderDetSIds;
                this.CustomerId = customerId;
                this.ExtOrderSIds = extOrderSIds;
                this.Source = source;
                this.PartNo = partNo;
                this.IsCancel = isCancel;
                this.OnlyAvailable = onlyAvailable;
                this.IncludeScope = includeScope;
            }

            /// <summary>
            /// 外銷訂單明細系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public ISystemId[] ExtQuotnDetSIds { get; set; }
            /// <summary>
            /// 客戶 ID（若為 null 則略過條件檢查）。
            /// </summary>
            public long? CustomerId { get; set; }
            /// <summary>
            /// 外銷訂單系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public ISystemId[] ExtOrderSIds { get; set; }
            /// <summary>
            /// 來源（1:一般品項 2:手動新增; 若為 null 則略過條件檢查）。
            /// </summary>
            public int? Source { get; set; }
            /// <summary>
            /// 料號（若為 null 或 empty 則略過條件檢查）。
            /// </summary>
            public string PartNo { get; set; }
            /// <summary>
            /// 是否已取消（若為 null 則略過條件檢查）。
            /// </summary>
            public bool? IsCancel { get; set; }
            /// <summary>
            /// 是否僅取得尚有可用數量的專案報價，若否則取得全部。
            /// </summary>
            public bool OnlyAvailable { get; set; }
            /// <summary>
            /// 資料取得所包含的範圍（是否註解刪除或啟用中）。
            /// </summary>
            public IncludeScope IncludeScope { get; set; }
        }
        #endregion

        #region GetInfoView
        /// <summary> 
        /// 依指定的參數取得資料。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <param name="size">指定的前 N 筆資料（若為 Int32.MaxValue 則表示全部）。</param>
        /// <param name="sqlOrder">SQL 排序（Order By）的組合類別（若為 null 則使用預設排序）。</param>
        /// <param name="inquireColumns">含有零或多個指定查詢時所要使用的欄位名稱 System.String 陣列。若陣列為零，則使用預設開放的欄位名稱。</param>
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetInfoView(InfoViewConds qConds, int size, SqlOrder sqlOrder, params string[] inquireColumns)
        {
            Returner returner = new Returner();

            SqlCondsSet condsMain = new SqlCondsSet();
            condsMain.Add(GetInfoViewCondsSet(qConds));

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
        public Returner GetInfoView(InfoViewConds qConds, PagingFlipper flipper, SqlOrder sqlOrder, params string[] inquireColumns)
        {
            Returner returner = new Returner();

            SqlCondsSet condsMain = new SqlCondsSet();
            condsMain.Add(GetInfoViewCondsSet(qConds));

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.Entity.GetPagingBy(flipper, sqlOrder.UseDefault ? base.DefaultSqlOrder : sqlOrder, condsMain, inquireColumns));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetInfoViewCount
        /// <summary> 
        /// 依指定的參數取得資料總數。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetInfoViewCount(InfoViewConds qConds)
        {
            Returner returner = new Returner();

            SqlCondsSet condsMain = new SqlCondsSet();
            condsMain.Add(GetInfoViewCondsSet(qConds));

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.Entity.GetCountBy(condsMain));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetInfoViewByCompoundSearch
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
        public Returner GetInfoViewByCompoundSearch(InfoViewConds qConds, string[] columnsName, string keyword, bool isAbsolute, PagingFlipper flipper, SqlOrder sqlOrder, params string[] inquireColumns)
        {
            Returner returner = new Returner();

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.GetInfoByCompoundSearch(columnsName, keyword, EntityHelper.KEYWORD_SEPARATOR, isAbsolute, flipper, sqlOrder, GetInfoViewCondsSet(qConds), inquireColumns));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetInfoViewByCompoundSearchCount
        /// <summary> 
        /// 依指定的參數取得資料總數（複合式查詢）。 
        /// 若欄位名稱陣列長度等於0，則會略過指定欄位名稱查詢動作。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <param name="columnsName">欄位名稱陣列。</param> 
        /// <param name="keyword">關鍵字。</param> 
        /// <param name="isAbsolute">是否為絕對查詢。若 false 則使用 like 查詢。</param> 
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetInfoViewByCompoundSearchCount(InfoViewConds qConds, string[] columnsName, string keyword, bool isAbsolute)
        {
            Returner returner = new Returner();

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.GetInfoByCompoundSearchCount(columnsName, keyword, EntityHelper.KEYWORD_SEPARATOR, isAbsolute, GetInfoViewCondsSet(qConds)));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetInfoViewCondsSet
        /// <summary> 
        /// 依指定的條件取得 SqlCondsSet。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <returns>EzCoding.Collections.SqlCondsSet。</returns> 
        SqlCondsSet GetInfoViewCondsSet(InfoViewConds qConds)
        {
            #region 自訂表格
            var custEntity = new SqlSyntaxBuilder(base.Entity, new SqlSyntax());

            //custEntity.Append("SELECT [ODR_DET].* ");
            //custEntity.Append("       , [QUOTN].[ODR_NO], [QUOTN].[IS_CANCEL] ");
            //custEntity.Append("       , [INV].[INVENTORY_ITEM_ID], [INV].[DESCRIPTION] AS [SUMMARY], [INV].[UNIT_WEIGHT], [INV].[WEIGHT_UOM_CODE], [INV].[ORGANIZATION_CODE] ");
            ////取外銷出貨單的使用量 (不取已註刪的資料)
            //custEntity.Append("       , (SELECT COALESCE(SUM([EXT_SHIPPING_ORDER_DET].[QTY]), 0) AS [CNT] FROM [EXT_SHIPPING_ORDER_DET] INNER JOIN [EXT_SHIPPING_ORDER] [SHIPPING] ON [SHIPPING].[SID] = [EXT_SHIPPING_ORDER_DET].[EXT_SHIPPING_ORDER_SID] AND [SHIPPING].[MDELED] = 'N' WHERE [EXT_SHIPPING_ORDER_DET].[EXT_ORDER_DET_SID] = [ODR_DET].[SID]) AS [SHIP_ODR_USE_QTY] ");
            //custEntity.Append("FROM [EXT_ORDER_DET] [ODR_DET] ");
            //custEntity.Append("     INNER JOIN [EXT_ORDER] [ODR] ON [ODR].[SID] = [ODR_DET].[EXT_ORDER_SID] ");
            //custEntity.Append("     INNER JOIN [EXT_QUOTN] [QUOTN] ON [QUOTN].[SID] = [ODR].[EXT_QUOTN_SID] ");
            ////因有手動品項, 不能使用「INNER JOIN」.
            //custEntity.Append("     LEFT JOIN [ERP_INV] [INV] ON [INV].[ITEM] = [ODR_DET].[PART_NO] ");

            custEntity.Append(@"
                SELECT [ODR_DET].* 
                       , [QUOTN].[ODR_NO], [QUOTN].[IS_CANCEL] 
                       , [INV].[INVENTORY_ITEM_ID], [INV].[DESCRIPTION] AS [SUMMARY], [INV].[UNIT_WEIGHT], [INV].[WEIGHT_UOM_CODE], [INV].[ORGANIZATION_CODE] 

	                   --累計生產量 (已確認生產), 同報價單所有同品項.
	                   , (SELECT COALESCE(SUM([CUM_PROD_DET].[PROD_QTY]), 0) FROM [EXT_PROD_ORDER_DET] [CUM_PROD_DET] INNER JOIN [EXT_PROD_ORDER] [CUM_PROD] ON [CUM_PROD].[SID] = [CUM_PROD_DET].[EXT_PROD_ORDER_SID] INNER JOIN [EXT_ORDER] [CUM_ODR] ON [CUM_ODR].[SID] = [CUM_PROD].[EXT_ORDER_SID] WHERE [QUOTN].[SID] = [CUM_ODR].[EXT_QUOTN_SID] AND [CUM_PROD].[STATUS] = 2 AND [CUM_PROD_DET].[PART_NO] = [ODR_DET].[PART_NO]) AS [CUM_PROD_QTY]

                       --取外銷出貨單的使用量 (不取已註刪的資料)
                       , (SELECT COALESCE(SUM([EXT_SHIPPING_ORDER_DET].[QTY]), 0) AS [CNT] FROM [EXT_SHIPPING_ORDER_DET] INNER JOIN [EXT_SHIPPING_ORDER] [SHIPPING] ON [SHIPPING].[SID] = [EXT_SHIPPING_ORDER_DET].[EXT_SHIPPING_ORDER_SID] AND [SHIPPING].[MDELED] = 'N' WHERE [EXT_SHIPPING_ORDER_DET].[EXT_ORDER_DET_SID] = [ODR_DET].[SID]) AS [SHIP_ODR_USE_QTY] 

                FROM [EXT_ORDER_DET] [ODR_DET] 
                     INNER JOIN [EXT_ORDER] [ODR] ON [ODR].[SID] = [ODR_DET].[EXT_ORDER_SID] 
                     INNER JOIN [EXT_QUOTN] [QUOTN] ON [QUOTN].[SID] = [ODR].[EXT_QUOTN_SID] 
                     --因有手動品項, 不能使用「INNER JOIN」.
                     LEFT JOIN [ERP_INV] [INV] ON [INV].[ITEM] = [ODR_DET].[PART_NO] 
            ");


            var sqlConds = new List<string>();

            if (qConds.ExtQuotnDetSIds != null && qConds.ExtQuotnDetSIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds("ODR_DET", "SID", SqlOperator.EqualTo, SystemId.ToString(qConds.ExtQuotnDetSIds), GenericDBType.Char));
            }

            if (qConds.CustomerId.HasValue)
            {
                sqlConds.Add(custEntity.BuildConds("ODR", "CUSTOMER_ID", SqlOperator.EqualTo, qConds.CustomerId.Value, GenericDBType.BigInt));
            }

            if (qConds.ExtOrderSIds != null && qConds.ExtOrderSIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds("ODR_DET", "EXT_ORDER_SID", SqlOperator.EqualTo, SystemId.ToString(qConds.ExtOrderSIds), GenericDBType.Char));
            }

            if (qConds.Source.HasValue)
            {
                sqlConds.Add(custEntity.BuildConds("ODR_DET", "SOURCE", SqlOperator.EqualTo, qConds.Source.Value, GenericDBType.Int));
            }

            if (!string.IsNullOrWhiteSpace(qConds.PartNo))
            {
                sqlConds.Add(custEntity.BuildConds("ODR_DET", "PART_NO", SqlOperator.EqualTo, qConds.PartNo, GenericDBType.VarChar));
            }

            if (qConds.IsCancel.HasValue)
            {
                sqlConds.Add(custEntity.BuildConds("QUOTN", "IS_CANCEL", SqlOperator.EqualTo, qConds.IsCancel.Value ? "Y" : "N", GenericDBType.Char));
            }

            sqlConds.Add(Handier.ToSysStatusSqlSyntax(qConds.IncludeScope, "ODR"));
            //sqlConds.Add(Handier.ToSysStatusSqlSyntax(qConds.IncludeScope, "ODR_DET"));

            custEntity.Append(SqlUtilBox.ToSqlConds(sqlConds.Where(q => !string.IsNullOrWhiteSpace(q)).ToArray(), " WHERE ", string.Empty));
            base.Entity.CustomEntitySqlSyntax = custEntity.GetSqlSyntax();
            #endregion

            SqlCondsSet condsMain = new SqlCondsSet();

            if (qConds.OnlyAvailable)
            {
                condsMain.Add(new LeftRightPair<string, string>("QTY", "SHIP_ODR_USE_QTY"), SqlCond.GreaterThan, SqlCondsSet.And);
            }

            return condsMain;
        }
        #endregion
        #endregion

        #region 檢視查詢 (for 「依品項建立」出貨單選擇品項用 - 群組序號)
        #region 表格資料
        /// <summary>
        /// 表格資料。
        /// </summary>
        public class CrossOrderItemsGrpInfoView : Info
        {
            /// <summary>
            /// 料號 ID。
            /// </summary>
            [SchemaMapping(Name = "INVENTORY_ITEM_ID", Type = ReturnType.Long, AllowNull = true)]
            public long? InventoryItemId { get; set; }
            /// <summary>
            /// 料號摘要。
            /// </summary>
            [SchemaMapping(Name = "SUMMARY", Type = ReturnType.String)]
            public string Summary { get; set; }
            /// <summary>
            /// 料號重量。
            /// </summary>
            [SchemaMapping(Name = "UNIT_WEIGHT", Type = ReturnType.Float, AllowNull = true)]
            public float? UnitWeight { get; set; }
            /// <summary>
            /// 重量單位。
            /// </summary>
            [SchemaMapping(Name = "WEIGHT_UOM_CODE", Type = ReturnType.String)]
            public string WeightUomCode { get; set; }
            /// <summary>
            /// 料號所屬製造組織代碼。
            /// </summary>
            [SchemaMapping(Name = "ORGANIZATION_CODE", Type = ReturnType.String)]
            public string OrganizationCode { get; set; }
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

            #region 繫結資料
            /// <summary>
            /// 繫結資料。
            /// </summary>
            new public static CrossOrderItemsGrpInfoView Binding(DataRow row)
            {
                return DBUtilBox.BindingRow<CrossOrderItemsGrpInfoView>(new CrossOrderItemsGrpInfoView(), row);
            }

            /// <summary>
            /// 繫結資料。
            /// </summary>
            new public static CrossOrderItemsGrpInfoView[] Binding(DataTable dTable)
            {
                List<CrossOrderItemsGrpInfoView> infos = new List<CrossOrderItemsGrpInfoView>();
                foreach (DataRow row in dTable.Rows)
                {
                    infos.Add(CrossOrderItemsGrpInfoView.Binding(row));
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
        public class CrossOrderItemsGrpInfoViewConds
        {
            ///// <summary>
            ///// 初始化 CrossOrderItemsGrpInfoViewConds 類別的新執行個體。
            ///// 預設略過所有條件。
            ///// </summary>
            //public CrossOrderItemsGrpInfoViewConds()
            //{

            //}

            /// <summary>
            /// 初始化 CrossOrderItemsGrpInfoViewConds 類別的新執行個體。
            /// </summary>
            /// <param name="extOrderDetSIds">外銷訂單明細系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="customerId">客戶 ID（若為 null 則略過條件檢查）。</param>
            /// <param name="extOrderSIds">外銷訂單系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="partNos">料號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="exportItemType">產品別（若為 null 或 empty 則略過條件檢查）。</param>
            /// <param name="category1">分類 1（若為 null 或 empty 則略過條件檢查）。</param>
            /// <param name="category2">分類 2（若為 null 或 empty 則略過條件檢查）。</param>
            /// <param name="category3">分類 3（若為 null 或 empty 則略過條件檢查）。</param>
            /// <param name="category4">分類 4（若為 null 或 empty 則略過條件檢查）。</param>
            /// <param name="category5">分類 5（若為 null 或 empty 則略過條件檢查）。</param>
            public CrossOrderItemsGrpInfoViewConds(ISystemId[] extOrderDetSIds, long? customerId, ISystemId[] extOrderSIds, string[] partNos, string exportItemType, string category1, string category2, string category3, string category4, string category5)
            {
                this.ExtQuotnDetSIds = extOrderDetSIds;
                this.CustomerId = customerId;
                this.ExtOrderSIds = extOrderSIds;
                this.PartNos = partNos;
                this.ExportItemType = exportItemType;
                this.Category1 = category1;
                this.Category2 = category2;
                this.Category3 = category3;
                this.Category4 = category4;
                this.Category5 = category5;
            }

            /// <summary>
            /// 外銷訂單明細系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public ISystemId[] ExtQuotnDetSIds { get; set; }
            /// <summary>
            /// 客戶 ID（若為 null 則略過條件檢查）。
            /// </summary>
            public long? CustomerId { get; set; }
            /// <summary>
            /// 外銷訂單系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public ISystemId[] ExtOrderSIds { get; set; }
            /// <summary>
            /// 料號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public string[] PartNos { get; set; }
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
        }
        #endregion

        #region GetCrossOrderItemsGrpInfoView
        /// <summary> 
        /// 依指定的參數取得資料。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <param name="size">指定的前 N 筆資料（若為 Int32.MaxValue 則表示全部）。</param>
        /// <param name="sqlOrder">SQL 排序（Order By）的組合類別（若為 null 則使用預設排序）。</param>
        /// <param name="inquireColumns">含有零或多個指定查詢時所要使用的欄位名稱 System.String 陣列。若陣列為零，則使用預設開放的欄位名稱。</param>
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetCrossOrderItemsGrpInfoView(CrossOrderItemsGrpInfoViewConds qConds, int size, SqlOrder sqlOrder, params string[] inquireColumns)
        {
            Returner returner = new Returner();

            SqlCondsSet condsMain = new SqlCondsSet();
            condsMain.Add(GetCrossOrderItemsGrpInfoViewCondsSet(qConds));

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
        public Returner GetCrossOrderItemsGrpInfoView(CrossOrderItemsGrpInfoViewConds qConds, PagingFlipper flipper, SqlOrder sqlOrder, params string[] inquireColumns)
        {
            Returner returner = new Returner();

            SqlCondsSet condsMain = new SqlCondsSet();
            condsMain.Add(GetCrossOrderItemsGrpInfoViewCondsSet(qConds));

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.Entity.GetPagingBy(flipper, sqlOrder.UseDefault ? base.DefaultSqlOrder : sqlOrder, condsMain, inquireColumns));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetCrossOrderItemsGrpInfoViewCount
        /// <summary> 
        /// 依指定的參數取得資料總數。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetCrossOrderItemsGrpInfoViewCount(CrossOrderItemsGrpInfoViewConds qConds)
        {
            Returner returner = new Returner();

            SqlCondsSet condsMain = new SqlCondsSet();
            condsMain.Add(GetCrossOrderItemsGrpInfoViewCondsSet(qConds));

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.Entity.GetCountBy(condsMain));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetCrossOrderItemsGrpInfoViewByCompoundSearch
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
        public Returner GetCrossOrderItemsGrpInfoViewByCompoundSearch(CrossOrderItemsGrpInfoViewConds qConds, string[] columnsName, string keyword, bool isAbsolute, PagingFlipper flipper, SqlOrder sqlOrder, params string[] inquireColumns)
        {
            Returner returner = new Returner();

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.GetInfoByCompoundSearch(columnsName, keyword, EntityHelper.KEYWORD_SEPARATOR, isAbsolute, flipper, sqlOrder, GetCrossOrderItemsGrpInfoViewCondsSet(qConds), inquireColumns));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetCrossOrderItemsGrpInfoViewByCompoundSearchCount
        /// <summary> 
        /// 依指定的參數取得資料總數（複合式查詢）。 
        /// 若欄位名稱陣列長度等於0，則會略過指定欄位名稱查詢動作。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <param name="columnsName">欄位名稱陣列。</param> 
        /// <param name="keyword">關鍵字。</param> 
        /// <param name="isAbsolute">是否為絕對查詢。若 false 則使用 like 查詢。</param> 
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetCrossOrderItemsGrpInfoViewByCompoundSearchCount(CrossOrderItemsGrpInfoViewConds qConds, string[] columnsName, string keyword, bool isAbsolute)
        {
            Returner returner = new Returner();

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.GetInfoByCompoundSearchCount(columnsName, keyword, EntityHelper.KEYWORD_SEPARATOR, isAbsolute, GetCrossOrderItemsGrpInfoViewCondsSet(qConds)));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetCrossOrderItemsGrpInfoViewCondsSet
        /// <summary> 
        /// 依指定的條件取得 SqlCondsSet。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <returns>EzCoding.Collections.SqlCondsSet。</returns> 
        SqlCondsSet GetCrossOrderItemsGrpInfoViewCondsSet(CrossOrderItemsGrpInfoViewConds qConds)
        {
            #region 自訂表格
            var custEntity = new SqlSyntaxBuilder(base.Entity, new SqlSyntax());

            #region 條件
            var sqlConds = new List<string>();

            if (qConds.ExtQuotnDetSIds != null && qConds.ExtQuotnDetSIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds("ODR_DET", "SID", SqlOperator.EqualTo, SystemId.ToString(qConds.ExtQuotnDetSIds), GenericDBType.Char));
            }

            if (qConds.CustomerId.HasValue)
            {
                sqlConds.Add(custEntity.BuildConds("ODR", "CUSTOMER_ID", SqlOperator.EqualTo, qConds.CustomerId.Value, GenericDBType.BigInt));
            }

            if (qConds.ExtOrderSIds != null && qConds.ExtOrderSIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds("ODR_DET", "EXT_ORDER_SID", SqlOperator.EqualTo, SystemId.ToString(qConds.ExtOrderSIds), GenericDBType.Char));
            }

            if (qConds.PartNos != null && qConds.PartNos.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds("ODR_DET", "PART_NO", SqlOperator.EqualTo, qConds.PartNos, GenericDBType.VarChar));
            }

            if (!string.IsNullOrWhiteSpace(qConds.ExportItemType))
            {
                sqlConds.Add(custEntity.BuildConds("ITEM_DET", "EXPORT_ITEM_TYPE", SqlOperator.EqualTo, qConds.ExportItemType, GenericDBType.NVarChar));
            }

            #region 分類值改成允許空字串
            string EMPTY_ITEM_VALUE = "[---]";

            if (!string.IsNullOrWhiteSpace(qConds.Category1))
            {
                if (qConds.Category1 == EMPTY_ITEM_VALUE)
                {
                    sqlConds.Add(custEntity.BuildConds("ITEM_DET", "CATEGORY1", SqlOperator.IsNull, string.Empty, GenericDBType.NVarChar));
                }
                else
                {
                    sqlConds.Add(custEntity.BuildConds("ITEM_DET", "CATEGORY1", SqlOperator.EqualTo, qConds.Category1, GenericDBType.NVarChar));
                }
            }

            if (!string.IsNullOrWhiteSpace(qConds.Category2))
            {
                if (qConds.Category2 == EMPTY_ITEM_VALUE)
                {
                    sqlConds.Add(custEntity.BuildConds("ITEM_DET", "CATEGORY2", SqlOperator.IsNull, string.Empty, GenericDBType.NVarChar));
                }
                else
                {
                    sqlConds.Add(custEntity.BuildConds("ITEM_DET", "CATEGORY2", SqlOperator.EqualTo, qConds.Category2, GenericDBType.NVarChar));
                }
            }

            if (!string.IsNullOrWhiteSpace(qConds.Category3))
            {
                if (qConds.Category3 == EMPTY_ITEM_VALUE)
                {
                    sqlConds.Add(custEntity.BuildConds("ITEM_DET", "CATEGORY3", SqlOperator.IsNull, string.Empty, GenericDBType.NVarChar));
                }
                else
                {
                    sqlConds.Add(custEntity.BuildConds("ITEM_DET", "CATEGORY3", SqlOperator.EqualTo, qConds.Category3, GenericDBType.NVarChar));
                }
            }

            if (!string.IsNullOrWhiteSpace(qConds.Category4))
            {
                if (qConds.Category4 == EMPTY_ITEM_VALUE)
                {
                    sqlConds.Add(custEntity.BuildConds("ITEM_DET", "CATEGORY4", SqlOperator.IsNull, string.Empty, GenericDBType.NVarChar));
                }
                else
                {
                    sqlConds.Add(custEntity.BuildConds("ITEM_DET", "CATEGORY4", SqlOperator.EqualTo, qConds.Category4, GenericDBType.NVarChar));
                }
            }

            if (!string.IsNullOrWhiteSpace(qConds.Category5))
            {
                if (qConds.Category5 == EMPTY_ITEM_VALUE)
                {
                    sqlConds.Add(custEntity.BuildConds("ITEM_DET", "CATEGORY5", SqlOperator.IsNull, string.Empty, GenericDBType.NVarChar));
                }
                else
                {
                    sqlConds.Add(custEntity.BuildConds("ITEM_DET", "CATEGORY5", SqlOperator.EqualTo, qConds.Category5, GenericDBType.NVarChar));
                }
            }
            #endregion

            sqlConds.Add(Handier.ToSysStatusSqlSyntax(IncludeScope.OnlyNotMarkDeleted, "ODR"));
            //sqlConds.Add(Handier.ToSysStatusSqlSyntax(IncludeScope.OnlyNotMarkDeleted, "ODR_DET"));
            #endregion

            custEntity.AppendFormat(@"
                SELECT [P_ODR_DET].* 
                       --, [P_QUOTN].[ODR_NO], [P_QUOTN].[IS_CANCEL] 
                       , [P_INV].[INVENTORY_ITEM_ID], [P_INV].[DESCRIPTION] AS [SUMMARY], [P_INV].[UNIT_WEIGHT], [P_INV].[WEIGHT_UOM_CODE], [P_INV].[ORGANIZATION_CODE] 
                       , [ITEM_DET].[EXPORT_ITEM_TYPE], [ITEM_DET].[CATEGORY1], [ITEM_DET].[CATEGORY2], [ITEM_DET].[CATEGORY3], [ITEM_DET].[CATEGORY4], [ITEM_DET].[CATEGORY5] 

                FROM [EXT_ORDER_DET] [P_ODR_DET] 
                     INNER JOIN [EXT_ORDER] [P_ODR] ON [P_ODR].[SID] = [P_ODR_DET].[EXT_ORDER_SID] 
                     --INNER JOIN [EXT_QUOTN] [P_QUOTN] ON [P_QUOTN].[SID] = [P_ODR].[EXT_QUOTN_SID] 
                     INNER JOIN [ERP_INV] [P_INV] ON [P_INV].[ITEM] = [P_ODR_DET].[PART_NO] 
                     INNER JOIN [EXT_ITEM_DETAILS] [ITEM_DET] ON [ITEM_DET].[ERP_ITEM] = [P_ODR_DET].[PART_NO] 
                WHERE EXISTS
                (

                SELECT [PART_NO]
                FROM
                (
                    SELECT [ODR_DET].[SID], [ODR_DET].[PART_NO] 
                           --, [QUOTN].[ODR_NO], [QUOTN].[IS_CANCEL] 
                           --, [ITEM_DET].[EXPORT_ITEM_TYPE], [ITEM_DET].[CATEGORY1], [ITEM_DET].[CATEGORY2], [ITEM_DET].[CATEGORY3], [ITEM_DET].[CATEGORY4], [ITEM_DET].[CATEGORY5] 

                           --取外銷出貨單的使用量 (不取已註刪的資料)
                           , (SELECT COALESCE(SUM([EXT_SHIPPING_ORDER_DET].[QTY]), 0) AS [CNT] FROM [EXT_SHIPPING_ORDER_DET] INNER JOIN [EXT_SHIPPING_ORDER] [SHIPPING] ON [SHIPPING].[SID] = [EXT_SHIPPING_ORDER_DET].[EXT_SHIPPING_ORDER_SID] AND [SHIPPING].[MDELED] = 'N' WHERE [EXT_SHIPPING_ORDER_DET].[EXT_ORDER_DET_SID] = [ODR_DET].[SID]) AS [SHIP_ODR_USE_QTY] 

                    FROM [EXT_ORDER_DET] [ODR_DET] 
                         INNER JOIN [EXT_ORDER] [ODR] ON [ODR].[SID] = [ODR_DET].[EXT_ORDER_SID] 
                         INNER JOIN [EXT_QUOTN] [QUOTN] ON [QUOTN].[SID] = [ODR].[EXT_QUOTN_SID] 
                         INNER JOIN [EXT_ITEM_DETAILS] [ITEM_DET] ON [ITEM_DET].[ERP_ITEM] = [ODR_DET].[PART_NO] 
                    --只取待出貨的訂單
                    WHERE [QUOTN].[IS_CANCEL] = 'N' AND [QUOTN].[IS_READJUST] = 'N' AND [ODR].[ACTIVE_FLAG] = 'Y' AND [ODR].[STATUS] IN (2,4) AND [ITEM_DET].[ACTIVE_FLAG] = 'Y' {0}
                ) [T]
                WHERE [PART_NO] = [P_ODR_DET].[PART_NO] AND [QTY] > [SHIP_ODR_USE_QTY]
                GROUP BY [PART_NO]
                HAVING [P_ODR_DET].[SID] = MAX([SID])
                )
            ", SqlUtilBox.ToSqlConds(sqlConds.Where(q => !string.IsNullOrWhiteSpace(q)).ToArray(), " AND ", string.Empty));

            base.Entity.CustomEntitySqlSyntax = custEntity.GetSqlSyntax();
            #endregion

            SqlCondsSet condsMain = new SqlCondsSet();

            return condsMain;
        }
        #endregion
        #endregion

        #region 檢視查詢 (for 「依品項建立」出貨單選擇品項用 - 序號清單)
        #region 表格資料
        /// <summary>
        /// 表格資料。
        /// </summary>
        public class CrossOrderItemsInfoView : Info
        {
            /// <summary>
            /// 報價單編號。
            /// </summary>
            [SchemaMapping(Name = "ODR_NO", Type = ReturnType.String)]
            public string OdrNo { get; set; }
            /// <summary>
            /// 料號 ID。
            /// </summary>
            [SchemaMapping(Name = "INVENTORY_ITEM_ID", Type = ReturnType.Long, AllowNull = true)]
            public long? InventoryItemId { get; set; }
            /// <summary>
            /// 料號摘要。
            /// </summary>
            [SchemaMapping(Name = "SUMMARY", Type = ReturnType.String)]
            public string Summary { get; set; }
            /// <summary>
            /// 料號重量。
            /// </summary>
            [SchemaMapping(Name = "UNIT_WEIGHT", Type = ReturnType.Float, AllowNull = true)]
            public float? UnitWeight { get; set; }
            /// <summary>
            /// 重量單位。
            /// </summary>
            [SchemaMapping(Name = "WEIGHT_UOM_CODE", Type = ReturnType.String)]
            public string WeightUomCode { get; set; }
            /// <summary>
            /// 料號所屬製造組織代碼。
            /// </summary>
            [SchemaMapping(Name = "ORGANIZATION_CODE", Type = ReturnType.String)]
            public string OrganizationCode { get; set; }
            /// <summary>
            /// 外銷出貨單使用量。
            /// </summary>
            [SchemaMapping(Name = "SHIP_ODR_USE_QTY", Type = ReturnType.Int)]
            public int ShipOdrUseQty { get; set; }

            #region 繫結資料
            /// <summary>
            /// 繫結資料。
            /// </summary>
            new public static CrossOrderItemsInfoView Binding(DataRow row)
            {
                return DBUtilBox.BindingRow<CrossOrderItemsInfoView>(new CrossOrderItemsInfoView(), row);
            }

            /// <summary>
            /// 繫結資料。
            /// </summary>
            new public static CrossOrderItemsInfoView[] Binding(DataTable dTable)
            {
                List<CrossOrderItemsInfoView> infos = new List<CrossOrderItemsInfoView>();
                foreach (DataRow row in dTable.Rows)
                {
                    infos.Add(CrossOrderItemsInfoView.Binding(row));
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
        public class CrossOrderItemsInfoViewConds
        {
            ///// <summary>
            ///// 初始化 CrossOrderItemsInfoViewConds 類別的新執行個體。
            ///// 預設略過所有條件。
            ///// </summary>
            //public CrossOrderItemsInfoViewConds()
            //{

            //}

            /// <summary>
            /// 初始化 CrossOrderItemsInfoViewConds 類別的新執行個體。
            /// </summary>
            /// <param name="extOrderDetSIds">外銷訂單明細系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="customerId">客戶 ID（若為 null 則略過條件檢查）。</param>
            /// <param name="extOrderSIds">外銷訂單系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="partNos">料號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            public CrossOrderItemsInfoViewConds(ISystemId[] extOrderDetSIds, long? customerId, ISystemId[] extOrderSIds, string[] partNos)
            {
                this.ExtQuotnDetSIds = extOrderDetSIds;
                this.CustomerId = customerId;
                this.ExtOrderSIds = extOrderSIds;
                this.PartNos = partNos;
            }

            /// <summary>
            /// 外銷訂單明細系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public ISystemId[] ExtQuotnDetSIds { get; set; }
            /// <summary>
            /// 客戶 ID（若為 null 則略過條件檢查）。
            /// </summary>
            public long? CustomerId { get; set; }
            /// <summary>
            /// 外銷訂單系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public ISystemId[] ExtOrderSIds { get; set; }
            /// <summary>
            /// 料號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public string[] PartNos { get; set; }
        }
        #endregion

        #region GetCrossOrderItemsInfoView
        /// <summary> 
        /// 依指定的參數取得資料。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <param name="size">指定的前 N 筆資料（若為 Int32.MaxValue 則表示全部）。</param>
        /// <param name="sqlOrder">SQL 排序（Order By）的組合類別（若為 null 則使用預設排序）。</param>
        /// <param name="inquireColumns">含有零或多個指定查詢時所要使用的欄位名稱 System.String 陣列。若陣列為零，則使用預設開放的欄位名稱。</param>
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetCrossOrderItemsInfoView(CrossOrderItemsInfoViewConds qConds, int size, SqlOrder sqlOrder, params string[] inquireColumns)
        {
            Returner returner = new Returner();

            SqlCondsSet condsMain = new SqlCondsSet();
            condsMain.Add(GetCrossOrderItemsInfoViewCondsSet(qConds));

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
        public Returner GetCrossOrderItemsInfoView(CrossOrderItemsInfoViewConds qConds, PagingFlipper flipper, SqlOrder sqlOrder, params string[] inquireColumns)
        {
            Returner returner = new Returner();

            SqlCondsSet condsMain = new SqlCondsSet();
            condsMain.Add(GetCrossOrderItemsInfoViewCondsSet(qConds));

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.Entity.GetPagingBy(flipper, sqlOrder.UseDefault ? base.DefaultSqlOrder : sqlOrder, condsMain, inquireColumns));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetCrossOrderItemsInfoViewCount
        /// <summary> 
        /// 依指定的參數取得資料總數。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetCrossOrderItemsInfoViewCount(CrossOrderItemsInfoViewConds qConds)
        {
            Returner returner = new Returner();

            SqlCondsSet condsMain = new SqlCondsSet();
            condsMain.Add(GetCrossOrderItemsInfoViewCondsSet(qConds));

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.Entity.GetCountBy(condsMain));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetCrossOrderItemsInfoViewByCompoundSearch
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
        public Returner GetCrossOrderItemsInfoViewByCompoundSearch(CrossOrderItemsInfoViewConds qConds, string[] columnsName, string keyword, bool isAbsolute, PagingFlipper flipper, SqlOrder sqlOrder, params string[] inquireColumns)
        {
            Returner returner = new Returner();

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.GetInfoByCompoundSearch(columnsName, keyword, EntityHelper.KEYWORD_SEPARATOR, isAbsolute, flipper, sqlOrder, GetCrossOrderItemsInfoViewCondsSet(qConds), inquireColumns));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetCrossOrderItemsInfoViewByCompoundSearchCount
        /// <summary> 
        /// 依指定的參數取得資料總數（複合式查詢）。 
        /// 若欄位名稱陣列長度等於0，則會略過指定欄位名稱查詢動作。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <param name="columnsName">欄位名稱陣列。</param> 
        /// <param name="keyword">關鍵字。</param> 
        /// <param name="isAbsolute">是否為絕對查詢。若 false 則使用 like 查詢。</param> 
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetCrossOrderItemsInfoViewByCompoundSearchCount(CrossOrderItemsInfoViewConds qConds, string[] columnsName, string keyword, bool isAbsolute)
        {
            Returner returner = new Returner();

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.GetInfoByCompoundSearchCount(columnsName, keyword, EntityHelper.KEYWORD_SEPARATOR, isAbsolute, GetCrossOrderItemsInfoViewCondsSet(qConds)));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetCrossOrderItemsInfoViewCondsSet
        /// <summary> 
        /// 依指定的條件取得 SqlCondsSet。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <returns>EzCoding.Collections.SqlCondsSet。</returns> 
        SqlCondsSet GetCrossOrderItemsInfoViewCondsSet(CrossOrderItemsInfoViewConds qConds)
        {
            #region 自訂表格
            var custEntity = new SqlSyntaxBuilder(base.Entity, new SqlSyntax());

            #region 條件
            var sqlConds = new List<string>();

            if (qConds.ExtQuotnDetSIds != null && qConds.ExtQuotnDetSIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds("ODR_DET", "SID", SqlOperator.EqualTo, SystemId.ToString(qConds.ExtQuotnDetSIds), GenericDBType.Char));
            }

            if (qConds.CustomerId.HasValue)
            {
                sqlConds.Add(custEntity.BuildConds("ODR", "CUSTOMER_ID", SqlOperator.EqualTo, qConds.CustomerId.Value, GenericDBType.BigInt));
            }

            if (qConds.ExtOrderSIds != null && qConds.ExtOrderSIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds("ODR_DET", "EXT_ORDER_SID", SqlOperator.EqualTo, SystemId.ToString(qConds.ExtOrderSIds), GenericDBType.Char));
            }

            if (qConds.PartNos != null && qConds.PartNos.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds("ODR_DET", "PART_NO", SqlOperator.EqualTo, qConds.PartNos, GenericDBType.VarChar));
            }

            sqlConds.Add(Handier.ToSysStatusSqlSyntax(IncludeScope.OnlyNotMarkDeleted, "ODR"));
            //sqlConds.Add(Handier.ToSysStatusSqlSyntax(IncludeScope.OnlyNotMarkDeleted, "ODR_DET"));
            #endregion

            custEntity.AppendFormat(@"
                SELECT *
                FROM
                (
                    SELECT [ODR_DET].* 
                           , [QUOTN].[ODR_NO] 
                           , [INV].[INVENTORY_ITEM_ID], [INV].[DESCRIPTION] AS [SUMMARY], [INV].[UNIT_WEIGHT], [INV].[WEIGHT_UOM_CODE], [INV].[ORGANIZATION_CODE] 

                           --取外銷出貨單的使用量 (不取已註刪的資料)
                           , (SELECT COALESCE(SUM([EXT_SHIPPING_ORDER_DET].[QTY]), 0) AS [CNT] FROM [EXT_SHIPPING_ORDER_DET] INNER JOIN [EXT_SHIPPING_ORDER] [SHIPPING] ON [SHIPPING].[SID] = [EXT_SHIPPING_ORDER_DET].[EXT_SHIPPING_ORDER_SID] AND [SHIPPING].[MDELED] = 'N' WHERE [EXT_SHIPPING_ORDER_DET].[EXT_ORDER_DET_SID] = [ODR_DET].[SID]) AS [SHIP_ODR_USE_QTY] 

                    FROM [EXT_ORDER_DET] [ODR_DET] 
                         INNER JOIN [EXT_ORDER] [ODR] ON [ODR].[SID] = [ODR_DET].[EXT_ORDER_SID] 
                         INNER JOIN [EXT_QUOTN] [QUOTN] ON [QUOTN].[SID] = [ODR].[EXT_QUOTN_SID] 
                         INNER JOIN [ERP_INV] [INV] ON [INV].[ITEM] = [ODR_DET].[PART_NO] 
                    --只取待出貨的訂單
                    WHERE [QUOTN].[IS_CANCEL] = 'N' AND [QUOTN].[IS_READJUST] = 'N' AND [ODR].[ACTIVE_FLAG] = 'Y' AND [ODR].[STATUS] IN (2,4) {0}
                ) [T]
                WHERE [QTY] > [SHIP_ODR_USE_QTY]
            ", SqlUtilBox.ToSqlConds(sqlConds.Where(q => !string.IsNullOrWhiteSpace(q)).ToArray(), " AND ", string.Empty));

            base.Entity.CustomEntitySqlSyntax = custEntity.GetSqlSyntax();
            #endregion

            SqlCondsSet condsMain = new SqlCondsSet();

            return condsMain;
        }
        #endregion
        #endregion

        #region 建議生產量查詢
        #region 表格資料
        /// <summary>
        /// 表格資料。
        /// </summary>
        public class RecProdInfoView
        {
            /// <summary>
            /// 系統代號。
            /// </summary>
            [SchemaMapping(Name = "SID", Type = ReturnType.SId)]
            public ISystemId SId { get; set; }
            /// <summary>
            /// 外銷訂單系統代號。
            /// </summary>
            [SchemaMapping(Name = "EXT_ORDER_SID", Type = ReturnType.SId)]
            public ISystemId ExtOrderSId { get; set; }
            /// <summary>
            /// 型號。
            /// </summary>
            [SchemaMapping(Name = "MODEL", Type = ReturnType.String)]
            public string Model { get; set; }
            /// <summary>
            /// 料號。
            /// </summary>
            [SchemaMapping(Name = "PART_NO", Type = ReturnType.String)]
            public string PartNo { get; set; }
            /// <summary>
            /// 數量。
            /// </summary>
            [SchemaMapping(Name = "QTY", Type = ReturnType.Int)]
            public int Qty { get; set; }
            /// <summary>
            /// 目前外銷訂單未出貨的同品項數量。
            /// </summary>
            [SchemaMapping(Name = "CUR_NOT_SHIP_QTY", Type = ReturnType.Int)]
            public int CurNotShipQty { get; set; }
            /// <summary>
            /// 所有外銷訂單未出貨的同品項數量。
            /// </summary>
            [SchemaMapping(Name = "ALL_NOT_SHIP_QTY", Type = ReturnType.Int)]
            public int AllNotShipQty { get; set; }
            /// <summary>
            /// 同一個訂單的前一個有產生生產單的版號的同品項數量。
            /// </summary>
            [SchemaMapping(Name = "PREV_PROD_QTY", Type = ReturnType.Int)]
            public int PrevProdQty { get; set; }

            #region 繫結資料
            /// <summary>
            /// 繫結資料。
            /// </summary>
            public static RecProdInfoView Binding(DataRow row)
            {
                return DBUtilBox.BindingRow<RecProdInfoView>(new RecProdInfoView(), row);
            }

            /// <summary>
            /// 繫結資料。
            /// </summary>
            public static RecProdInfoView[] Binding(DataTable dTable)
            {
                List<RecProdInfoView> infos = new List<RecProdInfoView>();
                foreach (DataRow row in dTable.Rows)
                {
                    infos.Add(RecProdInfoView.Binding(row));
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
        public class RecProdInfoViewConds
        {
            ///// <summary>
            ///// 初始化 RecProdInfoViewConds 類別的新執行個體。
            ///// 預設略過所有條件。
            ///// </summary>
            //public RecProdInfoViewConds()
            //{

            //}

            /// <summary>
            /// 初始化 RecProdInfoViewConds 類別的新執行個體。
            /// </summary>
            /// <param name="extOrderDetSIds">外銷訂單明細系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="extOrderSId">外銷訂單系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            public RecProdInfoViewConds(ISystemId[] extOrderDetSIds, ISystemId extOrderSId)
            {
                this.ExtQuotnDetSIds = extOrderDetSIds;
                this.ExtOrderSId = extOrderSId;
            }

            /// <summary>
            /// 外銷訂單明細系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public ISystemId[] ExtQuotnDetSIds { get; set; }
            /// <summary>
            /// 外銷訂單系統代號（若為 null 則略過條件檢查）。
            /// </summary>
            public ISystemId ExtOrderSId { get; set; }
        }
        #endregion

        #region GetRecProdInfoView
        /// <summary> 
        /// 依指定的參數取得資料。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetRecProdRecProdInfoView(RecProdInfoViewConds qConds)
        {
            #region 自訂表格
            var custEntity = new SqlSyntaxBuilder(base.Entity, new SqlSyntax());

            //外銷訂單未出貨的有從外銷出貨單明細 JOIN 回到外銷訂單, 判斷「正式訂單、正式訂單-未排程、正式訂單-已排程)」三種狀態.
            //            custEntity.Append(@"
            //                SELECT [ODR_DET].[SID], [ODR_DET].[EXT_ORDER_SID], [ODR_DET].[MODEL], [ODR_DET].[PART_NO], [ODR_DET].[QTY]
            //                       -- 所有外銷訂單未出貨的同品項數量 (不取已註刪的資料)
            //                       , COALESCE((SELECT SUM([SHIP_DET].[QTY]) FROM [EXT_SHIPPING_ORDER_DET] [SHIP_DET] INNER JOIN [EXT_SHIPPING_ORDER] [SHIP] ON [SHIP].[SID] = [SHIP_DET].[EXT_SHIPPING_ORDER_SID] INNER JOIN [EXT_ORDER_DET] [ODR_DET_S] ON [ODR_DET_S].[SID] = [SHIP_DET].[EXT_ORDER_DET_SID] INNER JOIN [EXT_ORDER] [ODR_S] ON [ODR_S].[SID] = [ODR_DET_S].[EXT_ORDER_SID] WHERE [SHIP].[MDELED] = 'N' AND [SHIP].[STATUS] IN (1) AND [SHIP_DET].[EXT_ORDER_DET_SID] = [ODR_DET].[SID] AND [ODR_S].[ACTIVE_FLAG] = 'Y' AND [ODR_S].[STATUS] IN (2,3,4)),0) AS [NOT_SHIP_QTY]
            //	                   -- 同一個訂單之前所有版本有產生生產單的同品項數量 (已確認的生產單)
            //	                   , COALESCE((SELECT SUM([PROD_DET].[QTY]) FROM [EXT_PROD_ORDER_DET] [PROD_DET] INNER JOIN [EXT_PROD_ORDER] [PROD] ON [PROD].[SID] = [PROD_DET].[EXT_PROD_ORDER_SID] AND [PROD].[STATUS] = 2 INNER JOIN [EXT_ORDER] [ODR_P] ON [ODR_P].[EXT_QUOTN_SID] = [ODR].[EXT_QUOTN_SID] WHERE [ODR_P].[ACTIVE_FLAG] = 'N' AND [PROD_DET].[PART_NO] = [ODR_DET].[PART_NO]), 0) AS [PREV_PROD_QTY]
            //                FROM [EXT_ORDER_DET] [ODR_DET]
            //                     INNER JOIN [EXT_ORDER] [ODR] ON [ODR].[SID] = [ODR_DET].[EXT_ORDER_SID]
            //            ");

            //外銷訂單未出貨的沒有從外銷出貨單明細 JOIN 回到外銷訂單, 因為有到出貨的, 就不會是外銷訂單「待轉訂單」.
            custEntity.Append(@"
                SELECT [ODR_DET].[SID], [ODR_DET].[EXT_ORDER_SID], [ODR_DET].[MODEL], [ODR_DET].[PART_NO], [ODR_DET].[QTY]
                       -- 目前外銷訂單未出貨的同品項數量 (不取已註刪的資料)
                       , COALESCE((SELECT SUM([SHIP_DET].[QTY]) FROM [EXT_SHIPPING_ORDER_DET] [SHIP_DET] INNER JOIN [EXT_SHIPPING_ORDER] [SHIP] ON [SHIP].[SID] = [SHIP_DET].[EXT_SHIPPING_ORDER_SID] WHERE [SHIP].[MDELED] = 'N' AND [SHIP].[STATUS] IN (1) AND [SHIP_DET].[EXT_ORDER_DET_SID] = [ODR_DET].[SID]),0) AS [CUR_NOT_SHIP_QTY]
                       -- 所有外銷訂單未出貨的同品項數量 (不取已註刪的資料)
                       , COALESCE((SELECT SUM([SHIP_DET].[QTY]) FROM [EXT_SHIPPING_ORDER_DET] [SHIP_DET] INNER JOIN [EXT_SHIPPING_ORDER] [SHIP] ON [SHIP].[SID] = [SHIP_DET].[EXT_SHIPPING_ORDER_SID] WHERE [SHIP].[MDELED] = 'N' AND [SHIP].[STATUS] IN (1) AND [SHIP_DET].[PART_NO] = [ODR_DET].[PART_NO]),0) AS [ALL_NOT_SHIP_QTY]
	                   -- 同一個訂單之前所有版本有產生生產單的同品項數量 (已確認的生產單)
	                   , COALESCE((SELECT SUM([PROD_DET].[QTY]) FROM [EXT_PROD_ORDER_DET] [PROD_DET] INNER JOIN [EXT_PROD_ORDER] [PROD] ON [PROD].[SID] = [PROD_DET].[EXT_PROD_ORDER_SID] AND [PROD].[STATUS] = 2 INNER JOIN [EXT_ORDER] [ODR_P] ON [ODR_P].[EXT_QUOTN_SID] = [ODR].[EXT_QUOTN_SID] WHERE [ODR_P].[ACTIVE_FLAG] = 'N' AND [PROD_DET].[PART_NO] = [ODR_DET].[PART_NO]), 0) AS [PREV_PROD_QTY]
                FROM [EXT_ORDER_DET] [ODR_DET]
                     INNER JOIN [EXT_ORDER] [ODR] ON [ODR].[SID] = [ODR_DET].[EXT_ORDER_SID]
            ");

            var sqlConds = new List<string>();

            if (qConds.ExtQuotnDetSIds != null && qConds.ExtQuotnDetSIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds("ODR_DET", "SID", SqlOperator.EqualTo, SystemId.ToString(qConds.ExtQuotnDetSIds), GenericDBType.Char));
            }

            if (qConds.ExtOrderSId != null)
            {
                sqlConds.Add(custEntity.BuildConds("ODR", "SID", SqlOperator.EqualTo, qConds.ExtOrderSId.ToString(), GenericDBType.Char));
            }

            custEntity.Append(SqlUtilBox.ToSqlConds(sqlConds.Where(q => !string.IsNullOrWhiteSpace(q)).ToArray(), " WHERE ", string.Empty));
            base.Entity.CustomEntitySqlSyntax = custEntity.GetSqlSyntax();
            #endregion

            Returner returner = new Returner();

            SqlCondsSet condsMain = new SqlCondsSet();

            try
            {
                base.Entity.EnableCustomEntity = true;
                return base.Entity.GetInfoBy(Int32.MaxValue, SqlOrder.Clear, condsMain);
            }
            finally
            {
                base.Entity.EnableCustomEntity = false;
            }
        }
        #endregion
        #endregion

        #region 最後一次產生生產單的外銷訂單品項查詢
        #region 查詢條件
        /// <summary>
        /// 查詢條件。
        /// </summary>
        public class ItemOfLastProdInfoViewConds
        {
            ///// <summary>
            ///// 初始化 ItemOfLastProdInfoViewConds 類別的新執行個體。
            ///// 預設略過所有條件。
            ///// </summary>
            //public ItemOfLastProdInfoViewConds()
            //{

            //}

            /// <summary>
            /// 初始化 ItemOfLastProdInfoViewConds 類別的新執行個體。
            /// </summary>
            /// <param name="extQuotnSId">外銷報價單系統代號。</param>
            public ItemOfLastProdInfoViewConds(ISystemId extQuotnSId)
            {
                this.ExtQuotnSId = extQuotnSId;
            }

            /// <summary>
            /// 外銷報價單系統代號。
            /// </summary>
            public ISystemId ExtQuotnSId { get; set; }
        }
        #endregion

        #region GetItemOfLastProdInfoView
        /// <summary> 
        /// 依指定的參數取得資料。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetItemOfLastProdInfoView(ItemOfLastProdInfoViewConds qConds)
        {
            #region 自訂表格
            var custEntity = new SqlSyntaxBuilder(base.Entity, new SqlSyntax());

            custEntity.AppendFormat(@"
                SELECT [ODR_DET].*
                       , [ODR].[EXT_QUOTN_SID], [ODR].[VERSION]
                FROM [EXT_ORDER_DET] [ODR_DET]
                     INNER JOIN [EXT_ORDER] [ODR] ON [ODR].[SID] = [ODR_DET].[EXT_ORDER_SID]
                     INNER JOIN [EXT_PROD_ORDER] [PROD] ON [ODR].[SID] = [PROD].[EXT_ORDER_SID] AND [PROD].[STATUS] = 2
                WHERE 
                    [ODR].[EXT_QUOTN_SID] = '{0}' AND 
                    EXISTS
                    (
                    SELECT [EXT_ORDER].[EXT_QUOTN_SID]
                    FROM [EXT_ORDER]
                         INNER JOIN [EXT_PROD_ORDER] ON [EXT_ORDER].[SID] = [EXT_PROD_ORDER].[EXT_ORDER_SID] AND [EXT_PROD_ORDER].[STATUS] = 2
                    WHERE [EXT_ORDER].[EXT_QUOTN_SID] = [ODR].[EXT_QUOTN_SID]
                    GROUP BY [EXT_ORDER].[EXT_QUOTN_SID]
                    HAVING [ODR].[EXT_QUOTN_SID] = [EXT_ORDER].[EXT_QUOTN_SID] AND [ODR].[VERSION] = MAX([EXT_ORDER].[VERSION])
                    )
            ", qConds.ExtQuotnSId);

            var sqlConds = new List<string>();

            custEntity.Append(SqlUtilBox.ToSqlConds(sqlConds.Where(q => !string.IsNullOrWhiteSpace(q)).ToArray(), " AND ", string.Empty));
            base.Entity.CustomEntitySqlSyntax = custEntity.GetSqlSyntax();
            #endregion

            Returner returner = new Returner();

            SqlCondsSet condsMain = new SqlCondsSet();

            try
            {
                base.Entity.EnableCustomEntity = true;
                return base.Entity.GetInfoBy(Int32.MaxValue, SqlOrder.Clear, condsMain);
            }
            finally
            {
                base.Entity.EnableCustomEntity = false;
            }
        }
        #endregion
        #endregion

        #region 未產生過生產單且未出貨的品項查詢 (未出貨的出貨單包含「草稿、未出貨」的出貨單狀態)
        #region 表格資料
        /// <summary>
        /// 表格資料。
        /// </summary>
        public class NotProdAndShippingInfoView
        {
            /// <summary>
            /// 系統代號。
            /// </summary>
            [SchemaMapping(Name = "SID", Type = ReturnType.SId)]
            public ISystemId SId { get; set; }
            /// <summary>
            /// 外銷訂單系統代號。
            /// </summary>
            [SchemaMapping(Name = "EXT_ORDER_SID", Type = ReturnType.SId)]
            public ISystemId ExtOrderSId { get; set; }
            /// <summary>
            /// 型號。
            /// </summary>
            [SchemaMapping(Name = "MODEL", Type = ReturnType.String)]
            public string Model { get; set; }
            /// <summary>
            /// 料號。
            /// </summary>
            [SchemaMapping(Name = "PART_NO", Type = ReturnType.String)]
            public string PartNo { get; set; }
            /// <summary>
            /// 數量。
            /// </summary>
            [SchemaMapping(Name = "QTY", Type = ReturnType.Int)]
            public int Qty { get; set; }
            /// <summary>
            /// 目前外銷訂單未出貨的同品項數量。
            /// </summary>
            [SchemaMapping(Name = "CUR_NOT_SHIP_QTY", Type = ReturnType.Int)]
            public int CurNotShipQty { get; set; }

            #region 繫結資料
            /// <summary>
            /// 繫結資料。
            /// </summary>
            public static NotProdAndShippingInfoView Binding(DataRow row)
            {
                return DBUtilBox.BindingRow<NotProdAndShippingInfoView>(new NotProdAndShippingInfoView(), row);
            }

            /// <summary>
            /// 繫結資料。
            /// </summary>
            public static NotProdAndShippingInfoView[] Binding(DataTable dTable)
            {
                List<NotProdAndShippingInfoView> infos = new List<NotProdAndShippingInfoView>();
                foreach (DataRow row in dTable.Rows)
                {
                    infos.Add(NotProdAndShippingInfoView.Binding(row));
                }

                return infos.ToArray();
            }
            #endregion
        }
        #endregion

        #region GetNotProdAndShippingInfoView
        /// <summary> 
        /// 依指定的參數取得資料。 
        /// </summary> 
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetNotProdAndShippingInfoView()
        {
            #region 自訂表格
            var custEntity = new SqlSyntaxBuilder(base.Entity, new SqlSyntax());

            custEntity.Append(@"
                SELECT [ODR_DET].[SID], [ODR_DET].[EXT_ORDER_SID], [ODR_DET].[MODEL], [ODR_DET].[PART_NO], [ODR_DET].[QTY]
                       , COALESCE([LAST_DET].[QTY],0) AS [LAST_QTY]
                       -- 目前品項數量 - 目前品項已出貨數量(不取已註刪的資料) = 目前品項未出貨數量
                       , ([ODR_DET].[QTY] - COALESCE((SELECT SUM([SHIP_DET].[QTY]) FROM [EXT_SHIPPING_ORDER_DET] [SHIP_DET] INNER JOIN [EXT_SHIPPING_ORDER] [SHIP] ON [SHIP].[SID] = [SHIP_DET].[EXT_SHIPPING_ORDER_SID] WHERE [SHIP].[MDELED] = 'N' AND [SHIP].[STATUS] IN (1,2) AND [SHIP_DET].[EXT_ORDER_DET_SID] = [ODR_DET].[SID]),0)) AS [CUR_NOT_SHIP_QTY]
                FROM [EXT_ORDER_DET] [ODR_DET]
                     INNER JOIN [EXT_ORDER] [ODR] ON [ODR].[SID] = [ODR_DET].[EXT_ORDER_SID]

                     LEFT JOIN 
                     (
                        -- 同報價單系列最後一次產生生產單的外銷訂單品項
                        SELECT [LAST_DET].[PART_NO], [LAST_DET].[QTY]
                                , [LAST_ODR].[EXT_QUOTN_SID], [LAST_ODR].[VERSION]
                        FROM [EXT_ORDER_DET] [LAST_DET]
                                INNER JOIN [EXT_ORDER] [LAST_ODR] ON [LAST_ODR].[SID] = [LAST_DET].[EXT_ORDER_SID]
                                INNER JOIN [EXT_PROD_ORDER] [LAST_PROD] ON [LAST_ODR].[SID] = [LAST_PROD].[EXT_ORDER_SID] AND [LAST_PROD].[STATUS] = 2
                        WHERE 
                            EXISTS
                            (
                            SELECT [EXT_ORDER].[EXT_QUOTN_SID]
                            FROM [EXT_ORDER]
                                    INNER JOIN [EXT_PROD_ORDER] ON [EXT_ORDER].[SID] = [EXT_PROD_ORDER].[EXT_ORDER_SID] AND [EXT_PROD_ORDER].[STATUS] = 2
                            WHERE [EXT_ORDER].[EXT_QUOTN_SID] = [LAST_ODR].[EXT_QUOTN_SID]
                            GROUP BY [EXT_ORDER].[EXT_QUOTN_SID]
                            HAVING [LAST_ODR].[EXT_QUOTN_SID] = [EXT_ORDER].[EXT_QUOTN_SID] AND [LAST_ODR].[VERSION] = MAX([EXT_ORDER].[VERSION])
                            )
                     ) [LAST_DET] ON [LAST_DET].[EXT_QUOTN_SID] = [ODR].[EXT_QUOTN_SID] AND [LAST_DET].[PART_NO] = [ODR_DET].[PART_NO]

                WHERE [ODR].[SID] IN
                (
	                -- 未產生生產單的外銷訂單
	                SELECT [EXT_ORDER].[SID]
	                FROM [EXT_ORDER]
	                WHERE 
	                NOT EXISTS
	                (
	                SELECT *
	                FROM [EXT_PROD_ORDER]
	                WHERE [EXT_PROD_ORDER].[EXT_ORDER_SID] = [EXT_ORDER].[SID]
	                )
                ) 
                AND [ODR].[ACTIVE_FLAG] = 'Y' 
	            -- 排除待轉訂單
                AND [ODR].[STATUS] IN (2,3,4)
            ");

            var sqlConds = new List<string>();

            custEntity.Append(SqlUtilBox.ToSqlConds(sqlConds.Where(q => !string.IsNullOrWhiteSpace(q)).ToArray(), " AND ", string.Empty));
            base.Entity.CustomEntitySqlSyntax = custEntity.GetSqlSyntax();
            #endregion

            Returner returner = new Returner();

            SqlCondsSet condsMain = new SqlCondsSet();
            //目前訂單品項數量大於同報價單系列前一版已產生過生產單的訂單品項數量
            condsMain.Add(new LeftRightPair<string, string>("QTY", "LAST_QTY"), SqlCond.GreaterThan, SqlCondsSet.And);

            try
            {
                base.Entity.EnableCustomEntity = true;
                return base.Entity.GetInfoBy(Int32.MaxValue, SqlOrder.Clear, condsMain);
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
