using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Models.common;

namespace DataAccess.helper
{
    public class ConnectionRepository : Iconnection
    {
        private IDbConnection _connection;
        private SqlConnection _Sqlconnection;
        
        private readonly IOptions<contextConfiguration> _configs;
        public ConnectionRepository(IOptions<contextConfiguration> Configs)
        {
            _configs = Configs;
        }

        public IDbConnection MasterDBConnection
        {
            get
            {
                if (_connection == null)
                {
                    _connection = new SqlConnection(_configs.Value.MasterDbConnectionString);
                }
                if (_connection.State != ConnectionState.Open)
                {
                    _connection.Open();
                }
                return _connection;
            }
        }

        public SqlConnection MasterDBSqlConnection
        {
            get
            {
                if (_Sqlconnection == null)
                {
                    _Sqlconnection = new SqlConnection(_configs.Value.MasterDbConnectionString);
                }
                if (_Sqlconnection.State != ConnectionState.Open)
                {
                    _Sqlconnection.Open();
                }
                return _Sqlconnection;
            }
        }

        public string MasterDBConnectionString
        {
            get
            {
                return _configs.Value.MasterDbConnectionString;
            }
        }

        //public MySqlConnection mySqlConnection
        //{
        //    get
        //    {
        //        if (_mySqlConnection == null)
        //        {
        //            _mySqlConnection = new MySqlConnection(_configs.Value.MasterDbConnectionString);
        //        }
        //        if (_mySqlConnection.State != ConnectionState.Open)
        //        {
        //            _mySqlConnection.Open();
        //        }
        //        return _mySqlConnection;
        //    }
        //}

        public void CloseConnection()
        {
            if (_connection != null && _connection.State == ConnectionState.Open)
            {
                _connection.Close();
                //_connection = null;
            }
        }

        //public void CloseMySqlConnection()
        //{
        //    if (_mySqlConnection != null && _mySqlConnection.State == ConnectionState.Open)
        //    {
        //        _mySqlConnection.Close();
        //    }
        //}
        public void CloseConnection(IDbConnection conn)
        {
            if (conn != null && conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }

        //public void CloseMySqlConnection(MySqlConnection conn)
        //{
        //    if (conn != null && conn.State == ConnectionState.Open)
        //    {
        //        conn.Close();
        //    }
        //}

    }
}
