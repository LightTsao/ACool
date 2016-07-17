using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACool.SqlServerExt.ExpressionExt
{
    public abstract class ExpBox
    {
        public abstract string ToSQL(ICollection<KeyValuePair<string, object>> para);
    }
}
