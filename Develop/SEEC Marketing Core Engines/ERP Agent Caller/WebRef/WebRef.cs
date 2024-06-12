using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Configuration;

namespace Seec.Marketing.NetTalk.WebService.Client
{
    #region TrustAllCertificatePolicy
    /// <summary>
    /// Implement ICertificatePolicy 在不檢查憑證的情況下，接受所有的SSL授權。
    /// </summary>
    public sealed class TrustAllCertificatePolicy : ICertificatePolicy
    {
        /// <summary>
        /// 
        /// </summary>
        public TrustAllCertificatePolicy() { }

        /// <summary>
        /// 驗證伺服器憑證。
        /// </summary>
        /// <param name="srvPoint">要使用憑證的 ServicePoint。</param>
        /// <param name="certificate">做驗證的憑證。</param>
        /// <param name="request">接收憑證的要求。</param>
        /// <param name="certificateProblem">使用憑證時所遇到的問題。</param>
        /// <returns></returns>
        public bool CheckValidationResult(ServicePoint srvPoint, X509Certificate certificate, WebRequest request, int certificateProblem)
        {
            return true;
        }
    }
    #endregion

    /// <summary>
    /// ERP Agent Web Service 參考。
    /// </summary>
    public class ErpAgentRef
    {
        #region ERP 介接相關
        /// <summary> 
        /// ERP Agent Web Service Url。 
        /// </summary> 
        static string ErpAgentWSUrl
        {
            get { return ConfigurationManager.AppSettings["ErpAgentWSUrl"]; }
        }

        /// <summary> 
        /// ERP Agent 客戶端 Private Key。 
        /// </summary> 
        static string ErpAgentPrivateKey
        {
            get { return ConfigurationManager.AppSettings["ErpAgentPrivateKey"]; }
        }
        #endregion

        ErpAgent _instance;

        /// <summary>
        /// 初始化 Seec.Marketing.NetTalk.WebService.Client.ErpAgentRef 類別的新執行個體。
        /// </summary>
        public ErpAgentRef()
        {
            //ServicePointManager.CertificatePolicy = new TrustAllCertificatePolicy();

            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);

            ErpAgent instance = new ErpAgent();
            instance.Url = ErpAgentRef.ErpAgentWSUrl;
            //instance.Discover();
            this._instance = instance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            //Always accept
            return true;
        }

        /// <summary>
        /// 取得參考實例。
        /// </summary>
        public ErpAgent Instance
        {
            get { return this._instance; }
        }

        #region GetCustomersInfo
        /// <summary>
        /// 取得 ERP 客戶資訊。
        /// </summary>
        /// <param name="afterTime">指定時間後的資料（若為 null 則表示全部）。</param> 
        public static TalkResponse GetCustomersInfo(DateTime? afterTime)
        {
            string jsonConds = CustJson.SerializeObject(new { AfterTime = afterTime });
            string hashVal = ComUtil.GetMD5HashValue(string.Format("{0}{1}", ErpAgentRef.ErpAgentPrivateKey, jsonConds));

            var instance = new ErpAgentRef();
            return CustJson.DeserializeObject<TalkResponse>(instance.Instance.GetCustomersInfo(jsonConds, hashVal));
        }
        #endregion

        #region GetWarehouseInfo
        /// <summary>
        /// 取得 ERP 倉庫資訊。
        /// </summary>
        /// <param name="mktgRanges">市場範圍陣列集合（1:內銷 2:外銷; 若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
        public static TalkResponse GetWarehouseInfo(int[] mktgRanges)
        {
            string jsonConds = CustJson.SerializeObject(new { MktgRanges = mktgRanges });
            string hashVal = ComUtil.GetMD5HashValue(string.Format("{0}{1}", ErpAgentRef.ErpAgentPrivateKey, jsonConds));

            var instance = new ErpAgentRef();
            return CustJson.DeserializeObject<TalkResponse>(instance.Instance.GetWarehouseInfo(jsonConds, hashVal));
        }
        #endregion

        #region GetInventoryInfo
        /// <summary>
        /// 取得 ERP 庫存資訊。
        /// </summary>
        /// <param name="afterTime">指定時間後的資料（若為 null 則表示全部）。</param> 
        public static TalkResponse GetInventoryInfo(DateTime? afterTime)
        {
            string jsonConds = CustJson.SerializeObject(new { AfterTime = afterTime });
            string hashVal = ComUtil.GetMD5HashValue(string.Format("{0}{1}", ErpAgentRef.ErpAgentPrivateKey, jsonConds));

            var instance = new ErpAgentRef();
            return CustJson.DeserializeObject<TalkResponse>(instance.Instance.GetInventoryInfo(jsonConds, hashVal));
        }
        #endregion

