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
    public class ExcelHelperX
    {
        private IWorkbook wb = null;
        public ExcelHelperX(Stream stream)
        {
            wb = new XSSFWorkbook(stream);
        }
        public List<T> GetEntities<T>(string SheetName)
        {
            return wb.GetExcelToEnties<T>(SheetName);
        }

        public bool ContainSheet(string SheetName)
        {
            return wb.GetSheet(SheetName) != null;
        }

        public DataTable GetDataTableBySheet(string SheetName, int titleRow = -1)
        {
            return wb.GetSheet(SheetName).ConvertToDataTable(titleRow);
        }

        public DataTable GetDataTableBySheet(int SheetIndex, int titleRow = -1)
        {
            return wb.GetSheetAt(SheetIndex).ConvertToDataTable(titleRow);
        }

    }
}
