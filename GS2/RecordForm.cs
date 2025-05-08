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

namespace GS2
{
    public partial class RecordForm : Form
    {
        private GameRecord record; // TODO - to je tohle?
        private int selectedID;
        private Dictionary<int,int> recordsIDfromSelectionBox = new Dictionary<int, int>();

        public int GetSelectedID()
        {
            return selectedID;
        }

        public void SetSelectedID(int value)
        {
            selectedID = value;
        }

        public RecordForm(GameRecord record)
        {
            InitializeComponent();
            this.record = record;
            List<ListOfRecords> records = record.ListAllRecords();
            for (int i = 0; i < records.Count; i++)
            {
                recordsIDfromSelectionBox.Add(i, Convert.ToInt32(records[i].ID));
                ListBox_Records.Items.Add(records[i].ID + "-" + records[i].Date + "-Level: " + records[i].Level 
                    + "-" + records[i].Score);
            }
        }

        private void Button_Save_And_Exit_Click(object sender, EventArgs e)
        {
            object? selectedItem = ListBox_Records.SelectedItem;
            if (selectedItem != null)
            {
                SetSelectedID(recordsIDfromSelectionBox[Convert.ToInt32(ListBox_Records.SelectedIndex)]);
            }
            else
            {
                throw new NoNullAllowedException();
            }
            this.Close();
        }

        private void Button_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
