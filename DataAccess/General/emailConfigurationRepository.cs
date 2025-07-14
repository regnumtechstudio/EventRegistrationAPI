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
    public class emailConfigurationRepository : IemailConfiguration 
    {
        private readonly Iconnection _iDbconnection;

        public emailConfigurationRepository(Iconnection iDbconnection)
        {
            _iDbconnection = iDbconnection;
        }
        public async Task<emailConfigurationModel> get(emailConfigurationModel emailConfiguration)
        {
            try
            {
                string procName = "EmailConfigurationGet";
                var param = new DynamicParameters();
                param.Add("@_id", emailConfiguration.id);
                param.Add("@_serviceType", emailConfiguration.serviceType);
                param.Add("@_isdefault", emailConfiguration.isDefault);

                var result = await _iDbconnection.MasterDBConnection.QueryAsync<emailConfigurationModel>(procName
                                                                        , param
                                                                        , commandType: CommandType.StoredProcedure);

                emailConfiguration = result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _iDbconnection.CloseConnection();
            }
            return emailConfiguration;
        }
    }
}
