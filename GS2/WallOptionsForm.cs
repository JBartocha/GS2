namespace GS2
{
    public partial class WallOptionsForm : Form
    {
        BlockTypes[,] Blocks;
        Point[] ForbiddenWallPositions;
        private Graphics Grap;
        private Bitmap surface;
        private int Rows;
        private int Columns;
        private int BlockSize;
        List<Point> WallPositions = new List<Point>();
        List<Point> OriginalWallPositions = new List<Point>();

        private Dictionary<BlockTypes, SolidBrush> brushes = new Dictionary<BlockTypes, SolidBrush>
        {
            { BlockTypes.EmptyBlock, new SolidBrush(Color.LightGray) },
            { BlockTypes.WallBlock, new SolidBrush(Color.Gray) },
            { BlockTypes.FoodBlock, new SolidBrush(Color.Green) },
            { BlockTypes.SnakeBody, new SolidBrush(Color.DarkRed) },
            { BlockTypes.SnakeHead, new SolidBrush(Color.Red) }
        };

        public WallOptionsForm(int rows, int columns, int blockSize, 
            Point[] ForbiddenWallPositions, List<Point> wallPositions)
        {
            InitializeComponent();

            this.BlockSize = blockSize;
            this.Rows = rows;
            this.Columns = columns;
            this.WallPositions = wallPositions;
            this.OriginalWallPositions = wallPositions;

            int ButtonWidth = Button_SaveAndExit.Width;
            this.Size = new Size(Columns * BlockSize + 42, Rows * BlockSize + 70 + Button_SaveAndExit.Height);
            Panel_Main.Size = new Size(Columns * BlockSize + 1, Rows * BlockSize + 1);
            Button_SaveAndExit.Location = new Point(12, Rows * BlockSize + 20);
            Button_CancelAndExit.Location = new Point(18+ButtonWidth, Rows * BlockSize + 20);
            Button_Reset.Location = new Point(24+2*ButtonWidth, Rows * BlockSize + 20);
            
            surface = new Bitmap(Panel_Main.Width, Panel_Main.Height);
            Grap = Graphics.FromImage(surface);
            Panel_Main.BackgroundImage = surface;
            Panel_Main.BackgroundImageLayout = ImageLayout.None;

            Blocks = new BlockTypes[rows, columns];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (WallPositions.Contains(new Point(i, j)))
                    {
                        Blocks[i, j] = BlockTypes.WallBlock;
                        DrawBlock(new Point(i, j), BlockTypes.WallBlock);
                    }
                    else
                    {
                        Blocks[i, j] = BlockTypes.EmptyBlock;
                        DrawBlock(new Point(i, j), BlockTypes.EmptyBlock);
                    }
                }
            }

            this.ForbiddenWallPositions = ForbiddenWallPositions;

            for (int i = 0; i <= Rows; i++)
            {
                Grap.DrawLine(Pens.Black, new Point(0, i * BlockSize), new Point(Columns * BlockSize, i * BlockSize));
            }
            for (int j = 0; j <= Columns; j++)
            {
                Grap.DrawLine(Pens.Black, new Point(j * BlockSize, 0), new Point(j * BlockSize, Rows * BlockSize));
            }
            for (int i = 0; i < ForbiddenWallPositions.Length; i++)
            {
                Grap.FillRectangle(Brushes.DarkRed, ForbiddenWallPositions[i].Y * BlockSize+1,
                    ForbiddenWallPositions[i].X * BlockSize+1, BlockSize-1, BlockSize-1);
            }

        }

        private void DrawBlock(Point point, BlockTypes blockType)
        {
            Grap.FillRectangle(brushes[blockType], point.Y * BlockSize + 1,
                    point.X * BlockSize + 1, BlockSize - 1, BlockSize - 1);
        }

        private void Button_Reset_Click(object sender, EventArgs e)
        {
            WallPositions.Clear();
            Panel_Main.Invalidate();
        }

        private void Buttton_CancelAndExit_Click(object sender, EventArgs e)
        {
            WallPositions.Clear();
            this.Close();
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
                        DrawBlock(new Point(i, j), BlockTypes.WallBlock);
                        //Grap.FillRectangle(brushes[BlockTypes.WallBlock], j * BlockSize + 1,
                        //    i * BlockSize + 1, BlockSize - 1, BlockSize - 1);
                    }
                    if (Blocks[i, j] == BlockTypes.EmptyBlock)
                    {
                        DrawBlock(new Point(i, j), BlockTypes.EmptyBlock);
                        //Grap.FillRectangle(brushes[BlockTypes.EmptyBlock], j * BlockSize + 1,
                        //    i * BlockSize + 1, BlockSize - 1, BlockSize - 1);
                    }
                    for (int k = 0; k < ForbiddenWallPositions.Length; k++)
                    {
                        if (i == ForbiddenWallPositions[k].X && j == ForbiddenWallPositions[k].Y)
                        {
                            DrawBlock(new Point(i, j), BlockTypes.SnakeBody);
                            //Grap.FillRectangle(Brushes.DarkRed, j * BlockSize + 1,
                            //    i * BlockSize + 1, BlockSize - 1, BlockSize - 1);
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
