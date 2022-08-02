using Philips.GDC.Dto;
using Philips.GDC.Interface;

namespace Philips.GDC.Lexical
{
    /// <summary>
    /// Responsible for Analyzing and creating node based of the source string
    /// </summary>
    internal class LexicalNodeProcessor : ILexicalNodeProcessor
    {
        ///<inheritdoc/>
        public (bool IsValid, NodeInput Node) AnalyzeAndCreateNode(string node, ref NodeInput previousNode)
        {
            var splitString = node.Split(" ", 3, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (splitString.Length < 2 || !uint.TryParse(splitString[0], out uint level) || string.IsNullOrWhiteSpace(splitString[1]))
                return default;

            return level switch
            {
                0 => CreateNodeForRootLevel(node, splitString, level, ref previousNode),
                1 => CreateSecondNode(node, splitString, level, ref previousNode),
                _ => CreateNode(node, splitString, level, ref previousNode)
            };
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
        private static (bool IsValid, NodeInput Node) CreateNodeForRootLevel(string node, string[] splitString, uint level, ref NodeInput previousNode)
        {
            var id = splitString[1].Trim();

            if (splitString.Length != 3 || string.IsNullOrWhiteSpace(id))
                return default;

            var currentNode = new NodeInput
            {
                Level = level,
                IsValid = true,
                Name = splitString[2]?.Trim()?.ToLower(),
                Attributes = new List<(string, string)> { ("id", id) },
                SourceString = node,
            };
            previousNode = currentNode;
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
        private static (bool IsValid, NodeInput Node) CreateSecondNode(string node, string[] splitString, uint level, ref NodeInput previousNode)

        {
            var name = splitString[1]?.Trim()?.ToLower();
            if (!name.Equals("name"))
                return CreateNode(node, splitString, level, ref previousNode);
            var parentNode = GetParentNode(previousNode, level - 1);
            if (parentNode == null) return default;

            var currentNode = new NodeInput
            {
                Level = level,
                IsValid = true,
                Name = splitString[1]?.Trim()?.ToLower(),
                Attributes = new List<(string, string)> { ("value", splitString.Length == 3 ? splitString[2]?.Trim() : null) },
                SourceString = node,
                Previous = previousNode
            };
            parentNode.Childs.Add(currentNode);
            previousNode = currentNode;
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
        private static (bool IsValid, NodeInput Node) CreateNode(string node, string[] splitString, uint level, ref NodeInput previousNode)
        {
            var parentNode = GetParentNode(previousNode, level - 1);
            if (parentNode == null) return default;
            var value = splitString.Length == 3 ? splitString[2]?.Trim() : null;
            var currentNode = new NodeInput
            {
                Level = level,
                IsValid = true,
                Name = splitString[1]?.Trim()?.ToLower(),
                Value = value,
                SourceString = node,
                Previous = previousNode
            };
            parentNode.Childs.Add(currentNode);
            previousNode = currentNode;
            return (true, currentNode);
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
