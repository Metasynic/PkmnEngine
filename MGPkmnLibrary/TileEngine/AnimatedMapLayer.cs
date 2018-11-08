using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MGPkmnLibrary.TileEngine
{
    /* This unfinished class represents a layer containing all the animated tiles on the map. */
    public class AnimatedMapLayer
    {
        /* This Dictionary contains every animated tile in the layer. The key is the coordinates of the tile.
         * The Dictionary needs to be serialized when levels are saved, so it's marked with a [ContentSerializer]. */
        private Dictionary<Point, AnimatedTile> animatedTiles = new Dictionary<Point, AnimatedTile>();
        [ContentSerializer]
        public Dictionary<Point, AnimatedTile> AnimatedTiles
        {
            get { return animatedTiles; }
            private set { animatedTiles = value; }
        }

        /* The constructor does nothing yet. The set of AnimatedTiles is set separately at the moment. */
        public AnimatedMapLayer()
        {

        }

        /* The Update() function just updates every tile in the Dictionary. */
        public void Update(GameTime gameTime)
        {
            foreach(Point p in animatedTiles.Keys)
            {
                AnimatedTiles[p].Update(gameTime);
            }
        }

        /* The Draw() function takes an AnimatedTileset as a parameter and uses it to draw each tile in the list. */
        public void Draw(SpriteBatch spriteBatch, AnimatedTileset tileset)
        {
            /* First, the destination Rectangle is initialized with no position and the height and width of a tile. */
            Rectangle dest = new Rectangle(0, 0, Engine.TileWidth, Engine.TileHeight);
            foreach(Point p in animatedTiles.Keys)
            {
                /* For each tile in the list, the destination is set to the tile coordinates multiplied by the tile width/height. */
                dest.X = p.X * Engine.TileWidth;
                dest.Y = p.Y * Engine.TileHeight;

                /* Finally, the image is drawn using the source frame from the AnimatedTileset, and no colour tint. */
                spriteBatch.Draw(tileset.Image, dest, tileset.SourceFrames[animatedTiles[p].TileIndex][animatedTiles[p].CurrentFrame], Color.White);
            }
        }
    }
}
