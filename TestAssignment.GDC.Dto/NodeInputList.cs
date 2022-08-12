namespace TestAssignment.GDC.Dto
{
    public class NodeInputList : List<NodeInput>
    {
        HashSet<uint> levels;
        public bool IsWellFormed { get; private set; }
        public uint NodeOrder { get; private set; }

        public NodeInputList(uint nodeOrder)
        {
            levels = new HashSet<uint>();
            NodeOrder = nodeOrder;
        }

        public new bool Add(NodeInput input)
        {
            IsWellFormed = (input.Level == 0 || levels.Contains(input.Level - 1));
            if (IsWellFormed)
            {
                levels.Add(input.Level);
                base.Add(input);
                return true;
            }
            return false;
        }
    }
}