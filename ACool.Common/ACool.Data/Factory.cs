using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACool
{
    public class Factory
    {
        public static IDBConnectHelper GetDBHelper(string connString = null)
        {
            Func<IDbConnection> sqlServerToLocalDB = () =>
            {
                IDbConnection conn = new SqlConnection();

                conn.ConnectionString = connString ?? LocalDB.DBConnString;

                return conn;
            };

            return new DBLinker(sqlServerToLocalDB);
        }
    }
}
