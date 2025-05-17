using System.Diagnostics;

namespace GS2
{

    interface ISnakeRecord
    {
        public Record GetGameRecord();
    }

    interface IGrid
    {
        public void AddFood(Point position, bool StartingPositionFood = false);
        public void AddWall(Point position);
        public void AddFood(bool StartingPositionFood = false);
    }

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

    public abstract class Grid : IGrid
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
                    if (_Block[x,y] == BlockTypes.EmptyBlock)
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









    public class Snake : Grid, ISnakeRecord
    {
        private List<Point> _SnakeBody; //0==Head...Other==body
        private Point _Movement;
        private string _MovementDirection = "Right";

        private string _ForbiddenDirection;
        private int _MoveCounter = 0; // Everytime snake moves, this is incremented by 1
        private Record _Record = new Record();

        public Snake(Point startingPosition, int Rows, int Columns, int blockSize, Graphics graphics)
            : base(Rows, Columns, blockSize, graphics)
        {
            _SnakeBody = new List<Point> { startingPosition };
            _SnakeBody.Add(new Point(startingPosition.X + 1, startingPosition.Y));
            //SnakeBody = new List<Point> { new Point(Rows / 2, Columns / 2) };
            //SnakeBody.Add(new Point(Rows / 2 + 1, Columns / 2));

            _Movement = new Point(1, 0); // Start moving Right
            _ForbiddenDirection = "Down"; // predetermined (static Snake Position at start)
            DrawSnake();
        }

        public delegate void BlockCollisionEventHandler(object sender, GridCollisionArgs args);
        public event BlockCollisionEventHandler? CellCollisionEvent;
        //public event EventHandler<GridCollisionArgs> CellCollisionEvent;

        public delegate void FoodEatenEventHandler(object sender, EventArgs args);
        public event FoodEatenEventHandler? FoodEatenEvent;

        public delegate void FullGridEventHandler(object sender, EventArgs args);
        public event FullGridEventHandler? FullGridEvent;

        public void Move()
        {
            Point newHeadPosition = new Point(_SnakeBody[0].X + _Movement.X, _SnakeBody[0].Y + _Movement.Y);
            _Region = new List<Region>();

            //Generate record for turn (move)
            this._MoveCounter++;
            _Record.Turns.Add(new TurnRecord { 
                TurnNumber = _MoveCounter, 
                MoveDirection = _MovementDirection, 
                GeneratedFoodPosition = null 
            });

            if (!IsValidPosition(newHeadPosition))
            {
                InvokeCollisionEvent("GameRecord Over: Snake is Out of Bounds.", BlockTypes.OutOfBoundsBlock);
            }
            else if (_Block[newHeadPosition.X, newHeadPosition.Y] == BlockTypes.SnakeBody)
            {
                InvokeCollisionEvent("Game Over: Snake collided with itself.", BlockTypes.SnakeBody);
            }
            else if (_Block[newHeadPosition.X, newHeadPosition.Y] == BlockTypes.WallBlock)
            {
                InvokeCollisionEvent("Game Over: Snake collided with wall.", BlockTypes.WallBlock);
            }
            else if (_Block[newHeadPosition.X, newHeadPosition.Y] == BlockTypes.SnakeBody)
            {
                InvokeCollisionEvent("Game Over: Snake collided with itself.", BlockTypes.SnakeBody);
            }
            else
            {
                if (_Block[newHeadPosition.X, newHeadPosition.Y] == BlockTypes.FoodBlock)
                {
                    _SnakeBody.Insert(0, newHeadPosition);
                    FoodEatenEvent?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    _SnakeBody.Insert(0, newHeadPosition);
                    Point tail = _SnakeBody.Last();
                    _SnakeBody.RemoveAt(_SnakeBody.Count - 1);
                    _Block[tail.X, tail.Y] = BlockTypes.EmptyBlock;
                    DrawBlock(tail, BlockTypes.EmptyBlock);
                }

                _Block[newHeadPosition.X, newHeadPosition.Y] = BlockTypes.SnakeHead;
                DetermineForbiddenDirection();
                DrawSnake();
            }
        }

        private void InvokeCollisionEvent(string message, BlockTypes BlockType)
        {
            GridCollisionArgs GridCollisionArgs = new GridCollisionArgs
            {
                BlockType = BlockType, // TODO nevyuzije se - zatim
                Message = message
            };
            CrossSnakeHead();
            //Debug_ListRecordValues();
            CellCollisionEvent?.Invoke(this, GridCollisionArgs);
        }

