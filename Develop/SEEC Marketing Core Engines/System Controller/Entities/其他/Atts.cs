using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
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
        /// 附加資料檔。
        /// </summary>
        public const string ATTS = "ATTS";
    }

    /// <summary>
    /// 附加資料檔的類別實作。
    /// </summary>
    public class Atts : SystemHandler
    {
        /// <summary>
        /// 初始化 Seec.Marketing.SystemEngines.Atts 類別的新執行個體。
        /// 預設使用的 Primary Key 欄位名稱為「SID」及 EzCoding.SystemIdVerifier 系統代號驗證物件。
        /// </summary>
        /// <param name="connInfo">資料庫連線資訊。</param>
        public Atts(string connInfo)
            : base(EntityHelper.GetEntity(connInfo, DBTableDefine.ATTS))
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
            /// 單元代碼（1:文件 1000:通用分類）。
            /// </summary>
            [SchemaMapping(Name = "UNIT_CODE", Type = ReturnType.Int)]
            public int UnitCode { get; set; }
            /// <summary>
            /// 單元系統代號。
            /// </summary>
            [SchemaMapping(Name = "UNIT_SID", Type = ReturnType.SId)]
            public ISystemId UnitSId { get; set; }
            /// <summary>
            /// 自訂代碼。
            /// </summary>
            [SchemaMapping(Name = "CUST_CODE", Type = ReturnType.Int)]
            public int CustCode { get; set; }
            /// <summary>
            /// 附加資料抬頭。
            /// </summary>
            [SchemaMapping(Name = "ATT_TITLE", Type = ReturnType.String)]
            public string AttTitle { get; set; }
            /// <summary>
            /// 附加資料名稱。
            /// </summary>
            [SchemaMapping(Name = "ATT_NAME", Type = ReturnType.String)]
            public string AttName { get; set; }
            /// <summary>
            /// 附加資料內容。
            /// </summary>
            [SchemaMapping(Name = "ATT_CONT", Type = ReturnType.String)]
            public string AttCont { get; set; }
            /// <summary>
            /// 說明。
            /// </summary>
            [SchemaMapping(Name = "DESC", Type = ReturnType.String)]
            public string Desc { get; set; }
            /// <summary>
            /// 附加資料類型（1:Text 2:File 3:TextWrap）。
            /// </summary>
            [SchemaMapping(Name = "ATT_TYPE", Type = ReturnType.Int)]
            public int AttType { get; set; }
            /// <summary>
            /// 附加資料用途（1:Text 2:Hyperlink 3:TextWrap 4:Download 5:Image 6:Flash 7:Video）。
            /// </summary>
            [SchemaMapping(Name = "ATT_USE", Type = ReturnType.Int)]
            public int AttUse { get; set; }
            /// <summary>
            /// 附加資料位置（0:不使用 1:置左 2:置中 3:置右 4:置上 5:置下）。
            /// </summary>
            [SchemaMapping(Name = "ATT_POS", Type = ReturnType.Int, AllowNull = true)]
            public int? AttPos { get; set; }
            /// <summary>
            /// 寬度。
            /// </summary>
            [SchemaMapping(Name = "WIDTH", Type = ReturnType.Int, AllowNull = true)]
            public int? Width { get; set; }
            /// <summary>
            /// 高度。
            /// </summary>
            [SchemaMapping(Name = "HEIGHT", Type = ReturnType.Int, AllowNull = true)]
            public int? Height { get; set; }
            /// <summary>
            /// 檔案大小。
            /// </summary>
            [SchemaMapping(Name = "FILE_SIZE", Type = ReturnType.Int, AllowNull = true)]
            public int? FileSize { get; set; }
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
            /// 單元代碼（1:文件 1000:通用分類）。
            /// </summary>
            public int? UnitCode { get; set; }
            /// <summary>
            /// 單元系統代號。
            /// </summary>
            public ISystemId UnitSId { get; set; }
            /// <summary>
            /// 自訂代碼。
            /// </summary>
            public int? CustCode { get; set; }
            /// <summary>
            /// 附加資料抬頭。
            /// </summary>
            public string AttTitle { get; set; }
            /// <summary>
            /// 附加資料名稱。
            /// </summary>
            public string AttName { get; set; }
            /// <summary>
            /// 附加資料內容。
            /// </summary>
            public string AttCont { get; set; }
            /// <summary>
            /// 說明。
            /// </summary>
            public string Desc { get; set; }
            /// <summary>
            /// 附加資料類型（1:Text 2:File 3:TextWrap）。
            /// </summary>
            public int? AttType { get; set; }
            /// <summary>
            /// 附加資料用途（1:Text 2:Hyperlink 3:TextWrap 4:Download 5:Image 6:Flash 7:Video）。
            /// </summary>
            public int? AttUse { get; set; }
            /// <summary>
            /// 附加資料位置（0:不使用 1:置左 2:置中 3:置右 4:置上 5:置下）。
            /// </summary>
            public int? AttPos { get; set; }
            /// <summary>
            /// 寬度。
            /// </summary>
            public int? Width { get; set; }
            /// <summary>
            /// 高度。
            /// </summary>
            public int? Height { get; set; }
            /// <summary>
            /// 檔案大小。
            /// </summary>
            public int? FileSize { get; set; }
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
        /// <param name="attsSId">指定的附加資料檔系統代號。</param>
        /// <param name="unitCode">單元代碼。</param>
        /// <param name="unitSId">單元系統代號。</param>
        /// <param name="custCode">自訂代碼。</param>
        /// <param name="attTitle">附加資料抬頭（null 或 empty 將自動略過操作）。</param>
        /// <param name="attName">附加資料名稱（檔名或網址）（null 或 empty 將自動略過操作）。</param>
        /// <param name="attCont">附加資料內容（純文字）（null 或 empty 將自動略過操作）。</param>
        /// <param name="desc">說明（null 或 empty 將自動略過操作）。</param>
        /// <param name="attType">附加資料類型（1:Text 2:File 3:TextWrap）。</param>
        /// <param name="attUse">附加資料用途（1:Text 2:Hyperlink 3:TextWrap 4:Download 5:Image 6:Flash 7:Video）。</param>
        /// <param name="attPos">附加資料位置（0:不使用 1:置左 2:置中 3:置右 4:置上 5:置下; null 將自動略過操作）。</param>
        /// <param name="width">寬度（適用於圖片或 Flash; null 將自動略過操作）</param>
        /// <param name="height">高度（適用於圖片或 Flash; null 將自動略過操作）</param>
        /// <param name="fileSize">檔案大小（適用於圖片或檔案; null 將自動略過操作）</param>
        /// <param name="sort">指定的排序。</param>
        /// <returns>EzCoding.Returner。若新增成功則回傳時包含著一個「NEW_SID」的 DataTable 和「TABLE_NAME」、「SID」兩個欄位。</returns>
        /// <exception cref="System.ArgumentNullException">參數 actorSId 不允許為 null 值。</exception>
        /// <exception cref="System.ArgumentNullException">參數 unitSId 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        public Returner Add(ISystemId actorSId, ISystemId attsSId, int unitCode, ISystemId unitSId, int custCode, string attTitle, string attName, string attCont, string desc, AttachType attType, AttachUse attUse, int? attPos, int? width, int? height, int? fileSize, int sort)
        {
            if (actorSId == null) { throw new ArgumentNullException("actorSId"); }
            if (unitSId == null) { throw new ArgumentNullException("unitSId"); }
            string[] exceptionSIds = this.SystemIdVerifier.CheckValid(actorSId, unitSId);
            if (exceptionSIds.Length > 0)
            {
                throw new LibraryException(string.Format("invaild ISystemId ({0}) format.", string.Join("、", exceptionSIds)));
            }

            Returner returner = new Returner();
            try
            {
                #region 交易緩衝區
                TransColValSet transSet = new TransColValSet();
                transSet.SmartAdd("SID", attsSId.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("CSID", actorSId.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("MSID", actorSId.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("UNIT_CODE", unitCode, GenericDBType.Int);
                transSet.SmartAdd("UNIT_SID", unitSId.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("CUST_CODE", custCode, GenericDBType.Int);
                transSet.SmartAdd("ATT_TITLE", attTitle, GenericDBType.NVarChar, true);
                transSet.SmartAdd("ATT_NAME", attName, GenericDBType.NVarChar, true);
                transSet.SmartAdd("ATT_CONT", attCont, GenericDBType.NVarChar, true);
                transSet.SmartAdd("DESC", desc, GenericDBType.NVarChar, true);
                transSet.SmartAdd("ATT_TYPE", (int)attType, GenericDBType.Int);
                transSet.SmartAdd("ATT_USE", (int)attUse, GenericDBType.Int);
                transSet.SmartAdd("ATT_POS", attPos, GenericDBType.Int);
                transSet.SmartAdd("WIDTH", width, GenericDBType.Int);
                transSet.SmartAdd("HEIGHT", height, GenericDBType.Int);
                transSet.SmartAdd("FILE_SIZE", fileSize, GenericDBType.Int);
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
        /// 依指定的參數，修改一筆資料。
        /// </summary>
        /// <param name="actorSId">操作人系統代號。為客戶端在操作時的操作人系統代號。</param>
        /// <param name="attSId">附加資料檔系統代號。</param>
        /// <param name="attTitle">附加資料抬頭（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="resetAttName">是否重設附加資料名稱。</param>
        /// <param name="attName">附加資料名稱（檔名或網址或純文字）（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="attCont">附加資料內容（純文字）（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="desc">說明（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="attPos">附加資料位置（0:不使用 1:置左 2:置中 3:置右 4:置上 5:置下; null 則直接設為 DBNull）。</param>
        /// <param name="width">寬度（適用於圖片或 Flash; null 則直接設為 DBNull）</param>
        /// <param name="height">高度（適用於圖片或 Flash; null 則直接設為 DBNull）</param>
        /// <param name="fileSize">檔案大小（適用於圖片或檔案; null 則直接設為 DBNull）</param>
        /// <param name="sort">指定的排序。</param>
        /// <returns>EzCoding.Returner。</returns>
        /// <exception cref="System.ArgumentNullException">參數 actorSId 不允許為 null 值。</exception>
        /// <exception cref="System.ArgumentNullException">參數 attSId 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        public Returner Modify(ISystemId actorSId, ISystemId attSId, string attTitle, bool resetAttName, string attName, string attCont, string desc, int? attPos, int? width, int? height, int? fileSize, int sort)
        {
            if (actorSId == null) { throw new ArgumentNullException("actorSId"); }
            if (attSId == null) { throw new ArgumentNullException("attSId"); }
            string[] exceptionSIds = this.SystemIdVerifier.CheckValid(actorSId, attSId);
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
                transSet.SmartAdd("ATT_TITLE", attTitle, GenericDBType.NVarChar, true, true);
                if (resetAttName)
                {
                    transSet.SmartAdd("ATT_NAME", attName, GenericDBType.NVarChar, true, true);
                    transSet.SmartAdd("FILE_SIZE", fileSize, GenericDBType.Int, true);
                }
                transSet.SmartAdd("ATT_CONT", attCont, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("DESC", desc, GenericDBType.NVarChar, true, true);
                transSet.SmartAdd("ATT_POS", attPos, GenericDBType.Int, true);
                transSet.SmartAdd("WIDTH", width, GenericDBType.Int, true);
                transSet.SmartAdd("HEIGHT", height, GenericDBType.Int, true);
                transSet.SmartAdd("SORT", sort, GenericDBType.Int);

                SqlCondsSet condsMain = new SqlCondsSet();
                condsMain.Add(base.PrimaryKey, SqlCond.EqualTo, attSId.ToString(), GenericDBType.Char, string.Empty);

                returner.ChangeInto(base.Modify(transSet, condsMain, true));
                #endregion

                return returner;
            }
            finally
            {
            }
        }
        #endregion

        #region Delete
        /// <summary>
        /// 依指定的參數刪除資料。
        /// </summary>
        /// <param name="actorSId">操作人系統代號。</param>
        /// <param name="unitCode">單元代碼。</param>
        /// <param name="unitSIds">單元系統代號陣列。</param>
        /// <param name="custCode">自訂代碼。</param>
        /// <param name="attType">附加資料類型（1:Text 2:File 3:TextWrap）。</param>
        /// <param name="attUse">附加資料用途（1:Text 2:Hyperlink 3:TextWrap 4:Download 5:Image 6:Flash 7:Video）。</param>
        /// <returns>EzCoding.Returner。</returns>
        /// <exception cref="System.ArgumentNullException">參數 actorSId 不允許為 null 值。</exception>
        /// <exception cref="System.ArgumentNullException">參數 systemids 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        public Returner Delete(ISystemId actorSId, int unitCode, ISystemId[] unitSIds, int custCode, AttachType attType, AttachUse attUse)
        {
            return this.Delete(actorSId, unitCode, unitSIds, custCode, attType, attUse, true);
        }

        /// <summary>
        /// 依指定的參數刪除資料。
        /// </summary>
        /// <param name="actorSId">操作人系統代號。</param>
        /// <param name="unitCode">單元代碼。</param>
        /// <param name="unitSIds">單元系統代號陣列。</param>
        /// <param name="custCode">自訂代碼。</param>
        /// <param name="attType">附加資料類型（1:Text 2:File 3:TextWrap）。</param>
        /// <param name="attUse">附加資料用途（1:Text 2:Hyperlink 3:TextWrap 4:Download 5:Image 6:Flash 7:Video）。</param>
        /// <param name="executeTransaction">是否開始執行交易。</param>
        /// <returns>EzCoding.Returner。</returns>
        /// <exception cref="System.ArgumentNullException">參數 actorSId 不允許為 null 值。</exception>
        /// <exception cref="System.ArgumentNullException">參數 systemids 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        internal Returner Delete(ISystemId actorSId, int unitCode, ISystemId[] unitSIds, int custCode, AttachType attType, AttachUse attUse, bool executeTransaction)
        {
            if (actorSId == null) { throw new ArgumentNullException("actorSId"); }
            if (unitSIds == null) { throw new ArgumentNullException("unitSIds"); }
            string[] exceptionSIds = this.SystemIdVerifier.CheckValid(actorSId);
            if (exceptionSIds.Length > 0)
            {
                throw new LibraryException(string.Format("invaild ISystemId ({0}) format.", string.Join("、", exceptionSIds)));
            }
            exceptionSIds = this.SystemIdVerifier.CheckValid(unitSIds);
            if (exceptionSIds.Length > 0)
            {
                throw new LibraryException(string.Format("invaild ISystemId ({0}) format.", string.Join("、", exceptionSIds)));
            }

            SqlCondsSet condsMain = new SqlCondsSet();
            condsMain.Add("UNIT_CODE", SqlCond.EqualTo, unitCode, GenericDBType.Int, SqlCondsSet.And);
            condsMain.Add("CUST_CODE", SqlCond.EqualTo, custCode, GenericDBType.Int, SqlCondsSet.And);
            condsMain.Add("ATT_TYPE", SqlCond.EqualTo, (int)attType, GenericDBType.Int, SqlCondsSet.And);
            condsMain.Add("ATT_USE", SqlCond.EqualTo, (int)attUse, GenericDBType.Int, SqlCondsSet.And);

            if (unitSIds != null && unitSIds.Length > 0)
            {
                condsMain.Add("UNIT_SID", true, SystemId.ToString(unitSIds), GenericDBType.Char, SqlCondsSet.And);
            }

            return base.Delete(condsMain, executeTransaction);
        }
        #endregion

        #region DeleteByUnitSId
        /// <summary>
        /// 依指定的單元系統代號刪除資料。
        /// </summary>
        /// <param name="unitCode">單元代碼。</param>
        /// <param name="unitSIds">單元系統代號陣列集合。</param>
        /// <returns>EzCoding.Returner。</returns>
        public Returner DeleteByUnitSId(int unitCode, ISystemId[] unitSIds)
        {
            if (unitSIds == null || unitSIds.Length == 0)
            {
                return new Returner();
            }

            Returner returner = new Returner();

            try
            {
                SqlCondsSet condsMain = new SqlCondsSet();
                condsMain.Add("UNIT_CODE", SqlCond.EqualTo, unitCode, GenericDBType.Int, SqlCondsSet.And);
                condsMain.Add("UNIT_SID", true, SystemId.ToString(unitSIds), GenericDBType.Char, SqlCondsSet.And);

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
            /// <param name="unitCode">單元代碼。</param>
            /// <param name="unitSId">單元系統代號。</param>
            /// <param name="custCode">自訂代碼（若為 null 則略過條件檢查）。</param>
            /// <param name="attTypes">附加資料類型陣列集合（1:Text 2:File 3:TextWrap; 若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="attUse">附加資料用途（1:Text 2:Hyperlink 3:TextWrap 4:Download 5:Image 6:Flash 7:Video; 若為 null 則略過條件檢查）。</param>
            public InfoConds(int unitCode, ISystemId unitSId, int? custCode, AttachType[] attTypes, AttachUse? attUse)
            {
                this.UnitCode = unitCode;
                this.UnitSId = unitSId;
                this.CustCode = custCode;
                this.AttTypes = attTypes;
                this.AttUse = attUse;
            }

            /// <summary>
            /// 單元代碼。
            /// </summary>
            public int UnitCode { get; set; }
            /// <summary>
            /// 單元系統代號。
            /// </summary>
            public ISystemId UnitSId { get; set; }
            /// <summary>
            /// 自訂代碼（若為 null 則略過條件檢查）。
            /// </summary>
            public int? CustCode { get; set; }
            /// <summary>
            /// 附加資料類型陣列集合（1:Text 2:File 3:TextWrap; 若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public AttachType[] AttTypes { get; set; }
            /// <summary>
            /// 附加資料用途（1:Text 2:Hyperlink 3:TextWrap 4:Download 5:Image 6:Flash 7:Video; 若為 null 則略過條件檢查）。
            /// </summary>
            public AttachUse? AttUse { get; set; }
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

            custEntity.Append("SELECT [ATTS].* ");
            custEntity.Append("FROM [ATTS] ");

            var sqlConds = new List<string>();

            sqlConds.Add(custEntity.BuildConds(string.Empty, "UNIT_CODE", SqlOperator.EqualTo, qConds.UnitCode, GenericDBType.Int));
            sqlConds.Add(custEntity.BuildConds(string.Empty, "UNIT_SID", SqlOperator.EqualTo, qConds.UnitSId.Value, GenericDBType.Char));

            if (qConds.CustCode.HasValue)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "CUST_CODE", SqlOperator.EqualTo, qConds.CustCode.Value, GenericDBType.Int));
            }

            if (qConds.AttUse.HasValue)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "ATT_USE", SqlOperator.EqualTo, qConds.AttUse.Value, GenericDBType.Int));
            }

            if (qConds.AttTypes != null && qConds.AttTypes.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "ATT_TYPE", SqlOperator.EqualTo, qConds.AttTypes.Select(q => (object)q).ToArray(), GenericDBType.Int));
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
