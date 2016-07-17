using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACool
{
    public class ExcelHelper
    {
        protected IWorkbook workbook = null;

        public SheetCollection Sheets { get; }
        private string CurrentFile { get; }

        public ExcelHelper()
        {
            workbook = new XSSFWorkbook();

            this.Sheets = new SheetCollection(this.workbook);
        }

        public ExcelHelper(string filepath)
        {
            this.CurrentFile = filepath;

            if (!File.Exists(filepath))
            {
                workbook = new XSSFWorkbook();

                Save();
            }
            else
            {
                this.workbook = WorkbookFactory.Create(filepath);
            }

            this.Sheets = new SheetCollection(this.workbook);
        }

        public void Save()
        {
            workbook.Export(this.CurrentFile);
        }
        public void Export(string filepath)
        {
            workbook.Export(filepath);
        }

    }
}
