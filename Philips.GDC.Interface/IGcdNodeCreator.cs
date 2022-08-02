using Philips.GDC.Dto;
using Philips.GDC.Lexical;

namespace Philips.GDC.Interface
{
    /// <summary>
    /// Responsible for creating and arranging all the nodes specified in the source file
    /// </summary>
    public interface IGcdNodeCreator
    {
        /// <summary>
        /// Set up root node based on "RootNodeName" in appSettings.json
        /// </summary>
        /// <param name="rootNode"></param>
        /// <returns></returns>
        Task SetUpRootNode(NodeInput rootNode);

        /// <summary>
        /// Create sub tree starting with Level 0
        /// </summary>
        /// <param name="node">Node to be attached with the tree specified</param>
        /// <param name="isLastNode">If it the last node being processed</param>
        /// <returns></returns>
        Task CreateSubTree(NodeInput node, bool isLastNode);

        /// <summary>
        /// Event handler to send the complete tree created from source file to the subscriber
        /// </summary>
        EventHandler<NodeToXmlArgs> OnComplete { get; set; }
    }
}
