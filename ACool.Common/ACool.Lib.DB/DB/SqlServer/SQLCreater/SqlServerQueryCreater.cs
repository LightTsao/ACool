using ACool.Library.DB.SQL.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ACool.Library.DB.SQL.SqlServer
{
    public class SqlServerQueryCreater<T> : BaseQueryCreater<T>
    {
        string tableName { get; set; }

        //Select

        List<string> Fields = new List<string>();

        int Top = 0;

        //Where
        string whereConditions { get; set; }
        Dictionary<string, object> Parameter { get; set; }


        //Order By & Page By
        private string orderField { get; set; }
        private string orderAsc { get; set; }
        private int PageNumber { get; set; }
        private int PageSize { get; set; }

        public SqlServerQueryCreater()
        {
            this.tableName = typeof(T).Name;
        }
        protected override string GetSQL()
        {
            if (Fields.Count == 0)
            {
                Fields.Add("*");
            }

            string fields = string.Join(",", Fields);

            if (Top > 0)
            {
                fields = $"TOP {Top} {fields}";
            }

            string resultSql = string.Format("select {0} from [{1}]", fields, tableName);

            if (!string.IsNullOrEmpty(whereConditions))
            {
                resultSql = string.Format(
@"{0}
where
{1}
", resultSql, whereConditions);
            }

            if (!string.IsNullOrEmpty(orderField))
            {
                resultSql = string.Format(
@"{0}
order by {1} {2}
", resultSql, orderField, orderAsc);

                if (PageSize > 0 && PageNumber > 0)
                {

                    resultSql = string.Format(
@"{0}
OFFSET({1} - 1) * {2} rows
 FETCH Next {2} rows only
", resultSql, PageNumber, PageSize);


                }

            }

            return resultSql;
        }

        protected override Dictionary<string, object> GetParamters()
        {
            return Parameter;
        }


        public override void Where(Expression<Func<T, bool>> WhereConditions)
        {
            WhereSqlConverter sqlConverter = new WhereSqlConverter();

            sqlConverter.Compute<T>(WhereConditions);

            this.whereConditions = sqlConverter.sResultSql;

            this.Parameter = sqlConverter.dicResultParameter;
        }

        public override void OrderByDescending<TKey>(Expression<Func<T, TKey>> OrderConditions, int PageNumber = 0, int PageSize = 0)
        {
            MemberExpression exp = OrderConditions.Body as MemberExpression;

            orderField = "[" + exp.Member.ReflectedType.Name + "]." + exp.Member.Name;

            orderAsc = "desc";

            this.PageNumber = PageNumber;

            this.PageSize = PageSize;
        }

        public override void OrderByAscending<TKey>(Expression<Func<T, TKey>> OrderConditions, int PageNumber = 0, int PageSize = 0)
        {
            MemberExpression exp = OrderConditions.Body as MemberExpression;

            orderField = "[" + exp.Member.ReflectedType.Name + "]." + exp.Member.Name;

            orderAsc = "asc";

            this.PageNumber = PageNumber;

            this.PageSize = PageSize;
        }

        public override void Select(Type Model)
        {
            this.Fields = Model.GetProperties().Select(x => x.Name).ToList();
        }
        public override void Select(params string[] fields)
        {
            this.Fields = fields.ToList();
        }
        public override void Select<S>()
        {
            this.Fields = typeof(S).GetProperties().Select(x => x.Name).ToList();
        }
        public override void Take(int n)
        {
            this.Top = n;
        }

    }
}
