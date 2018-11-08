using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MGPkmnLibrary.Controls
{
    /* This Control displays a list of text items and allows the user to select one of them. */
    public class ListBox : Control
    {
        /* There are three event handlers for the ListBox. One is fired when the selected item is changed.
         * The other two are fired when the user enters and leaves the ListBox. */
        public event EventHandler SelectionChanged;
        public event EventHandler Enter;
        public event EventHandler Leave;

        /* The items list contains the different strings that are listed in the ListBox. */
        List<string> items = new List<string>();
        public List<string> Items
        {
            get { return items; }
        }

        /* The startItem field represents the index of the item that should be selected in the list by default.
         * The selectedItem is the index of the item that is currently selected.
         * When set, the value is clamped between zero and the length of the item list to avoid NullReferenceExceptions.
         * The lineCount is the number of lines of text that need drawing.
         * It varies from the number of items because one item may need to span more than one line. */
        int startItem;
        int selectedIndex;
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set { selectedIndex = (int)MathHelper.Clamp(value, 0f, items.Count); }
        }
        int lineCount;

        /* These Texture2Ds represent the image in the background of the ListBox, and the image being used to show which item is currently selected. */
        Texture2D image;
        Texture2D cursor;

        /* The selectedColor is the colour of the text item that is selected. It's red by default. */
        Color selectedColor = Color.Red;
        public Color SelectedColor
        {
            get { return selectedColor; }
            set { selectedColor = value; }
        }

        /* SelectedItem exposes the currently selected string from the list. */
        public string SelectedItem
        {
            get { return Items[selectedIndex]; }
        }

        /* The HasFocus property of the ListBox overrides the normal HasFocus.
         * When it is set, one of the OnEnter() and OnLeave() functions are called, depending on whether the focus was set to true or false.
         * If HasFocus is set to true, OnEnter() is triggered, and if it's set to false, OnLeave() is triggered. */
        public override bool HasFocus
        {
            get { return hasFocus; }
            set { hasFocus = value;
                if (hasFocus)
                    OnEnter(null);
                else
                    OnLeave(null);
            }
        }

        /* The ListBox constructor takes the textures of the background and the cursor. */
        public ListBox(Texture2D background, Texture2D cursor) : base()
        {
            /* The hasFocus and tabStop fields are set to false by default. */
            hasFocus = false;
            tabStop = false;

            /* The background image and cursor image are set according to the parameters passed in. */
            image = background;
            this.cursor = cursor;

            /* Size is calculated with the image width and height.
             * The lineCount is worked out by dividing the image height by the amount of space taken up by one line. */
            Size = new Vector2(image.Width, image.Height);
            lineCount = image.Height / SpriteFont.LineSpacing;

            /* The startItem is zero by default, and the text colour is black. */
            startItem = 0;
            Color = Color.Black;
        }

        /* The ListBox does not update every frame, only when it receives input, so Update() is empty. */
        public override void Update(GameTime gameTime)
        {

        }

        /* Drawing a ListBox involves drawing the background image, cursor image, and text items. */
        public override void Draw(SpriteBatch spriteBatch)
        {
            /* First, the background image is drawn in the ListBox's position, with no tint colour. */
            spriteBatch.Draw(image, position, Color.White);

            /* A for loop iterates through the lines in the ListBox. */
            for (int i = 0; i < lineCount; i++)
            {
                /* If the startItem + i exceeds the list length, no more text can be drawn, so break. */
                if (startItem + i >= items.Count)
                    break;

                /* If the current line is the one with the selected text, the cursor needs to be drawn as well as the text. */
                if (startItem + i == selectedIndex)
                {
                    /* The string is drawn with SpriteBatch.DrawString(). The destination position is aligned to the X coordinate of the List Box.
                     * The destination Y position is found by adding the Y position of the ListBox to i times the line spacing.
                     * The text is drawn in the selectedColor.
                     * The cursor image is drawn to the left of the text. */
                    spriteBatch.DrawString(SpriteFont, items[startItem + i], new Vector2(Position.X, Position.Y + i * SpriteFont.LineSpacing), selectedColor);
                    spriteBatch.Draw(cursor, new Vector2(Position.X - (cursor.Width + 2), Position.Y + i * SpriteFont.LineSpacing + 5), Color.White);
                }
                else
                {
                    /* In this case, a non-selected piece of text needs to be drawn. The same call to DrawString() is used as before, except with the normal colour. */
                    spriteBatch.DrawString(SpriteFont, items[startItem + i], new Vector2(Position.X, 2 + Position.Y + i * SpriteFont.LineSpacing), color);
                }
            }
        }

        /* This function handles all input while the ListBox is active. */ 
        public override void HandleInput(PlayerIndex playerIndex)
        {
            /* If focus is not on the ListBox, it does not need to receive input, so the function returns. */
            if (!HasFocus)
                return;

            /* If the user releases the down arrow, or the left thumbstick is pushed down on a gamepad, the selection needs to move down. */
            if (InputHandler.KeyReleased(Keys.Down) || InputHandler.ButtonReleased(Buttons.LeftThumbstickDown, playerIndex))
            {
                /* This condition checks if the selected item isn't at the end of the list.
                 * The ListBox selection does not wrap around - it just won't do anything if you try to move outside the list. */
                if (selectedIndex < items.Count - 1)
                {
                    /* In this case, the selectedIndex is increased by one.
                     * If the selectedIndex is bigger than the startItem plus the lineCount,
                     * then the index is beyond the end of the list and the startItem needs to be changed accordingly. */
                    selectedIndex++;
                    if (selectedIndex >= startItem + lineCount)
                    {
                        startItem = selectedIndex - lineCount + 1;
                    }

                    /* Since the selection has been changed, the OnSelectionChanged() event handler function is called. */
                    OnSelectionChanged(null);
                }
            }

            /* Similarly, if the up arrow is released, or the left thumbstick is pushed up on a gamepad, the selection needs to move up. */
            else if (InputHandler.KeyReleased(Keys.Up) || InputHandler.ButtonReleased(Buttons.LeftThumbstickUp, playerIndex))
            {
                /* If the selected item isn't at the beginning of the list, then this block executes. */
                if (selectedIndex > 0)
                {
                    /* The selectedIndex is decremented, and if the selectedIndex is smaller than the startItem,
                     * the startItem is not set correctly, and it is changed to be the same as the selectedIndex. */
                    selectedIndex--;
                    if (selectedIndex < startItem)
                    {
                        startItem = selectedIndex;
                    }

                    /* The selection has been changed, so OnSelectionChanged() is called. */
                    OnSelectionChanged(null);
                }
            }

            /* If the enter key or A button is pressed, the user is confirmng their selection and HasFocus is set to false.
             * Since the property is being set to false, this also fires the OnLeave() event handler.
             * The parent OnSelected() event is also triggered. */
            if (InputHandler.KeyPressed(Keys.Enter) || InputHandler.ButtonReleased(Buttons.A, playerIndex))
            {
                HasFocus = false;
                OnSelected(null);
            }

            /* Finally, to escape from the list, the user can release the backspace key or B button.
             * If this happens, HasFocus is set to false, which calls OnLeave(). */
            if (InputHandler.KeyReleased(Keys.Back) || InputHandler.ButtonReleased(Buttons.B, playerIndex))
            {
                HasFocus = false;
            }
        }

        /* These three functions call the relevant event handler functions if they are not null. */
        protected virtual void OnSelectionChanged(EventArgs e)
        {
            SelectionChanged?.Invoke(this, e);
        }
        protected virtual void OnEnter(EventArgs e)
        {
            Enter?.Invoke(this, e);
        }
        protected virtual void OnLeave(EventArgs e)
        {
            Leave?.Invoke(this, e);
        }
    }
}
