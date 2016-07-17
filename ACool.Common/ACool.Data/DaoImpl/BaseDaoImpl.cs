using ACool.SqlServerExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ACool.Data.DaoImpl
{
    public abstract class BaseDaoImpl: IDAO
    {
        IDBConnectHelper db = Factory.GetDBHelper();
        public List<T> Query<T>(Expression<Func<T, bool>> conditions = null)
        {
            return db.Query<T>().Where(conditions).ToEnities();
        }
        public List<T> QueryOrderByAscending<T, TKey>(Expression<Func<T, bool>> Filter, Expression<Func<T, TKey>> OrderConditions)
        {
            return db.Query<T>().Where(Filter).OrderByAscending(OrderConditions).ToEnities();
        }
        public List<T> QueryOrderByDescending<T, TKey>(Expression<Func<T, bool>> Filter, Expression<Func<T, TKey>> OrderConditions)
        {
            return db.Query<T>().Where(Filter).OrderByDescending(OrderConditions).ToEnities();
        }
        public T QuerySingle<T>(Expression<Func<T, bool>> conditions = null)
        {
            return db.Query<T>().Where(conditions).ToEntity();
        }
        public void BatchInsert<T>(IEnumerable<T> entities, bool withCommit = false)
        {
            db.InsertBatch<T>(entities, withCommit);
        }
        public void Insert<T>(T entity, bool withCommit = false)
        {
            db.Insert<T>(entity, withCommit);
        }
        public void Update<T>(Expression<Func<T, bool>> conditions = null, bool withCommit = false, params Expression<Func<T, object>>[] sets)
        {
            db.Update<T>(sets).Where(conditions).Execute(withCommit);
        }
        public void Delete<T>(Expression<Func<T, bool>> conditions = null, bool withCommit = false)
        {
            db.Delete<T>().Where(conditions).Execute(withCommit);
        }
        public void Commit()
        {
            db.Commit();
        }
        public void RollBack()
        {
            db.RollBack();
        }
    }
}
