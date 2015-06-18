using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Currency.BusinessEntities
{
    [XmlRoot("processes")]
    public class ProcessesDictionary
    {
        [XmlElement("process")]
        public List<ProcessItem> Processes { get; set; }
    }

    public class ProcessItem
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("period_of_killing_sec")]
        public int Period { get; set; }

        [XmlAttribute("auto_start")]
        public bool AutoStart { get; set; }
    }
}
