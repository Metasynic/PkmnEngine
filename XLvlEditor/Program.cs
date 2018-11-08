using System;
using System.Windows.Forms;

namespace XLvlEditor
{
    /* This class is the main wrapper for the level editor. */
    static class Program
    {
        [STAThread]
        /* The Main() function is called when the program starts up. It is the entry point of the level editor. */
        static void Main(string[] args)
        {
            /* This line allows visual styles for the application, meaning controls can be drawn with different colours and fonts. */
            Application.EnableVisualStyles();

            /* CompatibleTextRendering is set as false for all controls, meaning the default text rendering system is used. */
            Application.SetCompatibleTextRenderingDefault(false);

            /* The application is run using a FormMain object which is the main window of the level editor. */
            using (FormMain formMain = new FormMain())
            {
                Application.Run(formMain);
            }
        }
    }
}

