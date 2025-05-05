using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GS2
{
    public partial class WallOptionsForm : Form
    {
        BlockTypes[,] Blocks;
        Point[] ForbiddenWallPositions;
        private Graphics? Grap;
        private Bitmap? surface;
        private int Rows;
        private int Columns;
        private int BlockSize;
        List<Point> WallPositions = new List<Point>();

        private Dictionary<BlockTypes, SolidBrush> brushes = new Dictionary<BlockTypes, SolidBrush>
        {
            { BlockTypes.EmptyBlock, new SolidBrush(Color.LightGray) },
            { BlockTypes.WallBlock, new SolidBrush(Color.Gray) },
            { BlockTypes.FoodBlock, new SolidBrush(Color.Green) },
            { BlockTypes.SnakeBody, new SolidBrush(Color.DarkRed) },
            { BlockTypes.SnakeHead, new SolidBrush(Color.Red) }
        };

        public WallOptionsForm(int rows, int columns, int blockSize, Point[] ForbiddenWallPositions)
        {
            InitializeComponent();

            this.BlockSize = blockSize;
            this.Rows = rows;
            this.Columns = columns;

            this.Size = new Size(Columns * BlockSize + 42, Rows * BlockSize + 120);
            Panel_Main.Size = new Size(Columns * BlockSize + 1, Rows * BlockSize + 1);
            Button_SaveAndExit.Location = new Point(12, Rows * BlockSize + 20);
            Button_CancelAndExit.Location = new Point(12+166, Rows * BlockSize + 20);
            Button_Reset.Location = new Point(12+166+166, Rows * BlockSize + 20);

            Blocks = new BlockTypes[rows, columns];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    Blocks[i, j] = BlockTypes.EmptyBlock;
                }
            }

            this.ForbiddenWallPositions = ForbiddenWallPositions;

            surface = new Bitmap(Panel_Main.Width, Panel_Main.Height);
            Grap = Graphics.FromImage(surface);
            Panel_Main.BackgroundImage = surface;
            Panel_Main.BackgroundImageLayout = ImageLayout.None;

            for (int i = 0; i <= Rows; i++)
            {
                //Grap.DrawLine(Pens.Black, i * BlockSize, 0, i * BlockSize, BlockSize * Rows);
                Grap.DrawLine(Pens.Black, new Point(0, i * BlockSize), new Point(Columns * BlockSize, i * BlockSize));
            }
            for (int j = 0; j <= Columns; j++)
            {
                Grap.DrawLine(Pens.Black, new Point(j * BlockSize, 0), new Point(j * BlockSize, Rows * BlockSize));
                //Grap.DrawLine(Pens.Black, 0, j * BlockSize, Columns * BlockSize, j * BlockSize);
            }
            for (int i = 0; i < ForbiddenWallPositions.Length; i++)
            {
                //Debug.WriteLine(ForbiddenWallPositions[i].ToString());
                Grap.FillRectangle(Brushes.DarkRed, ForbiddenWallPositions[i].Y * BlockSize+1,
                    ForbiddenWallPositions[i].X * BlockSize+1, BlockSize-1, BlockSize-1);
            }


        }

        private void Button_Reset_Click(object sender, EventArgs e)
        {
            WallPositions.Clear();
        }

        private void Buttton_CancelAndExit_Click(object sender, EventArgs e)
        {
            for(int i = 0; i < WallPositions.Count; i++)
            {
                Debug.WriteLine(WallPositions[i].ToString());
            }
        }

        private void Button_SaveAndExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DrawBlocks()
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    if (Blocks[i, j] == BlockTypes.WallBlock)
                    {
                        Grap.FillRectangle(brushes[BlockTypes.WallBlock], j * BlockSize + 1,
                            i * BlockSize + 1, BlockSize - 1, BlockSize - 1);
                    }
                    if (Blocks[i, j] == BlockTypes.EmptyBlock)
                    {
                        Grap.FillRectangle(brushes[BlockTypes.EmptyBlock], j * BlockSize + 1,
                            i * BlockSize + 1, BlockSize - 1, BlockSize - 1);
                    }
                    for (int k = 0; k < ForbiddenWallPositions.Length; k++)
                    {
                        if (i == ForbiddenWallPositions[k].X && j == ForbiddenWallPositions[k].Y)
                        {
                            Grap.FillRectangle(Brushes.DarkRed, j * BlockSize + 1,
                                i * BlockSize + 1, BlockSize - 1, BlockSize - 1);
                        }
                    }
                }
            }
        }



        private void Panel_Main_MouseClick(object sender, MouseEventArgs e)
        {

            int column = e.X / BlockSize;
            int row = e.Y / BlockSize;
            Point BlockPosition = new Point(row, column);

            if (ForbiddenWallPositions.Contains(BlockPosition))
            {
                return;
            }
            else if (Blocks[row, column] == BlockTypes.EmptyBlock)
            {
                Blocks[row, column] = BlockTypes.WallBlock;
                WallPositions.Add(BlockPosition);
            }
            else
            {
                WallPositions.Remove(BlockPosition);
                Blocks[row, column] = BlockTypes.EmptyBlock;
            }
            DrawBlocks();
            Panel_Main.Invalidate();
        }
    }
}
