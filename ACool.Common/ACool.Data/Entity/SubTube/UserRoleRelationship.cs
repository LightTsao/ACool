using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACool.MiddleWare.Auth.Data.Entity
{
    public class UserRoleRelationship
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
    }
}
