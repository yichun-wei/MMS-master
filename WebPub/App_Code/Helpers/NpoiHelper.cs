using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

using NPOI.SS.Util;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel; //xls
using NPOI.XSSF.UserModel; //xlsx
using NPOI.HSSF.Util;

namespace Seec.Marketing
{
    /// <summary>
    /// NPOI Helper。
    /// </summary>
    public static class NpoiHelper
    {
        #region GetWorkbook
        /// <summary>
        /// 取得匯入的 Workbook。
        /// </summary>
        /// <param name="filePath">檔案的實體路徑。</param>
        public static IWorkbook GetWorkbook(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return null;
            }

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                return NpoiHelper.GetWorkbook(fs, Path.GetExtension(filePath));
            }
        }

        /// <summary>
        /// 取得匯入的 Workbook。
        /// </summary>
        /// <param name="stream">檔案的資料流。</param>
        /// <param name="fileExt">檔案的副檔名。</param>
        /// <returns></returns>
        public static IWorkbook GetWorkbook(Stream stream, string fileExt)
        {
            if (stream == null || string.IsNullOrWhiteSpace(fileExt))
            {
                return null;
            }

            switch (fileExt.ToLowerInvariant())
            {
                case ".xls":
                    return new HSSFWorkbook(stream);
                case ".xlsx":
                    return new XSSFWorkbook(stream);
                default:
                    return null;
            }
        }
        #endregion

        #region GetCellIdx
        /// <summary>
        /// 取得欄位索引值。
        /// </summary>
        /// <param name="headerRow">列。</param>
        /// <param name="colName">欄位名稱。</param>
        public static int GetCellIdx(IRow headerRow, string colName)
        {
            for (int i = headerRow.FirstCellNum; i < headerRow.LastCellNum; i++)
            {
                if (NpoiHelper.GetCellVal(headerRow.GetCell(i)) == colName)
                {
                    return i;
                }
            }

            return -1;
        }
        #endregion

        #region GetCellVal
        /// <summary>
        /// 取得欄位值。
        /// </summary>
        /// <param name="cell">欄位。</param>
        public static string GetCellVal(ICell cell)
        {
            if (cell == null || cell.CellType == CellType.Blank)
            {
                return string.Empty;
            }

            switch (cell.CellType)
            {
                case CellType.Formula:
                    //欄位為公式, 取得運算後的值.
                    return cell.NumericCellValue.ToString();
                case CellType.Numeric:
                    //欄位為數值, 直接取數值 (若欄位有被格式化, 直接「cell.ToString()」取, 會取到格式化的值, 例如「- 0.05」, 負號與數值中間有一個空格).
                    //與 CellType.Formula 先分開來取.
                    return cell.NumericCellValue.ToString();
                default:
                    return cell.ToString();
            }
        }
        #endregion

        #region GetZeroBasedCellIdx
        static string[] CellAphabets = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

        /// <summary>
        /// 取得以 0 為起始的欄位字母對應的索引位置。
        /// </summary>
        /// <param name="aphabet">列字母。</param>
        public static int GetZeroBasedCellIdx(string aphabet)
        {
            aphabet = aphabet.ToUpperInvariant();
            for (int i = 0; i < CellAphabets.Length; i++)
            {
                if (aphabet == CellAphabets[i])
                {
                    return i;
                }
            }

            return 0;
        }
        #endregion

        #region DeleteRow
        /// <summary>
        /// 刪除指定範圍的資料列。
        /// </summary>
        /// <param name="sheet">資料表。</param>
        /// <param name="beginRowIdx">以 0 為啟始的資料列索引。</param>
        /// <param name="totalRows">共要刪除幾列。</param>
        public static void DeleteRow(ISheet sheet, int beginRowIdx, int totalRows)
        {
            int execCnt = 0;
            do
            {
                NpoiHelper.DeleteRow(sheet, beginRowIdx);
                execCnt++;
            } while (execCnt < totalRows);
        }

        /// <summary>
        /// 刪除資料列。
        /// </summary>
        /// <param name="sheet">資料表。</param>
        /// <<param name="rowIndex">以 0 為啟始的資料列索引。</param>
        public static void DeleteRow(ISheet sheet, int rowIdx)
        {
            int lastRowNum = sheet.LastRowNum;

            if (rowIdx >= 0 && rowIdx < lastRowNum)
            {
                sheet.ShiftRows(rowIdx + 1, lastRowNum, -1);
            }

            if (rowIdx == lastRowNum)
            {
                var removingRow = sheet.GetRow(rowIdx);
                if (removingRow != null)
                {
                    sheet.RemoveRow(removingRow);
                }
            }
        }
        #endregion

        #region ChangeDataFormat
        /// <summary>
        /// 改變儲存格資料格式。
        /// </summary>
        /// <param name="workbook">工作表。</param>
        /// <param name="style">樣式。</param>
        /// <param name="format">格式</param>
        public static ICellStyle ChangeDataFormat(IWorkbook workbook, ICellStyle style, string format)
        {
            var dataFormat = workbook.CreateDataFormat();
            style.DataFormat = dataFormat.GetFormat(format);
            return style;
        }
        #endregion

        #region CreateFont
        /// <summary>
        /// 建立字型。
        /// </summary>
        /// <param name="workbook">工作表。</param>
        /// <param name="size">字型大小。</param>
        /// <param name="name">字型名稱。</param>
        /// <param name="boldWeight">粗體樣式。</param>
        /// <param name="isItalic">是否為斜體。</param>
        /// <param name="underlineType">底線樣式。</param>
        /// <param name="color">顏色。</param>
        /// <returns></returns>
        public static IFont CreateFont(IWorkbook workbook, short? size, string name, short? boldWeight, bool? isItalic, Nullable<FontUnderlineType> underlineType, short? color)
        {
            IFont font = workbook.CreateFont();
            if (size.HasValue) { font.FontHeightInPoints = size.Value; }
            if (!string.IsNullOrWhiteSpace(name)) { font.FontName = name; }
            //範例「font.Boldweight = (short)FontBoldWeight.Bold;」
            if (boldWeight.HasValue) { font.Boldweight = boldWeight.Value; }
            if (isItalic.HasValue) { font.IsItalic = isItalic.Value; }
            //範例「font.Underline = FontUnderlineType.Single;」
            if (underlineType.HasValue) { font.Underline = underlineType.Value; }
            //範例「font.Color = HSSFColor.Red.Index;」
            if (color.HasValue) { font.Color = color.Value; }
            return font;
        }
        #endregion

        #region CreateCellStyle
        #region CellBorder
        /********************************************************************************
         * 因為會和 System.Web.UI.WebControls.BorderStyle 衝突, 另外建一個對應, 以方便除錯與維護.
        ********************************************************************************/
        /// <summary>
        /// 對應 NPOI.SS.UserModel.BorderStyle 的列舉對象。
        /// </summary>
        public enum CellBorder
        {
            /// <summary>
            /// No border。
            /// </summary>
            None = 0,
            /// <summary>
            /// Thin border。
            /// </summary>
            Thin = 1,
            /// <summary>
            /// Medium border。
            /// </summary>
            Medium = 2,
            /// <summary>
            /// Dash border。
            /// </summary>
            Dashed = 3,
            /// <summary>
            /// Dot border。
            /// </summary>
            Dotted = 4,
            /// <summary>
            /// Thick border。
            /// </summary>
            Thick = 5,
            /// <summary>
            /// Double-line border。
            /// </summary>
            Double = 6,
            /// <summary>
            /// Hair-line border。
            /// </summary>
            Hair = 7,
            /// <summary>
            /// Medium dashed border。
            /// </summary>
            MediumDashed = 8,
            /// <summary>
            /// Dash-dot border。
            /// </summary>
            DashDot = 9,
            /// <summary>
            /// Medium dash-dot border。
            /// </summary>
            MediumDashDot = 10,
            /// <summary>
            /// Dash-dot-dot border。
            /// </summary>
            DashDotDot = 11,
            /// <summary>
            /// Medium dash-dot-dot border。
            /// </summary>
            MediumDashDotDot = 12,
            /// <summary>
            /// Slanted dash-dot border。
            /// </summary>
            SlantedDashDot = 13,
        }
        #endregion

        /// <summary>
        /// 建立儲存格樣式。
        /// </summary>
        /// <param name="workbook">工作表。</param>
        /// <param name="font">字型。</param>
        /// <param name="wrapText">是否自動換行。</param>
        /// <param name="verticalAlignment">垂直對齊。</param>
        /// <param name="horizontalAlignment">水平對齊。</param>
        /// <param name="borderTop">上框線。</param>
        /// <param name="borderRight">右框線。</param>
        /// <param name="borderBottom">下框線。</param>
        /// <param name="borderLeft">左框線。</param>
        /// <param name="topBorderColor">上框線顏色。</param>
        /// <param name="rightBorderColor">右框線顏色。</param>
        /// <param name="bottomBorderColor">下框線顏色。</param>
        /// <param name="leftBorderColor">左框線顏色。</param>
        /// <param name="format">格式化樣式。</param>
        public static ICellStyle CreateCellStyle(IWorkbook workbook, IFont font, bool? wrapText, Nullable<VerticalAlignment> verticalAlignment, Nullable<HorizontalAlignment> horizontalAlignment, Nullable<CellBorder> borderTop, Nullable<CellBorder> borderRight, Nullable<CellBorder> borderBottom, Nullable<CellBorder> borderLeft, short? topBorderColor, short? rightBorderColor, short? bottomBorderColor, short? leftBorderColor, string format)
        {
            ICellStyle style = workbook.CreateCellStyle();

            if (font != null) { style.SetFont(font); }

            if (wrapText.HasValue) { style.WrapText = wrapText.Value; } //自動換行
            if (verticalAlignment.HasValue) { style.VerticalAlignment = verticalAlignment.Value; } //垂直對齊
            if (horizontalAlignment.HasValue) { style.Alignment = horizontalAlignment.Value; } //水平對齊
            if (borderTop.HasValue) { style.BorderTop = (BorderStyle)((int)borderTop.Value); } //上框線
            if (borderRight.HasValue) { style.BorderRight = (BorderStyle)((int)borderRight.Value); } //右框線
            if (borderBottom.HasValue) { style.BorderBottom = (BorderStyle)((int)borderBottom.Value); } //下框線
            if (borderLeft.HasValue) { style.BorderLeft = (BorderStyle)((int)borderLeft.Value); } //左框線
            //範例「style.TopBorderColor = HSSFColor.Grey25Percent.Index;」
            if (topBorderColor.HasValue) { style.TopBorderColor = topBorderColor.Value; } //上框線顏色
            if (rightBorderColor.HasValue) { style.RightBorderColor = rightBorderColor.Value; } //右框線顏色
            if (bottomBorderColor.HasValue) { style.BottomBorderColor = bottomBorderColor.Value; } //下框線顏色
            if (leftBorderColor.HasValue) { style.LeftBorderColor = HSSFColor.Grey25Percent.Index; } //左框線顏色

            //範例「#,##0」
            if (!string.IsNullOrWhiteSpace(format))
            {
                var dataFormat = workbook.CreateDataFormat();
                style.DataFormat = dataFormat.GetFormat(format);
            }

            return style;
        }
        #endregion

        #region FillEmptyCell
        /// <summary>
        /// 填補連續空值儲存格。
        /// </summary>
        /// <param name="row">列。</param>
        /// <param name="curCellIdx">作用中的儲存格索引。若與「firstEmptyCellIdx」索引相同，則會使用首欄儲存格樣式。</param>
        /// <param name="firstEmptyCellIdx">起始要填空值的儲存格索引。</param>
        /// <param name="lastEmptyIdx">結束要填空值的儲存格索引。</param>
        /// <param name="firstCellStyle">首欄儲存格樣式。</param>
        /// <param name="middleCellStyle">中間欄位儲存格樣式。</param>
        /// <param name="lastCellStyle">末欄儲存格樣式。</param>
        public static void FillEmptyCell(IRow row, int curCellIdx, int firstEmptyCellIdx, int lastEmptyIdx, ICellStyle firstCellStyle, ICellStyle middleCellStyle, ICellStyle lastCellStyle)
        {
            for (; curCellIdx <= lastEmptyIdx; curCellIdx++)
            {
                row.CreateCell(curCellIdx).SetCellValue(string.Empty);

                if (curCellIdx == firstEmptyCellIdx)
                {
                    if (firstCellStyle != null)
                    {
                        row.GetCell(curCellIdx).CellStyle = firstCellStyle;
                    }
                }
                else if (curCellIdx == lastEmptyIdx)
                {
                    if (lastCellStyle != null)
                    {
                        row.GetCell(curCellIdx).CellStyle = lastCellStyle;
                    }
                }
                else
                {
                    if (middleCellStyle != null)
                    {
                        row.GetCell(curCellIdx).CellStyle = middleCellStyle;
                    }
                }
            }
        }
        #endregion
    }
}
