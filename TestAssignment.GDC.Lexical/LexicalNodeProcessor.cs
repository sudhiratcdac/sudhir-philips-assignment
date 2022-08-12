using Microsoft.Extensions.Logging;
using TestAssignment.GDC.Dto;
using TestAssignment.GDC.Interface;

namespace TestAssignment.GDC.Lexical
{
    /// <summary>
    /// Responsible for Analyzing and creating node based of the source string
    /// </summary>
    internal class LexicalNodeProcessor : ILexicalNodeProcessor
    {
        private readonly ILogger<ILexicalNodeProcessor> _logger;

        public LexicalNodeProcessor(ILogger<ILexicalNodeProcessor> logger)
        {
            _logger = logger;
        }

        ///<inheritdoc/>
        public (bool IsValid, NodeInput Node) AnalyzeAndCreateNode(string node, NodeInput previousNode)
        {
            string[] splitString = node.Split(" ", 3, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            uint level = 0;

            if (!ValidateInput(node, splitString, out level))
            {
                return default;
            }
            return level switch
            {
                0 => CreateNodeForRootLevel(node, splitString, level),
                1 => CreateSecondNode(node, splitString, level, previousNode),
                _ => CreateNode(node, splitString, level, previousNode)
            };
        }

        /// <summary>
        /// Validate input string for the node
        /// </summary>
        /// <param name="node"></param>
        /// <param name="splitString"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        private bool ValidateInput(string node, string[] splitString, out uint level)
        {
            level = 0;
            if (splitString.Length < 2 || string.IsNullOrWhiteSpace(splitString[1]))
            {
                _logger.LogInformation($"Ignoring node : {node}");
                return false;
            }
            else if (splitString.Length == 2 && splitString[1].Contains("@"))
            {
                _logger.LogInformation($"Ignoring node since Id exists without node name : {node}");
                return false;
            }
            else if (!uint.TryParse(splitString[0], out level))
            {
                _logger.LogInformation($"Ignoring node since Id is not a positive number : {node}");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Create node for level 0
        /// </summary>
        /// <param name="node">source string for node</param>
        /// <param name="splitString">split string for node</param>
        /// <param name="level">Current level of node</param>
        /// <param name="previousNode">Last node created</param>
        /// <returns>
        /// IsValid: If the node is well formed or not. 
        /// Node: Node created from the source node string
        /// </returns>
        private (bool IsValid, NodeInput Node) CreateNodeForRootLevel(string node, string[] splitString, uint level)
        {
            var stringAtIndex1 = splitString[1].Trim();

            var currentNode = splitString.Count() > 2 ? new NodeInput
            {
                Level = level,
                IsValid = true,
                Name = splitString[2]?.Trim()?.ToLower(),
                Attributes = new List<(string, string)> { ("id", stringAtIndex1) },
                SourceString = node,
            } : new NodeInput
            {
                Level = level,
                IsValid = true,
                Name = stringAtIndex1?.ToLower(),
                SourceString = node,
            };
            return (true, currentNode);
        }

        /// <summary>
        /// Create node for level 1
        /// </summary>
        /// <param name="node">source string for node</param>
        /// <param name="splitString">split string for node</param>
        /// <param name="level">Current level of node</param>
        /// <param name="previousNode">Last node created</param>
        /// <returns>
        /// IsValid: If the node is well formed or not. 
        /// Node: Node created from the source node string
        /// </returns>
        private (bool IsValid, NodeInput Node) CreateSecondNode(string node, string[] splitString, uint level, NodeInput previousNode)
        {
            var name = splitString[1]?.Trim()?.ToLower();
            if (!name.Equals("name"))
                return CreateNode(node, splitString, level, previousNode);
            var parentNode = GetParentNode(previousNode, level - 1);
            if (parentNode == null)
            {
                _logger.LogInformation($"Ignoring node since parent node does not exist : {node}");
                return default;
            }

            var currentNode = new NodeInput
            {
                Level = level,
                IsValid = true,
                Name = splitString[1]?.Trim()?.ToLower(),
                Attributes = new List<(string, string)> { ("value", splitString.Length == 3 ? splitString[2]?.Trim() : null) },
                SourceString = node,
                Previous = parentNode
            };
            parentNode.Childs.Add(currentNode);
            return (true, currentNode);
        }

        /// <summary>
        /// Created node for level > 1
        /// </summary>
        /// <param name="node">source string for node</param>
        /// <param name="splitString">split string for node</param>
        /// <param name="level">Current level of node</param>
        /// <param name="previousNode">Last node created</param>
        /// <returns>
        /// IsValid: If the node is well formed or not. 
        /// Node: Node created from the source node string
        /// </returns>
        private (bool IsValid, NodeInput Node) CreateNode(string node, string[] splitString, uint level, NodeInput previousNode)
        {
            var parentNode = GetParentNode(previousNode, level - 1);
            if (parentNode == null)
            {
                _logger.LogInformation($"Ignoring node since parent node does not exist : {node}");
                return default;
            }
            var value = splitString.Length == 3 ? splitString[2]?.Trim() : null;
            //var currentNode = new NodeInput
            //{
            //    Level = level,
            //    IsValid = true,
            //    Name = splitString[1]?.Trim()?.ToLower(),
            //    Value = value,
            //    SourceString = node,
            //    Previous = parentNode
            //};
            //parentNode.Childs.Add(currentNode);
            previousNode?.Attributes.Add((splitString[1]?.Trim()?.ToLower(), value));
            return (false, default);
        }


        /// <summary>
        /// Get parent node for the node specified
        /// </summary>
        /// <param name="node">Node for which parent node need to be found</param>
        /// <param name="parentNodeLevel">Level of the parent node</param>
        /// <returns>Parent node if found otherwise default value</returns>
        private static NodeInput GetParentNode(NodeInput node, uint parentNodeLevel)
        {
            NodeInput currentNode = node;
            while (currentNode != null)
            {
                if (currentNode.Level == parentNodeLevel)
                    return currentNode;
                currentNode = currentNode.Previous;
            }
            return default(NodeInput);
        }
    }
}
