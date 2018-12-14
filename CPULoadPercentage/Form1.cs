using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace CPULoadPercentage
{
    public partial class Form1 : Form
    {
        NotifyIcon percentageIcon;

        Thread worker;

        public Form1()
        {
            InitializeComponent();

            percentageIcon = new NotifyIcon();
            percentageIcon.Visible = true;

            MenuItem quitMenuItem = new MenuItem("Quit");
            MenuItem programeNameMenuItem = new MenuItem("CPU Load");
            ContextMenu contextMenu = new ContextMenu();
            contextMenu.MenuItems.Add(programeNameMenuItem);
            contextMenu.MenuItems.Add(quitMenuItem);
            percentageIcon.ContextMenu = contextMenu;

            quitMenuItem.Click += QuitMenuItem_Click;

            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;

            worker = new Thread(new ThreadStart(CpuActivityThread));
            worker.Start();
        }

        private void CpuActivityThread()
        {
            PerformanceCounter perfCpuCounter = new PerformanceCounter("Processor Information", "% Processor Time", "_Total");

            while (true)
            {
                int currentCpu = (int)perfCpuCounter.NextValue();
                CreateTextIcon($"{currentCpu}");
             
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// Quit Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QuitMenuItem_Click(object sender, EventArgs e)
        {
            worker.Abort();
            percentageIcon.Dispose();
            this.Close();
        }

       /// <summary>
       /// Icon Creator
       /// </summary>
       /// <param name="str"></param>
       private void CreateTextIcon(string str)
        {
            Font fontToUse = new Font("Microsoft Sans Serif", 16, FontStyle.Bold, GraphicsUnit.Pixel);
            Brush brushToUse = new SolidBrush(Color.White);
            Bitmap bitmapText = new Bitmap(20, 20);
            Graphics g = System.Drawing.Graphics.FromImage(bitmapText);

            IntPtr hIcon;

            g.Clear(Color.Transparent);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            g.DrawString(str, fontToUse, brushToUse, -4, -2);
            hIcon = (bitmapText.GetHicon());
            percentageIcon.Icon = System.Drawing.Icon.FromHandle(hIcon);
        }
    }
}
