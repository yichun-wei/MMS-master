using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using System.IO;
using System.Transactions;

using EzCoding;
using EzCoding.Collections;
using EzCoding.Web.UI;
using EzCoding.DB;
using Seec.Marketing;
using Seec.Marketing.SystemEngines;

using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel; //xlsx

public partial class popup_dom_print_shipping : System.Web.UI.Page
{
    #region 網頁屬性
    /// <summary>
    /// 網頁的控制操作。
    /// </summary>
    WebPg WebPg { get; set; }
    /// <summary>
    /// 網頁是否已完成初始。
    /// </summary>
    bool HasInitial { get; set; }

    /// <summary>
    /// 主要資料的原始資訊。
    /// </summary>
    DomOrderHelper.InfoSet OrigInfo { get; set; }
    #endregion

    ISystemId _domOrderSId;

    #region 網頁初始的一連貫操作
    protected void Page_Init(object sender, EventArgs e)
    {
        if (!this.InitVital()) { return; }
        if (!this.InitPage()) { return; }
        if (!this.LoadIncludePage()) { return; }

        this.HasInitial = true;
    }

    #region 初始化的必要操作
    /// <summary>
    /// 初始化的必要操作。
    /// </summary>
    private bool InitVital()
    {
        this.WebPg = new WebPg(this, false, OperPosition.GeneralClient);

        this._domOrderSId = ConvertLib.ToSId(Request.QueryString["sid"]);
        if (this._domOrderSId == null)
        {
            JSBuilder.ClosePage(this);
            return false;
        }

        return true;
    }
    #endregion

    #region 初始化頁面
    /// <summary>
    /// 初始化頁面。
    /// </summary>
    private bool InitPage()
    {
        Returner returner = null;
        try
        {
            if (this.SetEditData(this._domOrderSId))
            {
                if (this.OrigInfo.Info.IsCancel)
                {
                    JSBuilder.ClosePage(this);
                    return false;
                }
            }
            else
            {
                JSBuilder.ClosePage(this);
                return false;
            }
        }
        finally
        {
            if (returner != null) { returner.Dispose(); }
        }

        return true;
    }
    #endregion

