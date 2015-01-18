using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Real_Time_Biometric_Face_Recognition_System
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static MainFrm main_frm;
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            main_frm = new MainFrm();
            Application.Run(main_frm);
        }
    }
}
