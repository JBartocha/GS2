using System.Diagnostics;
using System.Text.Json;
using Microsoft.Data.SqlClient;

namespace GS2
{
    public struct TurnRecord
    {
        public int TurnNumber = 0;
        public string MoveDirection;
        public Point? GeneratedFoodPosition;
        
        public TurnRecord(int turnNumber, string moveDirection)
        {
            TurnNumber = turnNumber;
            MoveDirection = moveDirection;
            GeneratedFoodPosition = null;
        }

        public TurnRecord(int turnNumber, string moveDirection, Point? generatedFoodPosition = null)
        {
            TurnNumber = turnNumber;
            MoveDirection = moveDirection;
            GeneratedFoodPosition = generatedFoodPosition;
        }
    }

    public struct Record
    {
        public string Settings;
        public List<Point> StartingFoodPositions;
        public List<TurnRecord> Turns;

        public Record()
        {
            Settings = string.Empty; // Initialize the non-nullable field with a default value  
            StartingFoodPositions = new List<Point>();
            Turns = new List<TurnRecord>();
        }

        public string toString()
        {
            System.String ret = "";
            for (int i = 0; i < StartingFoodPositions.Count; i++)
            {
                ret += "StartingFood Number: " + i + " = " + StartingFoodPositions[i].ToString() + "\n";
            }
            for (int i = 0; i < Turns.Count; i++)
            {
                ret += "Turn Number: " + Turns[i].TurnNumber + " Move Direction: " + Turns[i].MoveDirection + "\n";
                if (Turns[i].GeneratedFoodPosition != null)
                    ret += " Generated Food Position: " + Turns[i].GeneratedFoodPosition.ToString() + "\n";
            }
            return ret;
        }
    }

    public struct ListOfRecords
    {
        public string? ID;
        public DateTime Date;
        public int Level;
        public int Score;

        public ListOfRecords(string? iD, DateTime dT, int Lvl, int Score) : this()
        {
            this.ID = iD;
            this.Date = dT;
            this.Level = Lvl;
            this.Score = Score;
        }
    }

    interface IGameRecord
    {
        public void SaveGameRecord(Settings S, Record record);
        public Record LoadGameRecord(int ID);
        public Settings GetJsonSettings();
        public List<ListOfRecords> ListAllRecords();
    }

    public class GameRecord : IGameRecord
    {
        private int CurrentTurnNumber = 0; //•A turn is a single move made by the snake in the game.
        private Record RC = new Record();

        //private const string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Janba\\source\\repos\\GS2\\GS2\\SnakeDB.mdf;Integrated Security=True";
        private string connectionString = "";

        public GameRecord()
        {
            string DBfilename = Environment.CurrentDirectory;
            for (int i = 0; i < 3; i++)
            {
                DBfilename = Directory.GetParent(DBfilename).ToString();
            }
            DBfilename += "\\SnakeDB.mdf";

            var builder = new SqlConnectionStringBuilder
            {
                DataSource = @"(LocalDB)\MSSQLLocalDB",
                AttachDBFilename = @DBfilename,
                IntegratedSecurity = true
            };
            connectionString = builder.ConnectionString;
            Debug.WriteLine(connectionString);
        }

        public List<ListOfRecords> ListAllRecords()
        {
            try
            {
                // Updated to use Microsoft.Data.SqlClient.SqlConnection  
                using var connection = new SqlConnection(connectionString);
                connection.Open();

                List<ListOfRecords> PomList = new List<ListOfRecords>();

                string query = "SELECT GameNumbers_ID, Date, Level, Score FROM GameNumbers"; // Replace with your query  
                using var command = new SqlCommand(query, connection);
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string ID = reader["GameNumbers_ID"].ToString();
                    string Date = reader["Date"].ToString();
                    
                    string LevelString = reader["Level"].ToString();
                    int Level = 0;
                    if (int.TryParse(LevelString, out int level))
                    {
                        Level = level; // Assign to the int property
                    }
                    
                    string ScoreString = reader["Score"].ToString();
                    int Score = 0;
                    if (int.TryParse(ScoreString, out int score))
                    {
                        Score = score;
                    }

                    DateTime DT = DateTime.Parse(Date);
                    PomList.Add(new ListOfRecords(ID, DT, Level, Score));
                }
                return PomList;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred: {ex.Message}");
            }
        }

