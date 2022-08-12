using Microsoft.Extensions.Logging;
using Moq;

using TestAssignment.GDC.Lexical;
using TestAssignment.GDC.Utilities;
using TTestAssignment.GDC.Lexical;

namespace TestAssignment.GDC.Test
{
    [TestClass]
    public class LexicalControllerTest
    {
        ILexicalController _lexicalController;
        private ILogger<ILexicalController> _logger;
        private IFileProcessor _fileProcessor;
        private ILexicalNodeProcessor _lexicalNodeProcessor;
        private IGcdNodeCreator _gcdNodeCreator;
        private Mock<IApplicationConfiguration> _configuration;

        [TestInitialize]
        public void Init()
        {
            _logger = new Mock<ILogger<ILexicalController>>().Object;
            _fileProcessor = new TextFileProcessorMock(TestResult);

            _configuration = new Mock<IApplicationConfiguration>();
            _configuration.Setup(x => x.MaxXmlProcessor).Returns(2);
            _configuration.Setup(x => x.RootNodeName).Returns("testnode");
            _lexicalNodeProcessor = new LexicalNodeProcessor(new Mock<ILogger<ILexicalNodeProcessor>>().Object);
            _gcdNodeCreator = new GcdNodeCreator(_configuration.Object);
            _lexicalController = new LexicalController(_logger, _fileProcessor, _lexicalNodeProcessor, _gcdNodeCreator, _configuration.Object);
        }

        [TestMethod]
        public void LexicalControllerTest_Parse_Test()
        {
            _lexicalController.Parse(@"..\test.txt");
        }

        public void TestResult(string finalOutput)
        {
            string expectedResult = @"<testnode>
  <indi id=""@I1@"">
    <name value=""Sudhir / Kumar /"">
      <surn>Kumar</surn>
      <givn>Sudhir</givn>
    </name>
    <sex>M</sex>
  </indi>
</testnode>";
            Assert.AreEqual(expectedResult, finalOutput);
        }
    }
}
