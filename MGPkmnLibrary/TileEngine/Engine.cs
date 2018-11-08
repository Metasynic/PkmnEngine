using Microsoft.Xna.Framework;

namespace MGPkmnLibrary.TileEngine
{
    /* The Engine is a small class containing static values for a tile width and height.
     * By setting them here, the tile width and height values are kept consistent across the game. */
    public class Engine
    {
        /* These two fields represent the universal width/height in pixels of a tile throughout the game. */
        static int tileWidth;
        static int tileHeight;
        public static int TileWidth
        {
            get { return tileWidth; }
            set { tileWidth = value; }
        }
        public static int TileHeight
        {
            get { return tileHeight; }
            set { tileHeight = value; }
        }

        /* The constructor just takes the tile width and height and sets them. */
        public Engine(int tileWidth, int tileHeight)
        {
            Engine.tileWidth = tileWidth;
            Engine.tileHeight = tileHeight;
        }

        /* This function takes a Vector2 and converts it into a point representing the tile coordinates of the Vector2.
         * It does this by dividing the position's coordinates by the tile width and height.
         * For example, a Vector2 of (20, 36) would be converted into a point of (1, 2) with a tile height/width of 16 each. */
        public static Point PixelToTile(Vector2 position)
        {
            return new Point((int)position.X / tileWidth, (int)position.Y / tileHeight);
        }
    }
}