        private List<Point> LoadGameRecord_StartingPositions(int ID)
        {
            try
            {
                //RC.StartingFoodPositions = new List<Point>();
                List<Point> StartingFoodPositions = new List<Point>(); // Initialize the list to avoid null reference

                // Updated to use Microsoft.Data.SqlClient.SqlConnection  
                using var connection = new SqlConnection(connectionString);
                connection.Open();

                string query = "SELECT Settings, FO.PosX, FO.PosY " +
                    "FROM GameNumbers " +
                    "INNER JOIN FoodSettings FS ON GameNumbers.GameNumbers_ID = FS.GameID " +
                    "INNER JOIN Food FO ON FS.FoodID = FO.Food_ID " +
                    "WHERE Gamenumbers.GameNumbers_ID = @GameID;";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@GameID", ID);
                using var reader = command.ExecuteReader();

                bool first = true;

                while (reader.Read())
                {
                    if (first)
                    {
                        RC.Settings = reader["Settings"].ToString();
                        first = false;
                    }
                    int posX = Convert.ToInt32(reader["PosX"]);
                    int posY = Convert.ToInt32(reader["PosY"]);


                    StartingFoodPositions.Add(new Point(posX, posY));
                }
                return StartingFoodPositions;
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
                throw new Exception($"SQL error: {ex.Message}", ex);
            }     
        }

