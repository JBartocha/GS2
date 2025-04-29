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
        OutOfBoundsBlock
    }

    public struct MainFormSettings
    {
        public int Level { get; set; } = 0;
        public int FoodsEaten { get; set; } = 0;
        public int Moves { get; set; } = 0;
        public Point HeadPosition { get; set; } = new Point(5, 5);
        public bool GameOver { get; set; } = false;
        public string ForbiddenDirection { get; set; } = "Down";
        public Point SnakeStartingHeadPosition { get; set; } = new Point(5, 5);
        public bool Pause { get; set; } = true;

        public MainFormSettings()
        {
        }
    }

    public partial class Main_Form : Form
    {
        private Snake? Snake;
        private GameRecord GameRecord = new GameRecord();
        private Record record = new Record();

        private SnakeGameSettings GS = new SnakeGameSettings();
        private MainFormSettings MFS = new MainFormSettings(); 
        private Graphics? grap;
        private Bitmap? surface;

        private PeriodicTimer? timer;
        private string StartButtonText = "Start";
        private bool Simulation = false;

        public Main_Form()
        {
            InitializeComponent();

            ResetGame();
        }

        private void ResetGame()
        {

            string json = File.ReadAllText(SnakeGameSettings.JsonSaveFileName);

            if (Simulation == false)
            {
                var deserializedSettings = JsonSerializer.Deserialize<SnakeGameSettings>(json);
                if (deserializedSettings != null) // TODO abundant?
                {
                    GS = deserializedSettings;
                }
                else
                    throw new Exception("Failed to deserialize settings after ResetGame().");
            }
            else 
            {
                // TODO - Simulace - zacatek
            }

            InitializeGrid();

            AddStartingFood();

            ResetFormVariables();

            RegularTimer();
        }

        private void AddStartingFood()
        {
            if (Simulation)
            {
                // TODO - Simulace - 2.poradi
                for (int i = 0; i < GS.FoodCount; i++)
                {
                    Point Food = record.StartingFoodPositions[i];
                    Snake.AddFood(Food, true);
                }
            }
            else
            {
                GameRecord = new GameRecord();
                for (int i = 0; i < GS.FoodCount; i++)
                {
                    Snake.AddFood(true);
                }
            }
        }

        private void InitializeGrid()
        {
            surface = new Bitmap(Panel_Main.Width, Panel_Main.Height);
            grap = Graphics.FromImage(surface);
            Panel_Main.BackgroundImage = surface;
            Panel_Main.BackgroundImageLayout = ImageLayout.None;

            SaveSettingsToFile(SnakeGameSettings.JsonSaveFileName);

            if (Snake != null)
            {
                Snake.CellCollisionEvent -= OnCellCollisionEvent;
                Snake.FoodEatenEvent -= OnFoodEatenEvent;
            }
            
            this.Snake = new Snake(new Point(MFS.SnakeStartingHeadPosition.X, MFS.SnakeStartingHeadPosition.Y), 
                GS.Rows, GS.Columns, GS.CellSize, grap);
            Snake.SetMovement("Right");

            Snake.CellCollisionEvent += OnCellCollisionEvent;
            Snake.FoodEatenEvent += OnFoodEatenEvent;

        }

        private void ResetFormVariables()
        {
            Label_Food_Eaten.Text = "Points: " + MFS.FoodsEaten;
            Label_Moves.Text = "Moves: " + MFS.Moves;
            Button_Pause.Text = StartButtonText;
            Label_Speed.Text = "Speed: " + GS.TickInMilliseconds + "ms";
            Label_Level.Text = "LEVEL: " + MFS.Level;

            MFS.Level = 0;
            MFS.FoodsEaten = 0;
            MFS.Moves = 0;
            MFS.HeadPosition = new Point(5, 5);
            MFS.GameOver = false;
            MFS.ForbiddenDirection = "Down";
            MFS.Pause = true;
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

        private void SaveSettingsToFile(String JsonFilename)
        {
            if (TestFileExists(JsonFilename))
            {
                string json = File.ReadAllText(JsonFilename);
                var deserializedSettings = JsonSerializer.Deserialize<SnakeGameSettings>(json);
                if (deserializedSettings != null)
                {
                    GS = deserializedSettings;
                }
            }
            else
            {
                string json = JsonSerializer.Serialize(GS);
                File.WriteAllText(SnakeGameSettings.JsonSaveFileName, json);
            }
        }

        
        private void OnFoodEatenEvent(object sender, EventArgs args)
        {
            MFS.FoodsEaten++;
            if (MFS.FoodsEaten % GS.LevelIncreaseInterval == 0)
            {
                Label_Level.Text = "LEVEL: " + ++MFS.Level;
                IncreaseSpeed();
            }
            if (Simulation)
            {
                Point FoodPos;
                if(record.Turns[MFS.Moves].GeneratedFoodPosition.HasValue)
                {
                    FoodPos = record.Turns[MFS.Moves].GeneratedFoodPosition.Value;
                    Snake.AddFood(FoodPos, true);
                }
                else
                {
                    throw new Exception("Food position is null during simulation when expected Value.");
                }
            }
            else
            {
                Snake.AddFood(false);
            }
        }

        private void OnCellCollisionEvent(object sender, GridCollisionArgs args)
        {
            if (this.Simulation)
            {
                this.Simulation = false; // After end of simulation its regular game
                SetGameOver(args.Message);
            }
            else // regular
            {

                SetGameOver(args.Message);
            }
        }



        private void SetGameOver(string Message = "Game Over")
        {
            if (!Simulation)
            {
                string AdditionalMessage = "\nDo you want to save your record?";
                bool Save = SelectionMessageBox(Message + AdditionalMessage, "Game Over");
                if(Save)
                {
                    Record rec = Snake.GetGameRecord();
                    GameRecord.SaveGameRecord(GS, rec); // SAVE RECORD TO DATABASE
                    Debug.WriteLine(rec.toString());
                }
            }
            else
            {
                MessageBox.Show(Message);
            }
            timer.Dispose();
            Button_Pause.Text = StartButtonText;
            MFS.Pause = true;
            Panel_Main.Invalidate();
            MFS.GameOver = true;
            Simulation = false;
        }

        private bool SelectionMessageBox(string message, string caption = "")
        {
            DialogResult result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void IncreaseSpeed()
        {
            if (GS.TickInMilliseconds > 100)
            {
                GS.TickInMilliseconds -= (int)(GS.TickInMilliseconds * GS.DifficultyIncrease);
                Label_Speed.Text = "Speed: " + GS.TickInMilliseconds + "ms";
            }
            else
            {
                GS.TickInMilliseconds = 100;
            }
        }

        private string ConvertToHHMMSS(int seconds)
        {
            TimeSpan time = TimeSpan.FromSeconds(seconds);
            return string.Format("{0:D2}:{1:D2}:{2:D2}", (int)time.TotalHours, time.Minutes, time.Seconds);
        }

        private void SetMovementForSnake(string direction)
        {
            if (MFS.ForbiddenDirection != direction)
            {
                if(Snake.SetMovement(direction))
                Label_Movement_Direction.Text = direction;
            }
        }

        public async Task<bool> RegularTimer()
        {
            timer = new PeriodicTimer(TimeSpan.FromMilliseconds(GS.TickInMilliseconds));
            float i = 0f;
            //int MoveCounter = 0;
            while (await timer.WaitForNextTickAsync())
            {
                timer.Period = TimeSpan.FromMilliseconds(GS.TickInMilliseconds);
                i += GS.TickInMilliseconds / 1000f;
                Label_Timer.Text = "Running Time: " + ConvertToHHMMSS((int)i);
                if (!MFS.Pause)
                {
                    if (Simulation)
                    {
                        Snake.SetMovement(record.Turns[MFS.Moves].MoveDirection);
                    }
                        Snake.Move();
                    if (!MFS.GameOver)
                    {
                        Label_Moves.Text = "Moves: " + ++MFS.Moves; // TODO (duplicita?)
                        MFS.ForbiddenDirection = Snake.GetForbiddenMoveDirection();
                        MFS.HeadPosition = Snake.GetSnakeHeadPosition();
                        List<Region> reg = Snake.GetRegion();
                        Debug.WriteLine("---------------------------");
                        Debug.WriteLine("Region: " + reg.ToString());
                        foreach(Region r in reg)
                        {
                            Panel_Main.Invalidate(r);
                        }
                    }
                }
            }
            return true;
        }

        private void Button_Pause_Click(object sender, EventArgs e)
        {
            if (!MFS.GameOver)
            {
                if (MFS.Pause || Button_Pause.Text == StartButtonText) //TODO - probably Button_Pause.Text == StartButtonText not needed
                {
                    Button_Pause.Text = "Pause";
                    MFS.Pause = false;
                }
                else
                {
                    Button_Pause.Text = "Resume";
                    MFS.Pause = true;
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
            if (SelectedID != 0)
            {
                record = GameRecord.LoadGameRecord(SelectedID);

                GS = GameRecord.GetJsonSettings();

                this.Simulation = true;
                Debug.WriteLine("Okno records zavreno. vybrane ID: " + SelectedID);
                ResetGame();
                GS.UseKeyboardToMove = false;
                GS.UseMousePositionToMove = false;
                ResetFormVariables();
            }
            else
            {
                //Nothing selected (cancel button instead of save)
            }

        }

        private void PanelMain_MouseMove(object sender, MouseEventArgs e)
        {
            if (GS.UseMousePositionToMove)
            {
                Label_Mouse_Position.Text = $"X:{e.Location.X} Y:{e.Location.Y}";

                Point Cursor = new Point(e.Location.X, e.Location.Y);

                int deltaX = Cursor.X - ((MFS.HeadPosition.X) * GS.CellSize + (GS.CellSize / 2));
                int deltaY = Cursor.Y - ((MFS.HeadPosition.Y) * GS.CellSize + (GS.CellSize / 2));
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

        private void Main_Form_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (GS.UseKeyboardToMove)
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
                if (MFS.Pause)
                {
                    Button_Pause.Text = "Pause";
                    MFS.Pause = false;
                }
                else
                {
                    Button_Pause.Text = "Resume";
                    MFS.Pause = true;
                }
            }
        }
    }
}
