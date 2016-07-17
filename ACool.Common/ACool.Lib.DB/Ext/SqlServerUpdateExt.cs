using ACool.SqlServerExt.ExpressionExt;
using ACool.SqlServerExt.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ACool.SqlServerExt
{
    public class SqlServerUpdateExt
    {
        public static SQLBox ToUpdateSqlBox<T>(IEnumerable<Expression<Func<T, object>>> sets)
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();

            List<string> setSqls = ConvertSetSqls<T>(sets, paras);

            string sql = $"UPDATE [{typeof(T).Name}] SET {string.Join(",", setSqls)}";

            return new SQLBox(sql, paras);
        }

        private static List<string> ConvertSetSqls<T>(IEnumerable<Expression<Func<T, object>>> sets, Dictionary<string, object> paras)
        {
            List<string> setSqls = new List<string>();

            foreach (Expression<Func<T, object>> set in sets)
            {
                Expression exp = set.Body;

                if (exp is UnaryExpression)
                {
                    UnaryExpression expBinary = exp as UnaryExpression;

                    string parameterName = ((MemberExpression)((BinaryExpression)expBinary.Operand).Left).Member.Name;

                    object value = Expression.Lambda((((BinaryExpression)expBinary.Operand).Right)).Compile().DynamicInvoke();

                    setSqls.Add($"{parameterName} = @{parameterName}");

                    paras.Add($"@{parameterName}", value);
                }
            }

            return setSqls;
        }

    }
}
