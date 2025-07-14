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

namespace DataAccess.General
{
    public class participantRepository : IparticipantMaster
    {
        private readonly Iconnection _iDbconnection;
        public participantRepository(Iconnection iDbconnection)
        {
            _iDbconnection = iDbconnection;
        }
        public async Task<genericResponse> add(participantMaster clientmaster)
        {
            string procname = "eventRegistrationInsert";
            var param = new DynamicParameters();
            var response = new genericResponse();
            try
            {
                param.Add("@_eventId", clientmaster.eventId);
                param.Add("@_fullName", clientmaster.fullName);                
                param.Add("@_email", clientmaster.email);
                param.Add("@_mobileNo", clientmaster.mobileNo);
                param.Add("@_city", clientmaster.city);
                param.Add("@_outresult", 0, null, ParameterDirection.Output);
                param.Add("@_referenceNo", "", null, ParameterDirection.Output);

                await SqlMapper.ExecuteAsync(_iDbconnection.MasterDBConnection
                               , procname
                               , param
                               , commandType: CommandType.StoredProcedure);

                response.success = param.Get<int>("@_outresult");
                response.data = param.Get<string>("@_referenceNo");
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                _iDbconnection.CloseConnection();
            }
            return response;
        }
        public async Task<participantMaster> get(participantMaster clientmaster)
        {
            string procName = "eventRegistrationGet";
            var clientObj = new participantMaster();
            try
            {
                var param = new DynamicParameters();
                param.Add("@_clientId", clientmaster.clientId);
                param.Add("@_referenceNo", clientmaster.referenceNo);
                param.Add("@_mode", clientmaster.mode);

                var result = await _iDbconnection.MasterDBConnection.QueryAsync<participantMaster>(procName
                                                                        , param
                                                                        , commandType: CommandType.StoredProcedure);

                clientObj = result.FirstOrDefault();

            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                _iDbconnection.CloseConnection();
            }
            return clientObj;
        }
        public async Task<IList<participantMaster>> getList(participantMaster clientMaster)
        {
            string procName = "eventRegistrationGetList";
            try
            {
                var param = new DynamicParameters();
                var result = await _iDbconnection.MasterDBConnection.QueryAsync<participantMaster>(procName
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
                _iDbconnection.CloseConnection();
            }
        }
        //modifyStatus
        public async Task<genericResponse> tokenIssueStatusUpdate(participantMaster clientMaster)
        {
            string procname = "eventRegistrationStatusUpdate";
            var param = new DynamicParameters();
            var response = new genericResponse();
            try
            {
                param.Add("@_clientId", clientMaster.clientId);
                param.Add("@_eventId", clientMaster.eventId);
                param.Add("@_userId", clientMaster.userid);
                param.Add("@_outresult", 0, null, ParameterDirection.Output);

                await SqlMapper.ExecuteAsync(_iDbconnection.MasterDBConnection
                               , procname
                               , param
                               , commandType: CommandType.StoredProcedure);

                response.success = param.Get<int>("@_outresult");
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                _iDbconnection.CloseConnection();
            }
            return response;
        }
        public async Task<participantMasterParam> tokenGiftIssuedStatus(participantMaster clientMaster)
        {
            string procName = "eventRegistrationtokenStatus";
            var clientObj = new participantMasterParam();
            try
            {
                var param = new DynamicParameters();

                param.Add("@_mode", clientMaster.mode);

                var result = await _iDbconnection.MasterDBConnection.QueryAsync<participantMasterParam>(procName
                                                                        , param
                                                                        , commandType: CommandType.StoredProcedure);

                clientObj = result.FirstOrDefault();

            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                _iDbconnection.CloseConnection();
            }
            return clientObj;
        }
    }
}
