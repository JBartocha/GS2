using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
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
        private string Settings;
        private List<Point> StartingFoodPositions;
        private List<TurnRecord> Turns;
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
        public void SaveGameRecord();
        public void LoadGameRecord(int ID);
        public void SetJsonSettingsFile(string jsonSettingsFile);

    }

    public class GameRecord : IGameRecord
    {
        private int CurrentTurnNumber = 0; //•A turn is a single move made by the snake in the game.
        private List<Point> GeneratedFoodsAtStart = new List<Point>();
        private List<TurnRecord> SnakeMoves = new List<TurnRecord>();
        private string? JsonSettingsfile;

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
            GeneratedFoodsAtStart.Add(FoodPosition);
        }

        public void AddSnakeMove(string moveDirection)
        {
            SnakeMoves.Add(new TurnRecord(++CurrentTurnNumber, moveDirection));
            TurnRecord Last = SnakeMoves.Last();
            Last.GeneratedFoodPosition = null;
            SnakeMoves[SnakeMoves.Count - 1] = Last;    
        }

        public void AddSnakeMove(string moveDirection, Point generatedFoodPosition)
        {
            SnakeMoves.Add(new TurnRecord(CurrentTurnNumber, moveDirection, generatedFoodPosition));
        }

        public void ListValues()
        {
            Debug.WriteLine("Generated Foods at Start:");
            foreach (var food in GeneratedFoodsAtStart)
            {
                Debug.WriteLine(food);
            }
            Debug.WriteLine("Snake Moves:");
            foreach (var move in SnakeMoves)
            {
                Debug.WriteLine(move.MoveDirection);
                if (move.GeneratedFoodPosition == null)
                    Debug.WriteLine("null");
                else
                    Debug.WriteLine(move.GeneratedFoodPosition);
            }
        }

        public void LoadGameRecord(int ID)
        {
            try // TODO cele spatne ... udelat o znovu
            {
                // Updated to use Microsoft.Data.SqlClient.SqlConnection  
                using var connection = new SqlConnection(connectionString);
                connection.Open();

                string query = "SELECT GameNumbers_ID, Date, Settings FROM GameNumbers"; // Replace with your query  
                using var command = new SqlCommand(query, connection);
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string data = reader["Date"].ToString(); // Replace "ColumnName" with your column name  
                    string data2 = reader["Settings"].ToString(); // Replace "ColumnName" with your column name  
                    //MessageBox.Show(data);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }


            throw new NotImplementedException();
        }

        public void SaveGameRecord()
        {
            int LastGameNumbersID = 0; // Initialize LastGameNumbersID to 0

            try
            {
                // Using Microsoft.Data.SqlClient.SqlConnection
                using var connection = new SqlConnection(connectionString);
                connection.Open();

                string query = "INSERT INTO GameNumbers (Date, Settings) VALUES (@TimeNow, @JsonSettings); SELECT SCOPE_IDENTITY();";
                using var command = new SqlCommand(query, connection);

                DateTime dateTime = DateTime.Now;
                string CurrentTime = dateTime.ToString("yyyy-MM-dd HH:mm:ss");

                // Add parameter to prevent SQL injection
                command.Parameters.AddWithValue("@TimeNow", CurrentTime);
                command.Parameters.AddWithValue("@JsonSettings", JsonSettingsfile);

                //int rowsAffected = command.ExecuteNonQuery();
                System.Object result = command.ExecuteScalar();
                LastGameNumbersID = Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }

            //TODO - Insert starting Settings and Food Position
            for (int i = 0; i < GeneratedFoodsAtStart.Count; i++)
            {
                int FoodID = InsertFoodIntoDB(GeneratedFoodsAtStart[i]);
                InsertFoodSettingsIntoDB(LastGameNumbersID, FoodID);
                //Debug.WriteLine($"Iterace {i}");
            }
            //TODO - Insert moves and food added in turns
            for (int i = 0; i < SnakeMoves.Count; i++)
            {
                if (SnakeMoves[i].GeneratedFoodPosition.HasValue)
                {
                    Point FoodPos = (Point)SnakeMoves[i].GeneratedFoodPosition;
                    int CurrentFoodID = InsertFoodIntoDB(FoodPos);
                    InsertSnakeMoveIntoDB(LastGameNumbersID, SnakeMoves[i].MoveDirection, SnakeMoves[i].TurnNumber, CurrentFoodID);
                }
                else
                {
                    InsertSnakeMoveIntoDB(LastGameNumbersID, SnakeMoves[i].MoveDirection, SnakeMoves[i].TurnNumber, 0);
                }

            }
        }



        public void SetJsonSettingsFile(string jsonSettingsFile)
        {
            JsonSettingsfile = jsonSettingsFile;
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
                MessageBox.Show($"New record inserted into SnakeMoves with ID: {LastFoodSettingsID}");

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

    }
}
