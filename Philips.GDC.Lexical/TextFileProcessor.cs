using Microsoft.Extensions.Logging;
using Philips.GDC.Interface;
using System.Diagnostics.CodeAnalysis;

namespace Philips.GDC.Lexical
{
    /// <summary>
    /// Responsible for reading and writing from/to file
    /// </summary>
    internal class TextFileProcessor : IFileProcessor
    {
        private const int DEFAULT_BUFFER_SIZE = 1056;
        private const FileOptions DEFAULT_FILE_OPTIONS = FileOptions.Asynchronous | FileOptions.SequentialScan;
        private readonly ILogger<IFileProcessor> _logger;

        /// <summary>
        /// C'TOR
        /// Initialize logger
        /// </summary>
        /// <param name="logger"></param>
        public TextFileProcessor(ILogger<IFileProcessor> logger)
        {
            _logger = logger;
        }

        ///<inheritdoc/>
        public (bool IsValid, string ErrorMessage) IsValidFile(string filePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filePath)) return (false, "File path is null or empty");
                string fullPath = Path.GetFullPath(filePath);
                FileInfo fileInfo = new FileInfo(fullPath);
                if (fileInfo.Extension != ".txt")
                    return (false, "File is not a text file. Please enter a text file");
                if (!fileInfo.Exists)
                    return (false, $"File {fullPath} does not exists. Please enter a valid file path");
                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                string errorMessage = $"Error occurred while validating file path {filePath}";
                _logger.LogError(ex, errorMessage);
                return (false, $"{errorMessage}. Error:{ex.Message}");
            }
        }

        [ExcludeFromCodeCoverage]
        ///<inheritdoc/>
        public async IAsyncEnumerable<string> ReadLinesAsync(string filePath)
        {
            string fullPath = Path.GetFullPath(filePath);
            if (string.IsNullOrEmpty(fullPath)) yield return string.Empty;
            using FileStream fileStream = new(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read, DEFAULT_BUFFER_SIZE, DEFAULT_FILE_OPTIONS);
            using StreamReader reader = new(fileStream);
            string line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                yield return line;
            }
        }

        [ExcludeFromCodeCoverage]
        ///<inheritdoc/>
        public async Task WriteAsync(string filePath, string content)
        {
            try
            {
                string fullPath = Path.GetFullPath(filePath);
                _logger.LogInformation($"Writing following content to XML file: {content}");
                if (string.IsNullOrEmpty(fullPath)) return;
                var destinationFilePath = Path.ChangeExtension(fullPath, ".xml");
                using var fileStream = new FileStream(destinationFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
                using var writer = new StreamWriter(fileStream);
                await writer.WriteAsync(content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while connecting writer to file {filePath}");
            }
        }
    }
}
