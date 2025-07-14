using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.common
{
    public class userMaster : auditedColumn
    {
        public string companyname { get; set; }
        public string email { get; set; }
        public int? userType{ get; set; }
        public string appversion { get; set; }
        public int? roleid { get; set; }
        public DateTime? currentlogintime { get; set; }
        public string currentlogintimestr { get; set; }
        public DateTime? lastlogintime { get; set; }
        public string lastlogintimestr { get; set; }
        public string token { get; set; }
        public string refreshtoken { get; set; }
        public DateTime? refreshtokenexpirytime { get; set; }
        public string encryptionKey { get; set; }
        public long? entrybyuserid { get; set; }
        public string displayname { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string newpassword { get; set; }
        public string role { get; set; }
        public bool? active { get; set; } = null;
        public bool? nextloginchangepassword { get; set; } = false;
        public string firstname { get; set; }
        public string designation { get; set; }
        public string middelname { get; set; }
        public string lastname { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public string mobileno { get; set; }
        public string city { get; set; }
        public string notes { get; set; }
        public string address { get; set; }
    }

    public class UserMasterParam
    {
        public long? userId { get; set; }
        public int? roleId { get; set; }
        public string userName { get; set; }
        public int? userType { get; set; }
        public long? linkedToUserId { get; set; }
        public string mode { get; set; }
        public DateTime? refreshtokenExpiryTime { get; set; }
    }

    public class LoginInUserInfo
    {
        public long? userId { get; set; }
        public string userName { get; set; }
        public int? userType { get; set; }
        public int? roleId { get; set; }
    }
}
