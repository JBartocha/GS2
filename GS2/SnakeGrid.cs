using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS2
{

    interface ISnakeRecord
    {
        public Record GetGameRecord();
    }

    public struct Cell
    {
        public int x;
        public int y;
        public BlockTypes blockType;
    }

    public abstract class Grid
    {
        protected int Rows;
        protected int Columns;
        protected int BlockSize;
        protected Graphics Graphics;
        protected BlockTypes[,] Block;
        protected List<Region> Region = new List<Region>();

        public Grid(int GridRows, int GridColumns, int BlockSize, Graphics graphics)
        {
            Rows = GridRows;
            Columns = GridColumns;
            this.BlockSize = BlockSize;
            Graphics = graphics; 
            Block = new BlockTypes[Rows, Columns];

            InitializeGrid();
        }

        private void InitializeGrid()
        {
            for (int x = 0; x < Rows; x++)
            {
                for (int y = 0; y < Columns; y++)
                {
                    Debug.WriteLine("Initializing cell: " + x + "," + y);
                    Block[x, y] = BlockTypes.EmptyBlock;
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
            if (IsValidPosition(position) && Block[position.X, position.Y] == BlockTypes.EmptyBlock)
            {
                Block[position.X, position.Y] = BlockTypes.FoodBlock;
                DrawCell(position.X, position.Y, BlockTypes.FoodBlock);
            }
        }
         
        public List<Region> GetRegion()
        {
            return Region;
        }

        // TODO - not sure if should be here (no use)
        public virtual void AddFood(bool StartingPositionFood = false)
        {
            if (IsThereEmptyCellInGrid())
            {
                Random random = new Random();
                int x = random.Next(0, Rows);
                int y = random.Next(0, Columns);
                while (Block[x, y] != BlockTypes.EmptyBlock)
                {
                    x = random.Next(0, Rows); //TODO - check if is out of bounds
                    y = random.Next(0, Columns);
                }
                Block[x, y] = BlockTypes.FoodBlock;
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
                    if (Block[x,y] == BlockTypes.EmptyBlock)
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
            
            Region.Add(new Region(new Rectangle(x * BlockSize, y * BlockSize, BlockSize, BlockSize)));
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

        /*
        private void Debug_ListRecordValues() // TODO REMOVE DEBUG METHOD
        {
            for (int i = 0; i < record.StartingFoodPositions.Count; i++)
            {
                Debug.WriteLine("StartingFoodPosition: " + record.StartingFoodPositions[i].X + "," + record.StartingFoodPositions[i].Y);
            }
            Debug.WriteLine("--------------------------------------");
            for (int i = 0; i < record.Turns.Count; i++)
            {
                string w = record.Turns[i].GeneratedFoodPosition.HasValue ? record.Turns[i].GeneratedFoodPosition.Value.ToString() : "null";
                Debug.WriteLine("Turn: " + record.Turns[i].TurnNumber + 
                    " MoveDirection: " + record.Turns[i].MoveDirection + 
                    " Food: " + w);
            }
        }
        */

        public void Move()
        {
            Point newHeadPosition = new Point(SnakeBody[0].X + Movement.X, SnakeBody[0].Y + Movement.Y);
            Region = new List<Region>();

            //Generate record for turn (move)
            this.MoveCounter++;
            record.Turns.Add(new TurnRecord { 
                TurnNumber = MoveCounter, 
                MoveDirection = MovementDirection, 
                GeneratedFoodPosition = null 
            });

            if (!IsValidPosition(newHeadPosition))
            {
                InvokeCollisionEvent("GameRecord Over: Snake is Out of Bounds.", BlockTypes.OutOfBoundsBlock);
            }
            else if (Block[newHeadPosition.X, newHeadPosition.Y] == BlockTypes.SnakeBody)
            {
                InvokeCollisionEvent("Game Over: Snake collided with itself.", BlockTypes.SnakeBody);
            }
            else if (Block[newHeadPosition.X, newHeadPosition.Y] == BlockTypes.WallBlock)
            {
                InvokeCollisionEvent("Game Over: Snake collided with wall.", BlockTypes.WallBlock);
            }
            else if (Block[newHeadPosition.X, newHeadPosition.Y] == BlockTypes.SnakeBody)
            {
                InvokeCollisionEvent("Game Over: Snake collided with itself.", BlockTypes.SnakeBody);
            }

            if (Block[newHeadPosition.X, newHeadPosition.Y] == BlockTypes.FoodBlock)
            {
                SnakeBody.Insert(0, newHeadPosition);
                //AddFood();
                FoodEatenEvent?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                SnakeBody.Insert(0, newHeadPosition);
                Point tail = SnakeBody.Last();
                SnakeBody.RemoveAt(SnakeBody.Count - 1);
                Block[tail.X, tail.Y] = BlockTypes.EmptyBlock;
                DrawCell(tail.X, tail.Y, BlockTypes.EmptyBlock);
            }

            Block[newHeadPosition.X, newHeadPosition.Y] = BlockTypes.SnakeHead;
            DetermineForbiddenDirection();
            DrawSnake();
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

        public override void AddFood(Point position, bool StartingPositionFood = false)
        {
            if (IsValidPosition(position) && Block[position.X, position.Y] == BlockTypes.EmptyBlock)
            {
                Block[position.X, position.Y] = BlockTypes.FoodBlock;
                DrawCell(position.X, position.Y, BlockTypes.FoodBlock);
            }

            if (StartingPositionFood)
            {
                record.StartingFoodPositions.Add(new Point(position.X, position.Y));
            }

            else
            {
                TurnRecord ThisTurn = record.Turns.Last();
                ThisTurn.GeneratedFoodPosition = new Point(position.X, position.Y);
                record.Turns[MoveCounter - 1] = ThisTurn;
            }
        }
        
        public override void AddFood(bool StartingPositionFood = false)
        {
            if (IsThereEmptyCellInGrid())
            {
                Random random = new Random();
                int x = random.Next(0, Rows);
                int y = random.Next(0, Columns);
                while (Block[x, y] != BlockTypes.EmptyBlock)
                {
                    //TODO - check if possible? Also better algorithm
                    x = random.Next(0, Rows); 
                    y = random.Next(0, Columns);
                }
                Block[x, y] = BlockTypes.FoodBlock;
                
                if (StartingPositionFood)
                {
                    record.StartingFoodPositions.Add(new Point(x, y));
                }
                else
                {
                    TurnRecord ThisTurn = record.Turns.Last();
                    ThisTurn.GeneratedFoodPosition = new Point(x, y);
                    record.Turns[MoveCounter - 1] = ThisTurn;
                }
                
                DrawCell(x, y, BlockTypes.FoodBlock);
            }
            else
            {
                throw new Exception("There is no space to put Food!"); // TODO what to do
            }
        }
        
        private void DrawSnake()
        {
            /*
            foreach (Point segment in SnakeBody)
            {
                Cells[segment.X, segment.Y] = BlockTypes.SnakeBody;
                DrawCell(segment.X, segment.Y, BlockTypes.SnakeBody);
            }
            */
            Point head = SnakeBody[0];
            Block[head.X, head.Y] = BlockTypes.SnakeHead;
            DrawCell(head.X, head.Y, BlockTypes.SnakeHead);
            Point FirstBody = SnakeBody[1];
            Block[FirstBody.X, FirstBody.Y] = BlockTypes.SnakeBody;
            DrawCell(FirstBody.X, FirstBody.Y, BlockTypes.SnakeBody);
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
            
            Region.Add(new Region(
                new Rectangle(SnakeBody[0].X * BlockSize, SnakeBody[0].Y * BlockSize, BlockSize, BlockSize)));
        }

        public string GetForbiddenMoveDirection()
        {
            return ForbiddenDirection;
        }
        public Point GetSnakeHeadPosition()
        {
            return SnakeBody[0];
        }

        public Record GetGameRecord()
        {
            return record;
        }
    }
}
