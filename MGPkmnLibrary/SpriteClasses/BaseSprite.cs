using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MGPkmnLibrary.TileEngine;

namespace MGPkmnLibrary.SpriteClasses
{
    /* A BaseSprite is a sprite that stays the same and has no animation cycle. */
    public class BaseSprite
    {
        /* The texture field contains the source image for the sprite.
         * The sourceRectangle field stores the rectangle containing the sprite on the sheet.
         * Position is a Vector2 with the pixel coordinates of the sprite.
         * Velocity is a Vector2 describing which direction the sprite is moving. The setter for Velocity normalizes it.
         * Speed is currently set to 2.0 by default, which is arbitrary and needs changing.
         * The setter for speed clamps the value between 1 and 16. */
        protected Texture2D texture;
        protected Rectangle sourceRectangle;
        protected Vector2 position;
        protected Vector2 velocity;
        protected float speed = 2.0f;
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        public Vector2 Velocity
        {
            get { return velocity; }
            set
            {
                velocity = value;
                if (velocity != Vector2.Zero)
                    velocity.Normalize();
            }
        }
        public float Speed
        {
            get { return speed; }
            set { speed = MathHelper.Clamp(speed, 1.0f, 16f); }
        }

        /* The width and height properties return the width and height of the sprite's source rectangle. */
        public int Width
        {
            get { return sourceRectangle.Width; }
        }
        public int Height
        {
            get { return sourceRectangle.Height; }
        }

        /* The Rectangle property returns a Rectangle describing the sprite's location on the screen. */
        public Rectangle Rectangle
        {
            get { return new Rectangle((int)position.X, (int)position.Y, Width, Height); }
        }

        /* The BaseSprite constructor only needs two parameters: a source image, and possibly a source rectangle. */
        public BaseSprite(Texture2D image, Rectangle? sourceRectangle)
        {
            /* Firstly, the texture is set using the one passed in. */
            texture = image;

            /* The sourceRectangle parameter is nullable. If something is passed in, it's set correctly.
             * If the sourceRectangle is null, a default Rectangle at position (0,0) is created.
             * This means that if no sourceRectangle is passed in, the sprite will use the whole of the image. */
            if (sourceRectangle.HasValue)
            {
                this.sourceRectangle = sourceRectangle.Value;
            }
            else
            {
                this.sourceRectangle = new Rectangle(0, 0, image.Width, image.Height);
            }

            /* The position and velocity default to zero. */
            this.position = Vector2.Zero;
            this.velocity = Vector2.Zero;
        }
        
        /* This overload for the constructor takes a tile location and sets the position according to that location.
         * An initialized Engine is needed to multiply the tile coordinates by the pixel width/height of the tiles. */
        public BaseSprite(Texture2D image, Rectangle? sourceRectangle, Point tile) : this(image, sourceRectangle)
        {
            this.position = new Vector2(tile.X * Engine.TileWidth, tile.Y * Engine.TileHeight);
        }

        /* Since a BaseSprite is static and does not move on its own, or change animation, the Update() function is empty. */
        public virtual void Update (GameTime gameTime)
        {

        }

        /* The Draw() function creates a new destination rectangle to match the width and height of a tile.
         * It then uses SpriteBatch.Draw() to draw the texture to the screen using the source and destination Rectangles, and no tint colour. */
        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Rectangle destRect = new Rectangle((int)position.X, (int)position.Y, Engine.TileWidth, Engine.TileHeight);
            spriteBatch.Draw(texture, destRect, sourceRectangle, Color.White);
        }
    }
}
