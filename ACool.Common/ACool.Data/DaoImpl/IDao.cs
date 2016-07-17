using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ACool.Data.DaoImpl
{
    public interface IDAO
    {
        List<T> Query<T>(Expression<Func<T, bool>> conditions = null);
        T QuerySingle<T>(Expression<Func<T, bool>> conditions = null);
        List<T> QueryOrderByAscending<T, TKey>(Expression<Func<T, bool>> Filter, Expression<Func<T, TKey>> OrderConditions);
        List<T> QueryOrderByDescending<T, TKey>(Expression<Func<T, bool>> Filter, Expression<Func<T, TKey>> OrderConditions);
        void BatchInsert<T>(IEnumerable<T> entities, bool withCommit = false);

        void Insert<T>(T entity, bool withCommit = false);

        void Update<T>(Expression<Func<T, bool>> conditions = null, bool withCommit = false, params Expression<Func<T, object>>[] sets);
        void Delete<T>(Expression<Func<T, bool>> conditions = null, bool withCommit = false);
        void Commit();
        void RollBack();
    }
}
