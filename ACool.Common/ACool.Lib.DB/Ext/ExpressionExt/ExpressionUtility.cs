using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ACool.SqlServerExt.ExpressionExt
{
    public class ExpressionUtility
    {
        public static string WhereExpression<T>(Expression<Func<T, bool>> conditions, ICollection<KeyValuePair<string, object>> para)
        {
            ExpBox box = GetBox(conditions.Body);

            string sql = box.ToSQL(para);

            return sql;
        }
        public static ExpBox GetBox(Expression Exp)
        {
            ExpBox result = null;

            //Logic
            if (Exp is BinaryExpression)
            {
                BinaryExpression binaryExp = (Exp as BinaryExpression);

                if (binaryExp.NodeType == ExpressionType.AndAlso || binaryExp.NodeType == ExpressionType.OrElse)
                {
                    result = new LogicBox(binaryExp);
                }
                else
                {
                    result = CondationCheck(binaryExp);
                }
            }
            else if (Exp is MethodCallExpression)
            {
                MethodCallExpression MethodExp = (Exp as MethodCallExpression);

                result = MethodCheck(MethodExp);
            }
            else
            {
                throw new NotImplementedException();
            }

            return result;
        }

        public static ExpBox CondationCheck(BinaryExpression binaryExp)
        {
            ExpBox result = null;

            //Member
            MemberExpression member = null;

            if (binaryExp.Left is MemberExpression)
            {
                member = (binaryExp.Left as MemberExpression);
            }
            else if (binaryExp.Left is UnaryExpression)
            {
                UnaryExpression unaryExp = (binaryExp.Left as UnaryExpression);

                if (unaryExp.Operand != null && unaryExp.Operand is MemberExpression)
                {
                    member = (unaryExp.Operand as MemberExpression);
                }
            }

            if (member == null)
            {
                throw new NotImplementedException();
            }

            //Value
            object value = null;

            if (binaryExp.Right is ConstantExpression)
            {
                value = (binaryExp.Right as ConstantExpression).Value;
            }
            else if (binaryExp.Right is MethodCallExpression)
            {
                MethodCallExpression MethodExp = (binaryExp.Right as MethodCallExpression);

                if (MethodExp.Object is MemberExpression || MethodExp.Arguments.Any(x => x is MemberExpression))
                {
                    //Not Support Mehtod with Parameter
                    throw new NotImplementedException();
                }

                value = GetObjectValue(binaryExp.Right);
            }
            else if (binaryExp.Right is NewExpression)
            {
                // new DateTime()

                value = GetObjectValue(binaryExp.Right);
            }
            else if (binaryExp.Right is MemberExpression)
            {
                //MemberExpression ExpMember = binaryExp.Right as MemberExpression;
                //value = (ExpMember.Expression as ConstantExpression).Value;
            }
            else
            {
                throw new NotImplementedException();
            }

            result = new CompareCondtionBox(member, binaryExp.NodeType, value);

            return result;
        }

        public static ExpBox MethodCheck(MethodCallExpression MethodExp)
        {
            ExpBox result = null;

            Expression MemberParamter = null;

            Expression Object = null;

            switch (MethodExp.Method.Name)
            {
                case "Equals":
                    if (MethodExp.Object != null && MethodExp.Arguments.Count == 1)
                    {
                        if (MethodExp.Object is MemberExpression)
                        {
                            MemberParamter = MethodExp.Object;

                            Object = MethodExp.Arguments[0];
                        }
                        else if (MethodExp.Arguments[0] is MemberExpression)
                        {
                            MemberParamter = MethodExp.Arguments[0];

                            Object = MethodExp.Object;
                        }
          
                    }
                    else if (MethodExp.Object == null && MethodExp.Arguments.Count == 2)
                    {
                        if (MethodExp.Arguments[0] is MemberExpression)
                        {
                            MemberParamter = MethodExp.Arguments[0];

                            Object = MethodExp.Arguments[1];
                        }
                        else if (MethodExp.Arguments[1] is MemberExpression)
                        {
                            MemberParamter = MethodExp.Arguments[1];

                            Object = MethodExp.Arguments[0];
                        }
                    }

 

                    if (Object != null || MemberParamter != null && MemberParamter is MemberExpression)
                    {
                        object value = GetObjectValue(Object);

                        result = new CompareCondtionBox((MemberParamter as MemberExpression), ExpressionType.Equal, value);
                    }

                    break;
                case "Contains":

                    if (MethodExp.Object != null && MethodExp.Arguments.Count == 1)
                    {
                        MemberParamter = MethodExp.Arguments[0];

                        Object = MethodExp.Object;
                    }
                    else if (MethodExp.Object == null && MethodExp.Arguments.Count == 2)
                    {
                        MemberParamter = MethodExp.Arguments[1];

                        Object = MethodExp.Arguments[0];
                    }

                    if (Object != null || MemberParamter != null && MemberParamter is MemberExpression && typeof(IEnumerable).IsAssignableFrom(Object.Type))
                    {
                        IEnumerable value = (IEnumerable)GetObjectValue(Object);

                        result = new ContainBox((MemberParamter as MemberExpression), value);
                    }

                    break;
            }

            if (result == null)
            {
                throw new NotImplementedException();
            }

            return result;
        }

        public static object GetObjectValue(Expression Exp)
        {
            return Expression.Lambda(Exp).Compile().DynamicInvoke();
        }

        public static string ToTableFieldString(MemberExpression MemberExp)
        {
            return $"[{MemberExp.Member.ReflectedType.Name}].[{MemberExp.Member.Name}]";
        }

    }
}
