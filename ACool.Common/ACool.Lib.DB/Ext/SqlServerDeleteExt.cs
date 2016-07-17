using ACool.SqlServerExt.ExpressionExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ACool.SqlServerExt
{
    public class SqlServerDeleteExt
    {
        public static SQLBox ToDeleteSqlBox<T>()
        {
            string sql = $"DELETE FROM [{typeof(T).Name}]";

            Dictionary<string, object> paras = new Dictionary<string, object>();

            return new SQLBox(sql, paras);
        }
        
    }
}
