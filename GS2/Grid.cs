using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;

namespace GS2
{
    /*
    interface IGrid
    {
        public Point? AddFood();
        public bool AddFood(Point p);
    }

    internal class Grid : IGrid
    {
        private struct Cell
        {
            public int x;
            public int y;
            public BlockTypes blockType;
        }

        private int Rows;
        private int Columns;
        private int BlockSize;
        private Graphics Graphics;
        private Cell[,] Cells;
        private GridCollisionArgs GridCollisionEvent = new GridCollisionArgs();

        public delegate void BlockCollisionEventHandler(object sender, GridCollisionArgs args);
        public event BlockCollisionEventHandler BlockCollisionEvent;

        public delegate void NoFreeSpaceForFooodHandler(object sender, EventArgs args);
        public event NoFreeSpaceForFooodHandler NoFreeSpaceForFoodEvent;

        public Grid(int GridXSize, int GridYSize, int BlockSize, Graphics g)
        {
            this.Rows = GridXSize;
            this.Columns = GridYSize;
            this.BlockSize = BlockSize;
            Graphics = g;

            Cells = new Cell[GridXSize, GridYSize];
            for (int i = 0; i < GridXSize; i++)
            {
                for (int j = 0; j < GridYSize; j++)
                {
                    Cells[i, j] = new Cell();
                    Cells[i, j].x = i;
                    Cells[i, j].y = j;
                    Cells[i, j].blockType = BlockTypes.EmptyBlock;
                }
            }
            DrawGrid();

            //Drawing Snake for First Time
            DrawCell(5, 7, BlockTypes.SnakeBody);
            DrawCell(5, 6, BlockTypes.SnakeBody);
            DrawCell(5, 5, BlockTypes.SnakeHead);
        }

        //EVENT sent from SNAKE
        public void OnSnakeMoveEvent(object sender, SnakePointsEvent e)
        {
            Point PlannedMovePoint = e.Points.ElementAt(e.Points.Count - 1);
            //Debug.WriteLine("SnakeMoveEvent in Grid triggered with point: " + PlannedMovePoint.X + "," + PlannedMovePoint.Y);
            if (IsCollisionOrOutOfBoundsOrSnakeBody(PlannedMovePoint))
            {
                GridCollisionEvent.IsCollision = true;
                // We do not care what type of block is in the cell
                if (BlockCollisionEvent != null)
                    BlockCollisionEvent(this, GridCollisionEvent);
                return;
            }
            if (GetBlockType(PlannedMovePoint.X, PlannedMovePoint.Y) == BlockTypes.FoodBlock)
                GridCollisionEvent.BlockType = BlockTypes.FoodBlock;
            else
            {
                GridCollisionEvent.BlockType = BlockTypes.EmptyBlock;
            }

            if (BlockCollisionEvent != null)
                BlockCollisionEvent(this, GridCollisionEvent);

            GridCollisionEvent.IsCollision = false;
            GridCollisionEvent.BlockType = BlockTypes.EmptyBlock;
            RemoveSnakeCells(); // Clear the snake cells before drawing the new position
            DrawSnake(e.Points);
        }

        public bool AddFood(Point p)
        {
            if (GetEmptyCellsCount() == 0)
            {
                return true; // No empty cells available
            }
            else
            {
                if(GetBlockType(p.X, p.Y) != BlockTypes.EmptyBlock)
                {
                    return true; // Cell is not empty
                }
                DrawCell(p.X, p.Y, BlockTypes.FoodBlock);
                return false; // Food added successfully
            }
        }

        public Point? AddFood()
        {
            if(GetEmptyCellsCount() == 0)
            {
                NoFreeSpaceForFoodEvent?.Invoke(this, EventArgs.Empty);
                return null; // No empty cells available
            }
            else
            {
                Random random = new Random();
                int x = random.Next(0, Rows);
                int y = random.Next(0, Columns);
                while (GetBlockType(x, y) != BlockTypes.EmptyBlock)
                {
                    x = random.Next(0, Rows);
                    y = random.Next(0, Columns);
                }
                DrawCell(x, y, BlockTypes.FoodBlock);
                return new Point(x,y); // Food added successfully
            }
        }

        private int GetEmptyCellsCount()
        {
            int EmptyCellsCount = 0;
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    if (Cells[i, j].blockType == BlockTypes.EmptyBlock)
                    {
                        EmptyCellsCount++;
                    }
                }
            }
            return EmptyCellsCount;
        }

        private bool IsCollisionOrOutOfBoundsOrSnakeBody(Point PlannedMovePoint)
        {
            if (PlannedMovePoint.X < 0 || PlannedMovePoint.X >= Rows || PlannedMovePoint.Y < 0 || PlannedMovePoint.Y >= Columns)
            {
                CrossSnakeHead();
                return true;
            }
            else if (GetBlockType(PlannedMovePoint.X, PlannedMovePoint.Y) == BlockTypes.SnakeBody)
            {
                CrossSnakeHead();
                return true;
            }
            return false;
        }

        private void DrawCell(int x, int y, BlockTypes blockType)
        {
            // Draw the cell
            Rectangle rect = new Rectangle(x * BlockSize + 1, y * BlockSize +1, BlockSize - 1, BlockSize - 1);
            Cells[x, y].blockType = blockType;
            SolidBrush brush = GetBrushByType(blockType);
            Graphics.FillRectangle(brush, rect);     
        }

        private BlockTypes GetBlockType(int x, int y)
        {
            return Cells[x, y].blockType;
        }

        private SolidBrush GetBrushByType(BlockTypes type)
        {
            switch (type)
            {
                case BlockTypes.EmptyBlock:
                    return new SolidBrush(Color.DarkGray);
                case BlockTypes.WallBlock:
                    return new SolidBrush(Color.WhiteSmoke);
                case BlockTypes.FoodBlock:
                    return new SolidBrush(Color.GreenYellow);
                case BlockTypes.SnakeHead:
                    return new SolidBrush(Color.Red);
                case BlockTypes.SnakeBody:
                    return new SolidBrush(Color.DarkRed);
                default:
                    throw new NotImplementedException();
            }
        }

        private void DrawGrid()
        {
            // Draw the grid lines
            for (int i = 0; i <= Rows ; i++)
            {
                Graphics.DrawLine(Pens.Black, 0, i * BlockSize, Columns * BlockSize, i * BlockSize);
            }
            for (int j = 0; j <= Columns; j++)
            {
                Graphics.DrawLine(Pens.Black, j * BlockSize, 0, j * BlockSize, Rows * BlockSize);
            }
        }


        private void DrawSnake(List<Point> body)
        {
            // Draw the snake body
            for (int i = 0; i < body.Count - 1; i++)
            {
                DrawCell(body[i].X, body[i].Y, BlockTypes.SnakeBody);
            }
            DrawCell(body[body.Count - 1].X, body[body.Count - 1].Y, BlockTypes.SnakeHead);
        }

        private void RemoveSnakeCells()
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    if (Cells[i, j].blockType == BlockTypes.SnakeBody || Cells[i, j].blockType == BlockTypes.SnakeHead)
                    {
                        DrawCell(i, j, BlockTypes.EmptyBlock);
                    }
                }
            }
        }

        private void CrossSnakeHead()
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    if (Cells[i, j].blockType == BlockTypes.SnakeHead)
                    {
                        Graphics.DrawLine(Pens.Black, i * BlockSize, j * BlockSize, (i + 1) * BlockSize, (j + 1) * BlockSize);
                        Graphics.DrawLine(Pens.Black, (i + 1) * BlockSize, j * BlockSize, i * BlockSize, (j + 1) * BlockSize);
                    }
                }
            }
        }


    }
    */
}
