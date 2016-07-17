using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACool.Definition.Roles
{
    [Flags]
    public enum RoleEnum
    {
        Member = 0,

        SuperAdmin = 1
    }
}
