using Dapper;
using Models.common;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Models.ClsGloble;
using DataAccess.helper;

namespace DataAccess.General
{
    public class emailLogRepository : IemailLog
    {
        private readonly Iconnection _iDbConnection;
        public emailLogRepository(Iconnection iDbConnection)
        {
            _iDbConnection = iDbConnection;
        }
        public async Task<genericResponse> add(emailLogModel emailLog)
        {
            var response = new genericResponse();
            var procName = "EmailLogInsert";
            try
            {
                var param = new DynamicParameters();
                param.Add("@_hostMail", emailLog.hostMail);
                param.Add("@_toMail", emailLog.hostMail);
                param.Add("@_sentTime", emailLog.sentTime);
                param.Add("@_subject", emailLog.subject);
                param.Add("@_parentId", emailLog.parentId);
                param.Add("@_parentType", emailLog.parentType);
                param.Add("@_status", emailLog.status);
                param.Add("@_outResult", 0, null, ParameterDirection.Output);

                await _iDbConnection.MasterDBConnection.ExecuteAsync(procName
                                                       , param
                                                       , commandType: CommandType.StoredProcedure);

                response.success = param.Get<int>("@_outResult");
            }
            catch (Exception ex)
            {
                response.success = (int)enumResponseCode.error;
                response.message = ex.Message;
            }
            finally
            {
                _iDbConnection.CloseConnection();
            }
            return response;
        }
    }
}
