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
            //Debug.WriteLine("Pocet zaznamů: " + records.Count);
            for (int i = 0; i < records.Count; i++)
            {
                ListBox_Records.Items.Add(records[i].ID + " - " + records[i].Date + " - Level: " + records[i].Level);
            }
        }

        private void Button_Save_And_Exit_Click(object sender, EventArgs e)
        {
            object? selectedItem = ListBox_Records.SelectedItem;
            if (selectedItem != null)
            {
                string s = selectedItem.ToString();
                int separator = s.IndexOf('-');
                s = s.Substring(0, separator - 1);
                int SelectedIDRecord = Convert.ToInt32(s);
                SetSelectedID(SelectedIDRecord);
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
