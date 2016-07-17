using ACool.Library.File.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACool.Library.File.Extension
{
    public static class SearchAssemblyExt
    {
        public static List<Type> GetImplClassFromInterface(this Type type, string folder = "./")
        {
            if (type.IsInterface)
            {
                return SearchAssemblyUtility.GetAssemblies(folder, x => x.IsClass && type.IsAssignableFrom(x));
            }

            return null;
        }

        public static List<Type> GetExtendClassFromClass(this Type type, string folder = "./")
        {
            if (type.IsClass)
            {
                return SearchAssemblyUtility.GetAssemblies(folder, x => x.IsClass && x.IsSubclassOf(type));
            }

            return null;
        }

        public static List<Type> GetImplInterfaceFromInterface(this Type type, string folder = "./")
        {
            if (type.IsInterface)
            {
                return SearchAssemblyUtility.GetAssemblies(folder, x => x.IsInterface && type.IsAssignableFrom(x));
            }

            return null;
        }
    }
}
