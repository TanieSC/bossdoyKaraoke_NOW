using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using bossdoyKaraoke_NOW.Models;

namespace bossdoyKaraoke_NOW
{
    static class Program
    {

        private static ApplicationContext context;
        private static SplashForm sForm;
        private static MainForm mForm;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new MainForm());
            sForm = new SplashForm();
            mForm = new MainForm();

            //	first we retrieve an application context for usage in the 
            //    OnAppIdle event handler
            context = new ApplicationContext();

            //	then we subscribe to the OnAppIdle event...
            Application.Idle += new EventHandler(OnAppIdle);

            //	...and show our SplashForm
            sForm.Show();

            //	instead of running a window, we use the context
            Application.Run(context);
        }

        private static void OnAppIdle(object sender, EventArgs e)
        {
            if (context.MainForm == null)
            {
                //	first we remove the eventhandler
                Application.Idle -= new EventHandler(OnAppIdle);

                //	here we preload our form
                //mForm.PreLoad();

                //	now we set the main form for the context...
                context.MainForm = mForm;

                //	...show it...
                context.MainForm.Show();

                //	...and hide the splashscreen. done!
                sForm.Close();
                sForm = null;
            }
        }
    }
}
