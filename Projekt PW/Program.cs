using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Projekt_PW
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static Stacja Orlen = new Stacja();
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
           // Application.SetCompatibleTextRenderingDefault(false);
           // Stacja Orlen = new Stacja();
            Application.Run(Orlen);
        }
    }
}
