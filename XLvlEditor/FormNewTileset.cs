using System;
using SysImage = System.Drawing.Image;
using System.Windows.Forms;
using MGPkmnLibrary.WorldClasses;

namespace XLvlEditor
{
    /* This is the form that gets displayed when the user wants to create a new tileset.
     * It's used to get the user input about the new tileset. */
    public partial class FormNewTileset : Form
    {
        /* The formFinished bit describes whether the form has finished processing the new tileset.
         * The tilesetData field holds the TilesetData object that is currently being created. */
        bool formFinished;
        TilesetData tilesetData;
        public bool FormFinished
        {
            get { return formFinished; }
        }
        public TilesetData TilesetData
        {
            get { return tilesetData; }
        }

        /* The constructor for the form first calls InitializeComponent() to create everything defined in the designer.
         * Three event handlers in the "Select Image" button, "Cancel" button and "OK" button are subscribed to with local methods.
         * SetDefault() is called to put some default values into the text boxes. */
        public FormNewTileset()
        {
            InitializeComponent();
            btnSelectImage.Click += new EventHandler(btnSelectImage_Click);
            btnOK.Click += new EventHandler(btnOK_Click);
            btnCancel.Click += new EventHandler(btnCancel_Click);
            SetDefault();
        }

        /* The SetDefault() function changes the text boxes to default values, so that I don't have to set them every time I want to debug adding tilesets. */
        private void SetDefault()
        {
            /* The tileset name defaults to "Tileset Name", and the tile width and height are 16 by default. */
            tbTilesetName.Text = "Tileset Name";
            mtbTileWidth.Text = "16";
            mtbTileHeight.Text = "16";
        }

        /* When the user clicks "Select Image", the idea is that a dialogue box will appear where the user can select the image to load.
         * This function achieves that using an OpenFileDialog(), and is fired by the button's event handler. */
        void btnSelectImage_Click(object sender, EventArgs e)
        {
            /* The OpenFileDialog is initialized and is set to filter image files only.
             * It also checks if the selected file and path exist. */
            OpenFileDialog ofDialog = new OpenFileDialog();
            ofDialog.Filter = "Image Files|*.BMP;*.GIF;*.JPG;*.JPEG;*.TGA;*.PNG";
            ofDialog.CheckFileExists = true;
            ofDialog.CheckPathExists = true;

            /* The program works out whether OK has been pressed in the dialogue by using the result from ShowDialog().
             * If OK was pressed, then the tileset image text box in the form is set with the name of the selected file. */
            DialogResult result = ofDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                tbTilesetImage.Text = ofDialog.FileName;
            }
        }

        /* This function is called when the OK button is clicked. It creates the new tileset using the user input. */
        void btnOK_Click(object sender, EventArgs e)
        {
            /* First, the two text boxes are checked to make sure they contain text, using the string.IsNullOrEmpty() function.
             * If one of the text boxes is empty, then a message box is shown to the user telling them they need to write something. */
            if (string.IsNullOrEmpty(tbTilesetName.Text))
            {
                MessageBox.Show("You must enter a name for the tileset.");
                return;
            }
            if (string.IsNullOrEmpty(tbTilesetImage.Text))
            {
                MessageBox.Show("You must choose a tileset image.");
                return;
            }

            /* The tileWidth and tileHeight fields are initialized with zero as the default value.
             * The function tries to convert the contents of the tile width and height text boxes into integers using TryParse().
             * It also checks if either of the values are less than one.
             * If either of these fail, then a message box is displayed informing the user. */
            int tileWidth = 0;
            int tileHeight = 0;
            if (!int.TryParse(mtbTileWidth.Text, out tileWidth) || tileWidth < 1)
            {
                MessageBox.Show("Tile Width must be 1 or more.");
                return;
            }
            if (!int.TryParse(mtbTileHeight.Text, out tileHeight) || tileHeight < 1)
            {
                MessageBox.Show("Tile Height must be 1 or more.");
                return;
            }

            /* Next, the function loads in the actual image associated with the tileset as a System.Drawing.Image, from the file name in the tileset image text box.
             * The tilesWide and tilesHigh fields for the tileset are calculated by dividing the image's width/height by the width/height of each individual tile. */
            SysImage tilesetImage = SysImage.FromFile(tbTilesetImage.Text);

            /* The new TilesetData is initialized and set with the name and image path from the text boxes in the form.
             * The tile width and height are set from the numbers entered into the form.
             * The number of tiles wide/high in the tileset is calculated by dividing the tileset image width/height by the individual tile width/height.
             * Finally, since the form has finished processing the new tileset, formFinished is set to true. */
            tilesetData = new TilesetData();
            tilesetData.TilesetName = tbTilesetName.Text;
            tilesetData.TilesetImageName = tbTilesetImage.Text;
            tilesetData.TileWidthInPixels = tileWidth;
            tilesetData.TileHeightInPixels = tileHeight;
            tilesetData.TilesWide = tilesetImage.Width / tileWidth;
            tilesetData.TilesHigh = tilesetImage.Height / tileHeight;
            formFinished = true;
            Close();
        }

        /* This function is called when the "Cancel" button is pressed.
         * It sets formFinished to false, since the TilesetData has not been created successfully, and the form closes. */
        void btnCancel_Click(object sender, EventArgs e)
        {
            formFinished = false;
            Close();
        }
    }
}
