using System;
using System.Windows.Forms;
using MGPkmnLibrary.WorldClasses;

namespace XLvlEditor
{
    /* A class for the form that pops up when the user chooses to create a new layer. */
    public partial class FormNewLayer : Form
    {
        /* The mapWidth and mapHeight fields store the width and height of the map.
         * The formFinished bit represents whether the form has generated a MapLayerData. It is used by FormMain so it knows when to get the results from this form.
         * A MapLayerData object holds the layer currently being created. */
        int mapWidth;
        int mapHeight;
        bool formFinished;
        MapLayerData mapLayerData;
        public bool FormFinished
        {
            get { return formFinished; }
        }
        public MapLayerData MapLayerData
        {
            get { return mapLayerData; }
        }

        /* The constructor for a new layer form takes the width and height of the map as parameters.
         * Map width and height are set by the user when a level is created or loaded, and cannot be changed by the user in this form.
         * InitializeComponent() is called to set up the controls created in the designer file.
         * The map width and height are set according to the values passed in.
         * The btnOK.Click and btnCancel.Click event handlers are assigned with the corresponding functions defined in this class. */
        public FormNewLayer(int width, int height)
        {
            InitializeComponent();
            mapWidth = width;
            mapHeight = height;
            btnOK.Click += new EventHandler(btnOK_Click);
            btnCancel.Click += new EventHandler(btnCancel_Click);
        }

        /* This function is called when the OK button on the form is clicked.
         * It checks if the entered data is valid to create a MapLayerData, and then makes the MapLayerData if it can. */
        void btnOK_Click(object sender, EventArgs e)
        {
            /* The function checks if there's any text in the Layer Name text box, by using string.IsNullOrEmpty().
             * If the user not entered a Layer Name, a MessageBox is shown telling the user they need to enter a name, and the function returns. */
            if (string.IsNullOrEmpty(tbLayerName.Text))
            {
                MessageBox.Show("The layer must have a name.");
                return;
            }

            /* Once we know the layer will have a valid name, it needs to be constructed using the name, map width, and map height.
             * There's an optional check box (cbFill) that allows the user to choose a certain tile that they can fill the layer with.
             * If the check box is checked, the MapLayerData constructor will be passed the values of the tileset and tile NumericalUpDownSelectors.
             * Otherwise, the other overload of the constructor is used, which initializes each tile as empty. */
            if (cbFill.Checked)
            {
                mapLayerData = new MapLayerData(tbLayerName.Text, mapWidth, mapHeight, (int)nudTile.Value, (int)nudTileset.Value);
            }
            else
            {
                mapLayerData = new MapLayerData(tbLayerName.Text, mapWidth, mapHeight);
            }

            /* The formFinished field is set to true to let the main form know that everything has been done.
             * The form is then closed. */
            formFinished = true;
            Close();
        }

        /* This gets called when the "Cancel" button is clicked. */
        void btnCancel_Click(object sender, EventArgs e)
        {
            /* The formFinished field is set to false to tell the main form that the MapLayerData hasn't been made.
             * The form then closes. */
            formFinished = false;
            Close();
        }
    }
}
