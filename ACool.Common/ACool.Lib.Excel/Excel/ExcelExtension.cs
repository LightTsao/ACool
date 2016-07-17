using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACool
{
    internal static class ExcelExtension
    {
        public static string GetStringValue(this ICell cell)
        {
            if (cell == null)
            {
                return string.Empty;
            }

            string result = Convert.ToString(cell);

            switch (cell.CellType)
            {
                case CellType.Blank:
                case CellType.Error:
                case CellType.Unknown:

                    break;
                case CellType.Boolean:
                    result = Convert.ToString(cell.BooleanCellValue);
                    break;

                case CellType.Formula:
                    {
                        if (cell.CachedFormulaResultType == CellType.Numeric)
                        {
                            result = Convert.ToString(cell.NumericCellValue);

                            int tempInt;
                            double tempDoube;

                            if (!Int32.TryParse(result, out tempInt) && !Double.TryParse(result, out tempDoube))
                            {
                                result = cell.DateCellValue.ToString("yyyy/MM/dd hh:mm:ss");
                            }
                        }
                        else
                        {
                            result = cell.StringCellValue;
                        }

                        break;
                    }
                case CellType.String:

                    result = cell.StringCellValue;
                    break;

                case CellType.Numeric:
                    {

                        int tempInt;
                        double tempDoube;

                        if (!Int32.TryParse(result, out tempInt) && !Double.TryParse(result, out tempDoube))
                        {
                            result = cell.DateCellValue.ToString("yyyy/MM/dd hh:mm:ss");
                        }

                        break;
                    }

            }

            return result;
        }  
        public static int ColumnCount(this IRow row)
        {
            if (row == null)
            {
                return 0;
            }

            if (row.LastCellNum < 0)
            {
                return 0;
            }

            return row.LastCellNum;
        }
        public static bool isEmpty(this IRow currentRow)
        {
            return Enumerable.Range(0, currentRow.ColumnCount()).Select(x => currentRow.GetCell(x).GetStringValue()).All(x => string.IsNullOrEmpty(x));
        }
        public static List<string> GetStringValue(this IRow currentRow, int ColCount, int ColIndexStart = 0)
        {
            if (currentRow == null)
            {
                return Enumerable.Repeat(string.Empty, ColCount).ToList();
            }

            List<string> Values = new List<string>();

            for (int ColIndex = ColIndexStart; ColIndex < ColIndexStart + ColCount; ColIndex++)
            {
                ICell cell = currentRow.GetCell(ColIndex);

                string value = null;

                if (cell != null)
                {
                    value = cell.GetStringValue();
                }

                Values.Add(value);
            }

            return Values;
        }

        public static List<T> GetExcelToEnties<T>(this IWorkbook workbook, string SheetName)
        {
            ISheet sheet = workbook.GetSheet(SheetName);

            return sheet.ToEntities<T>();
        }

        public static void Export(this IWorkbook workbook, string filepath)
        {
            using (FileStream stream = new FileStream(filepath, FileMode.Create))
            {
                workbook.Write(stream);
            }
        }

        public static MemoryStream ConvertToStream(this IWorkbook workbook)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                ms.Seek(0, SeekOrigin.Begin);
                workbook.Write(ms);

                return new MemoryStream(ms.ToArray());
            }
        }


        public static int ColumnCount(this ISheet sheet)
        {
            return Enumerable.Range(0, sheet.RowCount()).Select(x => sheet.GetRow(x)).Max(x => x.ColumnCount());
        }

        public static int RowCount(this ISheet sheet)
        {
            return sheet.LastRowNum + 1;
        }

        public static DataTable ConvertToDataTable(this ISheet sheet, int TitleRowIndex = 0)
        {
            DataTable dt = new DataTable(sheet.SheetName);

            IRow titleRow = sheet.GetRow(TitleRowIndex);

            bool isEffectiveTitle = titleRow != null && TitleRowIndex >= 0 && TitleRowIndex < sheet.LastRowNum;

            if (isEffectiveTitle)
            {
                int ColCount = titleRow.ColumnCount();

                DataColumn[] cols = titleRow.GetStringValue(ColCount).Select(x => new DataColumn(x, typeof(string))).ToArray();

                dt.Columns.AddRange(cols);

                List<List<string>> tempRow = new List<List<string>>();

                for (int rowIndex = TitleRowIndex + 1; rowIndex < sheet.LastRowNum + 1; rowIndex++)
                {
                    IRow CurrentRow = sheet.GetRow(rowIndex);

                    List<string> values = CurrentRow.GetStringValue(ColCount);

                    if (CurrentRow.isEmpty())
                    {
                        tempRow.Add(values);
                    }
                    else
                    {
                        if (tempRow.Count() > 0)
                        {
                            foreach (List<string> row in tempRow)
                            {
                                dt.LoadDataRow(row.ToArray(), false);
                            }

                            tempRow.Clear();
                        }

                        dt.LoadDataRow(values.ToArray(), false);
                    }
                }
            }
            else
            {
                int ColCount = sheet.ColumnCount();

                DataColumn[] cols = Enumerable.Range(0, ColCount).Select(x => new DataColumn(ExcelUtilityX.ConvertToExcelColumnName(x), typeof(string))).ToArray();

                dt.Columns.AddRange(cols);

                List<List<string>> tempRow = new List<List<string>>();

                for (int rowIndex = 0; rowIndex < sheet.RowCount(); rowIndex++)
                {
                    IRow CurrentRow = sheet.GetRow(rowIndex);

                    List<string> values = CurrentRow.GetStringValue(ColCount);

                    if (CurrentRow.isEmpty())
                    {
                        tempRow.Add(values);
                    }
                    else
                    {
                        if (tempRow.Count() > 0)
                        {
                            foreach (List<string> row in tempRow)
                            {
                                dt.LoadDataRow(row.ToArray(), false);
                            }

                            tempRow.Clear();
                        }

                        dt.LoadDataRow(values.ToArray(), false);
                    }
                }
            }


            return dt;
        }
        public static List<T> ToEntities<T>(this ISheet sheet)
        {
            IRow titleRow = sheet.GetRow(0);

            List<T> Result = new List<T>();

            int colCount = sheet.ColumnCount();

            for (int rowIndex = 1; rowIndex < sheet.RowCount(); rowIndex++)
            {
                T entity = Activator.CreateInstance<T>();

                IRow currentRow = sheet.GetRow(rowIndex);

                if (currentRow.isEmpty())
                {
                    continue;
                }


                for (int columnIndex = 0; columnIndex < colCount; columnIndex++)
                {
                    string columnName = titleRow.GetCell(columnIndex).GetStringValue();

                    object value = currentRow.GetCell(columnIndex).GetStringValue();

                    var p = typeof(T).GetProperty(columnName);

                    if (p != null)
                    {
                        p.SetValue(entity, Convert.ChangeType(value, p.PropertyType));
                    }
                }

                Result.Add(entity);
            }

            return Result;
        }
    }
}
