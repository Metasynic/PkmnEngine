using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MGPkmnLibrary.Controls
{
    /* A Label is a basic type of control which represents a string of text drawn on the screen. */
    public class Label : Control
    {
        /* Labels have tabStop set to false by default in the constructor, as they shouldn't be selectable. 
         * They're just there to display text. */
        public Label()
        {
            tabStop = false;
        }

        /* Labels do not need updating, as they have no logical components that change per frame.
         * As a result there's nothing in the Update() function. */
        public override void Update(GameTime gameTime)
        {

        }

        /* The label itself is drawn using the SpriteBatch.DrawString() method, which takes a font, text string, Vector2 as a position, and a colour.
         * The font, text, position, and colour are all defined in the parent Control class. */
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(SpriteFont, Text, Position, Color);
        }

        /* Labels also do not require input from the user since they're just text boxes.
         * Therefore HandleInput() is also empty. */
        public override void HandleInput(PlayerIndex playerIndex)
        {

        }

    }
}
