using Philips.GDC.Interface;

namespace Philips.GDC.Test
{
    internal class TextFileProcessorMock : IFileProcessor
    {
        private readonly Action<string> writeResult;
        string[] nodeStrings = { "0 @I1@ INDI", "1 NAME Sudhir / Kumar /", "2 SURN Kumar", "2 GIVN Sudhir", "1 SEX M" };

        public TextFileProcessorMock(Action<string> writeResult)
        {
            this.writeResult = writeResult;
        }

        public (bool IsValid, string ErrorMessage) IsValidFile(string filePath)
        {
            return (true, "");
        }

        public async IAsyncEnumerable<string> ReadLinesAsync(string filePath)
        {
            foreach (var item in nodeStrings)
            {
                yield return item;
            }
            await Task.Yield();
        }

        public Task WriteAsync(string filePath, string content)
        {
            if (writeResult != null) writeResult(content);
            return Task.CompletedTask;
        }
    }
}
