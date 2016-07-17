using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACool.Library.DataProcess.Extension
{
    public static class EnumExt
    {
        public static List<string> GetStrings(this Type enumType)
        {
            List<string> result = new List<string>();

            foreach (string item in Enum.GetValues(enumType))
            {
                result.Add(item);
            }

            return result;
        }

        public static int FlagEnumJoin(this Type enumType, IEnumerable<string> enums)
        {
            int result = 0;

            foreach (var item in Enum.GetValues(enumType))
            {
                if (enums.Contains(item.ToString()))
                {
                    result |= (int)item;
                }
            }

            return result;
        }
    }
}
