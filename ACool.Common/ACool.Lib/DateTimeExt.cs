using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACool.Library.DataProcess.Extension
{
    public static class DateTimeExt
    {
        public static DateTime Convert13DigitToDateTime(this long num)
        {
            DateTime dt1970 = new DateTime(1970, 1, 1);

            return dt1970.AddMilliseconds(num);
        }

        public static long To13DigitNumber(this DateTime date)
        {
            DateTime dt1970 = new DateTime(1970, 1, 1);

            return (long)(date - dt1970).TotalMilliseconds;
        }
    }
}
