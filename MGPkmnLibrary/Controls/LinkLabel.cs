using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MGPkmnLibrary.Controls
{
    /* A LinkLabel is the same as a Label, except that it uses the OnSelected() event handler from the parent class.
     * The LinkLabel triggers an event when enter is pressed while it has focus. */
    public class LinkLabel : Control
    {
        /* When selected, the LinkLabel will be red. Otherwise it's white. */
        Color selectedColor = Color.Red;
        public Color SelectedColor
        {
            get { return selectedColor; }
            set { selectedColor = value; }
        }

        /* By default, a LinkLabel has TabStop set to true as it should be possible for it to take input.
         * HasFocus is initially false, and the default position is zero. */
        public LinkLabel()
        {
            TabStop = true;
            HasFocus = false;
            Position = Vector2.Zero;
        }

        /* Similarly to the Label class, LinkLabel does not need updating independently of input, so Update() is empty. */
        public override void Update(GameTime gameTime)
        {

        }

        /* The LinkLabel is drawn in exactly the same way as a Label, except that if it's selected, it's drawn in red. */
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (hasFocus)
                spriteBatch.DrawString(SpriteFont, Text, Position, selectedColor);
            else
                spriteBatch.DrawString(SpriteFont, Text, Position, Color);
        }

        /* The HandleInput() function checks if HasFocus is true for the LinkLabel.
         * If it is, then the function checks if Enter has been pressed. If it has, then the OnSelected() function is called. */
        public override void HandleInput(PlayerIndex playerIndex)
        {
            if (!HasFocus)
                return;
            if (InputHandler.KeyReleased(Keys.Enter) || InputHandler.ButtonReleased(Buttons.A, playerIndex))
                base.OnSelected(null);
        }
    }
}
