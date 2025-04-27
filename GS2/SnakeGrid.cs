using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS2
{
    interface IGrid
    {
        public Point? AddFood();
        public bool AddFood(Point p);
    }
    public struct Cell
    {
        public int x;
        public int y;
        public BlockTypes blockType;
    }

    /*
    public struct MoveResult
    {
        
    }
    */

    public class Grid
    {
        protected int Rows;
        protected int Columns;
        protected int BlockSize;
        protected Graphics Graphics;
        protected BlockTypes[,] Cells;

        public delegate void GridCollisionArgs(object sender, GridCollisionArgs args);
        public event GridCollisionArgs GridCollisionEvent;

        public Grid(int gridXSize, int gridYSize, int blockSize, Graphics graphics)
        {
            Rows = gridXSize;
            Columns = gridYSize;
            BlockSize = blockSize;
            Graphics = graphics;
            Cells = new BlockTypes[Rows, Columns];
            InitializeGrid();
        }

        public void OnGridCollisionEvent(object sender, GridCollisionArgs e)
        {
            if (GridCollisionEvent != null)
            {
                GridCollisionEvent(sender, e);
            }
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
         
        /*
        private Point AddFood()
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
                DrawCell(x, y, BlockTypes.FoodBlock);
                return new Point(x, y); // Food added successfully
            }
            else
            {
                throw new Exception("There is no space to put Food!");
            }

        }
        */

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
            Graphics.FillRectangle(brush, x * BlockSize, y * BlockSize, BlockSize, BlockSize);
        }

        protected SolidBrush GetBrushByType(BlockTypes type)
        {
            return type switch
            {
                BlockTypes.EmptyBlock => new SolidBrush(Color.White),
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
        private List<Point> SnakeBody;
        private Point Movement;
        private string ForbiddenDirection;

        public Snake(Point startingPosition, int gridXSize, int gridYSize, int blockSize, Graphics graphics)
            : base(gridXSize, gridYSize, blockSize, graphics)
        {
            SnakeBody = new List<Point> { startingPosition };
            Movement = new Point(1, 0); // Start moving right
            ForbiddenDirection = "Down";
            DrawSnake();
        }

        public void Move()
        {
            Point newHeadPosition = new Point(SnakeBody[0].X + Movement.X, SnakeBody[0].Y + Movement.Y);

            if (!IsValidPosition(newHeadPosition) || Cells[newHeadPosition.X, newHeadPosition.Y] == BlockTypes.SnakeBody)
            {
                // TODO - Invoke Event
                throw new Exception("Game Over: Snake collided with itself or the wall.");
            }

            if (Cells[newHeadPosition.X, newHeadPosition.Y] == BlockTypes.FoodBlock)
            {
                SnakeBody.Insert(0, newHeadPosition); 
                // TODO - Grow the snake
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
            DrawSnake();
        }

        public void SetMovement(string direction)
        {
            if (direction == "Up" && ForbiddenDirection != "Up") Movement = new Point(0, -1);
            else if (direction == "Down" && ForbiddenDirection != "Down") Movement = new Point(0, 1);
            else if (direction == "Left" && ForbiddenDirection != "Left") Movement = new Point(-1, 0);
            else if (direction == "Right" && ForbiddenDirection != "Right") Movement = new Point(1, 0);
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
    }
}
