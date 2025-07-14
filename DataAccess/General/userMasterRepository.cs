using Dapper;
using DataAccess.helper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Models.common;
using Models.General;
using System;
using System.Collections.Generic;
using System.Data;
//using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static Models.ClsGloble;

namespace DataAccess.General
{
    public class userMasterRepository : IuserMaster
    {
        private readonly Iconnection _iDbconnection;
        private readonly IOptions<JWTSettings> _jwtSettings;
        private IConfiguration _config;
        public userMasterRepository(Iconnection connection, IOptions<JWTSettings> jwtSettings, IConfiguration config)
        {
            _iDbconnection = connection;
            _jwtSettings = jwtSettings;
            _config = config;
        }
        public async Task<int> userlogin(loginModel loginModel)
        {
            string procName = "userlogin";
            var param = new DynamicParameters();
            int result = 0;
            try
            {
                param.Add("@_username", loginModel.userName);
                param.Add("@_password", loginModel.password);
                param.Add("@_outResult", 0, null, ParameterDirection.Output);
                // using (IDbConnection conn = _connectionFactory.MasterDBConnection)
                {
                    await _iDbconnection.MasterDBConnection.ExecuteAsync(procName
                                                , param
                                                , commandType: CommandType.StoredProcedure);

                    result = param.Get<int>("@_outResult");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                // _connectionFactory.CloseConnection();
            }
            return result;
        }
        string buildToken(userMaster usermaster)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Name, usermaster.username),
                new Claim(JwtRegisteredClaimNames.NameId,usermaster.userid.ToString()),
               // new Claim(JwtRegisteredClaimNames.Sub,usermaster.userType),
                new Claim(JwtRegisteredClaimNames.Iss, _jwtSettings.Value.Issuer)
            };

            foreach (var item in _jwtSettings.Value.Audiences)
            {
                claims.Add(new Claim(JwtRegisteredClaimNames.Aud, item));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Value.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokeOptions = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(expires: DateTime.Now.AddMinutes(Convert.ToInt32(_jwtSettings.Value.ExpireMinutes))
                                                    , claims: claims
                                                    , signingCredentials: creds);

            return new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(tokeOptions);
        }
        public async Task<int> add(userMaster usermaster)
        {
            string procName = "userMasterInsert";
            var param = new DynamicParameters();
            int result = 0;
            try
            {

                param.Add("@_displayname", usermaster.displayname);
                param.Add("@_username", usermaster.username);
                param.Add("@_password", usermaster.password);
                param.Add("@_type", usermaster.userType);
                param.Add("@_email", usermaster.email);
                param.Add("@_userby", usermaster.entrybyuserid);
                param.Add("@_active", usermaster.active);
                param.Add("@_outResult", 0, null, ParameterDirection.Output);


                await SqlMapper.ExecuteAsync(_iDbconnection.MasterDBConnection
                               , procName
                               , param
                               , commandType: CommandType.StoredProcedure);


                result = param.Get<int>("@_outResult");

            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                _iDbconnection.CloseConnection();
            }
            return result;
        }

