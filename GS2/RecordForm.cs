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
        private GameRecord _record; // TODO - to je tohle?
        private int _selectedID;
        private Dictionary<int,int> _RecordsIDfromSelectionBox = new Dictionary<int, int>();

        public int GetSelectedID()
        {
            return _selectedID;
        }

        public void SetSelectedID(int value)
        {
            _selectedID = value;
        }

        public RecordForm(GameRecord record)
        {
            InitializeComponent();
            this._record = record;
            List<ListOfRecords> records = record.ListAllRecords();
            for (int i = 0; i < records.Count; i++)
            {
                _RecordsIDfromSelectionBox.Add(i, Convert.ToInt32(records[i].ID));
                ListBox_Records.Items.Add(records[i].ID + "-" + records[i].Date + "-Level: " + records[i].Level 
                    + "-" + records[i].Score);
            }
        }

        private void Button_Save_And_Exit_Click(object sender, EventArgs e)
        {
            object? selectedItem = ListBox_Records.SelectedItem;
            if (selectedItem != null)
            {
                SetSelectedID(_RecordsIDfromSelectionBox[Convert.ToInt32(ListBox_Records.SelectedIndex)]);
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
