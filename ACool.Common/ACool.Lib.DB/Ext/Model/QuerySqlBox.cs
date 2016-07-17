using ACool.SqlServerExt.ExpressionExt;
using ACool.SqlServerExt.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ACool.SqlServerExt.Model
{
    public class QuerySqlBox<T>
    {
        private IDBConnectHelper helper { get; set; }
        public QuerySqlBox(IDBConnectHelper helper)
        {
            this.helper = helper;
        }

        private string SourceSql(IEnumerable<string> fields = null, int top = 0)
        {
            string fieldsString = string.Empty;

            if (fields == null)
            {
                fieldsString = "*";
            }
            else
            {
                fieldsString = string.Join(",", fields);
            }

            if (top > 0)
            {
                fieldsString = $"TOP {top} {fieldsString}";
            }

            return $"SELECT {fieldsString} FROM [{typeof(T).Name}]";
        }
        private SQLBox GetResultSql(IEnumerable<string> fields = null, int top = 0)
        {
            string ExecuteSql = SourceSql(fields, top);

            Dictionary<string, object> ExeParameter = new Dictionary<string, object>();

            //Where

            if (!string.IsNullOrEmpty(whereSql))
            {
                ExecuteSql = $"{ExecuteSql} WHERE {whereSql}";
            }

            if (whereCondition != null)
            {
                ExeParameter = ExeParameter.Concat(whereCondition).ToDictionary(x => x.Key, x => x.Value);
            }

            //Order

            if (orderSql.Count > 0)
            {
                ExecuteSql = $"{ExecuteSql} ORDER BY {string.Join(",", orderSql)}";
            }

            return new SQLBox(ExecuteSql, ExeParameter);
        }

        //where
        private string whereSql { get; set; }
        private Dictionary<string, object> whereCondition { get; set; }

        //order
        private List<string> orderSql = new List<string>();
        public QuerySqlBox<T> Where(Expression<Func<T, bool>> conditions)
        {
            if (conditions != null)
            {
                whereCondition = new Dictionary<string, object>();

                whereSql = ExpressionUtility.WhereExpression(conditions, whereCondition);
            }

            return this;
        }

        public QuerySqlBox<T> OrderByDescending<TKey>(Expression<Func<T, TKey>> OrderConditions)
        {
            MemberExpression exp = OrderConditions.Body as MemberExpression;

            string orderField = ExpressionUtility.ToTableFieldString(OrderConditions.Body as MemberExpression);

            string orderAsc = "DESC";

            orderSql.Add($"{orderField} {orderAsc}");

            return this;
        }

        public QuerySqlBox<T> OrderByAscending<TKey>(Expression<Func<T, TKey>> OrderConditions)
        {
            MemberExpression exp = OrderConditions.Body as MemberExpression;

            string orderField = ExpressionUtility.ToTableFieldString(OrderConditions.Body as MemberExpression);

            string orderAsc = "ASC";

            orderSql.Add($"{orderField} {orderAsc}");

            return this;
        }


        // To Object
        public DataTable ToDataTable()
        {
            SQLBox sqlBox = GetResultSql();

            return helper.QueryDataTable(sqlBox.Sql, sqlBox.Parameters);
        }
        public List<T> ToEnities()
        {
            SQLBox sqlBox = GetResultSql();

            return helper.Query<T>(sqlBox.Sql, sqlBox.Parameters).ToList();
        }
        public List<S> ToEnities<S>()
        {
            SQLBox sqlBox = GetResultSql(typeof(S).GetProperties().Select(x => x.Name));

            return helper.Query<S>(sqlBox.Sql, sqlBox.Parameters).ToList();
        }
        public T ToEntity()
        {
            SQLBox sqlBox = GetResultSql(null, 1);

            return helper.Query<T>(sqlBox.Sql, sqlBox.Parameters).FirstOrDefault();
        }
        public S ToEntity<S>()
        {
            SQLBox sqlBox = GetResultSql(typeof(S).GetProperties().Select(x => x.Name), 1);

            return helper.Query<S>(sqlBox.Sql, sqlBox.Parameters).FirstOrDefault();
        }
        public List<S> SelectToList<S>(Expression<Func<T, S>> field)
        {
            string fieldName = ExpressionUtility.ToTableFieldString(field.Body as MemberExpression);

            SQLBox sqlBox = GetResultSql(new[] { fieldName });

            return helper.Query<S>(sqlBox.Sql, sqlBox.Parameters).ToList();
        }
    }
}
