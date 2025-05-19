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
    public partial class HamiltonianForm : Form
    {
        private Settings settings = new Settings();
        private Hamiltonian hamiltonian;
        private Graphics? grap;
        private Bitmap? surface;
        public HamiltonianForm(Settings SS)
        {
            InitializeComponent();
            
            Debug.WriteLine(Panel_Main.Width + " " + Panel_Main.Height);
            surface = new Bitmap(Panel_Main.Width, Panel_Main.Height);
            grap = Graphics.FromImage(surface);
            Panel_Main.BackgroundImage = surface;
            Panel_Main.BackgroundImageLayout = ImageLayout.None;

            //hamiltonian = new Hamiltonian(SS.Rows, SS.Columns, SS.BlockSize, grap, SS.WallPositions);
            hamiltonian = new Hamiltonian(SS, grap);
            settings = SS;

            this.Size = new Size(SS.Columns * SS.BlockSize + 40, SS.Rows * SS.BlockSize + 40 + 100);


            Panel_Main.Invalidate();
        }
    }
}
