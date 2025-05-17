using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS2
{
    interface ISnakeRecord
    {
        public Record GetGameRecord();
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
            _Record.Turns.Add(new TurnRecord
            {
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
                DrawBlock(new Point(x, y), BlockTypes.FoodBlock);
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

