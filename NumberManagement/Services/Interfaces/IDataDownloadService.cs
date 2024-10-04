namespace NumberManagement.Services.Interfaces
{
    public interface IDataDownloadService
    {
        /// <summary>
        /// Downloads the data in binary format.
        /// </summary>
        /// <returns>
        /// A task with a byte array as the result containing the binary data.
        /// </returns>
        Task<bool> DownloadBinary();

        /// <summary>
        /// Downloads the data in XML format.
        /// </summary>
        /// <returns>
        /// A task with a byte array as the result containing the XML data.
        /// </returns>
        Task<bool> DownloadXml();
    }
}
