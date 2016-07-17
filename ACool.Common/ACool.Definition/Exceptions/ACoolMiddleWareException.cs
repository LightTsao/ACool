using ACool.Definition.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACool.Definition.Exceptions
{
    public class ACoolMiddleWareException : ACoolException
    {
        public ACoolMiddleWareException() : base()
        {
        }
        public ACoolMiddleWareException(string msg) : base(ModuleCode.MiddleWare, msg)
        {

        }
    }
}
