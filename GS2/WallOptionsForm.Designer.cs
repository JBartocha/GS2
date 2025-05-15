namespace GS2
{
    partial class WallOptionsForm
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
            Panel_Main = new Panel();
            Button_Reset = new Button();
            Button_CancelAndExit = new Button();
            Button_SaveAndExit = new Button();
            SuspendLayout();
            // 
            // Panel_Main
            // 
            Panel_Main.BackColor = SystemColors.ControlLight;
            Panel_Main.Location = new Point(12, 12);
            Panel_Main.Name = "Panel_Main";
            Panel_Main.Size = new Size(727, 487);
            Panel_Main.TabIndex = 0;
            Panel_Main.Paint += Panel_Main_Paint;
            Panel_Main.MouseDown += Panel_Main_MouseDown;
            Panel_Main.MouseMove += Panel_Main_MouseMove;
            Panel_Main.MouseUp += Panel_Main_MouseUp;
            // 
            // Button_Reset
            // 
            Button_Reset.Font = new Font("Segoe UI", 20F);
            Button_Reset.Location = new Point(344, 505);
            Button_Reset.Name = "Button_Reset";
            Button_Reset.Size = new Size(160, 50);
            Button_Reset.TabIndex = 8;
            Button_Reset.Text = "Reset";
            Button_Reset.UseVisualStyleBackColor = true;
            Button_Reset.Click += Button_Reset_Click;
            // 
            // Button_CancelAndExit
            // 
            Button_CancelAndExit.Font = new Font("Segoe UI", 12F);
            Button_CancelAndExit.Location = new Point(178, 505);
            Button_CancelAndExit.Name = "Button_CancelAndExit";
            Button_CancelAndExit.Size = new Size(160, 50);
            Button_CancelAndExit.TabIndex = 9;
            Button_CancelAndExit.Text = "Cancel and Exit";
            Button_CancelAndExit.UseVisualStyleBackColor = true;
            Button_CancelAndExit.Click += Buttton_CancelAndExit_Click;
            // 
            // Button_SaveAndExit
            // 
            Button_SaveAndExit.Font = new Font("Segoe UI", 12F);
            Button_SaveAndExit.Location = new Point(12, 505);
            Button_SaveAndExit.Name = "Button_SaveAndExit";
            Button_SaveAndExit.Size = new Size(160, 50);
            Button_SaveAndExit.TabIndex = 10;
            Button_SaveAndExit.Text = "Save and Exit";
            Button_SaveAndExit.UseVisualStyleBackColor = true;
            Button_SaveAndExit.Click += Button_SaveAndExit_Click;
            // 
            // WallOptionsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(751, 586);
            ControlBox = false;
            Controls.Add(Button_SaveAndExit);
            Controls.Add(Button_CancelAndExit);
            Controls.Add(Button_Reset);
            Controls.Add(Panel_Main);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Name = "WallOptionsForm";
            Text = "WallOptionsForm";
            ResumeLayout(false);
        }

        #endregion

        private Panel Panel_Main;
        private Button Button_Reset;
        private Button Button_CancelAndExit;
        private Button Button_SaveAndExit;
    }
}