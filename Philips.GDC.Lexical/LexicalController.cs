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
        private readonly ILogger<ILexicalController> _logger;
        private readonly IFileProcessor _fileProcessor;
        private readonly ILexicalNodeProcessor _lexicalNodeProcessor;
        private readonly IGcdNodeCreator _gcdNodeCreator;
        private readonly IApplicationConfiguration _configuration;
        private string _filePath;

        public LexicalController(ILogger<ILexicalController> logger, IFileProcessor fileProcessor, ILexicalNodeProcessor lexicalNodeProcessor, IGcdNodeCreator gcdNodeCreator, IApplicationConfiguration configuration)
        {
            _logger = logger;
            _fileProcessor = fileProcessor;
            _lexicalNodeProcessor = lexicalNodeProcessor;
            _gcdNodeCreator = gcdNodeCreator;
            _configuration = configuration;
            _gcdNodeCreator.OnComplete += GcDPubSub_OnProcessCompleteHandler;
        }

        public async Task Parse(string filePath)
        {
            _filePath = filePath;
            var fileValidationResult = _fileProcessor.IsValidFile(filePath);
            if (!fileValidationResult.IsValid)
            {
                Console.WriteLine(fileValidationResult.ErrorMessage);
                return;
            }
            await ParseNodes(filePath);
        }

        private async Task ParseNodes(string filePath)
        {
            _logger.LogInformation($"Processing file: {filePath}");
            IAsyncEnumerator<string> nodeString = _fileProcessor.ReadLinesAsync(filePath).GetAsyncEnumerator();
            try
            {
                uint nodeOrder = 0;
                NodeInput previousNode = default;
                NodeInput firstNode = null;
                List<Task> nodeTasks = new List<Task>();
                int nodeCounter = 0;

                nodeTasks.Add(_gcdNodeCreator.SetUpRootNode(new NodeInput { Level = 0, Name = _configuration.RootNodeName }));
                List<string> invalidNodes = new List<string>();
                while (await nodeString.MoveNextAsync())
                {
                    nodeCounter++;
                    _logger.LogInformation(nodeString.Current);
                    var nodeResult = _lexicalNodeProcessor.AnalyzeAndCreateNode(nodeString.Current, previousNode);
                    if (nodeResult.IsValid)
                    {
                        previousNode = nodeResult.Node;

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
                    else
                    {
                        invalidNodes.Add($"{nodeCounter}: {nodeString.Current}");
                    }

                }
                _logger.LogInformation($"Total nodes found {nodeCounter}. Invalid nodes", string.Join(", ", invalidNodes.Select(x => x)));

                nodeTasks.Add(_gcdNodeCreator.CreateSubTree(firstNode, true));
                await Task.WhenAll(nodeTasks);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while parsing nodes");
            }
            finally { if (nodeString != null) await nodeString.DisposeAsync(); }
        }

        private void GcDPubSub_OnProcessCompleteHandler(object sender, NodeToXmlArgs e)
        {
            _fileProcessor.WriteAsync(_filePath, e.XmlValue);
        }
    }
}
