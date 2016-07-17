using NPOI.SS.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACool
{
    public class SheetCollection : IEnumerable
    {
        private IWorkbook workbook = null;

        public SheetCollection(IWorkbook wb)
        {
            this.workbook = wb;
        }

        public IEnumerator GetEnumerator()
        {
            for (int i = 0; i < this.Count; i++)
            {
                yield return this[i];
            }
        }

        public int Count
        {
            get
            {
                return workbook.NumberOfSheets;
            }
        }

        public Sheet this[int sheetIndex]
        {
            get
            {
                return new Sheet(workbook.GetSheetAt(sheetIndex));
            }
        }

        public Sheet this[string sheetName]
        {
            get
            {
                return new Sheet(workbook.GetSheet(sheetName));
            }
        }

        public bool Contains(string sheetName)
        {
            return workbook.GetSheet(sheetName) != null;
        }

        public Sheet CreateSheet(string sheetName)
        {
            ISheet sheet = workbook.CreateSheet(sheetName);

            return new Sheet(sheet);
        }

        public Sheet CreateSheet(string sheetName, DataTable dt)
        {
            Sheet sheet = CreateSheet(sheetName);

            sheet.LoadDataTable(dt);

            return sheet;
        }

        public void Remove(string sheetName)
        {
            int sheetIndex = workbook.GetSheetIndex(sheetName);

            Remove(sheetIndex);
        }

        public void Remove(int sheetIndex)
        {
            workbook.RemoveSheetAt(sheetIndex);
        }

        public void Rename(string sheetName, string sheetNewName)
        {
            int sheetIndex = workbook.GetSheetIndex(sheetName);

            Rename(sheetIndex, sheetNewName);
        }

        public void Rename(int sheetIndex, string sheetNewName)
        {
            workbook.SetSheetName(sheetIndex, sheetNewName);
        }

    }
}
