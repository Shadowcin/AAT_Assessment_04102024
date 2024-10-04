using DataAccess.Models;
using DataAccess.Repositories.Interfaces;
using System.Data;
using Dapper;

namespace DataAccess.Repositories
{
    public class NumberRepository(IDbConnection connection) : INumberRepository
    {
        private readonly IDbConnection _connection = connection ?? throw new ArgumentNullException(nameof(connection));

        /// <inheritdoc />
        public async Task<IEnumerable<NumberModel>> GetNumberData()
        {
            const string sqlCommand = "SELECT Value, IsPrime FROM Number;";
            try
            {
                if (_connection.State != ConnectionState.Open)
                {
                    _connection.Open();
                }

                return await _connection.QueryAsync<NumberModel>(sqlCommand);
            }
            catch
            {
                throw new Exception("An error occurred while trying to retrieve the Number values.");
            }
            finally
            {
                if (_connection.State == ConnectionState.Open)
                {
                    _connection.Close();
                }
            }
        }

        /// <inheritdoc />
        public async Task<bool> BulkNumberInsert(IEnumerable<NumberModel> numberData)
        {
            const string sqlCommand = "INSERT INTO Number (Value, IsPrime) VALUES (@Value, @IsPrime);";
            IDbTransaction? transaction = null;
            try
            {
                if (_connection.State != ConnectionState.Open)
                {
                    _connection.Open();
                }

                transaction = _connection.BeginTransaction();
                _ = await _connection.ExecuteAsync("DELETE FROM Number"); //Clear the records of the last computation
                var result = await _connection.ExecuteAsync(sql: sqlCommand, param: numberData, transaction: transaction);
                transaction.Commit();

                return result > 0;
            }
            catch
            {
                transaction?.Rollback();
                throw new Exception("The transaction is in an uncommittable state. Rolling back transaction.");
            }
            finally
            {
                if (_connection.State == ConnectionState.Open)
                {
                    _connection.Close();
                }
            }
        }
    }
}
