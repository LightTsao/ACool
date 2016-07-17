using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACool
{
    public class DapperLinker : BaseDBConnectHelper, IDBConnectHelper
    {
        public DapperLinker(Func<IDbConnection> CreateConnection) : base(CreateConnection) { }

        //Query
        public DataTable QueryDataTable(string sql, ICollection<KeyValuePair<string, object>> paras = null)
        {
            DataTable dt = new DataTable();

            using (IDbConnection conn = CreateConnection())
            {
                IDataReader idr = conn.ExecuteReader(sql, paras);

                dt.Load(idr);
            }
            return dt;
        }
        public IEnumerable<T> Query<T>(string sql, ICollection<KeyValuePair<string, object>> paras = null)
        {
            using (IDbConnection conn = CreateConnection())
            {
                return conn.Query<T>(sql, paras);
            }
        }
        public object QueryScalar(string sql, ICollection<KeyValuePair<string, object>> paras = null)
        {
            using (IDbConnection conn = CreateConnection())
            {
                return conn.ExecuteScalar(sql, paras);
            }
        }
        public T QueryScalar<T>(string sql, ICollection<KeyValuePair<string, object>> paras = null)
        {
            using (IDbConnection conn = CreateConnection())
            {
                return conn.ExecuteScalar<T>(sql, paras);
            }
        }

        //Execute
        public override int ExecuteSingle(SQLBox sqlBox)
        {
            using (IDbConnection conn = CreateConnection())
            {
                return conn.Execute(sqlBox.Sql, sqlBox.Parameters);
            }
        }
        public override void ExecuteBatch(List<SQLBox> sqls)
        {
            using (IDbConnection conn = CreateConnection())
            {
                conn.Open();

                using (IDbTransaction trx = conn.BeginTransaction())
                {
                    foreach (SQLBox sqlBox in sqls)
                    {
                        conn.Execute(sqlBox.Sql, sqlBox.Parameters, trx);
                    }

                    trx.Commit();
                }
            }
        }
    }
}
