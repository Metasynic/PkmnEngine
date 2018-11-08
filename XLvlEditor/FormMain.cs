using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using SysBitmap = System.Drawing.Bitmap;
using SysGraphics = System.Drawing.Graphics;
using SysUnit = System.Drawing.GraphicsUnit;
using SysImage = System.Drawing.Image;
using SysRect = System.Drawing.Rectangle;

using MGPkmnLibrary.WorldClasses;
using MGPkmnLibrary.TileEngine;

namespace XLvlEditor
{
    /* The FormMain class is the main form class in the Level Editor. */
    public partial class FormMain : Form
    {
        /* The shadowPos Point stores the current tile position of the mouse.
         * It holds the position of the tile which should be shaded (the one that has the mouse over it).
         * The mouse Point stores the pixel position of the mouse. It is used to determine whether the map should scroll or not. */
        Point shadowPos = new Point();
        Point mouse = new Point();

        /* The brushWidth field is a measurement of how many tiles will be changed when the user draws new tiles in the Level Editor.
         * The gridColour is the tint colour that the grid texture is drawn with. It is white by default. */
        byte brushWidth = 1;
        Color gridColour = Color.White;

        /* The SpriteBatch for the form is used to draw the mapDisplay.
         * The LevelData stores the information about the current level being edited.
         * The TileMap stores the current map being edited.
         * Finally, the Camera and Engine are used to render the mapDisplay and set the tile width and height. */
        SpriteBatch spriteBatch;
        LevelData levelData;
        TileMap map;
        Camera camera;
        Engine engine;

        /* The tilesets and tilesetDataList fields are lists holding the Tileset and TilesetData objects in the editor.
         * Similarly, animatedTileset and animatedTilesetData hold the objects for an animated tileset, although it's not fully working yet. */
        List<Tileset> tilesetList = new List<Tileset>();
        List<TilesetData> tilesetDataList = new List<TilesetData>();
        AnimatedTileset animatedTileset;
        AnimatedTilesetData animatedTilesetData;

        /* The layers list holds every map layer currently loaded into the editor.
         * The tilesetImages list stores the actual images for each tileset. */
        List<InterfaceLayer> layerList = new List<InterfaceLayer>();
        List<SysImage> tilesetImageList = new List<SysImage>();

        /* This bit, mouseDown, is used to determine whether the editor should write tiles to the map (which happens if the user is holding the left mouse button).
         * The second bit, mouseTrack, is true whenever the mouse is in the mapDisplay and if it's true, the editor checks ehether to scroll the map. */
        bool mouseDown = false;
        bool mouseTrack = false;

        /* These two Texture2Ds hold the sprites of the cursor border rectangle and the grid border rectangle. */
        Texture2D cursor;
        Texture2D grid;

        /* The frameCount value is used to store how many frames have passed since the last map scroll. */
        int frameCount = 0;

        /* GraphicsDevice is a property exposing the mapDisplay's GraphicsDevice. */
        public GraphicsDevice GraphicsDevice
        {
            get { return mapDisplay.GraphicsDevice; }
        }

        /* The FormMain() constructor initializes most of the event handlers in the form. */
        public FormMain()
        {
            /* InitializeComponent() calls the designer file to create the form. */
            InitializeComponent();
            Load += new EventHandler(FormMain_Load);
            FormClosing += new FormClosingEventHandler(FormMain_FormClosing);
            mapDisplay.OnInitialize += new EventHandler(mapDisplay_OnInitialize);
            mapDisplay.OnDraw += new EventHandler(mapDisplay_OnDraw);

            /* Event handlers for the level, layer, and tileset menu items. */
            newLevelToolStripMenuItem.Click += new EventHandler(newLevelToolStripMenuItem_Click);
            newTilesetToolStripMenuItem.Click += new EventHandler(newTilesetToolStripMenuItem_Click);
            newLayerToolStripMenuItem.Click += new EventHandler(newLayerToolStripMenuItem_Click);
            saveLevelToolStripMenuItem.Click += new EventHandler(saveLevelToolStripMenuItem_Click);
            openLevelToolStripMenuItem.Click += new EventHandler(openLevelToolStripMenuItem_Click);
            saveTilesetToolStripMenuItem.Click += new EventHandler(saveTilesetToolStripMenuItem_Click);
            openTilesetToolStripMenuItem.Click += new EventHandler(openTilesetToolStripMenuItem_Click);
            saveLayerToolStripMenuItem.Click += new EventHandler(saveLayerToolStripMenuItem_Click);
            openLayerToolStripMenuItem.Click += new EventHandler(openLayerToolStripMenuItem_Click);

            /* More event handlers for the brush size menu items. */
            x1ToolStripMenuItem.Click += new EventHandler(x1ToolStripMenuItem_Click);
            x2ToolStripMenuItem.Click += new EventHandler(x2ToolStripMenuItem_Click);
            x4ToolStripMenuItem.Click += new EventHandler(x4ToolStripMenuItem_Click);
            x8ToolStripMenuItem.Click += new EventHandler(x8ToolStripMenuItem_Click);

            /* Even more event handlers for the grid colour menu items. */
            whiteToolStripMenuItem.Click += new EventHandler(whiteToolStripMenuItem_Click);
            redToolStripMenuItem.Click += new EventHandler(redToolStripMenuItem_Click);
            blueToolStripMenuItem.Click += new EventHandler(blueToolStripMenuItem_Click);
            greenToolStripMenuItem.Click += new EventHandler(greenToolStripMenuItem_Click);
            yellowToolStripMenuItem.Click += new EventHandler(yellowToolStripMenuItem_Click);
            blackToolStripMenuItem.Click += new EventHandler(blackToolStripMenuItem_Click);

            /* These menu items are disabled on startup, since the user must load or create a level before using other functions. */
            tilesetToolStripMenuItem.Enabled = false;
            mapLayerToolStripMenuItem.Enabled = false;
            nPCsToolStripMenuItem.Enabled = false;
        }

        /* This function is called when the form is first loaded. */
        private void FormMain_Load(object sender, EventArgs e)
        {
            /* Still more self-explanatory event handlers are wired here. */
            lbTileset.SelectedIndexChanged += new EventHandler(lbTileset_SelectedIndexChanged);
            nudCurrentTile.ValueChanged += new EventHandler(nudCurrentTile_ValueChanged);
            pbTilesetPreview.MouseDown += new MouseEventHandler(pbTilesetPreview_MouseDown);
            mapDisplay.SizeChanged += new EventHandler(mapDisplay_SizeChanged);

            /* The viewPort rectangle is the camera's rectangle. It has the width and height of the mapDisplay.
             * The camera is created with the viewPort rectangle and a zoom of one.
             * The engine is initialized with 16x16 tile dimensions. */
            Rectangle viewPort = new Rectangle(0, 0, mapDisplay.Width, mapDisplay.Height);
            camera = new Camera(viewPort, 1f, null);
            engine = new Engine(16, 16);

            /* The controlTimer is set with some important values here. There's an event handler for when it ticks.
             * Emabled is set to true by default, since it should be counting on startup.
             * Interval is 17 milliseconds, which yields approximately 60 frames per second. */
            controlTimer.Tick += new EventHandler(controlTimer_Tick);
            controlTimer.Enabled = true;
            controlTimer.Interval = 17;

            /* The coordinate text box has an alignment in the centre. */
            tbMapLocation.TextAlign = HorizontalAlignment.Center;
        }