        private List<TurnRecord> LoadGameRecord_SnakeMoves(int ID)
        {
            try
            {
                List<TurnRecord> Turns = new List<TurnRecord>();
                using var connection = new SqlConnection(connectionString);
                connection.Open();

                string query = "SELECT MoveNumber, Direction, PosX, PosY FROM GameNumbers " +
                    "INNER JOIN SnakeMoves SM ON GameNumbers.GameNumbers_ID = SM.GameID " +
                    "LEFT JOIN Food FoodFromMoves ON SM.FoodID = FoodFromMoves.Food_ID " +
                    "WHERE GameNumbers_ID = @GameID " +
                    "ORDER BY MoveNumber";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@GameID", ID);
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    TurnRecord turnRecord = new TurnRecord();
                    int Movenumber = Convert.ToInt32(reader["MoveNumber"]);
                    string Direction = reader["Direction"].ToString();

                    //Debug.WriteLine(reader["FoodFromMoves.PosX"]);
                    if (reader["PosX"] != DBNull.Value && reader["PosY"] != DBNull.Value)
                    {
                        int posX = Convert.ToInt32(reader["PosX"]);
                        int posY = Convert.ToInt32(reader["PosY"]);
                        turnRecord.GeneratedFoodPosition = new Point(posX, posY);
                    }
                    else
                    {
                        turnRecord.GeneratedFoodPosition = null;
                    }

                    turnRecord.TurnNumber = Movenumber;
                    turnRecord.MoveDirection = Direction;


                    Turns.Add(turnRecord);
                }
                return Turns;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message} \n {ex.Data}");
                throw new Exception($"SQL error: {ex.Message}", ex);
            }
        }

        public Record LoadGameRecord(int ID)
        {
            List<Point>? StartFoPos;
            List<TurnRecord>? Turns;

            StartFoPos = LoadGameRecord_StartingPositions(ID);
            if(StartFoPos == null)
            {
                throw new Exception("Failed to load starting food positions.");
            }
            RC.StartingFoodPositions = StartFoPos;
            Turns = LoadGameRecord_SnakeMoves(ID);
            if (Turns == null)
            {
                throw new Exception("Failed to load turns.");
            }
            RC.Turns = Turns;
            Record record = new Record();
            record.Settings = RC.Settings;
            record.StartingFoodPositions = StartFoPos;
            record.Turns = Turns;
            
            return record;            
        }

        private int CalculateReachedLevel(Settings S)
        {
            int FoodEaten = 0;
            for (int i = 0; i < RC.Turns.Count; i++)
            {
                if (RC.Turns[i].GeneratedFoodPosition.HasValue)
                    FoodEaten++;
            }
            return FoodEaten / S.LevelIncreaseInterval;
        }

        public void SaveGameRecord(Settings S, Record record)
        {
            int LastGameNumbersID = 0; // Initialize LastGameNumbersID to 0
            RC = record;
            int Level = CalculateReachedLevel(S);

            try
            {
                // Using Microsoft.Data.SqlClient.SqlConnection
                using var connection = new SqlConnection(connectionString);
                connection.Open();

                string query = "INSERT INTO GameNumbers (Date, Settings, Level, Score) VALUES (@TimeNow, @JsonSettings, @Level, @Score); SELECT SCOPE_IDENTITY();";
                using var command = new SqlCommand(query, connection);

                DateTime dateTime = DateTime.Now;
                string CurrentTime = dateTime.ToString("yyyy-MM-dd HH:mm:ss");

                // Add parameter to prevent SQL injection
                command.Parameters.AddWithValue("@TimeNow", CurrentTime);
                command.Parameters.AddWithValue("@JsonSettings", S.ToString());
                command.Parameters.AddWithValue("@Level", Level);
                command.Parameters.AddWithValue("@Score", S.Score);

                //int rowsAffected = command.ExecuteNonQuery();
                System.Object result = command.ExecuteScalar();
                LastGameNumbersID = Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }

            //Insert starting Settings and Food Position
            for (int i = 0; i < RC.StartingFoodPositions.Count; i++)
            {
                int FoodID = InsertFoodIntoDB(RC.StartingFoodPositions[i]);
                InsertFoodSettingsIntoDB(LastGameNumbersID, FoodID);
            }

            //Insert moves and food added in turns
            for (int i = 0; i < RC.Turns.Count; i++)
            {
                if (RC.Turns[i].GeneratedFoodPosition.HasValue)
                {
                    Point FoodPos = (Point)RC.Turns[i].GeneratedFoodPosition;
                    int CurrentFoodID = InsertFoodIntoDB(FoodPos);
                    InsertSnakeMoveIntoDB(LastGameNumbersID, RC.Turns[i].MoveDirection, RC.Turns[i].TurnNumber, CurrentFoodID);
                }
                else
                {
                    InsertSnakeMoveIntoDB(LastGameNumbersID, RC.Turns[i].MoveDirection, RC.Turns[i].TurnNumber, 0);
                }

            }

        }

        private int InsertFoodIntoDB(Point FoodPosition)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();

                string query = "INSERT INTO Food (PosX, PosY) VALUES (@PX, @PY); SELECT SCOPE_IDENTITY();";
                using var command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@PX", FoodPosition.X);
                command.Parameters.AddWithValue("@PY", FoodPosition.Y);

                object result = command.ExecuteScalar();
                int LastFoodID = Convert.ToInt32(result);
                
                return LastFoodID;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred in method InsertFoodIntoDB: {ex.Message}");
                throw new Exception("Error inserting food into database", ex);
            }
        }

        /// <summary>
        /// Inserts a snake move into the database.
        /// </summary>
        /// <param name="GameID">
        /// The unique identifier for the game. This is the ID of the game record in the database.
        /// </param>
        /// <param name="MoveDirection">
        /// The direction of the snake's move. This is typically a string representing directions like "Up", "Down", "Left", or "Right".
        /// </param>
        /// <param name="MoveNumber">
        /// The turn number of the move. This represents the sequence of the move in the game (e.g., 1 for the first move, 2 for the second move, etc.).
        /// </param>
        /// <param name="FoodID">
        /// The unique identifier for the food generated during the move. If no food was generated, this value can be 0 or null.
        /// </param>
        /// <returns>
        /// The unique identifier of the inserted snake move record in the database.
        /// </returns>
        private int InsertSnakeMoveIntoDB(int GameID, string MoveDirection, int MoveNumber, int FoodID)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();

                // Replace "YourTable" and "ColumnName" with your actual table and column names
                string query = "INSERT INTO SnakeMoves (GameID, Direction ,MoveNumber, FoodID) VALUES " +
                    "(@GameID, @Direction, @MoveNumber, @FoodID); SELECT SCOPE_IDENTITY();";
                using var command = new SqlCommand(query, connection);

                // Add parameter to prevent SQL injection
                command.Parameters.AddWithValue("@GameID", GameID);
                command.Parameters.AddWithValue("@Direction", MoveDirection);
                command.Parameters.AddWithValue("@MoveNumber", MoveNumber);
                if (FoodID > 0)
                    command.Parameters.AddWithValue("@FoodID", FoodID);
                else
                    command.Parameters.AddWithValue("@FoodID", DBNull.Value);

                object result = command.ExecuteScalar();
                int LastFoodSettingsID = Convert.ToInt32(result);
                //MessageBox.Show($"New record inserted into SnakeMoves with ID: {LastFoodSettingsID}");

                return LastFoodSettingsID;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred in method InsertFoodSettingsIntoDB: {ex.Message}");
                throw new Exception("Error inserting food into database", ex);
            }
        }

        private int InsertFoodSettingsIntoDB(int GameID, int FoodID)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();

                // Replace "YourTable" and "ColumnName" with your actual table and column names
                string query = "INSERT INTO FoodSettings (GameID, FoodID) VALUES (@GameID, @FoodID); SELECT SCOPE_IDENTITY();";
                using var command = new SqlCommand(query, connection);

                // Add parameter to prevent SQL injection
                command.Parameters.AddWithValue("@GameID", GameID);
                command.Parameters.AddWithValue("@FoodID", FoodID);

                // Execute the query and retrieve the last inserted ID
                object result = command.ExecuteScalar();
                int LastFoodSettingsID = Convert.ToInt32(result);
          
                return LastFoodSettingsID;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred in method InsertFoodSettingsIntoDB: {ex.Message}");
                throw new Exception("Error inserting food into database", ex);
            }
        }

        public Settings GetJsonSettings()
        {
            Settings deserializedSettings = JsonSerializer.Deserialize<Settings>(RC.Settings);
            if (deserializedSettings != null)
            {
                return deserializedSettings;
            }
            else
            {
                throw new Exception("Failed to deserialize settings.");
            }
        }
    }
}
