using Philips.GDC.Dto;

namespace Philips.GDC.Interface
{
    /// <summary>
    /// Responsible for Analyzing and creating node based of the source string
    /// </summary>
    public interface ILexicalNodeProcessor
    {
        /// <summary>
        /// Analyze and create node based of the source node string. Also set up the node for parent child relationship
        /// </summary>
        /// <param name="node">Source node string</param>
        /// <param name="previousNode">Node that has been created just before the current node</param>
        /// <returns>
        /// IsValid: If the node is well formed or not. 
        /// Node: Node created from the source node string
        /// </returns>
        (bool IsValid, NodeInput Node) AnalyzeAndCreateNode(string node, ref NodeInput previousNode);
    }
}