using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ACool
{
    public class CreateTableSQLUtility
    {

        public static string GetCreateTableString(Type type)
        {
            List<string> lstFields = new List<string>();

            foreach (var p in type.GetProperties())
            {
                string fieldString = GetFieldString(p, true);

                lstFields.Add(fieldString);
            }

            //lstFields.Add("PRIMARY KEY (" + DBdef.SerialNo + ")");

            string fields = string.Join(",\r\n", lstFields.ToArray());


            return string.Format("CREATE TABLE [dbo].[{0}](\r\n{1}\r\n)", type.Name, fields);
        }

        private static string GetFieldString(PropertyInfo p, bool allowNull = true, int StringLength = 200, bool UseChinese = false)
        {
            string fieldtype = string.Empty;

            string field = p.Name;

            if (p.PropertyType.Equals(typeof(string)))
            {
                if (UseChinese)
                {
                    fieldtype = "nvarchar";
                }
                else
                {
                    fieldtype = "varchar";
                }

                fieldtype = "[" + fieldtype + "](" + StringLength.ToString() + ")";
            }
            else
            {
                if (p.PropertyType.Equals(typeof(int)))
                {
                    fieldtype = "int";
                }

                else if (p.PropertyType.Equals(typeof(Guid)))
                {
                    fieldtype = "uniqueidentifier";
                }
                else if (p.PropertyType.Equals(typeof(DateTime)))
                {
                    fieldtype = "datetime";
                }
                else
                {

                }

                fieldtype = "[" + fieldtype + "]";
            }

            return string.Format("[{0}] {1} {2} NULL", field, fieldtype, allowNull ? "" : "NOT");
        }
    }
}