        public async Task<int> modify(userMaster usermaster)
        {
            string procName = "userMasterUpdate";
            var param = new DynamicParameters();
            int result = 0;
            try
            {

                param.Add("@_id", usermaster.userid);
                param.Add("@_displayname", usermaster.displayname);
                param.Add("@_userId", usermaster.entrybyuserid);
                param.Add("@_outResult", 0, null, ParameterDirection.Output);

                using (IDbConnection conn = _iDbconnection.MasterDBConnection)
                {
                    await conn.ExecuteAsync(procName
                                                , param
                                                , commandType: CommandType.StoredProcedure);
                    result = param.Get<int>("@_outResult");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                _iDbconnection.CloseConnection();
            }
            return result;
        }
        public async Task<int> verifyUser(userMaster userMaster)
        {
            int response = (int)enumResponseCode.fail;
            try
            {
                var procName = "authenticateUser";

                var param = new DynamicParameters();
                param.Add("@_userId", userMaster.userid);
                param.Add("@_userName", userMaster.username);
                param.Add("@_password", userMaster.password);
                param.Add("@_userType", userMaster.userType);
                param.Add("@_outResult", 0, null, ParameterDirection.Output);

                await _iDbconnection.MasterDBConnection.QueryAsync(procName
                                                        , param
                                                        , commandType: CommandType.StoredProcedure);

                response = param.Get<int>("@_outResult");
                if (response == (int)enumResponseCode.success)
                {
                    response = (int)enumResponseCode.success;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _iDbconnection.CloseConnection();
            }
            return response;
        }
        public async Task<genericResponse> userSignIn(loginModel model)
        {
            var response = new genericResponse { success = (int)enumResponseCode.fail };
            var userMasterModel = new userMaster();

            //get user detail
            userMasterModel = getDetailByParam(new UserMasterParam()
            {
                userType = (int)enumUserType.admin,
                userName = model.userName
            }).Result;
            if (userMasterModel != null)
            {
                var token = buildToken(userMasterModel);
                var refreshtoken = GenerateRefreshToken();
                userMasterModel.token = token;
                userMasterModel.refreshtoken = refreshtoken;
                var refreshtokenexpirytime = DateTime.Now.AddMinutes(_jwtSettings.Value.RefreshTokenExpireMinutes); //Convert.ToInt32(_config["Jwt:RefreshTokenExpireMinutes"])
                userMasterModel.refreshtokenexpirytime = refreshtokenexpirytime;

                //update token info 
                var loginTokenUpdateResponse = updatetokensLoginTime(userMasterModel).Result;
                if (loginTokenUpdateResponse == 1)
                {
                    userMasterModel.token = token;
                    userMasterModel.refreshtoken = refreshtoken;
                    userMasterModel.refreshtokenexpirytime = refreshtokenexpirytime;
                    userMasterModel.appversion = _config["appVersion"];

                    response.success = (int)enumResponseCode.success;
                    response.data = userMasterModel;
                }
                else
                {
                    response.success = (int)enumResponseCode.fail;
                    response.message = "Redirect to Login Page";
                }
            }
            return response;
        }
        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
        public async Task<userMaster> getDetailByParam(UserMasterParam userMasterParam)
        {
            var userMaster = new userMaster();

            var procName = "userMasterGet";
            try
            {
                var queryParameters = new DynamicParameters();
                queryParameters.Add("@_userName", userMasterParam.userName);
                queryParameters.Add("@_userType", userMasterParam.userType);

                var response = await _iDbconnection.MasterDBConnection.QueryAsync<userMaster>(procName
                                                                      , queryParameters
                                                                      , commandType: CommandType.StoredProcedure);
                if (response.Count() > 0)
                {
                    userMaster = response.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _iDbconnection.CloseConnection();
            }
            return userMaster;
        }

        public async Task<userMaster> getbyusername(string username)
        {
            string procName = "userMasterGetByUserName";
            var param = new DynamicParameters();
            var usermaster = new userMaster();
            param.Add("@_username", username);
            try
            {
                //using (IDbConnection conn = _connectionFactory.MasterDBConnection)
                {
                    var result = await _iDbconnection.MasterDBConnection.QueryAsync<userMaster>(procName
                                                                    , param
                                                                    , commandType: CommandType.StoredProcedure);
                    usermaster = result.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                // _connectionFactory.CloseConnection();
            }
            return usermaster;
        }
        public async Task<int> updatetokensLoginTime(userMaster usermasterModel)
        {
            string procName = "updatetokensLoginTime";
            var param = new DynamicParameters();
            int result = 0;
            try
            {
                param.Add("@_username", usermasterModel.username);
                param.Add("@_usertype", usermasterModel.userType);
                param.Add("@_refreshtoken", usermasterModel.refreshtoken);
                param.Add("@_refreshtokenexpirytime", usermasterModel.refreshtokenexpirytime);
                param.Add("@_outResult", 0, null, ParameterDirection.Output);

                //using (IDbConnection conn = _connectionFactory.MasterDBConnection)
                {
                    await _iDbconnection.MasterDBConnection.ExecuteAsync(procName
                                                , param
                                                , commandType: CommandType.StoredProcedure);
                    result = param.Get<int>("@_outResult");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                _iDbconnection.CloseConnection();
            }
            return result;
        }
    }
}
