using DataAccess.Repositories.Interfaces;
using NumberManagement.Services.Interfaces;
using System.Xml.Serialization;
using DataAccess.Models;
using NumberManagement.Helpers;

namespace NumberManagement.Services
{
    public class DataDownloadService(INumberRepository numberRepository) : IDataDownloadService
    {
        private readonly INumberRepository _numberRepository = numberRepository ?? throw new ArgumentNullException(nameof(numberRepository));

        /// <inheritdoc />
        public async Task<bool> DownloadBinary()
        {
            var batchSize = 100000;
            var binaryFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files/computeDataBinary.bin");
            var computeResultList = await _numberRepository.GetNumberData();
            return SaveAsBinaryFileInBatches(computeResultList, binaryFilePath, batchSize);
        }

        /// <inheritdoc />
        public async Task<bool> DownloadXml ()
        {
            var batchSize = 100000;
            var binaryFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files/computeDataXml.xml");
            var computeResultList = await _numberRepository.GetNumberData();
            return SaveAsXmlFileInBatches(computeResultList, binaryFilePath, batchSize);
        }

        private bool SaveAsXmlFileInBatches(IEnumerable<NumberModel> numbers, string filePath, int batchSize)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(List<NumberModel>));

                using var stream = new FileStream(filePath, FileMode.Create);
                using var writer = new StreamWriter(stream);
                writer.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                writer.WriteLine("<NumberModels>");

                foreach (var batch in numbers.Batch(batchSize))
                {
                    using var stringWriter = new StringWriter();
                    serializer.Serialize(stringWriter, batch.ToList());
                    writer.Write(stringWriter.ToString().Replace("<?xml version=\"1.0\" encoding=\"utf-8\"?>", "").Replace("<ArrayOfNumberModel>", "").Replace("</ArrayOfNumberModel>", ""));
                }

                writer.WriteLine("</NumberModels>");

                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool SaveAsBinaryFileInBatches(IEnumerable<NumberModel> numbers, string filePath, int batchSize)
        {
            try
            {
                using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                using var writer = new BinaryWriter(fs);
                foreach (var batch in numbers.Batch(batchSize))
                {
                    foreach (var number in batch)
                    {
                        writer.Write(number.Value);
                        writer.Write(number.IsPrime);
                    }

                    writer.Flush();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
