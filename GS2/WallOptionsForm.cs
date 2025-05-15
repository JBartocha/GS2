using System.Data.Common;
using System.Diagnostics;
using System.Drawing;

namespace GS2
{
    public partial class WallOptionsForm : Form
    {
        private BlockTypes[,] _Blocks;
        private Point[] _ForbiddenWallPositions;
        private Graphics _Grap;
        private Bitmap _surface;
        private int _Rows;
        private int _Columns;
        private int _BlockSize;
        private List<Point> _WallPositions = new List<Point>();
        private List<Point> _OriginalWallPositions = new List<Point>();
        private bool _Multiselect = false;
        private Point? _MultiselectStart;
        private Point? _MultiselectEnd;
  
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

            #pragma warning disable CS8602 // Dereference of a possibly null reference.
            Panel_Main.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.NonPublic).SetValue(Panel_Main, true, null);
            #pragma warning restore CS8602 // Dereference of a possibly null reference.


            this._BlockSize = blockSize;
            this._Rows = rows;
            this._Columns = columns;
            this._WallPositions = wallPositions;
            this._OriginalWallPositions = new List<Point>(wallPositions);

            int ButtonWidth = Button_SaveAndExit.Width;
            if (_Columns * _BlockSize + 42 < ButtonWidth * 3 + 60)
            {
                this.Size = new Size(ButtonWidth * 3 + 60, _Rows * _BlockSize + 70 + Button_SaveAndExit.Height);
            }
            else
            {
                this.Size = new Size(_Columns * _BlockSize + 42, _Rows * _BlockSize + 70 + Button_SaveAndExit.Height);
            }
            Panel_Main.Size = new Size(_Columns * _BlockSize + 1, _Rows * _BlockSize + 1);
            Button_SaveAndExit.Location = new Point(12, _Rows * _BlockSize + 20);
            Button_CancelAndExit.Location = new Point(18 + ButtonWidth, _Rows * _BlockSize + 20);
            Button_Reset.Location = new Point(24 + 2 * ButtonWidth, _Rows * _BlockSize + 20);

            _surface = new Bitmap(Panel_Main.Width, Panel_Main.Height);
            _Grap = Graphics.FromImage(_surface);
            Panel_Main.BackgroundImage = _surface;
            Panel_Main.BackgroundImageLayout = ImageLayout.None;

