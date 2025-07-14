using Models.common;
using Models.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.General
{
    public interface IotpManager
    {
        Task<genericResponse> add(otpManager otpManager);
        Task<genericResponse> verify(otpManager otpManager);
        Task<List<otpManager>> get(otpManager otpManager);
        Task<genericResponse> validOTP(otpManager otpManager);
       
    }
}
