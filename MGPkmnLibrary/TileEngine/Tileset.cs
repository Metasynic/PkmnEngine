using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MGPkmnLibrary.TileEngine
{
    /* A Tileset is a sprite sheet image with a list of source rectangles representing the locations of the sprites on the sheet. */
    public class Tileset
    {
        /* The image field represents the sprite sheet texture.
         * The tileWidthInPixels and tileHeightInPixels fields store the dimensions of a single tile.
         * These fields are used as the width and height of all the source rectangles.
         * The other two integer fields, tilesWide and tilesHigh, are the width and height in tiles of the sprite sheet.
         * Finally, the sourceRectangles array stores one Rectangle object with the location of each sprite on the sheet. */
        Texture2D image;
        int tileWidthInPixels;
        int tileHeightInPixels;
        int tilesWide;
        int tilesHigh;
        Rectangle[] sourceRectangles;

        public Texture2D Texture
        {
            get { return image; }
            set { image = value; }
        }
        public int TileWidth
        {
            get { return tileWidthInPixels; }
            set { tileWidthInPixels = value; }
        }
        public int TileHeight
        {
            get { return tileHeightInPixels; }
            set { tileHeightInPixels = value; }
        }
        public int TilesWide
        {
            get { return tilesWide; }
            set { tilesWide = value; }
        }
        public int TilesHigh
        {
            get { return tilesHigh; }
            set { tilesHigh = value; }
        }
        public Rectangle[] SourceRectangles
        {
            get { return (Rectangle[])sourceRectangles.Clone(); }
        }

        /* The Tileset constructor takes the Texture2D of the sprite sheet and the four integer fields.
         * All five are set directly according to what was passed in. */
        public Tileset(Texture2D image, int tilesWide, int tilesHigh, int tileWidth, int tileHeight)
        {
            Texture = image;
            TilesWide = tilesWide;
            TilesHigh = tilesHigh;
            TileWidth = tileWidth;
            TileHeight = tileHeight;

            /* Next, the required length of the sourceRectangles array is worked out by multiplying the tilesWide by tilesHigh.
             * The sourceRectangles array is initialized with the new length. */
            int length = tilesWide * tilesHigh;
            sourceRectangles = new Rectangle[length];

            /* To set all the Rectangles in the array, nested for loops interate through the number of tiles high and wide.
             * The index field is used to keep track of where the function is in the array, and increments after each rectangle is set.
             * To set a source rectangle, the x and y values (the tile coordinates of the tile on the sheet) are multiplied by the tile width/height.
             * The width and height of the rectangles are the same as the tile width/height. */
            int index = 0;
            for (int y = 0; y < tilesHigh; y++)
            {
                for (int x = 0; x < tilesWide; x++)
                {
                    sourceRectangles[index] = new Rectangle(x * tileWidth, y * tileHeight, tileWidth, tileHeight);
                    index++;
                }
            }
        }
    }
}
