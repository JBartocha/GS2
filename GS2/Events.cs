namespace GS2
{
    public class GridCollisionArgs : EventArgs
    {
        public BlockTypes BlockType { get; set; }
        public string Message { get; set; }
        public GridCollisionArgs()
        {
            BlockType = BlockTypes.EmptyBlock;
            Message = string.Empty;
        }
    }

}
