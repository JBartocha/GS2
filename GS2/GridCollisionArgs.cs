using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;

namespace GS2
{

    public class GridCollisionArgs : EventArgs
    {
        public BlockTypes BlockType { get; set; }
        public bool IsCollision { get; set; }

        public GridCollisionArgs()
        {
            IsCollision = false;
            BlockType = BlockTypes.EmptyBlock;
        }
    }

}
