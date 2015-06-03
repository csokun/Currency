using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using BankClient.NBRBService;

namespace BankClient
{
    public static class Client
    {
        public static void Execute(Action<ExRatesSoapClient> action)
        {
            ExRatesSoapClient serviceClient = new ExRatesSoapClient();

            try
            {
                action(serviceClient);

                serviceClient.Close();
            }
            catch (CommunicationException)
            {
                serviceClient.Abort();
            }
            catch (TimeoutException)
            {
                serviceClient.Abort();
            }
            catch (Exception)
            {
                serviceClient.Abort();
                throw;
            }
        }
    }
}
