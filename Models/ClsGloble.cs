using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ClsGloble
    {
        public enum enumUserType
        {
            admin = 1,
            client = 2
        }

        public enum enumResponseCode
        {
            error = -1,
            fail = 0,
            success = 1,
            exist = 2
        }
        public enum enumEmailServiceType
        {
            /// <summary>
            /// use for OTP, welcomEmail, ForgotPWD, ResetPWD, reneal Reminders, send Invoices, Etc...
            /// </summary>
            GeneralService = 0,
            /// <summary>
            /// use for, client send invoice to his customer, client send login details to new creted user, client will send payment reminders to his customers, etc..
            /// </summary>
            //clientSignUp = 1
        }
        public enum enumEmailHostType
        {
            smtp = 0,
            googleWorkspaceEmail = 1,
            awsEmail = 1,
        }
        public enum enumMailLogParentType
        {
            SignupOTPVerification = 1,

        }
        public enum enumOTPVerificationStatus
        {
            error = -1,
            invalid = 0,
            success = 1,
            expired = 2
        }
    }
}
