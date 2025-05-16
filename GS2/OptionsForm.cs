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
using Microsoft.IdentityModel.Tokens;
using System.Configuration;

namespace GS2
{
    public partial class OptionsForm : Form
    {
        private Settings _SS;
        private Point _OriginalSize;
        public OptionsForm()
        {

            InitializeComponent();

            for (int i = 1; i < 5; i++)
            {
                ListBoxFoodInterval.Items.Add(i.ToString());
                ListBoxSpeedPercent.Items.Add((i * 5).ToString());
            }
            for (int i = 1; i < 16; i++)
            {
                ListBoxFoodCount.Items.Add(i.ToString());
            }

            LoadJsonSettings();

            TextBoxRows.Text = TrackBarRows.Value.ToString();
            TextBoxColumns.Text = TrackBarColumns.Value.ToString();
            TextBoxCellSize.Text = TrackBarCellSize.Value.ToString();

            _OriginalSize = new Point(TrackBarRows.Value, TrackBarColumns.Value);
        }

        private void LoadJsonSettings()
        {
            System.IO.FileSystemInfo fileInfo = new System.IO.FileInfo(Settings.JsonSaveFileName);
            if (fileInfo.Exists)
            {
                string json = File.ReadAllText(Settings.JsonSaveFileName);
                _SS = JsonSerializer.Deserialize<Settings>(json);
                if (_SS == null)
                    throw new Exception("Failed to deserialize settings.");
            }
            else
            {
                _SS = new Settings();
            }


            string FoodCount = _SS.FoodCount.ToString();
            ListBoxFoodCount.SelectedIndex = ListBoxFoodCount.Items.IndexOf(FoodCount);
            string FoodInterval = _SS.LevelIncreaseInterval.ToString();
            ListBoxFoodInterval.SelectedIndex = ListBoxFoodInterval.Items.IndexOf(FoodInterval);
            string SpeedPercent = (Convert.ToInt32(_SS.DifficultyIncrease * 100)).ToString();
            ListBoxSpeedPercent.SelectedIndex = ListBoxSpeedPercent.Items.IndexOf(SpeedPercent);

            TextBoxInitialSpeed.SelectedText = _SS.TickInMilliseconds.ToString();
            TextBoxInitialSpeed.Text = _SS.TickInMilliseconds.ToString();
            TrackBarRows.Value = _SS.Rows;
            TrackBarColumns.Value = _SS.Columns;
            TrackBarCellSize.Value = _SS.CellSize;
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

            _SS.CellSize = TrackBarCellSize.Value;
            _SS.FoodCount = int.Parse(ListBoxFoodCount.SelectedItem.ToString());
            _SS.DifficultyIncrease = int.Parse(ListBoxSpeedPercent.SelectedItem.ToString()) / 100f;
            _SS.LevelIncreaseInterval = int.Parse(ListBoxFoodInterval.SelectedItem.ToString());
            _SS.TickInMilliseconds = int.Parse(TextBoxInitialSpeed.Text);
            _SS.Rows = TrackBarRows.Value;
            _SS.Columns = TrackBarColumns.Value;
            _SS.CellSize = TrackBarCellSize.Value;

            if(_OriginalSize.X != TrackBarRows.Value || _OriginalSize.Y != TrackBarColumns.Value)
            {
                _SS.WallPositions.Clear();
            }

            string json = JsonSerializer.Serialize(_SS);
            File.WriteAllText(Settings.JsonSaveFileName, json);
            this.Close();
        }

        private void CheckBox_MouseControl_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckBox_MouseControl.Checked == true) _SS.UseMousePositionToMove = true;
            if (CheckBox_MouseControl.Checked == false)
            {
                _SS.UseMousePositionToMove = false;
                if (CheckBox_KeyboardControl.Checked == false)
                {
                    CheckBox_KeyboardControl.Checked = true;
                    _SS.UseMousePositionToMove = true;
                }
            }

        }

        private void CheckBox_KeyboardControl_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckBox_KeyboardControl.Checked == true) _SS.UseMousePositionToMove = true;
            if (CheckBox_KeyboardControl.Checked == false)
            {
                _SS.UseMousePositionToMove = false;
                if (CheckBox_MouseControl.Checked == false)
                {
                    CheckBox_MouseControl.Checked = true;
                    _SS.UseMousePositionToMove = true;
                }
            }
        }


        private void Button_Walls_Option_Click(object sender, EventArgs e)
        {
            this.Hide();
            int rows = TrackBarRows.Value;
            int columns = TrackBarColumns.Value;
            int cellSize = TrackBarCellSize.Value;

            Point[] forbiddenPoints = new Point[]
            {
                new Point(_SS.SnakeStartingHeadPosition.X, _SS.SnakeStartingHeadPosition.Y),
                new Point(_SS.SnakeStartingHeadPosition.X+1, _SS.SnakeStartingHeadPosition.Y),
            };

            WallOptionsForm optionsForm = new WallOptionsForm(rows, columns, cellSize,
                forbiddenPoints, _SS.WallPositions);
            optionsForm.ShowDialog();
            

            this.Show();
        }
    }
}