        public bool SetMovement(string direction)
        {
            if (direction == "Up" && _ForbiddenDirection != "Up")
            {
                //Movement = new Point(0, -1);
                _Movement = new Point(-1, 0);
                _MovementDirection = direction;
                return true;
            }
            else if (direction == "Down" && _ForbiddenDirection != "Down")
            {
                //Movement = new Point(0, 1);
                _Movement = new Point(1, 0);
                _MovementDirection = direction;
                return true;
            }
            else if (direction == "Left" && _ForbiddenDirection != "Left")
            {
                //Movement = new Point(-1, 0);
                _Movement = new Point(0, -1);
                _MovementDirection = direction;
                return true;
            }
            else if (direction == "Right" && _ForbiddenDirection != "Right")
            {
                //Movement = new Point(1, 0);
                _Movement = new Point(0, 1);
                _MovementDirection = direction;
                return true;
            }
            return false;
        }

        public override void AddFood(Point position, bool StartingPositionFood = false)
        {
            if (IsValidPosition(position) && _Block[position.X, position.Y] == BlockTypes.EmptyBlock)
            {
                _Block[position.X, position.Y] = BlockTypes.FoodBlock;
                DrawBlock(position, BlockTypes.FoodBlock);
            }

            if (StartingPositionFood)
            {
                _Record.StartingFoodPositions.Add(new Point(position.X, position.Y));
            }

            else
            {
                TurnRecord ThisTurn = _Record.Turns.Last();
                ThisTurn.GeneratedFoodPosition = new Point(position.X, position.Y);
                _Record.Turns[_MoveCounter - 1] = ThisTurn;
            }
        }
        
        public override void AddFood(bool StartingPositionFood = false)
        {
            if (IsThereEmptyCellInBlock())
            {
                Random random = new Random();
                int x = random.Next(0, _Rows);
                int y = random.Next(0, _Columns);
                while (_Block[x, y] != BlockTypes.EmptyBlock)
                {
                    //TODO - check if possible? Also better algorithm
                    x = random.Next(0, _Rows); 
                    y = random.Next(0, _Columns);
                }
                _Block[x, y] = BlockTypes.FoodBlock;
                if (StartingPositionFood)
                {
                    _Record.StartingFoodPositions.Add(new Point(x, y));
                }
                else
                {
                    TurnRecord ThisTurn = _Record.Turns.Last();
                    ThisTurn.GeneratedFoodPosition = new Point(x, y);
                    _Record.Turns[_MoveCounter - 1] = ThisTurn;
                }
                DrawBlock(new Point(x,y), BlockTypes.FoodBlock);
            }
            else
            {
                FullGridEvent?.Invoke(this, EventArgs.Empty);
            }
        }

        public override void AddWall(Point position)
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
        
        private void DrawSnake()
        {
            Point head = _SnakeBody[0];
            _Block[head.X, head.Y] = BlockTypes.SnakeHead;
            DrawBlock(head, BlockTypes.SnakeHead);
            Point FirstBody = _SnakeBody[1];
            _Block[FirstBody.X, FirstBody.Y] = BlockTypes.SnakeBody;
            DrawBlock(FirstBody, BlockTypes.SnakeBody);
        }

        private void DetermineForbiddenDirection()
        {
            Point HeadPosition = _SnakeBody[0];
            Point FirstBodyPart = _SnakeBody[1];

            switch (HeadPosition.X - FirstBodyPart.X)
            {
                case 1:
                    _ForbiddenDirection = "Up";
                    break;
                case -1:
                    _ForbiddenDirection = "Down";
                    break;
                case 0:
                    if (HeadPosition.Y - FirstBodyPart.Y == 1)
                        _ForbiddenDirection = "Left";
                    else
                        _ForbiddenDirection = "Right";
                    break;
            }
        }

        private void CrossSnakeHead()
        {
            _Graphics.DrawLine(Pens.Black, _SnakeBody[0].Y * _BlockSize, _SnakeBody[0].X * _BlockSize, 
                (_SnakeBody[0].Y + 1) * _BlockSize, (_SnakeBody[0].X + 1) * _BlockSize);
            _Graphics.DrawLine(Pens.Black, (_SnakeBody[0].Y + 1) * _BlockSize, _SnakeBody[0].X * _BlockSize,
                _SnakeBody[0].Y * _BlockSize, (_SnakeBody[0].X + 1) * _BlockSize);
            
            _Region.Add(new Region(
                new Rectangle(_SnakeBody[0].Y * _BlockSize, _SnakeBody[0].X * _BlockSize, _BlockSize, _BlockSize)));
        }

        public string GetForbiddenMoveDirection()
        {
            return _ForbiddenDirection;
        }
        public Point GetSnakeHeadPosition()
        {
            return _SnakeBody[0];
        }

        public Record GetGameRecord()
        {
            return _Record;
        }
    }
}
