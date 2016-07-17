using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACool
{
    public class TradeItem
    {
        public Guid TradeItemId { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string CostItem { get; set; }
        public int CostMoney { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
    }
}
