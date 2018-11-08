using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MGPkmnLibrary.TileEngine
{
    /* This Interface is inherited by MapLayers. It contains a template for the Update() function.
     * It also contains templates for two overloads for Draw(), which are explained in the MapLayer class. */
    public interface InterfaceLayer
    {
        void Update(GameTime gameTime);
        void Draw(SpriteBatch spriteBatch, Camera camera, List<Tileset> tilesets);
        void Draw(SpriteBatch spriteBatch, Camera camera, List<Tileset> tilesets, bool shadeShadow, bool shadeSolid, bool shadeSpawn, int mouseX, int mouseY);
    }
}