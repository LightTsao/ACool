using ACool.Library.DB.SQL.Base;
using ACool.Library.DB.SQL.Extension;
using ACool.Library.DB.SQL.SqlServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ACool.Library.DB.SqlServerExt
{
    public static class SqlServerExt
    {
        public static IEnumerable<T> QueryOrderByAscending<T, TKey>(this IDBConnectHelper hepler, Expression<Func<T, bool>> Filter, Expression<Func<T, TKey>> OrderConditions)
        {
            BaseQueryCreater<T> creater = new SqlServerQueryCreater<T>();

            if (Filter != null)
            {
                creater.Where(Filter);
            }

            creater.OrderByAscending(OrderConditions);

            SQLBox sqlBox = creater.ToSqlBox();

            return hepler.Query<T>(sqlBox.Sql, sqlBox.Parameters);
        }

        public static IEnumerable<T> QueryOrderByDescending<T, TKey>(this IDBConnectHelper hepler, Expression<Func<T, bool>> Filter, Expression<Func<T, TKey>> OrderConditions)
        {
            BaseQueryCreater<T> creater = new SqlServerQueryCreater<T>();

            if (Filter != null)
            {
                creater.Where(Filter);
            }

            creater.OrderByDescending(OrderConditions);

            SQLBox sqlBox = creater.ToSqlBox();

            return hepler.Query<T>(sqlBox.Sql, sqlBox.Parameters);
        }

        public static IEnumerable<T> Query<T>(this IDBConnectHelper hepler, Expression<Func<T, bool>> Filter = null)
        {
            BaseQueryCreater<T> creater = new SqlServerQueryCreater<T>();

            if (Filter != null)
            {
                creater.Where(Filter);
            }

            SQLBox sqlBox = creater.ToSqlBox();

            return hepler.Query<T>(sqlBox.Sql, sqlBox.Parameters);
        }
        public static T QuerySingle<T>(this IDBConnectHelper hepler, Expression<Func<T, bool>> conditions = null)
        {
            BaseQueryCreater<T> queryCreater = new SqlServerQueryCreater<T>();

            if (conditions != null)
            {
                queryCreater.Where(conditions);
            }

            queryCreater.Take(1);

            SQLBox sqlBox = queryCreater.ToSqlBox();

            return hepler.Query<T>(sqlBox.Sql, sqlBox.Parameters).FirstOrDefault();
        }
        public static IEnumerable<Dictionary<string, object>> QueryDictionary(this IDBConnectHelper helper, string sql, ICollection<KeyValuePair<string, object>> paras = null)
        {
            IEnumerable<Dictionary<string, object>> entities = helper.Query<object>(sql, paras).Select(x => ((ICollection<KeyValuePair<string, object>>)x).ToDictionary(y => y.Key, y => y.Value));

            return entities;
        }
        public static T Insert<T>(this IDBConnectHelper hepler, T entity, bool withCommit = false)
        {
            SQLBox sqlBox = entity.ToInsertSqlBox();

            hepler.Execute(sqlBox.Sql, sqlBox.Parameters, false);

            if (withCommit)
            {
                hepler.Commit();
            }

            return entity;
        }
        public static void BatchInsert<T>(this IDBConnectHelper hepler, IEnumerable<T> entities, bool withCommit = false)
        {
            if (entities.Count() == 0)
            {
                return;
            }
            
            List<SQLBox> sqlBoxes = entities.ConvertToInsertSqlBoxList();

            foreach (SQLBox sqlBox in sqlBoxes)
            {
                hepler.Execute(sqlBox.Sql, sqlBox.Parameters, false);
            }

            if (withCommit)
            {
                hepler.Commit();
            }
        }

        public static void Update<T>(this IDBConnectHelper hepler, Expression<Func<T, bool>> conditions = null, bool withCommit = false, params Expression<Func<T, object>>[] sets)
        {
            if (sets.Count() == 0)
            {
                return;
            }

            List<string> setSqls = new List<string>();

            Dictionary<string, object> paras = new Dictionary<string, object>();

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


            string resultSql = $"UPDATE [{typeof(T).Name}] SET {string.Join(",", setSqls)}";


            if (conditions != null)
            {
                WhereSqlConverter sqlConverter = new WhereSqlConverter();

                sqlConverter.Compute<T>(conditions);

                resultSql = string.Format(
@"{0}
where
{1}
", resultSql, sqlConverter.sResultSql);

                foreach (KeyValuePair<string, object> item in sqlConverter.dicResultParameter)
                {
                    paras.Add(item.Key, item.Value);
                }
            }

            hepler.Execute(resultSql, paras, false);

            if (withCommit)
            {
                hepler.Commit();
            }
        }

        public static void Delete<T>(this IDBConnectHelper hepler, Expression<Func<T, bool>> conditions = null, bool withCommit = false)
        {
            string resultSql = string.Format("DELETE FROM [{0}]", typeof(T).Name);

            Dictionary<string, object> paras = null;

            if (conditions != null)
            {
                WhereSqlConverter sqlConverter = new WhereSqlConverter();

                sqlConverter.Compute<T>(conditions);

                resultSql = string.Format(
@"{0}
where
{1}
", resultSql, sqlConverter.sResultSql);

                paras = sqlConverter.dicResultParameter;
            }

            hepler.Execute(resultSql, paras, false);

            if (withCommit)
            {
                hepler.Commit();
            }
        }
    }
}
