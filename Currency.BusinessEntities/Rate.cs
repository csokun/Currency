using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Currency.BusinessEntities
{
    public class Rate
    {
        public string QuotName { get; set; }
        public int Scale { get; set; }
        public double OfficialRate { get; set; }
        public string Code { get; set; }
        public Abbreviation Abbreviation { get; set; }
        public DateTime Date { get; set; }
    }
}
