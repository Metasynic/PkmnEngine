using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace MGPkmnLibrary.Controls
{
    /* The FourButton is the type of control used by the main battle menu. As the name suggests, it has four buttons which are assigned with text and events. */
    public class FourButton : Control
    {
        /* The first EventHandler is triggered whenever the user changes their selection of button. */
        public event EventHandler SelectionChanged;

        /* These four event handlers are invoked when the user selects a specific button in the FourButton. */
        public event EventHandler FirstButtonPressed;
        public event EventHandler SecondButtonPressed;
        public event EventHandler ThirdButtonPressed;
        public event EventHandler FourthButtonPressed;

        /* The selectedColor is the colour that a button's text will turn when it's currently selected.
         * The selectedIndex byte holds the index of the button that's highlighted.
         * An array of strings holds the text for each of the four buttons.
         * Finally, the background holds the image displayed behind the control. */
        Color selectedColor = Color.Red;
        byte selectedIndex;
        string[] labels = new string[4];
        Texture2D background;

        public Color SelectedColor
        {
            get { return selectedColor; }
            set { selectedColor = value; }
        }
        public byte SelectedIndex
        {
            get { return selectedIndex; }
            set { selectedIndex = (byte)MathHelper.Clamp(value, 0f, 3f); }
        }
        public string SelectedItem
        {
            get { return labels[selectedIndex]; }
        }
        public string[] Labels
        {
            get { return labels; }
        }

        /* The FourButton constructor only takes a Texture2D for the background image.
         * Everything else is set outside the constructor. */
        public FourButton(Texture2D image)
        {
            /* The hasFocus and tabStop bools are always true.
             * The default text colour is black.
             * The background image is set from the argument passed in, and selectedIndex starts at zero. */
            hasFocus = true;
            tabStop = true;
            color = Color.Black;

            background = image;
            selectedIndex = 0;
        }
        
        /* This function can be used to set the text for each button. It takes four strings as arguments, and sets them into the labels array. */
        public void SetButtons(string firstButton, string secondButton, string thirdButton, string fourthButton)
        {
            labels[0] = firstButton;
            labels[1] = secondButton;
            labels[2] = thirdButton;
            labels[3] = fourthButton;
        }

        /* This function is called whenever the selected button is changed, and invokes the relevant event handler. */
        protected void OnSelectionChanged()
        {
            SelectionChanged?.Invoke(this, null);
        }

        /* Each of these four functions call the related event handler when a button is pressed. */
        protected void FirstButton()
        {
            FirstButtonPressed?.Invoke(this, null);
        }
        protected void SecondButton()
        {
            SecondButtonPressed?.Invoke(this, null);
        }
        protected void ThirdButton()
        {
            ThirdButtonPressed?.Invoke(this, null);
        }
        protected void FourthButton()
        {
            FourthButtonPressed?.Invoke(this, null);
        }

        /* A FourButton doesn't do anything unless it receives input, so this function is empty. */
        public override void Update(GameTime gameTime)
        {
            
        }

        /* Drawing a FourButton is not as complicated as it looks. Basically, the background is drawn, then each button is drawn. */
        public override void Draw(SpriteBatch spriteBatch)
        {
            /* A new Vector2 is created to use as the "pen" for drawing with.
             * The background image is drawn in the original position of the FourButton. */
            Vector2 drawTo = position;
            spriteBatch.Draw(background, drawTo, Color.White);

            /* The "pen" is moved to a tenth of the width and height of the entire control.
             * If there is text in the array slot holding the first button label, then it is drawn.
             * If the selectedIndex is zero, it's drawn in the selectedColor, otherwise the regular colour. */
            drawTo.X += size.X / 10;
            drawTo.Y += size.Y / 10;
            if (labels[0] != null)
            {
                if (selectedIndex == 0)
                    spriteBatch.DrawString(spriteFont, labels[0], drawTo, selectedColor);
                else
                    spriteBatch.DrawString(spriteFont, labels[0], drawTo, color);
            }

            /* The "pen" is then moved to the right, two-fifths of the way along the control.
             * As long as there is text to draw in the correct slot of the array, the text is drawn.
             * When selectedIndex is one, the text is drawn in red, otherwise it's in black. */
            drawTo.X += size.X / (float)2.5;
            if (labels[1] != null)
            {
                if (selectedIndex == 1)
                    spriteBatch.DrawString(spriteFont, labels[1], drawTo, selectedColor);
                else
                    spriteBatch.DrawString(spriteFont, labels[1], drawTo, color);
            }

            /* To draw the bottom-left button, the "pen" X moves left two-fifths of the way along, and the Y one third of the way down.
             * The text itself is drawn identically to the first two. */
            drawTo.X -= size.X / (float)2.5;
            drawTo.Y += size.Y / 3;
            if (labels[2] != null)
            {
                if (selectedIndex == 2)
                    spriteBatch.DrawString(spriteFont, labels[2], drawTo, selectedColor);
                else
                    spriteBatch.DrawString(spriteFont, labels[2], drawTo, color);
            }

            /* Finally the "pen" is moved back to the right to draw the last label.
             * It is only drawn if the last item in the labels array isn't null. */
            drawTo.X += size.X / (float)2.5;
            if (labels[3] != null)
            {
                if (selectedIndex == 3)
                    spriteBatch.DrawString(spriteFont, labels[3], drawTo, selectedColor);
                else
                    spriteBatch.DrawString(spriteFont, labels[3], drawTo, color);
            }
        }

        /* This function has a lot of code but accomplishes a very simple purpose.
         * It will do different things depending on what button is currently selected. */
        public override void HandleInput(PlayerIndex playerIndex)
        {
            switch(selectedIndex)
            {
                /* If the top-left button is under selection, then the user can move to the top-right by pressing right,
                 * or the bottom-left button by pressing down. If the user hits enter, FirstButton() calls the correct event handler. */
                case 0:
                    if (InputHandler.KeyReleased(Keys.Right) && labels[1] != null)
                    {
                        selectedIndex = 1;
                        OnSelectionChanged();
                    }
                    else if (InputHandler.KeyReleased(Keys.Down) && labels[2] != null)
                    {
                        selectedIndex = 2;
                        OnSelectionChanged();
                    }
                    else if (InputHandler.KeyReleased(Keys.Enter))
                    {
                        FirstButton();
                    }
                    break;

                /* If the top-right button is currently selected, then the user can move left or down.
                 * If they press enter, the SecondButton() function calls the event handler for this button. */
                case 1:
                    if (InputHandler.KeyReleased(Keys.Left) && labels[0] != null) {
                        selectedIndex = 0;
                        OnSelectionChanged();
                    }
                    else if (InputHandler.KeyReleased(Keys.Down) && labels[3] != null)
                    {
                        selectedIndex = 3;
                        OnSelectionChanged();
                    }
                    else if (InputHandler.KeyReleased(Keys.Enter))
                    {
                        SecondButton();
                    }
                    break;

                /* When the bottom-left button is selected, the user can move their selection right or upwards.
                 * Pressing enter will call the event handler for the third button. */
                case 2:
                    if (InputHandler.KeyReleased(Keys.Right) && labels[3] != null)
                    {
                        selectedIndex = 3;
                        OnSelectionChanged();
                    }
                    else if (InputHandler.KeyReleased(Keys.Up) && labels[0] != null)
                    {
                        selectedIndex = 0;
                        OnSelectionChanged();
                    }
                    else if (InputHandler.KeyReleased(Keys.Enter))
                    {
                        ThirdButton();
                    }
                    break;

                /* Lastly, if the bottom-right button is highlighted, the user can choose to move left or up.
                 * If they hit enter, then the FourthButton() function calls the appropriate event handler. */
                case 3:
                    if (InputHandler.KeyReleased(Keys.Left) && labels[2] != null)
                    {
                        selectedIndex = 2;
                        OnSelectionChanged();
                    }
                    else if (InputHandler.KeyReleased(Keys.Up) && labels[1] != null)
                    {
                        selectedIndex = 1;
                        OnSelectionChanged();
                    }
                    else if (InputHandler.KeyReleased(Keys.Enter))
                    {
                        FourthButton();
                    }
                    break;

                /* If there's no selectedIndex, then an exception will be thrown. */
                default:
                    throw new Exception("Error processing FourButton input.");
            }
        }

        /* This function simply sets all the button event handlers as null to clear them. */
        public void ClearButtonEvents()
        {
            FirstButtonPressed = null;
            SecondButtonPressed = null;
            ThirdButtonPressed = null;
            FourthButtonPressed = null;
        }

        /* ResetIndex() literally just sets the selectedIndex to zero. */
        public void ResetIndex()
        {
            selectedIndex = 0;
        }
    }
}
