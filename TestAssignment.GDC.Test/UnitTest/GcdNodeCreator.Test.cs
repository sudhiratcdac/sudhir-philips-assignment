using Moq;
using TestAssignment.GDC.Dto;
using TestAssignment.GDC.Interface;
using TestAssignment.GDC.Lexical;

namespace TestAssignment.GDC.Test
{
    [TestClass]
    public class GcdNodeCreatorTest
    {
        private IGcdNodeCreator _gcdNodeCreator;
        private Mock<IApplicationConfiguration> _configuration;

        [TestInitialize]
        public void Init()
        {
            _configuration = new Mock<IApplicationConfiguration>();
            _configuration.Setup(x => x.MaxXmlProcessor).Returns(2);
            _configuration.Setup(x => x.RootNodeName).Returns("testnode");

            _gcdNodeCreator = new GcdNodeCreator(_configuration.Object);
            _gcdNodeCreator.OnComplete += XmlProcessor_OnProcessCompleteHandler;
        }

        [TestMethod]
        public void GcdNodeCreator_CreateTree_Test()
        {
            _gcdNodeCreator.SetUpRootNode(new NodeInput { Level = 0, Name = "testnode" }).GetAwaiter().GetResult();
            var rootNode = new NodeInput
            {
                Level = 0,
                Name = "indi",
                Attributes = new List<(string, string)> { ("id", "@I1@") },
                NodeOrder = 0,
            };

            var nameNode = new NodeInput
            {
                Level = 1,
                Name = "name",
                Attributes = new List<(string, string)> { ("value", "Sudhir / Kumar /") }
            };
            var sNameNode = new NodeInput
            {
                Level = 2,
                Name = "surn",
                Value = "Kumar"
            };
            var gNameNode = new NodeInput
            {
                Level = 2,
                Name = "givn",
                Value = "Sudhir"
            };
            var sexNode = new NodeInput
            {
                Level = 1,
                Name = "sex",
                Value = "M"
            };

            nameNode.Previous = rootNode;
            sexNode.Previous = rootNode;
            rootNode.Childs.Add(nameNode);
            rootNode.Childs.Add(sexNode);

            sNameNode.Previous = gNameNode.Previous = nameNode;
            nameNode.Childs.Add(sNameNode);
            nameNode.Childs.Add(gNameNode);
            _gcdNodeCreator.CreateSubTree(rootNode, true);
        }

        private void XmlProcessor_OnProcessCompleteHandler(object sender, NodeToXmlArgs e)
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
            Assert.AreEqual(expectedResult, e.XmlValue);
        }
    }
}
