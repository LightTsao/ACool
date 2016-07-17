using ACool.SqlServerExt.ExpressionExt;
using ACool.SqlServerExt.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ACool.SqlServerExt.Model
{
    public class ExecuteSqlBox<T>
    {
        private IDBConnectHelper helper { get; set; }
        private SQLBox sqlBox { get; set; }
        public ExecuteSqlBox(IDBConnectHelper helper, SQLBox box)
        {
            this.helper = helper;

            this.sqlBox = box;
        }
        public ExecuteSqlBox<T> Where(Expression<Func<T, bool>> conditions)
        {
            if (conditions != null)
            {
                sqlBox.Sql = $"{sqlBox.Sql} WHERE {ExpressionUtility.WhereExpression(conditions, sqlBox.Parameters)}";
            }

            return this;
        }

        public void Execute(bool withCommit = false)
        {
            helper.Execute(sqlBox.Sql, sqlBox.Parameters, withCommit);
        }
    }
}
