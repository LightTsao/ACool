using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ACool
{
    public class SqlServerUtility
    {
        public static string SelectSQL(string table, params string[] fields)
        {
            string field = string.Empty;

            if (fields.Count() == 0)
            {
                field = "*";
            }
            else
            {
                field = string.Join(",", fields);
            }

            return string.Format("select {0} from [{1}]", field, table);
        }

        public static string PageBySelectSQL(string SelectSQL, string OrderBy, string OrderAsc, int PageSize, int PageNumber, string primaryKey)
        {

            string sql = string.Format(@"SELECT * FROM [{0}] AS O
  INNER JOIN (SELECT * FROM [{0}] ORDER BY Name asc OFFSET (3 - 1) * 10 ROWS FETCH NEXT 10 ROWS ONLY) as K ON O.SerialNo = K.SerialNo
ORDER BY O.Name asc", SelectSQL);

            return sql;
        }

        public static string NormalPageBySelectSQL(string SelectSQL, string OrderBy, string OrderAsc, int PageSize, int PageNumber)
        {
            string sql = string.Format(
@"{0}
ORDER BY {1} {2}
OFFSET ({4} - 1) * {3} 
ROWS FETCH NEXT {3} ROWS ONLY", SelectSQL, OrderBy, OrderAsc, PageSize, PageNumber);

            return sql;
        }


        public const string GetTableInfoSQL = "SELECT * FROM INFORMATION_SCHEMA.TABLES";




    }
}