        #region GetOnHandInfo
        /// <summary>
        /// 取得 ERP 在手量資訊。
        /// </summary>
        /// <param name="inventoryItemIds">料號 ID 陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
        /// <param name="items">料號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
        /// <param name="whse">倉庫。</param>
        public static TalkResponse GetOnHandInfo(long[] inventoryItemIds, string[] items, string whse)
        {
            //switch (ErpAgentRef.SystemPhase)
            //{
            //    case SystemPhase.Development:
            //        //從 ERP 取在手量速度太慢, 拖慢開發時間.
            //        return CustJson.DeserializeObject<TalkResponse>(CustJson.SerializeObject(new TalkResponse()
            //        {
            //            Code = "0000",
            //            Desc = string.Empty,
            //            JsonObj = "[]"
            //        }));
            //    default:
            string jsonConds = CustJson.SerializeObject(new { InventoryItemIds = inventoryItemIds, Items = items, Whse = whse });
            string hashVal = ComUtil.GetMD5HashValue(string.Format("{0}{1}", ErpAgentRef.ErpAgentPrivateKey, jsonConds));

            var instance = new ErpAgentRef();
            return CustJson.DeserializeObject<TalkResponse>(instance.Instance.GetOnHandInfo(jsonConds, hashVal));
            //}
        }
        #endregion

        #region GetPriceBookInfo
        /// <summary>
        /// 取得 ERP 價目表資訊。
        /// </summary>
        /// <param name="priceListId">價目表 ID（若為 null 則表示全部）。</param> 
        /// <param name="inventoryItemIds">料號 ID 陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
        /// <param name="items">料號陣列集合（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
        public static TalkResponse GetPriceBookInfo(long? priceListId, long[] inventoryItemIds, string[] items)
        {
            string jsonConds = CustJson.SerializeObject(new { PriceListId = priceListId, InventoryItemIds = inventoryItemIds, Items = items });
            string hashVal = ComUtil.GetMD5HashValue(string.Format("{0}{1}", ErpAgentRef.ErpAgentPrivateKey, jsonConds));

            var instance = new ErpAgentRef();
            return CustJson.DeserializeObject<TalkResponse>(instance.Instance.GetPriceBookInfo(jsonConds, hashVal));
        }
        #endregion

        #region GetCurrencyBookInfo
        /// <summary>
        /// 取得 ERP 幣別表資訊。
        /// </summary>
        /// <param name="priceListId">價目表 ID（若為 null 則表示全部）。</param> 
        /// <param name="currenctCode">幣別（若為 null 或 empty 則略過條件檢查）。</param>
        public static TalkResponse GetCurrencyBookInfo(long? priceListId, string currenctCode)
        {
            string jsonConds = CustJson.SerializeObject(new { PriceListId = priceListId, CurrenctCode = currenctCode });
            string hashVal = ComUtil.GetMD5HashValue(string.Format("{0}{1}", ErpAgentRef.ErpAgentPrivateKey, jsonConds));

            var instance = new ErpAgentRef();
            return CustJson.DeserializeObject<TalkResponse>(instance.Instance.GetCurrencyBookInfo(jsonConds, hashVal));
        }
        #endregion

        #region GetDiscountsInfo
        /// <summary>
        /// 取得 ERP 折扣資訊。
        /// </summary>
        /// <param name="afterTime">指定時間後的資料（若為 null 則表示全部）。</param> 
        public static TalkResponse GetDiscountsInfo(DateTime? afterTime)
        {
            string jsonConds = CustJson.SerializeObject(new { AfterTime = afterTime });
            string hashVal = ComUtil.GetMD5HashValue(string.Format("{0}{1}", ErpAgentRef.ErpAgentPrivateKey, jsonConds));

            var instance = new ErpAgentRef();
            return CustJson.DeserializeObject<TalkResponse>(instance.Instance.GetDiscountsInfo(jsonConds, hashVal));
        }
        #endregion

        #region UploadErp
        /// <summary>
        /// 上傳 ERP。
        /// </summary>
        /// <param name="uploadInfo">上傳資訊。</param> 
        public static TalkResponse UploadErp(ErpBaseHelper.ErpUploader uploadInfo)
        {
            string jsonInput = CustJson.SerializeObject(uploadInfo);
            string hashVal = ComUtil.GetMD5HashValue(string.Format("{0}{1}", ErpAgentRef.ErpAgentPrivateKey, jsonInput));

            var instance = new ErpAgentRef();
            return CustJson.DeserializeObject<TalkResponse>(instance.Instance.UploadErp(jsonInput, hashVal));
        }
        #endregion

        #region GetErpOrderInfo
        /// <summary>
        /// 取得 ERP 訂單資訊。
        /// </summary>
        /// <param name="headerIds">ERP 訂單 ID（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
        /// <param name="originalSystemReferences">XS 營銷訂單號碼（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
        /// <param name="orderNumbers">ERP 訂單號碼（若為 null 或陣列長度等於 0 則略過條件檢查）。</param>
        public static TalkResponse GetErpOrderInfo(long[] headerIds, string[] originalSystemReferences, int[] orderNumbers)
        {
            string jsonConds = CustJson.SerializeObject(new { HeaderIds = headerIds, OriginalSystemReferences = originalSystemReferences, OrderNumbers = orderNumbers });
            string hashVal = ComUtil.GetMD5HashValue(string.Format("{0}{1}", ErpAgentRef.ErpAgentPrivateKey, jsonConds));

            var instance = new ErpAgentRef();
            return CustJson.DeserializeObject<TalkResponse>(instance.Instance.GetErpOrderInfo(jsonConds, hashVal));
        }
        #endregion
    }
}
