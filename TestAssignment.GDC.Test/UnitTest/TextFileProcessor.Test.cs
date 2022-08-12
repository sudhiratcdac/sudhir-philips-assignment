using Microsoft.Extensions.Logging;
using Moq;
using TestAssignment.GDC.Interface;
using TestAssignment.GDC.Lexical;

namespace TestAssignment.GDC.Test
{
    [TestClass]
    public class TextFileProcessorTest
    {
        private IFileProcessor fileProcessor;
        private ILogger<IFileProcessor> _logger;
        private string _filePath;

        [TestInitialize]
        public void Init()
        {
            _logger = new Mock<ILogger<IFileProcessor>>().Object;
            fileProcessor = new TextFileProcessor(_logger);
            _filePath = Path.Combine(Directory.GetCurrentDirectory(), "TestFiles");
        }

        [TestMethod]
        public void TextFileProcessor_IsValidFile_Empty_FilePath_Test()
        {
            var result = fileProcessor.IsValidFile(string.Empty);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("File path is null or empty", result.ErrorMessage);
            result = fileProcessor.IsValidFile(null);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("File path is null or empty", result.ErrorMessage);
            result = fileProcessor.IsValidFile("\t");
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("File path is null or empty", result.ErrorMessage);
        }


        [TestMethod]
        public void TextFileProcessor_IsValidFile_No_TxtFile_Test()
        {
            var result = fileProcessor.IsValidFile(Path.Combine(_filePath, "test.xml"));
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("File is not a text file. Please enter a text file", result.ErrorMessage);
        }

        [TestMethod]
        public void TextFileProcessor_IsValidFile_File_Not_Exists_Test()
        {
            var filePath = Path.Combine(_filePath, "test1.txt");
            var result = fileProcessor.IsValidFile(filePath);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual($"File {filePath} does not exists. Please enter a valid file path", result.ErrorMessage);
        }
    }
}
