using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACool.Library.DB.SQL.Base
{
    public abstract class BaseSqlCreater
    {
        protected Dictionary<Type, string> TableMap { get; set; }
        protected abstract string GetSQL();
        protected abstract Dictionary<string, object> GetParamters();
        public SQLBox ToSqlBox()
        {
            string sql = GetSQL();

            Dictionary<string, object> paras = GetParamters();

            return new SQLBox(sql, paras);
        }
    }
}
