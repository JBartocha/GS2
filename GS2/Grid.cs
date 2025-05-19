using System.Diagnostics;

namespace GS2
{
    public enum BlockTypes
    {
        EmptyBlock,
        WallBlock,
        FoodBlock,
        SnakeBody,
        SnakeHead,
        OutOfBoundsBlock
    }

    public static class BlockBrushes
    {
        public static readonly Dictionary<BlockTypes, SolidBrush> brushes = new Dictionary<BlockTypes, SolidBrush>
        {
            { BlockTypes.EmptyBlock, new SolidBrush(Color.LightGray) },
            { BlockTypes.WallBlock, new SolidBrush(Color.Gray) },
            { BlockTypes.FoodBlock, new SolidBrush(Color.Green) },
            { BlockTypes.SnakeBody, new SolidBrush(Color.DarkRed) },
            { BlockTypes.SnakeHead, new SolidBrush(Color.Red) }
        };
    }

    public class Grid
    {
        protected int _Rows;
        protected int _Columns;
        protected int _BlockSize;
        protected Graphics _Graphics;
        protected BlockTypes[,] _Block;
        protected List<Region> _Region = new List<Region>();

        public Grid(int Rows, int Columns, int BlockSize, Graphics graphics)
        {
            this._Rows = Rows;
            this._Columns = Columns;
            this._BlockSize = BlockSize;
            _Graphics = graphics;
            _Block = new BlockTypes[this._Rows, this._Columns];

            InitializeGrid();
        }

        private void InitializeGrid()
        {
            for (int x = 0; x < _Rows; x++)
            {
                for (int y = 0; y < _Columns; y++)
                {
                    _Block[x, y] = BlockTypes.EmptyBlock;
                }
            }

            // Draw the grid lines
            for (int i = 0; i <= _Rows; i++)
            {
                _Graphics.DrawLine(Pens.Black, 0, i * _BlockSize, _Columns * _BlockSize, i * _BlockSize);
            }
            for (int j = 0; j <= _Columns; j++)
            {
                _Graphics.DrawLine(Pens.Black, j * _BlockSize, 0, j * _BlockSize, _Rows * _BlockSize);
            }
        }

        // TODO - not sure if should be here
        public virtual void AddFood(Point position, bool StartingPositionFood = false)
        {
            if (IsValidPosition(position) && _Block[position.X, position.Y] == BlockTypes.EmptyBlock)
            {
                _Block[position.X, position.Y] = BlockTypes.FoodBlock;
                DrawBlock(position, BlockTypes.FoodBlock);
            }
        }

        public List<Region> GetRegion()
        {
            return _Region;
        }

        public virtual void AddWall(Point position)
        {
            if (IsValidPosition(position) && _Block[position.X, position.Y] == BlockTypes.EmptyBlock)
            {
                _Block[position.X, position.Y] = BlockTypes.WallBlock;
                DrawBlock(position, BlockTypes.WallBlock);
            }
            else
            {
                throw new Exception("Error: wall inicialization outside of valid bounds!");
            }
        }

        public virtual void AddFood(bool StartingPositionFood = false)
        {
            Point p = GetRandomEmptyCellPosition();

            _Block[p.X, p.Y] = BlockTypes.FoodBlock;
            DrawBlock(p, BlockTypes.FoodBlock);
        }

        protected Point GetRandomEmptyCellPosition()
        {
            if (IsThereEmptyCellInBlock())
            {
                List<Point> emptyCells = new List<Point>();
                for (int x = 0; x < _Rows; x++)
                {
                    for (int y = 0; y < _Columns; ++y)
                    {
                        if (_Block[x, y] == BlockTypes.EmptyBlock)
                            emptyCells.Add(new Point(x, y));
                    }
                }
                int randomIndex = new Random().Next(0, emptyCells.Count);
                return emptyCells[randomIndex];
            }
            else
            {
                throw new Exception("There is no space to put Food!"); // TODO what to do
            }
        }

        protected bool IsThereEmptyCellInBlock()
        {
            for (int x = 0; x < _Rows; x++)
            {
                for (int y = 0; y < _Columns; y++)
                {
                    if (_Block[x, y] == BlockTypes.EmptyBlock)
                        return true;
                }
            }
            return false;
        }


        protected bool IsValidPosition(Point position)
        {
            return position.X >= 0 && position.X < this._Rows && position.Y >= 0 && position.Y < this._Columns;
        }

        protected void DrawBlock(Point p, BlockTypes blockType)
        {
            SolidBrush brush = GetBrushByType(blockType);
            _Graphics.FillRectangle(brush, p.Y * _BlockSize + 1, p.X * _BlockSize + 1, _BlockSize - 1, _BlockSize - 1);
            _Region.Add(new Region(new Rectangle(p.Y * _BlockSize, p.X * _BlockSize, _BlockSize, _BlockSize)));
        }

        protected SolidBrush GetBrushByType(BlockTypes type)
        {
            return type switch
            {
                BlockTypes.EmptyBlock => new SolidBrush(Color.LightGray),
                BlockTypes.WallBlock => new SolidBrush(Color.Gray),
                BlockTypes.FoodBlock => new SolidBrush(Color.Green),
                BlockTypes.SnakeBody => new SolidBrush(Color.DarkRed),
                BlockTypes.SnakeHead => new SolidBrush(Color.Red),
                _ => new SolidBrush(Color.Black),
            };
        }
    }
} 