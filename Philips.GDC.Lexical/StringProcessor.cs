using Microsoft.Extensions.Logging;
using Philips.GDC.Interface;

namespace Philips.GDC.Lexical
{
    /// <summary>
    /// Responsible for handling string input
    /// </summary>
    internal class StringProcessor : IFileProcessor
    {
        private readonly ILogger<IFileProcessor> _logger;

        public StringProcessor(ILogger<IFileProcessor> logger)
        {
            _logger = logger;
        }

        ///<inheritdoc/>
        public (bool IsValid, string ErrorMessage) IsValidFile(string filePath)
        {
            return (true, "");
        }

        ///<inheritdoc/>
        public async IAsyncEnumerable<string> ReadLinesAsync(string filePath)
        {
            var splittedStrings = filePath.Split(Environment.NewLine);
            foreach (var nodeString in splittedStrings)
            {
                yield return nodeString;
            }
            await Task.Yield();
        }

        ///<inheritdoc/>
        public async Task WriteAsync(string filePath, string content)
        {
            _logger.LogInformation($"Final output: {Environment.NewLine} {content}");
            await Task.CompletedTask;
        }
    }
}
