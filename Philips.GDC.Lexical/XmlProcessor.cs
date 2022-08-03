using Philips.GDC.Dto;
using System.Xml.Linq;

namespace Philips.GDC.Lexical
{
    /// <summary>
    /// Responsible for converting source node string to equivalent XML element
    /// </summary>
    internal sealed class XmlProcessor
    {
        public EventHandler<NodeToXmlArgs> OnProcessComplete { get; set; }
        public async Task<XElement> Process(NodeInput node)
        {
            XElement rootNode = null;
            rootNode = CreateNode(node);
            if (OnProcessComplete != null)
            {
                OnProcessComplete(this, new NodeToXmlArgs(node, rootNode));
            }
            return await Task.FromResult(rootNode);
        }

        private XElement CreateNode(NodeInput node)
        {
            XElement element = new XElement(node.Name);
            if (node.Value != null)
                element.SetValue(node.Value);
            if (node.Attributes.Any())
            {
                foreach (var attribute in node.Attributes)
                {
                    if (attribute.Value != null)
                        element.SetAttributeValue(attribute.Name, attribute.Value);
                }
            }

            for (int i = 0; i < node.Childs.Count; i++)
            {
                var childNode = CreateNode(node.Childs[i]);
                element.Add(childNode);
            }
            return element;
        }
    }
}
