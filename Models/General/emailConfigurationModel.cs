using Models.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.General
{
    public class emailConfigurationModel : auditedColumn
    {
        public int? id { get; set; }
        public string hostUser { get; set; } //email
        public string displayName { get; set; }
        public string hostServer { get; set; } // server 
        public int hostType { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public int port { get; set; }
        public bool usessl { get; set; }
        public bool isDefault { get; set; }
        public int secureSocketOptions { get; set; }
        public int serviceType { get; set; }
    }
}
