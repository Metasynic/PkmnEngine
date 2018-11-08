using System;
using MGPkmnLibrary;
using MGPkmnLibrary.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace PkmnEngine.GameScreens
{
    /* The SplashScreen displays when the engine is booted. At the moment, it just has a logo and a start button.
     * Although it is not a part of the user requirements, I would like to add some graphics to show the content loading progress. */
    public class SplashScreen : BaseGameState
    {
        /* There are two controls on the SplashScreen. The splashImage is a black picture with a "Metasynic" (my developer name) logo.
         * The goLabel field is a LinkLabel which allows the user to advance to the StartScreen. */
        Texture2D splashImage;
        LinkLabel goLabel;

        /* The SplashScreen does not have any special constructor code separate from its parent.
         * Thus this function is empty, and passes parameters into the parent constructor. */
        public SplashScreen(Game game, GameStateManager manager) : base(game, manager)
        {

        }
        
        /* The LoadContent() function creates the splashImage by loading it in with Content.Load<T>().
         * It then initializes the goLabel and sets its position, text, and colour.
         * Its TabStop and HasFocus properties are set to true, snce we want this to be the only selectable control on the screen.
         * Its Selected event handler is wired with goLabel_Selected(), and the control is added to the ControlManager so it gets updated and drawn. */
        protected override void LoadContent()
        {
            splashImage = Game.Content.Load<Texture2D>(@"Misc/Logo");
            base.LoadContent();
            goLabel = new LinkLabel();
            goLabel.Position = new Vector2(150, 400);
            goLabel.Text = "PRESS ENTER TO START";
            goLabel.Color = Color.White;
            goLabel.TabStop = true;
            goLabel.HasFocus = true;
            goLabel.Selected += new EventHandler(goLabel_Selected);
            ControlManager.Add(goLabel);
           
        }

        /* The Update() function just updates the control manager and calls the parent function. */
        public override void Update(GameTime gameTime)
        {
            ControlManager.Update(gameTime, PlayerIndex.One);
            base.Update(gameTime);
        }

        /* Draw() initializes a SpriteBatch object with a draw buffer and calls the parent function.
         * It then draws the splashImage to the window rectangle with no tint colour.
         * The goLabel is drawn by calling ControlManager.Draw(), and then End() sends the contents of the draw buffer to the screen. */
        public override void Draw(GameTime gameTime)
        {
            GameRef.SpriteBatch.Begin();
            base.Draw(gameTime);
            GameRef.SpriteBatch.Draw(splashImage, GameRef.ScreenRectangle, Color.White);
            ControlManager.Draw(GameRef.SpriteBatch);
            GameRef.SpriteBatch.End();
        }

        /* When the goLabel is selected, the game state is changed by pushing the StartScreen on to the stack of screens. */
        private void goLabel_Selected(object sender, EventArgs e)
        {
            Change(ChangeType.Push, GameRef.StartScreen);
        }
    }
}
