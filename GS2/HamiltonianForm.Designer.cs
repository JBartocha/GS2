namespace GS2
{
    partial class HamiltonianForm
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
            SuspendLayout();
            // 
            // Panel_Main
            // 
            Panel_Main.BackColor = SystemColors.ControlLight;
            Panel_Main.Location = new Point(12, 12);
            Panel_Main.Name = "Panel_Main";
            Panel_Main.Size = new Size(1166, 743);
            Panel_Main.TabIndex = 0;
            // 
            // HamiltonianForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1190, 767);
            Controls.Add(Panel_Main);
            Name = "HamiltonianForm";
            Text = "Hamiltonian grid";
            ResumeLayout(false);
        }

        #endregion

        private Panel Panel_Main;
    }
}