using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using static System.ComponentModel.Design.ObjectSelectorEditor;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        public TurnRecord(int turnNumber, string moveDirection, Point? generatedFoodPosition)
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
    }

    public struct ListOfRecords
    {

        public string? ID;
        public DateTime Date;

        public ListOfRecords(string? iD, DateTime dT) : this()
        {
            this.ID = iD;
            this.Date = dT;
        }
    }

    interface IGameRecord
    {
        public void AddGeneratedFoodAtStart(Point foodPosition);
        public void AddSnakeMove(string moveDirection, Point generatedFoodPosition);
        public string GetMoveDirectionOnTurn(int turnNumber);
        public Point? GetGeneratedFoodPosition(int turnNumber);
        public List<Point> GetGeneratedFoodAtStart();
        public void SaveGameRecord(SnakeGameSettings S);
        public void LoadGameRecord(int ID);
        public void SetJsonSettingsFile(string jsonSettingsFile);
        public SnakeGameSettings GetJsonSettings();

    }

    public class GameRecord : IGameRecord
    {
        private int CurrentTurnNumber = 0; //•A turn is a single move made by the snake in the game.
        private Record RC = new Record();

        private const string connectionString = "Data Source = (LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Janba\\source\\repos\\GS2\\GS2\\SnakeDB.mdf;Integrated Security = True";

        public List<ListOfRecords> ListAllRecords()
        {
            try // TODO cele spatne ... udelat o znovu
            {
                // Updated to use Microsoft.Data.SqlClient.SqlConnection  
                using var connection = new SqlConnection(connectionString);
                connection.Open();

                List<ListOfRecords> PomList = new List<ListOfRecords>();

                string query = "SELECT GameNumbers_ID, Date FROM GameNumbers"; // Replace with your query  
                using var command = new SqlCommand(query, connection);
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string ID = reader["GameNumbers_ID"].ToString();
                    string Date = reader["Date"].ToString();
                    DateTime DT = DateTime.Parse(Date);
                    PomList.Add(new ListOfRecords(ID, DT));
                }
                return PomList;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred: {ex.Message}");
            }
        }

        public void AddGeneratedFoodAtStart(Point FoodPosition)
        {
            RC.StartingFoodPositions.Add(FoodPosition);
        }

        public void AddSnakeMove(string moveDirection)
        {
            RC.Turns.Add(new TurnRecord(++CurrentTurnNumber, moveDirection));
            TurnRecord Last = RC.Turns.Last();
            Last.GeneratedFoodPosition = null;
            RC.Turns[RC.Turns.Count - 1] = Last;
        }

        public void AddSnakeMove(string moveDirection, Point generatedFoodPosition)
        {
            RC.Turns.Add(new TurnRecord(++CurrentTurnNumber, moveDirection, generatedFoodPosition));
        }

        public void ListValues()
        {
            Debug.WriteLine("Generated Foods at Start:");
            foreach (var food in RC.StartingFoodPositions)
            {
                Debug.WriteLine(food);
            }
            Debug.WriteLine("Snake Moves:");
            foreach (var move in RC.Turns)
            {
                Debug.WriteLine(move.MoveDirection);
                if (move.GeneratedFoodPosition == null)
                    Debug.WriteLine("null");
                else
                    Debug.WriteLine(move.GeneratedFoodPosition);
            }
        }


        private void LoadGameRecord_StartingPositions(int ID)
        {
            try
            {
                RC.StartingFoodPositions = new List<Point>();

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


                    RC.StartingFoodPositions.Add(new Point(posX, posY));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void LoadGameRecord_SnakeMoves(int ID)
        {
            try
            {
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


                    RC.Turns.Add(turnRecord);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message} \n {ex.Data}");
            }
        }

        public void LoadGameRecord(int ID)
        {
            LoadGameRecord_StartingPositions(ID);
            LoadGameRecord_SnakeMoves(ID); 

            ListValues();
        }

        public void SaveGameRecord(SnakeGameSettings S)
        {
            int LastGameNumbersID = 0; // Initialize LastGameNumbersID to 0

            try
            {
                // Using Microsoft.Data.SqlClient.SqlConnection
                using var connection = new SqlConnection(connectionString);
                connection.Open();

                string query = "INSERT INTO GameNumbers (Date, Settings, Level) VALUES (@TimeNow, @JsonSettings, @Level); SELECT SCOPE_IDENTITY();";
                using var command = new SqlCommand(query, connection);

                DateTime dateTime = DateTime.Now;
                string CurrentTime = dateTime.ToString("yyyy-MM-dd HH:mm:ss");

                // Add parameter to prevent SQL injection
                command.Parameters.AddWithValue("@TimeNow", CurrentTime);
                command.Parameters.AddWithValue("@JsonSettings", RC.Settings);
                command.Parameters.AddWithValue("@Level", S.Level);

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



        public void SetJsonSettingsFile(string jsonSettingsFile)
        {
            RC.Settings = jsonSettingsFile;
        }

        private int InsertFoodIntoDB(Point FoodPosition)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();

                // Replace "YourTable" and "ColumnName" with your actual table and column names
                string query = "INSERT INTO Food (PosX, PosY) VALUES (@PX, @PY); SELECT SCOPE_IDENTITY();";
                using var command = new SqlCommand(query, connection);

                // Add parameter to prevent SQL injection
                command.Parameters.AddWithValue("@PX", FoodPosition.X);
                command.Parameters.AddWithValue("@PY", FoodPosition.Y);

                // Execute the query and retrieve the last inserted ID
                object result = command.ExecuteScalar();
                int LastFoodID = Convert.ToInt32(result);
                //MessageBox.Show($"New record inserted into Food with ID: {LastFoodID}");

                return LastFoodID;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred in method InsertFoodIntoDB: {ex.Message}");
                // Replace the invalid SqlException instantiation with a generic Exception throw
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
                //MessageBox.Show($"New record inserted into FoodSettings with ID: {LastFoodSettingsID}");

                return LastFoodSettingsID;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred in method InsertFoodSettingsIntoDB: {ex.Message}");
                throw new Exception("Error inserting food into database", ex);
            }
        }
        
        public SnakeGameSettings GetJsonSettings()
        {
            if(RC.Settings == null)
            {
                throw new Exception("Settings not set in GameRecords.GetJsonSettings()");
            }
            return JsonSerializer.Deserialize<SnakeGameSettings>(RC.Settings);
        }

        public string GetMoveDirectionOnTurn(int turnNumber)
        {
            return RC.Turns[turnNumber - 1].MoveDirection;
        }

        public Point? GetGeneratedFoodPosition(int turnNumber)
        {
            Debug.WriteLine(RC.Turns[turnNumber - 1].GeneratedFoodPosition);
            return RC.Turns[turnNumber - 1].GeneratedFoodPosition;
        }

        public List<Point> GetGeneratedFoodAtStart()
        {
            return RC.StartingFoodPositions;
        }
    }
}
