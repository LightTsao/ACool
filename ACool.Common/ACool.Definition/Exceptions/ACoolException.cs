using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACool.Definition.Exceptions
{
    public class ACoolException : Exception
    {
        public string Module { get; set; }
        public string Msg { get; set; }

        public ACoolException()
        {

        }

        public ACoolException(string module, string msg)
        {
            this.Module = module;

            this.Msg = msg;
        }
    }
}
