using System.Diagnostics;

namespace GS2
{
    public enum BlockTypes
    {
        EmptyBlock,
        WallBlock,
        FoodBlock,
        SnakeBody,
        SnakeHead,
    }

    public partial class Main_Form : Form
    {
        private Snake Snake;
        private Grid Grid;
        private bool Pause = true;
        private bool GameOver = false;
        private int TickInMilliseconds = 500;
        private Point HeadPosition = new Point(5,5);
        private string ForbiddenDirection = "Down";
        private int Moves = 0;
        private int CellSize = 40;

        public Graphics grap;
        Bitmap surface;
        private PeriodicTimer timer;


        public Main_Form()
        {
            InitializeComponent();

            InitializeGrid();

            RunTimer();
        }

        private void InitializeGrid()
        {
            surface = new Bitmap(Panel_Main.Width, Panel_Main.Height);
            grap = Graphics.FromImage(surface);
            Panel_Main.BackgroundImage = surface;
            Panel_Main.BackgroundImageLayout = ImageLayout.None;

            this.Grid = new Grid(11, 11, CellSize, grap);
            this.Snake = new Snake(new Point(5, 5));

            Snake.SnakeMoveEvent += Grid.OnSnakeMoveEvent;
            Grid.BlockCollisionEvent += Snake.OnGridCollisionEvent;
            Grid.BlockCollisionEvent += OnGridCollisionEvent;
        }

        //EVENT from GRID
        private void OnGridCollisionEvent(object sender, GridCollisionArgs args)
        {
            if (args.IsCollision)
            {
                Button_Pause.Text = "Unpause";
                Pause = true;
                //Grid.CrossSnakeHead(grap);
                Panel_Main.Refresh();
                GameOver = true;
                MessageBox.Show("Game Over");
            }
            else
            {
                if (args.BlockType == BlockTypes.FoodBlock)
                {
                    Moves++;
                    Label_Moves.Text = "Moves: " + Moves;
                    Grid.AddFood();
                    Label_Food_Eaten.Text = "Points: " + Moves;
                }
            }
        }

        public async Task<bool> RunTimer()
        {
            timer = new PeriodicTimer(TimeSpan.FromMilliseconds(TickInMilliseconds));
            float i = 0f;
            int MoveCounter = 0;
            while (await timer.WaitForNextTickAsync())
            {
                i += TickInMilliseconds / 1000f;
                Label_Timer.Text = ConvertToHHMMSS((int)i);
                if (!Pause || GameOver)
                {
                    // Precalculate collision for next move
                    Snake.Move();
                    SetMoveCounter(++MoveCounter);

                    // predelat
                    if (false)
                    //if (IsCollision(Snake.GetPlannedMovePoint()))
                    {
                        Button_Pause.Text = "Unpause";
                        Pause = true;
                        //Grid.CrossSnakeHead(grap);
                        Panel_Main.Refresh();
                        GameOver = true;
                        MessageBox.Show("Game Over");
                        break;
                    }

                    ForbiddenDirection = Snake.GetForbiddenMoveDirection();
                    HeadPosition = Snake.GetSnakeHeadPosition();
                    Panel_Main.Refresh();
                }
            }
            //Panel_Main.Refresh();
            return true;
        }

        private string ConvertToHHMMSS(int seconds)
        {
            TimeSpan time = TimeSpan.FromSeconds(seconds);
            return string.Format("{0:D2}:{1:D2}:{2:D2}", (int)time.TotalHours, time.Minutes, time.Seconds);
        }

        private void PanelMain_MouseMove(object sender, MouseEventArgs e)
        {
            Label_Mouse_Position.Text = $"X:{e.Location.X} Y:{e.Location.Y}";

            Point Cursor = new Point(e.Location.X, e.Location.Y);

            int deltaX = Cursor.X - ((HeadPosition.X) * CellSize + (CellSize / 2));
            int deltaY = Cursor.Y - ((HeadPosition.Y) * CellSize + (CellSize / 2));
            if (Math.Abs(deltaX) > Math.Abs(deltaY))
            {
                if (deltaX > 0)
                {
                    if (this.ForbiddenDirection != "Right")
                    {
                        Snake.SetMovement("Right");
                        Label_Movement_Direction.Text = "Right";
                    }
                }
                else
                {
                    if (this.ForbiddenDirection != "Left")
                    {
                        Snake.SetMovement("Left");
                        Label_Movement_Direction.Text = "Left";
                    }
                }
            }
            else
            {
                if (deltaY > 0)
                {
                    if (this.ForbiddenDirection != "Down")
                    {
                        Snake.SetMovement("Down");
                        Label_Movement_Direction.Text = "Down";
                    }
                }
                else
                {
                    if (this.ForbiddenDirection != "Up")
                    {
                        Snake.SetMovement("Up");
                        Label_Movement_Direction.Text = "Up";
                    }
                }
            }
        }

        private void SetMoveCounter(int count)
        {
            Label_Moves.Text = "Moves: " + count;
        }

        private void Button_Pause_Click(object sender, EventArgs e)
        {
            if (Pause || Button_Pause.Text == "Start")
            {
                Button_Pause.Text = "Resume";
                Pause = false;
            }
            else
            {
                Button_Pause.Text = "Pause";
                Pause = true;
            }
        }

        private void Panel_Main_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
