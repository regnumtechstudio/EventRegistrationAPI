using Dapper;
using DataAccess.helper;
using Models.General;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.General
{
    public class eventRepository : IeventMaster
    {
        private readonly Iconnection _iDbconnection;
        public eventRepository(Iconnection iDbconnection)
        {
            _iDbconnection = iDbconnection;
        }
        public async Task<eventMaster> eventGet(eventMaster eventmaster)
        {
            string procName = "eventMasterGet";
            var eventObj = new eventMaster();
            try
            {
                var param = new DynamicParameters();
                param.Add("@_id", eventmaster.id);

                var result = await _iDbconnection.MasterDBConnection.QueryAsync<eventMaster>(procName
                                                                        , param
                                                                        , commandType: CommandType.StoredProcedure);

                eventObj = result.FirstOrDefault();

            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                _iDbconnection.CloseConnection();
            }
            return eventObj;
        }
        public async Task<IList<eventMaster>> eventGetList(eventMaster eventmaster)
        {
            string procName = "eventMasterGetList";
            try
            {
                var param = new DynamicParameters();
                var result = await _iDbconnection.MasterDBConnection.QueryAsync<eventMaster>(procName
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


    }
}
