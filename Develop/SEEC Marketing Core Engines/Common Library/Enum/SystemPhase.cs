
namespace Seec.Marketing
{
    /// <summary>
    /// 目前系統的運作階段。
    /// </summary>
    public enum SystemPhase
    {
        /// <summary>
        /// 開發階段（亦為 Debug 階段）。
        /// </summary>
        Development,
        /// <summary>
        /// 黑箱測試階段（亦為 Unit test 階段）。針對某個 component 或 method 的測試。
        /// </summary>
        BlackBoxTest,
        /// <summary>
        /// 白箱測試階段。針對內容實作的流程測試。
        /// </summary>
        WhiteBoxTest,
        /// <summary>
        /// 壓力測試階段。測試系統的效能極限。
        /// </summary>
        StressTest,
        /// <summary>
        /// 回歸測試階段。當新功能增加的同時，會不會影響舊功能的正確性測試。
        /// </summary>
        RegressionTest,
        /// <summary>
        /// 整合測試階段。兩個或多個系統以上的整合測試。
        /// </summary>
        IntegrationTest,
        /// <summary>
        /// 系統測試階段。
        /// </summary>
        SystemTest,
        /// <summary>
        /// 內部測試人員的測試階段。
        /// </summary>
        AlphaTest,
        /// <summary>
        /// 外部使用者的測試階段。
        /// </summary>
        BetaTest,
        /// <summary>
        /// 搞怪測試階段。盡可能惡搞來測試系統的穩定度（針對功能面）。
        /// </summary>
        MonkeyTest,
        /// <summary>
        /// 驗收測試。測試系統或產品是否滿足客戶的需求。
        /// </summary>
        AcceptanceTest,
        /// <summary>
        /// 產品或上線階段。系統已正式運作。
        /// </summary>
        Production
    }
}
