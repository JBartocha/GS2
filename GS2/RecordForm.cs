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
        private GameRecord record;
        public RecordForm(GameRecord record)
        {
            InitializeComponent();
            this.record = record;
            List<ListOfRecords> records = record.ListAllRecords();
            Debug.WriteLine("Pocet zaznamů: " + records.Count);
            for (int i = 0; i < records.Count; i++)
            {
                ListBox_Records.Items.Add(records[i].ID + " - " + records[i].Date);
                //Debug.WriteLine("ID: " + records[i].ID + "DATE: " + records[i].Date);
            }
            //IEnumerator<ListOfRecords> iter = records.GetEnumerator();
            //while (iter.MoveNext())
            //{
            //    ListBox_Records.Items.Add(iter.Current.ID + " - " + iter.Current.Date);
            //}
            
        }

        private void Button_Save_And_Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Button_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
