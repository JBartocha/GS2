namespace GS2
{
    partial class OptionsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Button_EXIT = new Button();
            TrackBarRows = new TrackBar();
            TextBoxRows = new TextBox();
            label1 = new Label();
            label2 = new Label();
            TrackBarColumns = new TrackBar();
            TextBoxColumns = new TextBox();
            label3 = new Label();
            ListBoxFoodCount = new ListBox();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            ListBoxFoodInterval = new ListBox();
            TextBoxInitialSpeed = new TextBox();
            ListBoxSpeedPercent = new ListBox();
            ButtonSave = new Button();
            label7 = new Label();
            TextBoxCellSize = new TextBox();
            TrackBarCellSize = new TrackBar();
            ((System.ComponentModel.ISupportInitialize)TrackBarRows).BeginInit();
            ((System.ComponentModel.ISupportInitialize)TrackBarColumns).BeginInit();
            ((System.ComponentModel.ISupportInitialize)TrackBarCellSize).BeginInit();
            SuspendLayout();
            // 
            // Button_EXIT
            // 
            Button_EXIT.Location = new Point(321, 413);
            Button_EXIT.Name = "Button_EXIT";
            Button_EXIT.Size = new Size(119, 37);
            Button_EXIT.TabIndex = 0;
            Button_EXIT.Text = "EXIT";
            Button_EXIT.UseVisualStyleBackColor = true;
            Button_EXIT.Click += Button_EXIT_Click;
            // 
            // TrackBarRows
            // 
            TrackBarRows.Location = new Point(310, 308);
            TrackBarRows.Maximum = 15;
            TrackBarRows.Minimum = 7;
            TrackBarRows.Name = "TrackBarRows";
            TrackBarRows.Size = new Size(210, 45);
            TrackBarRows.TabIndex = 1;
            TrackBarRows.Value = 11;
            TrackBarRows.ValueChanged += TrackBarRows_ValueChanged;
            // 
            // TextBoxRows
            // 
            TextBoxRows.Font = new Font("Segoe UI", 18F);
            TextBoxRows.Location = new Point(576, 308);
            TextBoxRows.Name = "TextBoxRows";
            TextBoxRows.ReadOnly = true;
            TextBoxRows.Size = new Size(64, 39);
            TextBoxRows.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(12, 308);
            label1.Name = "label1";
            label1.Size = new Size(254, 21);
            label1.TabIndex = 3;
            label1.Text = "Velikost Herni Plochy - Počet řádků:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F);
            label2.Location = new Point(12, 359);
            label2.Name = "label2";
            label2.Size = new Size(268, 21);
            label2.TabIndex = 4;
            label2.Text = "Velikost Herni Plochy - Počet sloupců:";
            // 
            // TrackBarColumns
            // 
            TrackBarColumns.Location = new Point(310, 359);
            TrackBarColumns.Maximum = 15;
            TrackBarColumns.Minimum = 7;
            TrackBarColumns.Name = "TrackBarColumns";
            TrackBarColumns.Size = new Size(210, 45);
            TrackBarColumns.TabIndex = 5;
            TrackBarColumns.Value = 11;
            TrackBarColumns.ValueChanged += TrackBarColumns_ValueChanged;
            // 
            // TextBoxColumns
            // 
            TextBoxColumns.Font = new Font("Segoe UI", 18F);
            TextBoxColumns.Location = new Point(576, 353);
            TextBoxColumns.Name = "TextBoxColumns";
            TextBoxColumns.ReadOnly = true;
            TextBoxColumns.Size = new Size(64, 39);
            TextBoxColumns.TabIndex = 6;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F);
            label3.Location = new Point(351, 57);
            label3.Name = "label3";
            label3.Size = new Size(169, 21);
            label3.TabIndex = 7;
            label3.Text = "Počet potravy na ploše:";
            // 
            // ListBoxFoodCount
            // 
            ListBoxFoodCount.Font = new Font("Segoe UI", 18F);
            ListBoxFoodCount.FormattingEnabled = true;
            ListBoxFoodCount.Location = new Point(576, 51);
            ListBoxFoodCount.Name = "ListBoxFoodCount";
            ListBoxFoodCount.Size = new Size(64, 36);
            ListBoxFoodCount.TabIndex = 8;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 12F);
            label4.Location = new Point(219, 101);
            label4.Name = "label4";
            label4.Size = new Size(301, 21);
            label4.TabIndex = 9;
            label4.Text = "Po kolika jídlech se má zvysovat obtížnost:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 12F);
            label5.Location = new Point(292, 144);
            label5.Name = "label5";
            label5.Size = new Size(228, 21);
            label5.TabIndex = 10;
            label5.Text = "procentuální navýšení rychlosti:";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 12F);
            label6.Location = new Point(351, 189);
            label6.Name = "label6";
            label6.Size = new Size(172, 21);
            label6.TabIndex = 11;
            label6.Text = "Počáteční rychlost (ms):";
            // 
            // ListBoxFoodInterval
            // 
            ListBoxFoodInterval.Font = new Font("Segoe UI", 18F);
            ListBoxFoodInterval.FormattingEnabled = true;
            ListBoxFoodInterval.Location = new Point(576, 93);
            ListBoxFoodInterval.Name = "ListBoxFoodInterval";
            ListBoxFoodInterval.Size = new Size(64, 36);
            ListBoxFoodInterval.TabIndex = 14;
            // 
            // TextBoxInitialSpeed
            // 
            TextBoxInitialSpeed.Font = new Font("Segoe UI", 18F);
            TextBoxInitialSpeed.Location = new Point(576, 180);
            TextBoxInitialSpeed.Name = "TextBoxInitialSpeed";
            TextBoxInitialSpeed.Size = new Size(64, 39);
            TextBoxInitialSpeed.TabIndex = 15;
            TextBoxInitialSpeed.KeyPress += TextBoxInitialSpeed_KeyPress;
            // 
            // ListBoxSpeedPercent
            // 
            ListBoxSpeedPercent.Font = new Font("Segoe UI", 18F);
            ListBoxSpeedPercent.FormattingEnabled = true;
            ListBoxSpeedPercent.Location = new Point(576, 138);
            ListBoxSpeedPercent.Name = "ListBoxSpeedPercent";
            ListBoxSpeedPercent.Size = new Size(64, 36);
            ListBoxSpeedPercent.TabIndex = 16;
            // 
            // ButtonSave
            // 
            ButtonSave.Location = new Point(196, 413);
            ButtonSave.Name = "ButtonSave";
            ButtonSave.Size = new Size(119, 37);
            ButtonSave.TabIndex = 17;
            ButtonSave.Text = "SAVE";
            ButtonSave.UseVisualStyleBackColor = true;
            ButtonSave.Click += ButtonSave_Click;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 12F);
            label7.Location = new Point(118, 21);
            label7.Name = "label7";
            label7.Size = new Size(186, 21);
            label7.TabIndex = 20;
            label7.Text = "Velikost Buňky v pixelech:";
            // 
            // TextBoxCellSize
            // 
            TextBoxCellSize.Font = new Font("Segoe UI", 18F);
            TextBoxCellSize.Location = new Point(576, 9);
            TextBoxCellSize.Name = "TextBoxCellSize";
            TextBoxCellSize.ReadOnly = true;
            TextBoxCellSize.Size = new Size(64, 39);
            TextBoxCellSize.TabIndex = 19;
            // 
            // TrackBarCellSize
            // 
            TrackBarCellSize.Location = new Point(310, 9);
            TrackBarCellSize.Maximum = 60;
            TrackBarCellSize.Minimum = 15;
            TrackBarCellSize.Name = "TrackBarCellSize";
            TrackBarCellSize.Size = new Size(210, 45);
            TrackBarCellSize.TabIndex = 18;
            TrackBarCellSize.Value = 40;
            TrackBarCellSize.ValueChanged += TrackBarCellSize_ValueChanged;
            // 
            // OptionsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(652, 462);
            Controls.Add(label7);
            Controls.Add(TextBoxCellSize);
            Controls.Add(TrackBarCellSize);
            Controls.Add(ButtonSave);
            Controls.Add(ListBoxSpeedPercent);
            Controls.Add(TextBoxInitialSpeed);
            Controls.Add(ListBoxFoodInterval);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(ListBoxFoodCount);
            Controls.Add(label3);
            Controls.Add(TextBoxColumns);
            Controls.Add(TrackBarColumns);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(TextBoxRows);
            Controls.Add(TrackBarRows);
            Controls.Add(Button_EXIT);
            Name = "OptionsForm";
            Text = "OptionsForm";
            ((System.ComponentModel.ISupportInitialize)TrackBarRows).EndInit();
            ((System.ComponentModel.ISupportInitialize)TrackBarColumns).EndInit();
            ((System.ComponentModel.ISupportInitialize)TrackBarCellSize).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button Button_EXIT;
        private TrackBar TrackBarRows;
        private TextBox TextBoxRows;
        private Label label1;
        private Label label2;
        private TrackBar TrackBarColumns;
        private TextBox TextBoxColumns;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private ListBox ListBoxFoodInterval;
        private ListBox ListBoxFoodCount;
        private TextBox TextBoxInitialSpeed;
        private ListBox ListBoxSpeedPercent;
        private Button ButtonSave;
        private Label label7;
        private TextBox TextBoxCellSize;
        private TrackBar TrackBarCellSize;
    }
}