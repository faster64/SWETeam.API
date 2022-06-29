using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace SWETeam.Common.MySQL
{
    public interface IUnitOfWork
    {
        /// <summary>
        /// Get MySqlConnection
        /// </summary>
        MySqlConnection GetConnection();

        /// <summary>
        /// Get MySqlTransaction
        /// </summary>
        MySqlTransaction GetTransaction();

        /// <summary>
        /// Khởi tạo transaction
        /// </summary>
        void BeginTransaction();

        /// <summary>
        /// Commit
        /// </summary>
        public void Commit();

        /// <summary>
        /// Rollback
        /// </summary>
        public void Rollback();

        /// <summary>
        /// Executes a query, returning the data typed as T.
        /// </summary>
        IEnumerable<T> Query<T>(string sql, object param, int? commandTimeout = null, CommandType? commandType = CommandType.Text);

        /// <summary>
        ///  Return a sequence of dynamic objects with properties matching the columns.
        /// </summary>
        dynamic Query(string sql, object param, int? commandTimeout = null, CommandType? commandType = CommandType.Text);

        /// <summary>
        ///  Execute a query asynchronously using Task.
        /// </summary>
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object param, int? commandTimeout = null, CommandType? commandType = CommandType.Text);

        /// <summary>
        ///  Execute a query asynchronously using Task.
        /// </summary>
        Task<IEnumerable<dynamic>> QueryAsync(string sql, object param, int? commandTimeout = null, CommandType? commandType = CommandType.Text);

        /// <summary>
        /// Executes a single-row query, returning the data typed as T.
        /// </summary>
        T QueryFirstOrDefault<T>(string sql, object param, int? commandTimeout = null, CommandType? commandType = CommandType.Text);

        /// <summary>
        /// Return a dynamic object with properties matching the columns.
        /// </summary>
        dynamic QueryFirstOrDefault(string sql, object param, int? commandTimeout = null, CommandType? commandType = CommandType.Text);

        /// <summary>
        /// Execute a single-row query asynchronously using Task.
        /// </summary>
        Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param, int? commandTimeout = null, CommandType? commandType = CommandType.Text);

        /// <summary>
        /// Execute a single-row query asynchronously using Task.
        /// </summary>
        Task<dynamic> QueryFirstOrDefaultAsync(string sql, object param, int? commandTimeout = null, CommandType? commandType = CommandType.Text);

        /// <summary>
        /// Executes a single-row query, returning the data typed as T.
        /// </summary>
        T QuerySingleOrDefault<T>(string sql, object param, int? commandTimeout = null, CommandType? commandType = CommandType.Text);

        /// <summary>
        /// Return a dynamic object with properties matching the columns.
        /// </summary>
        dynamic QuerySingleOrDefault(string sql, object param, int? commandTimeout = null, CommandType? commandType = CommandType.Text);

        /// <summary>
        ///  Execute a single-row query asynchronously using Task.
        /// </summary>
        Task<T> QuerySingleOrDefaultAsync<T>(string sql, object param, int? commandTimeout = null, CommandType? commandType = CommandType.Text);

        /// <summary>
        /// Execute a single-row query asynchronously using Task.
        /// </summary>
        Task<dynamic> QuerySingleOrDefaultAsync(string sql, object param, int? commandTimeout = null, CommandType? commandType = CommandType.Text);

        /// <summary>
        /// Execute parameterized SQL.
        /// </summary>
        /// <returns> The number of rows affected. </returns>
        int Execute(string sql, object param, int? commandTimeout = null, CommandType? commandType = CommandType.Text);

        /// <summary>
        /// Execute parameterized SQL with auto commit
        /// </summary>
        /// <returns> The number of rows affected. </returns>
        int ExecuteAutoCommit(string sql, object param, int? commandTimeout = null, CommandType? commandType = CommandType.Text);

        /// <summary>
        /// Execute a command asynchronously using Task.
        /// </summary>
        /// <returns> The number of rows affected. </returns>
        Task<int> ExecuteAsync(string sql, object param, int? commandTimeout = null, CommandType? commandType = CommandType.Text);


        /// <summary>
        /// Execute parameterized SQL that selects a single value.
        /// </summary>
        /// <returns> The first cell selected as System.Object. </returns>
        object ExecuteScalar(string sql, object param, int? commandTimeout = null, CommandType? commandType = CommandType.Text);

        /// <summary>
        /// Execute parameterized SQL that selects a single value.
        /// </summary>
        /// <returns> The first cell selected as System.Object. </returns>
        Task<object> ExecuteScalarAsync(string sql, object param, int? commandTimeout = null, CommandType? commandType = CommandType.Text);
    }
}
