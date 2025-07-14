using Models.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class emailLogModel : auditedColumn
    {
        public string hostMail { get; set; }
        public string toMail { get; set; }
        public List<string> toMails { get; set; }
        public DateTime? sentTime { get; set; }
        //public int? attemptsCount { get; set; }
        public bool? status { get; set; }
        public string referenceId { get; set; }
        public int? parentId { get; set; }
        public int? parentType { get; set; }
        public string subject { get; set; }
        public string notes { get; set; }

    }
    
}
