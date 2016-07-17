using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACool.MiddleWare.Auth.Data.Entity
{
    public class Permission
    {
        public Guid PermissionId { get; set; }
        public string PermissionCode { get; set; }
    }
}
