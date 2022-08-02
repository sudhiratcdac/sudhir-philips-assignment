using Philips.GDC.Dto;
using Philips.GDC.Lexical;

namespace Philips.GDC.Test
{
    [TestClass]
    public class XmlProcessorTest
    {
        XmlProcessor processor = null;
        [TestInitialize]
        public void Init()
        {
            processor = new XmlProcessor();
        }

        [TestMethod]
        public void XmlProcessor_Process_Level0_Success()
        {
            processor.OnProcessComplete += XmlProcessor_Process_Level0_Success_OnProcessCompleteHandler;

            processor.Process(new NodeInput { Level = 0, Name = "indi", Attributes = new List<(string, string)> { ("id", "@I0001@") } }).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private void XmlProcessor_Process_Level0_Success_OnProcessCompleteHandler(object sender, NodeToXmlArgs e)
        {
            Assert.IsNotNull(sender);
            Assert.IsNotNull(e);
            Assert.AreSame(processor, sender);
            Assert.AreEqual("indi", e.SourceNode.Name);
            Assert.AreEqual("id", e.SourceNode.Attributes.First().Name);
            Assert.AreEqual("@I0001@", e.SourceNode.Attributes.First().Value);
            Assert.AreEqual(@"<indi id=""@I0001@"" />", e.XmlValue);
        }

        [TestMethod]
        public void XmlProcessor_Process_Level1_Name_With_Value_Success()
        {
            processor.OnProcessComplete += XmlProcessor_Process_Level1_Name_With_Value_Success_OnProcessCompleteHandler;
            var level0Node = new NodeInput
            {
                Level = 0,
                Name = "indi",
                Attributes = new List<(string, string)> { ("id", "@I0001@") }
            };

            var nameNode = new NodeInput
            {
                Level = 1,
                Name = "name",
                Attributes = new List<(string, string)> { ("value", "Elizabeth Alexandra Mary /Windsor/") },
                Previous = level0Node
            };
            level0Node.Childs.Add(nameNode);

            processor.Process(level0Node).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private void XmlProcessor_Process_Level1_Name_With_Value_Success_OnProcessCompleteHandler(object sender, NodeToXmlArgs e)
        {
            Assert.IsNotNull(sender);
            Assert.IsNotNull(e);
            Assert.AreSame(processor, sender);
            Assert.AreEqual(@$"<indi id=""@I0001@"">{Environment.NewLine}  <name value=""Elizabeth Alexandra Mary /Windsor/"" />{Environment.NewLine}</indi>", e.XmlValue);
        }

        [TestMethod]
        public void XmlProcessor_Process_Level1_Name_Without_Value_Success()
        {
            processor.OnProcessComplete += XmlProcessor_Process_Level1_Name_Without_Value_Success_OnProcessCompleteHandler;
            var level0Node = new NodeInput
            {
                Level = 0,
                Name = "indi",
                Attributes = new List<(string, string)> { ("id", "@I0001@") }
            };

            var nameNode = new NodeInput
            {
                Level = 1,
                Name = "name",
                Attributes = new List<(string, string)> { ("value", null) },
                Previous = level0Node
            };
            level0Node.Childs.Add(nameNode);

            processor.Process(level0Node).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private void XmlProcessor_Process_Level1_Name_Without_Value_Success_OnProcessCompleteHandler(object sender, NodeToXmlArgs e)
        {
            Assert.IsNotNull(sender);
            Assert.IsNotNull(e);
            Assert.AreSame(processor, sender);
            Assert.AreEqual(@$"<indi id=""@I0001@"">{Environment.NewLine}  <name />{Environment.NewLine}</indi>", e.XmlValue);
        }

        [TestMethod]
        public void XmlProcessor_Process_Level2_Success()
        {
            processor.OnProcessComplete += XmlProcessor_Process_Level2_Success_OnProcessCompleteHandler;
            var level0Node = new NodeInput
            {
                Level = 0,
                Name = "indi",
                Attributes = new List<(string, string)> { ("id", "@I0001@") }
            };

            var nameNode = new NodeInput
            {
                Level = 1,
                Name = "name",
                Attributes = new List<(string, string)> { ("value", "Elizabeth Alexandra Mary /Windsor/") },
                Previous = level0Node
            };
            level0Node.Childs.Add(nameNode);
            var birthNode = new NodeInput
            {
                Level = 1,
                Name = "birt",
                Value = null,
                Previous = level0Node
            };
            level0Node.Childs.Add(birthNode);

            var birthDateNode = new NodeInput
            {
                Level = 2,
                Name = "date",
                Value = "21 Apr 1926",
                Previous = birthNode
            };
            birthNode.Childs.Add(birthDateNode);

            var birthPlaceNode = new NodeInput
            {
                Level = 2,
                Name = "plac",
                Value = "17 Bruton Street, London, W1",
                Previous = birthNode
            };
            birthNode.Childs.Add(birthPlaceNode);

            var sexNode = new NodeInput
            {
                Level = 1,
                Name = "sex",
                Value = "M",
                Previous = level0Node
            };
            level0Node.Childs.Add(sexNode);


            processor.Process(level0Node).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private void XmlProcessor_Process_Level2_Success_OnProcessCompleteHandler(object sender, NodeToXmlArgs e)
        {
            string expectedOutput = $@"<indi id=""@I0001@"">
  <name value=""Elizabeth Alexandra Mary /Windsor/"" />
  <birt>
    <date>21 Apr 1926</date>
    <plac>17 Bruton Street, London, W1</plac>
  </birt>
  <sex>M</sex>
</indi>";
            Assert.IsNotNull(sender);
            Assert.IsNotNull(e);
            Assert.AreSame(processor, sender);
            Assert.AreEqual(expectedOutput, e.XmlValue);
        }
    }
}