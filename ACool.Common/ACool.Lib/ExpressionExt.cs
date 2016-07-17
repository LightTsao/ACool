using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ACool
{
    public static class Extentions
    {
        //public static IEnumerable<TSource> Where<TSource>(this Repo<TSource> source, Expression<Func<TSource, bool>> predicate)
        //{
        //    // hacks all the way
        //    dynamic operation = predicate.Body;
        //    dynamic left = operation.Left;
        //    dynamic right = operation.Right;

        //    var ops = new Dictionary<ExpressionType, String>();
        //    ops.Add(ExpressionType.Equal, "=");
        //    ops.Add(ExpressionType.GreaterThan, ">");
        //    // add all required operations here            

        //    // Instead of SELECT *, select all required fields, since you know the type
        //    var q = String.Format("SELECT * FROM {0} WHERE {1} {2} {3}", typeof(TSource), left.Member.Name, ops[operation.NodeType], right.Value);
        //    return source.RunQuery(q);
        //}

        public static Expression<Func<T, bool>> strToFunc<T>(string propName, string opr, string value, Expression<Func<T, bool>> expr = null)
        {
            Expression<Func<T, bool>> func = null;
            try
            {
                var prop = typeof(T).GetProperty(propName);
                ParameterExpression tpe = Expression.Parameter(typeof(T));
                Expression left = Expression.Property(tpe, prop);
                Expression right = Expression.Convert(ToExprConstant(prop, value), prop.PropertyType);
                Expression<Func<T, bool>> innerExpr = Expression.Lambda<Func<T, bool>>(ApplyFilter(opr, left, right), tpe);
                if (expr != null)
                    innerExpr = innerExpr.And(expr);
                func = innerExpr;
            }
            catch { }
            return func;
        }
        private static Expression ToExprConstant(PropertyInfo prop, string value)
        {
            object val = null;
            switch (prop.PropertyType.Name)
            {
                case "System.Guid":
                    val = new Guid(value);
                    break;
                default:
                    val = Convert.ChangeType(value, prop.PropertyType);
                    break;
            }
            return Expression.Constant(val);
        }



        public static Func<Expression, Expression, BinaryExpression> ToExpression(this string opr)
        {
            Func<Expression, Expression, BinaryExpression> InnerLambda = null;
            switch (opr)
            {
                case "==":
                case "=":
                    InnerLambda = Expression.Equal;
                    break;
                case "<":
                    InnerLambda = Expression.LessThan;
                    break;
                case ">":
                    InnerLambda = Expression.GreaterThan;
                    break;
                case ">=":
                    InnerLambda = Expression.GreaterThanOrEqual;
                    break;
                case "<=":
                    InnerLambda = Expression.LessThanOrEqual;
                    break;
                case "!=":
                    InnerLambda = Expression.NotEqual;
                    break;
                case "&&":
                    InnerLambda = Expression.And;
                    break;
                case "||":
                    InnerLambda = Expression.Or;
                    break;
            }
            return InnerLambda;
        }
        private static BinaryExpression ApplyFilter(string opr, Expression left, Expression right)
        {
            BinaryExpression InnerLambda = null;
            switch (opr)
            {
                case "==":
                case "=":
                    InnerLambda = Expression.Equal(left, right);
                    break;
                case "<":
                    InnerLambda = Expression.LessThan(left, right);
                    break;
                case ">":
                    InnerLambda = Expression.GreaterThan(left, right);
                    break;
                case ">=":
                    InnerLambda = Expression.GreaterThanOrEqual(left, right);
                    break;
                case "<=":
                    InnerLambda = Expression.LessThanOrEqual(left, right);
                    break;
                case "!=":
                    InnerLambda = Expression.NotEqual(left, right);
                    break;
                case "&&":
                    InnerLambda = Expression.And(left, right);
                    break;
                case "||":
                    InnerLambda = Expression.Or(left, right);
                    break;
            }
            return InnerLambda;
        }
        public static Expression<Func<T, TResult>> And<T, TResult>(this Expression<Func<T, TResult>> expr1, Expression<Func<T, TResult>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, TResult>>(Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
        }
        public static Func<T, TResult> ExpressionToFunc<T, TResult>(this Expression<Func<T, TResult>> expr)
        {
            return expr.Compile();
        }
    }
}
