using Microsoft.Extensions.Logging;
using Moq;
using Philips.GDC.Interface;
using Philips.GDC.Lexical;

namespace Philips.GDC.Test
{
    [TestClass]
    public class LexicalNodeProcessorTest
    {
        ILexicalNodeProcessor _lexicalNodeProcessor;
        private ILogger<ILexicalNodeProcessor> _logger;

        [TestInitialize]
        public void Init()
        {
            _logger = new Mock<ILogger<ILexicalNodeProcessor>>().Object;
            _lexicalNodeProcessor = new LexicalNodeProcessor(_logger);
        }
        [TestMethod]
        public void LexicalNodeProcessor_AnalyzeAndCreateNode_Length_Equals_One_Test()
        {
            var result = _lexicalNodeProcessor.AnalyzeAndCreateNode("0", null);
            Assert.IsFalse(result.IsValid);
            Assert.IsNull(result.Node);
        }

        [TestMethod]
        public void LexicalNodeProcessor_AnalyzeAndCreateNode_Length_Equals_Two_With_Id_Test()
        {
            var result = _lexicalNodeProcessor.AnalyzeAndCreateNode("0 @I1@", null);
            Assert.IsFalse(result.IsValid);
            Assert.IsNull(result.Node);
        }

        [TestMethod]
        public void LexicalNodeProcessor_AnalyzeAndCreateNode_Length_Equals_Two_With_Invalid_Level_Test()
        {
            var result = _lexicalNodeProcessor.AnalyzeAndCreateNode("@I1@ 0", null);
            Assert.IsFalse(result.IsValid);
            Assert.IsNull(result.Node);
        }

        [TestMethod]
        public void LexicalNodeProcessor_AnalyzeAndCreateNode_Level_Zero_Test()
        {
            var result = _lexicalNodeProcessor.AnalyzeAndCreateNode("0 @I1@ INDI", null);
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual("indi", result.Node.Name);
            Assert.AreEqual(1, result.Node.Attributes.Count);
            Assert.AreEqual("id", result.Node.Attributes.First().Name);
            Assert.AreEqual("@I1@", result.Node.Attributes.First().Value);
            Assert.IsNull(result.Node.Value);
        }

        [TestMethod]
        public void LexicalNodeProcessor_AnalyzeAndCreateNode_Level_One_Test()
        {
            var result = _lexicalNodeProcessor.AnalyzeAndCreateNode("0 @I1@ INDI", null);
            result = _lexicalNodeProcessor.AnalyzeAndCreateNode("1 NAME Sudhir /Kumar/", result.Node);

            Assert.IsTrue(result.IsValid);
            Assert.AreEqual("name", result.Node.Name);
            Assert.AreEqual(1, result.Node.Attributes.Count);
            Assert.AreEqual("value", result.Node.Attributes.First().Name);
            Assert.AreEqual("Sudhir /Kumar/", result.Node.Attributes.First().Value);
            Assert.IsNull(result.Node.Value);

            Assert.IsNotNull(result.Node.Previous);
            Assert.AreEqual(1, result.Node.Previous.Childs.Count);
            Assert.AreEqual("indi", result.Node.Previous.Name);
            Assert.AreEqual(1, result.Node.Previous.Attributes.Count);
            Assert.AreEqual("id", result.Node.Previous.Attributes.First().Name);
            Assert.AreEqual("@I1@", result.Node.Previous.Attributes.First().Value);
            Assert.IsNull(result.Node.Previous.Value);
        }

        [TestMethod]
        public void LexicalNodeProcessor_AnalyzeAndCreateNode_Level_Two_Test()
        {
            var result = _lexicalNodeProcessor.AnalyzeAndCreateNode("0 @I1@ INDI", null);
            var firstNode = result;
            result = _lexicalNodeProcessor.AnalyzeAndCreateNode("1 NAME Sudhir /Kumar/", result.Node);
            result = _lexicalNodeProcessor.AnalyzeAndCreateNode("2 SURN Kumar", result.Node);
            result = _lexicalNodeProcessor.AnalyzeAndCreateNode("2 GIVN Sudhir", result.Node);
            result = _lexicalNodeProcessor.AnalyzeAndCreateNode("1 SEX M", result.Node);



            Assert.IsTrue(firstNode.IsValid);
            Assert.AreEqual("indi", firstNode.Node.Name);
            Assert.AreEqual(1, firstNode.Node.Attributes.Count);
            Assert.AreEqual("id", firstNode.Node.Attributes.First().Name);
            Assert.AreEqual("@I1@", firstNode.Node.Attributes.First().Value);
            Assert.IsNull(firstNode.Node.Value);
            Assert.AreEqual(2, firstNode.Node.Childs.Count);

            var nameNode = firstNode.Node.Childs.First();

            Assert.AreEqual("indi", nameNode.Previous.Name);
            Assert.AreEqual("name", nameNode.Name);
            Assert.AreEqual(1, nameNode.Attributes.Count);
            Assert.AreEqual("value", nameNode.Attributes.First().Name);
            Assert.AreEqual("Sudhir /Kumar/", nameNode.Attributes.First().Value);
            Assert.IsNull(firstNode.Node.Value);
            Assert.AreEqual(2, nameNode.Childs.Count);

            var snameNode = nameNode.Childs.First();

            Assert.AreEqual("name", snameNode.Previous.Name);
            Assert.AreEqual("surn", snameNode.Name);
            Assert.AreEqual(0, snameNode.Attributes.Count);
            Assert.AreEqual("Kumar", snameNode.Value);

            var gnameNode = nameNode.Childs.Skip(1).First();

            Assert.AreEqual("name", gnameNode.Previous.Name);
            Assert.AreEqual("givn", gnameNode.Name);
            Assert.AreEqual(0, gnameNode.Attributes.Count);
            Assert.AreEqual("Sudhir", gnameNode.Value);

            var sexNode = firstNode.Node.Childs.Skip(1).First();

            Assert.AreEqual("indi", nameNode.Previous.Name);
            Assert.AreEqual("sex", sexNode.Name);
            Assert.AreEqual(0, sexNode.Attributes.Count);
            Assert.AreEqual("M", sexNode.Value);
        }
    }
}
