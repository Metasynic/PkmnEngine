using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MGPkmnLibrary.TileEngine
{
    /* This unfinished class represents a tile set providing sprites for one animation per row. */
    public class AnimatedTileset
    {
        /* There are six fields in an AnimatedTileset.
         * Image is a Texture2D field for the sprite sheet.
         * TileWidthInPixels and TileHeightInPixels do what they say on the tin.
         * FrameCount is the number of frames on each row.
         * TilesHigh is the number of animations on the tileset.
         * SourceFrames a list of arrays of Rectangles. Each array contains animations for one AnimatedTile. */
        Texture2D image;
        int tileWidthInPixels;
        int tileHeightInPixels;
        int frameCount;
        int tilesHigh;
        List<Rectangle[]> sourceFrames = new List<Rectangle[]>();
        public Texture2D Image
        {
            get { return image; }
            private set { image = value; }
        }
        public int TileWidth
        {
            get { return tileWidthInPixels; }
            private set { tileWidthInPixels = value; }
        }
        public int TileHeight
        {
            get { return tileHeightInPixels; }
            private set { tileHeightInPixels = value; }
        }
        public int FrameCount
        {
            get { return frameCount; }
            private set { frameCount = value; }
        }
        public int TilesHigh
        {
            get { return tilesHigh; }
            private set { tilesHigh = value; }
        }
        public List<Rectangle[]> SourceFrames
        {
            get { return sourceFrames; }
        }

        /* The AnimatedTileset constructor takes the texture, frame count (columns), tile height (rows), tile height and tile width. */
        public AnimatedTileset(Texture2D image, int frameCount, int tilesHigh, int tileWidth, int tileHeight)
        {
            /* All the fields except the frame array are set directly. */
            this.image = image;
            this.frameCount = frameCount;
            tileWidthInPixels = tileWidth;
            tileHeightInPixels = tileHeight;
            this.tilesHigh = tilesHigh;
            
            /* The frame arrays are created by finding a source rectangle for each tile on the tileset. */
            for (int y = 0; y < tilesHigh; y++)
            {
                /* One array is made for each row in the tileset.
                 * The array is filled with Rectangles using x and y as offsets when multiplied by the tile width/height.
                 * For example, if x was 3 and y was 2, and the tile width/height was 16,
                 * the Rectangle's position would be (48, 32). */
                Rectangle[] frames = new Rectangle[frameCount];
                for (int x = 0; x < frameCount; x++)
                {
                    frames[x] = new Rectangle(x * tileWidth, y * tileHeight, tileWidth, tileHeight);
                }
                sourceFrames.Add(frames);
            }
        }
    }
}
