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
        public const string PROJ_QUOTE_Dist = "PROJ_QUOTE";
    }

    /// <summary>
    /// 專案報價的類別實作。
    /// </summary>
    public class ProjQuote_dist : SystemBase
    {
        /// <summary>
        /// 初始化 Seec.Marketing.SystemEngines.ProjQuote 類別的新執行個體。
        /// </summary>
        /// <param name="connInfo">資料庫連線資訊。</param>
        public ProjQuote_dist(string connInfo)
            : base(EntityHelper.GetEntityBase(connInfo, DBTableDefine.PROJ_QUOTE))
        {
            SqlOrder sorting = new SqlOrder();
            sorting.Add("QUOTEDATE", Sort.Descending);
            sorting.Add("QUOTENUMBER", Sort.Descending);

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
            /// ERP客戶名稱。
            /// </summary>
            [SchemaMapping(Name = "DIST_NAME", Type = ReturnType.String)]
            public string DistName { get; set; }
            /// <summary>
            /// 內銷地區系統代號。
            /// </summary>
            [SchemaMapping(Name = "NAME", Type = ReturnType.SId)]
            public ISystemId DomDistSId { get; set; }
            /// <summary>
            /// ERP產品料號。
            /// </summary>
            [SchemaMapping(Name = "ITEM", Type = ReturnType.String)]
            public string ITEM { get; set; }
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

        #region 一般查詢
        #region 查詢條件 InfoConds
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
            /// <param name="beginEdd">範圍查詢的起始預計出貨日（若為 null 則略過條件檢查）。</param>
            /// <param name="endEdd">範圍查詢的結束預計出貨日（若為 null 則略過條件檢查）。</param>
            /// <param name="domDistSIds">內銷地區系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="quoteNumbers">報價單號碼陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="dealer_erp_name">客戶名稱陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            public InfoConds(DateTime? beginEdd, DateTime? endEdd, string[] domDistSIds, string[] quoteNumbers, string[] dealer_erp_name, bool? isCancel, string active)
            {
                this.BeginEdd = beginEdd;
                this.EndEdd = endEdd;
                this.DomDistSIds = domDistSIds;
                this.QuoteNumbers = quoteNumbers;
                //2017-05-25 CRM專案取消 客戶名稱
                this.Dealer_ERP_Name = dealer_erp_name;
                this.IsCancel = isCancel;
                //2017-05-24 CRM專案取消 使用完畢之報價單不顯示
                this.ACTIVE = active;                
            }

            /// <summary>
            /// 報價單號碼陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public string[] QuoteNumbers { get; set; }
            //2017-05-25 CRM專案取消 客戶名稱
            /// <summary>
            /// 客戶名稱陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public string[] Dealer_ERP_Name { get; set; }
            /// <summary>
            /// 報價單明細項次陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public string[] QuoteItemIds { get; set; }
            /// <summary>
            /// ERP 客戶 ID（若為 null 或 empty 則略過條件檢查）。
            /// </summary>
            public string DealerCustomerId { get; set; }
            /// <summary>
            /// 內銷地區系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public string[] DomDistSIds { get; set; }
            /// <summary>
            /// 範圍查詢的起始預計出貨日（若為 null 則略過條件檢查）。
            /// </summary>
            public DateTime? BeginEdd { get; set; }
            /// <summary>
            /// 範圍查詢的結束預計出貨日（若為 null 則略過條件檢查）。
            /// </summary>
            public DateTime? EndEdd { get; set; }
            /// <summary>
            /// 是否已取消（若為 null 則略過條件檢查）。
            /// </summary>
            public bool? IsCancel { get; set; }
            /// <summary>
            /// 是否已用完（若為 null 則略過條件檢查）。
            /// </summary>
            public string ACTIVE { get; set; }            
        }
        #endregion

        #region 取得資料 GetInfo
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

        #region 取得資料總數 GetInfoCount
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

            custEntity.Append("SELECT distinct [QUOTENUMBER] ");
            custEntity.Append(", [QUOTEID] ");            
            custEntity.Append(", [DEALER_ERP_NAME] ");
            custEntity.Append(", [QUOTEDATE] ");
            custEntity.Append(", [QUOTETITLE] ");
            custEntity.Append(", [REMARK] ");
            //custEntity.Append(", [QUOTEITEMID] ");
            custEntity.Append(", [SID], [CODE], [NAME] ");
            custEntity.Append("FROM [PROJ_QUOTE] ");
            custEntity.Append("inner join [PUB_CAT] on SUBSTRING([PROJ_QUOTE].[QUOTENUMBER], 0, 5)=CODE ");

            var sqlConds = new List<string>();

            if (qConds.BeginEdd != null)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "QUOTEDATE", SqlOperator.GreaterEqualThan, qConds.BeginEdd.Value.ToString("yyyyMMdd"), GenericDBType.Char));
            }

            if (qConds.EndEdd != null)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "QUOTEDATE", SqlOperator.LessEqualThan, qConds.EndEdd.Value.ToString("yyyyMMdd235959"), GenericDBType.Char));
            }

            if (qConds.DomDistSIds != null && qConds.DomDistSIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "SID", SqlOperator.EqualTo, qConds.DomDistSIds, GenericDBType.NVarChar));
            }

            if (qConds.QuoteNumbers != null && qConds.QuoteNumbers.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "QUOTENUMBER", SqlOperator.EqualTo, qConds.QuoteNumbers, GenericDBType.NVarChar));
            }
            //2017-05-25 CRM專案取消 客戶名稱
            if (qConds.Dealer_ERP_Name != null && qConds.Dealer_ERP_Name.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "DEALER_ERP_NAME", SqlOperator.EqualTo, qConds.Dealer_ERP_Name, GenericDBType.NVarChar));
            }
            if (qConds.IsCancel.HasValue)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "IS_CANCEL", SqlOperator.EqualTo, qConds.IsCancel.Value ? "Y" : "N", GenericDBType.Char));
            }
            //2017-05-24 CRM專案取消 使用完畢之報價單不顯示
            if (string.IsNullOrWhiteSpace(qConds.ACTIVE))
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "ACTIVE", SqlOperator.IsNull, qConds.ACTIVE, GenericDBType.NVarChar));
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
            /// 內銷地區名稱。
            /// </summary>
            [SchemaMapping(Name = "NAME", Type = ReturnType.String)]
            public string DomDistName { get; set; }

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
        #endregion

        #region 報價單號碼查詢
        #region 表格資料
        /// <summary>
        /// 表格資料。
        /// </summary>
        public class QuoteContentInfoView : InfoBase
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
            /// 內銷地區系統代號。
            /// </summary>
            [SchemaMapping(Name = "DomDistName", Type = ReturnType.SId)]
            public ISystemId DomDistSId { get; set; }
            /// <summary>
            /// ERP產品料號。
            /// </summary>
            [SchemaMapping(Name = "ITEM", Type = ReturnType.String)]
            public string ITEM { get; set; }

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

        #region 查詢條件
        /// <summary>
        /// 查詢條件。
        /// </summary>
        public class QuoteContentInfoViewConds
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
            /// <param name="quoteID">報價單ID系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            public QuoteContentInfoViewConds(string quoteID)
            {
                this.QuoteID = quoteID;
            }

            /// <summary>
            /// 報價單ID陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public string QuoteID { get; set; }
        }
        #endregion

        #region 取得資料 QuoteContentGetInfoView
        /// <summary> 
        /// 依指定的參數取得資料。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <param name="size">指定的前 N 筆資料（若為 Int32.MaxValue 則表示全部）。</param>
        /// <param name="sqlOrder">SQL 排序（Order By）的組合類別（若為 null 則使用預設排序）。</param>
        /// <param name="inquireColumns">含有零或多個指定查詢時所要使用的欄位名稱 System.String 陣列。若陣列為零，則使用預設開放的欄位名稱。</param>
        /// <returns>EzCoding.Returner。</returns> 
        public Returner QuoteContentGetInfoView(QuoteContentInfoViewConds qConds, int size, SqlOrder sqlOrder, params string[] inquireColumns)
        {
            Returner returner = new Returner();

            SqlCondsSet condsMain = new SqlCondsSet();
            condsMain.Add(QuoteContentGetInfoViewCondsSet(qConds));

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
        public Returner QuoteContentGetInfoView(QuoteContentInfoViewConds qConds, PagingFlipper flipper, SqlOrder sqlOrder, params string[] inquireColumns)
        {
            Returner returner = new Returner();

            SqlCondsSet condsMain = new SqlCondsSet();
            condsMain.Add(QuoteContentGetInfoViewCondsSet(qConds));

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.Entity.GetPagingBy(flipper, sqlOrder.UseDefault ? base.DefaultSqlOrder : sqlOrder, condsMain, inquireColumns));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region 取得資料總數 GetInfoCount
        /// <summary> 
        /// 依指定的參數取得資料總數。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <returns>EzCoding.Returner。</returns> 
        public Returner QuoteContentGetInfoCount(QuoteContentInfoViewConds qConds)
        {
            Returner returner = new Returner();

            SqlCondsSet condsMain = new SqlCondsSet();
            condsMain.Add(QuoteContentGetInfoViewCondsSet(qConds));

            base.Entity.EnableCustomEntity = true;
            returner.ChangeInto(base.Entity.GetCountBy(condsMain));
            base.Entity.EnableCustomEntity = false;
            return returner;
        }
        #endregion

        #region SQL QuoteContentGetInfoViewCondsSet
        /// <summary> 
        /// 依指定的條件取得 SqlCondsSet。 
        /// </summary> 
        /// <param name="qConds">查詢條件。</param> 
        /// <returns>EzCoding.Collections.SqlCondsSet。</returns> 
        SqlCondsSet QuoteContentGetInfoViewCondsSet(QuoteContentInfoViewConds qConds)
        {
            #region 自訂表格
            var custEntity = new SqlSyntaxBuilder(base.Entity, new SqlSyntax());

            custEntity.Append("SELECT [QUOTEID] ");
            custEntity.Append(", [QUOTENUMBER] ");
            custEntity.Append(", [DEALER_ERP_NAME] ");
            custEntity.Append(", [QUOTEDATE] ");
            custEntity.Append(", [QUOTETITLE] ");
            custEntity.Append(", [REMARK] ");
            custEntity.Append(", [PRODUCTID] ");
            custEntity.Append(", [QUANTITY] ");
            custEntity.Append(", [UNITPRICE] ");
            custEntity.Append(", [PROJ_QUOTE].[DISCOUNT] ");
            custEntity.Append(", [QUOTEITEMID] ");
            custEntity.Append(", [DEALER_CUSTOMER_ID] ");
            custEntity.Append(", [ERP_INV].[ITEM] ");
            custEntity.Append("FROM [PROJ_QUOTE] ");
            custEntity.Append("LEFT OUTER JOIN [ERP_INV] ON [ERP_INV].[ITEM]=[PROJ_QUOTE].PRODUCTID");

            var sqlConds = new List<string>();

            if (qConds.QuoteID != null && qConds.QuoteID.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "QUOTEID", SqlOperator.EqualTo, qConds.QuoteID, GenericDBType.NVarChar));
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
