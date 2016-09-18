using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        BackgroundWorker t = new BackgroundWorker();
        int y;
        public Form1()
        {
            InitializeComponent();
            t.DoWork += t_DoWork;
        }

        void t_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int i = 0; i < 5; i++)
            {
                 y= i;
                Thread.Sleep(1000);
            }    
        }

        private void button1_Click(object sender, EventArgs e)
        {
            t.RunWorkerAsync();
            this.label1.Text = y.ToString();
        }
    }
}
