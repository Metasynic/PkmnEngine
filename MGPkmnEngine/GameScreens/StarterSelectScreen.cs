using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MGPkmnLibrary;
using MGPkmnLibrary.Controls;
using MGPkmnLibrary.PokemonClasses;

namespace PkmnEngine.GameScreens
{
    /* The StarterSelectScreen is where the user decides what starter Pokemon they would like to use in a new game. */
    public class StarterSelectScreen : BaseGameState
    {
        /* The Screen contains a LeftRightSelector that is used to choose the starter.
         * There are two PictureBox objects which hold the background and the current starter preview image.
         * The starterImages array holds the three preview textures.
         * A corresponding array holds the text for the names of each choice.
         * A property, SelectedStarter, exposes the LeftRightSelector's currently selected item. */
        LeftRightSelector starterSelector;
        PictureBox backgroundImage;
        PictureBox starterImage;
        Texture2D[] starterImages;
        string[] starterItems = { "Bulbasaur", "Charmander", "Squirtle" };
        public string SelectedStarter
        {
            get { return starterSelector.SelectedItem; }
        }

        /* The constructor for the StarterSelectScreen is empty, as everything is taken care of in the parent constructor. */
        public StarterSelectScreen(Game game, GameStateManager manager) : base(game, manager)
        {
            
        }

        /* Initialize() calls its parent function. */
        public override void Initialize()
        {
            base.Initialize();
        }

        /* LoadContent() calls its parent, then the internal LoadImages() and CreateControls() methods to set up the screen. */
        protected override void LoadContent()
        {
            base.LoadContent();
            LoadImages();
            CreateControls();
        }

        /* This method loads in the textures for the screen.
         * The starterImages array is initialized with a length equal to starterItems.
         * Each texture is then loaded into the array using Content.Load<T>(), with the name of the image being "[Name]_Starter". */
        private void LoadImages()
        {
            starterImages = new Texture2D[starterItems.Length];
            for (int i = 0; i < starterItems.Length; i++)
            {
                starterImages[i] = Game.Content.Load<Texture2D>(@"Tilesets/" + starterItems[i] + "_Starter");
            }
        }

        /* CreateControls() sets up most of what's visible on the screen. */
        private void CreateControls()
        {
            /* The two PictureBox objects are constructed with textures that are loaded in, then added to the ControlManager. */
            backgroundImage = new PictureBox(Game.Content.Load<Texture2D>(@"Backgrounds/startscreen"), GameRef.ScreenRectangle);
            ControlManager.Add(backgroundImage);
            starterImage = new PictureBox(starterImages[0], new Rectangle(0, 0, 240, 240), new Rectangle(350, 100, 240, 240));
            ControlManager.Add(starterImage);

            /* The title and confirm labels are created and set with the right text, position, and size.
             * The event handler is wired so that when the user chooses "Confirm", confirmStarter_Selected is called. */
            Label title = new Label();
            title.Text = "Choose your starter";
            title.Size = title.SpriteFont.MeasureString(title.Text);
            title.Position = new Vector2((GameRef.Window.ClientBounds.Width - title.Size.X) / 2, 50);
            ControlManager.Add(title);
            LinkLabel confirmStarter = new LinkLabel();
            confirmStarter.Text = "Confirm";
            confirmStarter.Position = new Vector2(title.Position.X, 300);
            confirmStarter.Selected += new EventHandler(confirmStarter_Selected);
            ControlManager.Add(confirmStarter);

            /* The three textures for the LeftRightSelector are loaded in using Content.Load<Texture2D>().
             * The LeftRightSelector is then initialised using the loaded textures.
             * Its items are set to the array of starter names, and the position is set correctly.
             * Its SelectionChanged is wired to the selectionChanged function, and then it's added to the ControlManager. */
            Texture2D leftTexture = Game.Content.Load<Texture2D>(@"GUI/leftArrowUp");
            Texture2D rightTexture = Game.Content.Load<Texture2D>(@"GUI/rightArrowUp");
            Texture2D stopTexture = Game.Content.Load<Texture2D>(@"GUI/stopBar");
            starterSelector = new LeftRightSelector(leftTexture, rightTexture, stopTexture);
            starterSelector.SetItems(starterItems, 175);
            starterSelector.Position = new Vector2(title.Position.X - 100, 200);
            starterSelector.SelectionChanged += new EventHandler(selectionChanged);
            ControlManager.Add(starterSelector);
            
            /* NextControl() is called to make sure a valid control is selected when the screen is displayed. */
            ControlManager.NextControl();
        }

