using System.Diagnostics;
using System.IO;
using Currency.BusinessEntities;
using Currency.Helpers;

namespace Currency.ApplicationData
{
    public static class SharedData
    {
        private static ProcessesDictionary _processesToKill;

        public static ProcessesDictionary ProcessesToKill
        {
            get
            {
                if (_processesToKill == null)
                {
                    _processesToKill = XmlSerializerHelper.Deserialize<ProcessesDictionary>(Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), @"Configuration\Processes\Processes.xml"));
                }

                return _processesToKill;
            }
        }
    }
}
