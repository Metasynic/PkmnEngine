using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MGPkmnLibrary.ItemClasses
{
    /* A GameItem is essentially a BaseItem combined with an image on the screen.
     * It is an object representing an item that can be drawn. */
    public class GameItem
    {
        /* The GameItem has five fields.
         * The position Vector2 decribes the coordinates of the item on the screen. *
         * Image stores the Texture associated with the item.
         * The nullable Rectangle sourceRectangle is the area of the texture that is drawn.
         * If sourceRectangle is null, then the whole texture will be drawn to the screen.
         * There is a BaseItem field which stores the actual item represented on the screen.
         * Finally, the type field indicates the type of BaseItem being drawn, such as Berry or Mega Stone. */
        public Vector2 position;
        private Texture2D image;
        private Rectangle? sourceRectangle;
        private readonly BaseItem baseItem;
        private Type type;
        
        public Texture2D Image
        {
            get { return image; }
        }
        public Rectangle? SourceRectangle
        {
            get { return sourceRectangle; }
            set { sourceRectangle = value; }
        }
        public BaseItem Item
        {
            get { return baseItem; }
        }
        public Type Type
        {
            get { return type; }
        }

        /* The GameItem constructor takes the BaseItem, the texture, and a nullable source rectangle.
         * It sets the type field to the type of the BaseItem passed in. */
        public GameItem(BaseItem item, Texture2D texture, Rectangle? source)
        {
            baseItem = item;
            image = texture;
            sourceRectangle = source;
            type = item.GetType();
        }

        /* The Draw() function draws the image to the screen at the position Vector2,
         * using the sourceRectangle if there is one, and using no colour tint. */
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(image, position, sourceRectangle, Color.White);
        }
    }
}
