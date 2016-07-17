using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ACool.Library.DB.SQL.Base
{
    public abstract class BaseQueryCreater<T> : BaseSqlCreater
    {
        public abstract void Select(Type Model);
        public abstract void Select<S>();
        public abstract void Select(params string[] fields);
        public abstract void Take(int n);
        public abstract void Where(Expression<Func<T, bool>> WhereConditions);
        public abstract void OrderByDescending<TKey>(Expression<Func<T, TKey>> OrderConditions, int PageNumber = 0, int PageSize = 0);
        public abstract void OrderByAscending<TKey>(Expression<Func<T, TKey>> OrderConditions, int PageNumber = 0, int PageSize = 0);

    }
}
