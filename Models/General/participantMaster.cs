using Models.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.General
{
    public class participantMaster : auditedColumn
    {
        public long? clientId { get; set; }
        public int? eventId { get; set; }
        public string fullName { get; set; }
        public string email { get; set; }
        public string mobileNo { get; set; }
        public string city { get; set; }
        public string otpMobile { get; set; }
        public bool? tokenGiftIssued { get; set; }
        public string referenceNo { get; set; }
        public string eventName { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string mode { get; set; }
      
    }
    public class participantMasterParam
    {
        public int? totalcount { get; set; }
    }

    public class eventMaster : auditedColumn
    {
        public int? id { get; set; }
        public  string eventName { get; set; }
        public  DateTime? startDate { get; set; }
        public string startDateStr { get; set; }
        public  DateTime? endDate { get; set; }
        public string endDateStr { get; set; }
        public  bool? active { get; set; }
    }
}