        /* Nothing special happens when the form is closed. */
        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        /* When the "New Level" button is clicked, this function is run. */
        private void newLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /* A new FormNewLevel is used to capture input about the new level. */
            using (FormNewLevel formNewLevel = new FormNewLevel())
            {
                /* The formNewLevel is shown using ShowDialog(). Once the form is finished,
                 * the levelData field is set using the form's LevelData, and the tileset tool strip is enabled, since a level now exists. */
                formNewLevel.ShowDialog();
                if (formNewLevel.FormFinished)
                {
                    levelData = formNewLevel.LevelData;
                    tilesetToolStripMenuItem.Enabled = true;
                }
            }
        }

        /* When the "New Tileset" button is pressed, its event handler calls this function. */
        private void newTilesetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /* A new FormNewTileset is created and shown using ShowDialog(). */
            using (FormNewTileset formNewTileset = new FormNewTileset())
            {
                formNewTileset.ShowDialog();

                /* If the OK button is pressed on the form, then a new TilesetData object will be made using the one from the form. */
                if (formNewTileset.FormFinished)
                {
                    TilesetData tilesetData = formNewTileset.TilesetData;
                    try
                    {
                        /* The program will try to load the actual image from the file name in the tileset data entered in.
                         * It then adds the image to the tilesetImageList. */
                        SysImage image = SysImage.FromFile(tilesetData.TilesetImageName);
                        tilesetImageList.Add(image);

                        /* A new FileStream object with the tileset image being read is created.
                         * The Texture2D for the tileset is made using Texture2D.FromStream() with the FileStream.
                         * Once the texture is loaded in, the actual Tileset object can be created using the data and added to the tilesetList. */
                        Stream s = new FileStream(tilesetData.TilesetImageName, FileMode.Open, FileAccess.Read);
                        Texture2D texture = Texture2D.FromStream(GraphicsDevice, s);
                        Tileset tileset = new Tileset(texture, tilesetData.TilesWide, tilesetData.TilesHigh, tilesetData.TileWidthInPixels, tilesetData.TileHeightInPixels);
                        tilesetList.Add(tileset);

                        /* The tileset data is also added to its respective list. */
                        tilesetDataList.Add(tilesetData);

                        /* If the map exists then the tileset is added to it. */
                        if (map != null)
                            map.AddTileset(tileset);

                        /* The FileStream is no longer needed, so it's disposed of. */
                        s.Dispose();
                    }

                    /* If something goes wrong while loading the tileset, then a message box will be shown with the error. */
                    catch (Exception x)
                    {
                        MessageBox.Show("Error creating new tileset.\n" + x.Message, "Tileset Image Error");
                    }

                    /* The tileset list box has the new tileset's name added to it.
                     * If there's no selected item in the list box (which would mean the list box was empty before), the selected index is changed to zero.
                     * This means the new tileset will be selected in the list box.
                     * Now that there's a tileset in the level, map layers can be made, so the map layer menu is enabled. */
                    lbTileset.Items.Add(tilesetData.TilesetName);
                    if (lbTileset.SelectedItem == null)
                        lbTileset.SelectedIndex = 0;
                    mapLayerToolStripMenuItem.Enabled = true;
                }
            }
        }

        /* This function is attached to the "New Layer" button's event handler, and is invoked when the button is clicked. */
        private void newLayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /* A new FormNewLayer is used to read user input about the new layer. The map width and height are passed in. */
            using (FormNewLayer formNewLayer = new FormNewLayer(levelData.MapWidth, levelData.MapHeight))
            {
                /* The form is shown using ShowDialog(), and the program waits until the form has finished processing. */
                formNewLayer.ShowDialog();
                if (formNewLayer.FormFinished)
                {
                    /* A new MapLayerData object is created using the data entered into the form.
                     * If there is already a layer with the same name in the map layer check box, then the user is notified and the function returns. */
                    MapLayerData mapLayerData = formNewLayer.MapLayerData;
                    if (clbLayers.Items.Contains(mapLayerData.MapLayerName))
                    {
                        MessageBox.Show("Map Layer with name " + mapLayerData.MapLayerName + " already exists.", "Layer already exists");
                        return;
                    }

                    /* Once we know that the new layer doesn't have the same name as any existing layers,
                     * the actual MapLayer is made using the static MapLayer.FromMapLayerData() function.
                     * The layer's name is added to the map layer check list and is selected by default.
                     * The check list's selected index is set to the last item in the list (which should be the new layer).
                     * Finally, the map layer is added to the layerList. */
                    MapLayer layer = MapLayer.FromMapLayerData(mapLayerData);
                    clbLayers.Items.Add(mapLayerData.MapLayerName, true);
                    clbLayers.SelectedIndex = clbLayers.Items.Count - 1;
                    layerList.Add(layer);

                    /* This is the point when the map gets created if it doesn't exist already.
                     * The map's constructor has the first tileset, the animated tileset, and the first layer passed in.
                     * After that, any other tilesets or layers in their respective lists are added to the map. */
                    if (map == null)
                    {
                        map = new TileMap(tilesetList[0], animatedTileset, (MapLayer)layerList[0]);
                        for (int i = 1; i < tilesetList.Count; i++)
                        {
                            map.AddTileset(tilesetList[i]);
                        }
                        for (int i = 1; i < layerList.Count; i++)
                        {
                            map.AddLayer(layerList[i]);
                        }
                    }

                    /* Now that a layer exists, the player should be able to add NPC trainers to the level.
                     * Creating trainers with the level editor isn't part of the user requirements so it's not in the code (yet). */
                    nPCsToolStripMenuItem.Enabled = true;
                }
            }
        }

        /* When the "Save Level" button is clicked, this function is called. */
        private void saveLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /* If the map doesn't exist, then there's nothing to save, so the function returns. */
            if (map == null)
                return;

            /* The program creates a new list for the MapLayerData objects to be saved. */
            List<MapLayerData> mapLayerDataList = new List<MapLayerData>();

            /* For each of the map layers in the layerList, the program checks if the InterfaceLayer is a MapLayer (it should always be). */
            for (int i = 0; i < clbLayers.Items.Count; i++)
            {
                if (layerList[i] is MapLayer)
                {
                    /* The MapLayer is converted into a new MapLayerData using the properties of the MapLayer.
                     * Then, for each tile in the layer, the tile in the MapLayerData is set to the tile in the MapLayer.
                     * Once the MapLayerData is complete, it is added to the list of data to be saved. */
                    MapLayerData data = new MapLayerData(clbLayers.Items[i].ToString(), ((MapLayer)layerList[i]).Width, ((MapLayer)layerList[i]).Height);
                    for (int y = 0; y < ((MapLayer)layerList[i]).Height; y++)
                    {
                        for (int x = 0; x < ((MapLayer)layerList[i]).Width; x++)
                        {
                            data.SetTile(x, y, ((MapLayer)layerList[i]).GetTile(x, y).TileIndex, ((MapLayer)layerList[i]).GetTile(x, y).Tileset, ((MapLayer)layerList[i]).GetTile(x, y).Solid, ((MapLayer)layerList[i]).GetTile(x, y).Spawn);
                        }
                    }
                    mapLayerDataList.Add(data);
                }
            }

            /* A new MapData object is created using the mapLayerDataList, the tilesetDataList, a new animated layer, and a new animated tileset. */
            MapData mapData = new MapData(levelData.MapName, mapLayerDataList, new AnimatedMapLayer(), tilesetDataList, new AnimatedTilesetData());

            /* A FolderBrowserDialog is used to get the user input for which folder to save to.
             * The starting path is the program's startup path. */
            FolderBrowserDialog folderDialogue = new FolderBrowserDialog();
            folderDialogue.Description = "Select GameData Folder";
            folderDialogue.SelectedPath = Application.StartupPath;

            /* The program determines whether the "OK" button was pressed in the folderDialogue.
             * If the "OK" button was pressed, the path to save the Level and Map are worked out using Path.Combine().
             * The level will be saved as an XML file in [SelectedFolder]/LevelData/.
             * The map will be saved as an XML file in [SelectedFolder]/LevelData/MapData/.
             * If the folders do not exist, then they will be created.
             * The XmlTool class is used to save the LevelData and MapData objects. */
            DialogResult result = folderDialogue.ShowDialog();
            if (result == DialogResult.OK)
            {
                string LevelPath = Path.Combine(folderDialogue.SelectedPath, @"LevelData/");
                string MapPath = Path.Combine(LevelPath, @"MapData/");
                if (!Directory.Exists(LevelPath))
                    Directory.CreateDirectory(LevelPath);
                if (!Directory.Exists(MapPath))
                    Directory.CreateDirectory(MapPath);
                XmlTool.Save(LevelPath + levelData.LevelName + ".xml", levelData);
                XmlTool.Save(MapPath + mapData.MapName + ".xml", mapData);
            }
        }

        /* This function is called when the "Open Level" button is clicked. */
        private void openLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /* An OpenFileDialog is used to get the user input for which level should be loaded.
             * The openDialogue is filtered so that it only looks at XML files, and checks if the selected path and file exist. */
            OpenFileDialog openDialogue = new OpenFileDialog();
            openDialogue.Filter = "XML Files (*.xml)|*.xml";
            openDialogue.CheckFileExists = true;
            openDialogue.CheckPathExists = true;

            /* The program works out whether OK has been pressed by using openDialogue.ShowDialog().
             * If the OK button has been pressed, the function continues. */
            DialogResult result = openDialogue.ShowDialog();
            if (result != DialogResult.OK)
                return;

            /* The base path to load the files from is obtained from the file name in the dialogue.
             * The LevelData and MapData are declared in preparation for loading. */
            string path = Path.GetDirectoryName(openDialogue.FileName);
            LevelData newLevelData;
            MapData newMapData;
            try
            {
                /* The level data and map data are loaded in using the XmlTool and the path from the dialogue.
                 * The map data is found by taking the folder the level data was in, adding "/MapData/" to the path, 
                 * and using the level data's map name to get an XML file. */
                newLevelData = XmlTool.Load<LevelData>(openDialogue.FileName);
                string mapPath = path + @"/MapData/" + newLevelData.MapName + ".xml";
                newMapData = XmlTool.Load<MapData>(mapPath);
            }
            catch (Exception x)
            {
                /* If something goes wrong during the level loading process, a message box is shown detailing the error.
                 * The level data and map data are set as null so the program can continue. */
                MessageBox.Show(x.Message, "Error reading Level");
                newLevelData = null;
                newMapData = null;
            }

            /* All the different lists of components are cleared, since they need to be filled with the items from the new level and map. */
            tilesetImageList.Clear();
            tilesetDataList.Clear();
            tilesetList.Clear();
            layerList.Clear();
            lbTileset.Items.Clear();
            clbLayers.Items.Clear();

            /* The current level data field is set to the level data read in. */
            levelData = newLevelData;

            /* The function iterates through the TilesetData fields in the new map data, and adds each one to the tilesetDataList.
             * The tileset's name is also added to the tileset list box so it shows on the screen.
             * Next, the actual sprite sheet image is loaded in using the TilesetImageName property of the TilesetData.
             * The image is added to the list of tileset images. */
            foreach (TilesetData data in newMapData.Tilesets)
            {
                Texture2D texture = null;
                tilesetDataList.Add(data);
                lbTileset.Items.Add(data.TilesetName);
                SysImage image = SysImage.FromFile(data.TilesetImageName);
                tilesetImageList.Add(image);

                /* A new FileStream is used to read in the image as a Texture2D.
                 * The new Tileset can now be created and added to the list, using the texture and fields from the TilesetData. */
                using (Stream s = new FileStream(data.TilesetImageName, FileMode.Open, FileAccess.Read))
                {
                    texture = Texture2D.FromStream(GraphicsDevice, s);
                    tilesetList.Add(new Tileset(texture, data.TilesWide, data.TilesHigh, data.TileWidthInPixels, data.TileHeightInPixels));
                }
            }

            /* After this, the function goes through each of the MapLayerData objects in the new map data.
             * It adds each layer to the layer check box so it shows up on the screen.
             * A new MapLayer object is derived from the MapLayerData using the static FromMapLayerData() function. */
            foreach (MapLayerData data in newMapData.Layers)
            {
                clbLayers.Items.Add(data.MapLayerName, true);
                layerList.Add(MapLayer.FromMapLayerData(data));
            }

            /* The indexes of all the lists and selectors are reset to zero. */
            lbTileset.SelectedIndex = 0;
            clbLayers.SelectedIndex = 0;
            nudCurrentTile.Value = 0;

            /* The TileMap for the editor is constructed using the first tileset, the first map layer, and the animated tileset.
             * Then, the other tilesets and map layers are added to the map through iteration. */
            map = new TileMap(tilesetList[0], animatedTileset, (MapLayer)layerList[0]);
            for (int i = 1; i < tilesetList.Count; i++)
            {
                map.AddTileset(tilesetList[i]);
            }
            for (int i = 1; i < layerList.Count; i++)
            {
                map.AddLayer(layerList[i]);
            }

            /* Now that a full map with layers and tilesets has been added, all the tool strips are enabled since the user can now edit however they like. */
            tilesetToolStripMenuItem.Enabled = true;
            mapLayerToolStripMenuItem.Enabled = true;
            nPCsToolStripMenuItem.Enabled = true;
        }

        /* This function is called when the "Save Tileset" button is pressed.
         * Saving a tileset allows a user to load it again without having to find the image and enter its tile dimensions each time they want to use it. */
        private void saveTilesetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /* If there are no TilesetData objects in the tilesetDataList, there's nothing to save, so the function returns. */
            if (tilesetDataList.Count == 0)
                return;

            /* The program will try to save the TilesetData to the "PkmnEditor/GameData/Tilesets/" folder, using the data's TilesetName property as the XML file name.
             * If there's an error, it will be handled and a message box will display containing details of the exception. */
            try
            {
                XmlTool.Save(@"../../../../../GameData/Tilesets/" + tilesetDataList[lbTileset.SelectedIndex].TilesetName + ".xml", tilesetDataList[lbTileset.SelectedIndex]);
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message, "Error saving tileset");
            }
        }

        /* This function is invoked via event handler when the "Open Tileset" button is clicked. */
        private void openTilesetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /* An OpenFileDialog is used to get the user input for what file to extract a TilesetData object from.
             * The folder selection dialogue filters XML files only, as that is the format that tilesets are saved in.
             * It also checks if the selected path and file exist. RestoreDirectory is true so that the working folder for the program remains the same. */
            OpenFileDialog openDialogue = new OpenFileDialog();
            openDialogue.Filter = "Tileset Data (*.xml)|*.xml";
            openDialogue.CheckPathExists = true;
            openDialogue.CheckFileExists = true;
            openDialogue.RestoreDirectory = true;

            /* The function displays the openDialogue, allowing the user to select a file to load from.
             * If the user didn't click "OK" on the openDialogue, then the function returns as they pressed "Cancel". */
            DialogResult result = openDialogue.ShowDialog();
            if (result != DialogResult.OK)
                return;

            /* Four fields to hold the tileset's data, texture, object, and image are initialized as null. */
            TilesetData data = null;
            Texture2D texture = null;
            Tileset tileset = null;
            SysImage image = null;

            /* Now that we have a file name to work with, the function tries to load the TilesetData from the file using the XmlTool.
             * The openDialogue.FileName is passed in because that's the name of the file that the user selected in the dialogue.
             * Next, the Texture2D for the tileset image is loaded using a FileStream with the tileset image name from the data passed in.
             * Texture2D.FromStream() is used to read the image from the stream.
             * A System.Drawing.Image for the tileset image is loaded using SysImage.FromFile().
             * Finally, the Tileset object itself is created using the texture and information from the data. */
            try
            {
                data = XmlTool.Load<TilesetData>(openDialogue.FileName);
                using (Stream s = new FileStream(data.TilesetImageName, FileMode.Open, FileAccess.Read))
                {
                    texture = Texture2D.FromStream(GraphicsDevice, s);
                    s.Close();
                }
                image = SysImage.FromFile(data.TilesetImageName);
                tileset = new Tileset(texture, data.TilesWide, data.TilesHigh, data.TileWidthInPixels, data.TileHeightInPixels);
            }

            /* If an error is caught during the loading process, then a message box shows with an error decribing the problem. */
            catch (Exception x)
            {
                MessageBox.Show(x.Message, "Error Loading Tileset");
            }

            /* In the next part of the process, the function iterates through all of the currently existing tilesets 
             * and checks if there's one with the same name as the one that's been loaded in.
             * It does this by iterating through the tileset list box and checking if each piece of text (tileset name) in the list box matches the data's name.
             * If they match, the program returns since it can't handle two tilesets with the same name. */
            for (int i = 0; i < lbTileset.Items.Count; i++)
            {
                if (lbTileset.Items[i].ToString() == data.TilesetName)
                {
                    MessageBox.Show("There is already a tileset with this name.", "Tileset already exists");
                    return;
                }
            }

            /* The data, tileset and image are added to their respective lists.
             * After this, the tileset list box has the new tileset's name added to it. */
            tilesetDataList.Add(data);
            tilesetList.Add(tileset);
            tilesetImageList.Add(image);
            lbTileset.Items.Add(data.TilesetName);

            /* Next, the new tileset needs to be selected in the editor.
             * This is done by changing the tileset preview image to the tileset image, setting the selected index in the tileset list box to the last item,
             * and resetting the tile NumericalUpDownSelector's index to zero. */
            pbTilesetPreview.Image = image;
            lbTileset.SelectedIndex = lbTileset.Items.Count - 1;
            nudCurrentTile.Value = 0;

            /* Finally, since a valid tileset has now been loaded, MapLayers can be worked with, so the tool strip item is enabled. */
            mapLayerToolStripMenuItem.Enabled = true;
        }

        /* This function is called when the "Save Layer" button is pressed in the menu.
         * It saves a single MapLayer for use later. This is useful because someone making levels might want to save a common template, such as a large square of grass or water. */
        private void saveLayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /* If there are no layers in the list, then there's nothing that can be saved, so the function returns. */
            if (layerList.Count == 0)
                return;

            /* An if clause checks if the currently selected layer is a MapLayer. It should be, since only MapLayer inherits from InterfaceLayer. */
            if (layerList[clbLayers.SelectedIndex] is MapLayer)
            {
                /* A new MapLayerData object is initialized using the layer's name (obtained from the check box), its width and its height.
                 * This is the MapLayerData that will get serialized into XML, but the tiles need to be added to it first.
                 * Two nested for loops iterate through the height and width of the map, looking at each tile in turn.
                 * MapLayerData.SetTile() is used to set each tile in the data, using properties obtained using MapLayer.GetTile().
                 * The y and x variables represent the y and x coordinates of the current tile being processed. */
                MapLayerData data = new MapLayerData(clbLayers.SelectedItem.ToString(), ((MapLayer)layerList[clbLayers.SelectedIndex]).Width, ((MapLayer)layerList[clbLayers.SelectedIndex]).Height);
                for (int y = 0; y < ((MapLayer)layerList[clbLayers.SelectedIndex]).Height; y++)
                {
                    for (int x = 0; x < ((MapLayer)layerList[clbLayers.SelectedIndex]).Width; x++)
                    {
                        data.SetTile(x, y, ((MapLayer)layerList[clbLayers.SelectedIndex]).GetTile(x, y).TileIndex, ((MapLayer)layerList[clbLayers.SelectedIndex]).GetTile(x, y).Tileset, ((MapLayer)layerList[clbLayers.SelectedIndex]).GetTile(x, y).Solid, ((MapLayer)layerList[clbLayers.SelectedIndex]).GetTile(x, y).Spawn);
                    }
                }

                /* The function tries to save the MapLayerData to the "PkmnEditor/GameData/Layers/" folder using the XmlTool. */
                try
                {
                    XmlTool.Save(@"../../../../../GameData/Layers/" + data.MapLayerName + ".xml", data);
                }

                /* If there's an error during the saving process, a message box is displayed with the exception details on it. */
                catch (Exception x)
                {
                    MessageBox.Show(x.Message, "Error saving Map Layer");
                }
            }
        }

        /* This function is wired to the openLayerToolStripMenuItem.Click event handler, so it's invoked when the user presses "Open Layer". */
        private void openLayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /* An OpenFileDialog is used to let the user choose what file to load a layer from.
             * It is filtered to only accept XML files (as that's the format layers are saved in).
             * It will also check if the path and file it's trying to load from exist. */
            OpenFileDialog openDialogue = new OpenFileDialog();
            openDialogue.Filter = "Map Layer Data (*.xml)|*.xml";
            openDialogue.CheckPathExists = true;
            openDialogue.CheckFileExists = true;

            /* The dialogue is displayed by calling ShowDialog(), which returns a DialogResult describing which button was pressed.
             * If the button pressed wasn't "OK" (implying it was "Cancel"), the function returns.
             * A new MapLayerData object is initialized as null in preparation to be set as the loaded data. */
            DialogResult result = openDialogue.ShowDialog();
            if (result != DialogResult.OK)
                return;
            MapLayerData data = null;

            /* The function tries to load the MapLayerData from the file specified in the OpenFileDialog, using the XmlTool.
             * The openDialogue.FileName is passed in since that's the name of the file selected by the user. */
            try
            {
                data = XmlTool.Load<MapLayerData>(openDialogue.FileName);
            }

            /* If there's an error loading the layer, the details are shown to the user using a MessageBox and the function returns. */
            catch (Exception x)
            {
                MessageBox.Show(x.Message, "Error reading map layer data");
                return;
            }

            /* Next, a for loop iterates through each item in the layer check box list.
             * It checks if the newly loaded MapLayerData has the same name as any of the items in the list.
             * If they match, there are duplicate names, which are not supported by the editor, so a MessageBox is shown informing the user, and the function returns. */
            for (int i = 0; i < clbLayers.Items.Count; i++)
            {
                if (data.MapLayerName == clbLayers.Items[i].ToString())
                {
                    MessageBox.Show("There is already a layer with this name.", "Map Layer already exists");
                    return;
                }
            }

            /* The new layer's name is added to the check box list and is checked by default.
             * An actual MapLayer object is created from the data using the static MapLayer.FromMapLayerData() function, and added to the layerList. */
            clbLayers.Items.Add(data.MapLayerName, true);
            layerList.Add(MapLayer.FromMapLayerData(data));

            /* If there is no map, then this is the first layer to be loaded in.
             * The map is initialized with the first tileset, the animated tileset, and the newly loaded layer.
             * Finally, the function iterates through any remaining tilesets in the tilesetList and adds them to the map as well. */
            if (map == null)
            {
                map = new TileMap(tilesetList[0], animatedTileset, (MapLayer)layerList[0]);
                for (int i = 1; i < tilesetList.Count; i++)
                {
                    map.AddTileset(tilesetList[i]);
                }
            }
        }

        /* This function initializes the mapDisplay on the main window.
         * The mapDisplay is the control on the screen that uses XNA to draw the map. */
        private void mapDisplay_OnInitialize(object sender, EventArgs e)
        {
            /* The SpriteBatch is initialized using the GraphicsDevice. It will be needed later for drawing things.
             * The five event handlers related to the mouse are assigned with their relevant functions. */
            spriteBatch = new SpriteBatch(GraphicsDevice);
            mapDisplay.MouseEnter += new EventHandler(mapDisplay_MouseEnter);
            mapDisplay.MouseLeave += new EventHandler(mapDisplay_MouseLeave);
            mapDisplay.MouseMove += new MouseEventHandler(mapDisplay_MouseMove);
            mapDisplay.MouseUp += new MouseEventHandler(mapDisplay_MouseUp);
            mapDisplay.MouseDown += new MouseEventHandler(mapDisplay_MouseDown);

            /* The editor uses two separate FileStream objects to load in the two border images for use in the editor.
             * In both cases, the Texture2D.FromStream() function is used to create the texture from the file. */
            try
            {
                using (Stream stream = new FileStream(@"Content/grid.png", FileMode.Open, FileAccess.Read))
                {
                    grid = Texture2D.FromStream(GraphicsDevice, stream);
                    stream.Close();
                }
                using (Stream stream = new FileStream(@"Content/cursor.png", FileMode.Open, FileAccess.Read))
                {
                    cursor = Texture2D.FromStream(GraphicsDevice, stream);
                    stream.Close();
                }
            }

            /* If there is a problem loading the grid and cursor images, the user is informed via a MessageBox and the fields are set as null. */
            catch (Exception x)
            {
                MessageBox.Show(x.Message, "Error loading images");
                grid = null;
                cursor = null;
            }
        }

        /* This function is called when the mapDisplay is drawn.
         * It clears the screen with a light grey colour, and then calls the local Render() function to draw the map. */
        private void mapDisplay_OnDraw(object sender, EventArgs e)
        {
            GraphicsDevice.Clear(Color.LightGray);
            MapDraw();
        }

        /* When the mouse enters the mapDisplay, this function is invoked via event handler.
         * The mouseTrack bit is set to true so that the program will track mouse movement to determine shadow position.
         * When the mouse leaves the mapDisplay, mouseTrack is set to false so the program no longer tracks the mouse position. */
        private void mapDisplay_MouseEnter(object sender, EventArgs e)
        {
            mouseTrack = true;
        }
        private void mapDisplay_MouseLeave(object sender, EventArgs e)
        {
            mouseTrack = false;
        }

        /* When the mouse moves inside the mapDisplay, this function grabs the mouse coordinates (which are in the MouseEventArgs). */
        private void mapDisplay_MouseMove(object sender, MouseEventArgs e)
        {
            mouse.X = e.X;
            mouse.Y = e.Y;
        }

        /* These two functions are called when the mouse button is pressed or released while the mouse is in the mapDisplay.
         * They check if the button in question is the left mouse button.
         * If it is, the mouseDown bit is set appropriately depending on whether the mouse is down or up.
         * When mouseDown is true, the program will draw a new tile on the location of the mouse. */
        private void mapDisplay_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                mouseDown = false;
        }
        private void mapDisplay_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                mouseDown = true;
        }

        /* The controlTimer behaves like a clock to trigger the MapUpdate() function. It increases the frameCount and sets it back to zero if it's six.
         * This means the frame count will constantly tick from zero to six repeatedly, meaning the map display will scroll every six frames (see below).
         * The Invalidate() function is called to draw the whole mapDisplay again, and then MapUpdate() is called to handle scrolling and tile placement. */
        private void controlTimer_Tick(object sender, EventArgs e)
        {
            frameCount++;
            frameCount %= 6;
            mapDisplay.Invalidate();
            MapUpdate();
        }

        /* This function updates the mapDisplay and scrolls the map. */
        private void MapUpdate()
        {
            /* If there are no layers, nothing needs to happen because there's no level, so the function returns. */
            if (layerList.Count == 0)
                return;

            /* If the mouseTrack bit is true, the function will scroll the map.
             * The frameCount being zero will occur once every six frames (as it wraps around from six after incrementing, see above).
             * A Vector2 called newPos is used to hold the new camera position as it's being modified.
             * If the mouse is hovering in a position where it's less than one tile away from the edge of the map, the camera moves by one tile.
             * The direction the camera scrolls depends on the edge the mouse was next to.
             * The camera's position is set as the newPos, and the camera is locked so it doesn't go past the edge of the map. */
            if (mouseTrack == true)
            {
                if (frameCount == 0)
                {
                    Vector2 newPos = camera.Position;
                    if (mouse.X < Engine.TileWidth)
                        newPos.X -= Engine.TileWidth;
                    if (mouse.X > mapDisplay.Width - Engine.TileWidth)
                        newPos.X += Engine.TileWidth;
                    if (mouse.Y < Engine.TileHeight)
                        newPos.Y -= Engine.TileHeight;
                    if (mouse.Y > mapDisplay.Height - Engine.TileHeight)
                        newPos.Y += Engine.TileHeight;

                    camera.Position = newPos;
                    camera.LockCamera();
                }

                /* The tile that the mouse is currently over is calculated using the static Engine.PixelToTile() function.
                 * The position checked is the camera position and the mouse position added together, which gives the mouse's coordinates on the map.
                 * After this, the shadowPos coordinates are set to the tile's coordinates. This means the tile being hovered over will be shaded. */
                Point tile = Engine.PixelToTile(new Vector2(mouse.X + camera.Position.X, mouse.Y + camera.Position.Y));
                shadowPos.X = tile.X;
                shadowPos.Y = tile.Y;

                /* The map location text box is updated with the coordinates of the tile that's being hovered over, for debugging purposes. */
                tbMapLocation.Text = "(" + tile.X.ToString() + ", " + tile.Y.ToString() + ")";

                /* If the mouseDown bit is true, then the editor does something to the tile being hovered over.
                 * Depending on which radio button is checked, a different operation happens.
                 * In most cases, rbDraw is checked, which will set all the tiles under the brush "shadow" to the currently selected tile.
                 * When rbErase is checked, the tiles under the brush "shadow" will be set with a tileset and tile index of -1, effectively making them blank.
                 * The rbSolid and rbSolidErase check boxes will set the tiles' Solid properties to true or false respectively.
                 * The rbSpawn and rbSpawnErase check boxes set the tiles' Spawn bits to true or false respectively.
                 * The SetTiles() method is used to edit every tile under the brush "shadow", instead of SetTile() which modifies one tile at a time. */
                if (mouseDown == true)
                {
                    if (rbDraw.Checked)
                    {
                        SetTiles(tile, (int)nudCurrentTile.Value, lbTileset.SelectedIndex);
                    }
                    if (rbErase.Checked)
                    {
                        SetTiles(tile, -1, -1);
                    }
                    if (rbSolid.Checked)
                    {
                        SetTilesSolid(tile, true);
                    }
                    if (rbSolidErase.Checked)
                    {
                        SetTilesSolid(tile, false);
                    }
                    if (rbSpawn.Checked)
                    {
                        SetTilesSpawn(tile, true);
                    }
                    if (rbSpawnErase.Checked)
                    {
                        SetTilesSpawn(tile, false);
                    }
                }
            }
        }

        /* The MapDraw() function is a loose equivalent to the XNA Draw() function. */
        private void MapDraw()
        {
            /* The SpriteBatch.Begin() method is called with the arguments that stop antialiasing taking place.
             * This means the map will be drawn with the tiles exactly as they should be, rather than being blended together.
             * The camera's transformation matrix is also applied to the render. */
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, camera.Transformation);

            /* Every layer that is checked in the layer check box is drawn using MapLayer.Draw().
             * The shadowPos coordinates are passed in so that the correct tile with the mouse over it is shaded.
             * The shadeShadow, shadeSolid, and shadeSpawn bits are all passed in as true as we want to see solid and spawn tiles in the editor. */
            for (int i = 0; i < layerList.Count; i++)
            {
                if (clbLayers.GetItemChecked(i))
                {
                    layerList[i].Draw(spriteBatch, camera, tilesetList, true, true, true, shadowPos.X, shadowPos.Y);
                }
            }

            /* SpriteBatch.End() is called to flush the internal draw buffer to the actual screen.
             * DrawOverlay() is called last to draw the cursor and the grid to the screen. */
            spriteBatch.End();
            DrawOverlay();
        }

        /* The SetTiles function is similar to the MapLayer.SetTile() function, except it uses the brushWidth variable to set tiles under the brush shadow.
         * It takes the Point with the tile coordinates of the mouse, and the tile index and tileset to use in the new tiles. */
        private void SetTiles(Point tile, int tileIndex, int tileset)
        {
            /* An integer variable holds the index of the the currently selected layer.
             * This is the index of the layer that will be modified by the function.
             * The idea is that when the user presses the left mouse button, the tiles under the brush shadow will change to the currently selected tile.
             * It's a bit like the pencil tool in Microsoft Paint, except it changes the tile texture instead of just changing the colour of an area. */
            int layerIndex = clbLayers.SelectedIndex;

            /* The function checks if the currently selected layer is a MapLayer. It should always be, since the InterfaceLayer is only inherited by MapLayer.
             * There are two nested for loops going between zero and the brush width/height. 
             * Note that in this case, the x and y variables are offsets to the mouse's tile position, not positions in themselves.
             * The function goes through all the tiles in the brush shadow (a square with its top left corner where the mouse is,
             * and with a width and height in tiles set by the user in the Brushes tool strip), and changes them all to the currently selected tile. */
            if (layerList[layerIndex] is MapLayer)
            {
                for (int y = 0; y < brushWidth; y++)
                {
                    /* If the current Y position (the mouse Y plus the offset Y) goes past the MapLayer's height, the loop breaks since it's hit the bottom of the level. */
                    if (tile.Y + y >= ((MapLayer)layerList[layerIndex]).Height)
                        break;
                    for (int x = 0; x < brushWidth; x++)
                    {
                        /* If the mouse's X plus the offset X is beyond the MapLayer's width, then the tile is not set, since it's past the right hand edge of the map. */
                        if (tile.X + x < ((MapLayer)layerList[layerIndex]).Width)
                            ((MapLayer)layerList[layerIndex]).SetTile(tile.X + x, tile.Y + y, tileIndex, tileset);
                    }
                }
            }
        }

        /* SetTilesSolid() works in exactly the same way as the SetTiles() function, using nested loops to change the properties of multiple tiles.
         * It sets all the tiles under the brush shadow with a certain bit as their Solid property.
         * It takes a bit denoting which value to set the Solid properties of the tiles to.
         * The only difference inside the function is that SetSolid() is used instead of SetTile() to modify each individual tile. */
        private void SetTilesSolid(Point tile, bool solid)
        {
            int layerIndex = clbLayers.SelectedIndex;
            if (layerList[layerIndex] is MapLayer)
            {
                for (int y = 0; y < brushWidth; y++)
                {
                    if (tile.Y + y >= ((MapLayer)layerList[layerIndex]).Height)
                        break;
                    for (int x = 0; x < brushWidth; x++)
                    {
                        if (tile.X + x < ((MapLayer)layerList[layerIndex]).Width)
                            ((MapLayer)layerList[layerIndex]).SetSolid(tile.X + x, tile.Y + y, solid);
                    }
                }
            }
        }

        /* SetTilesSpawn() is almost identical to the previous two functions, except is changes the Spawn property of the tiles.
         * It takes a bit for whether the Spawn properties should be set as true or false.
         * The SetSpawn() method is used to change each tile's Spawn value. */
        private void SetTilesSpawn(Point tile, bool spawn)
        {
            int layerIndex = clbLayers.SelectedIndex;
            if (layerList[layerIndex] is MapLayer)
            {
                for (int y = 0; y < brushWidth; y++)
                {
                    if (tile.Y + y >= ((MapLayer)layerList[layerIndex]).Height)
                        break;
                    for (int x = 0; x < brushWidth; x++)
                    {
                        if (tile.X + x < ((MapLayer)layerList[layerIndex]).Width)
                            ((MapLayer)layerList[layerIndex]).SetSpawn(tile.X + x, tile.Y + y, spawn);
                    }
                }
            }
        }

        /* This function draws the grid and cursor over the top of the map.
         * The cursor is a scaled preview of the currently selected tile with the top-left corner at the exact pixel position of the mouse, bordered by the cursor texture.
         * The grid is a white border texture which is drawn over each tile and can be tinted in different colours. */
        private void DrawOverlay()
        {
            /* If there's no map, there's no point drawing the overlay, so the function returns. */
            if (map == null)
                return;

            /* The dest Rectangle is set with a position of zero and the width and height of a tile.
             * If the display grid button is checked, then the grid needs to be drawn, so the next block executes. */
            Rectangle dest = new Rectangle(0, 0, Engine.TileWidth, Engine.TileHeight);
            if (displayGridToolStripMenuItem.Checked)
            {
                /* The maxGridX variable is the maximum tile X coordinate that should have the grid texture drawn over it.
                 * Similarly, maxGridY is the maximum tile Y coordinate to have the grid texture drawn over it. */
                int maxGridX = mapDisplay.Width / Engine.TileWidth;
                int maxGridY = mapDisplay.Height / Engine.TileHeight;

                /* The SpriteBatch.Begin() method initializes a memory buffer to draw the grid overlay to. */
                spriteBatch.Begin();

                /* Nested for loops iterate through every tile coordinate on the map, row by row.
                 * It goes from zero to the maximum x/y coordinate.
                 * The pixel coordinates of the dest rectangle are found by multiplying the tile coordinate by the tile dimensions.
                 * For each tile in the map, the grid texture is drawn to the buffer at the dest position, with gridColour as the tint colour.
                 * This means that the grid colour selected by the user will tint the white grid texture, thus making the grid appear to be in the selected colour. */
                for (int y = 0; y <= maxGridY; y++)
                {
                    dest.Y = y * Engine.TileHeight;
                    for (int x = 0; x <= maxGridX; x++)
                    {
                        dest.X = x * Engine.TileWidth;
                        spriteBatch.Draw(grid, dest, gridColour);
                    }
                }

                /* Finally, SpriteBatch.End() is called to send the contents of the memory buffer to the screen. */
                spriteBatch.End();
            }

            /* SpriteBatch.Begin() is called again to prepare for the cursor being drawn.
             * The coordinates of the dest rectangle are set to the pixel coordinates of the mouse. */
            spriteBatch.Begin();
            dest.X = mouse.X;
            dest.Y = mouse.Y;

            /* If the draw radio button is selected, the cursor texture is drawn with the current tile's texture, the destination rectangle, and the cursor's source rectangle in the tileset.
             * The cursor texture is drawn in the same position as the border. Neither draw operation uses a tint colour. */
            if (rbDraw.Checked)
            {
                spriteBatch.Draw(tilesetList[lbTileset.SelectedIndex].Texture, dest, tilesetList[lbTileset.SelectedIndex].SourceRectangles[(int)nudCurrentTile.Value], Color.White);
                spriteBatch.Draw(cursor, dest, Color.White);
            }

            /* SpriteBatch.End() is called again to send the drawn textures to the physical screen. */
            spriteBatch.End();
        }

        /* This function is called when the selected tileset is changed. */
        private void lbTileset_SelectedIndexChanged(object sender, EventArgs e)
        {
            /* If the list box's selected item is null, there are no tilesets, and this block won't run.
             * Otherwise, there's a valid selected tileset and the block runs. */
            if (lbTileset.SelectedItem != null)
            {
                /* The tile selector's value is reset to zero, and its maximum value is set to the length of the tileset's source rectangle array minus one.
                 * Removing one from the length of the source rectangle array accounts for the array indexes starting at zero, not one.
                 * Finally, FillPreviews() is called to place a preview of the new tileset in the tileset preview picture box, and change the tile preview. */
                nudCurrentTile.Value = 0;
                nudCurrentTile.Maximum = tilesetList[lbTileset.SelectedIndex].SourceRectangles.Length - 1;
                FillPreviews();
            }
        }

        /* This function is called when the selected tile index is changed in the NumericalUpDownSelector. */
        private void nudCurrentTile_ValueChanged(object sender, EventArgs e)
        {
            /* If there is a valid selected tileset, FillPreviews() is called to change the tileset and tile previews. */
            if (lbTileset.SelectedItem != null)
            {
                FillPreviews();
            }
        }

        /* FillPreviews() changes the tileset preview and tile preview picture boxes to match any newly selected tilesets/tiles. */
        private void FillPreviews()
        {
            /* Two variables for the current tileset and tile indexes are created and set. */
            int tilesetIndex = lbTileset.SelectedIndex;
            int tileIndex = (int)nudCurrentTile.Value;

            /* The initial image for the tile preview is generated with a width and height of the tile preview picture box.
             * The destination rectangle for the tile preview is set with the image's width and height, and a position of zero.
             * The XNA source rectangle for the new tile is set into the the tileRect field.
             * A System.Drawing.Rectangle with the source is created using the position and dimensions of tileRect.
             * A System.Drawing.Graphics object is needed to draw the image, and is created using the initial preview image.
             * Finally, the actual tile preview image is drawn to an internal buffer using the g.DrawImage(), 
             * passing in the tileset image, dest and source rectangles, and Pixel as the drawing unit.
             * The two preview image picture boxes can then be set to the current tileset image and the new tile image in the internal buffer respectively. */
            SysImage preview = new SysBitmap(pbTilePreview.Width, pbTilePreview.Height);
            SysRect dest = new SysRect(0, 0, preview.Width, preview.Height);
            Rectangle tileRect = tilesetList[tilesetIndex].SourceRectangles[tileIndex];
            SysRect source = new SysRect(tileRect.X, tileRect.Y, tileRect.Width, tileRect.Height);
            SysGraphics g = SysGraphics.FromImage(preview);
            g.DrawImage(tilesetImageList[tilesetIndex], dest, source, SysUnit.Pixel);
            pbTilesetPreview.Image = tilesetImageList[tilesetIndex];
            pbTilePreview.Image = preview;
        }

        /* This function is called when the mouse is clicked over the tileset preview image.
         * Clicking on the tileset preview image implies that the user wants to change the current tile to the one that's being clicked on. */
        private void pbTilesetPreview_MouseDown(object sender, MouseEventArgs e)
        {
            /* If there are no tilesets in the list, or the left mouse button wasn't clicked, the function returns. */
            if (lbTileset.Items.Count == 0)
                return;
            if (e.Button != MouseButtons.Left)
                return;

            /* A variable called tilesetIndex stores the currently selected index from the tileset list box.
             * Since the tileset image preview is stretched inside the picture box, some scaling needs to be done with the mouse coordinates.
             * We want the tile that is clicked on to be selected, even though it's probably squashed or stretched in the preview.
             * Two floats representing the ratio between the actual tileset image's dimensions and the picture box's dimensions are calculated.
             * For example, if the tileset was 100x75 and the picture box was 25x25, the xScale would be 4 and the yScale would be 3. */
            int tilesetIndex = lbTileset.SelectedIndex;
            float xScale = (float)tilesetImageList[tilesetIndex].Width / pbTilesetPreview.Width;
            float yScale = (float)tilesetImageList[tilesetIndex].Height / pbTilesetPreview.Height;

            /* First, the coordinates of the mouse on the preview picture box, obtained through the MouseEventArgs, are set into previewPoint.
             * Next, another Point representing the corresponding mouse coordinates on the actual tileset is calculated by multiplying the previewPoint
             * coordinates by the scale factors and saving them into another Point named tilesetPoint.
             * Finally, the Point containing the tile coordinates of the mouse on the tileset is found by dividing the tilesetPoint coordinates by the tile width/height.
             * Now we know the coordinates of the tile on the tileset, the appropriate tile index is easily worked out.
             * The tile index is the Y coordinate of the tile multiplied by the number of tiles in each row, and then add the X coordinate.
             * The current tile NumericalUpDownSelector is set with the new tile index. */
            Point previewPoint = new Point(e.X, e.Y);
            Point tilesetPoint = new Point((int)(previewPoint.X * xScale), (int)(previewPoint.Y * yScale));
            Point tile = new Point(tilesetPoint.X / Engine.TileWidth, tilesetPoint.Y / Engine.TileHeight);
            nudCurrentTile.Value = tile.Y * tilesetList[tilesetIndex].TilesWide + tile.X;
        }

        /* This function is called when the mapDisplay's size is changed.
         * As the size of the other controls in the window are fixed, this is essentially called when the window is resized.
         * It's mildly bugged because one can make the window bigger with no problems, but when the window is made smaller, tile scaling goes very odd. */
        private void mapDisplay_SizeChanged(object sender, EventArgs e)
        {
            /* A new rectangle with the size of the new mapDisplay is created.
             * A Vector2 variable stores the original camera position.
             * The camera is reconstructed using the new Rectangle and the original position with the same zoom.
             * The camera is locked so it doesn't go off the map, and the mapDisplay is invalidated so it gets redrawn. */
            Rectangle viewPort = new Rectangle(0, 0, mapDisplay.Width, mapDisplay.Height);
            Vector2 camPos = camera.Position;
            camera = new Camera(viewPort, camPos, 1f, null);
            camera.LockCamera();
            mapDisplay.Invalidate();
        }

        /* The following four methods are all very similar. Each one is invoked when one of the brush size buttons is clicked.
         * In each case, the clicked item is checked and all the other brush sizes are unchecked.
         * The brushWidth is then set to the width that was clicked on. */
        private void x1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            x1ToolStripMenuItem.Checked = true;
            x2ToolStripMenuItem.Checked = false;
            x4ToolStripMenuItem.Checked = false;
            x8ToolStripMenuItem.Checked = false;
            brushWidth = 1;
        }
        private void x2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            x1ToolStripMenuItem.Checked = false;
            x2ToolStripMenuItem.Checked = true;
            x4ToolStripMenuItem.Checked = false;
            x8ToolStripMenuItem.Checked = false;
            brushWidth = 2;
        }
        private void x4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            x1ToolStripMenuItem.Checked = false;
            x2ToolStripMenuItem.Checked = false;
            x4ToolStripMenuItem.Checked = true;
            x8ToolStripMenuItem.Checked = false;
            brushWidth = 4;
        }
        private void x8ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            x1ToolStripMenuItem.Checked = false;
            x2ToolStripMenuItem.Checked = false;
            x4ToolStripMenuItem.Checked = false;
            x8ToolStripMenuItem.Checked = true;
            brushWidth = 8;
        }

        /* Similarly, these last six methods change the tint colour of the grid texture.
         * In each case, when a grid colour button is pressed, its menu item is checked and all other grid colour buttons are unchecked.
         * The gridColour is then set to a colour matching the one that was clicked on. */
        private void whiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gridColour = Color.White;
            whiteToolStripMenuItem.Checked = true;
            redToolStripMenuItem.Checked = false;
            blueToolStripMenuItem.Checked = false;
            greenToolStripMenuItem.Checked = false;
            yellowToolStripMenuItem.Checked = false;
            blackToolStripMenuItem.Checked = false;
        }
        private void redToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gridColour = Color.Red;
            whiteToolStripMenuItem.Checked = false;
            redToolStripMenuItem.Checked = true;
            blueToolStripMenuItem.Checked = false;
            greenToolStripMenuItem.Checked = false;
            yellowToolStripMenuItem.Checked = false;
            blackToolStripMenuItem.Checked = false;
        }
        private void blueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gridColour = Color.Blue;
            whiteToolStripMenuItem.Checked = false;
            redToolStripMenuItem.Checked = false;
            blueToolStripMenuItem.Checked = true;
            greenToolStripMenuItem.Checked = false;
            yellowToolStripMenuItem.Checked = false;
            blackToolStripMenuItem.Checked = false;
        }
        private void greenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gridColour = Color.Green;
            whiteToolStripMenuItem.Checked = false;
            redToolStripMenuItem.Checked = false;
            blueToolStripMenuItem.Checked = false;
            greenToolStripMenuItem.Checked = true;
            yellowToolStripMenuItem.Checked = false;
            blackToolStripMenuItem.Checked = false;
        }
        private void yellowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gridColour = Color.Yellow;
            whiteToolStripMenuItem.Checked = false;
            redToolStripMenuItem.Checked = false;
            blueToolStripMenuItem.Checked = false;
            greenToolStripMenuItem.Checked = false;
            yellowToolStripMenuItem.Checked = true;
            blackToolStripMenuItem.Checked = false;
        }
        private void blackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gridColour = Color.Black;
            whiteToolStripMenuItem.Checked = false;
            redToolStripMenuItem.Checked = false;
            blueToolStripMenuItem.Checked = false;
            greenToolStripMenuItem.Checked = false;
            yellowToolStripMenuItem.Checked = false;
            blackToolStripMenuItem.Checked = true;
        }
    }
}  