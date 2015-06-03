using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankClient;
using Currency.BusinessEntities;
using System.IO;
using Newtonsoft.Json;

namespace Currency.Helpers
{
    public static class RatesHelper
    {

        public static IEnumerable<Rate> ConvertToRates(DataTable dt)
        {
            return ConvertToRates(dt, null);
        }

        public static IEnumerable<Rate> ConvertToRates(DataTable dt, DateTime? date)
        {
            List<Rate> result = new List<Rate>();

            foreach (DataRow row in dt.Rows)
            {
                result.Add(ConvertToRates(row, date));
            }
            return result;
        }

        public static Rate ConvertToRates(DataRow row)
        {
            return ConvertToRates(row, null);
        }

        public static Rate ConvertToRates(DataRow row, DateTime? date)
        {
            Rate result = new Rate()
            {
                QuotName = row[0].ToString(),
                Scale = int.Parse(row[1].ToString()),
                OfficialRate = double.Parse(row[2].ToString()),
                Code = row[3].ToString(),
                Date = date ?? default(DateTime),
                Abbreviation = AbbreviationHelper.ToAbbreviation(row[4].ToString())
            };
            return result;
        }

        public static List<Rate> GetLastDailyRates()
        {
            List<Rate> result = new List<Rate>();
            Client.Execute(client =>
            {
                var lastDate = client.LastDailyExRatesDate();
                var data = client.ExRatesDaily(lastDate).Tables[0];
                result = ConvertToRates(data, lastDate).ToList();
            });
            return result;
        }

        public static List<Rate> GetPrevDailyRates()
        {
            List<Rate> result = new List<Rate>();
            Client.Execute(client =>
            {
                var lastDate = client.LastDailyExRatesDate();
                var prevDate = lastDate.AddDays(lastDate.DayOfWeek == DayOfWeek.Monday ? -3 : -1);
                var data = client.ExRatesDaily(prevDate).Tables[0];

                result = ConvertToRates(data, prevDate).ToList();
            });
            return result;
        }

        public static List<Rate> GetRatesForPeriod(Abbreviation abb, DateTime from, DateTime to)
        {
            List<Rate> result = new List<Rate>();

            Client.Execute(client =>
                {
                    var t = GetCurrenciesData().First(item => item.Abbreviation == abb.ToString()).ID;

                    var dt = client.ExRatesDyn(t, from, to).Tables[0];
                    foreach (DataRow row in dt.Rows)
                    {
                        result.Add(new Rate()
                        {
                            OfficialRate = double.Parse(row["Cur_OfficialRate"].ToString()),
                            Date = DateTime.Parse(row["Date"].ToString()),
                            Abbreviation = abb
                        });
                    }
                });

            return result;
        }

        public static List<CurrencyObject> GetCurrenciesData()
        {
            List<CurrencyObject> result = new List<CurrencyObject>();

            if (!File.Exists("currencies.json"))
            {
                Client.Execute(client =>
                {
                    var dt = client.CurrenciesRef(0).Tables[0];

                    foreach (DataRow row in dt.Rows)
                    {
                        result.Add(new CurrencyObject()
                            {
                                ID = int.Parse(row["Cur_Id"].ToString()),
                                Abbreviation = row["Cur_Abbreviation"].ToString()
                            });
                    }
                    var abs = GetLastDailyRates().Select(item => item.Abbreviation.ToString());

                    result = result.Where(item => abs.Contains(item.Abbreviation)).ToList();

                    List<CurrencyObject> wtf = new List<CurrencyObject>();
                    DataSet j;

                    foreach (var h in result)
                    {
                        j = client.ExRatesDyn(h.ID, DateTime.Now.AddDays(-2), DateTime.Now);
                        if (j == null)
                        {
                            wtf.Add(h);
                        }
                    }
                    result = result.Where(item => !wtf.Select(it => it.Abbreviation).Contains(item.Abbreviation)).ToList();
                });

                File.WriteAllText("currencies.json", JsonConvert.SerializeObject(result));
            }

            result = JsonConvert.DeserializeObject<List<CurrencyObject>>(File.ReadAllText("currencies.json"));

            return result;
        }

        public class CurrencyObject
        {
            public int ID { get; set; }
            public string Abbreviation { get; set; }
        }
    }
}
