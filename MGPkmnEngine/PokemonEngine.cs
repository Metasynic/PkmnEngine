using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MGPkmnLibrary;
using PkmnEngine.GameScreens;

namespace PkmnEngine
{
    /* This is the central class of the engine. It replaces MonoGame's usual Game1 with a more appropriately named class.
     * Obviously, it inherits from Microsoft.Xna.Framework.Game so that all the features of MonoGame can be used. */
    public class PokemonEngine : Game
    {
        /* A lot of core components for the engine are declared here.
         * The GraphicsDeviceManager is a MonoGame class which handles graphical operations and is needed to initialize the SpriteBatch.
         * The SpriteBatch is an object used to draw everything to the screen. Calls to draw items are made through the SpriteBatch.
         * The stateManager field is the GameStateManager for the game, which holds a stack of all the GameStates. */
        GraphicsDeviceManager graphics;
        public SpriteBatch SpriteBatch;
        GameStateManager stateManager;

        /* An instance for each screen is also declared here. The purpose of each screen is detailed in their respective class files. */
        public SplashScreen SplashScreen;
        public StartScreen StartScreen;
        public WorldScreen WorldScreen;
        public CharacterSelectScreen CharacterSelectScreen;
        public LoadGameScreen LoadGameScreen;
        public StarterSelectScreen StarterSelectScreen;
        public InventoryScreen InventoryScreen;
        public BattleScreen BattleScreen;
        public SwitchScreen SwitchScreen;

        /* The screenWidth and screenHeight constants are used to define the width and height of the window.
         * They are defined as constants here so that if the size of the window needs changing, only these constants need changing.
         * The ScreenRectangle is a Rectangle object representing the window. */
        const int screenWidth = 640;
        const int screenHeight = 480;
        public readonly Rectangle ScreenRectangle;

        /* These fields help to hold information about the frames per second in memory.
         * The fps field stores the current frames per second of the engine.
         * The interval is the amount of time that should pass before the FPS display is updated.
         * The timeSinceLastUpdate is self-explanatory - it stores the amount of time that's passed since the last iteration of the game loop.
         * The frameCount field holds the number of times the game loop has run since the last update to the FPS count. */
        private float fps;
        private float interval = 1.0f;
        private float timeSinceLastUpdate = 0.0f;
        private float frameCount = 0;

        /* The PokemonEngine() constructor initializes all of the important parts of the engine, including managers and screens. */
        public PokemonEngine()
        {
            /* The GraphicsDeviceManager is initialized, and the PreferredBackBuffer properties are set to the constants screenWidth and screenHeight.
             * This means that the engine window will have these dimensions, if possible.
             * The ScreenRectangle is also initialized with a position of zero and the screen width and height. */
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.PreferredBackBufferHeight = screenHeight;
            ScreenRectangle = new Rectangle(0, 0, screenWidth, screenHeight);

            /* The two vital components of the engine, the InputHandler and the GameStateManager, are initialized and added to the engine's master component list.
             * Whenever base.Update() and base.Draw() are called from this class, everything on the component list is updated and drawn.
             * Since these classes contain all code for handling input and for drawing the game states, everything will now be updated and drawn properly. */
            Components.Add(new InputHandler(this));
            stateManager = new GameStateManager(this);
            Components.Add(stateManager);

            /* Every screen is initialized. Each has a reference to the engine and the state manager passed in. */
            SplashScreen = new SplashScreen(this, stateManager);
            StartScreen = new StartScreen(this, stateManager);
            WorldScreen = new WorldScreen(this, stateManager);
            CharacterSelectScreen = new CharacterSelectScreen(this, stateManager);
            LoadGameScreen = new LoadGameScreen(this, stateManager);
            StarterSelectScreen = new StarterSelectScreen(this, stateManager);
            InventoryScreen = new InventoryScreen(this, stateManager);
            BattleScreen = new BattleScreen(this, stateManager);
            SwitchScreen = new SwitchScreen(this, stateManager);

            /* The state stack is reset and the SplashScreen is pushed on. This means the first thing to appear when the engine is run will be the SplashScreen. */
            stateManager.ChangeState(SplashScreen);

            /* The content root directory is set to the "/Content/" folder, meaning every filename string passed to Content.Load<T>() will start in this folder. */
            Content.RootDirectory = "Content";

            /* Type matchups are initialized in the PkmnUtils class, which creates the type advantage lookup table. */
            PkmnUtils.InitTypeMatchups();

            /* These two settings make the game run as many times per second as it can.
             * They could be changed later depending on how well the engine performs. */
            IsFixedTimeStep = false;
            graphics.SynchronizeWithVerticalRetrace = false;
        }

        /* The Initialize() function calls its parent function in Microsoft.Xna.Framework.Game. */
        protected override void Initialize()
        {
            base.Initialize();
        }

        /* The LoadContent() method loads the large lists of data into the game.
         * The LoadContent() methods of each GameState do not need calling here,
         * as they appear to be called when the GameStates are added to the list of components. */
        protected override void LoadContent()
        {
            /* First, the SpriteBatch for the game is initialized taking the GraphicsDevice as an argument. */
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            /* Next, the DataManager reads in the different types of XML and texture data used in the engine. */
            DataManager.ReadPokemonData(Content);
            DataManager.ReadMoveData(Content);
            DataManager.ReadTrainerData(Content);
            DataManager.ReadLevelData(Content);
            DataManager.ReadAllPkmnSprites(Content);

            /* Once the Pokemon Species are loaded in, a reference to them is stored in the Battle class as it will be needed later. */
            MGPkmnLibrary.BattleClasses.Battle.PokemonSpeciesRef = DataManager.PokemonSpecies;
        }

        /* UnloadContent() is empty since I am not bypassing the ContentManager's default unloading behaviour. */
        protected override void UnloadContent()
        {

        }

        /* The game's Update() method checks if the Escape key or the first GamePad's back button is down.
         * If it is, then the game force quits.
         * Lastly, the parent function is called, which updates every component in the engine's master component list. */
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        /* The Draw() function for the game first clears the screen with a yellow colour.
         * The yellow colour should not show up since the active GameState should draw over it.
         * This means it is obvious if something isn't drawing when it should be (some yellow will show on the screen). */
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Yellow);

            /* Next, the parent Draw() function is called, which draws every component in the master Game.Components list. */
            base.Draw(gameTime);

            /* This block of code handles the calculation of the frames per second (FPS).
             * First, the frameCount variable is incremented since one cycle of the game loop has passed.
             * Then, the timeSinceLastUpdate field is incremented by the amount of time that has passed since the last call to Update(). */
            frameCount++;
            timeSinceLastUpdate += (float)gameTime.ElapsedGameTime.TotalSeconds;

            /* Last, the function checks if the time since the last update to the FPS count is greater than the update interval. */
            if (timeSinceLastUpdate > interval)
            {
                /* The fps is calculated by dividing the number of frames in the last interval by the time since the last update of the count. */
                fps = frameCount / timeSinceLastUpdate;

                /* The FPS can then be displayed to the window's title bar, but it's commented out.
                 * This is because the window title bar is currently used to hold the tile coordinates of the player. */
                // this.Window.Title = "FPS: " + fps.ToString();

                /* The frameCount is reset to zero, and the timeSinceLastUpdate has the interval removed from it. */
                frameCount = 0;
                timeSinceLastUpdate -= interval;
            }
        }
    }
}
