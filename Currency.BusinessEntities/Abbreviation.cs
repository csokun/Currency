using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Currency.BusinessEntities
{
    [Flags]
    public enum Abbreviation
    {
        NONE = 0,
        AUD = 1 << 1,
        BGN = 1 << 2,
        UAH = 1 << 3,
        DKK = 1 << 4,
        USD = 1 << 5,
        EUR = 1 << 6,
        PLN = 1 << 7,
        ISK = 1 << 8,
        CAD = 1 << 9,
        CNY = 1 << 10,
        KWD = 1 << 11,
        MDL = 1 << 12,
        NZD = 1 << 13,
        NOK = 1 << 14,
        RUB = 1 << 15,
        XDR = 1 << 16,
        SGD = 1 << 17,
        KGS = 1 << 18,
        KZT = 1 << 19,
        TRY = 1 << 20,
        GBP = 1 << 21,
        CZK = 1 << 22,
        SEK = 1 << 23,
        CHF = 1 << 24,
        JPY = 1 << 25,
        IRR = 1 << 26
    }
}
