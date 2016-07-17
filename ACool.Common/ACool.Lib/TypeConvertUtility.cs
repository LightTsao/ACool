using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACool
{
    public class TypeConvertUtility
    {
        public static object ChangeType(object value, Type type)
        {
            object result = null;

            if (type.IsEnum)
            {
                result = Enum.Parse(type, Convert.ToString(value));
            }
            else if (type == typeof(Guid))
            {
                Guid id;

                result = Guid.TryParse(Convert.ToString(value), out id) ? id : Guid.Empty;
            }
            else if (type == typeof(DateTime))
            {
                DateTime date;

                result = DateTime.TryParse(Convert.ToString(value), out date) ? date : new DateTime();
            }
            else if (type == typeof(object))
            {
                result = value;
            }
            else
            {
                result = Convert.ChangeType(value, type);
            }

            return result;
        }
    }
}
