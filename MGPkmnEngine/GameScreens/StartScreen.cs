using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MGPkmnLibrary;
using MGPkmnLibrary.Controls;

namespace PkmnEngine.GameScreens
{
    /* The StartScreen is essentially the main menu screen. It allows the user to choose between a new game, a saved game, and quitting. */
    public class StartScreen : BaseGameState
    {
        /* There are five controls on the screen. The backgroundImage field is the picture being displayed in the background of the menu.
         * The arrowImage is the cursor that points to the currently selected option.
         * Three LinkLabels, newGame, loadGame, and exitGame, form the menu. */
        PictureBox backgroundImage;
        PictureBox arrowImage;
        LinkLabel newGame;
        LinkLabel loadGame;
        LinkLabel exitGame;

        /* The maxItemWidth represents the maximum width of a label in the menu. It starts off as zero. */
        float maxItemWidth = 0f;

        /* The StartScreen has no special construction code so it just passes the parameters into the parent constructor. */
        public StartScreen(Game game, GameStateManager manager) : base(game, manager)
        {

        }

        /* The Initialize() function calls the parent function. */
        public override void Initialize()
        {
            base.Initialize();
        }

        /* The LoadContent() function loads images and creates controls for the StartScreen. */
        protected override void LoadContent()
        {
            base.LoadContent();

            /* The backgroundImage is loaded in to the game using Content.Load<T>(). */
            backgroundImage = new PictureBox(Game.Content.Load<Texture2D>(@"Backgrounds/startscreen"), GameRef.ScreenRectangle);
            ControlManager.Add(backgroundImage);

            /* The arrowImage is created by loading in the texture, and creating a new PictureBox with a position of zero.
             * TabStop is set to false, since the arrowImage itself should not be selectable. */
            Texture2D arrowTexture = Game.Content.Load<Texture2D>(@"GUI/leftarrowUp");
            arrowImage = new PictureBox(arrowTexture, new Rectangle(0, 0, arrowTexture.Width, arrowTexture.Height));
            arrowImage.TabStop = false;
            ControlManager.Add(arrowImage);

            /* The three LinkLabels are initialized in the same way.
             * The Text is set manually, and the Size is calculated using SpriteFont.MeasureString().
             * The Selected event handlers are wired with the menuItem_Selected() function. */
            newGame = new LinkLabel();
            newGame.Text = "New Game";
            newGame.Size = newGame.SpriteFont.MeasureString(newGame.Text);
            newGame.Selected += new EventHandler(menuItem_Selected);
            ControlManager.Add(newGame);
            loadGame = new LinkLabel();
            loadGame.Text = "Load Game";
            loadGame.Size = loadGame.SpriteFont.MeasureString(loadGame.Text);
            loadGame.Selected += menuItem_Selected;
            ControlManager.Add(loadGame);
            exitGame = new LinkLabel();
            exitGame.Text = "Exit Game";
            exitGame.Size = exitGame.SpriteFont.MeasureString(exitGame.Text);
            exitGame.Selected += menuItem_Selected;
            ControlManager.Add(exitGame);

            /* The NextControl() function selects the first available control. */
            ControlManager.NextControl();

            /* The FocusChanged event handler in the control manager is wired with ControlManager_FocusChanged(). */
            ControlManager.FocusChanged += new EventHandler(ControlManager_FocusChanged);

            /* The position of each LinkLabel is aligned to the same x position. First, a starting Vector2 position is created. */
            Vector2 position = new Vector2(250, 200);
            foreach(Control c in ControlManager)
            {
                if (c is LinkLabel) {
                    /* For every LinkLabel in the ControlManager, the maxItemWidth is set to its width if it's the biggest one yet.
                     * This essentially means that the maxItemWidth will be set to the width of the widest LinkLabel.
                     * The position of the LinkLabel is set to the starting position plus the LinkLabel's height plus 5 pixels. */
                    if (c.Size.X > maxItemWidth)
                        maxItemWidth = c.Size.X;
                    c.Position = position;
                    position.Y += c.Size.Y + 5f;
                }
            }

            /* The ControlManager_FocusChanged() event handler is called with newGame as the sender. 
             * This means the arrowImage's position will be aligned to the newGame LinkLabel. */
            ControlManager_FocusChanged(newGame, null);
        }

        /* When the focus is changed in the ControlManager on the StartScreen, it means that the menu selection has changed.
         * Thus the position of the arrow image needs to change to align with the new selected menu item. */
        void ControlManager_FocusChanged(object sender, EventArgs e)
        {
            /* This is a reference to the control that has just been selected. */
            Control control = sender as Control;

            /* The position of the arrowImage is set with an X of the control's X plus maxItemWidth add 10 pixels. The Y position is the same as the control's. */
            arrowImage.SetPosition(new Vector2(control.Position.X + maxItemWidth + 10f, control.Position.Y));
        }

        /* This function is called when one of the menu items is selected. */
        private void menuItem_Selected(object sender, EventArgs e)
        {
            /* If the selected menu item is the newGame LinkLabel, the CharacterSelectScreen is pushed onto the state stack. */
            if (sender == newGame)
                Change(ChangeType.Push, GameRef.CharacterSelectScreen);

            /* If the selected menu item is the loadGame LinkLabel, then the LoadGameScreen is pushed onto the state stack. */
            if (sender == loadGame)
                Change(ChangeType.Push, GameRef.LoadGameScreen);

            /* Finally, if the selected menu item is the exitGame LinkLabel, then the game exits. */
            if (sender == exitGame)
                GameRef.Exit();
        }

        /* The Update() function updates the ControlManager. */
        public override void Update(GameTime gameTime)
        {
            ControlManager.Update(gameTime, playerIndexInControl);
            base.Update(gameTime);
        }

        /* Drawing the StartScreen involves drawing all the controls on the screen. */
        public override void Draw(GameTime gameTime)
        {
            /* The SpriteBatch.Begin() function is called to create the memory buffer that will be drawn to. */
            GameRef.SpriteBatch.Begin();
            base.Draw(gameTime);

            /* ControlManager.Draw() draws every control to the memory buffer.
             * SpriteBatch.End() then sends the contents of the buffer to the actual screen. */
            ControlManager.Draw(GameRef.SpriteBatch);
            GameRef.SpriteBatch.End();
        }
    }
}
