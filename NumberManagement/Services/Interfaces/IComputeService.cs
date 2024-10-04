using DataAccess.Models;

namespace NumberManagement.Services.Interfaces
{
    public interface IComputeService
    {
        /// <summary>
        /// Starts the number generation process asynchronously.
        /// </summary>
        /// <returns>
        /// Starts the data computation.
        /// </returns>
        Task StartAsync();

        /// <summary>
        /// Displays the current statistics of the computation.
        /// </summary>
        /// <returns>
        /// A <see cref="TotalsModel"/> object containing the statistics.
        /// </returns>
        TotalsModel DisplayStatistics();

        /// <summary>
        /// Saves the computed data to the database.
        /// </summary>
        Task<bool> SaveComputeData();
    }
}

