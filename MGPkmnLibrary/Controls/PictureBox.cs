using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MGPkmnLibrary.Controls
{
    /* This is a simple Control to display a texture to the screen.
     * It's quite useful because you can change the source rectangle in the texture.
     * Changing the source rectangle is implemented in the InventoryScreen, 
     * where it's used to display different sprites from the same sheet in the same PictureBox. */
    public class PictureBox : Control
    {
        /* This is the main Texture that acts as the source for the picture.
         * The sourceRect Rectangle represents the area of the texture that is drawn.
         * The destRect Rectangle represents the area of the screen that is drawn to. */
        Texture2D image;
        Rectangle sourceRect;
        Rectangle destRect;
        public Texture2D Image
        {
            get { return image; }
            set { image = value; }
        }
        public Rectangle SourceRectangle
        {
            get { return sourceRect; }
            set { sourceRect = value; }
        }
        public Rectangle DestinationRectangle
        {
            get { return destRect; }
            set { destRect = value; }
        }

        /* The constructor for a PictureBox takes the Texture2D of its image, and its source and destination Rectangles.
         * The default colour is set to White, which means the image will not be tinted. */
        public PictureBox(Texture2D image, Rectangle source, Rectangle destination)
        {
            Image = image;
            SourceRectangle = source;
            DestinationRectangle = destination;
            Color = Color.White;
        }

        /* There is also an overload where no source Rectangle is taken.
         * The source Rectangle defaults to using the entire image as a source.
         * This is useful if the whole image should be displayed in the PictureBox. */
        public PictureBox(Texture2D image, Rectangle destination)
        {
            Image = image;
            DestinationRectangle = destination;
            SourceRectangle = new Rectangle(0, 0, image.Width, image.Height);
            Color = Color.White;
        }

        /* No updating is necessary for a PictureBox. */
        public override void Update(GameTime gameTime)
        {
            
        }

        /* The Draw() function uses SpriteBatch.Draw(), passing in the image, source and destination Rectangles, and tint colour. */
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(image, destRect, sourceRect, color);
        }

        /* No input is needed for a PictureBox. */
        public override void HandleInput(PlayerIndex playerIndex)
        {
            
        }

        /* This function is a neat way of changing the position of a PictureBox on the screen.
         * It is used to change the location of the arrow image in the StartScreen and BattleScreen.
         * The destination Rectangle is set to a new Rectangle matching the coordinates in the Vector2 passed in. */
        public void SetPosition(Vector2 newPosition)
        {
            destRect = new Rectangle((int)newPosition.X, (int)newPosition.Y, sourceRect.Width, sourceRect.Height);
        }
    }
}
