using Dapper;
using DataAccess.helper;
using Models.common;
using Models.General;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Models.ClsGloble;

namespace DataAccess.General
{
    public class otpManagerRepository : IotpManager
    {
        private readonly Iconnection _iDbConnection;

        public otpManagerRepository(Iconnection iDbConnection)
        {
            _iDbConnection = iDbConnection;
        }

        public async Task<genericResponse> add(otpManager otpManager)
        {
            var response = new genericResponse();
            try
            {
                string procName = "otpManagerInsert";
                var param = new DynamicParameters();
                param.Add("@_email", otpManager.email);
                param.Add("@_mobile", otpManager.mobile);
                param.Add("@_otpCode", otpManager.otpCode);
               // param.Add("@_otpSource", otpManager.otpSource);
                param.Add("@_isActive", otpManager.isActive);
                param.Add("@_outResult", 0, null, ParameterDirection.Output);
                //param.Add("@_insertedId", 0, null, ParameterDirection.Output);

                await SqlMapper.ExecuteAsync(_iDbConnection.MasterDBConnection,
                                        procName
                                        , param
                                        , commandType: CommandType.StoredProcedure);

                response.success = param.Get<int>("@_outResult");

            }
            catch (Exception ex)
            {
                response = new genericResponse
                {
                    success = (int)enumResponseCode.error,
                    message = ex.Message.ToString()
                };
            }
            finally
            {
                _iDbConnection.CloseConnection();
            }
            return response;
        }

        public async Task<List<otpManager>> get(otpManager otpManager)
        {
            try
            {
                string procName = "otpManagerGet";
                var param = new DynamicParameters();
                param.Add("@_email", otpManager.email);
                param.Add("@_otpCode", otpManager.otpCode);
                param.Add("@_mobile", otpManager.mobile);
                param.Add("@_mode", otpManager.mode);
               // param.Add("@_otpSource", otpManager.otpSource);

                var result = await _iDbConnection.MasterDBConnection.QueryAsync<otpManager>(
                                      procName
                                     , param
                                     , commandType: CommandType.StoredProcedure);

                return result.ToList();
            }
            catch (Exception ex)
            {
                throw;

            }
            finally
            {
                _iDbConnection.CloseConnection();
            }
        }

        public async Task<genericResponse> verify(otpManager otpManager)
        {
            var response = new genericResponse();
            try
            {
                string procName = "otpManagerVerification";
                var param = new DynamicParameters();
                param.Add("@_email", otpManager.email);
                param.Add("@_mobile", otpManager.mobile);
                param.Add("@_otpCode", otpManager.otpCode);
              //  param.Add("@_otpSource", otpManager.otpSource);
                param.Add("@_outResult", 0, null, ParameterDirection.Output);

                await SqlMapper.ExecuteAsync(_iDbConnection.MasterDBConnection
                                    , procName
                                    , param
                                    , commandType: CommandType.StoredProcedure);

                response.success = param.Get<int>("@_outResult");
            }
            catch (Exception ex)
            {
                response = new genericResponse
                {
                    success = (int)enumResponseCode.error,
                    message = ex.Message.ToString()
                };

            }
            finally
            {
                _iDbConnection.CloseConnection();
            }
            return response;
        }
        public async Task<genericResponse> validOTP(otpManager otpManager)
        {
            var response = new genericResponse();
            try
            {
                var otplist = await get(otpManager);
                otpManager = otplist.FirstOrDefault();

                if (otpManager == null)
                {
                    response.success = (int)enumOTPVerificationStatus.invalid;
                    response.message = "Invalid OTP";
                }
                else if (!(otpManager.generateTime <= DateTime.Now && otpManager.expiryTime >= DateTime.Now))
                {
                    response.success = (int)enumOTPVerificationStatus.expired;
                    response.message = "OTP is expired";
                }
                else
                {
                    response.success = (int)enumOTPVerificationStatus.success;
                    response.message = "Valid OTP";
                }
            }
            catch (Exception ex)
            {
                response = new genericResponse
                {
                    success = (int)enumResponseCode.error,
                    message = ex.Message
                };
            }
            finally
            {
                _iDbConnection.CloseConnection();
            }
            return response;
        }
    }
}
