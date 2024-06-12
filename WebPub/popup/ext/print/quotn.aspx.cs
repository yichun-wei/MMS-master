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

using NPOI.SS.Util;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel; //xlsx

public partial class popup_ext_print_quotn : System.Web.UI.Page
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
    ExtQuotnHelper.InfoSet OrigInfo { get; set; }
    #endregion

    ISystemId _extQuotnSId;

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

        this._extQuotnSId = ConvertLib.ToSId(Request.QueryString["sid"]);
        if (this._extQuotnSId == null)
        {
            //若接收的參數為「quotnSId」, 則導到報價單調整中的列印.
            this._extQuotnSId = ConvertLib.ToSId(Request.QueryString["quotnSId"]);
            if (this._extQuotnSId != null)
            {
                Server.TransferRequest("quotn_readjust.aspx");
                return false;
            }
        }

        if (this._extQuotnSId == null)
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
            if (this.SetEditData())
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
    bool SetEditData()
    {
        Returner returner = null;
        try
        {
            this.OrigInfo = ExtQuotnHelper.Binding(this._extQuotnSId);
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

    #region 列印
    void Print()
    {
        //網頁未完成初始時不執行
        if (!this.HasInitial) { return; }

        var fileName = string.Format("報價單_{0}.xlsx", this.OrigInfo.Info.OdrNo);

        string templatePath = WebUtilBox.MapPath("~/sample/ext/報價單.xlsx");

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
            int curCellIdx;
            int firstEmptyCellIdx, lastEmptyIdx;

            double totalAmt = 0d;

            curRowIdx = 11;
            row = sheet.GetRow(curRowIdx);
            row.GetCell(NpoiHelper.GetZeroBasedCellIdx("B")).SetCellValue(ConvertLib.TraditionalToSimplified(this.OrigInfo.Info.CustomerName)); //客戶名稱
            row.GetCell(NpoiHelper.GetZeroBasedCellIdx("I")).SetCellValue(this.OrigInfo.Info.OdrNo); //報價單編號

            curRowIdx = 12;
            row = sheet.GetRow(curRowIdx);
            row.GetCell(NpoiHelper.GetZeroBasedCellIdx("I")).SetCellValue(this.OrigInfo.Info.QuotnDate.ToString("yyyy/MM/dd")); //報價單日期

            #region 品項資訊
            curRowIdx = 18;
            if (this.OrigInfo.DetInfos != null && this.OrigInfo.DetInfos.Count > 0)
            {
                #region Style
                //首欄
                ICellStyle firstCellStyle = NpoiHelper.CreateCellStyle(workbook
                    , NpoiHelper.CreateFont(workbook, 12, "Arial", DefVal.Short, DefVal.Bool, null, DefVal.Short)
                    , false
                    , VerticalAlignment.Bottom, HorizontalAlignment.Center
                    , NpoiHelper.CellBorder.Dotted, NpoiHelper.CellBorder.Dotted, NpoiHelper.CellBorder.Dotted, NpoiHelper.CellBorder.Medium
                    , DefVal.Short, DefVal.Short, DefVal.Short, DefVal.Short
                    , DefVal.Str);

                //文字欄位
                ICellStyle textCellStyle = NpoiHelper.CreateCellStyle(workbook
                    , NpoiHelper.CreateFont(workbook, 12, "Arial", DefVal.Short, DefVal.Bool, null, DefVal.Short)
                    , false
                    , VerticalAlignment.Center, HorizontalAlignment.Left
                    , NpoiHelper.CellBorder.Dotted, NpoiHelper.CellBorder.Dotted, NpoiHelper.CellBorder.Dotted, NpoiHelper.CellBorder.Dotted
                    , DefVal.Short, DefVal.Short, DefVal.Short, DefVal.Short
                    , DefVal.Str);

                //數值欄位
                ICellStyle numeralCellStyle = NpoiHelper.CreateCellStyle(workbook
                    , NpoiHelper.CreateFont(workbook, 12, "Arial", DefVal.Short, DefVal.Bool, null, DefVal.Short)
                    , false
                    , VerticalAlignment.Bottom, HorizontalAlignment.Center
                    , NpoiHelper.CellBorder.Dotted, NpoiHelper.CellBorder.Dotted, NpoiHelper.CellBorder.Dotted, NpoiHelper.CellBorder.Dotted
                    , DefVal.Short, DefVal.Short, DefVal.Short, DefVal.Short
                    , DefVal.Str);

                //末欄
                ICellStyle lastCellStyle = NpoiHelper.CreateCellStyle(workbook
                    , NpoiHelper.CreateFont(workbook, 12, "Arial", DefVal.Short, DefVal.Bool, null, DefVal.Short)
                    , false
                    , VerticalAlignment.Bottom, HorizontalAlignment.Center
                    , NpoiHelper.CellBorder.Dotted, NpoiHelper.CellBorder.Medium, NpoiHelper.CellBorder.Dotted, NpoiHelper.CellBorder.Dotted
                    , DefVal.Short, DefVal.Short, DefVal.Short, DefVal.Short
                    , DefVal.Str);
                #endregion

                for (int i = 0; i < this.OrigInfo.DetInfos.Count; i++, curRowIdx++)
                {
                    var detInfo = this.OrigInfo.DetInfos[i];

                    curCellIdx = 0;
                    row = sheet.CreateRow(curRowIdx);

                    //No.
                    row.CreateCell(curCellIdx).SetCellValue(i + 1);
                    row.GetCell(curCellIdx).CellStyle = firstCellStyle;
                    curCellIdx++;

                    //Description
                    row.CreateCell(curCellIdx).SetCellValue(detInfo.Model);
                    firstEmptyCellIdx = NpoiHelper.GetZeroBasedCellIdx("B");
                    lastEmptyIdx = NpoiHelper.GetZeroBasedCellIdx("E");
                    sheet.AddMergedRegion(new CellRangeAddress(curRowIdx, curRowIdx, firstEmptyCellIdx, lastEmptyIdx));
                    row.GetCell(curCellIdx).CellStyle = textCellStyle;
                    curCellIdx++;
                    //因合併儲存格, 補上被合併的樣式.
                    for (; firstEmptyCellIdx < lastEmptyIdx; firstEmptyCellIdx++)
                    {
                        row.CreateCell(curCellIdx).CellStyle = textCellStyle;
                        curCellIdx++;
                    }

                    //Quantity
                    row.CreateCell(curCellIdx).SetCellValue(ConvertLib.ToAccounting(detInfo.Qty));
                    row.GetCell(curCellIdx).CellStyle = numeralCellStyle;
                    curCellIdx++;

                    //Unit Price
                    row.CreateCell(curCellIdx).SetCellValue(ConvertLib.ToAccounting(MathLib.Round(detInfo.UnitPrice, 2)));
                    firstEmptyCellIdx = NpoiHelper.GetZeroBasedCellIdx("G");
                    lastEmptyIdx = NpoiHelper.GetZeroBasedCellIdx("I");
                    sheet.AddMergedRegion(new CellRangeAddress(curRowIdx, curRowIdx, firstEmptyCellIdx, lastEmptyIdx));
                    row.GetCell(curCellIdx).CellStyle = numeralCellStyle;
                    curCellIdx++;
                    //因合併儲存格, 補上被合併的樣式.
                    for (; firstEmptyCellIdx < lastEmptyIdx; firstEmptyCellIdx++)
                    {
                        row.CreateCell(curCellIdx).CellStyle = textCellStyle;
                        curCellIdx++;
                    }

                    //Amount
                    float subtotalAmt = (float)MathLib.Round(detInfo.PaidAmt, 2);
                    totalAmt += subtotalAmt;
                    row.CreateCell(curCellIdx).SetCellValue(ConvertLib.ToAccounting(subtotalAmt));
                    row.GetCell(curCellIdx).CellStyle = lastCellStyle;
                    curCellIdx++;
                }
            }
            #endregion

            #region 一列空行
            if (true)
            {
                #region Style
                //首欄
                ICellStyle firstCellStyle = NpoiHelper.CreateCellStyle(workbook
                    , NpoiHelper.CreateFont(workbook, 12, "Arial", DefVal.Short, DefVal.Bool, null, DefVal.Short)
                    , false
                    , VerticalAlignment.Bottom, HorizontalAlignment.Center
                    , null, NpoiHelper.CellBorder.None, NpoiHelper.CellBorder.Medium, NpoiHelper.CellBorder.Medium
                    , DefVal.Short, DefVal.Short, DefVal.Short, DefVal.Short
                    , DefVal.Str);

                //中間欄位
                ICellStyle middleCellStyle = NpoiHelper.CreateCellStyle(workbook
                    , NpoiHelper.CreateFont(workbook, 12, "Arial", DefVal.Short, DefVal.Bool, null, DefVal.Short)
                    , false
                    , VerticalAlignment.Center, HorizontalAlignment.Left
                    , null, NpoiHelper.CellBorder.None, NpoiHelper.CellBorder.Medium, NpoiHelper.CellBorder.None
                    , DefVal.Short, DefVal.Short, DefVal.Short, DefVal.Short
                    , DefVal.Str);

                //末欄
                ICellStyle lastCellStyle = NpoiHelper.CreateCellStyle(workbook
                    , NpoiHelper.CreateFont(workbook, 12, "Arial", DefVal.Short, DefVal.Bool, null, DefVal.Short)
                    , false
                    , VerticalAlignment.Bottom, HorizontalAlignment.Center
                    , null, NpoiHelper.CellBorder.Medium, NpoiHelper.CellBorder.Medium, NpoiHelper.CellBorder.None
                    , DefVal.Short, DefVal.Short, DefVal.Short, DefVal.Short
                    , DefVal.Str);
                #endregion

                firstEmptyCellIdx = NpoiHelper.GetZeroBasedCellIdx("A");
                lastEmptyIdx = NpoiHelper.GetZeroBasedCellIdx("J");
                curCellIdx = firstEmptyCellIdx;
                row = sheet.CreateRow(curRowIdx);
                NpoiHelper.FillEmptyCell(row, curCellIdx, firstEmptyCellIdx, lastEmptyIdx, firstCellStyle, middleCellStyle, lastCellStyle);
                curRowIdx++;
            }
            #endregion

            #region Notes
            if (true)
            {
                #region Style
                //首欄
                ICellStyle firstCellStyle = NpoiHelper.CreateCellStyle(workbook
                    , NpoiHelper.CreateFont(workbook, 9, "Arial", DefVal.Short, DefVal.Bool, null, DefVal.Short)
                    , true
                    , VerticalAlignment.Top, HorizontalAlignment.Left
                    , null, NpoiHelper.CellBorder.None, NpoiHelper.CellBorder.None, NpoiHelper.CellBorder.Medium
                    , DefVal.Short, DefVal.Short, DefVal.Short, DefVal.Short
                    , DefVal.Str);

                //中間欄位
                ICellStyle middleCellStyle = NpoiHelper.CreateCellStyle(workbook
                    , NpoiHelper.CreateFont(workbook, 9, "Arial", DefVal.Short, DefVal.Bool, null, DefVal.Short)
                    , true
                    , VerticalAlignment.Top, HorizontalAlignment.Left
                    , null, NpoiHelper.CellBorder.None, NpoiHelper.CellBorder.None, NpoiHelper.CellBorder.None
                    , DefVal.Short, DefVal.Short, DefVal.Short, DefVal.Short
                    , DefVal.Str);

                //末欄
                ICellStyle lastCellStyle = NpoiHelper.CreateCellStyle(workbook
                    , NpoiHelper.CreateFont(workbook, 9, "Arial", DefVal.Short, DefVal.Bool, null, DefVal.Short)
                    , true
                    , VerticalAlignment.Top, HorizontalAlignment.Left
                    , null, NpoiHelper.CellBorder.Medium, NpoiHelper.CellBorder.None, NpoiHelper.CellBorder.None
                    , DefVal.Short, DefVal.Short, DefVal.Short, DefVal.Short
                    , DefVal.Str);
                #endregion

                List<string> textList;

                firstEmptyCellIdx = NpoiHelper.GetZeroBasedCellIdx("A");
                lastEmptyIdx = NpoiHelper.GetZeroBasedCellIdx("J");

                curCellIdx = firstEmptyCellIdx;
                row = sheet.CreateRow(curRowIdx);
                row.CreateCell(curCellIdx).SetCellValue("Notes:");
                sheet.AddMergedRegion(new CellRangeAddress(curRowIdx, curRowIdx, firstEmptyCellIdx, lastEmptyIdx));
                row.GetCell(curCellIdx).CellStyle = firstCellStyle;
                //因合併儲存格, 補上被合併的樣式.
                NpoiHelper.FillEmptyCell(row, ++curCellIdx, firstEmptyCellIdx, lastEmptyIdx, firstCellStyle, middleCellStyle, lastCellStyle);
                curRowIdx++;

                curCellIdx = firstEmptyCellIdx;
                row = sheet.CreateRow(curRowIdx);
                row.Height = (short)(row.Height * 2);
                row.CreateCell(curCellIdx).SetCellValue("1. Manufacturer's inspection certificate is to be final, should any inspection by 3rd party surveyor be required, such fee should be paid by buyer's account.");
                sheet.AddMergedRegion(new CellRangeAddress(curRowIdx, curRowIdx, firstEmptyCellIdx, lastEmptyIdx));
                row.GetCell(curCellIdx).CellStyle = firstCellStyle;
                //因合併儲存格, 補上被合併的樣式.
                NpoiHelper.FillEmptyCell(row, ++curCellIdx, firstEmptyCellIdx, lastEmptyIdx, firstCellStyle, middleCellStyle, lastCellStyle);
                curRowIdx++;

                textList = new List<string>();
                textList.Add("2. The price quoted above is net without any commission.");
                textList.Add("3. The price quoted above is exclusive of installation and commissioning fees.");
                textList.Add("4. International Bank Change Form:");
                textList.Add("● Company Name:  XIAMEN SHIHLIN ELECTRIC&ENGINEERING CO.,LTD");
                textList.Add("●  Bank Name:  THE AGRICULTURAL BANK OF CHINA, XIAMEN BRANCH");
                textList.Add("●  Bank Address:  NO. 1, XING XI ROAD, XINGLIN AREA, XIAMEN, CHINA 361022");
                textList.Add("●  Bank Swift Number: ABOCCNBJ400");
                textList.Add("●  Account Number:  40325014040001173");
                textList.Add("5. Warranty: 12 months after the date of SEEC shipment");
                textList.Add("6. Document requirement:");
                textList.Add("    Merchanting trade: □YES, □NO");
                textList.Add("    B/L: □ Telex Release,  □ Original");
                textList.Add("    Delivery address:");
                textList.Add("7. Way of Delivery: □Courier, □Air, □SEA");
                textList.Add("8. Transit fee: □Buyer, □Manufactory, □Others");

                foreach (var text in textList)
                {
                    curCellIdx = firstEmptyCellIdx;
                    row = sheet.CreateRow(curRowIdx);
                    row.CreateCell(curCellIdx).SetCellValue(text);
                    sheet.AddMergedRegion(new CellRangeAddress(curRowIdx, curRowIdx, firstEmptyCellIdx, lastEmptyIdx));
                    row.GetCell(curCellIdx).CellStyle = firstCellStyle;
                    //因合併儲存格, 補上被合併的樣式.
                    NpoiHelper.FillEmptyCell(row, ++curCellIdx, firstEmptyCellIdx, lastEmptyIdx, firstCellStyle, middleCellStyle, lastCellStyle);
                    curRowIdx++;
                }
            }
            #endregion

            #region Notes (末列)
            if (true)
            {
                #region Style
                //首欄
                ICellStyle firstCellStyle = NpoiHelper.CreateCellStyle(workbook
                    , NpoiHelper.CreateFont(workbook, 9, "Arial", DefVal.Short, DefVal.Bool, null, DefVal.Short)
                    , true
                    , VerticalAlignment.Top, HorizontalAlignment.Left
                    , null, NpoiHelper.CellBorder.None, NpoiHelper.CellBorder.Medium, NpoiHelper.CellBorder.Medium
                    , DefVal.Short, DefVal.Short, DefVal.Short, DefVal.Short
                    , DefVal.Str);

                //中間欄位
                ICellStyle middleCellStyle = NpoiHelper.CreateCellStyle(workbook
                    , NpoiHelper.CreateFont(workbook, 9, "Arial", DefVal.Short, DefVal.Bool, null, DefVal.Short)
                    , true
                    , VerticalAlignment.Top, HorizontalAlignment.Left
                    , null, NpoiHelper.CellBorder.None, NpoiHelper.CellBorder.Medium, NpoiHelper.CellBorder.None
                    , DefVal.Short, DefVal.Short, DefVal.Short, DefVal.Short
                    , DefVal.Str);

                //末欄
                ICellStyle lastCellStyle = NpoiHelper.CreateCellStyle(workbook
                    , NpoiHelper.CreateFont(workbook, 9, "Arial", DefVal.Short, DefVal.Bool, null, DefVal.Short)
                    , true
                    , VerticalAlignment.Top, HorizontalAlignment.Left
                    , null, NpoiHelper.CellBorder.Medium, NpoiHelper.CellBorder.Medium, NpoiHelper.CellBorder.None
                    , DefVal.Short, DefVal.Short, DefVal.Short, DefVal.Short
                    , DefVal.Str);
                #endregion

                firstEmptyCellIdx = NpoiHelper.GetZeroBasedCellIdx("A");
                lastEmptyIdx = NpoiHelper.GetZeroBasedCellIdx("J");

                curCellIdx = firstEmptyCellIdx;
                row = sheet.CreateRow(curRowIdx);
                row.CreateCell(curCellIdx).SetCellValue("9. Need to charge handling cost USD 150, if the minimum order less than USD 5,000.00 /per time.");
                sheet.AddMergedRegion(new CellRangeAddress(curRowIdx, curRowIdx, firstEmptyCellIdx, lastEmptyIdx));
                row.GetCell(curCellIdx).CellStyle = firstCellStyle;
                //因合併儲存格, 補上被合併的樣式.
                NpoiHelper.FillEmptyCell(row, ++curCellIdx, firstEmptyCellIdx, lastEmptyIdx, firstCellStyle, middleCellStyle, lastCellStyle);
                curRowIdx++;
            }
            #endregion

            #region 一列空行
            if (true)
            {
                #region Style
                //首欄
                ICellStyle firstCellStyle = NpoiHelper.CreateCellStyle(workbook
                    , NpoiHelper.CreateFont(workbook, 20, "Arial", DefVal.Short, DefVal.Bool, null, DefVal.Short)
                    , false
                    , VerticalAlignment.Bottom, HorizontalAlignment.Center
                    , null, NpoiHelper.CellBorder.None, NpoiHelper.CellBorder.None, NpoiHelper.CellBorder.Medium
                    , DefVal.Short, DefVal.Short, DefVal.Short, DefVal.Short
                    , DefVal.Str);

                //中間欄位
                ICellStyle middleCellStyle = NpoiHelper.CreateCellStyle(workbook
                    , NpoiHelper.CreateFont(workbook, 20, "Arial", DefVal.Short, DefVal.Bool, null, DefVal.Short)
                    , false
                    , VerticalAlignment.Center, HorizontalAlignment.Left
                    , null, NpoiHelper.CellBorder.None, NpoiHelper.CellBorder.None, NpoiHelper.CellBorder.None
                    , DefVal.Short, DefVal.Short, DefVal.Short, DefVal.Short
                    , DefVal.Str);

                //末欄
                ICellStyle lastCellStyle = NpoiHelper.CreateCellStyle(workbook
                    , NpoiHelper.CreateFont(workbook, 20, "Arial", DefVal.Short, DefVal.Bool, null, DefVal.Short)
                    , false
                    , VerticalAlignment.Bottom, HorizontalAlignment.Center
                    , null, NpoiHelper.CellBorder.Medium, NpoiHelper.CellBorder.None, NpoiHelper.CellBorder.None
                    , DefVal.Short, DefVal.Short, DefVal.Short, DefVal.Short
                    , DefVal.Str);
                #endregion

                firstEmptyCellIdx = NpoiHelper.GetZeroBasedCellIdx("A");
                lastEmptyIdx = NpoiHelper.GetZeroBasedCellIdx("J");
                curCellIdx = firstEmptyCellIdx;
                row = sheet.CreateRow(curRowIdx);
                row.Height = 26 * 20;
                NpoiHelper.FillEmptyCell(row, curCellIdx, firstEmptyCellIdx, lastEmptyIdx, firstCellStyle, middleCellStyle, lastCellStyle);
                curRowIdx++;
            }
            #endregion

            #region 總金額
            if (true)
            {
                #region Style
                //首欄
                ICellStyle firstCellStyle = NpoiHelper.CreateCellStyle(workbook
                    , NpoiHelper.CreateFont(workbook, 12, "Arial", DefVal.Short, DefVal.Bool, null, DefVal.Short)
                    , false
                    , VerticalAlignment.Bottom, HorizontalAlignment.Center
                    , null, NpoiHelper.CellBorder.None, NpoiHelper.CellBorder.Medium, NpoiHelper.CellBorder.Medium
                    , DefVal.Short, DefVal.Short, DefVal.Short, DefVal.Short
                    , DefVal.Str);

                //中間欄位
                ICellStyle middleCellStyle = NpoiHelper.CreateCellStyle(workbook
                    , NpoiHelper.CreateFont(workbook, 12, "Arial", DefVal.Short, DefVal.Bool, null, DefVal.Short)
                    , false
                    , VerticalAlignment.Center, HorizontalAlignment.Left
                    , null, NpoiHelper.CellBorder.None, NpoiHelper.CellBorder.Medium, NpoiHelper.CellBorder.None
                    , DefVal.Short, DefVal.Short, DefVal.Short, DefVal.Short
                    , DefVal.Str);

                //文字欄位
                ICellStyle textStyle = NpoiHelper.CreateCellStyle(workbook
                    , NpoiHelper.CreateFont(workbook, 12, "Arial", DefVal.Short, DefVal.Bool, null, DefVal.Short)
                    , false
                    , VerticalAlignment.Center, HorizontalAlignment.Left
                    , null, NpoiHelper.CellBorder.None, NpoiHelper.CellBorder.Medium, NpoiHelper.CellBorder.None
                    , DefVal.Short, DefVal.Short, DefVal.Short, DefVal.Short
                    , DefVal.Str);

                //數值欄位
                ICellStyle numeralStyle = NpoiHelper.CreateCellStyle(workbook
                    , NpoiHelper.CreateFont(workbook, 12, "Arial", DefVal.Short, DefVal.Bool, null, DefVal.Short)
                    , false
                    , VerticalAlignment.Center, null
                    , null, NpoiHelper.CellBorder.None, NpoiHelper.CellBorder.Medium, NpoiHelper.CellBorder.None
                    , DefVal.Short, DefVal.Short, DefVal.Short, DefVal.Short
                    , DefVal.Str);

                //末欄
                ICellStyle lastCellStyle = NpoiHelper.CreateCellStyle(workbook
                    , NpoiHelper.CreateFont(workbook, 12, "Arial", DefVal.Short, DefVal.Bool, null, DefVal.Short)
                    , false
                    , VerticalAlignment.Center, HorizontalAlignment.Center
                    , null, NpoiHelper.CellBorder.Medium, NpoiHelper.CellBorder.Medium, NpoiHelper.CellBorder.None
                    , DefVal.Short, DefVal.Short, DefVal.Short, DefVal.Short
                    , DefVal.Str);
                #endregion

                firstEmptyCellIdx = NpoiHelper.GetZeroBasedCellIdx("A");
                lastEmptyIdx = NpoiHelper.GetZeroBasedCellIdx("E");
                curCellIdx = firstEmptyCellIdx;
                row = sheet.CreateRow(curRowIdx);
                row.Height = 19 * 20;
                NpoiHelper.FillEmptyCell(row, curCellIdx, firstEmptyCellIdx, lastEmptyIdx, firstCellStyle, middleCellStyle, middleCellStyle);
                curCellIdx = lastEmptyIdx + 1;

                row.CreateCell(curCellIdx).SetCellValue("  TOTAL  CIF   ......................");
                row.Height = 19 * 20;
                row.GetCell(curCellIdx).CellStyle = textStyle;
                firstEmptyCellIdx = NpoiHelper.GetZeroBasedCellIdx("F");
                lastEmptyIdx = NpoiHelper.GetZeroBasedCellIdx("H");
                sheet.AddMergedRegion(new CellRangeAddress(curRowIdx, curRowIdx, firstEmptyCellIdx, lastEmptyIdx));
                curCellIdx++;
                //因合併儲存格, 補上被合併的樣式.
                for (; firstEmptyCellIdx < lastEmptyIdx; firstEmptyCellIdx++)
                {
                    row.CreateCell(curCellIdx).CellStyle = numeralStyle;
                    curCellIdx++;
                }

                row.CreateCell(curCellIdx).SetCellValue(this.OrigInfo.Info.CurrencyCode);
                row.Height = 19 * 20;
                row.GetCell(curCellIdx).CellStyle = textStyle;
                curCellIdx++;

                row.CreateCell(curCellIdx).SetCellValue(totalAmt);
                row.CreateCell(curCellIdx).SetCellValue(ConvertLib.ToAccounting(totalAmt, 2, true));
                row.Height = 19 * 20;
                row.GetCell(curCellIdx).CellStyle = lastCellStyle;
                curCellIdx++;

                curRowIdx++;
            }
            #endregion

            #region 一列空行
            if (true)
            {
                #region Style
                //首欄
                ICellStyle firstCellStyle = NpoiHelper.CreateCellStyle(workbook
                    , NpoiHelper.CreateFont(workbook, 12, "Arial", DefVal.Short, DefVal.Bool, null, DefVal.Short)
                    , false
                    , VerticalAlignment.Bottom, HorizontalAlignment.Center
                    , null, NpoiHelper.CellBorder.None, NpoiHelper.CellBorder.None, NpoiHelper.CellBorder.None
                    , DefVal.Short, DefVal.Short, DefVal.Short, DefVal.Short
                    , DefVal.Str);

                //中間欄位
                ICellStyle middleCellStyle = NpoiHelper.CreateCellStyle(workbook
                    , NpoiHelper.CreateFont(workbook, 12, "Arial", DefVal.Short, DefVal.Bool, null, DefVal.Short)
                    , false
                    , VerticalAlignment.Center, HorizontalAlignment.Left
                    , null, NpoiHelper.CellBorder.None, NpoiHelper.CellBorder.None, NpoiHelper.CellBorder.None
                    , DefVal.Short, DefVal.Short, DefVal.Short, DefVal.Short
                    , DefVal.Str);

                //末欄
                ICellStyle lastCellStyle = NpoiHelper.CreateCellStyle(workbook
                    , NpoiHelper.CreateFont(workbook, 12, "Arial", DefVal.Short, DefVal.Bool, null, DefVal.Short)
                    , false
                    , VerticalAlignment.Center, HorizontalAlignment.Left
                    , null, NpoiHelper.CellBorder.None, NpoiHelper.CellBorder.None, NpoiHelper.CellBorder.None
                    , DefVal.Short, DefVal.Short, DefVal.Short, DefVal.Short
                    , DefVal.Str);
                #endregion

                firstEmptyCellIdx = NpoiHelper.GetZeroBasedCellIdx("A");
                lastEmptyIdx = NpoiHelper.GetZeroBasedCellIdx("J");
                curCellIdx = firstEmptyCellIdx;
                row = sheet.CreateRow(curRowIdx);
                NpoiHelper.FillEmptyCell(row, curCellIdx, firstEmptyCellIdx, lastEmptyIdx, firstCellStyle, middleCellStyle, lastCellStyle);
                curRowIdx++;
            }
            #endregion

            #region 甲乙雙方名稱
            if (true)
            {
                #region Style
                //首欄
                ICellStyle firstCellStyle = NpoiHelper.CreateCellStyle(workbook
                    , NpoiHelper.CreateFont(workbook, 12, "Arial", DefVal.Short, DefVal.Bool, null, DefVal.Short)
                    , false
                    , VerticalAlignment.Bottom, HorizontalAlignment.Left
                    , null, NpoiHelper.CellBorder.None, NpoiHelper.CellBorder.None, NpoiHelper.CellBorder.None
                    , DefVal.Short, DefVal.Short, DefVal.Short, DefVal.Short
                    , DefVal.Str);

                //中間欄位
                ICellStyle middleCellStyle = NpoiHelper.CreateCellStyle(workbook
                    , NpoiHelper.CreateFont(workbook, 12, "Arial", DefVal.Short, DefVal.Bool, null, DefVal.Short)
                    , false
                    , VerticalAlignment.Bottom, HorizontalAlignment.Left
                    , null, NpoiHelper.CellBorder.None, NpoiHelper.CellBorder.None, NpoiHelper.CellBorder.None
                    , DefVal.Short, DefVal.Short, DefVal.Short, DefVal.Short
                    , DefVal.Str);

                //文字欄位
                ICellStyle textStyle = NpoiHelper.CreateCellStyle(workbook
                    , NpoiHelper.CreateFont(workbook, 12, "Arial", DefVal.Short, DefVal.Bool, null, DefVal.Short)
                    , true
                    , VerticalAlignment.Top, HorizontalAlignment.Left
                    , null, NpoiHelper.CellBorder.None, NpoiHelper.CellBorder.None, NpoiHelper.CellBorder.None
                    , DefVal.Short, DefVal.Short, DefVal.Short, DefVal.Short
                    , DefVal.Str);

                //末欄
                ICellStyle lastCellStyle = NpoiHelper.CreateCellStyle(workbook
                    , NpoiHelper.CreateFont(workbook, 12, "Arial", DefVal.Short, DefVal.Bool, null, DefVal.Short)
                    , true
                    , VerticalAlignment.Top, HorizontalAlignment.Left
                    , null, NpoiHelper.CellBorder.None, NpoiHelper.CellBorder.None, NpoiHelper.CellBorder.None
                    , DefVal.Short, DefVal.Short, DefVal.Short, DefVal.Short
                    , DefVal.Str);
                #endregion

                curCellIdx = 0;
                row = sheet.CreateRow(curRowIdx);
                row.Height = (short)(row.Height * 3);

                row.CreateCell(curCellIdx).SetCellValue(string.Empty);
                row.GetCell(curCellIdx).CellStyle = firstCellStyle;
                curCellIdx++;

                row.CreateCell(curCellIdx).SetCellValue("Shihlin Electric & Engineering Corp");
                firstEmptyCellIdx = NpoiHelper.GetZeroBasedCellIdx("B");
                lastEmptyIdx = NpoiHelper.GetZeroBasedCellIdx("E");
                sheet.AddMergedRegion(new CellRangeAddress(curRowIdx, curRowIdx, firstEmptyCellIdx, lastEmptyIdx));
                row.GetCell(curCellIdx).CellStyle = textStyle;
                curCellIdx++;
                //因合併儲存格, 補上被合併的樣式.
                for (; firstEmptyCellIdx < lastEmptyIdx; firstEmptyCellIdx++)
                {
                    row.CreateCell(curCellIdx).CellStyle = textStyle;
                    curCellIdx++;
                }

                row.CreateCell(curCellIdx).SetCellValue(string.Empty);
                row.GetCell(curCellIdx).CellStyle = middleCellStyle;
                curCellIdx++;

                row.CreateCell(curCellIdx).SetCellValue(this.OrigInfo.Info.CustomerName);
                firstEmptyCellIdx = NpoiHelper.GetZeroBasedCellIdx("G");
                lastEmptyIdx = NpoiHelper.GetZeroBasedCellIdx("J");
                sheet.AddMergedRegion(new CellRangeAddress(curRowIdx, curRowIdx, firstEmptyCellIdx, lastEmptyIdx));
                row.GetCell(curCellIdx).CellStyle = lastCellStyle;
                curCellIdx++;
                //因合併儲存格, 補上被合併的樣式.
                for (; firstEmptyCellIdx < lastEmptyIdx; firstEmptyCellIdx++)
                {
                    row.CreateCell(curCellIdx).CellStyle = lastCellStyle;
                    curCellIdx++;
                }

                curRowIdx++;
            }
            #endregion

            #region 兩列空行
            if (true)
            {
                #region Style
                //首欄
                ICellStyle firstCellStyle = NpoiHelper.CreateCellStyle(workbook
                    , NpoiHelper.CreateFont(workbook, 12, "Arial", DefVal.Short, DefVal.Bool, null, DefVal.Short)
                    , false
                    , VerticalAlignment.Bottom, HorizontalAlignment.Center
                    , null, NpoiHelper.CellBorder.None, NpoiHelper.CellBorder.None, NpoiHelper.CellBorder.None
                    , DefVal.Short, DefVal.Short, DefVal.Short, DefVal.Short
                    , DefVal.Str);

                //中間欄位
                ICellStyle middleCellStyle = NpoiHelper.CreateCellStyle(workbook
                    , NpoiHelper.CreateFont(workbook, 12, "Arial", DefVal.Short, DefVal.Bool, null, DefVal.Short)
                    , false
                    , VerticalAlignment.Center, HorizontalAlignment.Left
                    , null, NpoiHelper.CellBorder.None, NpoiHelper.CellBorder.None, NpoiHelper.CellBorder.None
                    , DefVal.Short, DefVal.Short, DefVal.Short, DefVal.Short
                    , DefVal.Str);

                //末欄
                ICellStyle lastCellStyle = NpoiHelper.CreateCellStyle(workbook
                    , NpoiHelper.CreateFont(workbook, 12, "Arial", DefVal.Short, DefVal.Bool, null, DefVal.Short)
                    , false
                    , VerticalAlignment.Bottom, HorizontalAlignment.Center
                    , null, NpoiHelper.CellBorder.None, NpoiHelper.CellBorder.None, NpoiHelper.CellBorder.None
                    , DefVal.Short, DefVal.Short, DefVal.Short, DefVal.Short
                    , DefVal.Str);
                #endregion

                firstEmptyCellIdx = NpoiHelper.GetZeroBasedCellIdx("A");
                lastEmptyIdx = NpoiHelper.GetZeroBasedCellIdx("J");
                curCellIdx = firstEmptyCellIdx;
                row = sheet.CreateRow(curRowIdx);
                NpoiHelper.FillEmptyCell(row, curCellIdx, firstEmptyCellIdx, lastEmptyIdx, firstCellStyle, middleCellStyle, lastCellStyle);
                curRowIdx++;

                curCellIdx = firstEmptyCellIdx;
                row = sheet.CreateRow(curRowIdx);
                NpoiHelper.FillEmptyCell(row, curCellIdx, firstEmptyCellIdx, lastEmptyIdx, firstCellStyle, middleCellStyle, lastCellStyle);
                curRowIdx++;
            }
            #endregion

            #region 甲乙雙方簽名列
            if (true)
            {
                #region Style
                //首欄
                ICellStyle firstCellStyle = NpoiHelper.CreateCellStyle(workbook
                    , NpoiHelper.CreateFont(workbook, 12, "Arial", DefVal.Short, DefVal.Bool, null, DefVal.Short)
                    , false
                    , VerticalAlignment.Bottom, HorizontalAlignment.Left
                    , null, NpoiHelper.CellBorder.None, NpoiHelper.CellBorder.None, NpoiHelper.CellBorder.None
                    , DefVal.Short, DefVal.Short, DefVal.Short, DefVal.Short
                    , DefVal.Str);

                //中間欄位
                ICellStyle middleCellStyle = NpoiHelper.CreateCellStyle(workbook
                    , NpoiHelper.CreateFont(workbook, 12, "Arial", DefVal.Short, DefVal.Bool, null, DefVal.Short)
                    , false
                    , VerticalAlignment.Bottom, HorizontalAlignment.Left
                    , null, NpoiHelper.CellBorder.None, NpoiHelper.CellBorder.None, NpoiHelper.CellBorder.None
                    , DefVal.Short, DefVal.Short, DefVal.Short, DefVal.Short
                    , DefVal.Str);

                //文字欄位
                ICellStyle textStyle = NpoiHelper.CreateCellStyle(workbook
                    , NpoiHelper.CreateFont(workbook, 12, "Arial", DefVal.Short, DefVal.Bool, null, DefVal.Short)
                    , false
                    , VerticalAlignment.Bottom, HorizontalAlignment.Left
                    , null, NpoiHelper.CellBorder.None, NpoiHelper.CellBorder.None, NpoiHelper.CellBorder.None
                    , DefVal.Short, DefVal.Short, DefVal.Short, DefVal.Short
                    , DefVal.Str);
                #endregion

                curCellIdx = 0;
                row = sheet.CreateRow(curRowIdx);

                row.CreateCell(curCellIdx).SetCellValue(string.Empty);
                row.GetCell(curCellIdx).CellStyle = firstCellStyle;
                curCellIdx++;

                row.CreateCell(curCellIdx).SetCellValue(string.Empty);
                firstEmptyCellIdx = NpoiHelper.GetZeroBasedCellIdx("B");
                lastEmptyIdx = NpoiHelper.GetZeroBasedCellIdx("E");
                sheet.AddMergedRegion(new CellRangeAddress(curRowIdx, curRowIdx, firstEmptyCellIdx, lastEmptyIdx));
                row.GetCell(curCellIdx).CellStyle = textStyle;
                curCellIdx++;
                //因合併儲存格, 補上被合併的樣式.
                for (; firstEmptyCellIdx < lastEmptyIdx; firstEmptyCellIdx++)
                {
                    row.CreateCell(curCellIdx).CellStyle = textStyle;
                    curCellIdx++;
                }

                row.CreateCell(curCellIdx).SetCellValue(string.Empty);
                row.GetCell(curCellIdx).CellStyle = middleCellStyle;
                curCellIdx++;

                row.CreateCell(curCellIdx).SetCellValue(string.Empty);
                firstEmptyCellIdx = NpoiHelper.GetZeroBasedCellIdx("G");
                lastEmptyIdx = NpoiHelper.GetZeroBasedCellIdx("J");
                sheet.AddMergedRegion(new CellRangeAddress(curRowIdx, curRowIdx, firstEmptyCellIdx, lastEmptyIdx));
                row.GetCell(curCellIdx).CellStyle = textStyle;
                curCellIdx++;
                //因合併儲存格, 補上被合併的樣式.
                for (; firstEmptyCellIdx < lastEmptyIdx; firstEmptyCellIdx++)
                {
                    row.CreateCell(curCellIdx).CellStyle = textStyle;
                    curCellIdx++;
                }

                curRowIdx++;
            }
            #endregion

            #region 甲乙雙方簽名提示
            if (true)
            {
                #region Style
                //首欄
                ICellStyle firstCellStyle = NpoiHelper.CreateCellStyle(workbook
                    , NpoiHelper.CreateFont(workbook, 12, "Arial", DefVal.Short, DefVal.Bool, null, DefVal.Short)
                    , false
                    , VerticalAlignment.Bottom, HorizontalAlignment.Left
                    , null, NpoiHelper.CellBorder.None, NpoiHelper.CellBorder.None, NpoiHelper.CellBorder.None
                    , DefVal.Short, DefVal.Short, DefVal.Short, DefVal.Short
                    , DefVal.Str);

                //中間欄位
                ICellStyle middleCellStyle = NpoiHelper.CreateCellStyle(workbook
                    , NpoiHelper.CreateFont(workbook, 12, "Arial", DefVal.Short, DefVal.Bool, null, DefVal.Short)
                    , false
                    , VerticalAlignment.Bottom, HorizontalAlignment.Left
                    , null, NpoiHelper.CellBorder.None, NpoiHelper.CellBorder.None, NpoiHelper.CellBorder.None
                    , DefVal.Short, DefVal.Short, DefVal.Short, DefVal.Short
                    , DefVal.Str);

                //文字欄位
                ICellStyle textStyle = NpoiHelper.CreateCellStyle(workbook
                    , NpoiHelper.CreateFont(workbook, 12, "Arial", DefVal.Short, DefVal.Bool, null, DefVal.Short)
                    , false
                    , VerticalAlignment.Bottom, HorizontalAlignment.Left
                    , null, NpoiHelper.CellBorder.None, NpoiHelper.CellBorder.None, NpoiHelper.CellBorder.None
                    , DefVal.Short, DefVal.Short, DefVal.Short, DefVal.Short
                    , DefVal.Str);
                #endregion

                curCellIdx = 0;
                row = sheet.CreateRow(curRowIdx);

                row.CreateCell(curCellIdx).SetCellValue(string.Empty);
                row.GetCell(curCellIdx).CellStyle = firstCellStyle;
                curCellIdx++;

                row.CreateCell(curCellIdx).SetCellValue("Signature over Printed Name");
                firstEmptyCellIdx = NpoiHelper.GetZeroBasedCellIdx("B");
                lastEmptyIdx = NpoiHelper.GetZeroBasedCellIdx("E");
                sheet.AddMergedRegion(new CellRangeAddress(curRowIdx, curRowIdx, firstEmptyCellIdx, lastEmptyIdx));
                row.GetCell(curCellIdx).CellStyle = textStyle;
                curCellIdx++;
                //因合併儲存格, 補上被合併的樣式.
                for (; firstEmptyCellIdx < lastEmptyIdx; firstEmptyCellIdx++)
                {
                    row.CreateCell(curCellIdx).CellStyle = textStyle;
                    curCellIdx++;
                }

                row.CreateCell(curCellIdx).SetCellValue(string.Empty);
                row.GetCell(curCellIdx).CellStyle = middleCellStyle;
                curCellIdx++;

                row.CreateCell(curCellIdx).SetCellValue("Signature over Printed Name");
                firstEmptyCellIdx = NpoiHelper.GetZeroBasedCellIdx("G");
                lastEmptyIdx = NpoiHelper.GetZeroBasedCellIdx("J");
                sheet.AddMergedRegion(new CellRangeAddress(curRowIdx, curRowIdx, firstEmptyCellIdx, lastEmptyIdx));
                row.GetCell(curCellIdx).CellStyle = textStyle;
                curCellIdx++;
                //因合併儲存格, 補上被合併的樣式.
                for (; firstEmptyCellIdx < lastEmptyIdx; firstEmptyCellIdx++)
                {
                    row.CreateCell(curCellIdx).CellStyle = textStyle;
                    curCellIdx++;
                }

                curRowIdx++;
            }
            #endregion

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
    #endregion
}