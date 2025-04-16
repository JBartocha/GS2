using System.Diagnostics;
using System.Text.Json;
using System.IO;

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
        private class Settings
        {
            public Point SnakeStartingHeadPosition { get; set; } = new Point(5, 5);
            public bool Pause { get; set; } = true;
            public bool GameOver { get; set; } = false;
            public int Level { get; set; } = 1;
            public int FoodCount { get; set; } = 3;
            public int LevelIncreaseInterval { get; set; } = 2;
            public int TickInMilliseconds { get; set; } = 500;
            public float DifficultyIncrease { get; set; } = 0.1f;
            public Point HeadPosition { get; set; } = new Point(5, 5);
            public string ForbiddenDirection { get; set; } = "Down";
            public int Moves { get; set; } = 0;
            public int FoodsEaten { get; set; } = 0;
            public int CellSize { get; set; } = 40;
        }

        private Snake? Snake;
        private Grid? Grid;
        //private Point SnakeStartingHeadPosition = new Point(5, 5);
        //private bool Pause = true;
        //private bool GameOver = false;
        //private int Level = 1;
        //private int FoodCount = 3; // Number of food items to spawn
        //private int LevelIncreaseInterval = 2; // Every 2 points
        //private int TickInMilliseconds = 500;
        //private float difficultyIncrease = 0.1f; // 10% increase
        //private Point HeadPosition = new Point(5, 5);
        //private string ForbiddenDirection = "Down";
        //private int Moves = 0;
        //private int FoodsEaten = 0;
        //private int CellSize = 40;

        Settings SS = new Settings();
        public Graphics? grap;
        Bitmap? surface;
        private PeriodicTimer? timer;


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
            
            SerializeConfigSettings();

            this.Grid = new Grid(11, 11, SS.CellSize, grap);
            this.Snake = new Snake(new Point(SS.SnakeStartingHeadPosition.X, SS.SnakeStartingHeadPosition.Y));

            for(int i = 0; i < SS.FoodCount; i++)
            {
                if (Grid.AddFood())
                {
                    SetGameOver();
                }
            }

            Snake.SnakeMoveEvent += Grid.OnSnakeMoveEvent;
            Grid.BlockCollisionEvent += Snake.OnGridCollisionEvent;
            Grid.BlockCollisionEvent += OnGridCollisionEvent;
        }

        private void SerializeConfigSettings()
        {
            if (TestFileExists("Settings.json")) 
            {
                string json = File.ReadAllText("Settings.json");
                var deserializedSettings = JsonSerializer.Deserialize<Settings>(json);
                if (deserializedSettings != null)
                {
                    SS = deserializedSettings;
                }
                //Debug.WriteLine(json.ToString());
            }
            else
            {
                string json = JsonSerializer.Serialize(SS);
                File.WriteAllText("Settings.json", json);
                //Debug.WriteLine(json.ToString());
                //Debug.WriteLine("Settings file not found, using default settings.");
            }
        }

        private bool TestFileExists(string file)
        {
            System.IO.FileSystemInfo fileInfo = new System.IO.FileInfo(file);
            if (fileInfo.Exists)
            {
                //Debug.WriteLine("File exists: " + file);
                return true;
            }
            else
            {
                //Debug.WriteLine("File does not exist: " + file);
                return false;
            }
        }

        //EVENT from GRID
        private void OnGridCollisionEvent(object sender, GridCollisionArgs args)
        {
            if (args.IsCollision)
            {
                SetGameOver();
            }
            else
            {
                if (args.BlockType == BlockTypes.FoodBlock)
                {
                    if (Grid.AddFood())
                    {
                        SetGameOver();
                    }
                    Label_Food_Eaten.Text = "Points: " + ++SS.FoodsEaten;
                    if (SS.FoodsEaten % SS.LevelIncreaseInterval == 0)
                    {
                        Label_Level.Text = "LEVEL: " + ++SS.Level;
                        IncreaseSpeed();
                    }
                }
            }
        }

        public async Task<bool> RunTimer()
        {
            timer = new PeriodicTimer(TimeSpan.FromMilliseconds(SS.TickInMilliseconds));
            float i = 0f;
            int MoveCounter = 0;
            while (await timer.WaitForNextTickAsync())
            {
                timer.Period = TimeSpan.FromMilliseconds(SS.TickInMilliseconds);
                i += SS.TickInMilliseconds / 1000f;
                Label_Timer.Text = ConvertToHHMMSS((int)i);
                if (!SS.Pause)
                {
                    // Precalculate collision for next move
                    Snake.Move();

                    if (!SS.GameOver)
                    {
                        Label_Moves.Text = "Moves: " + MoveCounter;
                        Label_Moves.Text = "Moves: " + ++SS.Moves; // TODO (duplicita?)
                        SS.ForbiddenDirection = Snake.GetForbiddenMoveDirection();
                        SS.HeadPosition = Snake.GetSnakeHeadPosition();
                        Panel_Main.Refresh();
                    }
                }
            }
            //Panel_Main.Refresh();
            return true;
        }

        private void ResetGame()
        {
            SS.FoodsEaten = 0;
            SS.Moves = 0;
            Label_Food_Eaten.Text = "Points: " + SS.FoodsEaten;
            Label_Moves.Text = "Moves: " + SS.Moves;
            SS.GameOver = false;
            SS.Pause = true;
            Button_Pause.Text = "Start";
            SS.ForbiddenDirection = "Down";
            SS.TickInMilliseconds = 500;
            Label_Speed.Text = "Speed: " + SS.TickInMilliseconds + "ms";

            InitializeGrid();

            RunTimer();
        }

        private void SetGameOver()
        {
            Button_Pause.Text = "Unpause";
            SS.Pause = true;
            //Grid.CrossSnakeHead(grap);
            Panel_Main.Refresh();
            SS.GameOver = true;
            MessageBox.Show("Game Over");
        }

        private void IncreaseSpeed()
        {
            if (SS.TickInMilliseconds > 100)
            {
                SS.TickInMilliseconds -= (int)(SS.TickInMilliseconds * SS.DifficultyIncrease);
                Debug.WriteLine("Speed increased to: " + SS.TickInMilliseconds);
                Label_Speed.Text = "Speed: " + SS.TickInMilliseconds + "ms";
            }
            else
            {
                SS.TickInMilliseconds = 100;
            }
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

            int deltaX = Cursor.X - ((SS.HeadPosition.X) * SS.CellSize + (SS.CellSize / 2));
            int deltaY = Cursor.Y - ((SS.HeadPosition.Y) * SS.CellSize + (SS.CellSize / 2));
            if (Math.Abs(deltaX) > Math.Abs(deltaY))
            {
                if (deltaX > 0)
                {
                    if (SS.ForbiddenDirection != "Right")
                    {
                        Snake.SetMovement("Right");
                        Label_Movement_Direction.Text = "Right";
                    }
                }
                else
                {
                    if (this.SS.ForbiddenDirection != "Left")
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
                    if (SS.ForbiddenDirection != "Down")
                    {
                        Snake.SetMovement("Down");
                        Label_Movement_Direction.Text = "Down";
                    }
                }
                else
                {
                    if (SS.ForbiddenDirection != "Up")
                    {
                        Snake.SetMovement("Up");
                        Label_Movement_Direction.Text = "Up";
                    }
                }
            }
        }

        private void Button_Pause_Click(object sender, EventArgs e)
        {
            if (SS.Pause || Button_Pause.Text == "Start")
            {
                Button_Pause.Text = "Pause";
                SS.Pause = false;
            }
            else
            {
                Button_Pause.Text = "Resume";
                SS.Pause = true;
            }
        }

        private void Panel_Main_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Restart_Click(object sender, EventArgs e)
        {
            ResetGame();
        }
    }
}
