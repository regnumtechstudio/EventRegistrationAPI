using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.General
{
    public class JWTSettings
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public List<string> Audiences { get; set; }
        public int ExpireMinutes { get; set; }
        public int RefreshTokenExpireMinutes { get; set; }

    }
}

