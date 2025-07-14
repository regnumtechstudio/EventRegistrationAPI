using Models.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Utilities
{
    public class JwtHelper
    {
        public static string ClaimValueByJwtToken(string token, string claimName)
        {
            var result = string.Empty;
            try
            {
                token = token.Replace(JwtBearerDefaults.AuthenticationScheme, string.Empty).Trim();

                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = (JwtSecurityToken)tokenHandler.ReadToken(token);

                var claimValue = securityToken.Claims.FirstOrDefault(c => c.Type == claimName)?.Value;
                result = claimValue;
            }
            catch (Exception)
            {
                //TODO: Logger.Error
                //return "";
            }
            return result;
        }

        public static LoginInUserInfo getLoginUserInfoByJwtToken(string jwtToken)
        {
            var userLoginInfo = new LoginInUserInfo();

            Int64 userId = 0;
            string userName = "";
            int userType = -1;

            try
            {
                Int64.TryParse(JwtHelper.ClaimValueByJwtToken(jwtToken, claimName: JwtRegisteredClaimNames.NameId), out userId);
                userName = JwtHelper.ClaimValueByJwtToken(jwtToken, claimName: JwtRegisteredClaimNames.Name);
                int.TryParse(JwtHelper.ClaimValueByJwtToken(jwtToken, claimName: JwtRegisteredClaimNames.Sub), out userType);

                userLoginInfo.userId = userId;
                userLoginInfo.userName = userName;
                userLoginInfo.userType = userType;
            }
            catch
            {

            }
            return userLoginInfo;
        }
    }
}
