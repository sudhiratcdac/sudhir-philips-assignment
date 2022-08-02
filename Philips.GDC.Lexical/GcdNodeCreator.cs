using Philips.GDC.Dto;
using Philips.GDC.Interface;
using System.Collections.Concurrent;
using System.Xml.Linq;

namespace Philips.GDC.Lexical
{
    /// <summary>
    /// Responsible for creating and arranging all the nodes specified in the source file
    /// </summary>
    internal class GcdNodeCreator : IGcdNodeCreator
    {
        private readonly ConcurrentQueue<NodeInput> _nodes;
        private readonly ConcurrentQueue<XmlProcessor> _xmlProcessors;
        private readonly int _maxXmlProcessor;
        private bool _isProcessingStarted = false;
        private XElement _rootDocElement;
        private uint _currentDocIndex;
        private uint _totalNodeCount = 0;
        private ConcurrentDictionary<uint, XElement> _processedNodes;

        ///<inheritdoc/>
        public EventHandler<NodeToXmlArgs> OnComplete { get; set; }

        /// <summary>
        /// C'TOR
        /// Initialize the Queue for both node to created, processor that would  create node, processed nodes and 
        /// maximum number of processor that would be kick started at the start of processing
        /// </summary>
        /// <param name="configuration"></param>
        public GcdNodeCreator(IApplicationConfiguration configuration)
        {
            _nodes = new ConcurrentQueue<NodeInput>();
            _xmlProcessors = new ConcurrentQueue<XmlProcessor>();
            _processedNodes = new ConcurrentDictionary<uint, XElement>();
            _maxXmlProcessor = configuration.MaxXmlProcessor;
            InitializeProcessorPool(_maxXmlProcessor);
        }

        ///<inheritdoc/>
        public async Task SetUpRootNode(NodeInput rootNode)
        {
            var processor = new XmlProcessor();
            _rootDocElement = await processor.Process(rootNode);
        }

        ///<inheritdoc/>
        public async Task CreateSubTree(NodeInput node, bool isLastNode)
        {
            _nodes.Enqueue(node);
            _totalNodeCount++;

            if (!_isProcessingStarted)
            {
                _isProcessingStarted = true;
                await Task.Run(() => CreateSubTree());
            }
            if (isLastNode)
            {
                _isProcessingStarted = false;
            }
        }

        /// <summary>
        /// Create sub tree for the nodes added in queue using the processor responsible to create xml node
        /// </summary>
        private async void CreateSubTree()
        {
            while (_isProcessingStarted || _nodes.Any())
            {
                if (_xmlProcessors.Any())
                {
                    if (_nodes.TryDequeue(out var nodeToProcess))
                    {
                        if (_xmlProcessors.TryDequeue(out var processor))
                        {
                            await processor.Process(nodeToProcess);
                        }
                    }
                }
                await Task.Delay(10);
            }
        }

        /// <summary>
        /// Initialize XML node processor with the number specified in appSettings.json
        /// </summary>
        /// <param name="processorCount">Number of processor to be created</param>
        private void InitializeProcessorPool(int processorCount)
        {
            for (int i = 0; i < processorCount; i++)
            {
                var processor = new XmlProcessor();
                processor.OnProcessComplete += XmlProcessor_OnProcessCompleteHandler;
                _xmlProcessors.Enqueue(processor);
            }
        }

        /// <summary>
        /// After mapping the source string to corresponding node, add the node to the root node.
        /// Add processor to the queue once again for further uses
        /// </summary>
        /// <param name="sender">XmlProcessor</param>
        /// <param name="e">NodeToXmlArgs to contain the source node data and corresponding XML node</param>
        private void XmlProcessor_OnProcessCompleteHandler(object sender, NodeToXmlArgs e)
        {
            AddChildNodes(e);
            if (OnComplete != null && _currentDocIndex >= _totalNodeCount)
            {
                _currentDocIndex = 0;
                _totalNodeCount = 0;
                OnComplete(null, new NodeToXmlArgs(null, _rootDocElement));
            }
            _xmlProcessors.Enqueue((XmlProcessor)sender);
        }

        /// <summary>
        /// Add Child node to root node in the order specified in the document
        /// </summary>
        /// <param name="e">NodeToXmlArgs to contain the source node data and corresponding XML node</param>
        private void AddChildNodes(NodeToXmlArgs e)
        {
            if (_currentDocIndex == e.SourceNode.NodeOrder || _currentDocIndex + 1 == e.SourceNode.NodeOrder)
            {
                _rootDocElement.Add(e.XmlNode);
                _currentDocIndex++;
                while (_processedNodes.ContainsKey(_currentDocIndex))
                {
                    _rootDocElement.Add(e.XmlNode);
                    _currentDocIndex++;
                }
            }
            else
            {
                _processedNodes.TryAdd(e.SourceNode.NodeOrder, e.XmlNode);
            }
        }
    }
}
