using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ACool.Library.File.Utility
{
    public class SearchAssemblyUtility
    {
        public static List<string> SearchDllFilePaths(string folder)
        {
            List<string> dlls = new List<string>();

            dlls.AddRange(Directory.GetFiles(folder, "*.dll"));
            dlls.AddRange(Directory.GetFiles(folder, "*.exe"));

            return dlls;
        }

        public static List<Type> GetAssemblies(string folder, Func<Type, bool> FilterCondition = null)
        {
            List<Type> itypes = new List<Type>();

            foreach (string dll in SearchDllFilePaths(folder))
            {
                try
                {
                    IEnumerable<Type> types = Assembly.LoadFrom(dll).ExportedTypes;

                    foreach (var type in types)
                    {
                        if (FilterCondition == null || (FilterCondition != null && FilterCondition(type)))
                        {
                            itypes.Add(type);
                        }
                    }
                }
                catch
                {
                    continue;
                }
            }

            return itypes;
        }
    }
}
