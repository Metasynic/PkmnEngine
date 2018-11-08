using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MGPkmnLibrary.Controls
{
    /* This class is a list of all the controls in a screen. It inherits directly from a list of Controls.
     * There is one ControlManager in each PkmnEngine.GameScreens.BaseGameState object, which all screens inherit from. */
    public class ControlManager : List<Control>
    {
        /* The acceptsInput bit controls whether the Control Manager will take any keyboard signals. */
        bool acceptsInput = true;
        public bool AcceptsInput
        {
            get { return acceptsInput; }
            set { acceptsInput = value; }
        }

        /* FocusChanged stores an event handler which fires when the focus is changed from one control in the manager to another. */
        public event EventHandler FocusChanged;

        /* The index of the current control in the list being acted on. */
        int selectedControl = 0;

        /* The SpriteFont with which all the text in the controls are drawn. */
        static SpriteFont spriteFont;
        public static SpriteFont SpriteFont
        {
            get { return spriteFont; }
        }

        /* There are three different constructors for the Control Manager.
         * One sets the SpriteFont and leaves the parent List<Control> with no specified capacity. */
        public ControlManager(SpriteFont spriteFont) : base()
        {
            ControlManager.spriteFont = spriteFont;
        }

        /* The second one sets the SpriteFont and takes a capacity to initialize the parent List<Control> with. */
        public ControlManager(SpriteFont spriteFont, int capacity) : base(capacity)
        {
            ControlManager.spriteFont = spriteFont;
        }

        /* The last one sets the SpriteFont and takes an IEnumerable of Controls to add to the List<Control>. */
        public ControlManager(SpriteFont spriteFont, IEnumerable<Control> collection) : base(collection)
        {
            ControlManager.spriteFont = spriteFont;
        }

        /* The job of the Control Manager is to update and draw every control in the parent List<Control>. */
        public void Update(GameTime gameTime, PlayerIndex playerIndex)
        {
            /* Obviously if there are no controls, no updating needs to be done, so the function returns. */
            if (Count == 0)
                return;

            /* Every control with Enabled set to true will be updated.
             * Every control with HasFocus set to true will have input handled. */
            foreach (Control c in this)
            {
                if (c.Enabled)
                    c.Update(gameTime);
                if (c.HasFocus)
                    c.HandleInput(playerIndex);

            }

            /* If the ControlManager is not currently accepting input, the function needs to stop now before it checks input. */
            if (!acceptsInput)
                return;

            /* Otherwise, the CntrolManager checks the up and down arrow keys will switch between controls in the list.
             * To do this, it uses NextControl() and PreviousControl(), which are defined further down in the class. */
            if (InputHandler.ButtonPressed(Buttons.LeftThumbstickUp, playerIndex) || InputHandler.ButtonPressed(Buttons.DPadUp, playerIndex) || InputHandler.KeyPressed(Keys.Up))
                PreviousControl();
            if (InputHandler.ButtonPressed(Buttons.LeftThumbstickDown, playerIndex) || InputHandler.ButtonPressed(Buttons.DPadDown, playerIndex) || InputHandler.KeyPressed(Keys.Down))
                NextControl();
        }

        /* The Draw() function just draws each control in the list with Visible set to true.
         * The SpriteBatch is passed in from the Screen that the ControlManager belongs to. */
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Control c in this)
            {
                if (c.Visible)
                    c.Draw(spriteBatch);
            }
        }

        /* The following two functions toggle the selected control in the list and handle errors. */
        public void NextControl()
        {
            /* If there are no controls, the function should stop. */
            if (Count == 0)
                return;

            /* Next, a new variable representing the current control index is created.
             * The current control's HasFocus property is set to false, in preparation for another control having HasFocus set to true later. */
            int currentControl = selectedControl;
            this[selectedControl].HasFocus = false;
            do
            {
                /* This do-while loop runs as long as the control it's looking at is not the current control.
                 * First increment the selectedControl index.
                 * Then, if the new selectedControl index is beyond the length of the list, it wraps around to 0. */
                selectedControl++;
                if (selectedControl == Count)
                    selectedControl = 0;
                
                /* If the current control in the list has TabStop and Enabled (in other words, if it can be selected and have input),
                 * then fire the FocusChanged event for the ControlManager, passing in the new control as the sender. */
                if (this[selectedControl].TabStop && this[selectedControl].Enabled)
                {
                    if (FocusChanged != null)
                        FocusChanged(this[selectedControl], null);
                    break;
                }
            } while (currentControl != selectedControl);

            /* Finally, the new control's HasFocus is set to true, since it now has focus. */
            this[selectedControl].HasFocus = true;
        }

        /* This function works in a similar way.
         * The code is mostly the same. */
        public void PreviousControl()
        {
            if (Count == 0)
                return;
            int currentControl = selectedControl;
            this[selectedControl].HasFocus = false;
            do
            {
                /* The only difference is that each iteration of the loop decrements selectedControl instead of incrementing,
                 * Once the selectedControl index goes below 0, the index will wrap around to the end of the list. */
                selectedControl--;
                if (selectedControl < 0)
                    selectedControl = Count - 1;

                /* After that, everything is the same as in NextControl(). */
                if (this[selectedControl].TabStop && this[selectedControl].Enabled)
                {
                    if (FocusChanged != null)
                        FocusChanged(this[selectedControl], null);
                    break;
                }

                /* This loop also runs as long as the control it's looking at isn't the one it already has selected.
                 * This ensures the loop will go through every control in the list, other than the currently selected one. */
            } while (currentControl != selectedControl);
            this[selectedControl].HasFocus = true;
        }
    }
}
