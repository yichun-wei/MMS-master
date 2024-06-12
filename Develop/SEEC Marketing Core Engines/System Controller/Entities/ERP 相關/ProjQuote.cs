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
        /// 專案報價。
        /// </summary>
        public const string PROJ_QUOTE = "PROJ_QUOTE";
    }

    /// <summary>
    /// 專案報價的類別實作。
    /// </summary>
    public class ProjQuote : SystemBase
    {
        /// <summary>
        /// 初始化 Seec.Marketing.SystemEngines.ProjQuote 類別的新執行個體。
        /// </summary>
        /// <param name="connInfo">資料庫連線資訊。</param>
        public ProjQuote(string connInfo)
            : base(EntityHelper.GetEntityBase(connInfo, DBTableDefine.PROJ_QUOTE))
        {
            SqlOrder sorting = new SqlOrder();
            sorting.Add("QUOTENUMBER", Sort.Ascending);
            sorting.Add("QUOTEITEMID", Sort.Ascending);

            base.DefaultSqlOrder = sorting;
        }

        #region 表格資料
        /// <summary>
        /// 表格資料。
        /// </summary>
        public class Info : InfoBase
        {
            /// <summary>
            /// 報價單 ID。
            /// </summary>
            [SchemaMapping(Name = "QUOTEID", Type = ReturnType.String)]
            public string QuoteId { get; set; }
            /// <summary>
            /// 報價單號碼。
            /// </summary>
            [SchemaMapping(Name = "QUOTENUMBER", Type = ReturnType.String)]
            public string QuoteNumber { get; set; }
            /// <summary>
            /// 報價單日期。
            /// </summary>
            [SchemaMapping(Name = "QUOTEDATE", Type = ReturnType.DateTime)]
            public DateTime QuoteDate { get; set; }
            /// <summary>
            /// 報價單抬頭。
            /// </summary>
            [SchemaMapping(Name = "QUOTETITLE", Type = ReturnType.String)]
            public string QuoteTitle { get; set; }
            /// <summary>
            /// 報價單備註。
            /// </summary>
            [SchemaMapping(Name = "REMARK", Type = ReturnType.String)]
            public string Remark { get; set; }
            /// <summary>
            /// 簽核狀態。
            /// </summary>
            [SchemaMapping(Name = "FLOWSTATUS", Type = ReturnType.String)]
            public string FlowStatus { get; set; }
            /// <summary>
            /// 報價單明細項次。
            /// </summary>
            [SchemaMapping(Name = "QUOTEITEMID", Type = ReturnType.String)]
            public string QuoteItemId { get; set; }
            /// <summary>
            /// 產品料號。
            /// </summary>
            [SchemaMapping(Name = "PRODUCTID", Type = ReturnType.String)]
            public string ProductId { get; set; }
            /// <summary>
            /// 原始單價。
            /// </summary>
            [SchemaMapping(Name = "UNITPRICE", Type = ReturnType.Float)]
            public float UnitPrice { get; set; }
            /// <summary>
            /// 數量。
            /// </summary>
            [SchemaMapping(Name = "QUANTITY", Type = ReturnType.Int)]
            public int Quantity { get; set; }
            /// <summary>
            /// 折數。
            /// </summary>
            [SchemaMapping(Name = "DISCOUNT", Type = ReturnType.Float)]
            public float Discount { get; set; }
            /// <summary>
            /// ERP 客戶 ID。
            /// </summary>
            [SchemaMapping(Name = "DEALER_CUSTOMER_ID", Type = ReturnType.String)]
            public string DealerCustomerId { get; set; }
            /// <summary>
            /// ERP 客戶編號。
            /// </summary>
            [SchemaMapping(Name = "DEALER_ERP_NUM", Type = ReturnType.String)]
            public string DealerErpNum { get; set; }
            /// <summary>
            /// ERP客戶名稱。
            /// </summary>
            [SchemaMapping(Name = "DEALER_ERP_NAME", Type = ReturnType.String)]
            public string DealerErpName { get; set; }
            /// <summary>
            ///是否有效。
            /// </summary>
            [SchemaMapping(Name = "ACTIVE", Type = ReturnType.String)]
            public string Active { get; set; }

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
        /// <param name="quoteId">報價單 ID。</param>
        /// <param name="quoteNumber">報價單號碼。</param>
        /// <param name="quoteDate">報價單日期。</param>
        /// <param name="quoteTitle">報價單抬頭（null 或 empty 將自動略過操作）。</param>
        /// <param name="remark">報價單備註（null 或 empty 將自動略過操作）。</param>
        /// <param name="flowStatus">簽核狀態。</param>
        /// <param name="quoteItemId">報價單明細項次。</param>
        /// <param name="productId">產品料號。</param>
        /// <param name="unitPrice">原始單價。</param>
        /// <param name="quantity">數量。</param>
        /// <param name="discount">折數。</param>
        /// <param name="dealerCustomerId">ERP 客戶 ID（null 或 empty 將自動略過操作）。</param>
        /// <param name="dealerErpNum">ERP 客戶編號（null 或 empty 將自動略過操作）。</param>
        /// <param name="dealerErpName">ERP客戶名稱（null 或 empty 將自動略過操作）。</param>
         /// <param name="active">是否有效（null 或 empty 將自動略過操作）。</param>

        /// <returns>EzCoding.Returner。</returns>
        /// <exception cref="System.ArgumentNullException">參數 quoteNumber 不允許為 null 值。</exception>
        /// <exception cref="System.ArgumentNullException">參數 quoteItemId 不允許為 null 值。</exception>
        public Returner Add(string quoteId, string quoteNumber, DateTime quoteDate, string quoteTitle, string remark, string flowStatus, string quoteItemId, string productId, float unitPrice, int quantity, float discount, string dealerCustomerId, string dealerErpNum, string dealerErpName, string active)
        {
            if (quoteNumber == null) { throw new ArgumentNullException("quoteNumber"); }
            if (quoteItemId == null) { throw new ArgumentNullException("quoteItemId"); }

            Returner returner = new Returner();
            try
            {
                #region 交易緩衝區
                TransColValSet transSet = new TransColValSet();
                transSet.SmartAdd("QUOTEID", quoteId, GenericDBType.NVarChar, false);
                transSet.SmartAdd("QUOTENUMBER", quoteNumber, GenericDBType.NVarChar, false);
                transSet.SmartAdd("QUOTEDATE", quoteDate, "yyyyMMddHHmmss", GenericDBType.Char);
                transSet.SmartAdd("QUOTETITLE", quoteTitle, GenericDBType.NVarChar, true);
                transSet.SmartAdd("REMARK", remark, GenericDBType.NText, true);
                transSet.SmartAdd("FLOWSTATUS", flowStatus, GenericDBType.NVarChar, false);
                transSet.SmartAdd("QUOTEITEMID", quoteItemId, GenericDBType.NVarChar, false);
                transSet.SmartAdd("PRODUCTID", productId, GenericDBType.NVarChar, false);
                transSet.SmartAdd("UNITPRICE", unitPrice, GenericDBType.Float);
                transSet.SmartAdd("QUANTITY", quantity, GenericDBType.Int);
                transSet.SmartAdd("DISCOUNT", discount, GenericDBType.Float);
                transSet.SmartAdd("DEALER_CUSTOMER_ID", dealerCustomerId, GenericDBType.NVarChar, true);
                transSet.SmartAdd("DEALER_ERP_NUM", dealerErpNum, GenericDBType.NVarChar, true);
                transSet.SmartAdd("DEALER_ERP_NAME", dealerErpName, GenericDBType.NVarChar, true);
                transSet.SmartAdd("ACTIVE", active, GenericDBType.NVarChar, true);

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
        /// 依指定的參數，修改一筆 專案報價。
        /// </summary>
        /// <param name="quoteNumber">報價單號碼。</param>
        /// <param name="quoteItemId">報價單明細項次。</param>
        /// <param name="quoteId">報價單 ID。</param>
        /// <param name="quoteDate">報價單日期。</param>
        /// <param name="quoteTitle">報價單抬頭（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="remark">報價單備註（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="flowStatus">簽核狀態。</param>
        /// <param name="productId">產品料號。</param>
        /// <param name="unitPrice">原始單價。</param>
        /// <param name="quantity">數量。</param>
        /// <param name="discount">折數。</param>
        /// <param name="dealerCustomerId">ERP 客戶 ID（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="dealerErpNum">ERP 客戶編號（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="dealerErpName">ERP客戶名稱（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="active">是否有效（null 或 empty 將自動略過操作）。</param>
        /// <returns>EzCoding.Returner。</returns>
        /// <exception cref="System.ArgumentNullException">參數 quoteNumber 不允許為 null 值。</exception>
        /// <exception cref="System.ArgumentNullException">參數 quoteItemId 不允許為 null 值。</exception>
        public Returner Modify(string quoteNumber, string quoteItemId, string quoteId, DateTime quoteDate, string quoteTitle, string remark, string flowStatus, string productId, float unitPrice, int quantity, float discount, string dealerCustomerId, string dealerErpNum, string dealerErpName,string active)
        {
            if (quoteNumber == null) { throw new ArgumentNullException("quoteNumber"); }
            if (quoteItemId == null) { throw new ArgumentNullException("quoteItemId"); }

            Returner returner = new Returner();
            try
            {
                #region 交易緩衝區
                TransColValSet transSet = new TransColValSet();
                transSet.SmartAdd("QUOTEID", quoteId, GenericDBType.NVarChar, false);
                transSet.SmartAdd("QUOTEDATE", quoteDate, "yyyyMMddHHmmss", GenericDBType.Char);
                transSet.SmartAdd("QUOTETITLE", quoteTitle, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("REMARK", remark, GenericDBType.NText, true, true);
                transSet.SmartAdd("FLOWSTATUS", flowStatus, GenericDBType.NVarChar, false);
                transSet.SmartAdd("PRODUCTID", productId, GenericDBType.NVarChar, false);
                transSet.SmartAdd("UNITPRICE", unitPrice, GenericDBType.Float);
                transSet.SmartAdd("QUANTITY", quantity, GenericDBType.Int);
                transSet.SmartAdd("DISCOUNT", discount, GenericDBType.Float);
                transSet.SmartAdd("DEALER_CUSTOMER_ID", dealerCustomerId, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("DEALER_ERP_NUM", dealerErpNum, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("DEALER_ERP_NAME", dealerErpName, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("ACTIVE", active, GenericDBType.NVarChar, true);

                SqlCondsSet condsMain = new SqlCondsSet();
                condsMain.Add("QUOTENUMBER", SqlCond.EqualTo, quoteNumber, GenericDBType.NVarChar, SqlCondsSet.And);
                condsMain.Add("QUOTEITEMID", SqlCond.EqualTo, quoteItemId, GenericDBType.NVarChar, SqlCondsSet.And);

                returner.ChangeInto(base.Modify(transSet, condsMain, true));
                #endregion

                return returner;
            }
            finally
            {
            }
        }
        #endregion

        #region UpdateCancelInfo
        /// <summary>
        /// 更新取消資訊。
        /// </summary>
        /// <param name="quoteNumber">報價單號碼。</param>        
        /// <param name="isCancel">是否已取消。</param>
        /// <param name="cancelDT">取消時間（null 則直接設為 DBNull）。</param>
        public Returner UpdateCancelInfo(string quoteNumber, bool isCancel, DateTime? cancelDT)
        {
            if (quoteNumber == null) { throw new ArgumentNullException("quoteNumber"); }

            Returner returner = new Returner();
            try
            {
                #region 交易緩衝區
                TransColValSet transSet = new TransColValSet();
                transSet.SmartAdd("IS_CANCEL", isCancel ? "Y" : "N", GenericDBType.Char, false);
                transSet.SmartAdd("CANCEL_DT", cancelDT, "yyyyMMddHHmmss", GenericDBType.Char, true);

                SqlCondsSet condsMain = new SqlCondsSet();
                condsMain.Add("QUOTENUMBER", SqlCond.EqualTo, quoteNumber, GenericDBType.NVarChar, SqlCondsSet.And);

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
            /// <param name="quoteNumbers">報價單號碼陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="quoteItemIds">報價單明細項次陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="dealerCustomerId">ERP 客戶 ID（若為 null 或 empty 則略過條件檢查）。</param>
            public InfoConds(string[] quoteNumbers, string[] quoteItemIds, string dealerCustomerId)
            {
                this.QuoteNumbers = quoteNumbers;
                this.QuoteItemIds = quoteItemIds;
                this.DealerCustomerId = dealerCustomerId;
            }

            /// <summary>
            /// 報價單號碼陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public string[] QuoteNumbers { get; set; }
            /// <summary>
            /// 報價單明細項次陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public string[] QuoteItemIds { get; set; }
            /// <summary>
            /// ERP 客戶 ID（若為 null 或 empty 則略過條件檢查）。
            /// </summary>
            public string DealerCustomerId { get; set; }
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

            custEntity.Append("SELECT [PROJ_QUOTE].* ");
            custEntity.Append("FROM [PROJ_QUOTE] ");

            var sqlConds = new List<string>();

            if (qConds.QuoteNumbers != null && qConds.QuoteNumbers.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "QUOTENUMBER", SqlOperator.EqualTo, qConds.QuoteNumbers, GenericDBType.NVarChar));
            }

            if (qConds.QuoteItemIds != null && qConds.QuoteItemIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "QUOTEITEMID", SqlOperator.EqualTo, qConds.QuoteItemIds, GenericDBType.NVarChar));
            }

            if (!string.IsNullOrWhiteSpace(qConds.DealerCustomerId))
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "DEALER_CUSTOMER_ID", SqlOperator.EqualTo, qConds.DealerCustomerId, GenericDBType.NVarChar));
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
            /// 料號摘要。
            /// </summary>
            [SchemaMapping(Name = "SUMMARY", Type = ReturnType.String)]
            public string Summary { get; set; }
            /// <summary>
            /// 備貨單使用量。
            /// </summary>
            [SchemaMapping(Name = "PG_ORDER_USE_QTY", Type = ReturnType.Int)]
            public int PGOrderUseQty { get; set; }
            /// <summary>
            /// 內銷訂單使用量。
            /// </summary>
            [SchemaMapping(Name = "DOM_ORDER_USE_QTY", Type = ReturnType.Int)]
            public int DomOrderUseQty { get; set; }

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
            /// <param name="quoteNumbers">報價單號碼陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="quoteItemIds">報價單明細項次陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="dealerCustomerId">ERP 客戶 ID（若為 null 或 empty 則略過條件檢查）。</param>
            public InfoViewConds(string[] quoteNumbers, string[] quoteItemIds, string dealerCustomerId)
            {
                this.QuoteNumbers = quoteNumbers;
                this.QuoteItemIds = quoteItemIds;
                this.DealerCustomerId = dealerCustomerId;
            }

            /// <summary>
            /// 報價單號碼陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public string[] QuoteNumbers { get; set; }
            /// <summary>
            /// 報價單明細項次陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public string[] QuoteItemIds { get; set; }
            /// <summary>
            /// ERP 客戶 ID（若為 null 或 empty 則略過條件檢查）。
            /// </summary>
            public string DealerCustomerId { get; set; }
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
        /// <param name="size">指定的前 N 筆資料（若為 Int32.MaxValue 則表示全部）。</param>
        /// <param name="sqlOrder">SQL 排序（Order By）的組合類別（若為 null 則使用預設排序）。</param>
        /// <param name="inquireColumns">含有零或多個指定查詢時所要使用的欄位名稱 System.String 陣列。若陣列為零，則使用預設開放的欄位名稱。</param>
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetInfoViewByCompoundSearch(InfoViewConds qConds, string[] columnsName, string keyword, bool isAbsolute, int size, SqlOrder sqlOrder, params string[] inquireColumns)
        {
            Returner returner = new Returner();

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.GetInfoByCompoundSearch(columnsName, keyword, EntityHelper.KEYWORD_SEPARATOR, isAbsolute, size, sqlOrder, GetInfoViewCondsSet(qConds), inquireColumns));
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

        #region GetGroupInfoView
        /// <summary> 
        /// 依指定的欄位取得群組資料。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <param name="fieldNames">欄位名稱陣列集合。</param>
        /// <param name="sort">排序方式。</param>
        /// <returns>EzCoding.Returner。</returns>
        public Returner GetGroupInfoView(InfoViewConds qConds, string[] fieldNames, Sort sort)
        {
            SqlCondsSet condsMain = new SqlCondsSet();
            condsMain.Add(GetInfoViewCondsSet(qConds));

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

            custEntity.Append("SELECT [PROJ_QUOTE].* ");
            custEntity.Append("       , [ERP_INV].[DESCRIPTION] AS [SUMMARY] ");
            //取備貨單的使用量 (不取已註刪的資料)
            custEntity.Append("       , (SELECT COALESCE(SUM([PG_ORDER_DET].[QTY]), 0) AS [CNT] FROM [PG_ORDER_DET] INNER JOIN [PG_ORDER] ON [PG_ORDER].[SID] = [PG_ORDER_DET].[PG_ORDER_SID] AND [PG_ORDER].[MDELED] = 'N' WHERE [PG_ORDER_DET].[SOURCE] = 2 AND [PG_ORDER_DET].[QUOTENUMBER] = [PROJ_QUOTE].[QUOTENUMBER] AND [PG_ORDER_DET].[QUOTEITEMID] = [PROJ_QUOTE].[QUOTEITEMID]) AS [PG_ORDER_USE_QTY] ");
            //取內銷訂單的使用量. 若內銷訂單中的專案報價品項來自於備貨單, 則不列入計算. (不取已註刪或已取消的資料)
            custEntity.Append("       , (SELECT COALESCE(SUM([DOM_ORDER_DET].[QTY]), 0) AS [CNT] FROM [DOM_ORDER_DET] INNER JOIN [DOM_ORDER] ON [DOM_ORDER].[SID] = [DOM_ORDER_DET].[DOM_ORDER_SID] AND [DOM_ORDER].[MDELED] = 'N' AND [DOM_ORDER].[IS_CANCEL] = 'N' WHERE [DOM_ORDER_DET].[SOURCE] = 2 AND [DOM_ORDER_DET].[QUOTENUMBER] = [PROJ_QUOTE].[QUOTENUMBER] AND [DOM_ORDER_DET].[QUOTEITEMID] = [PROJ_QUOTE].[QUOTEITEMID] AND [DOM_ORDER_DET].[PG_ORDER_DET_SID] IS NULL) AS [DOM_ORDER_USE_QTY] ");
            custEntity.Append("FROM [PROJ_QUOTE] ");
            custEntity.Append("     INNER JOIN [ERP_INV] ON [ERP_INV].[ITEM] = [PROJ_QUOTE].[PRODUCTID] ");

            var sqlConds = new List<string>();

            if (qConds.QuoteNumbers != null && qConds.QuoteNumbers.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "QUOTENUMBER", SqlOperator.EqualTo, qConds.QuoteNumbers, GenericDBType.NVarChar));
            }

            if (qConds.QuoteItemIds != null && qConds.QuoteItemIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "QUOTEITEMID", SqlOperator.EqualTo, qConds.QuoteItemIds, GenericDBType.NVarChar));
            }

            if (!string.IsNullOrWhiteSpace(qConds.DealerCustomerId))
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "DEALER_CUSTOMER_ID", SqlOperator.EqualTo, qConds.DealerCustomerId, GenericDBType.NVarChar));
            }

            custEntity.Append(SqlUtilBox.ToSqlConds(sqlConds.Where(q => !string.IsNullOrWhiteSpace(q)).ToArray(), " WHERE ", string.Empty));
            base.Entity.CustomEntitySqlSyntax = custEntity.GetSqlSyntax();
            #endregion

            SqlCondsSet condsMain = new SqlCondsSet();

            return condsMain;
        }
        #endregion
        #endregion

        //僅用於內銷訂單選專案報價的用途
        #region 群組報價單號碼查詢
        #region 表格資料
        /// <summary>
        /// 表格資料。
        /// </summary>
        public class GrpQuoteInfoView
        {
            /// <summary>
            /// 報價單號碼。
            /// </summary>
            [SchemaMapping(Name = "QUOTENUMBER", Type = ReturnType.String)]
            public string QuoteNumber { get; set; }
            /// <summary>
            /// 報價單日期。
            /// </summary>
            [SchemaMapping(Name = "QUOTEDATE", Type = ReturnType.DateTime)]
            public DateTime QuoteDate { get; set; }
            /// <summary>
            /// 報價單抬頭。
            /// </summary>
            [SchemaMapping(Name = "QUOTETITLE", Type = ReturnType.String)]
            public string QuoteTitle { get; set; }
            /// <summary>
            /// 報價單備註。
            /// </summary>
            [SchemaMapping(Name = "REMARK", Type = ReturnType.String)]
            public string Remark { get; set; }

            #region 繫結資料
            /// <summary>
            /// 繫結資料。
            /// </summary>
            public static GrpQuoteInfoView Binding(DataRow row)
            {
                return DBUtilBox.BindingRow<GrpQuoteInfoView>(new GrpQuoteInfoView(), row);
            }

            /// <summary>
            /// 繫結資料。
            /// </summary>
            public static GrpQuoteInfoView[] Binding(DataTable dTable)
            {
                List<GrpQuoteInfoView> infos = new List<GrpQuoteInfoView>();
                foreach (DataRow row in dTable.Rows)
                {
                    infos.Add(GrpQuoteInfoView.Binding(row));
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
        public class GrpQuoteInfoViewConds
        {
            ///// <summary>
            ///// 初始化 GrpQuoteInfoViewConds 類別的新執行個體。
            ///// 預設略過所有條件。
            ///// </summary>
            //public GrpQuoteInfoViewConds()
            //{

            //}

            /// <summary>
            /// 初始化 GrpQuoteInfoViewConds 類別的新執行個體。
            /// </summary>
            /// <param name="quoteNumber">報價單號碼（若為 null 或 empty 則略過條件檢查）。</param>
            /// <param name="quoteItemId">報價單明細項次（若為 null 或 empty 則略過條件檢查）。</param>
            /// <param name="dealerCustomerId">ERP 客戶 ID（若為 null 或 empty 則略過條件檢查）。</param>
            /// <param name="onlyAvailable">是否僅取得尚有可用數量的專案報價，若否則取得全部。</param>
            public GrpQuoteInfoViewConds(string quoteNumber, string quoteItemId, string dealerCustomerId, bool onlyAvailable)
            {
                this.QuoteNumber = quoteNumber;
                this.QuoteItemId = quoteItemId;
                this.DealerCustomerId = dealerCustomerId;
                this.OnlyAvailable = onlyAvailable;
            }

            /// <summary>
            /// 報價單號碼（若為 null 或 empty 則略過條件檢查）。
            /// </summary>
            public string QuoteNumber { get; set; }
            /// <summary>
            /// 報價單明細項次（若為 null 或 empty 則略過條件檢查）。
            /// </summary>
            public string QuoteItemId { get; set; }
            /// <summary>
            /// ERP 客戶 ID（若為 null 或 empty 則略過條件檢查）。
            /// </summary>
            public string DealerCustomerId { get; set; }
            /// <summary>
            /// 是否僅取得尚有可用數量的專案報價，若否則取得全部。
            /// </summary>
            public bool OnlyAvailable { get; set; }
        }
        #endregion

        #region GetGrpQuoteInfoView
        /// <summary> 
        /// 依指定的參數取得資料。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <param name="size">指定的前 N 筆資料（若為 Int32.MaxValue 則表示全部）。</param>
        /// <param name="sqlOrder">SQL 排序（Order By）的組合類別（若為 null 則使用預設排序）。</param>
        /// <param name="inquireColumns">含有零或多個指定查詢時所要使用的欄位名稱 System.String 陣列。若陣列為零，則使用預設開放的欄位名稱。</param>
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetGrpQuoteInfoView(GrpQuoteInfoViewConds qConds, int size, SqlOrder sqlOrder, params string[] inquireColumns)
        {
            Returner returner = new Returner();

            SqlCondsSet condsMain = new SqlCondsSet();
            condsMain.Add(GetGrpQuoteInfoViewCondsSet(qConds));

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
        public Returner GetGrpQuoteInfoView(GrpQuoteInfoViewConds qConds, PagingFlipper flipper, SqlOrder sqlOrder, params string[] inquireColumns)
        {
            Returner returner = new Returner();

            SqlCondsSet condsMain = new SqlCondsSet();
            condsMain.Add(GetGrpQuoteInfoViewCondsSet(qConds));

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.Entity.GetPagingBy(flipper, sqlOrder.UseDefault ? base.DefaultSqlOrder : sqlOrder, condsMain, inquireColumns));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetGrpQuoteInfoViewCount
        /// <summary> 
        /// 依指定的參數取得資料總數。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetGrpQuoteInfoViewCount(GrpQuoteInfoViewConds qConds)
        {
            Returner returner = new Returner();

            SqlCondsSet condsMain = new SqlCondsSet();
            condsMain.Add(GetGrpQuoteInfoViewCondsSet(qConds));

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.Entity.GetCountBy(condsMain));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetGrpQuoteInfoViewByCompoundSearch
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
        public Returner GetGrpQuoteInfoViewByCompoundSearch(GrpQuoteInfoViewConds qConds, string[] columnsName, string keyword, bool isAbsolute, PagingFlipper flipper, SqlOrder sqlOrder, params string[] inquireColumns)
        {
            Returner returner = new Returner();

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.GetInfoByCompoundSearch(columnsName, keyword, EntityHelper.KEYWORD_SEPARATOR, isAbsolute, flipper, sqlOrder, GetGrpQuoteInfoViewCondsSet(qConds), inquireColumns));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetGrpQuoteInfoViewByCompoundSearchCount
        /// <summary> 
        /// 依指定的參數取得資料總數（複合式查詢）。 
        /// 若欄位名稱陣列長度等於0，則會略過指定欄位名稱查詢動作。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <param name="columnsName">欄位名稱陣列。</param> 
        /// <param name="keyword">關鍵字。</param> 
        /// <param name="isAbsolute">是否為絕對查詢。若 false 則使用 like 查詢。</param> 
        /// <returns>EzCoding.Returner。</returns> 
        public Returner GetGrpQuoteInfoViewByCompoundSearchCount(GrpQuoteInfoViewConds qConds, string[] columnsName, string keyword, bool isAbsolute)
        {
            Returner returner = new Returner();

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.GetInfoByCompoundSearchCount(columnsName, keyword, EntityHelper.KEYWORD_SEPARATOR, isAbsolute, GetGrpQuoteInfoViewCondsSet(qConds)));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region GetGrpQuoteInfoViewCondsSet
        /// <summary> 
        /// 依指定的條件取得 SqlCondsSet。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <returns>EzCoding.Collections.SqlCondsSet。</returns> 
        SqlCondsSet GetGrpQuoteInfoViewCondsSet(GrpQuoteInfoViewConds qConds)
        {
            #region 自訂表格
            var custEntity = new SqlSyntaxBuilder(base.Entity, new SqlSyntax());

            custEntity.Append("SELECT [QUOTENUMBER], MIN([QUOTEDATE]) AS [QUOTEDATE], MIN([QUOTETITLE]) AS [QUOTETITLE], MIN([REMARK]) AS [REMARK] ");

            if (qConds.OnlyAvailable)
            {
                #region 計算專案報價在 備貨單 + 內銷訂單 的總使用量 (已註解)
                //custEntity.Append("FROM (");

                //custEntity.Append("SELECT * ");
                //custEntity.Append("FROM (");
                //custEntity.Append("SELECT [PROJ_QUOTE].* ");
                ////取備貨單的使用量 (不取已註刪的資料)
                //custEntity.Append("       , (SELECT COALESCE(SUM([PG_ORDER_DET].[QTY]), 0) AS [CNT] FROM [PG_ORDER_DET] INNER JOIN [PG_ORDER] ON [PG_ORDER].[SID] = [PG_ORDER_DET].[PG_ORDER_SID] AND [PG_ORDER].[MDELED] = 'N' WHERE [PG_ORDER_DET].[SOURCE] = 2 AND [PG_ORDER_DET].[QUOTENUMBER] = [PROJ_QUOTE].[QUOTENUMBER] AND [PG_ORDER_DET].[QUOTEITEMID] = [PROJ_QUOTE].[QUOTEITEMID]) AS [PG_ORDER_USE_QTY] ");
                ////取內銷訂單的使用量. 若內銷訂單中的專案報價品項來自於備貨單, 則不列入計算. (不取已註刪或已取消的資料)
                //custEntity.Append("       , (SELECT COALESCE(SUM([DOM_ORDER_DET].[QTY]), 0) AS [CNT] FROM [DOM_ORDER_DET] INNER JOIN [DOM_ORDER] ON [DOM_ORDER].[SID] = [DOM_ORDER_DET].[DOM_ORDER_SID] AND [DOM_ORDER].[MDELED] = 'N' AND [DOM_ORDER].[IS_CANCEL] = 'N' WHERE [DOM_ORDER_DET].[SOURCE] = 2 AND [DOM_ORDER_DET].[QUOTENUMBER] = [PROJ_QUOTE].[QUOTENUMBER] AND [DOM_ORDER_DET].[QUOTEITEMID] = [PROJ_QUOTE].[QUOTEITEMID] AND [DOM_ORDER_DET].[PG_ORDER_DET_SID] IS NULL) AS [DOM_ORDER_USE_QTY] ");
                //custEntity.Append("FROM [PROJ_QUOTE]");
                //custEntity.Append(") [PROJ_QUOTE] ");
                //custEntity.Append("WHERE [QUANTITY] > ([PG_ORDER_USE_QTY] + [DOM_ORDER_USE_QTY])");

                //custEntity.Append(") [PROJ_QUOTE] ");
                #endregion

                #region 僅計算專案報價在 內銷訂單 (包含從備貨單選的) 的總使用量
                custEntity.Append("FROM (");

                custEntity.Append("SELECT * ");
                custEntity.Append("FROM (");
                custEntity.Append("SELECT [PROJ_QUOTE].* ");
                //取內銷訂單 (包含從備貨單選的) 的使用量. (不取已註刪或已取消的資料)
                custEntity.Append("       , (SELECT COALESCE(SUM([DOM_ORDER_DET].[QTY]), 0) AS [CNT] FROM [DOM_ORDER_DET] INNER JOIN [DOM_ORDER] ON [DOM_ORDER].[SID] = [DOM_ORDER_DET].[DOM_ORDER_SID] AND [DOM_ORDER].[MDELED] = 'N' AND [DOM_ORDER].[IS_CANCEL] = 'N' WHERE [DOM_ORDER_DET].[QUOTENUMBER] = [PROJ_QUOTE].[QUOTENUMBER] AND [DOM_ORDER_DET].[QUOTEITEMID] = [PROJ_QUOTE].[QUOTEITEMID] AND [DOM_ORDER_DET].[PG_ORDER_DET_SID] IS NULL) AS [DOM_ORDER_USE_QTY] ");
                //custEntity.Append("FROM [PROJ_QUOTE]");
                //2016.8.29 米雪UPDATE：多加一個欄位判斷，ACTIVE IS NULL 代表專案明細還可以使用
                custEntity.Append("FROM [PROJ_QUOTE] WHERE [PROJ_QUOTE].[ACTIVE] IS NULL AND [PROJ_QUOTE].[IS_CANCEL]='N'");
                custEntity.Append(") [PROJ_QUOTE] ");
                custEntity.Append("WHERE [QUANTITY] > [DOM_ORDER_USE_QTY]");
                custEntity.Append(") [PROJ_QUOTE] ");
             
                #endregion
            }
            else
            {
                custEntity.Append("FROM [PROJ_QUOTE] ");
            }

            var sqlConds = new List<string>();

            if (!string.IsNullOrWhiteSpace(qConds.QuoteNumber))
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "QUOTENUMBER", SqlOperator.EqualTo, qConds.QuoteNumber, GenericDBType.NVarChar));
            }

            if (!string.IsNullOrWhiteSpace(qConds.QuoteItemId))
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "QUOTEITEMID", SqlOperator.EqualTo, qConds.QuoteItemId, GenericDBType.NVarChar));
            }

            if (!string.IsNullOrWhiteSpace(qConds.DealerCustomerId))
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "DEALER_CUSTOMER_ID", SqlOperator.EqualTo, qConds.DealerCustomerId, GenericDBType.NVarChar));
            }

            custEntity.Append(SqlUtilBox.ToSqlConds(sqlConds.Where(q => !string.IsNullOrWhiteSpace(q)).ToArray(), " WHERE ", string.Empty));


            custEntity.Append("GROUP BY [QUOTENUMBER] ");

            base.Entity.CustomEntitySqlSyntax = custEntity.GetSqlSyntax();
            #endregion

            SqlCondsSet condsMain = new SqlCondsSet();

            return condsMain;
        }
        #endregion
        #endregion
    }
}
