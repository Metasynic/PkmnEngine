using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MGPkmnLibrary.SpriteClasses;

namespace MGPkmnLibrary.ItemClasses
{
    /* This class combines a BaseItem with a BaseSprite.
     * The combination represents a BaseItem being displayed in the world or in the inventory. */
    public class ItemSprite
    {
        /* These two fields are the BaseItem and BaseSprite being combined to form the ItemSprite. */
        BaseSprite sprite;
        BaseItem item;
        public BaseSprite Sprite
        {
            get { return sprite; }
        }
        public BaseItem Item
        {
            get { return item; }
        }

        /* The constructor takes the sprite and item and sets them in the ItemSprite. */
        public ItemSprite(BaseSprite sprite, BaseItem item)
        {
            this.sprite = sprite;
            this.item = item;
        }

        /* The Update() function calls the Update() function of the sprite only, since the item doesn't need updating. */
        public virtual void Update(GameTime gameTime)
        {
            sprite.Update(gameTime);
        }

        /* The Draw() function draws the sprite to the screen using BaseSprite.Draw(). */
        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            sprite.Draw(gameTime, spriteBatch);
        }
    }
}
