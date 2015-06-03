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
            Abbreviation result;
            switch (abbreviation)
            {
                case "AUD": result = Abbreviation.AUD;
                    break;
                case "BGN": result = Abbreviation.BGN;
                    break;
                case "UAH": result = Abbreviation.UAH;
                    break;
                case "DKK": result = Abbreviation.DKK;
                    break;
                case "USD": result = Abbreviation.USD;
                    break;
                case "EUR": result = Abbreviation.EUR;
                    break;
                case "PLN": result = Abbreviation.PLN;
                    break;
                case "ISK": result = Abbreviation.ISK;
                    break;
                case "CAD": result = Abbreviation.CAD;
                    break;
                case "CNY": result = Abbreviation.CNY;
                    break;
                case "KWD": result = Abbreviation.KWD;
                    break;
                case "MDL": result = Abbreviation.MDL;
                    break;
                case "NZD": result = Abbreviation.NZD;
                    break;
                case "NOK": result = Abbreviation.NOK;
                    break;
                case "RUB": result = Abbreviation.RUB;
                    break;
                case "XDR": result = Abbreviation.XDR;
                    break;
                case "SGD": result = Abbreviation.SGD;
                    break;
                case "KGS": result = Abbreviation.KGS;
                    break;
                case "KZT": result = Abbreviation.KZT;
                    break;
                case "TRY": result = Abbreviation.TRY;
                    break;
                case "GBP": result = Abbreviation.GBP;
                    break;
                case "CZK": result = Abbreviation.CZK;
                    break;
                case "SEK": result = Abbreviation.SEK;
                    break;
                case "CHF": result = Abbreviation.CHF;
                    break;
                case "JPY": result = Abbreviation.JPY;
                    break;
                case "IRR": result = Abbreviation.IRR;
                    break;
                default:
                    result = Abbreviation.NONE;
                    break;
            }
            return result;
        }
    }
}
