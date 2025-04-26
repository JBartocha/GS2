using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Text.Json;

namespace GS2
{
    public partial class OptionsForm : Form
    {
        private SnakeGameSettings SS;
        public OptionsForm()
        {

            InitializeComponent();


            for (int i = 1; i < 5; i++)
            {
                ListBoxFoodInterval.Items.Add(i.ToString());
                ListBoxFoodCount.Items.Add(i.ToString());
                ListBoxSpeedPercent.Items.Add((i * 5).ToString());
            }

            LoadJsonSettings();

            TextBoxRows.Text = TrackBarRows.Value.ToString();
            TextBoxColumns.Text = TrackBarColumns.Value.ToString();
            TextBoxCellSize.Text = TrackBarCellSize.Value.ToString();
            /*
            ListBoxFoodCount.SelectedIndex = 1;
            ListBoxFoodInterval.SelectedIndex = 1;
            ListBoxSpeedPercent.SelectedIndex = 1;
            */
        }

        private void LoadJsonSettings()
        {
            System.IO.FileSystemInfo fileInfo = new System.IO.FileInfo(SnakeGameSettings.JsonSaveFileName);
            if (fileInfo.Exists)
            {
                string json = File.ReadAllText(SnakeGameSettings.JsonSaveFileName);
                SS = JsonSerializer.Deserialize<SnakeGameSettings>(json);
                if (SS == null)
                    throw new Exception("Failed to deserialize settings.");
            }
            else
            {
                SS = new SnakeGameSettings();
            }
            string FoodCount = SS.FoodCount.ToString();
            ListBoxFoodCount.SelectedIndex = ListBoxFoodCount.Items.IndexOf(FoodCount);
            string FoodInterval = SS.LevelIncreaseInterval.ToString();
            ListBoxFoodInterval.SelectedIndex = ListBoxFoodInterval.Items.IndexOf(FoodInterval);
            string SpeedPercent = (SS.DifficultyIncrease * 100).ToString();
            ListBoxSpeedPercent.SelectedIndex = ListBoxSpeedPercent.Items.IndexOf(SpeedPercent);
            TrackBarCellSize.Value = SS.CellSize;
            TextBoxInitialSpeed.Text = SS.TickInMilliseconds.ToString();

        }

        private void Button_EXIT_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void TrackBarRows_ValueChanged(object sender, EventArgs e)
        {
            TextBoxRows.Text = TrackBarRows.Value.ToString();
        }

        private void TrackBarColumns_ValueChanged(object sender, EventArgs e)
        {
            TextBoxColumns.Text = TrackBarColumns.Value.ToString();
        }

        private void TrackBarCellSize_ValueChanged(object sender, EventArgs e)
        {
            TextBoxCellSize.Text = TrackBarCellSize.Value.ToString();
        }

        private void TextBoxInitialSpeed_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow only digits, control keys (e.g., backspace)
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Reject the input
            }
        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            if (ListBoxFoodCount.SelectedIndex == -1 || ListBoxFoodInterval.SelectedIndex == -1 || ListBoxSpeedPercent.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a food count.");
                return;
            }

            SS.CellSize = TrackBarCellSize.Value;
            SS.FoodCount = int.Parse(ListBoxFoodCount.SelectedItem.ToString());
            SS.DifficultyIncrease = int.Parse(ListBoxSpeedPercent.SelectedItem.ToString()) / 100f;
            SS.LevelIncreaseInterval = int.Parse(ListBoxFoodInterval.SelectedItem.ToString());
            SS.TickInMilliseconds = int.Parse(TextBoxInitialSpeed.Text);

            string json = JsonSerializer.Serialize(SS);
            File.WriteAllText(SnakeGameSettings.JsonSaveFileName, json);
        }

        private void CheckBox_MouseControl_CheckedChanged(object sender, EventArgs e)
        {
            if(CheckBox_MouseControl.Checked == true) SS.UseMousePositionToMove = true;
            if(CheckBox_MouseControl.Checked == false) SS.UseMousePositionToMove = false;
        }
        private void CheckBox_KeyboardControl_CheckedChanged(object sender, EventArgs e)
        {
            if(CheckBox_KeyboardControl.Checked == true) SS.UseMousePositionToMove = true;
            if(CheckBox_KeyboardControl.Checked == false) SS.UseMousePositionToMove = false;
        }
    }
}

