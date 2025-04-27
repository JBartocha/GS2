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
        int Xss { get; set; }
        public Point? AddFood();
        public bool AddFood(Point p);
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

        public virtual bool AddFood(Point position)
        {
            if (IsValidPosition(position) && Cells[position.X, position.Y] == BlockTypes.EmptyBlock)
            {
                Cells[position.X, position.Y] = BlockTypes.FoodBlock;
                DrawCell(position.X, position.Y, BlockTypes.FoodBlock);
                return true;
            }
            return false;
        }
         
        public Point AddFood()
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
                return new Point(x, y); // Food added successfully
            }
            else
            {
                throw new Exception("There is no space to put Food!"); // TODO what to do
            }
        }

        private bool IsThereEmptyCellInGrid()
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
            return position.X >= 0 && position.X < Rows && position.Y >= 0 && position.Y < Columns;
        }

        protected void DrawCell(int x, int y, BlockTypes blockType)
        {
            // Drawing logic for a cell
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
        private string ForbiddenDirection;
        private int CycleCounter = 0; // Everytime snake moves, this is incremented by 1

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


        public void Move()
        {
            this.CycleCounter++;
            Point newHeadPosition = new Point(SnakeBody[0].X + Movement.X, SnakeBody[0].Y + Movement.Y);

            if (!IsValidPosition(newHeadPosition) || Cells[newHeadPosition.X, newHeadPosition.Y] == BlockTypes.SnakeBody)
            {
                // TODO - Invoke Event
                CellCollisionEvent(this, new GridCollisionArgs
                {
                    BlockType = Cells[newHeadPosition.X, newHeadPosition.Y],
                    IsCollision = true,
                    Message = "Game Over: Snake collided with itself or the wall."
                });
                Debug.WriteLine("NIKDY SE NEUKAZE: Snake collided with itself or the wall.");

                throw new Exception("Game Over: Snake collided with itself or the wall.");
            }
            if (Cells[newHeadPosition.X, newHeadPosition.Y] == BlockTypes.FoodBlock)
            {
                SnakeBody.Insert(0, newHeadPosition);
                FoodEatenEvent?.Invoke(this, EventArgs.Empty);
                AddFood();
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
                return true;
            }
            else if (direction == "Down" && ForbiddenDirection != "Down")
            {
                Movement = new Point(0, 1);
                return true;
            }
            else if (direction == "Left" && ForbiddenDirection != "Left")
            {
                Movement = new Point(-1, 0);
                return true;
            }
            else if (direction == "Right" && ForbiddenDirection != "Right")
            {
                Movement = new Point(1, 0);
                return true;
            }
            return false;
        }

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

        public string GetForbbidenMoveDirection()
        {
            return this.ForbiddenDirection;
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
