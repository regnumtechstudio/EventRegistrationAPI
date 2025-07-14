using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.General
{
    public class smsServiceModel
    {
         public string smsContent { get; set; }
         public string routeId { get; set; }
         public string mobileNumbers { get; set; }
         public string senderId { get; set; }
         public string  smsContentType { get; set; }
         public string signature { get; set; }
         public string entityid { get; set; }
         public string tmid { get; set; }
         public string templateid { get; set; }
         public string peChainID { get; set; }
         public int? concentFailoverId { get; set; }
    }
    public class smsResponse
    {
        public bool status { get; set; }
        public string message { get; set; }
    }
}
