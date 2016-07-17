using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ACool.SqlServerExt.ExpressionExt
{
    public class ContainBox : ExpBox
    {
        private MemberExpression MemberExp { get; set; }
        private IEnumerable Value { get; set; }

        public ContainBox(MemberExpression memberExp, IEnumerable value)
        {
            this.MemberExp = memberExp;

            this.Value = value;
        }

        public override string ToSQL(ICollection<KeyValuePair<string, object>> para)
        {
            string result = string.Empty;

            string memberParamter = $"[{MemberExp.Member.ReflectedType.Name}].[{MemberExp.Member.Name}]";

            string parameterString = "NULL";

            if (this.Value != null)
            {
                List<string> keys = new List<string>();

                foreach (object value in this.Value)
                {
                    string key = "@para" + para.Count.ToString();

                    keys.Add(key);

                    para.Add(new KeyValuePair<string, object>(key, value));
                }

                if (keys.Count > 0)
                {
                    parameterString = string.Join(",", keys);
                }
            }
 
            result = $"{memberParamter} in ({parameterString})";

            return result;
        }
    }
}
