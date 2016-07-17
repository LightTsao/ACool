using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACool
{
    public class SQLBox
    {
        public string Sql { get; set; }
        public ICollection<KeyValuePair<string, object>> Parameters { get; set; }
        public SQLBox(string sql, ICollection<KeyValuePair<string, object>> Parameters = null)
        {
            this.Sql = sql;

            this.Parameters = Parameters;
        }
    }
}
