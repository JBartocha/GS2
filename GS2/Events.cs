using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS2
{
    public class GridCollisionArgs : EventArgs
    {
        public BlockTypes BlockType { get; set; }
        public bool IsCollision { get; set; }
        public string Message { get; set; }
        public GridCollisionArgs()
        {
            IsCollision = false;
            BlockType = BlockTypes.EmptyBlock;
            Message = string.Empty;
        }
    }

    public class SnakePointsEvent : EventArgs
    {
        public List<Point> Points = [];
    }

}
