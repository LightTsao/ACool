using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Generic
{
    public static class DictionaryExt
    {
        public static void AddOrReplace<TKEY, TVALUE>(this Dictionary<TKEY, TVALUE> dic, TKEY key, TVALUE value)
        {
            if (dic.ContainsKey(key))
            {
                dic[key] = value;
            }
            else
            {
                dic.Add(key, value);
            }
        }
    }
}
