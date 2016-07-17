using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACool
{
    public class Sheet
    {
        private ISheet sheet = null;

        public string SheetName
        {
            get
            {
                return sheet.SheetName;
            }
        }

        public Sheet(ISheet sheet)
        {
            this.sheet = sheet;
        }

        public int RowCount
        {
            get
            {
                return sheet.RowCount();
            }
        }

        public int ColCount
        {
            get
            {

                IEnumerable<int> nums = Enumerable.Range(0, RowCount);

                IEnumerable<IRow> rows = nums.Select(i => sheet.GetRow(i)).Where(row => row != null);

                if (rows.Count() == 0)
                {
                    return 0;
                }
                return rows.Max(row => row.LastCellNum);
            }
        }

        public DataTable ConvertToDataTable(bool FirstRowAsTitle = false)
        {
            DataTable dt = new DataTable(SheetName);

            if (FirstRowAsTitle)
            {
                for (int rowIndex = 0; rowIndex < RowCount; rowIndex++)
                {
                    if (rowIndex == 0)
                    {
                        for (int colIndex = 0; colIndex < ColCount; colIndex++)
                        {
                            dt.Columns.Add(Convert.ToString(GetValue(rowIndex, colIndex)));
                        }
                    }
                    else
                    {
                        object[] values = GetValues(rowIndex);

                        dt.LoadDataRow(values, false);
                    }
                }
            }
            else
            {
                for (int colIndex = 0; colIndex < ColCount; colIndex++)
                {
                    dt.Columns.Add(ExcelUtilityX.ConvertToExcelColumnName(colIndex));
                }

                for (int rowIndex = 0; rowIndex < RowCount; rowIndex++)
                {
                    object[] values = GetValues(rowIndex);

                    dt.LoadDataRow(values, false);
                }
            }

            return dt;
        }

        public void SetValue(int rowIndex, List<object> values)
        {
            var row = sheet.CreateRow(rowIndex);

            for (int i = 0; i < values.Count; i++)
            {
                row.CreateCell(i).SetCellValue(Convert.ToString(values[i]));
            }
        }

        public void SetValue(int rowIndex, int colIndex, object value)
        {
            IRow row = sheet.GetRow(rowIndex) ?? sheet.CreateRow(rowIndex);

            row.CreateCell(colIndex).SetCellValue(Convert.ToString(value));
        }

        public void LoadDataTable(DataTable dt)
        {
            foreach (DataColumn dc in dt.Columns)
            {
                SetValue(0, dc.Ordinal, dc.ColumnName);
            }

            int RowIndex = 1;

            foreach (DataRow dr in dt.Rows)
            {
                foreach (DataColumn dc in dt.Columns)
                {
                    SetValue(RowIndex, dc.Ordinal, Convert.ToString(dr[dc]));
                }

                RowIndex++;
            }
        }

        public bool HasValues(int RowIndex)
        {
            if (sheet.GetRow(RowIndex) == null)
            {
                return false;
            }

            return true;
        }
        public object[] GetValues(int RowIndex)
        {
            return Enumerable.Range(0, ColCount).Select(x => GetValue(RowIndex, x)).ToArray();
        }

        public object GetValue(int RowIndex, int ColIndex)
        {
            object NullObject = null;

            IRow currentRow = sheet.GetRow(RowIndex);

            if (currentRow != null)
            {
                ICell cell = currentRow.GetCell(ColIndex);

                if (cell != null)
                {
                    if (cell.CellType == CellType.Formula)
                    {
                        return cell.StringCellValue;
                    }

                    return cell.ToString();
                }
                else
                {
                    return NullObject;
                }

            }
            else
            {
                return NullObject;
            }
        }

        public void DeleteRow(int RowIndex)
        {
            IRow currentRow = sheet.GetRow(RowIndex);

            sheet.RemoveRow(currentRow);
        }

        public int GetColumnWidth(int colIndex)
        {
            return sheet.GetColumnWidth(colIndex);
        }

        public void SetDefaultSize(int width, float height)
        {
            sheet.DefaultColumnWidth = width;

            sheet.DefaultRowHeightInPoints = height;
        }

        private BorderStyle borderStyleLine { get { return BorderStyle.Thin; } }

        private short borderColor
        {
            get
            {
                return 8;//8=black 
            }
        }

        public void SetFrame(int Lx, int Ly, int ColCount, int RowCount)
        {
            for (int x = Lx; x < Lx + ColCount; x++)
            {
                //draw top line

                DrawTopLine(x, Ly);

                //draw bottom line

                DrawBottomLine(x, Ly + RowCount - 1);
            }

            for (int y = Ly; y < Ly + RowCount; y++)
            {
                //draw left line

                DrawLeftLine(Lx, y);

                //draw right line

                DrawRightLine(Lx + ColCount - 1, y);
            }
        }



        public void DrawTopLine(int x, int y)
        {
            ICell cell = GetCell(x, y);

            ChangeStyle(cell, DrawTop);

        }

        public void DrawBottomLine(int x, int y)
        {
            ICell cell = GetCell(x, y);

            ChangeStyle(cell, DrawBottom);

        }

        public void DrawLeftLine(int x, int y)
        {
            ICell cell = GetCell(x, y);

            ChangeStyle(cell, DrawLeft);
        }

        private void ChangeStyle(ICell cell, Action<ICellStyle> action)
        {
            ICellStyle oStyle = (sheet.Workbook as XSSFWorkbook).CreateCellStyle();

            oStyle.BorderLeft = cell.CellStyle.BorderLeft;
            oStyle.BorderRight = cell.CellStyle.BorderRight;
            oStyle.BorderTop = cell.CellStyle.BorderTop;
            oStyle.BorderBottom = cell.CellStyle.BorderBottom;

            action(oStyle);

            cell.CellStyle = oStyle;
        }

        public void DrawRightLine(int x, int y)
        {
            ICell cell = GetCell(x, y);

            ChangeStyle(cell, DrawRight);
        }




        private ICell GetCell(int x, int y)
        {
            IRow currentRow = sheet.GetRow(y);

            if (currentRow == null)
            {
                currentRow = sheet.CreateRow(y);
            }

            ICell cell = currentRow.GetCell(x);

            if (cell == null)
            {
                cell = currentRow.CreateCell(x);
            }

            return cell;
        }


        public void MergeRow(int Lx, int Ly, int ColCount, int RowCount)
        {
            for (int y = Ly; y < Ly + RowCount; y++)
            {
                sheet.AddMergedRegion(new CellRangeAddress(y, y, Lx, Lx + ColCount - 1));

                MiddleText(Lx, y);
            }
        }

        public void MiddleText(int x, int y)
        {
            ICell cell = GetCell(x, y);

            ChangeStyle(cell, MiddleText);
        }

        private void DrawTop(ICellStyle oStyle)
        {
            oStyle.BorderTop = borderStyleLine;
            oStyle.TopBorderColor = borderColor;
        }

        private void DrawBottom(ICellStyle oStyle)
        {
            oStyle.BorderBottom = borderStyleLine;
            oStyle.BottomBorderColor = borderColor;
        }

        private void DrawLeft(ICellStyle oStyle)
        {
            oStyle.BorderLeft = borderStyleLine;
            oStyle.LeftBorderColor = borderColor;
        }

        private void DrawRight(ICellStyle oStyle)
        {
            oStyle.BorderRight = borderStyleLine;
            oStyle.RightBorderColor = borderColor;
        }

        private void MiddleText(ICellStyle oStyle)
        {
            oStyle.Alignment = HorizontalAlignment.Center;
        }



    }
}
