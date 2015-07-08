using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Currency.BusinessEntities;

namespace Currency.Helpers
{
    public static class AbbreviationHelper
    {
        public static Abbreviation ToAbbreviation(string abbreviation)
        {
            Abbreviation result = Abbreviation.NONE;
            Enum.TryParse<Abbreviation>(abbreviation.ToUpper(), out abbr);
            
            return result;
        }
    }
}
