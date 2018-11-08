using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace MGPkmnLibrary.Controls
{
    /* This is a control which allows the selection of one string from a list.
     * The user can change the currently chosen item with the left and right arrow keys.
     * It's used in the engine to select the player character, starter, and scroll between Pokemon in the inventory.
     * It consists of a string of text and two textures on the left and right of the text. */
    public class LeftRightSelector : Control
    {
        /* This EventHandler is fired when the selected item in the LeftRightSelector is changed.
         * In other words, it's indirectly called when someone hits the left or right arrow key. */
        public event EventHandler SelectionChanged;

        /* This is the list holding the different available items in the selector. */
        List<string> items = new List<string>();

        /* These three textures represent the left arrow to be drawn to the left of the selected text,
         * The right arrow to be drawn to the right of the selected text,
         * And the stop bar to be drawn when the user cannot scroll any further left or right. */
        Texture2D leftTexture;
        Texture2D rightTexture;
        Texture2D stopTexture;

        /* The currently selected text will be rendered in red so as to make it obvious when the LeftRightSelector is receiving input. */
        Color selectedColor = Color.Red;

        /* The maxItemWidth int represents the distance between the two arrows on either side of the text.
         * Basically it's how much room there is to draw the text.
         * The selectedItem stores the index of the current position in the list.
         * When the selectedItem field is exposed, its setter makes sure the value is valid for the list. */
        int maxItemWidth;
        int selectedItem;
        public Color SelectedColor
        {
            get { return selectedColor; }
            set { selectedColor = value; }
        }
        public int SelectedIndex
        {
            get { return selectedItem; }
            set { selectedItem = (int)MathHelper.Clamp(value, 0f, items.Count); }
        }
        public string SelectedItem
        {
            get { return items[selectedItem]; }
        }
        public List<string> Items
        {
            get { return items; }
        }

        /* The LeftRightSelector constructor takes the textures of the left arrow, right arrow, and stop bar.
         * TabStop is set to true, since the LeftRightSelector should be able to take input.
         * The default colour is white, which should only be seen when the Selector does not have focus. */
        public LeftRightSelector(Texture2D leftArrow, Texture2D rightArrow, Texture2D stop)
        {
            leftTexture = leftArrow;
            rightTexture = rightArrow;
            stopTexture = stop;
            TabStop = true;
            Color = Color.White;
        }

        /* SetItems() takes an existing array of strings and uses it to fill the list of items.
         * It also sets the maximum width for the Selector. */
        public void SetItems(string[] items, int maxWidth)
        {
            this.items.Clear();
            foreach(string s in items)
            {
                this.items.Add(s);
            }
            maxItemWidth = maxWidth;
        }

        /* This function is called when the selection is changed on the Selector.
         * It activates the relevant event handler. */
        protected void OnSelectionChanged()
        {
            if (SelectionChanged != null)
            {
                SelectionChanged(this, null);
            }
        }

        /* The LeftRightSelector has no logic without input, the Update() is empty.
         * Input logic is handled in HandleInput(). */
        public override void Update(GameTime gameTime)
        {
            
        }

        /* The Draw() function uses a single Vector2 named drawTo to represent the location of the digital "pen" drawing the control.
         * The location of the "pen" changes after drawing each component of the control. */
        public override void Draw(SpriteBatch spriteBatch)
        {
            /* The drawTo vector starts off as the position field of the control.
             * This means that when setting the position of a LeftRightSelector, this position is where the drawing begins. */
            Vector2 drawTo = position;
            
            /* First, the texture to the left of the text is drawn.
             * If the current index isn't zero, the user can scroll left, so the left arrow is drawn.
             * If the current index is zero, the user can't scroll left (it doesn't wrap) so the stop bar is drawn. */
            if (selectedItem != 0)
                spriteBatch.Draw(leftTexture, drawTo, Color.White);
            else
                spriteBatch.Draw(stopTexture, drawTo, Color.White);

            /* Next, the "pen" Vector2 is moved to the right by the width of the left arrow, plus five pixels. */
            drawTo.X += leftTexture.Width + 5f;

            /* The width of the text to be drawn is worked out using SpriteFont.MeasureString(), which returns a width in pixels.
             * The difference between the width of the text and the maximum width is halved to make an offset.
             * The "pen" is moved to the right by this offset, which means the text will be drawn centrally in comparison to the arrows. */
            float itemWidth = spriteFont.MeasureString(items[selectedItem]).X;
            float offset = (maxItemWidth - itemWidth) / 2;
            drawTo.X += offset;

            /* If the Selector's HasFocus property is true, then the text will be drawn to the screen in red.
             * If HasFocus is false, the text is drawn in white.
             * The DrawString() method is passed the control's font, the currently selected string, the "pen" location, and the colour. */
            if (hasFocus)
                spriteBatch.DrawString(spriteFont, items[selectedItem], drawTo, selectedColor);
            else
                spriteBatch.DrawString(spriteFont, items[selectedItem], drawTo, color);

            /* Now that the text has been drawn, our "pen" can now be moved to the correct location for the right arrow.
             * The "pen" is moved left by offset, then right by the maximum width plus five pixels.
             * This keeps the Selector's arrows symmetrical. */
            drawTo.X += -1 * offset + maxItemWidth + 5f;

            /* Finally, the right texture is drawn. If the selected index is not the last one in the list, the right arrow is drawn.
             * If the selected item is the end of the list, the stop bar is drawn as the user cannot scroll right. */
            if (selectedItem != Items.Count - 1)
                spriteBatch.Draw(rightTexture, drawTo, Color.White);
            else
                spriteBatch.Draw(stopTexture, drawTo, Color.White);
        }

        /* HandleInput controls the changing of the selected item. */
        public override void HandleInput(PlayerIndex playerIndex)
        {
            /* If the list is empty, the function must stop here to avoid crashes. */
            if (items.Count == 0)
                return;

            /* If the left arrow key is pressed, then the selected index is reduced by one.
             * The selected index cannot go below zero, and if it goes below, it is set to zero again.
             * The OnSelectionChanged() function is called so that the right event handler is fired. */
            if (InputHandler.ButtonReleased(Buttons.LeftThumbstickLeft, playerIndex) || InputHandler.ButtonReleased(Buttons.DPadLeft, playerIndex) || InputHandler.KeyReleased(Keys.Left))
            {
                selectedItem--;
                if (selectedItem < 0)
                    selectedItem = 0;
                OnSelectionChanged();
            }

            /* If the right arrow key is pressed, the selected index increments.
             * The index can't go past the end of the list, and if it tries, it's set to the end of the list again.
             * Finally, OnSelectionChanged() is called to trigger the event handler. */
            if (InputHandler.ButtonReleased(Buttons.LeftThumbstickRight, playerIndex) || InputHandler.ButtonReleased(Buttons.DPadRight, playerIndex) || InputHandler.KeyReleased(Keys.Right))
            {
                selectedItem++;
                if (selectedItem >= items.Count)
                    selectedItem = items.Count - 1;
                OnSelectionChanged();
            }
        }
    }
}
