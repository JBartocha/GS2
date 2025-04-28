using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS2
{
    interface IGrid
    {
        public Point? AddFood(bool StartingPositionFood = false);
        public bool AddFood(Point p, bool StartingPositionFood = false);
    }
    public struct Cell
    {
        public int x;
        public int y;
        public BlockTypes blockType;
    }

    public class Grid
    {
        protected int Rows;
        protected int Columns;
        protected int BlockSize;
        protected Graphics Graphics;
        protected BlockTypes[,] Cells;
        
        public Grid(int gridXSize, int gridYSize, int blockSize, Graphics graphics)
        {
            Rows = gridXSize;
            Columns = gridYSize;
            BlockSize = blockSize;
            Graphics = graphics; 
            Cells = new BlockTypes[Rows, Columns];
            InitializeGrid();
        }

        private void InitializeGrid()
        {
            for (int x = 0; x < Rows; x++)
            {
                for (int y = 0; y < Columns; y++)
                {
                    Cells[x, y] = BlockTypes.EmptyBlock;
                }
            }

            // Draw the grid lines
            for (int i = 0; i <= Rows; i++)
            {
                Graphics.DrawLine(Pens.Black, 0, i * BlockSize, Columns * BlockSize, i * BlockSize);
            }
            for (int j = 0; j <= Columns; j++)
            {
                Graphics.DrawLine(Pens.Black, j * BlockSize, 0, j * BlockSize, Rows * BlockSize);
            }
        }

        // TODO - not sure if should be here (no use)
        public virtual void AddFood(Point position, bool StartingPositionFood = false)
        {
            if (IsValidPosition(position) && Cells[position.X, position.Y] == BlockTypes.EmptyBlock)
            {
                Cells[position.X, position.Y] = BlockTypes.FoodBlock;
                DrawCell(position.X, position.Y, BlockTypes.FoodBlock);
            }
        }
         
        // TODO - not sure if should be here (no use)
        public virtual void AddFood(bool StartingPositionFood = false)
        {
            if (IsThereEmptyCellInGrid())
            {
                Random random = new Random();
                int x = random.Next(0, Rows);
                int y = random.Next(0, Columns);
                while (Cells[x, y] != BlockTypes.EmptyBlock)
                {
                    x = random.Next(0, Rows); //TODO - check if is out of bounds
                    y = random.Next(0, Columns);
                }
                Cells[x, y] = BlockTypes.FoodBlock;
                DrawCell(x, y, BlockTypes.FoodBlock);
            }
            else
            {
                throw new Exception("There is no space to put Food!"); // TODO what to do
            }
        }

        protected bool IsThereEmptyCellInGrid()
        {
            for (int x = 0; x < Rows; Rows++)
            {
                for (int y = 0; y < Columns; ++y)
                {
                    if (Cells[x,y] == BlockTypes.EmptyBlock)
                        return true;
                }
            }
            return false;
        }
        

        protected bool IsValidPosition(Point position)
        {
            return position.X >= 0 && position.X < this.Rows && position.Y >= 0 && position.Y < this.Columns;
        }

        protected void DrawCell(int x, int y, BlockTypes blockType)
        {
            SolidBrush brush = GetBrushByType(blockType);
            Graphics.FillRectangle(brush, x * BlockSize + 1, y * BlockSize + 1, BlockSize - 1, BlockSize - 1);
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

    public class Snake : Grid
    {
        private List<Point> SnakeBody; //0==Head...Other==body
        private Point Movement;
        private string MovementDirection;

        private string ForbiddenDirection;
        private int MoveCounter = 0; // Everytime snake moves, this is incremented by 1
        private Record record = new Record();
        
        public Snake(Point startingPosition, int gridXSize, int gridYSize, int blockSize, Graphics graphics)
            : base(gridXSize, gridYSize, blockSize, graphics)
        {
            SnakeBody = new List<Point> { startingPosition };
            SnakeBody.Add(new Point(startingPosition.X, startingPosition.Y + 1));
            SnakeBody.Add(new Point(startingPosition.X, startingPosition.Y + 2));

            Movement = new Point(1, 0); // Start moving Right
            ForbiddenDirection = "Down"; // predetermined (static Snake Position at start)
            DrawSnake();
        }

        public delegate void BlockCollisionEventHandler(object sender, GridCollisionArgs args);
        public event BlockCollisionEventHandler CellCollisionEvent;
        //public event EventHandler<GridCollisionArgs> CellCollisionEvent;

        public delegate void FoodEatenEventHandler(object sender, EventArgs args);
        public event FoodEatenEventHandler FoodEatenEvent;

        private void Debug_ListRecordValues()
        {
            for (int i = 0; i < record.StartingFoodPositions.Count; i++)
            {
                Debug.WriteLine("StartingFoodPosition: " + record.StartingFoodPositions[i].X + "," + record.StartingFoodPositions[i].Y);
            }
            Debug.WriteLine("--------------------------------------");
            for (int i = 0; i < record.Turns.Count; i++)
            {
                Debug.WriteLine("Turn: " + record.Turns[i].TurnNumber + " MoveDirection: " + record.Turns[i].MoveDirection);
            }
        }

        public void Move()
        {
            Point newHeadPosition = new Point(SnakeBody[0].X + Movement.X, SnakeBody[0].Y + Movement.Y);
            
            this.MoveCounter++;
            /*
            record.Turns[MoveCounter - 1] = new TurnRecord
            {
                TurnNumber = MoveCounter,
                MoveDirection = MovementDirection,
                GeneratedFoodPosition = null
            };
            */
            if (!IsValidPosition(newHeadPosition) || Cells[newHeadPosition.X, newHeadPosition.Y] == BlockTypes.SnakeBody)
            {
                GridCollisionArgs GridCollisionArgs = new GridCollisionArgs
                {
                    BlockType = BlockTypes.WallBlock, // TODO nevyuzije se - zatim
                    Message = "Game Over: Snake collided."
                };
                CrossSnakeHead();
                //Debug_ListRecordValues();
                CellCollisionEvent?.Invoke(this, GridCollisionArgs);
            }
            if (Cells[newHeadPosition.X, newHeadPosition.Y] == BlockTypes.FoodBlock)
            {
                SnakeBody.Insert(0, newHeadPosition);
                AddFood();
                FoodEatenEvent?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                SnakeBody.Insert(0, newHeadPosition);
                Point tail = SnakeBody.Last();
                SnakeBody.RemoveAt(SnakeBody.Count - 1);
                Cells[tail.X, tail.Y] = BlockTypes.EmptyBlock;
                DrawCell(tail.X, tail.Y, BlockTypes.EmptyBlock);
            }

            Cells[newHeadPosition.X, newHeadPosition.Y] = BlockTypes.SnakeHead;
            DetermineForbiddenDirection();
            DrawSnake();
        }

        public bool SetMovement(string direction)
        {
            if (direction == "Up" && ForbiddenDirection != "Up")
            {
                Movement = new Point(0, -1);
                MovementDirection = direction;
                return true;
            }
            else if (direction == "Down" && ForbiddenDirection != "Down")
            {
                Movement = new Point(0, 1);
                MovementDirection = direction;
                return true;
            }
            else if (direction == "Left" && ForbiddenDirection != "Left")
            {
                Movement = new Point(-1, 0);
                MovementDirection = direction;
                return true;
            }
            else if (direction == "Right" && ForbiddenDirection != "Right")
            {
                Movement = new Point(1, 0);
                MovementDirection = direction;
                return true;
            }
            return false;
        }
        /*
        public override void AddFood(Point position, bool StartingPositionFood = false)
        {
            if (IsValidPosition(position) && Cells[position.X, position.Y] == BlockTypes.EmptyBlock)
            {
                Cells[position.X, position.Y] = BlockTypes.FoodBlock;
                DrawCell(position.X, position.Y, BlockTypes.FoodBlock);
            }

            if (StartingPositionFood)
            {
                record.StartingFoodPositions.Add(new Point(position.X, position.Y));
            }
            else
            {
                string MoveDirection = record.Turns.ElementAt(MoveCounter - 1).MoveDirection;
                int TurnNumber = record.Turns.ElementAt(MoveCounter - 1).TurnNumber;
                TurnRecord turnRecord = new TurnRecord
                {
                    MoveDirection = MoveDirection,
                    TurnNumber = TurnNumber,
                    GeneratedFoodPosition = new Point(position.X, position.Y)
                };
                record.Turns[MoveCounter] = turnRecord;
            }
        }*/
        /*
        public override void AddFood(bool StartingPositionFood = false)
        {
            if (IsThereEmptyCellInGrid())
            {
                Random random = new Random();
                int x = random.Next(0, Rows);
                int y = random.Next(0, Columns);
                while (Cells[x, y] != BlockTypes.EmptyBlock)
                {
                    //TODO - check if possible?
                    x = random.Next(0, Rows); 
                    y = random.Next(0, Columns);
                }
                Cells[x, y] = BlockTypes.FoodBlock;
                
                if (StartingPositionFood)
                {
                    record.StartingFoodPositions.Add(new Point(x, y));
                }
                else
                {
                    string MoveDirection = record.Turns.ElementAt(MoveCounter - 1).MoveDirection;
                    int TurnNumber = record.Turns.ElementAt(MoveCounter - 1).TurnNumber;

                    TurnRecord turnRecord = new TurnRecord { 
                        MoveDirection = MoveDirection, 
                        TurnNumber = TurnNumber,
                        GeneratedFoodPosition = new Point(x, y)
                    };
                    record.Turns[MoveCounter] = turnRecord;
                }
                
                DrawCell(x, y, BlockTypes.FoodBlock);
            }
            else
            {
                throw new Exception("There is no space to put Food!"); // TODO what to do
            }
        }
        */
        private void DrawSnake()
        {
            foreach (Point segment in SnakeBody)
            {
                Cells[segment.X, segment.Y] = BlockTypes.SnakeBody;
                DrawCell(segment.X, segment.Y, BlockTypes.SnakeBody);
            }

            Point head = SnakeBody[0];
            Cells[head.X, head.Y] = BlockTypes.SnakeHead;
            DrawCell(head.X, head.Y, BlockTypes.SnakeHead);
        }

        private void DetermineForbiddenDirection()
        {
            Point HeadPosition = SnakeBody[0];
            Point FirstBodyPart = SnakeBody[1];

            switch (HeadPosition.X - FirstBodyPart.X)
            {
                case 1:
                    ForbiddenDirection = "Left";
                    break;
                case -1:
                    ForbiddenDirection = "Right";
                    break;
                case 0:
                    if (HeadPosition.Y - FirstBodyPart.Y == 1)
                        ForbiddenDirection = "Up";
                    else
                        ForbiddenDirection = "Down";
                    break;
            }
        }
        private void CrossSnakeHead()
        {
            Graphics.DrawLine(Pens.Black, SnakeBody[0].X * BlockSize, SnakeBody[0].Y * BlockSize, 
                (SnakeBody[0].X + 1) * BlockSize, (SnakeBody[0].Y + 1) * BlockSize);
            Graphics.DrawLine(Pens.Black, (SnakeBody[0].X + 1) * BlockSize, SnakeBody[0].Y * BlockSize,
                SnakeBody[0].X * BlockSize, (SnakeBody[0].Y + 1) * BlockSize);
        }

        public string GetForbiddenMoveDirection()
        {
            return ForbiddenDirection;
        }
        public Point GetSnakeHeadPosition()
        {
            return SnakeBody[0];
        }
    }
}
