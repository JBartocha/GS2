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
    public struct Turn
    {
        public int TurnNumber = 0;
        public string MoveDirection;
        public Point? GeneratedFoodPosition;
        
        public Turn(int turnNumber, string moveDirection)
        {
            TurnNumber = turnNumber;
            MoveDirection = moveDirection;
            GeneratedFoodPosition = null;
        }
        public Turn(int turnNumber, string moveDirection, Point? generatedFoodPosition)
        {
            TurnNumber = turnNumber;
            MoveDirection = moveDirection;
            GeneratedFoodPosition = generatedFoodPosition;
        }
    }

    interface IGameRecord
    {
        public void AddGeneratedFoodAtStart(Point foodPosition);
        public void AddSnakeMove(string moveDirection, Point generatedFoodPosition);
        public void SaveGameRecord();
        public void LoadGameRecord();
        public void SetJsonSettingsFile(string jsonSettingsFile);
    }

    internal class GameRecord : IGameRecord
    {
        

        private int CurrentTurnNumber = 0; //•A turn is a single move made by the snake in the game.
        private List<Point> GeneratedFoodsAtStart = new List<Point>();
        private List<Turn> SnakeMoves = new List<Turn>();
        private string? JsonSettingsfile;

        private static string connectionString = "Data Source = (LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Janba\\source\\repos\\GS2\\GS2\\SnakeDB.mdf;Integrated Security = True";

        public void AddGeneratedFoodAtStart(Point FoodPosition)
        {
            GeneratedFoodsAtStart.Add(FoodPosition);
        }

        public void AddSnakeMove(string moveDirection)
        {
            SnakeMoves.Add(new Turn(++CurrentTurnNumber, moveDirection));
        }

        public void AddSnakeMove(string moveDirection, Point generatedFoodPosition)
        {
            SnakeMoves.Add(new Turn(CurrentTurnNumber, moveDirection, generatedFoodPosition));
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

        public void SaveGameRecord()
        {
            try
            {
                string columnValue = "Test"; // Example value to insert

                // Using Microsoft.Data.SqlClient.SqlConnection
                using var connection = new SqlConnection(connectionString);
                connection.Open();

                // Replace "YourTable" and "ColumnName" with your actual table and column names
                string query = "INSERT INTO Test (Sloupec1, Cas) VALUES (@ColumnValue, @cas); SELECT SCOPE_IDENTITY();";
                using var command = new SqlCommand(query, connection);


                DateTime dateTime = DateTime.Now;
                string sloupecCas = dateTime.ToString("yyyy-MM-dd HH:mm:ss");

                // Add parameter to prevent SQL injection
                command.Parameters.AddWithValue("@ColumnValue", columnValue);
                //command.Parameters.AddWithValue("@cas", dateTime);
                command.Parameters.AddWithValue("@cas", sloupecCas);


                int rowsAffected = command.ExecuteNonQuery();
                System.Object result = command.ExecuteScalar();
                int newId = Convert.ToInt32(result);
                MessageBox.Show($"New record inserted with ID: {newId}\n" +
                    $"{rowsAffected} row(s) inserted successfully.");

            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }



            throw new NotImplementedException();
        }

        public void LoadGameRecord()
        {
            try
            {
                // Updated to use Microsoft.Data.SqlClient.SqlConnection  
                using var connection = new SqlConnection(connectionString);
                connection.Open();

                string query = "SELECT * FROM GameNumbers"; // Replace with your query  
                using var command = new SqlCommand(query, connection);
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string data = reader["ColumnName"].ToString(); // Replace "ColumnName" with your column name  
                    MessageBox.Show(data);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }


            throw new NotImplementedException();
        }

        public void SetJsonSettingsFile(string jsonSettingsFile)
        {
            JsonSettingsfile = jsonSettingsFile;
        }


    }
}
