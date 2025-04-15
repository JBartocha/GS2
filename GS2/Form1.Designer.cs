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
            Panel_Main = new Panel();
            Button_Pause = new Button();
            Label_Movement_Direction = new Label();
            Label_Mouse_Position = new Label();
            Label_Food_Eaten = new Label();
            Label_Moves = new Label();
            Label_Timer = new Label();
            Restart = new Button();
            SuspendLayout();
            // 
            // Panel_Main
            // 
            Panel_Main.BackColor = SystemColors.ActiveBorder;
            Panel_Main.Location = new Point(12, 12);
            Panel_Main.Name = "Panel_Main";
            Panel_Main.Size = new Size(589, 522);
            Panel_Main.TabIndex = 0;
            Panel_Main.Paint += Panel_Main_Paint;
            Panel_Main.MouseMove += PanelMain_MouseMove;
            // 
            // Button_Pause
            // 
            Button_Pause.Font = new Font("Segoe UI", 28F);
            Button_Pause.Location = new Point(607, 470);
            Button_Pause.Name = "Button_Pause";
            Button_Pause.Size = new Size(184, 64);
            Button_Pause.TabIndex = 1;
            Button_Pause.Text = "Start";
            Button_Pause.UseVisualStyleBackColor = true;
            Button_Pause.Click += Button_Pause_Click;
            // 
            // Label_Movement_Direction
            // 
            Label_Movement_Direction.AutoSize = true;
            Label_Movement_Direction.Font = new Font("Segoe UI", 12F);
            Label_Movement_Direction.Location = new Point(607, 12);
            Label_Movement_Direction.Name = "Label_Movement_Direction";
            Label_Movement_Direction.Size = new Size(47, 21);
            Label_Movement_Direction.TabIndex = 2;
            Label_Movement_Direction.Text = "Right";
            // 
            // Label_Mouse_Position
            // 
            Label_Mouse_Position.Font = new Font("Segoe UI", 12F);
            Label_Mouse_Position.Location = new Point(607, 33);
            Label_Mouse_Position.Name = "Label_Mouse_Position";
            Label_Mouse_Position.Size = new Size(100, 23);
            Label_Mouse_Position.TabIndex = 3;
            Label_Mouse_Position.Text = "Mouse position: ";
            // 
            // Label_Food_Eaten
            // 
            Label_Food_Eaten.AutoSize = true;
            Label_Food_Eaten.Font = new Font("Segoe UI", 12F);
            Label_Food_Eaten.Location = new Point(607, 56);
            Label_Food_Eaten.Name = "Label_Food_Eaten";
            Label_Food_Eaten.Size = new Size(110, 21);
            Label_Food_Eaten.TabIndex = 4;
            Label_Food_Eaten.Text = "Foods Eaten: 0";
            // 
            // Label_Moves
            // 
            Label_Moves.AutoSize = true;
            Label_Moves.Font = new Font("Segoe UI", 12F);
            Label_Moves.Location = new Point(607, 77);
            Label_Moves.Name = "Label_Moves";
            Label_Moves.Size = new Size(63, 21);
            Label_Moves.TabIndex = 5;
            Label_Moves.Text = "Moves: ";
            // 
            // Label_Timer
            // 
            Label_Timer.AutoSize = true;
            Label_Timer.Font = new Font("Segoe UI", 12F);
            Label_Timer.Location = new Point(607, 98);
            Label_Timer.Name = "Label_Timer";
            Label_Timer.Size = new Size(115, 21);
            Label_Timer.TabIndex = 6;
            Label_Timer.Text = "Time : 00:00:00";
            // 
            // Restart
            // 
            Restart.Font = new Font("Segoe UI", 28F);
            Restart.Location = new Point(607, 402);
            Restart.Name = "Restart";
            Restart.Size = new Size(184, 64);
            Restart.TabIndex = 7;
            Restart.Text = "Restart";
            Restart.UseVisualStyleBackColor = true;
            Restart.Click += Restart_Click;
            // 
            // Main_Form
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(803, 546);
            Controls.Add(Restart);
            Controls.Add(Label_Timer);
            Controls.Add(Label_Moves);
            Controls.Add(Label_Food_Eaten);
            Controls.Add(Label_Mouse_Position);
            Controls.Add(Label_Movement_Direction);
            Controls.Add(Button_Pause);
            Controls.Add(Panel_Main);
            Name = "Main_Form";
            Text = "Main";
            ResumeLayout(false);
            PerformLayout();
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
    }
}
