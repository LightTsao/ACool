using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACool
{
    public interface IDBConnectHelper
    {
        //Query
        object QueryScalar(string sql, ICollection<KeyValuePair<string, object>> paras = null);
        T QueryScalar<T>(string sql, ICollection<KeyValuePair<string, object>> paras = null);
        DataTable QueryDataTable(string sql, ICollection<KeyValuePair<string, object>> paras = null);
        IEnumerable<T> Query<T>(string sql, ICollection<KeyValuePair<string, object>> paras = null);

        //Execute
        void Execute(string sql, ICollection<KeyValuePair<string, object>> paras = null, bool withCommit = false);
        void Commit();
        void RollBack();
    }
}
