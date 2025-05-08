namespace GS2
{
    partial class RecordForm
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
            ListBox_Records = new ListBox();
            Button_Save_And_Exit = new Button();
            Button_Cancel = new Button();
            SuspendLayout();
            // 
            // ListBox_Records
            // 
            ListBox_Records.Font = new Font("Segoe UI", 12F);
            ListBox_Records.FormattingEnabled = true;
            ListBox_Records.Location = new Point(2, 7);
            ListBox_Records.Name = "ListBox_Records";
            ListBox_Records.Size = new Size(304, 361);
            ListBox_Records.TabIndex = 0;
            // 
            // Button_Save_And_Exit
            // 
            Button_Save_And_Exit.Location = new Point(2, 377);
            Button_Save_And_Exit.Name = "Button_Save_And_Exit";
            Button_Save_And_Exit.Size = new Size(149, 38);
            Button_Save_And_Exit.TabIndex = 1;
            Button_Save_And_Exit.Text = "Select and Exit";
            Button_Save_And_Exit.UseVisualStyleBackColor = true;
            Button_Save_And_Exit.Click += Button_Save_And_Exit_Click;
            // 
            // Button_Cancel
            // 
            Button_Cancel.Location = new Point(157, 377);
            Button_Cancel.Name = "Button_Cancel";
            Button_Cancel.Size = new Size(149, 38);
            Button_Cancel.TabIndex = 2;
            Button_Cancel.Text = "Cancel";
            Button_Cancel.UseVisualStyleBackColor = true;
            Button_Cancel.Click += Button_Cancel_Click;
            // 
            // RecordForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(308, 421);
            Controls.Add(Button_Cancel);
            Controls.Add(Button_Save_And_Exit);
            Controls.Add(ListBox_Records);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Name = "RecordForm";
            Text = "RecordForm";
            ResumeLayout(false);
        }

        #endregion

        private ListBox ListBox_Records;
        private Button Button_Save_And_Exit;
        private Button Button_Cancel;
    }
}