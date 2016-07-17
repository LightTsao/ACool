using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using System.Reflection;

namespace ACool
{
    public class ExcelEntityDBHelper
    {
        protected ExcelHelper eh = null;

        protected List<Sheet> Tables = null;

        protected int GetRowIndex<T>(T entity, string IdProperty = "Id")
        {
            Guid id = (Guid)typeof(T).GetProperty(IdProperty).GetValue(entity);

            return GetRowIndex<T>(id, IdProperty);
        }

        protected int GetRowIndex<T>(Guid id, string IdProperty = "Id")
        {
            string tableName = typeof(T).Name;

            Sheet sheet = eh.Sheets[tableName];

            int ColIndex = -1;

            for (int i = 0; i < sheet.ColCount; i++)
            {
                string colName = Convert.ToString(sheet.GetValue(0, i));

                if (colName == IdProperty)
                {
                    ColIndex = i;
                    break;
                }
            }

            int RowIndex = -1;

            for (int i = 1; i < sheet.RowCount; i++)
            {
                if (!sheet.HasValues(i))
                {
                    continue;
                }

                Guid CurrentId = new Guid(Convert.ToString(sheet.GetValue(i, ColIndex)));

                if (CurrentId.Equals(id))
                {
                    RowIndex = i;
                    break;
                }
            }


            return RowIndex;
        }

        protected T ToEntity<T>(Sheet sheet, int RowIndex)
        {
            T entity = Activator.CreateInstance<T>();

            for (int col = 0; col < sheet.ColCount; col++)
            {
                string columnName = Convert.ToString(sheet.GetValue(0, col));

                object value = sheet.GetValue(RowIndex, col);

                if (value != null)
                {
                    Type type = typeof(T).GetProperty(columnName).PropertyType;

                    typeof(T).GetProperty(columnName).SetValue(entity, Convert.ChangeType(value, type));
                }
            }

            return entity;
        }

        private Sheet CreateTable<T>()
        {
            string sheetName = typeof(T).Name;

            Sheet sheet = eh.Sheets.CreateSheet(sheetName);

            List<object> titleList = typeof(T).GetProperties().Select(x => (object)x.Name).ToList();

            sheet.SetValue(0, titleList);

            return sheet;
        }

        private Sheet GetSheet<T>()
        {
            string tableName = typeof(T).Name;

            Sheet sheet = null;

            if (eh.Sheets.Contains(tableName))
            {
                sheet = eh.Sheets[tableName];
            }
            else
            {
                sheet = CreateTable<T>();
            }

            return sheet;
        }

        public ExcelEntityDBHelper(string filepath)
        {
            eh = new ExcelHelper(filepath);

            Tables = new List<Sheet>();

            Tables.AddRange(eh.Sheets.Cast<Sheet>());
        }

        public virtual List<T> QueryAll<T>()
        {
            string tableName = typeof(T).Name;

            List<T> entities = new List<T>();

            if (eh.Sheets.Contains(tableName))
            {
                Sheet sheet = eh.Sheets[tableName];

                for (int i = 1; i < sheet.RowCount; i++)
                {
                    if (sheet.HasValues(i))
                    {
                        entities.Add(ToEntity<T>(sheet, i));
                    }
                }

            }

            return entities;
        }

        public virtual T Get<T>(Guid id)
        {
            string tableName = typeof(T).Name;

            if (eh.Sheets.Contains(tableName))
            {
                Sheet sheet = eh.Sheets[tableName];

                int RowIndex = GetRowIndex<T>(id);

                return ToEntity<T>(sheet, RowIndex);
            }

            return default(T);
        }

        public virtual T Insert<T>(T entity)
        {
            List<object> values = entity.GetValues().ToList();

            Sheet sheet = GetSheet<T>();

            sheet.SetValue(sheet.RowCount, values);

            eh.Save();

            return entity;
        }

        public virtual T Update<T>(T entity)
        {
            List<object> values = entity.GetValues().ToList();

            Sheet sheet = GetSheet<T>();

            int RowIndex = GetRowIndex(entity);

            if (RowIndex > 0)
            {
                sheet.SetValue(RowIndex, values);

                eh.Save();
            }

            return entity;
        }

        public virtual void Delete<T>(T entity)
        {
            Sheet sheet = GetSheet<T>();

            int RowIndex = GetRowIndex(entity);

            if (RowIndex > 0)
            {
                sheet.DeleteRow(RowIndex);

                eh.Save();
            }
        }

        public virtual void Delete<T>(Guid id)
        {
            Sheet sheet = GetSheet<T>();

            int RowIndex = GetRowIndex(id);

            if (RowIndex > 0)
            {
                sheet.DeleteRow(RowIndex);

                eh.Save();
            }
        }

    }
}
