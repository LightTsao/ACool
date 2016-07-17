using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ACool.Library.DB.SQL.SqlServer
{
    public class WhereSqlConverter
    {
        Dictionary<ExpressionType, string> dicMiddleSql = new Dictionary<ExpressionType, string>()
        {
            [ExpressionType.AndAlso] = "and",
            [ExpressionType.OrElse] = "or",
            [ExpressionType.Equal] = "=",
            [ExpressionType.NotEqual] = "!=",
            [ExpressionType.GreaterThan] = ">",
            [ExpressionType.GreaterThanOrEqual] = ">=",
            [ExpressionType.LessThan] = "<",
            [ExpressionType.LessThanOrEqual] = "<="
        };

        public string sResultSql { get; set; }

        public Dictionary<string, object> dicResultParameter = new Dictionary<string, object>();

        public WhereSqlConverter()
        {

        }

        public void Compute<T>(Expression<Func<T, bool>> conditions)
        {
            sResultSql = ToSQL(conditions.Body);
        }

        private string AddValue(Expression exp)
        {
            string result = string.Empty;

            object value = Expression.Lambda(exp).Compile().DynamicInvoke();

            if (value != null)
            {
                result = "@para" + dicResultParameter.Count.ToString();

                dicResultParameter.Add(result, value);
            }

            return result;
        }

        private string BinaryExpressionToSql(BinaryExpression exp)
        {
            string result = string.Empty;

            Expression left = exp.Left;
            Expression right = exp.Right;


            string middleSql = string.Empty;

            if (dicMiddleSql.ContainsKey(exp.NodeType))
            {
                middleSql = dicMiddleSql[exp.NodeType];
            }
            else
            {
                throw new Exception();
            }

            string leftSql = ToSQL(left);
            string rightSql = ToSQL(right);

            result = string.Format("({0}) {2} ({1})", leftSql, rightSql, middleSql);

            return result;
        }

        private string MemberExpressionToSql(MemberExpression exp)
        {
            string result = string.Empty;

            if (exp.Expression is ParameterExpression)
            {
                result = "[" + exp.Member.ReflectedType.Name + "].[" + exp.Member.Name + "]";
            }
            else if (exp.Expression is ConstantExpression)
            {
                result = AddValue(exp);
            }
            else if (exp.Type == typeof(Guid) || exp.Type == typeof(Nullable<Guid>))
            {
                result = AddValue(exp);
            }
            else if (exp.Type == typeof(DateTime) || exp.Type == typeof(Nullable<DateTime>))
            {
                result = AddValue(exp);
            }
            else if (exp.Type == typeof(String))
            {
                result = AddValue(exp);
            }

            Type type = Nullable.GetUnderlyingType(typeof(DateTime));

            return result;
        }

        private string NewExpressionToSql(NewExpression exp)
        {
            string result = string.Empty;

            if (exp.Arguments.Any(x => x is ParameterExpression))
            {
                //x=> new XXX(x)
                throw new Exception();
            }
            else
            {
                result = AddValue(exp);
            }

            return result;
        }

        private string MethodCallExpressionToSql(MethodCallExpression exp)
        {
            string result = string.Empty;

            if (exp.Object is ParameterExpression || exp.Arguments.Any(x => x is ParameterExpression) || (exp.Object is MemberExpression && ((MemberExpression)exp.Object).Expression is ParameterExpression))
            {
                //Not Support Mehtod with Parameter
                throw new Exception();
            }
            else if (exp.Object is ConstantExpression)
            {
                result = AddValue(exp);
            }
            else if (exp.Object != null && exp.Object.Type.IsEnum && exp.Method.Name == "ToString")
            {
                result = AddValue(exp);
            }
            else if (exp.Method.Name == "Contains")
            {
                /// Where(x=> XXX.Contains(x.P))
                if (exp.Object != null && exp.Arguments.Count == 1 && exp.Arguments[0] is MemberExpression && typeof(IEnumerable).IsAssignableFrom(exp.Object.Type))
                {
                    object value = Expression.Lambda(exp.Object).Compile().DynamicInvoke();

                    string paraName = "@para" + dicResultParameter.Count.ToString();

                    dicResultParameter.Add(paraName, value);

                    result = ToSQL(exp.Arguments[0]) + " in " + paraName;
                }
                else if (exp.Object == null && exp.Arguments.Count == 2 && exp.Arguments[1] is MemberExpression && typeof(IEnumerable).IsAssignableFrom(exp.Arguments[0].Type))
                {
                    object value = Expression.Lambda(exp.Arguments[0]).Compile().DynamicInvoke();

                    string paraName = "@para" + dicResultParameter.Count.ToString();

                    dicResultParameter.Add(paraName, value);

                    result = ToSQL(exp.Arguments[1]) + " in " + paraName;
                }
            }
            else
            {

            }

            return result;
        }


        private string ToSQL(Expression exp)
        {
            string result = string.Empty;

            if (exp is BinaryExpression)
            {
                result = BinaryExpressionToSql(exp as BinaryExpression);
            }
            else if (exp is MemberExpression)
            {
                result = MemberExpressionToSql(exp as MemberExpression);
            }
            else if (exp is NewExpression)
            {
                result = NewExpressionToSql(exp as NewExpression);
            }
            else if (exp is MethodCallExpression)
            {
                result = MethodCallExpressionToSql(exp as MethodCallExpression);
            }
            else if (exp is UnaryExpression)
            {
                if (((UnaryExpression)exp).Operand is MemberExpression)
                {
                    result = ToSQL(((UnaryExpression)exp).Operand);
                }
                else
                {
                    result = AddValue(exp);
                }
            }
            else if (exp is ConstantExpression)
            {
                result = AddValue(exp);
            }
            else
            {

            }


            //dynamic body = exp;

            //if (exp.NodeType == ExpressionType.MemberAccess)
            //{
            //    if (((Expression)body.Expression).NodeType == ExpressionType.Parameter)
            //    {
            //        //result = "[" + body.Member.ReflectedType.Name + "]." + body.Member.Name;
            //    }
            //    else
            //    {
            //        result = AddValue(exp);
            //    }
            //}
            //else if (exp.NodeType == ExpressionType.Call)
            //{
            //    if (body.Object is ConstantExpression)
            //    {
            //        result = AddValue(exp);
            //    }
            //    else
            //    {
            //        if (((Expression)body.Object.Expression).NodeType == ExpressionType.Parameter)
            //        {

            //        }
            //        else
            //        {

            //        }
            //    }

            //}



            return result;
        }
    }
}
