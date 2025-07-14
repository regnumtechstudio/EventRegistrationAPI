using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.common;

namespace DataAccess.General
{
    public interface IuserMaster
    {
        Task<int> userlogin(loginModel loginModel);
        Task<int> add(userMaster usermaster);
        Task<int> modify(userMaster usermaster);
        Task<userMaster> getbyusername(string username);
        Task<int> updatetokensLoginTime(userMaster usermasterModel);
        Task<int> verifyUser(userMaster userMaster);
        Task<genericResponse> userSignIn(loginModel model);
    }
}
