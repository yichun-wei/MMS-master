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
        /// ERP 庫存。
        /// </summary>
        public const string ERP_INV = "ERP_INV";
    }

    /// <summary>
    /// ERP 庫存的類別實作。
    /// </summary>
    public class ErpInv : SystemHandler
    {
        /// <summary>
        /// 初始化 Seec.Marketing.SystemEngines.ErpInv 類別的新執行個體。
        /// 預設使用的 Primary Key 欄位名稱為「SID」及 EzCoding.SystemIdVerifier 系統代號驗證物件。
        /// </summary>
        /// <param name="connInfo">資料庫連線資訊。</param>
        public ErpInv(string connInfo)
            : base(EntityHelper.GetEntity(connInfo, DBTableDefine.ERP_INV))
        {
            SqlOrder sorting = new SqlOrder();
            sorting.Add("INVENTORY_ITEM_ID", Sort.Ascending);

            base.DefaultSqlOrder = sorting;
        }

        #region 表格資料
        /// <summary>
        /// 表格資料。
        /// </summary>
        public class Info : InfoBase, IErpInv
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
            /// 料號 ID。
            /// </summary>
            [SchemaMapping(Name = "INVENTORY_ITEM_ID", Type = ReturnType.Long)]
            public long InventoryItemId { get; set; }
            /// <summary>
            /// 料號。
            /// </summary>
            [SchemaMapping(Name = "ITEM", Type = ReturnType.String)]
            public string Item { get; set; }
            /// <summary>
            /// 料號摘要。
            /// </summary>
            [SchemaMapping(Name = "DESCRIPTION", Type = ReturnType.String)]
            public string Description { get; set; }
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
            /// 預設明細折數。
            /// </summary>
            [SchemaMapping(Name = "DISCOUNT", Type = ReturnType.Float, AllowNull = true)]
            public float? Discount { get; set; }
            /// <summary>
            /// 料號大分類。
            /// </summary>
            [SchemaMapping(Name = "SEGMENT1", Type = ReturnType.String)]
            public string Segment1 { get; set; }
            /// <summary>
            /// 料號中分類。
            /// </summary>
            [SchemaMapping(Name = "SEGMENT2", Type = ReturnType.String)]
            public string Segment2 { get; set; }
            /// <summary>
            /// 料號小分類。
            /// </summary>
            [SchemaMapping(Name = "SEGMENT3", Type = ReturnType.String)]
            public string Segment3 { get; set; }
            /// <summary>
            /// 料號所屬製造組織 ID。
            /// </summary>
            [SchemaMapping(Name = "ORGANIZATION_ID", Type = ReturnType.Long)]
            public long OrganizationId { get; set; }
            /// <summary>
            /// 料號所屬製造組織代碼。
            /// </summary>
            [SchemaMapping(Name = "ORGANIZATION_CODE", Type = ReturnType.String)]
            public string OrganizationCode { get; set; }
            /// <summary>
            /// 料號啟用。
            /// </summary>
            [SchemaMapping(Name = "ENABLED_FLAG", Type = ReturnType.Bool)]
            public bool EnabledFlag { get; set; }
            /// <summary>
            /// 料號最後更新日期。
            /// </summary>
            [SchemaMapping(Name = "LAST_UPDATE_DATE", Type = ReturnType.DateTime)]
            public DateTime LastUpdateDate { get; set; }
            /// <summary>
            /// XS型號。
            /// </summary>
            [SchemaMapping(Name = "XSD_ITEM", Type = ReturnType.String)]
            public string XSDItem { get; set; }

            /// <summary>
            /// 型號。
            /// </summary>
            [SchemaMapping(Name = "MODEL", Type = ReturnType.String)]
            public string Model { get; set; }

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
        /// <param name="actorSId">操作人系統代號。為庫存端在操作時的操作人系統代號。</param> 
        /// <param name="erpInvSId">指定的 ERP 庫存系統代號。</param>
        /// <param name="enabled">資料的啟用狀態。</param>
        /// <param name="inventoryItemId">料號 ID。</param>
        /// <param name="item">料號（null 或 empty 將自動略過操作）。</param>
        /// <param name="description">料號摘要（null 或 empty 將自動略過操作）。</param>
        /// <param name="unitWeight">料號重量（null 將自動略過操作）。</param>
        /// <param name="weightUomCode">重量單位（null 或 empty 將自動略過操作）。</param>
        /// <param name="discount">預設明細折數（null 將自動略過操作）。</param>
        /// <param name="segment1">料號大分類（null 或 empty 將自動略過操作）。</param>
        /// <param name="segment2">料號中分類（null 或 empty 將自動略過操作）。</param>
        /// <param name="segment3">料號小分類（null 或 empty 將自動略過操作）。</param>
        /// <param name="organizationId">料號所屬製造組織 ID。</param>
        /// <param name="organizationCode">料號所屬製造組織代碼（null 或 empty 將自動略過操作）。</param>
        /// <param name="enabledFlag">料號啟用。</param>
        /// <param name="lastUpdateDate">料號最後更新日期。</param>
        /// <param name="XSDItem">XS型號。</param>
        /// <returns>EzCoding.Returner。若新增成功則回傳時包含著一個「NEW_SID」的 DataTable 和「TABLE_NAME」、「SID」兩個欄位。</returns>
        /// <exception cref="System.ArgumentNullException">參數 actorSId 不允許為 null 值。</exception>
        /// <exception cref="System.ArgumentNullException">參數 erpInvSId 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        public Returner Add(ISystemId actorSId, ISystemId erpInvSId, bool enabled, long inventoryItemId, string item, string description, float? unitWeight, string weightUomCode, float? discount, string segment1, string segment2, string segment3, long organizationId, string organizationCode, bool enabledFlag, DateTime lastUpdateDate, string XSDItem)
        {
            if (erpInvSId == null) { throw new ArgumentNullException("erpInvSId"); }
            string[] exceptionSIds = this.SystemIdVerifier.CheckValid(erpInvSId);
            if (exceptionSIds.Length > 0)
            {
                throw new LibraryException(string.Format("invaild ISystemId ({0}) format.", string.Join("、", exceptionSIds)));
            }

            Returner returner = new Returner();
            try
            {
                #region 交易緩衝區
                TransColValSet transSet = new TransColValSet();
                transSet.SmartAdd("SID", erpInvSId.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("CSID", actorSId.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("MSID", actorSId.ToString(), GenericDBType.Char, false);
                transSet.SmartAdd("ENABLED", enabled ? "Y" : "N", GenericDBType.Char, false);
                transSet.SmartAdd("INVENTORY_ITEM_ID", inventoryItemId, GenericDBType.BigInt);
                transSet.SmartAdd("ITEM", item, GenericDBType.VarChar, true);
                transSet.SmartAdd("DESCRIPTION", description, GenericDBType.VarChar, true);
                transSet.SmartAdd("UNIT_WEIGHT", unitWeight, GenericDBType.Float);
                transSet.SmartAdd("WEIGHT_UOM_CODE", weightUomCode, GenericDBType.VarChar, true);
                transSet.SmartAdd("DISCOUNT", discount, GenericDBType.Float);
                transSet.SmartAdd("SEGMENT1", segment1, GenericDBType.VarChar, true);
                transSet.SmartAdd("SEGMENT2", segment2, GenericDBType.VarChar, true);
                transSet.SmartAdd("SEGMENT3", segment3, GenericDBType.VarChar, true);
                transSet.SmartAdd("ORGANIZATION_ID", organizationId, GenericDBType.BigInt);
                transSet.SmartAdd("ORGANIZATION_CODE", organizationCode, GenericDBType.VarChar, true);
                transSet.SmartAdd("ENABLED_FLAG", enabledFlag ? "Y" : "N", GenericDBType.VarChar, false);
                transSet.SmartAdd("LAST_UPDATE_DATE", lastUpdateDate, "yyyyMMddHHmmss", GenericDBType.Char);
                transSet.SmartAdd("XSD_ITEM", XSDItem, GenericDBType.VarChar, true);

                returner.ChangeInto(base.Add(transSet, true));
                #endregion

                return returner;
            }
            finally
            {
            }
        }
        #endregion

        #region Modify (僅匯入用)
        /// <summary>
        /// 依指定的參數，修改一筆 ERP 庫存。
        /// </summary>
        /// <param name="actorSId">操作人系統代號。為庫存端在操作時的操作人系統代號。</param>
        /// <param name="inventoryItemId">料號 ID。</param>
        /// <param name="enabled">資料的啟用狀態。</param>
        /// <param name="item">料號（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="description">料號摘要（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="unitWeight">料號重量（null 則直接設為 DBNull）。</param>
        /// <param name="weightUomCode">重量單位（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="discount">預設明細折數（null 則直接設為 DBNull）。</param>
        /// <param name="segment1">料號大分類（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="segment2">料號中分類（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="segment3">料號小分類（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="organizationId">料號所屬製造組織 ID。</param>
        /// <param name="organizationCode">料號所屬製造組織代碼（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="enabledFlag">料號啟用。</param>
        /// <param name="lastUpdateDate">料號最後更新日期。</param>
        /// <param name="XSDItem">XS型號。</param>
        /// <returns>EzCoding.Returner。</returns>
        /// <exception cref="System.ArgumentNullException">參數 actorSId 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        public Returner Modify(ISystemId actorSId, long inventoryItemId, bool enabled, string item, string description, float? unitWeight, string weightUomCode, float? discount, string segment1, string segment2, string segment3, long organizationId, string organizationCode, bool enabledFlag, DateTime lastUpdateDate, string XSDItem)
        {
            if (actorSId == null) { throw new ArgumentNullException("actorSId"); }
            string[] exceptionSIds = this.SystemIdVerifier.CheckValid(actorSId);
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
                transSet.SmartAdd("ITEM", item, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("DESCRIPTION", description, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("UNIT_WEIGHT", unitWeight, GenericDBType.Float, true);
                transSet.SmartAdd("WEIGHT_UOM_CODE", weightUomCode, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("DISCOUNT", discount, GenericDBType.Float, true);
                transSet.SmartAdd("SEGMENT1", segment1, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("SEGMENT2", segment2, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("SEGMENT3", segment3, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("ORGANIZATION_ID", organizationId, GenericDBType.BigInt);
                transSet.SmartAdd("ORGANIZATION_CODE", organizationCode, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("ENABLED_FLAG", enabledFlag ? "Y" : "N", GenericDBType.VarChar, false);
                transSet.SmartAdd("LAST_UPDATE_DATE", lastUpdateDate, "yyyyMMddHHmmss", GenericDBType.Char);
                transSet.SmartAdd("XSD_ITEM", XSDItem, GenericDBType.VarChar, true);

                SqlCondsSet condsMain = new SqlCondsSet();
                condsMain.Add("INVENTORY_ITEM_ID", SqlCond.EqualTo, inventoryItemId, GenericDBType.BigInt, string.Empty);

                returner.ChangeInto(base.Modify(transSet, condsMain, true));
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
        /// 依指定的參數，修改一筆 ERP 庫存。
        /// </summary>
        /// <param name="actorSId">操作人系統代號。為庫存端在操作時的操作人系統代號。</param>
        /// <param name="erpInvSId">ERP 庫存系統代號。</param>
        /// <param name="enabled">資料的啟用狀態。</param>
        /// <param name="inventoryItemId">料號 ID。</param>
        /// <param name="item">料號（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="description">料號摘要（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="unitWeight">料號重量（null 則直接設為 DBNull）。</param>
        /// <param name="weightUomCode">重量單位（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="discount">預設明細折數（null 則直接設為 DBNull）。</param>
        /// <param name="segment1">料號大分類（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="segment2">料號中分類（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="segment3">料號小分類（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="organizationId">料號所屬製造組織 ID。</param>
        /// <param name="organizationCode">料號所屬製造組織代碼（null 或 empty 則直接設為 DBNull）。</param>
        /// <param name="enabledFlag">料號啟用。</param>
        /// <param name="lastUpdateDate">料號最後更新日期。</param>
        /// <param name="XSDItem">XS型號。</param>
        /// <returns>EzCoding.Returner。</returns>
        /// <exception cref="System.ArgumentNullException">參數 actorSId 不允許為 null 值。</exception>
        /// <exception cref="System.ArgumentNullException">參數 erpInvSId 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        public Returner Modify(ISystemId actorSId, ISystemId erpInvSId, bool enabled, long inventoryItemId, string item, string description, float? unitWeight, string weightUomCode, float? discount, string segment1, string segment2, string segment3, long organizationId, string organizationCode, bool enabledFlag, DateTime lastUpdateDate, string XSDItem)
        {
            if (actorSId == null) { throw new ArgumentNullException("actorSId"); }
            if (erpInvSId == null) { throw new ArgumentNullException("erpInvSId"); }
            string[] exceptionSIds = this.SystemIdVerifier.CheckValid(actorSId, erpInvSId);
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
                transSet.SmartAdd("INVENTORY_ITEM_ID", inventoryItemId, GenericDBType.BigInt);
                transSet.SmartAdd("ITEM", item, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("DESCRIPTION", description, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("UNIT_WEIGHT", unitWeight, GenericDBType.Float, true);
                transSet.SmartAdd("WEIGHT_UOM_CODE", weightUomCode, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("DISCOUNT", discount, GenericDBType.Float, true);
                transSet.SmartAdd("SEGMENT1", segment1, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("SEGMENT2", segment2, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("SEGMENT3", segment3, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("ORGANIZATION_ID", organizationId, GenericDBType.BigInt);
                transSet.SmartAdd("ORGANIZATION_CODE", organizationCode, GenericDBType.VarChar, true, true);
                transSet.SmartAdd("ENABLED_FLAG", enabledFlag ? "Y" : "N", GenericDBType.VarChar, false);
                transSet.SmartAdd("LAST_UPDATE_DATE", lastUpdateDate, "yyyyMMddHHmmss", GenericDBType.Char);
                transSet.SmartAdd("XSD_ITEM", XSDItem, GenericDBType.VarChar, true);

                SqlCondsSet condsMain = new SqlCondsSet();
                condsMain.Add(base.PrimaryKey, SqlCond.EqualTo, erpInvSId.ToString(), GenericDBType.Char, string.Empty);

                returner.ChangeInto(base.Modify(transSet, condsMain, true));
                #endregion

                return returner;
            }
            finally
            {
            }
        }
        #endregion

        #region UpdateMode
        /// <summary>
        /// 更新型號。
        /// </summary>
        /// <param name="erpInvSId">ERP 庫存系統代號。</param>
        /// <param name="model">型號（null 或 empty 則直接設為 DBNull）。</param>
        /// <returns>EzCoding.Returner。</returns>
        /// <exception cref="System.ArgumentNullException">參數 erpInvSId 不允許為 null 值。</exception>
        /// <exception cref="EzCoding.LibraryException">系統代號（ISystemId）的格式不正確。</exception>
        public Returner UpdateMode(ISystemId erpInvSId, string model)
        {
            if (erpInvSId == null) { throw new ArgumentNullException("erpInvSId"); }
            string[] exceptionSIds = this.SystemIdVerifier.CheckValid(erpInvSId);
            if (exceptionSIds.Length > 0)
            {
                throw new LibraryException(string.Format("invaild ISystemId ({0}) format.", string.Join("、", exceptionSIds)));
            }

            Returner returner = new Returner();
            try
            {
                #region 交易緩衝區
                TransColValSet transSet = new TransColValSet();
                transSet.SmartAdd("MODEL", model, GenericDBType.VarChar, true, true);

                SqlCondsSet condsMain = new SqlCondsSet();
                condsMain.Add(base.PrimaryKey, SqlCond.EqualTo, erpInvSId.ToString(), GenericDBType.Char, string.Empty);

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
            /// <param name="erpInvSIds">ERP 庫存系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="inventoryItemIds">料號 ID 陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="items">料號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
            /// <param name="segment1">料號大分類（若為 null 或 empty 則略過條件檢查）。</param>
            /// <param name="segment2">料號中分類（若為 null 或 empty 則略過條件檢查）。</param>
            /// <param name="segment3">料號小分類（若為 null 或 empty 則略過條件檢查）。</param>
            /// <param name="isModelExists">是否已建立型號（若為 null 則略過條件檢查）。</param>
            public InfoConds(ISystemId[] erpInvSIds, long[] inventoryItemIds, string[] items, string segment1, string segment2, string segment3, bool? isModelExists)
            {
                this.ErpCusterSIds = erpInvSIds;
                this.InventoryItemIds = inventoryItemIds;
                this.Items = items;
                this.Segment1 = segment1;
                this.Segment2 = segment2;
                this.Segment3 = segment3;
                this.IsModelExists = isModelExists;
            }

            /// <summary>
            /// ERP 庫存系統代號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public ISystemId[] ErpCusterSIds { get; set; }
            /// <summary>
            /// 料號 ID 陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public long[] InventoryItemIds { get; set; }
            /// <summary>
            /// 料號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。
            /// </summary>
            public string[] Items { get; set; }
            /// <summary>
            /// 料號大分類（若為 null 或 empty 則略過條件檢查）。
            /// </summary>
            public string Segment1 { get; set; }
            /// <summary>
            /// 料號中分類（若為 null 或 empty 則略過條件檢查）。
            /// </summary>
            public string Segment2 { get; set; }
            /// <summary>
            /// 料號小分類（若為 null 或 empty 則略過條件檢查）。
            /// </summary>
            public string Segment3 { get; set; }
            /// <summary>
            /// 是否已建立型號（若為 null 則略過條件檢查）。
            /// </summary>
            public bool? IsModelExists { get; set; }
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
        ///// <summary> 
        ///// 依指定的參數取得資料（複合式查詢）。 
        ///// </summary> 
        ///// <param name="qConds">查詢條件。</param> 
        ///// <param name="columnsName">欄位名稱陣列。</param>
        ///// <param name="keyword">關鍵字。</param>
        ///// <param name="isAbsolute">是否為絕對查詢。若 false 則使用 like 查詢。</param>
        ///// <param name="size">指定的前 N 筆資料（若為 Int32.MaxValue 則表示全部）。</param>
        ///// <param name="sqlOrder">SQL 排序（Order By）的組合類別（若為 null 則使用預設排序）。</param>
        ///// <param name="includeScope">資料取得所包含的範圍（是否註解刪除或啟用中）。</param>
        ///// <param name="inquireColumns">含有零或多個指定查詢時所要使用的欄位名稱 System.String 陣列。若陣列為零，則使用預設開放的欄位名稱。</param>
        ///// <returns>EzCoding.Returner。</returns> 
        //public Returner GetInfoByCompoundSearch(InfoConds qConds, string[] columnsName, string keyword, bool isAbsolute, int size, SqlOrder sqlOrder, IncludeScope includeScope, params string[] inquireColumns)
        //{
        //    Returner returner = new Returner();

        //    base.Entity.EnableCustomEntity = true;
        //    returner.ChangeInto(base.GetInfoByCompoundSearch(columnsName, keyword, EntityHelper.KEYWORD_SEPARATOR, isAbsolute, size, sqlOrder, GetInfoCondsSet(qConds), includeScope, inquireColumns));
        //    base.Entity.EnableCustomEntity = false;
        //    return returner;
        //}

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
            returner.ChangeInto(base.GetInfoByCompoundSearch(columnsName, keyword, EntityHelper.KEYWORD_SEPARATOR, sqlLikeMode, sqlLikeOperator, size, sqlOrder, GetInfoCondsSet(qConds), includeScope, inquireColumns));
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

            custEntity.Append("SELECT [ERP_INV].* ");
            custEntity.Append("FROM [ERP_INV] ");

            var sqlConds = new List<string>();

            if (qConds.ErpCusterSIds != null && qConds.ErpCusterSIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "SID", SqlOperator.EqualTo, SystemId.ToString(qConds.ErpCusterSIds), GenericDBType.Char));
            }

            if (qConds.InventoryItemIds != null && qConds.InventoryItemIds.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "INVENTORY_ITEM_ID", SqlOperator.EqualTo, qConds.InventoryItemIds.Select(q => (object)q).ToArray(), GenericDBType.BigInt));
            }

            if (qConds.Items != null && qConds.Items.Length > 0)
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "ITEM", SqlOperator.EqualTo, qConds.Items, GenericDBType.VarChar));
            }

            if (!string.IsNullOrWhiteSpace(qConds.Segment1))
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "SEGMENT1", SqlOperator.EqualTo, qConds.Segment1, GenericDBType.VarChar));
            }

            if (!string.IsNullOrWhiteSpace(qConds.Segment2))
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "SEGMENT2", SqlOperator.EqualTo, qConds.Segment2, GenericDBType.VarChar));
            }

            if (!string.IsNullOrWhiteSpace(qConds.Segment3))
            {
                sqlConds.Add(custEntity.BuildConds(string.Empty, "SEGMENT3", SqlOperator.EqualTo, qConds.Segment3, GenericDBType.VarChar));
            }

            switch (qConds.IsModelExists)
            {
                case true:
                    sqlConds.Add("[MODEL] IS NOT NULL");
                    break;
                case false:
                    sqlConds.Add("[MODEL] IS NULL");
                    break;
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
