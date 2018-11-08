using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MGPkmnLibrary.WorldClasses;

namespace XLvlEditor
{
    /* This class is the form displayed when the user creates a new level.
     * It is used to obtain the user input about the level to be created. */
    public partial class FormNewLevel : Form
    {
        /* The formFinished bool represents whether the form has been completed successfully.
         * The levelData object holds the LevelData that is being created by the form. */
        bool formFinished;
        LevelData levelData;
        public bool FormFinished
        {
            get { return formFinished; }
        }
        public LevelData LevelData
        {
            get { return levelData; }
        }

        /* When the form is constructed, InitializeComponent() is called first, which creates everything made in the designer.
         * The OK button and Cancel button's click events are subscribed to with btnOK_Click() and btnCancel_Click().
         * SetDefault() is then called to give some initial values to the fields in the form. */
        public FormNewLevel()
        {
            InitializeComponent();
            btnOK.Click += new EventHandler(btnOK_Click);
            btnCancel.Click += new EventHandler(btnCancel_Click);
            SetDefault();
        }

        /* This function just fills the form fields with some default data, so that when testing, I don't have to type the same thing in over and over again. */
        private void SetDefault()
        {
            /* The Level and Map Names are set as "Example Name" and the map width and height are 20. */
            tbLevelName.Text = "Example Name";
            tbMapName.Text = "Example Name";
            mtbMapWidth.Text = "20";
            mtbMapHeight.Text = "20";
        }

        /* This function is called when the OK button is pressed in the form. */
        void btnOK_Click(object sender, EventArgs e)
        {
            /* First, the function checks that text has been entered into the level name and map name fields using string.IsNullOrEmpty().
             * If one of the strings is null or empty, a message box will be shown telling the user they need to enter something, and the function returns. */
            if (string.IsNullOrEmpty(tbLevelName.Text))
            {
                MessageBox.Show("Enter a level name.", "Missing Level Name");
                return;
            }
            if (string.IsNullOrEmpty(tbMapName.Text))
            {
                MessageBox.Show("Enter a map name.", "Missing Map Name");
                return;
            }

            /* Two integer variables are initialized with values of zero to hold the width and height.
             * The text in the map width and map height text boxes are then converted to ints using int.TryParse().
             * If one of the TryParse() fails, or one of the values is less than one, a message box is shown to tell the user to try again. */
            int mapWidth = 0;
            int mapHeight = 0;
            if (!int.TryParse(mtbMapWidth.Text, out mapWidth) || mapWidth < 1)
            {
                MessageBox.Show("The map must have a width of 1 or more.", "Map Width Error");
                return;
            }
            if (!int.TryParse(mtbMapHeight.Text, out mapHeight) || mapHeight < 1)
            {
                MessageBox.Show("The map must have a height of 1 or more.", "Map Height Error");
                return;
            }

            /* A new LevelData is created using the text in the name boxes and the values obtained from TryParse().
             * An empty List<string> is passed in as the list of character names.
             * Now that everything's done, the formFinished field is set to true so the main form knows the LevelData has been created successfully.
             * The form will now close. */
            levelData = new LevelData(tbLevelName.Text, tbMapName.Text, mapWidth, mapHeight, new List<string>());
            formFinished = true;
            Close();
        }

        /* If the Cancel button is pressed, this function runs via event handler. */
        void btnCancel_Click(object sender, EventArgs e)
        {
            /* Since no LevelData has been created, the formFinished field is set to false so the main form knows this form wasn't completed.
             * The form then closes. */
            formFinished = false;
            Close();
        }
    }
}
