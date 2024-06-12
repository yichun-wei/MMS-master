﻿using System;
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
using NPOI.HSSF.Util;

public partial class popup_ext_print_prod : System.Web.UI.Page
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
    ExtProdOrderHelper.InfoSet OrigInfo { get; set; }
    #endregion

    //改成用生產單系統代號
    //ISystemId _extQuotnSId;
    ISystemId _extProdOrderSId;

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

        //改成用生產單系統代號
        //this._extQuotnSId = ConvertLib.ToSId(Request.QueryString["quotnSId"]);
        //if (this._extQuotnSId == null)
        //{
        //    JSBuilder.ClosePage(this);
        //    return false;
        //}
        this._extProdOrderSId = ConvertLib.ToSId(Request.QueryString["sid"]);
        if (this._extProdOrderSId == null)
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
            //改成用生產單系統代號
            //this.OrigInfo = ExtProdOrderHelper.Binding(this._extQuotnSId, DefVal.SId, DefVal.Int, DefVal.Bool, DefVal.SId, DefVal.Int, true);
            this.OrigInfo = ExtProdOrderHelper.Binding(DefVal.SId, DefVal.SId, DefVal.Int, DefVal.Bool, this._extProdOrderSId, DefVal.Int, DefVal.Bool);
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

        var fileName = string.Format("外銷生產單_{0}.xlsx", this.OrigInfo.Info.OdrNo);

        string templatePath = WebUtilBox.MapPath("~/sample/ext/生產單.xlsx");

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

            #region 訂單資訊
            curRowIdx = 1;
            row = sheet.GetRow(curRowIdx);
            row.GetCell(NpoiHelper.GetZeroBasedCellIdx("B")).SetCellValue(this.OrigInfo.Info.OdrNo); //訂單編號
            row.GetCell(NpoiHelper.GetZeroBasedCellIdx("F")).SetCellValue(ConvertLib.TraditionalToSimplified(ExtProdOrderHelper.GetExtProdOrderStatusName(this.OrigInfo.Info.Status))); //生產單狀態

            curRowIdx = 2;
            row = sheet.GetRow(curRowIdx);
            row.GetCell(NpoiHelper.GetZeroBasedCellIdx("B")).SetCellValue(ConvertLib.ToStr(this.OrigInfo.Info.Cdt, string.Empty, "yyyy/MM/dd")); //建單日期
            row.GetCell(NpoiHelper.GetZeroBasedCellIdx("F")).SetCellValue(ConvertLib.ToStr(this.OrigInfo.Info.QuotnDate, string.Empty, "yyyy/MM/dd")); //訂單日期

            curRowIdx = 3;
            row = sheet.GetRow(curRowIdx);
            row.GetCell(NpoiHelper.GetZeroBasedCellIdx("B")).SetCellValue(this.OrigInfo.Info.ProdOdrNo); //生產單號
            row.GetCell(NpoiHelper.GetZeroBasedCellIdx("F")).SetCellValue(ConvertLib.ToStr(this.OrigInfo.Info.Edd, string.Empty, "yyyy/MM/dd")); //預計交貨日

            curRowIdx = 4;
            row = sheet.GetRow(curRowIdx);
            row.GetCell(NpoiHelper.GetZeroBasedCellIdx("B")).SetCellValue(ConvertLib.TraditionalToSimplified(this.OrigInfo.Info.CustomerName)); //客戶名稱
            row.GetCell(NpoiHelper.GetZeroBasedCellIdx("F")).SetCellValue(ConvertLib.ToStr(this.OrigInfo.Info.Cdd, string.Empty, "yyyy/MM/dd")); //客戶需求日 
            #endregion

            #region 品項資訊
            curRowIdx = 8;
            if (this.OrigInfo.DetInfos != null && this.OrigInfo.DetInfos.Count > 0)
            {
                //依製造部門排序
                this.OrigInfo.DetInfos = this.OrigInfo.DetInfos.OrderBy(q => q.OrganizationCode).ToList();

                #region Style
                //首欄
                ICellStyle firstCellStyle = NpoiHelper.CreateCellStyle(workbook
                    , NpoiHelper.CreateFont(workbook, 12, "新細明體", DefVal.Short, DefVal.Bool, null, DefVal.Short)
                    , false
                    , VerticalAlignment.Center, HorizontalAlignment.Right
                    , NpoiHelper.CellBorder.Medium, NpoiHelper.CellBorder.None, NpoiHelper.CellBorder.Medium, NpoiHelper.CellBorder.Medium
                    , HSSFColor.Grey25Percent.Index, HSSFColor.Grey25Percent.Index, HSSFColor.Grey25Percent.Index, HSSFColor.Grey25Percent.Index
                    , DefVal.Str);

                //文字欄位
                ICellStyle textCellStyle = NpoiHelper.CreateCellStyle(workbook
                    , NpoiHelper.CreateFont(workbook, 12, "新細明體", DefVal.Short, DefVal.Bool, null, DefVal.Short)
                    , true
                    , VerticalAlignment.Center, HorizontalAlignment.Center
                    , NpoiHelper.CellBorder.Medium, NpoiHelper.CellBorder.None, NpoiHelper.CellBorder.Medium, NpoiHelper.CellBorder.None
                    , HSSFColor.Grey25Percent.Index, HSSFColor.Grey25Percent.Index, HSSFColor.Grey25Percent.Index, HSSFColor.Grey25Percent.Index
                    , DefVal.Str);

                //數值欄位
                ICellStyle numeralCellStyle = NpoiHelper.CreateCellStyle(workbook
                    , NpoiHelper.CreateFont(workbook, 12, "新細明體", DefVal.Short, DefVal.Bool, null, DefVal.Short)
                    , false
                    , VerticalAlignment.Center, HorizontalAlignment.Center
                    , NpoiHelper.CellBorder.Medium, NpoiHelper.CellBorder.None, NpoiHelper.CellBorder.Medium, NpoiHelper.CellBorder.None
                    , HSSFColor.Grey25Percent.Index, HSSFColor.Grey25Percent.Index, HSSFColor.Grey25Percent.Index, HSSFColor.Grey25Percent.Index
                    , DefVal.Str);

                //末欄
                ICellStyle lastCellStyle = NpoiHelper.CreateCellStyle(workbook
                    , NpoiHelper.CreateFont(workbook, 12, "新細明體", DefVal.Short, DefVal.Bool, null, DefVal.Short)
                    , false
                    , VerticalAlignment.Center, HorizontalAlignment.Center
                    , NpoiHelper.CellBorder.Medium, NpoiHelper.CellBorder.Medium, NpoiHelper.CellBorder.Medium, NpoiHelper.CellBorder.None
                    , HSSFColor.Grey25Percent.Index, HSSFColor.Grey25Percent.Index, HSSFColor.Grey25Percent.Index, HSSFColor.Grey25Percent.Index
                    , DefVal.Str);
                #endregion

                for (int i = 0; i < this.OrigInfo.DetInfos.Count; i++, curRowIdx++)
                {
                    var detInfo = this.OrigInfo.DetInfos[i];

                    curCellIdx = 0;
                    row = sheet.CreateRow(curRowIdx);

                    //序號
                    row.CreateCell(curCellIdx).SetCellValue(i + 1);
                    row.GetCell(curCellIdx).CellStyle = firstCellStyle;
                    curCellIdx++;

                    //製造部門
                    row.CreateCell(curCellIdx).SetCellValue(detInfo.OrganizationCode);
                    row.GetCell(curCellIdx).CellStyle = textCellStyle;
                    curCellIdx++;

                    //料號
                    row.CreateCell(curCellIdx).SetCellValue(detInfo.PartNo);
                    row.GetCell(curCellIdx).CellStyle = textCellStyle;
                    curCellIdx++;

                    //生產需求量
                    row.CreateCell(curCellIdx).SetCellValue(ConvertLib.ToAccounting(detInfo.ProdQty));
                    row.GetCell(curCellIdx).CellStyle = numeralCellStyle;
                    curCellIdx++;

                    //預計繳庫日
                    row.CreateCell(curCellIdx).SetCellValue(string.Empty);
                    row.GetCell(curCellIdx).CellStyle = textCellStyle;
                    curCellIdx++;

                    //備註
                    row.CreateCell(curCellIdx).SetCellValue(ConvertLib.TraditionalToSimplified(detInfo.Rmk));
                    row.GetCell(curCellIdx).CellStyle = lastCellStyle;
                    curCellIdx++;
                }
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