using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACool.Windows
{
    public static class WindowSystemPath
    {
        public static string MyDocuments
        {
            get
            {
                return System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            }
        }
    }
}
