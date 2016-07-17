using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACool.Library.DB.DAO
{
    public class DaoFactory<T> where T : BaseDaoRegister
    {
        public static TDAO GetInstance<TDAO>()
        {
            BaseDaoRegister reister = Activator.CreateInstance<T>();

            reister.Create();

            if (reister.dicTypes.ContainsKey(typeof(TDAO)))
            {
                Type Impl = reister.dicTypes[typeof(TDAO)];

                return (TDAO)Activator.CreateInstance(Impl);
            }

            return default(TDAO);
        }
    }
}
