using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.General
{
    public class otpManager
    {
        public int id { get; set; }
        public string otpCode { get; set; }
        public string email { get; set; }
        public string mobile { get; set; }
        public DateTime? generateTime { get; set; }
        public DateTime? expiryTime { get; set; }
       // public enumOtpSource? otpSource { get; set; }
        public bool isUsed { get; set; } = false;
        public bool? isActive { get; set; }
        public string mode { get; set; }
    }
}
