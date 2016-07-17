using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACool
{
    public class CheckItem
    {
        public Guid CheckItemId { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string CostItem { get; set; }
        public int MoneySum { get; set; }
    }
}
