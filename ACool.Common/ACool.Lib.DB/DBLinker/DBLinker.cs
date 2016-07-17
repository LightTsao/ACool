using ACool;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACool
{
    public class DBLinker : BaseDBConnectHelper, IDBConnectHelper
    {
        public DBLinker(Func<IDbConnection> CreateConnection) : base(CreateConnection) { }

        //Query
        private void QueryReader(string sql, ICollection<KeyValuePair<string, object>> paras, Action<IDbCommand> QueryReader)
        {
            using (IDbConnection conn = CreateConnection())
            {
                conn.Open();

                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;

                    if (paras != null)
                    {
                        cmd.AddParameters(paras);
                    }

                    if (cmd != null)
                    {
                        QueryReader(cmd);
                    }
                }
            }
        }
        public DataTable QueryDataTable(string sql, ICollection<KeyValuePair<string, object>> paras = null)
        {
            DataTable dt = new DataTable();

            QueryReader(sql, paras,
                (cmd) =>
              {
                  IDataReader idr = cmd.ExecuteReader();

                  dt.Load(idr);
              });

            return dt;
        }
        public IEnumerable<T> Query<T>(string sql, ICollection<KeyValuePair<string, object>> paras = null)
        {
            List<T> entities = new List<T>();

            QueryReader(sql, paras, (cmd) =>
            {
                IDataReader idr = cmd.ExecuteReader();

                entities = idr.ToEntities<T>();
            });

            return entities;
        }
        public object QueryScalar(string sql, ICollection<KeyValuePair<string, object>> paras = null)
        {
            object result = null;

            QueryReader(sql, paras,
                (cmd) =>
                {
                    result = cmd.ExecuteScalar();
                }
                );

            return result;
        }
        public T QueryScalar<T>(string sql, ICollection<KeyValuePair<string, object>> paras = null)
        {
            T result = default(T);

            QueryReader(sql, paras,
                (cmd) =>
                {
                    result = (T)Convert.ChangeType(cmd.ExecuteScalar(), typeof(T));
                }
                );

            return result;
        }

        // Execute

        public void ExecuteAction(Action<IDbCommand> CmdAction)
        {
            using (IDbConnection conn = CreateConnection())
            {
                conn.Open();

                using (IDbCommand cmd = conn.CreateCommand())
                {
                    using (IDbTransaction trx = conn.BeginTransaction())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Transaction = trx;

                        if (cmd != null)
                        {
                            CmdAction(cmd);
                        }

                        trx.Commit();
                    }
                }
            }
        }

        public override int ExecuteSingle(SQLBox sqlBox)
        {
            int result = 0;

            ExecuteAction((cmd) =>
            {
                result = cmd.ExecuteSqlBox(sqlBox);
            });

            return result;
        }
        public override void ExecuteBatch(List<SQLBox> sqlBoxes)
        {
            ExecuteAction((cmd) =>
            {
                foreach (SQLBox sqlBox in sqls)
                {
                    cmd.ExecuteSqlBox(sqlBox);
                }
            });
        }
    }


}
