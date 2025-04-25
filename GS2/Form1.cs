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
        private Snake? Snake;
        private Grid? Grid;
        private GameRecord GameRecord = new GameRecord();

        private SnakeGameSettings SS = new SnakeGameSettings();
        public Graphics? grap;
        Bitmap? surface;
        private PeriodicTimer? timer;
        private string LastMoveDirection;
        private string StartButtonText = "Start";
        private bool Simulation = false;


        public Main_Form()
        {
            InitializeComponent();

            ResetGame();

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

            for (int i = 0; i < SS.FoodCount; i++)
            {
                Point? FoodPos = Grid.AddFood(); //Redundant check in AddFood() method for empty space
                if (!FoodPos.HasValue)
                {
                    SetGameOver();
                }
                else
                {
                    GameRecord.AddGeneratedFoodAtStart(FoodPos.Value);
                }
            }

            Snake.SnakeMoveEvent += Grid.OnSnakeMoveEvent;
            Grid.BlockCollisionEvent += Snake.OnGridCollisionEvent;
            Grid.BlockCollisionEvent += OnGridCollisionEvent;
            Grid.NoFreeSpaceForFoodEvent += SetGameOverDueToNoSpaceForFoodEvent;
        }



        private void SerializeConfigSettings()
        {
            if (TestFileExists(SnakeGameSettings.JsonSaveFileName))
            {
                string json = File.ReadAllText(SnakeGameSettings.JsonSaveFileName);
                var deserializedSettings = JsonSerializer.Deserialize<SnakeGameSettings>(json);
                if (deserializedSettings != null)
                {
                    SS = deserializedSettings;
                }
            }
            else
            {
                string json = JsonSerializer.Serialize(SS);
                File.WriteAllText(SnakeGameSettings.JsonSaveFileName, json);
            }
        }

        private bool TestFileExists(string file)
        {
            System.IO.FileSystemInfo fileInfo = new System.IO.FileInfo(file);
            if (fileInfo.Exists)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //EVENT from GRID
        private void OnGridCollisionEvent(object sender, GridCollisionArgs args)
        {
            if (args.IsCollision)
            {
                GameRecord.AddSnakeMove(LastMoveDirection);
                SetGameOver();
            }
            else
            {
                if (args.BlockType == BlockTypes.FoodBlock)
                {
                    Point? FoodPos = Grid.AddFood();
                    GameRecord.AddSnakeMove(LastMoveDirection, FoodPos.Value);
                    //if (!FoodPos.HasValue)
                    //{
                    //    SetGameOver();
                    //}

                    Label_Food_Eaten.Text = "Points: " + ++SS.FoodsEaten;
                    if (SS.FoodsEaten % SS.LevelIncreaseInterval == 0)
                    {
                        Label_Level.Text = "LEVEL: " + ++SS.Level;
                        IncreaseSpeed();
                    }
                }
                else
                    GameRecord.AddSnakeMove(LastMoveDirection);
            }
        }

        public async Task<bool> RunTimer()
        {
            timer = new PeriodicTimer(TimeSpan.FromMilliseconds(SS.TickInMilliseconds));
            float i = 0f;
            //int MoveCounter = 0;
            while (await timer.WaitForNextTickAsync())
            {
                timer.Period = TimeSpan.FromMilliseconds(SS.TickInMilliseconds);
                i += SS.TickInMilliseconds / 1000f;
                Label_Timer.Text = "Running Time: " + ConvertToHHMMSS((int)i);
                if (!SS.Pause)
                {
                    LastMoveDirection = Snake.GetMovement();
                    Snake.Move();

                    if (!SS.GameOver)
                    {
                        //Label_Moves.Text = "Moves: " + MoveCounter;
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
            string json = File.ReadAllText(SnakeGameSettings.JsonSaveFileName);
            GameRecord = new GameRecord();

            var deserializedSettings = JsonSerializer.Deserialize<SnakeGameSettings>(json);
            if (deserializedSettings != null) // TODO abundant
            {
                SS = deserializedSettings;
            }
            else
                throw new Exception("Failed to deserialize settings after ResetGame().");

            Label_Food_Eaten.Text = "Points: " + SS.FoodsEaten;
            Label_Moves.Text = "Moves: " + SS.Moves;
            Button_Pause.Text = StartButtonText;
            Label_Speed.Text = "Speed: " + SS.TickInMilliseconds + "ms";
            Label_Level.Text = "LEVEL: " + SS.Level;
            InitializeGrid();

            RunTimer();
        }

        private void SetGameOver(string Message = "Game Over")
        {
            Button_Pause.Text = StartButtonText;
            SS.Pause = true;
            //Grid.CrossSnakeHead(grap);
            Panel_Main.Refresh();
            SS.GameOver = true;
            MessageBox.Show(Message);
            //GameRecord.ListValues();
            List<ListOfRecords> RecordList = GameRecord.ListAllRecords();
            
            GameRecord.SetJsonSettingsFile(SS.ToString());
            GameRecord.SaveGameRecord(SS);
            //GameRecord.LoadGameRecord(17);
        }

        private void SetGameOverDueToNoSpaceForFoodEvent(object sender, EventArgs args)
        {
            SetGameOver("No free space for food. Game Over.");
        }

        private void IncreaseSpeed()
        {
            if (SS.TickInMilliseconds > 100)
            {
                SS.TickInMilliseconds -= (int)(SS.TickInMilliseconds * SS.DifficultyIncrease);
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
            if (SS.UseMousePositionToMove)
            {
                Label_Mouse_Position.Text = $"X:{e.Location.X} Y:{e.Location.Y}";

                Point Cursor = new Point(e.Location.X, e.Location.Y);

                int deltaX = Cursor.X - ((SS.HeadPosition.X) * SS.CellSize + (SS.CellSize / 2));
                int deltaY = Cursor.Y - ((SS.HeadPosition.Y) * SS.CellSize + (SS.CellSize / 2));
                if (Math.Abs(deltaX) > Math.Abs(deltaY))
                {
                    if (deltaX > 0)
                    {
                        SetMovementForSnake("Right");
                    }
                    else
                    {
                        SetMovementForSnake("Left");
                    }
                }
                else
                {
                    if (deltaY > 0)
                    {
                        SetMovementForSnake("Down");
                    }
                    else
                    {
                        SetMovementForSnake("Up");
                    }
                }
            }
        }

        private void SetMovementForSnake(string direction)
        {
            if (SS.ForbiddenDirection != direction)
            {
                Snake.SetMovement(direction);
                Label_Movement_Direction.Text = direction;
            }
        }

        private void Button_Pause_Click(object sender, EventArgs e)
        {
            if (!SS.GameOver)
            {
                if (SS.Pause || Button_Pause.Text == StartButtonText) //TODO - probably Button_Pause.Text == StartButtonText not needed
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
        }

        private void Panel_Main_Paint(object sender, PaintEventArgs e)
        {
            // So lonely
        }

        private void Restart_Click(object sender, EventArgs e)
        {
            ResetGame();
        }

        private void Button_Options_Click(object sender, EventArgs e)
        {
            this.Hide();
            OptionsForm optionsForm = new OptionsForm();
            optionsForm.ShowDialog();
            this.Show();

        }
        private void Button_Load_Record_Click(object sender, EventArgs e)
        {
            this.Hide();
            RecordForm recordForm = new RecordForm(this.GameRecord);
            recordForm.ShowDialog();
            this.Show();
            int SelectedID = recordForm.GetSelectedID();
            Debug.WriteLine("Okno records zavreno. vybrane ID: " + SelectedID);
        }

        private void Main_Form_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (SS.UseKeyboardToMove)
            {
                if (e.KeyChar == 'd' || e.KeyChar == 'D')
                {
                    SetMovementForSnake("Right");
                }
                else if (e.KeyChar == 'a' || e.KeyChar == 'A')
                {
                    SetMovementForSnake("Left");
                }
                else if (e.KeyChar == 's' || e.KeyChar == 'S')
                {
                    SetMovementForSnake("Down");
                }
                else if (e.KeyChar == 'w' || e.KeyChar == 'W')
                {
                    SetMovementForSnake("Up");
                }
            }

            if (e.KeyChar == 'p' || e.KeyChar == 'P')
            {
                if (SS.Pause)
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
            
        }

    }
}
