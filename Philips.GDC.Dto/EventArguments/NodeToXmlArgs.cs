using Philips.GDC.Dto;
using System.Xml.Linq;

namespace Philips.GDC.Lexical
{
    public class NodeToXmlArgs : EventArgs
    {
        public NodeToXmlArgs(NodeInput sourceNode, XElement xElement)
        {
            SourceNode = sourceNode;
            XmlNode = xElement;
        }

        public NodeInput SourceNode { get; }
        public string XmlValue => XmlNode?.ToString();
        public XElement XmlNode { get; }
    }
}
