using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACool
{
    public class ExcelUtilityX
    {
        public static string ConvertToExcelColumnName(int n)
        {
            int unit = 26;

            if (n < unit)
            {
                return Convert.ToChar(65 + n).ToString();
            }
            else
            {
                int FirstNumber = n % unit;

                int SecondNumber = n / unit - 1;

                return ConvertToExcelColumnName(SecondNumber) + ConvertToExcelColumnName(FirstNumber);
            }
        }

        public static Dictionary<string, DataTable> GetExcelToDataTable(Stream stream, int TitleRowIndex = 0)
        {
            Dictionary<string, DataTable> results = new Dictionary<string, DataTable>();

            IWorkbook workbook = new XSSFWorkbook(stream);

            for (int i = 0; i < workbook.NumberOfSheets; i++)
            {
                ISheet sheet = workbook.GetSheetAt(i);

                results.Add(sheet.SheetName, sheet.ConvertToDataTable(TitleRowIndex));
            }

            return results;
        }


    }
}
