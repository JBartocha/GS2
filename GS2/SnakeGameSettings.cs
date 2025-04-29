using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GS2
{
    public class SnakeGameSettings
    {
        public static string JsonSaveFileName { get; set; } = "Settings.json";
        public bool UseMousePositionToMove { get; set; } = true;
        public bool UseKeyboardToMove { get; set; } = true;
        public int FoodCount { get; set; } = 3;
        public int LevelIncreaseInterval { get; set; } = 2;
        public int TickInMilliseconds { get; set; } = 500;
        public float DifficultyIncrease { get; set; } = 0.1f;
        public int CellSize { get; set; } = 40;
        public int Rows { get; set; } = 11;
        public int Columns { get; set; } = 11;

        public override string ToString()
        {
            string jsonString = JsonSerializer.Serialize(this);
            return jsonString;
        }
    }
}
