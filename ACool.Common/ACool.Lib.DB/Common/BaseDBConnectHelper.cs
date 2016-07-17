using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACool
{
    public abstract class BaseDBConnectHelper
    {
        protected Func<IDbConnection> CreateConnection { get; set; }
        
        public BaseDBConnectHelper(Func<IDbConnection> CreateConnection)
        {
            this.CreateConnection = CreateConnection;
        }

        //Execute

        public abstract int ExecuteSingle(SQLBox sql);
        public abstract void ExecuteBatch(List<SQLBox> sqls);

        protected List<SQLBox> sqls = new List<SQLBox>();
        public void Execute(string sql, ICollection<KeyValuePair<string, object>> paras = null, bool withCommit = false)
        {
            sqls.Add(new SQLBox(sql, paras));

            if (withCommit)
            {
                Commit();
            }
        }
        public void Commit()
        {
            if (this.sqls.Count == 0)
            {
                return;
            }
            else if (this.sqls.Count == 1)
            {
                this.ExecuteSingle(this.sqls.FirstOrDefault());
            }
            else
            {
                ExecuteBatch(this.sqls);
            }

            RollBack();
        }
        public void RollBack()
        {
            this.sqls.Clear();
        }

        

    }
}
