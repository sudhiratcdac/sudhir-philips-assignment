namespace TestAssignment.GDC.Dto
{
    public class NodeInput
    {
        public string SourceString { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public IList<(string Name, string Value)> Attributes { get; set; } = new List<(string Name, string Value)>();
        public uint Level { get; set; }
        public bool IsValid { get; set; }

        public uint NodeOrder { get; set; }

        public NodeInput Previous { get; set; }
        public List<NodeInput> Childs { get; } = new List<NodeInput>();
    }
}