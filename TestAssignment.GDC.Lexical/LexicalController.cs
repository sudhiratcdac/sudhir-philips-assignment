using Microsoft.Extensions.Logging;
using System.Diagnostics;
using TestAssignment.GDC.Dto;
using TestAssignment.GDC.Utilities;
using TTestAssignment.GDC.Lexical;

namespace TestAssignment.GDC.Lexical
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

        ///<inheritdoc/>
        public async Task Parse(string sourceStringOrFilePath)
        {
            _filePath = sourceStringOrFilePath;
            var fileValidationResult = _fileProcessor.IsValidFile(sourceStringOrFilePath);
            if (!fileValidationResult.IsValid)
            {
                Console.WriteLine(fileValidationResult.ErrorMessage);
                return;
            }
            await ParseNodes(sourceStringOrFilePath);
        }

        /// <summary>
        /// Parse and create node
        /// </summary>
        /// <param name="sourceStringOrFilePath"></param>
        /// <returns></returns>
        private async Task ParseNodes(string sourceStringOrFilePath)
        {
            _logger.LogInformation($"Processing file: {sourceStringOrFilePath}");
            IAsyncEnumerator<string> nodeString = _fileProcessor.ReadLinesAsync(sourceStringOrFilePath).GetAsyncEnumerator();
            try
            {
                Stopwatch sw = new Stopwatch();
                uint nodeOrder = 0;
                NodeInput previousNode = default;
                NodeInput firstNode = null;
                List<Task> nodeTasks = new List<Task>();
                int nodeCounter = 0;
                sw.Start();
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
                if (firstNode != null)
                    nodeTasks.Add(_gcdNodeCreator.CreateSubTree(firstNode, true));
                sw.Stop();
                var ticksFileRead = sw.ElapsedMilliseconds;
                sw.Restart();
                await Task.WhenAll(nodeTasks);
                sw.Stop();
                _logger.LogInformation($"File read time: {ticksFileRead} milliseconds, Processing time {sw.ElapsedMilliseconds}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while parsing nodes");
            }
            finally { if (nodeString != null) await nodeString.DisposeAsync(); }
        }

        /// <summary>
        /// Handler when all the nodes and been formed 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GcDPubSub_OnProcessCompleteHandler(object sender, NodeToXmlArgs e)
        {
            _fileProcessor.WriteAsync(_filePath, e.XmlValue);
        }
    }
}
