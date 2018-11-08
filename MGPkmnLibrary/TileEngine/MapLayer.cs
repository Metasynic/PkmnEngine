using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MGPkmnLibrary.WorldClasses;

namespace MGPkmnLibrary.TileEngine
{
    /* This class represents a layer of tiles on the map.
     * Its central component is a two-dimensional array of Tile objects. */
    public class MapLayer : InterfaceLayer
    {
        /* The map field stores the two-dimensional array of Tiles that make up the map. */
        Tile[,] map;

        /* The Width and Height properties represent the Width and Height in tiles of the map.
         * They return the length of the inner arrays, and the length of the outer array respectively. */
        public int Width
        {
            get { return map.GetLength(1); }
        }
        public int Height
        {
            get { return map.GetLength(0); }
        }

        /* GetTile() and SetTile() return or set a tile with the x and y coordinates of the tile passed in.
         * SetTile() has another overload that takes a tileIndex and tileset value which can be used to make a new Tile,
         * instead of passing in an already existing Tile. */
        public Tile GetTile(int x, int y)
        {
            return map[y, x];
        }
        public void SetTile(int x, int y, Tile tile)
        {
            map[y, x] = tile;
        }
        public void SetTile(int x, int y, int tileIndex, int tileset)
        {
            try
            {
                map[y, x] = new Tile(tileIndex, tileset);
            }
            catch (Exception exc)
            {
                /* Error handling in case something goes wrong making the new Tile and adding it to the map. */
                throw new Exception(exc.Message);
            }
        }

        /* These two functions set a tile's Tile.Solid or Tile.Spawn property according to the bit passed in.
         * The Solid and Spawn properties are explained in the Tile class. */
        public void SetSolid(int x, int y, bool tileSolid)
        {
            map[y, x].Solid = tileSolid;
        }
        public void SetSpawn(int x, int y, bool tileSpawn)
        {
            map[y, x].Spawn = tileSpawn;
        }
        
        /* The first constructor creates a MapLayer using an existing two-dimensional array of Tiles.
         * It is not currently used, but it might be useful at some point. */
        public MapLayer(Tile[,] map)
        {
            this.map = (Tile[,])map.Clone();
        }

