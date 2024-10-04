using System.Data;
using DataAccess.Models;

namespace DataAccess.Repositories.Interfaces
{
    public interface INumberRepository
    {

        /// <summary>
        /// Retrieves a collection of number data.
        /// </summary>
        /// <returns>
        /// The task result contains an enumerable
        /// collection of NumberModel objects.
        /// </returns>
        Task<IEnumerable<NumberModel>> GetNumberData();

        /// <summary>
        /// Inserts a bulk collection of number data.
        /// </summary>
        /// <param name="numberData">An enumerable collection of NumberModel objects to be inserted.</param>
        /// <returns>
        /// The task result indicates whether the bulk insert was successful (true) or not (false).
        /// </returns>
        Task<bool> BulkNumberInsert(IEnumerable<NumberModel> numberData);
    }
}
