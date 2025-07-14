using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.common
{
    public class loginModel
    {
        public string userName { get; set; }
        public string idtoken { get; set; }
        public string password { get; set; }
        public int? userType { get; set; }
    }
}
