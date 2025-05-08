namespace GS2
{
    partial class Main_Form
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            Panel_Main = new Panel();
            Button_Pause = new Button();
            Label_Movement_Direction = new Label();
            Label_Mouse_Position = new Label();
            Label_Food_Eaten = new Label();
            Label_Moves = new Label();
            Label_Timer = new Label();
            Restart = new Button();
            Label_Speed = new Label();
            Label_Level = new Label();
            timer1 = new System.Windows.Forms.Timer(components);
            Button_Options = new Button();
            Button_Load_Record = new Button();
            Panel_Right = new Panel();
            Label_Score = new Label();
            Panel_Right.SuspendLayout();
            SuspendLayout();
            // 
            // Panel_Main
            // 
            Panel_Main.BackColor = SystemColors.ActiveBorder;
            Panel_Main.Location = new Point(12, 12);
            Panel_Main.Name = "Panel_Main";
            Panel_Main.Size = new Size(441, 443);
            Panel_Main.TabIndex = 0;
            Panel_Main.Paint += Panel_Main_Paint;
            Panel_Main.MouseMove += PanelMain_MouseMove;
            // 
            // Button_Pause
            // 
            Button_Pause.Font = new Font("Segoe UI", 20F);
            Button_Pause.Location = new Point(4, 380);
            Button_Pause.Name = "Button_Pause";
            Button_Pause.Size = new Size(160, 50);
            Button_Pause.TabIndex = 1;
            Button_Pause.Text = "Start";
            Button_Pause.UseVisualStyleBackColor = true;
            Button_Pause.Click += Button_Pause_Click;
            // 
            // Label_Movement_Direction
            // 
            Label_Movement_Direction.AutoSize = true;
            Label_Movement_Direction.Font = new Font("Segoe UI", 12F);
            Label_Movement_Direction.Location = new Point(4, 0);
            Label_Movement_Direction.Name = "Label_Movement_Direction";
            Label_Movement_Direction.Size = new Size(47, 21);
            Label_Movement_Direction.TabIndex = 2;
            Label_Movement_Direction.Text = "Right";
            // 
            // Label_Mouse_Position
            // 
            Label_Mouse_Position.Font = new Font("Segoe UI", 12F);
            Label_Mouse_Position.Location = new Point(4, 21);
            Label_Mouse_Position.Name = "Label_Mouse_Position";
            Label_Mouse_Position.Size = new Size(100, 23);
            Label_Mouse_Position.TabIndex = 3;
            Label_Mouse_Position.Text = "Mouse position: ";
            // 
            // Label_Food_Eaten
            // 
            Label_Food_Eaten.AutoSize = true;
            Label_Food_Eaten.Font = new Font("Segoe UI", 12F);
            Label_Food_Eaten.Location = new Point(4, 44);
            Label_Food_Eaten.Name = "Label_Food_Eaten";
            Label_Food_Eaten.Size = new Size(110, 21);
            Label_Food_Eaten.TabIndex = 4;
            Label_Food_Eaten.Text = "Foods Eaten: 0";
            // 
            // Label_Moves
            // 
            Label_Moves.AutoSize = true;
            Label_Moves.Font = new Font("Segoe UI", 12F);
            Label_Moves.Location = new Point(4, 65);
            Label_Moves.Name = "Label_Moves";
            Label_Moves.Size = new Size(63, 21);
            Label_Moves.TabIndex = 5;
            Label_Moves.Text = "Moves: ";
            // 
            // Label_Timer
            // 
            Label_Timer.AutoSize = true;
            Label_Timer.Font = new Font("Segoe UI", 12F);
            Label_Timer.Location = new Point(4, 188);
            Label_Timer.Name = "Label_Timer";
            Label_Timer.Size = new Size(115, 21);
            Label_Timer.TabIndex = 6;
            Label_Timer.Text = "Time : 00:00:00";
            // 
            // Restart
            // 
            Restart.Font = new Font("Segoe UI", 20F);
            Restart.Location = new Point(3, 324);
            Restart.Name = "Restart";
            Restart.Size = new Size(160, 50);
            Restart.TabIndex = 7;
            Restart.Text = "Restart";
            Restart.UseVisualStyleBackColor = true;
            Restart.Click += Restart_Click;
            // 
            // Label_Speed
            // 
            Label_Speed.AutoSize = true;
            Label_Speed.Font = new Font("Segoe UI", 12F);
            Label_Speed.Location = new Point(4, 86);
            Label_Speed.Name = "Label_Speed";
            Label_Speed.Size = new Size(91, 21);
            Label_Speed.TabIndex = 8;
            Label_Speed.Text = "Speed (ms):";
            // 
            // Label_Level
            // 
            Label_Level.AutoSize = true;
            Label_Level.Font = new Font("Segoe UI", 18F);
            Label_Level.Location = new Point(4, 107);
            Label_Level.Name = "Label_Level";
            Label_Level.Size = new Size(80, 32);
            Label_Level.TabIndex = 9;
            Label_Level.Text = "LEVEL:";
            // 
            // Button_Options
            // 
            Button_Options.Font = new Font("Segoe UI", 18F);
            Button_Options.Location = new Point(3, 268);
            Button_Options.Name = "Button_Options";
            Button_Options.Size = new Size(160, 50);
            Button_Options.TabIndex = 10;
            Button_Options.Text = "Options";
            Button_Options.UseVisualStyleBackColor = true;
            Button_Options.Click += Button_Options_Click;
            // 
            // Button_Load_Record
            // 
            Button_Load_Record.Font = new Font("Segoe UI", 14F);
            Button_Load_Record.Location = new Point(3, 212);
            Button_Load_Record.Name = "Button_Load_Record";
            Button_Load_Record.Size = new Size(160, 50);
            Button_Load_Record.TabIndex = 11;
            Button_Load_Record.Text = "Load Replay";
            Button_Load_Record.UseVisualStyleBackColor = true;
            Button_Load_Record.Click += Button_Load_Record_Click;
            // 
            // Panel_Right
            // 
            Panel_Right.Controls.Add(Label_Score);
            Panel_Right.Controls.Add(Button_Load_Record);
            Panel_Right.Controls.Add(Button_Pause);
            Panel_Right.Controls.Add(Button_Options);
            Panel_Right.Controls.Add(Label_Movement_Direction);
            Panel_Right.Controls.Add(Label_Level);
            Panel_Right.Controls.Add(Label_Mouse_Position);
            Panel_Right.Controls.Add(Label_Speed);
            Panel_Right.Controls.Add(Label_Food_Eaten);
            Panel_Right.Controls.Add(Restart);
            Panel_Right.Controls.Add(Label_Moves);
            Panel_Right.Controls.Add(Label_Timer);
            Panel_Right.Location = new Point(482, 12);
            Panel_Right.Name = "Panel_Right";
            Panel_Right.Size = new Size(175, 443);
            Panel_Right.TabIndex = 12;
            // 
            // Label_Score
            // 
            Label_Score.AutoSize = true;
            Label_Score.Font = new Font("Segoe UI", 12F);
            Label_Score.Location = new Point(4, 139);
            Label_Score.Name = "Label_Score";
            Label_Score.Size = new Size(52, 21);
            Label_Score.TabIndex = 12;
            Label_Score.Text = "Score:";
            // 
            // Main_Form
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(669, 467);
            Controls.Add(Panel_Right);
            Controls.Add(Panel_Main);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            KeyPreview = true;
            Name = "Main_Form";
            Text = "Main";
            KeyPress += Main_Form_KeyPress;
            Panel_Right.ResumeLayout(false);
            Panel_Right.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel Panel_Main;
        private Button Button_Pause;
        private Label Label_Movement_Direction;
        private Label Label_Mouse_Position;
        private Label Label_Food_Eaten;
        private Label Label_Moves;
        private Label Label_Timer;
        private Button Restart;
        private Label Label_Speed;
        private Label Label_Level;
        private System.Windows.Forms.Timer timer1;
        private Button Button_Options;
        private Button Button_Load_Record;
        private Panel Panel_Right;
        private Label Label_Score;
    }
}
