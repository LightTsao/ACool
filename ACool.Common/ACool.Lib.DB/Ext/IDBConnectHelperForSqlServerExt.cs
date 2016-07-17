using ACool.SqlServerExt.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ACool.SqlServerExt
{
    public static class IDBConnectHelperForSqlServerExt
    {
        public static void Insert<T>(this IDBConnectHelper helper, T entity, bool withCommit = false)
        {
            SQLBox sqlBox = SqlServerInsertExt.ToInsertSqlBox(entity);

            helper.Execute(sqlBox.Sql, sqlBox.Parameters, withCommit);
        }
        public static void InsertBatch<T>(this IDBConnectHelper helper, IEnumerable<T> entities, bool withCommit = false)
        {
            List<SQLBox> sqlBoxes = SqlServerInsertExt.ToInsertSqlBoxes(entities);

            foreach (SQLBox sqlBox in sqlBoxes)
            {
                helper.Execute(sqlBox.Sql, sqlBox.Parameters, false);
            }

            if (withCommit)
            {
                helper.Commit();
            }
        }
        public static ExecuteSqlBox<T> Delete<T>(this IDBConnectHelper helper)
        {
            SQLBox sqlBox = SqlServerDeleteExt.ToDeleteSqlBox<T>();

            return new ExecuteSqlBox<T>(helper, sqlBox);
        }
        public static ExecuteSqlBox<T> Update<T>(this IDBConnectHelper helper, params Expression<Func<T, object>>[] sets)
        {
            SQLBox sqlBox = SqlServerUpdateExt.ToUpdateSqlBox<T>(sets);

            return new ExecuteSqlBox<T>(helper, sqlBox);
        }
        public static QuerySqlBox<T> Query<T>(this IDBConnectHelper helper)
        {
            return new QuerySqlBox<T>(helper);
        }
    }
}
