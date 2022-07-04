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
        private readonly MySqlConnection _MySqlConnection;
        private readonly string _connectionString = "";
        private readonly IConfiguration _config;
        private readonly IServiceProvider _provier;
        private MySqlTransaction _transaction;
        #endregion

        #region Constructor
        public UnitOfWork(IServiceProvider provider)
        {
            _provier = provider;
            _config = provider.GetRequiredService<IConfiguration>();
            _connectionString = _config.GetSection("mysql:trial_mysql").Value;
            _MySqlConnection = new MySqlConnection(_connectionString);
            _MySqlConnection.Open();
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Get MySqlConnection
        /// </summary>
        public MySqlConnection GetConnection()
        {
            return _MySqlConnection;
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
            _transaction = _MySqlConnection.BeginTransaction();
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
            return _MySqlConnection.Query<T>(sql, param, commandTimeout: commandTimeout, commandType: commandType);
        }

        /// <summary>
        ///  Execute a query asynchronously using Task.
        /// </summary>
        public Task<IEnumerable<T>> QueryAsync<T>(string sql, object param, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {
            return _MySqlConnection.QueryAsync<T>(sql, param, commandTimeout: commandTimeout, commandType: commandType);
        }

        /// <summary>
        /// Executes a single-row query, returning the data typed as T.
        /// </summary>
        public T QueryFirstOrDefault<T>(string sql, object param, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {
            return _MySqlConnection.QueryFirstOrDefault<T>(sql, param, commandTimeout: commandTimeout, commandType: commandType);
        }

        /// <summary>
        /// Execute a single-row query asynchronously using Task.
        /// </summary>
        public Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {
            return _MySqlConnection.QueryFirstOrDefaultAsync<T>(sql, param, commandTimeout: commandTimeout, commandType: commandType);
        }

        /// <summary>
        /// Executes a single-row query, returning the data typed as T.
        /// </summary>
        public T QuerySingleOrDefault<T>(string sql, object param, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {
            return _MySqlConnection.QuerySingleOrDefault<T>(sql, param, commandTimeout: commandTimeout, commandType: commandType);
        }

        /// <summary>
        ///  Execute a single-row query asynchronously using Task.
        /// </summary>
        public Task<T> QuerySingleOrDefaultAsync<T>(string sql, object param, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {
            return _MySqlConnection.QuerySingleOrDefaultAsync<T>(sql, param, commandTimeout: commandTimeout, commandType: commandType);
        }

        /// <summary>
        ///  Return a sequence of dynamic objects with properties matching the columns.
        /// </summary>
        public dynamic Query(string sql, object param, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {
            return _MySqlConnection.Query(sql, param, commandTimeout: commandTimeout, commandType: commandType);
        }

        /// <summary>
        ///  Execute a query asynchronously using Task.
        /// </summary>
        public Task<IEnumerable<dynamic>> QueryAsync(string sql, object param, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {
            return _MySqlConnection.QueryAsync(sql, param, commandTimeout: commandTimeout, commandType: commandType);

        }

        /// <summary>
        /// Return a dynamic object with properties matching the columns.
        /// </summary>
        public dynamic QueryFirstOrDefault(string sql, object param, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {
            return _MySqlConnection.QueryFirstOrDefault(sql, param, commandTimeout: commandTimeout, commandType: commandType);

        }

        /// <summary>
        /// Execute a single-row query asynchronously using Task.
        /// </summary>
        public Task<dynamic> QueryFirstOrDefaultAsync(string sql, object param, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {
            return _MySqlConnection.QueryFirstOrDefaultAsync(sql, param, commandTimeout: commandTimeout, commandType: commandType);

        }

        /// <summary>
        /// Return a dynamic object with properties matching the columns.
        /// </summary>
        public dynamic QuerySingleOrDefault(string sql, object param, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {
            return _MySqlConnection.QuerySingleOrDefault(sql, param, commandTimeout: commandTimeout, commandType: commandType);

        }

        /// <summary>
        /// Execute a single-row query asynchronously using Task.
        /// </summary>
        public Task<dynamic> QuerySingleOrDefaultAsync(string sql, object param, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {
            return _MySqlConnection.QuerySingleOrDefaultAsync(sql, param, commandTimeout: commandTimeout, commandType: commandType);

        }

        /// <summary>
        /// Execute parameterized SQL.
        /// </summary>
        /// <returns> The number of rows affected. </returns>
        public int Execute(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {
            BeginTransaction();
            return _MySqlConnection.Execute(sql, param, _transaction, commandTimeout, commandType);
        }

        /// <summary>
        /// Execute parameterized SQL with auto commit
        /// </summary>
        /// <returns> The number of rows affected. </returns>
        public int ExecuteAutoCommit(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {
            BeginTransaction();
            int result = _MySqlConnection.Execute(sql, param, _transaction, commandTimeout, commandType);
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
            return _MySqlConnection.ExecuteAsync(sql, param, _transaction, commandTimeout, commandType);
        }

        /// <summary>
        /// Execute parameterized SQL that selects a single value.
        /// </summary>
        /// <returns> The first cell selected as System.Object. </returns>
        public object ExecuteScalar(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {
            BeginTransaction();
            return _MySqlConnection.ExecuteScalar(sql, param, _transaction, commandTimeout, commandType);
        }

        /// <summary>
        /// Execute parameterized SQL that selects a single value.
        /// </summary>
        /// <returns> The first cell selected as System.Object. </returns>
        public Task<object> ExecuteScalarAsync(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {
            BeginTransaction();
            return _MySqlConnection.ExecuteScalarAsync(sql, param, _transaction, commandTimeout, commandType);
        }
        #endregion

        #region Dispose
        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            if (_MySqlConnection.State == ConnectionState.Open)
            {
                _MySqlConnection.Close();
            }
        }
        #endregion
    }
}