        /* The Update() function updates the ControlManager so that all controls are updated, and calls its parent function. */
        public override void Update(GameTime gameTime)
        {
            ControlManager.Update(gameTime, PlayerIndex.One);
            base.Update(gameTime);
        }

        /* This function draws the StarterSelectScreen. The SpriteBatch.Begin() method is called with default parameters.
         * The base Draw() method is called, followed by drawing all the controls on the screen.
         * Finally, SpriteBatch.End() flushes the drawn graphics from the internal buffer to the screen. */
        public override void Draw(GameTime gameTime)
        {
            GameRef.SpriteBatch.Begin();
            ControlManager.Draw(GameRef.SpriteBatch);
            base.Draw(gameTime);
            GameRef.SpriteBatch.End();
        }
        
        /* The selectionChanged() function gets called when the user presses the left or right arrow keys to change their starter.
         * It changes the source image on the screen to the corresponding texture from the starterImages array. */
        void selectionChanged(object sender, EventArgs e)
        {
            starterImage.Image = starterImages[starterSelector.SelectedIndex];
        }

        /* This event handler method is called when the "Confirm" LinkLabel is selected.
         * The InputHandler is flushed so that the user's keypress does not carry over to the next screen.
         * CreateStarter() is called internally to make the player's Pokemon team.
         * The WorldScreen will then be pushed onto the GameStateManager's stack. */
        void confirmStarter_Selected(object sender, EventArgs e)
        {
            InputHandler.Flush();

            CreateStarter();

            Change(ChangeType.Change, GameRef.WorldScreen);
        }

        /* This internal method is called when the "Confirm" label is selected.
         * It fills the player's team with three Pokemon: The chosen starter, a Lv10 Mewtwo, and a Lv20 Arbok. */
        private void CreateStarter()
        {
            /* The ID field holds the ID of the chosen starter.
             * An array is created to hold the starter's moves. */
            ushort id;
            Move[] moves = new Move[4];
            
            /* Different things will now happen depending on which starter has been chosen. */
            switch (starterSelector.SelectedItem)
            {
                /* If the name of the selected starter was "Bulbasaur", the ID is set to 1 and the moves as Scratch and Razor Leaf. */
                case "Bulbasaur":
                    id = 1;
                    moves[0] = DataManager.Moves["Scratch"];
                    moves[1] = DataManager.Moves["Razor Leaf"];
                    break;
                /* If "Charmander" is chosen, then the ID is 4 and the moves are Scratch and Flamethrower. */
                case "Charmander":
                    id = 4;
                    moves[0] = DataManager.Moves["Scratch"];
                    moves[1] = DataManager.Moves["Flamethrower"];
                    break;
                /* If "Squirtle" is the choice, the ID will be 7 and the moves will be set to Scratch and Bubble Beam. */
                case "Squirtle":
                    id = 7;
                    moves[0] = DataManager.Moves["Scratch"];
                    moves[1] = DataManager.Moves["Bubble Beam"];
                    break;
                /* If for some reason an invalid option was chosen, an exception will be thrown. */
                default:
                    throw new Exception("Invalid starterSelector.SelectedItem");
            }

            /* The first Pokemon is created using the ID from the selector, and the move array.
             * Player.InitTeam() has this new Pokemon passed to it, which creates the team and sets the starter as the first item. */
            Pokemon starter = new Pokemon(DataManager.PokemonSpecies[id], Pokemon.GetGender(DataManager.PokemonSpecies[id].GenderRatio), moves, 5, null);
            WorldScreen.Player.InitTeam(starter);

            /* The second and third Pokemon in the team are created using specific IDs and the same moveset as before.
             * They are added to the player team using Player.AddToTeam(). */
            Pokemon second = new Pokemon(DataManager.PokemonSpecies[150], Pokemon.GetGender(DataManager.PokemonSpecies[150].GenderRatio), moves, 10, null);
            WorldScreen.Player.AddToTeam(second);
            Pokemon third = new Pokemon(DataManager.PokemonSpecies[237], Pokemon.GetGender(DataManager.PokemonSpecies[237].GenderRatio), moves, 20, null);
            WorldScreen.Player.AddToTeam(third);
        }
    }
}
