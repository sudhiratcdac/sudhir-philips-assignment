using Microsoft.Extensions.Logging;
using Philips.GDC.Dto;
using Philips.GDC.Interface;

namespace Philips.GDC.Lexical
{
    /// <summary>
    /// Responsible for parsing string to the NodeInput class
    /// </summary>
    internal class LexicalController : ILexicalController
    {
        private readonly ILogger<LexicalController> _logger;
        private readonly IFileProcessor _fileProcessor;
        private readonly ILexicalNodeProcessor _lexicalAnalyzer;
        private readonly IGcdNodeCreator _gcdNodeCreator;
        private readonly IApplicationConfiguration _configuration;

        public LexicalController(ILogger<LexicalController> logger, IFileProcessor fileProcessor, ILexicalNodeProcessor lexicalAnalyzer, IGcdNodeCreator gcdNodeCreator, IApplicationConfiguration configuration)
        {
            _logger = logger;
            _fileProcessor = fileProcessor;
            _lexicalAnalyzer = lexicalAnalyzer;
            _gcdNodeCreator = gcdNodeCreator;
            _configuration = configuration;
            _gcdNodeCreator.OnComplete += GcDPubSub_OnProcessCompleteHandler;
        }

        public async Task Parse(string filePath)
        {
            var fileValidationResult = _fileProcessor.IsValidFile(filePath);
            if (!fileValidationResult.IsValid)
            {
                Console.WriteLine(fileValidationResult.ErrorMessage);
                return;
            }
            _logger.LogInformation($"Processing file: {filePath}");
            IAsyncEnumerator<string> e = _fileProcessor.ReadLinesAsync().GetAsyncEnumerator();
            try
            {
                uint nodeOrder = 0;
                NodeInput previousNode = null;
                NodeInput firstNode = null;
                List<Task> nodeTasks = new List<Task>();

                nodeTasks.Add(_gcdNodeCreator.SetUpRootNode(new NodeInput { Level = 0, Name = _configuration.RootNodeName }));

                while (await e.MoveNextAsync())
                {
                    _logger.LogInformation(e.Current);
                    var nodeResult = _lexicalAnalyzer.AnalyzeAndCreateNode(e.Current, ref previousNode);
                    if (nodeResult.IsValid)
                    {
                        if (nodeResult.Node.Level == 0)
                        {
                            if (nodeOrder > 0)
                            {
                                nodeTasks.Add(_gcdNodeCreator.CreateSubTree(firstNode, false));
                            }
                            firstNode = nodeResult.Node;
                            firstNode.NodeOrder = nodeOrder;
                            nodeOrder++;
                        }
                    }
                }
                nodeTasks.Add(_gcdNodeCreator.CreateSubTree(firstNode, true));
                await Task.WhenAll(nodeTasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while parsing nodes");
            }
            finally { if (e != null) await e.DisposeAsync(); }
        }

        private void GcDPubSub_OnProcessCompleteHandler(object sender, NodeToXmlArgs e)
        {
            _fileProcessor.WriteAsync(e.XmlValue);
        }
    }
}
