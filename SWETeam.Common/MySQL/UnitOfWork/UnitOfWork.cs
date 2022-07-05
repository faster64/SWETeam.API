using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace SWETeam.Common.MySQL
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        #region Declare
        private readonly MySqlConnection _mySqlConnection;
        private readonly string _connectionString = "";
        private readonly IConfiguration _config;
        private readonly IServiceProvider _provider;
        private MySqlTransaction _transaction;
        #endregion

        #region Constructor
        public UnitOfWork(IServiceProvider provider)
        {
            _provider = provider;
            _config = _provider.GetRequiredService<IConfiguration>();
            _connectionString = _config.GetSection("mysql:trial_mysql").Value;
            _mySqlConnection = new MySqlConnection(_connectionString);
            OpenConnection();
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Open connection
        /// </summary>
        public void OpenConnection()
        {
            _mySqlConnection.Open();
        }

        /// <summary>
        /// Get MySqlConnection
        /// </summary>
        public MySqlConnection GetConnection()
        {
            return _mySqlConnection;
        }

        /// <summary>
        /// Get MySqlTransaction
        /// </summary>
        public MySqlTransaction GetTransaction()
        {
            return _transaction;
        }

        /// <summary>
        /// Khởi tạo transaction
        /// </summary>
        public void BeginTransaction()
        {
            _transaction = _mySqlConnection.BeginTransaction();
        }

        /// <summary>
        /// Commit
        /// </summary>
        public void Commit()
        {
            if (_transaction != null)
            {
                _transaction.Commit();
            }
        }

        /// <summary>
        /// Rollback
        /// </summary>
        public void Rollback()
        {
            if (_transaction != null)
            {
                _transaction.Rollback();
            }
        }

        /// <summary>
        /// Executes a query, returning the data typed as T.
        /// </summary>
        public IEnumerable<T> Query<T>(string sql, object param, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {
            return _mySqlConnection.Query<T>(sql, param, commandTimeout: commandTimeout, commandType: commandType);
        }

        /// <summary>
        ///  Execute a query asynchronously using Task.
        /// </summary>
        public Task<IEnumerable<T>> QueryAsync<T>(string sql, object param, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {
            return _mySqlConnection.QueryAsync<T>(sql, param, commandTimeout: commandTimeout, commandType: commandType);
        }

        /// <summary>
        /// Executes a single-row query, returning the data typed as T.
        /// </summary>
        public T QueryFirstOrDefault<T>(string sql, object param, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {
            return _mySqlConnection.QueryFirstOrDefault<T>(sql, param, commandTimeout: commandTimeout, commandType: commandType);
        }

        /// <summary>
        /// Execute a single-row query asynchronously using Task.
        /// </summary>
        public Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {
            return _mySqlConnection.QueryFirstOrDefaultAsync<T>(sql, param, commandTimeout: commandTimeout, commandType: commandType);
        }

        /// <summary>
        /// Executes a single-row query, returning the data typed as T.
        /// </summary>
        public T QuerySingleOrDefault<T>(string sql, object param, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {
            return _mySqlConnection.QuerySingleOrDefault<T>(sql, param, commandTimeout: commandTimeout, commandType: commandType);
        }

        /// <summary>
        ///  Execute a single-row query asynchronously using Task.
        /// </summary>
        public Task<T> QuerySingleOrDefaultAsync<T>(string sql, object param, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {
            return _mySqlConnection.QuerySingleOrDefaultAsync<T>(sql, param, commandTimeout: commandTimeout, commandType: commandType);
        }

        /// <summary>
        ///  Return a sequence of dynamic objects with properties matching the columns.
        /// </summary>
        public dynamic Query(string sql, object param, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {
            return _mySqlConnection.Query(sql, param, commandTimeout: commandTimeout, commandType: commandType);
        }

        /// <summary>
        ///  Execute a query asynchronously using Task.
        /// </summary>
        public Task<IEnumerable<dynamic>> QueryAsync(string sql, object param, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {
            return _mySqlConnection.QueryAsync(sql, param, commandTimeout: commandTimeout, commandType: commandType);

        }

        /// <summary>
        /// Return a dynamic object with properties matching the columns.
        /// </summary>
        public dynamic QueryFirstOrDefault(string sql, object param, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {
            return _mySqlConnection.QueryFirstOrDefault(sql, param, commandTimeout: commandTimeout, commandType: commandType);

        }

        /// <summary>
        /// Execute a single-row query asynchronously using Task.
        /// </summary>
        public Task<dynamic> QueryFirstOrDefaultAsync(string sql, object param, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {
            return _mySqlConnection.QueryFirstOrDefaultAsync(sql, param, commandTimeout: commandTimeout, commandType: commandType);

        }

        /// <summary>
        /// Return a dynamic object with properties matching the columns.
        /// </summary>
        public dynamic QuerySingleOrDefault(string sql, object param, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {
            return _mySqlConnection.QuerySingleOrDefault(sql, param, commandTimeout: commandTimeout, commandType: commandType);

        }

        /// <summary>
        /// Execute a single-row query asynchronously using Task.
        /// </summary>
        public Task<dynamic> QuerySingleOrDefaultAsync(string sql, object param, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {
            return _mySqlConnection.QuerySingleOrDefaultAsync(sql, param, commandTimeout: commandTimeout, commandType: commandType);

        }

        /// <summary>
        /// Execute parameterized SQL.
        /// </summary>
        /// <returns> The number of rows affected. </returns>
        public int Execute(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {
            BeginTransaction();
            return _mySqlConnection.Execute(sql, param, _transaction, commandTimeout, commandType);
        }

        /// <summary>
        /// Execute parameterized SQL with auto commit
        /// </summary>
        /// <returns> The number of rows affected. </returns>
        public int ExecuteAutoCommit(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {
            BeginTransaction();
            int result = _mySqlConnection.Execute(sql, param, _transaction, commandTimeout, commandType);
            if (result > 0)
            {
                _transaction.Commit();
            }
            else
            {
                _transaction.Rollback();
            }

            return result;
        }

        /// <summary>
        /// Execute a command asynchronously using Task.
        /// </summary>
        /// <returns> The number of rows affected. </returns>
        public Task<int> ExecuteAsync(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {
            BeginTransaction();
            return _mySqlConnection.ExecuteAsync(sql, param, _transaction, commandTimeout, commandType);
        }

        /// <summary>
        /// Execute parameterized SQL that selects a single value.
        /// </summary>
        /// <returns> The first cell selected as System.Object. </returns>
        public object ExecuteScalar(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {
            BeginTransaction();
            return _mySqlConnection.ExecuteScalar(sql, param, _transaction, commandTimeout, commandType);
        }

        /// <summary>
        /// Execute parameterized SQL that selects a single value.
        /// </summary>
        /// <returns> The first cell selected as System.Object. </returns>
        public Task<object> ExecuteScalarAsync(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {
            BeginTransaction();
            return _mySqlConnection.ExecuteScalarAsync(sql, param, _transaction, commandTimeout, commandType);
        }
        #endregion

        #region Dispose
        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            if (_mySqlConnection.State == ConnectionState.Open)
            {
                _mySqlConnection.Close();
            }
        }
        #endregion
    }
}
