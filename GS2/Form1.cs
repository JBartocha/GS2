using System.Configuration;
using System.Diagnostics;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic.ApplicationServices;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

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


    public partial class Main_Form : Form
    {
        private Snake? Snake;
        private GameRecord GameRecord = new GameRecord();
        private Record record = new Record();

        private Settings SS = new Settings();
        private Graphics? grap;
        private Bitmap? surface;

        private PeriodicTimer? timer;
        private string StartButtonText = "Start";
        private bool Simulation = false;

        public Main_Form()
        {
            LoadSettingsFromFile();

            InitializeComponent();

            FormularEntitiesResizing();

            ResetGame();
        }

        private void ResetGame()
        {
            FormularEntitiesResizing();

            InitializeGrid();

            AddWalls();
            
            AddStartingFood();

            ResetFormVariables();

            TurnPeriodicTimer();
        }

        private void AddStartingFood()
        {
            if (Simulation)
            {
                for (int i = 0; i < SS.FoodCount; i++)
                {
                    Point Food = record.StartingFoodPositions[i];
                    Snake.AddFood(Food, true);
                }
            }
            else
            {
                GameRecord = new GameRecord();
                for (int i = 0; i < SS.FoodCount; i++)
                {
                    Snake.AddFood(true);
                }
            }
        }

        private int ScoreCounter()
        {
            //starting speed koefficient
            double StartingSpeed = SS.TickInMilliseconds / 1000.0;
            StartingSpeed = 1.2 / Math.Pow(StartingSpeed, 2);

            //zrychlovací koefficient
            double SpeedIncrease = 0.1 / SS.DifficultyIncrease;

            //current speed koefficient
            double CurrentSpeed = SS.CurrentSpeed / 1000.0;
            CurrentSpeed = 1 / Math.Pow(CurrentSpeed,2);
            
            //Food Count koefficient
            double FoodMultiplier = (1.0 / (SS.FoodCount)) * Math.Pow(SS.FoodCount, 0.75);

            //Level increase koefficient
            double LevelIncreaseIntervalMultiplier = 4.0 / SS.LevelIncreaseInterval;

            //Size of grid not included - maybe change?

            double result = CurrentSpeed * SpeedIncrease * StartingSpeed * FoodMultiplier * LevelIncreaseIntervalMultiplier;

            return Convert.ToInt32(result);
        }

        private void AddWalls()
        {
            for(int i = 0; i < SS.WallPositions.Count; i++)
            {
                Point Wall = SS.WallPositions[i];
                Snake.AddWall(Wall);
            }
        }

        private void InitializeGrid()
        {
            surface = new Bitmap(Panel_Main.Width, Panel_Main.Height);
            grap = Graphics.FromImage(surface);
            Panel_Main.BackgroundImage = surface;
            Panel_Main.BackgroundImageLayout = ImageLayout.None;
             
            if (Snake != null)
            {
                Snake.CellCollisionEvent -= OnCellCollisionEvent;
                Snake.FoodEatenEvent -= OnFoodEatenEvent;
                Snake.FullGridEvent -= OnNoPlaceForFoodEvent;
            }

            this.Snake = new Snake(new Point(SS.SnakeStartingHeadPosition.X, SS.SnakeStartingHeadPosition.Y),
                SS.Rows, SS.Columns, SS.CellSize, grap);
            Snake.SetMovement("Right");

            Snake.CellCollisionEvent += OnCellCollisionEvent;
            Snake.FoodEatenEvent += OnFoodEatenEvent;
            Snake.FullGridEvent += OnNoPlaceForFoodEvent;
        }

        private void FormularEntitiesResizing()
        {
            int Width = SS.Columns * SS.CellSize + 260;
            int Height = SS.Rows * SS.CellSize + 70;
            if (Height < 500)
            {
                Height = 500;
            }

            this.Size = new Size(Width, Height);
            Panel_Main.Size = new Size(SS.Columns * SS.CellSize + 1, SS.Rows * SS.CellSize + 1);
            Panel_Right.Location = new Point(Panel_Main.Width + 40, Panel_Main.Location.Y);
        }

        private void ResetFormVariables()
        {
            SS.Level = 0;
            SS.FoodsEaten = 0;
            SS.Moves = 0;
            SS.Score = 0;
            SS.HeadPosition = new Point(5, 5);
            SS.GameOver = false;
            SS.ForbiddenDirection = "Down";
            SS.Pause = true;
            SS.CurrentSpeed = SS.TickInMilliseconds;

            Label_Food_Eaten.Text = "Points: " + SS.FoodsEaten;
            Label_Moves.Text = "Moves: " + SS.Moves;
            Button_Pause.Text = StartButtonText;
            Label_Speed.Text = "Speed: " + SS.CurrentSpeed + "ms";
            Label_Level.Text = "LEVEL: " + SS.Level;
            Label_Score.Text = "Score: " + SS.Score;

        }

        private void OnFoodEatenEvent(object sender, EventArgs args)
        {
            SS.FoodsEaten++;
            SS.Score += ScoreCounter();
            Debug.WriteLine("Score: " + SS.Score);
            Label_Score.Text = "Score: " + SS.Score;
            //TODO - opravit

            if (SS.FoodsEaten % SS.LevelIncreaseInterval == 0)
            {
                Label_Level.Text = "LEVEL: " + ++SS.Level;
                IncreaseSpeed();
            }
            if (Simulation)
            {
                Point FoodPos;
                if (record.Turns[SS.Moves].GeneratedFoodPosition.HasValue)
                {
                    FoodPos = record.Turns[SS.Moves].GeneratedFoodPosition.Value;
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
                SetGameOver(args.Message);
                this.Simulation = false; // After end of simulation its regular game
            }
            else // regular game
            {
                SetGameOver(args.Message);
            }
        }

        private void OnNoPlaceForFoodEvent(object sender, EventArgs args)
        {
            if (Simulation)
            {
                SetGameOver("No place for food. Simulation ended.");
                this.Simulation = false; // After end of simulation its regular game
            }
            else
            {
                SetGameOver("No place for food.");
            }
        }

        private void LoadSettingsFromFile()
        {
            if (File.Exists(Settings.JsonSaveFileName) == false)
            {
                string jsonString = JsonSerializer.Serialize(SS);
                File.WriteAllText(Settings.JsonSaveFileName, jsonString);
            }

            string json = File.ReadAllText(Settings.JsonSaveFileName);

            var deserializedSettings = JsonSerializer.Deserialize<Settings>(json);
            if (deserializedSettings != null) // TODO abundant?
            {
                SS = deserializedSettings;
            }
            else
                throw new Exception("Failed to deserialize settings after ResetGame().");
        }

        private void SetGameOver(string Message = "Game Over")
        {
            if (!Simulation)
            {
                string AdditionalMessage = "\nDo you want to save your record?";
                bool Save = SelectionMessageBox(Message + AdditionalMessage, "Game Over");
                if (Save)
                {
                    Record rec = Snake.GetGameRecord();
                    GameRecord.SaveGameRecord(SS, rec); // SAVE RECORD TO DATABASE
                }
            }
            else
            {
                MessageBox.Show(Message);
            }
            timer.Dispose();
            Button_Pause.Text = StartButtonText;
            SS.Pause = true;
            Panel_Main.Invalidate();
            SS.GameOver = true;
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
            if (SS.CurrentSpeed > 100)
            {
                SS.CurrentSpeed -= (int)(SS.CurrentSpeed * SS.DifficultyIncrease);
                Label_Speed.Text = "Speed: " + SS.CurrentSpeed + "ms";
            }
            else
            {
                SS.CurrentSpeed = 100;
            }
        }

        private string ConvertToHHMMSS(int seconds)
        {
            TimeSpan time = TimeSpan.FromSeconds(seconds);
            return string.Format("{0:D2}:{1:D2}:{2:D2}", (int)time.TotalHours, time.Minutes, time.Seconds);
        }

        private void SetMovementForSnake(string direction)
        {
            if (SS.ForbiddenDirection != direction)
            {
                if (Snake.SetMovement(direction))
                    Label_Movement_Direction.Text = direction;
            }
        }

        public async Task<bool> TurnPeriodicTimer()
        {
            timer = new PeriodicTimer(TimeSpan.FromMilliseconds(SS.CurrentSpeed));
            float i = 0f;
            //int MoveCounter = 0;
            while (await timer.WaitForNextTickAsync())
            {
                timer.Period = TimeSpan.FromMilliseconds(SS.CurrentSpeed);
                i += SS.CurrentSpeed / 1000f;
                Label_Timer.Text = "Running Time: " + ConvertToHHMMSS((int)i);
                if (!SS.Pause)
                {
                    if (Simulation)
                    {
                        Snake.SetMovement(record.Turns[SS.Moves].MoveDirection);
                    }
                    Snake.Move();
                    if (!SS.GameOver)
                    {
                        Label_Moves.Text = "Moves: " + ++SS.Moves; // TODO (duplicita?)
                        SS.ForbiddenDirection = Snake.GetForbiddenMoveDirection();
                        SS.HeadPosition = Snake.GetSnakeHeadPosition();


                        List<Region> reg = Snake.GetRegion(); // For redrawing only blocks that changed
                        foreach (Region r in reg)
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
            LoadSettingsFromFile();
            ResetGame();
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

                SS = GameRecord.GetJsonSettings();

                this.Simulation = true;
                Debug.WriteLine("Okno records zavreno. vybrane ID: " + SelectedID);
                ResetGame();
                SS.UseKeyboardToMove = false;
                SS.UseMousePositionToMove = false;
                ResetFormVariables();
            }
            else
            {
                //Nothing selected (cancel button instead of save)
            }
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
