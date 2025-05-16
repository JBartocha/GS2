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
        private Snake? _Snake;
        private GameRecord _GameRecord = new GameRecord();
        private Record _Record = new Record();

        private Settings _SS = new Settings();
        private Graphics? _grap;
        private Bitmap? _surface;

        private PeriodicTimer? _Timer;
        private string _StartButtonText = "Start";
        private bool _Simulation = false;

        public Main_Form()
        {
            LoadSettingsFromFile();

            InitializeComponent();

            #pragma warning disable CS8602 // Dereference of a possibly null reference.
            Panel_Main.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance 
                | System.Reflection.BindingFlags.NonPublic).SetValue(Panel_Main, true, null);
            #pragma warning restore CS8602 // Dereference of a possibly null reference.

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
            if (_Simulation)
            {
                for (int i = 0; i < _SS.FoodCount; i++)
                {
                    Point Food = _Record.StartingFoodPositions[i];
                    _Snake.AddFood(Food, true);
                }
            }
            else
            {
                _GameRecord = new GameRecord();
                for (int i = 0; i < _SS.FoodCount; i++)
                {
                    _Snake.AddFood(true);
                }
            }
        }

        private int ScoreCounter()
        {
            //Starting speed koefficient
            double StartingSpeed = _SS.TickInMilliseconds / 1000.0;
            StartingSpeed = 1.2 / Math.Pow(StartingSpeed, 2);

            //Speed-up koefficient
            double SpeedIncrease = 0.1 / _SS.DifficultyIncrease;

            //Current speed koefficient
            double CurrentSpeed = _SS.CurrentSpeed / 1000.0;
            CurrentSpeed = 1 / Math.Pow(CurrentSpeed, 2);

            //Food Count koefficient
            double FoodMultiplier = (1.0 / (_SS.FoodCount)) * Math.Pow(_SS.FoodCount, 0.75);

            //Level increase koefficient
            double LevelIncreaseIntervalMultiplier = 4.0 / _SS.LevelIncreaseInterval;

            //Size of grid not included - maybe change?

            double result = CurrentSpeed * SpeedIncrease * StartingSpeed * FoodMultiplier * LevelIncreaseIntervalMultiplier;

            return Convert.ToInt32(result);
        }

        private void AddWalls()
        {
            for (int i = 0; i < _SS.WallPositions.Count; i++)
            {
                Point Wall = _SS.WallPositions[i];
                _Snake.AddWall(Wall);
            }
        }

        private void InitializeGrid()
        {
            _surface = new Bitmap(Panel_Main.Width, Panel_Main.Height);
            _grap = Graphics.FromImage(_surface);
            Panel_Main.BackgroundImage = _surface;
            Panel_Main.BackgroundImageLayout = ImageLayout.None;

            if (_Snake != null)
            {
                _Snake.CellCollisionEvent -= OnCellCollisionEvent;
                _Snake.FoodEatenEvent -= OnFoodEatenEvent;
                _Snake.FullGridEvent -= OnNoPlaceForFoodEvent;
            }

            this._Snake = new Snake(new Point(_SS.SnakeStartingHeadPosition.X, _SS.SnakeStartingHeadPosition.Y),
                _SS.Rows, _SS.Columns, _SS.CellSize, _grap);
            _Snake.SetMovement("Right");

            _Snake.CellCollisionEvent += OnCellCollisionEvent;
            _Snake.FoodEatenEvent += OnFoodEatenEvent;
            _Snake.FullGridEvent += OnNoPlaceForFoodEvent;
        }

        private void FormularEntitiesResizing()
        {
            int Width = _SS.Columns * _SS.CellSize + 260;
            int Height = _SS.Rows * _SS.CellSize + 70;
            if (Height < 510)
            {
                Height = 510;
            }

            this.Size = new Size(Width, Height);
            Panel_Main.Size = new Size(_SS.Columns * _SS.CellSize + 1, _SS.Rows * _SS.CellSize + 1);
            Panel_Right.Location = new Point(Panel_Main.Width + 40, Panel_Main.Location.Y);
            Panel_Main.BackColor = Color.LightGray;
        }

        private void ResetFormVariables()
        {
            _SS.Level = 0;
            _SS.FoodsEaten = 0;
            _SS.Moves = 0;
            _SS.Score = 0;
            _SS.HeadPosition = new Point(5, 5);
            _SS.GameOver = false;
            _SS.ForbiddenDirection = "Down";
            _SS.Pause = true;
            _SS.CurrentSpeed = _SS.TickInMilliseconds;

            Label_Food_Eaten.Text = "Points: " + _SS.FoodsEaten;
            Label_Moves.Text = "Moves: " + _SS.Moves;
            Button_Pause.Text = _StartButtonText;
            Label_Speed.Text = "Speed: " + _SS.CurrentSpeed + "ms";
            Label_Level.Text = "LEVEL: " + _SS.Level;
            Label_Score.Text = "Score: " + _SS.Score;

        }

        private void OnFoodEatenEvent(object sender, EventArgs args)
        {
            _SS.FoodsEaten++;
            _SS.Score += ScoreCounter();
            Label_Score.Text = "Score: " + _SS.Score;

            if (_SS.FoodsEaten % _SS.LevelIncreaseInterval == 0)
            {
                Label_Level.Text = "LEVEL: " + ++_SS.Level;
                IncreaseSpeed();
            }
            if (_Simulation)
            {
                Point FoodPos;
                if (_Record.Turns[_SS.Moves].GeneratedFoodPosition.HasValue)
                {
                    FoodPos = _Record.Turns[_SS.Moves].GeneratedFoodPosition.Value;
                    _Snake.AddFood(FoodPos, true);
                }
                else
                {
                    throw new Exception("Food position is null during simulation when expected Value.");
                }
            }
            else
            {
                _Snake.AddFood(false);
            }

        }

        private void OnCellCollisionEvent(object sender, GridCollisionArgs args)
        {
            if (this._Simulation)
            {
                SetGameOver(args.Message);
                this._Simulation = false; // After end of simulation its regular game
            }
            else // regular game
            {
                SetGameOver(args.Message);
            }
        }

        private void OnNoPlaceForFoodEvent(object sender, EventArgs args)
        {
            if (_Simulation)
            {
                SetGameOver("No place for food. Simulation ended.");
                this._Simulation = false; // After end of simulation its regular game
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
                string jsonString = JsonSerializer.Serialize(_SS);
                File.WriteAllText(Settings.JsonSaveFileName, jsonString);
            }

            string json = File.ReadAllText(Settings.JsonSaveFileName);

            var deserializedSettings = JsonSerializer.Deserialize<Settings>(json);
            if (deserializedSettings != null) // TODO abundant?
            {
                _SS = deserializedSettings;
            }
            else
                throw new Exception("Failed to deserialize settings.");
        }

        private void SetGameOver(string Message = "Game Over")
        {
            if (!_Simulation)
            {
                string AdditionalMessage = "\nDo you want to save your record?";
                bool Save = SelectionMessageBox(Message + AdditionalMessage, "Game Over");
                if (Save)
                {
                    Record rec = _Snake.GetGameRecord();
                    _GameRecord.SaveGameRecord(_SS, rec); // SAVE RECORD TO DATABASE
                }
            }
            else
            {
                MessageBox.Show(Message);
            }
            _Timer.Dispose();
            Button_Pause.Text = _StartButtonText;
            _SS.Pause = true;
            Panel_Main.Invalidate();
            _SS.GameOver = true;
            _Simulation = false;
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
            if (_SS.CurrentSpeed > 100)
            {
                _SS.CurrentSpeed -= (int)(_SS.CurrentSpeed * _SS.DifficultyIncrease);
                Label_Speed.Text = "Speed: " + _SS.CurrentSpeed + "ms";
            }
            else
            {
                _SS.CurrentSpeed = 100;
            }
        }

        private string ConvertToHHMMSS(int seconds)
        {
            TimeSpan time = TimeSpan.FromSeconds(seconds);
            return string.Format("{0:D2}:{1:D2}:{2:D2}", (int)time.TotalHours, time.Minutes, time.Seconds);
        }

        private void SetMovementForSnake(string direction)
        {
            if (_SS.ForbiddenDirection != direction)
            {
                if (_Snake.SetMovement(direction))
                    Label_Movement_Direction.Text = direction;
            }
        }

        public async Task<bool> TurnPeriodicTimer()
        {
            _Timer = new PeriodicTimer(TimeSpan.FromMilliseconds(_SS.CurrentSpeed));
            float i = 0f;
            //int MoveCounter = 0;
            while (await _Timer.WaitForNextTickAsync())
            {
                _Timer.Period = TimeSpan.FromMilliseconds(_SS.CurrentSpeed);
                i += _SS.CurrentSpeed / 1000f;
                Label_Timer.Text = "Running Time: " + ConvertToHHMMSS((int)i);
                if (!_SS.Pause)
                {
                    if (_Simulation)
                    {
                        _Snake.SetMovement(_Record.Turns[_SS.Moves].MoveDirection);
                    }
                    _Snake.Move();
                    if (!_SS.GameOver)
                    {
                        Label_Moves.Text = "Moves: " + ++_SS.Moves; // TODO (duplicita?)
                        _SS.ForbiddenDirection = _Snake.GetForbiddenMoveDirection();
                        _SS.HeadPosition = _Snake.GetSnakeHeadPosition();


                        List<Region> reg = _Snake.GetRegion(); // For redrawing only blocks that changed
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
            if (!_SS.GameOver)
            {
                if (_SS.Pause || Button_Pause.Text == _StartButtonText) //TODO - probably Button_Pause.Text == StartButtonText not needed
                {
                    Button_Pause.Text = "Pause";
                    _SS.Pause = false;
                }
                else
                {
                    Button_Pause.Text = "Resume";
                    _SS.Pause = true;
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
            RecordForm recordForm = new RecordForm(this._GameRecord);
            recordForm.ShowDialog();
            this.Show();
            int SelectedID = recordForm.GetSelectedID();
            if (SelectedID != 0)
            {
                _Record = _GameRecord.LoadGameRecord(SelectedID);

                _SS = _GameRecord.GetJsonSettings();

                this._Simulation = true;
                Debug.WriteLine("Okno records zavreno. vybrane ID: " + SelectedID);
                ResetGame();
                _SS.UseKeyboardToMove = false;
                _SS.UseMousePositionToMove = false;
                ResetFormVariables();
            }
            else
            {
                //Nothing selected (cancel button instead of save)
            }
        }

        private void PanelMain_MouseMove(object sender, MouseEventArgs e)
        {
            if (_SS.UseMousePositionToMove)
            {
                Label_Mouse_Position.Text = $"X:{e.Location.X} Y:{e.Location.Y}";

                Point Cursor = new Point(e.Location.X, e.Location.Y);

                int DeltaWidth = Cursor.X - ((_SS.HeadPosition.Y) * _SS.CellSize + (_SS.CellSize / 2));
                int DeltaHeight = Cursor.Y - ((_SS.HeadPosition.X) * _SS.CellSize + (_SS.CellSize / 2));

                if(Math.Abs(DeltaWidth) > Math.Abs(DeltaHeight))
                {
                    if (DeltaWidth > 0)
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
                    if (DeltaHeight > 0)
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
            if (_SS.UseKeyboardToMove)
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
                if (_SS.Pause)
                {
                    Button_Pause.Text = "Pause";
                    _SS.Pause = false;
                }
                else
                {
                    Button_Pause.Text = "Resume";
                    _SS.Pause = true;
                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Spuštìní Hamiltonianského cyklu nad aktuální møížkou? Složitost algoritmu pro výpoèet algoritmu je O(n!)" +
                " - pøi více než 55 non-wall blocích trvá výpoèet velmi dlouho." +
                "\nAlgoritmus provede výpoèet a zobrazí na møížce cestu." +
                "\nProvést výpoèet?", // Message
                "Spustit Výpoèet?",                 // Title
                MessageBoxButtons.YesNo,            // Buttons
                MessageBoxIcon.Question             // Icon
            );
            if (result == DialogResult.Yes)
            {
                this.Hide();
                HamiltonianForm HF = new HamiltonianForm(_SS);
                HF.ShowDialog();
                this.Show();
            }
            else
            {
                //do nothing
            }
        }
    }
}
