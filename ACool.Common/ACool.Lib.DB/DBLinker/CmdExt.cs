using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACool
{
    public static class CmdExt
    {
        public static void AddParameters(this IDbCommand cmd, ICollection<KeyValuePair<string, object>> paras)
        {
            foreach (KeyValuePair<string, object> p in paras)
            {
                IDbDataParameter paraMeter = cmd.CreateParameter();

                paraMeter.ParameterName = p.Key;
                paraMeter.Value = p.Value;

                cmd.Parameters.Add(paraMeter);
            }
        }

        public static int ExecuteSqlBox(this IDbCommand cmd, SQLBox sqlBox)
        {
            cmd.CommandText = sqlBox.Sql;

            if (sqlBox.Parameters != null)
            {
                cmd.AddParameters(sqlBox.Parameters);
            }

            int result  = cmd.ExecuteNonQuery();

            cmd.Parameters.Clear();

            return result;
        }
    }
}
