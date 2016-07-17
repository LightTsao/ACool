using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACool.Definition.Permissions
{
    [Flags]
    public enum PermissionEnum
    {
        NoPermission = 0,
        SuperAdmin = 1
    }
}
