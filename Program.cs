using System;
using System.Windows.Forms;

using DxLibDLL;

namespace KeystrokeCounter
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1 form = new Form1();
            form.Show();
            //while (DX.ScreenFlip() == 0 && DX.ProcessMessage() == 0 && DX.ClearDrawScreen() == 0 && form.Created)
            while (DX.ProcessMessage() == 0 && form.Created)
            {
                form.MainLoop();
                Application.DoEvents();
            }
        }
    }
}
