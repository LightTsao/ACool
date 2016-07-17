using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACool.Library.DB.DAO
{
    public abstract class BaseDaoRegister
    {
        public Dictionary<Type, Type> dicTypes = new Dictionary<Type, Type>();
        public abstract void Create();
        public void Register<TDAO, TImpl>() where TImpl : TDAO
        {
            if (dicTypes.ContainsKey(typeof(TDAO)))
            {
                dicTypes[typeof(TDAO)] = typeof(TImpl);
            }
            else
            {
                dicTypes.Add(typeof(TDAO), typeof(TImpl));
            }
        }
    }
}