        /* The other overload is the main constructor, and takes the width and height of the map.
         * The constructor initializes the outer array with the height of the map, and the inner arrays with the width.
         * This means each inner array is one row of the map.
         * After this, each Tile in the array is set to tileset zero, tileIndex zero (null). */
        public MapLayer(int width, int height)
        {
            map = new Tile[height, width];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    map[y, x] = new Tile(0, 0);
                }
            }
        }
        
        /* A MapLayer does not update itself, so the Update() function is empty. */
        public void Update(GameTime gameTime)
        {

        }

        /* There are two overloads for the Draw() method. Both overloads are almost identical.
         * The first overload is for drawing the MapLayer to the screen in the regular PkmnEngine.
         * The second overload is for drawing the MapLayer in the Level Editor, and differences will be explained below. */
        public void Draw(SpriteBatch spriteBatch, Camera camera, List<Tileset> tilesets)
        {
            /* First, a Point is generated containing the tile coordinates of the top left of the camera.
             * Then, another Point is generated containing the tile coordinates of the bottom right of the camera.
             * The function checks whether topLeft's X or Y values are above or equal to zero.
             * Whichever value is bigger out of topLeft's coordinates or zero are added to min.
             * Whichever value is smaller out of bottomRight's coordinates and the dimensions of the layer are added to max.
             * This ensures that the function will not draw outside the MapLayer, even if Engine.PixelToTile() returns values outside the MapLayer. */
            Point topLeft = Engine.PixelToTile(camera.Position * (1 / camera.Zoom));
            Point bottomRight = Engine.PixelToTile(new Vector2((camera.Position.X + camera.ViewportRectangle.Width) * (1 / camera.Zoom), (camera.Position.Y + camera.ViewportRectangle.Height) * (1 / camera.Zoom)));
            Point min = new Point();
            Point max = new Point();
            min.X = Math.Max(0, topLeft.X - 1);
            min.Y = Math.Max(0, topLeft.Y - 1);
            max.X = Math.Min(bottomRight.X + 1, Width);
            max.Y = Math.Min(bottomRight.Y + 1, Height);

            /* A new Rectangle is created with zero position, and the width and height of a tile.
             * A new Tile is also initialized to be used in the nested for loops below. */
            Rectangle destination = new Rectangle(0, 0, Engine.TileWidth, Engine.TileHeight);
            Tile tile;
            for (int y = min.Y; y < max.Y; y++)
            {
                /* The nested for loop goes between the Y values of min and max, and the X values of min and max.
                 * This means that the visible area of the map gets drawn left to right, top to bottom (the same way as you read a book).
                 * The Y coordinate of the destination Rectangle is obtained by multiplying y by the tile height.
                 * The X coordinate of the destination Rectangle comes from multiplying x by the tile width. */
                destination.Y = y * Engine.TileHeight;
                for (int x = min.X; x < max.X; x++)
                {
                    /* The current tile is checked to see if its Tileset or TileIndex properties are -1.
                     * If they are, the tile is null and should not be drawn, and the loop continues without drawing the tile. */
                    tile = GetTile(x, y);
                    if (tile.TileIndex == -1 || tile.Tileset == -1)
                        continue;
                    destination.X = x * Engine.TileWidth;

                    /* Finally, the tile is drawn to the screen. The source texture is the tileset Texture (obtained using the tileset index field of the tile).
                     * The source Rectangle for the tile is found using the tileIndex field to find it in the tileset's source rectangles array.
                     * The tint colour is white as no tint colour should be applied. */
                    spriteBatch.Draw(tilesets[tile.Tileset].Texture, destination, tilesets[tile.Tileset].SourceRectangles[tile.TileIndex], Color.White);
                }
            }
        }

        /* This is the overload used in the Level Editor. It works in exactly the same way as the first overload,
         * except it takes extra parameters for the location of the mouse, and whether shadow/solid/spawn tiles should be tinted.
         * The shadeShadow parameter represents whether the tile will be shaded grey. This should happen if the mouse is over it.
         * The shadeSolid and shadeSpawn parameters decide whether tiles with Solid/Spawn set to true should be shaded Orange/Green respectively. */
        public void Draw(SpriteBatch spriteBatch, Camera camera, List<Tileset> tilesets, bool shadeShadow, bool shadeSolid, bool shadeSpawn, int mouseX, int mouseY)
        {
            /* This is all exactly the same as the previoius overload. */
            Point topLeft = Engine.PixelToTile(camera.Position * (1 / camera.Zoom));
            Point bottomRight = Engine.PixelToTile(new Vector2((camera.Position.X + camera.ViewportRectangle.Width) * (1 / camera.Zoom), (camera.Position.Y + camera.ViewportRectangle.Height) * (1 / camera.Zoom)));
            Point min = new Point();
            Point max = new Point();
            min.X = Math.Max(0, topLeft.X - 1);
            min.Y = Math.Max(0, topLeft.Y - 1);
            max.X = Math.Min(bottomRight.X + 1, Width);
            max.Y = Math.Min(bottomRight.Y + 1, Height);
            Rectangle destination = new Rectangle(0, 0, Engine.TileWidth, Engine.TileHeight);
            Tile tile;
            for (int y = min.Y; y < max.Y; y++)
            {
                destination.Y = y * Engine.TileHeight;
                for (int x = min.X; x < max.X; x++)
                {
                    tile = GetTile(x, y);
                    if (tile.TileIndex == -1 || tile.Tileset == -1)
                        continue;
                    destination.X = x * Engine.TileWidth;

                    /* The only difference between the overloads is in the following block.
                     * The tint colour changes depending on the parameters given to the function.
                     * If shadeShadow is true, and the mouse position matches the tile position,
                     * the tile is drawn with a LightSteelBlue tint colour.
                     * If shadeSolid is true, and the tile's Solid property is true, the tile is tinted orange.
                     * If shadeSpawn is true, and the tile's Spawn property is true, the tile is tinted dark green.
                     * If none of these conditions are fulfilled, the tile is drawn as normal with no tint colour. */
                    if (shadeShadow && x == mouseX && y == mouseY)
                        spriteBatch.Draw(tilesets[tile.Tileset].Texture, destination, tilesets[tile.Tileset].SourceRectangles[tile.TileIndex], Color.LightSteelBlue);
                    else if (shadeSolid && tile.Solid)
                        spriteBatch.Draw(tilesets[tile.Tileset].Texture, destination, tilesets[tile.Tileset].SourceRectangles[tile.TileIndex], Color.Orange);
                    else if (shadeSpawn && tile.Spawn)
                        spriteBatch.Draw(tilesets[tile.Tileset].Texture, destination, tilesets[tile.Tileset].SourceRectangles[tile.TileIndex], Color.DarkGreen);
                    else
                        spriteBatch.Draw(tilesets[tile.Tileset].Texture, destination, tilesets[tile.Tileset].SourceRectangles[tile.TileIndex], Color.White);
                }
            }
        }

        /* This function converts a MapLayerData object (created in the Level Editor) into a MapLayer. */
        public static MapLayer FromMapLayerData(MapLayerData data)
        {
            /* First, a new MapLayer is created using the width and height of the MapLayerData.
             * Then, for each Tile in the MapLayerData, the tile is copied from the MapLayerData to the MapLayer.
             * The TileIndex, TilesetIndex, Solid, and Spawn properties are copied separately using the relevant methods. */
            MapLayer layer = new MapLayer(data.Width, data.Height);
            for (int y = 0; y < data.Height; y++)
            {
                for (int x = 0; x < data.Width; x++)
                {
                    layer.SetTile(x, y, data.GetTile(x, y).TileIndex, data.GetTile(x, y).TilesetIndex);
                    layer.SetSolid(x, y, data.GetTile(x, y).Solid);
                    layer.SetSpawn(x, y, data.GetTile(x, y).Spawn);
                }
            }

            /* Finally, the newly created layer is returned. */
            return layer;
        }
    }
}
