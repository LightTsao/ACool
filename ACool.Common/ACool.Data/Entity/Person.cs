using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACool.Entity
{
    public class Person
    {
        public Guid PersonId { get; set; }
        public Guid? UserId { get; set; }
        public string NickName { get; set; }
        public string Picture { get; set; }
    }
}