    #region 載入使用者控制項
    /// <summary>
    /// 載入使用者控制項。
    /// </summary>
    private bool LoadIncludePage()
    {
        return true;
    }
    #endregion
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        //列印
        this.Print();
    }

    #region SetEditData
    bool SetEditData(ISystemId systemId)
    {
        Returner returner = null;
        try
        {
            this.OrigInfo = DomOrderHelper.Binding(systemId);
            if (this.OrigInfo != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        finally
        {
            if (returner != null) { returner.Dispose(); }
        }
    }
    #endregion

    #region 列印出貨單
    void Print()
    {
        //網頁未完成初始時不執行
        if (!this.HasInitial) { return; }

        var fileName = string.Format("出貨清單_{0}.xlsx", this.OrigInfo.Info.OdrNo);

        string templatePath = WebUtilBox.MapPath("~/sample/dom/出貨清單.xlsx");

        using (FileStream fs = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
        {
            var workbook = new XSSFWorkbook(fs);
            var sheet = workbook.GetSheetAt(0);

            //列印設定 (註解的表示使用預設)
            //sheet.PrintSetup.Landscape = false; //頁面方向 -> true:橫向 false:直向
            //sheet.PrintSetup.Scale = 100; //縮放比例
            //sheet.PrintSetup.FitWidth = 1; //1 頁寬
            //sheet.PrintSetup.FitHeight = 1; //1 頁高
            sheet.PrintSetup.PaperSize = 9; //紙張大小 -> 9:A4
            //sheet.PrintSetup.UsePage = false; //啟始頁碼 -> false:自動
            //sheet.PrintSetup.PageStart = 1; //啟始頁碼 -> 第一頁

            IRow row;
            int curRowIdx;

            string domDistCompName = string.Empty, domDistTel = string.Empty, domDistFax = string.Empty, domDistAddr = string.Empty;
            #region 自訂欄位
            if (!string.IsNullOrEmpty(this.OrigInfo.Info.DomDistCustField))
            {
                var custFileds = CustJson.DeserializeObject<List<PubCat.CustField>>(this.OrigInfo.Info.DomDistCustField);

                domDistCompName = custFileds.Where(q => q.Name == PubCat.CustField.DomDist.CompName).DefaultIfEmpty(new PubCat.CustField()).SingleOrDefault().Value;
                domDistTel = custFileds.Where(q => q.Name == PubCat.CustField.DomDist.Tel).DefaultIfEmpty(new PubCat.CustField()).SingleOrDefault().Value;
                domDistFax = custFileds.Where(q => q.Name == PubCat.CustField.DomDist.Fax).DefaultIfEmpty(new PubCat.CustField()).SingleOrDefault().Value;
                domDistAddr = custFileds.Where(q => q.Name == PubCat.CustField.DomDist.Addr).DefaultIfEmpty(new PubCat.CustField()).SingleOrDefault().Value;
            }
            #endregion

            curRowIdx = 0;
            row = sheet.GetRow(curRowIdx);
            //字與字間要加一個空格
            row.GetCell(NpoiHelper.GetZeroBasedCellIdx("B")).SetCellValue(ConvertLib.TraditionalToSimplified(string.Join(" ", domDistCompName.ToArray()))); //表頭-內銷地區

            curRowIdx = 1;
            row = sheet.GetRow(curRowIdx);
            List<string> domDistCntInfo = new List<string>();
            if (!string.IsNullOrWhiteSpace(domDistAddr)) { domDistCntInfo.Add(ConvertLib.TraditionalToSimplified(domDistAddr)); }
            if (!string.IsNullOrWhiteSpace(domDistTel)) { domDistCntInfo.Add(domDistTel); }
            if (!string.IsNullOrWhiteSpace(domDistFax)) { domDistCntInfo.Add(domDistFax); }
            row.GetCell(NpoiHelper.GetZeroBasedCellIdx("B")).SetCellValue(string.Join("  ", domDistCntInfo.ToArray())); //表頭-內銷地區通訊欄

            curRowIdx = 3;
            //2016.4.25 瀋陽轉換會出錯，先用替換作法!Michelle
            string exChsNameA = "沈阳";
            string exChsNameB = "渖阳";
            string cellComCHS = ConvertLib.TraditionalToSimplified(this.OrigInfo.Info.CustomerName);

            row = sheet.GetRow(curRowIdx);
            //row.GetCell(NpoiHelper.GetZeroBasedCellIdx("C")).SetCellValue(ConvertLib.TraditionalToSimplified(this.OrigInfo.Info.CustomerName)); //客戶
            row.GetCell(NpoiHelper.GetZeroBasedCellIdx("C")).SetCellValue(cellComCHS.Replace(exChsNameB, exChsNameA)); //客戶

            row.GetCell(NpoiHelper.GetZeroBasedCellIdx("L")).SetCellValue(this.OrigInfo.Info.ErpOrderNumber); //編號

            curRowIdx = 4;
            string cellAddCHS = ConvertLib.TraditionalToSimplified(this.OrigInfo.Info.CustomerAddr);

            row = sheet.GetRow(curRowIdx);
            // row.GetCell(NpoiHelper.GetZeroBasedCellIdx("C")).SetCellValue(ConvertLib.TraditionalToSimplified(this.OrigInfo.Info.CustomerAddr)); //地址
            row.GetCell(NpoiHelper.GetZeroBasedCellIdx("C")).SetCellValue(cellAddCHS.Replace(exChsNameB, exChsNameA)); //地址
            row.GetCell(NpoiHelper.GetZeroBasedCellIdx("L")).SetCellValue(this.OrigInfo.Info.Cdt.ToString("yyyy/MM/dd")); //列印日期

            curRowIdx = 5;
            row = sheet.GetRow(curRowIdx);
            row.GetCell(NpoiHelper.GetZeroBasedCellIdx("C")).SetCellValue(string.Format("TEL: {0}", ComUtil.ToDispTel(this.OrigInfo.Info.CustomerAreaCode, this.OrigInfo.Info.CustomerTel))); //電話
            row.GetCell(NpoiHelper.GetZeroBasedCellIdx("G")).SetCellValue(string.Format("FAX: {0}", ComUtil.ToDispTel(this.OrigInfo.Info.CustomerAreaCode, this.OrigInfo.Info.CustomerFax))); //傳真
            row.GetCell(NpoiHelper.GetZeroBasedCellIdx("L")).SetCellValue(ConvertLib.ToStr(this.OrigInfo.Info.Edd, string.Empty, "yyyy/MM/dd")); //出貨日期

            curRowIdx = 6;
            row = sheet.GetRow(curRowIdx);
            row.GetCell(NpoiHelper.GetZeroBasedCellIdx("L")).SetCellValue(ConvertLib.TraditionalToSimplified(this.OrigInfo.Info.Whse)); //倉庫

            curRowIdx = 7;
            string cellRcpAddCHS = ConvertLib.TraditionalToSimplified(this.OrigInfo.Info.RcptAddr);

            row = sheet.GetRow(curRowIdx);
            //row.GetCell(NpoiHelper.GetZeroBasedCellIdx("C")).SetCellValue(ConvertLib.TraditionalToSimplified(string.Format("地址: {0}", this.OrigInfo.Info.RcptAddr))); //收貨人地址
            row.GetCell(NpoiHelper.GetZeroBasedCellIdx("C")).SetCellValue(ConvertLib.TraditionalToSimplified(string.Format("地址: {0}", cellRcpAddCHS.Replace(exChsNameB, exChsNameA)))); //收貨人地址

            curRowIdx = 8;
            row = sheet.GetRow(curRowIdx);
            row.GetCell(NpoiHelper.GetZeroBasedCellIdx("C")).SetCellValue(ConvertLib.TraditionalToSimplified(string.Format("收貨人: {0}", this.OrigInfo.Info.RcptName))); //收貨人
            row.GetCell(NpoiHelper.GetZeroBasedCellIdx("G")).SetCellValue(ConvertLib.TraditionalToSimplified(string.Format("聯絡電話: {0}", this.OrigInfo.Info.RcptTel))); //收貨人電話
            row.GetCell(NpoiHelper.GetZeroBasedCellIdx("L")).SetCellValue(ConvertLib.TraditionalToSimplified(DomOrderHelper.GetDomOrderProdTypeName(this.OrigInfo.Info.ProdType))); //產品別

            curRowIdx = 9;
            row = sheet.GetRow(curRowIdx);
            row.GetCell(NpoiHelper.GetZeroBasedCellIdx("C")).SetCellValue(ConvertLib.TraditionalToSimplified(this.OrigInfo.Info.Rmk)); //備註
            row.GetCell(NpoiHelper.GetZeroBasedCellIdx("L")).SetCellValue(ConvertLib.TraditionalToSimplified(this.OrigInfo.Info.FreightWayName)); //運輸運費

            //每 sheet 品項數
            int listCnt = 24;
            //總 sheet 數
            int totalSheets = ((this.OrigInfo.DetInfos.Count / listCnt) + ((this.OrigInfo.DetInfos.Count % (listCnt + 0d)) == 0 ? 0 : 1));

            //改成以料號排序
            this.OrigInfo.DetInfos = this.OrigInfo.DetInfos.OrderBy(q => q.PartNo).ToList();
            for (int pageNo = 1; pageNo <= totalSheets; pageNo++)
            {
                int beginIdx = pageNo * listCnt - listCnt;
                var detInfos = this.OrigInfo.DetInfos.Select((q, idx) => new { Index = idx, Item = q }).Where(q => q.Index >= beginIdx && q.Index < beginIdx + listCnt).Select(q => q.Item).ToArray();
                this.CreateShippingSheet(sheet.CopySheet(string.Format("Page {0}", pageNo), true), detInfos, beginIdx, pageNo == totalSheets);
            }

            //刪除樣板 sheet
            workbook.RemoveAt(0);

            if (false)
            {
                #region 品項清單
                //品項清單起始列索引
                curRowIdx = 14;

                //所有表頭折扣總和
                float totalHeaderDct = this.OrigInfo.HeaderDiscountInfos.Sum(q => q.Discount);

                for (int i = 0; i < this.OrigInfo.DetInfos.Count; i++)
                {
                    var detInfo = this.OrigInfo.DetInfos[i];

                    row = sheet.GetRow(curRowIdx++);

                    var curCellIdx = 0;
                    row.GetCell(++curCellIdx).SetCellValue(i + 1); //序號
                    row.GetCell(++curCellIdx).SetCellValue(detInfo.PartNo); //產品代號
                    //只是不想讓儲存格出現文字是否轉數值的提示
                    if (detInfo.UnitWeight.HasValue)
                    {
                        row.GetCell(curCellIdx += 2).SetCellValue(detInfo.UnitWeight.Value * detInfo.Qty); //重量
                    }
                    else
                    {
                        curCellIdx += 2; //重量
                    }
                    //20171002 12842 Vicky 型號欄位內容改為 摘要 → XS型號(摘要)
                    row.GetCell(++curCellIdx).SetCellValue(detInfo.XSDItem+"("+detInfo.Summary+")"); //型號
                    row.GetCell(curCellIdx += 4).SetCellValue(detInfo.Qty); //數量

                    //單價要被扣掉的價錢
                    float unitPriceDownPrice = (float)MathLib.Round(detInfo.UnitPrice * totalHeaderDct, 4);
                    //折扣後的單價
                    float unitPriceDct = detInfo.UnitPrice - unitPriceDownPrice;
                    //row.GetCell(++curCellIdx).SetCellValue(detInfo.UnitPrice); //單價
                    row.GetCell(++curCellIdx).SetCellValue(unitPriceDct * (1 + DomOrderHelper.RmbTax)); //含稅單價

                    //小計要被扣掉的價錢
                    float subtotalDownPrice = (float)MathLib.Round(detInfo.PaidAmt * totalHeaderDct, 2);
                    //折扣後的小計
                    float subtotalDct = detInfo.PaidAmt - subtotalDownPrice;
                    //金額 (含稅)
                    //row.GetCell(++curCellIdx).SetCellValue(subtotalDct); //單價 - 折扣後的小計
                    //row.GetCell(++curCellIdx).SetCellValue(detInfo.PaidAmt * (1 + DomOrderHelper.RmbTax)); //含稅金額
                    row.GetCell(++curCellIdx).SetCellValue(subtotalDct * (1 + DomOrderHelper.RmbTax)); //含稅金額
                }

                //合計
                curRowIdx = 38;
                row = sheet.GetRow(curRowIdx);
                var totalUnitWeight = this.OrigInfo.DetInfos.Sum(q => q.UnitWeight * q.Qty);
                if (totalUnitWeight.HasValue)
                {
                    row.GetCell(4).SetCellValue(totalUnitWeight.Value); //重量合計
                }
                //row.GetCell(9).SetCellValue(this.OrigInfo.DetInfos.Sum(q => q.Qty)); //數量合計
                //row.GetCell(10).SetCellValue(this.OrigInfo.DetInfos.Sum(q => q.UnitPrice)); //數量合計

                //不要直接加總品項小計 * 稅額, 直接取訂單的「折扣後總金額」..
                //row.GetCell(11).SetCellValue(this.OrigInfo.DetInfos.Sum(q => q.PaidAmt) * (1 + DomOrderHelper.RmbTax)); //含稅金額合計
                row.GetCell(11).SetCellValue(ConvertLib.ToStr(this.OrigInfo.Info.DctTotalAmt, string.Empty)); //折扣後總金額
                #endregion
            }

            #region 輸出
            MemoryStream memory = new MemoryStream();
            try
            {
                workbook.Write(memory);

                Response.Clear();
                Response.ContentType = "application/download";

                bool isIE = false;
                if (Request.Browser.Browser == "IE")
                {
                    isIE = true;
                }
                else if (Request.Browser.Browser.ToUpper() == "MOZILLA")
                {
                    // IE 11.0 User Agent String: Mozilla/5.0 (Windows NT 6.3; Trident/7.0; rv:11.0) like Gecko
                    if (Request.UserAgent.Contains("rv:11.0"))
                    {
                        isIE = true;
                    }
                }

                if (isIE)
                {
                    Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}", Server.UrlPathEncode(fileName)));
                }
                else
                {
                    Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}", fileName));
                }
                Response.BinaryWrite(memory.ToArray());
                Response.End();
            }
            finally
            {
                workbook = null;
                memory.Close();
            }
            #endregion
        }
    }

    void CreateShippingSheet(ISheet sheet, DomOrderDet.InfoView[] detInfos, int beginIdx, bool lastPage)
    {
        IRow row;
        int curRowIdx;

        #region 品項清單
        //品項清單起始列索引
        curRowIdx = 14;

        //所有表頭折扣總和
        float totalHeaderDct = this.OrigInfo.HeaderDiscountInfos.Sum(q => q.Discount);

        for (int i = 0; i < detInfos.Length; i++)
        {
            var detInfo = detInfos[i];

            row = sheet.GetRow(curRowIdx++);

            var curCellIdx = 0;
            row.GetCell(++curCellIdx).SetCellValue(++beginIdx); //序號
            row.GetCell(++curCellIdx).SetCellValue(detInfo.PartNo); //產品代號
            //只是不想讓儲存格出現文字是否轉數值的提示
            if (detInfo.UnitWeight.HasValue)
            {
                row.GetCell(curCellIdx += 2).SetCellValue(detInfo.UnitWeight.Value * detInfo.Qty); //重量
            }
            else
            {
                curCellIdx += 2; //重量
            }
            //20171002 12842 Vicky 型號欄位內容改為 摘要 → XS型號(摘要)
            row.GetCell(++curCellIdx).SetCellValue(detInfo.XSDItem + "(" + detInfo.Summary + ")"); //型號
            row.GetCell(curCellIdx += 4).SetCellValue(detInfo.Qty); //數量

            ////單價要被扣掉的價錢
            //float unitPriceDownPrice = (float)MathLib.Round(detInfo.UnitPrice * totalHeaderDct, 4);
            ////折扣後的單價
            //float unitPriceDct = detInfo.UnitPrice - unitPriceDownPrice;
            ////row.GetCell(++curCellIdx).SetCellValue(detInfo.UnitPrice); //單價
            //row.GetCell(++curCellIdx).SetCellValue(unitPriceDct * (1 + DomOrderHelper.RmbTax)); //含稅單價

            //不需要金額相關
            ////小計要被扣掉的價錢
            //float subtotalDownPrice = (float)MathLib.Round(detInfo.PaidAmt * totalHeaderDct, 2);
            ////折扣後的小計
            //float subtotalDct = detInfo.PaidAmt - subtotalDownPrice;
            ////金額 (含稅)
            ////row.GetCell(++curCellIdx).SetCellValue(subtotalDct); //單價 - 折扣後的小計
            ////row.GetCell(++curCellIdx).SetCellValue(detInfo.PaidAmt * (1 + DomOrderHelper.RmbTax)); //含稅金額
            //row.GetCell(++curCellIdx).SetCellValue(subtotalDct * (1 + DomOrderHelper.RmbTax)); //含稅金額
        }

        //最後一頁
        if (lastPage)
        {
            //合計
            curRowIdx = 38;
            row = sheet.GetRow(curRowIdx);
            var totalUnitWeight = this.OrigInfo.DetInfos.Sum(q => q.UnitWeight * q.Qty);
            if (totalUnitWeight.HasValue)
            {
                row.GetCell(4).SetCellValue(totalUnitWeight.Value); //重量合計
            }
            //row.GetCell(9).SetCellValue(this.OrigInfo.DetInfos.Sum(q => q.Qty)); //數量合計
            //row.GetCell(10).SetCellValue(this.OrigInfo.DetInfos.Sum(q => q.UnitPrice)); //數量合計

            //不需要金額相關
            ////不要直接加總品項小計 * 稅額, 直接取訂單的「折扣後總金額」..
            ////row.GetCell(11).SetCellValue(this.OrigInfo.DetInfos.Sum(q => q.PaidAmt) * (1 + DomOrderHelper.RmbTax)); //含稅金額合計
            //row.GetCell(11).SetCellValue(this.OrigInfo.Info.DctTotalAmt.Value); //折扣後總金額
        }
        else
        {
            //[20160224 by 米雪兒] 如果不是最後一頁，金額合計欄位放上"接下頁"字樣。
            curRowIdx = 38;
            row = sheet.GetRow(curRowIdx);
            row.GetCell(11).SetCellValue(ConvertLib.TraditionalToSimplified("接下頁")); //折扣後總金額
        }
        #endregion
    }
    #endregion
}