            _Blocks = new BlockTypes[rows, columns];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (_WallPositions.Contains(new Point(i, j)))
                    {
                        _Blocks[i, j] = BlockTypes.WallBlock;
                        DrawBlock(new Point(i, j), BlockTypes.WallBlock);
                    }
                    else
                    {
                        _Blocks[i, j] = BlockTypes.EmptyBlock;
                        DrawBlock(new Point(i, j), BlockTypes.EmptyBlock);
                    }
                }
            }

            this._ForbiddenWallPositions = ForbiddenWallPositions;

            for (int i = 0; i <= _Rows; i++)
            {
                _Grap.DrawLine(Pens.Black, new Point(0, i * _BlockSize), new Point(_Columns * _BlockSize, i * _BlockSize));
            }
            for (int j = 0; j <= _Columns; j++)
            {
                _Grap.DrawLine(Pens.Black, new Point(j * _BlockSize, 0), new Point(j * _BlockSize, _Rows * _BlockSize));
            }
            for (int i = 0; i < ForbiddenWallPositions.Length; i++)
            {
                _Grap.FillRectangle(Brushes.DarkRed, ForbiddenWallPositions[i].Y * _BlockSize + 1,
                    ForbiddenWallPositions[i].X * _BlockSize + 1, _BlockSize - 1, _BlockSize - 1);
            }
            DrawBlocks();
        }

        private void DrawBlock(Point point, BlockTypes blockType)
        {
            _Grap.FillRectangle(brushes[blockType], point.Y * _BlockSize + 1,
                    point.X * _BlockSize + 1, _BlockSize - 1, _BlockSize - 1);
        }

        private void Button_Reset_Click(object sender, EventArgs e)
        {
            _WallPositions.Clear();
            Panel_Main.Invalidate();
        }

        private void Buttton_CancelAndExit_Click(object sender, EventArgs e)
        {
            _WallPositions.Clear();
            _WallPositions.AddRange(_OriginalWallPositions);
            this.Close();
        }

        private void Button_SaveAndExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DrawBlocks()
        {
            for (int i = 0; i < _Rows; i++)
            {
                for (int j = 0; j < _Columns; j++)
                {
                    if (_Blocks[i, j] == BlockTypes.WallBlock)
                    {
                        DrawBlock(new Point(i, j), BlockTypes.WallBlock);
                    }
                    if (_Blocks[i, j] == BlockTypes.EmptyBlock)
                    {
                        DrawBlock(new Point(i, j), BlockTypes.EmptyBlock);
                    }
                    for (int k = 0; k < _ForbiddenWallPositions.Length; k++)
                    {
                        if (i == _ForbiddenWallPositions[k].X && j == _ForbiddenWallPositions[k].Y)
                        {
                            DrawBlock(new Point(i, j), BlockTypes.SnakeBody);
                        }
                    }
                }
            }
        }

        private void Panel_Main_MouseDown(object sender, MouseEventArgs e)
        {
            _Multiselect = true;
            _MultiselectStart = new Point(e.X, e.Y);
        }

        private void Panel_Main_MouseMove(object sender, MouseEventArgs e)
        {
            if (_Multiselect)
            {
                _MultiselectEnd = new Point(e.X, e.Y);
            }
            Panel_Main.Invalidate();
        }

        private void Panel_Main_MouseUp(object sender, MouseEventArgs e)
        {
            ReverseBlocks();
            _Multiselect = false;
            _MultiselectStart = null;
            _MultiselectEnd = null;
        }

        private Point CheckBounds(Point point)
        {
            int maxX = (_Columns-1) * _BlockSize;
            int maxY = (_Rows-1) * _BlockSize;
            if (point.X < 0)
            {
                point.X = 0;
            }
            else if (point.X > maxX)
            {
                point.X = maxX;
            }
            if (point.Y < 0)
            {
                point.Y = 0;
            }
            else if (point.Y > maxY)
            {
                point.Y = maxY;
            }
            return point;
        }

        private void ReverseBlocks()
        {
            if (_MultiselectEnd != null)
            {
                Point start = _MultiselectStart.Value;
                Point end = _MultiselectEnd.Value;
                start = CheckBounds(start);
                end = CheckBounds(end);
                Point pom = new Point(Math.Min(start.X, end.X) / _BlockSize, Math.Min(start.Y, end.Y) / _BlockSize);
                end = new Point(Math.Max(start.X, end.X) / _BlockSize, Math.Max(start.Y, end.Y) / _BlockSize);
                start = pom;

                
                for (int column = start.X; column <= end.X; column++)
                {
                    for (int row = start.Y; row <= end.Y; row++)
                    {
                        Point BlockPosition = new Point(row, column);
                        if (_ForbiddenWallPositions.Contains(BlockPosition))
                        {
                            continue;
                        }
                        else if (_Blocks[row, column] == BlockTypes.EmptyBlock)
                        {
                            _Blocks[row, column] = BlockTypes.WallBlock;
                            _WallPositions.Add(BlockPosition);
                            DrawBlock(BlockPosition, BlockTypes.WallBlock);
                        }
                        else
                        {
                            _WallPositions.Remove(BlockPosition);
                            _Blocks[row, column] = BlockTypes.EmptyBlock;
                            DrawBlock(BlockPosition, BlockTypes.EmptyBlock);
                        }
                    }
                }
            }
            else
            {
                Point SinglePoint = new Point(_MultiselectStart.Value.Y / _BlockSize, _MultiselectStart.Value.X / _BlockSize);

                if (_ForbiddenWallPositions.Contains(SinglePoint))
                {
                    return;
                }
                else if (_Blocks[SinglePoint.X, SinglePoint.Y] == BlockTypes.EmptyBlock)
                {
                    _Blocks[SinglePoint.X, SinglePoint.Y] = BlockTypes.WallBlock;
                    _WallPositions.Add(SinglePoint);
                    DrawBlock(SinglePoint, BlockTypes.WallBlock);
                }
                else
                {
                    _WallPositions.Remove(SinglePoint);
                    _Blocks[SinglePoint.X, SinglePoint.Y] = BlockTypes.EmptyBlock;
                    DrawBlock(SinglePoint, BlockTypes.EmptyBlock);
                }
            }
        }

        private void Panel_Main_Paint(object sender, PaintEventArgs e)
        {
            if(_MultiselectStart != null && _MultiselectEnd != null)
            {
                Point StartPosition = _MultiselectStart.Value;
                Point EndPosition = _MultiselectEnd.Value;
                int minX = Math.Min(StartPosition.X, EndPosition.X);
                int minY = Math.Min(StartPosition.Y, EndPosition.Y);

                if (_Multiselect)
                {
                    e.Graphics.DrawRectangle(Pens.Black, minX, minY,
                        Math.Abs(EndPosition.X - StartPosition.X), Math.Abs(EndPosition.Y - StartPosition.Y));
                }
            }
        }
    }
}
