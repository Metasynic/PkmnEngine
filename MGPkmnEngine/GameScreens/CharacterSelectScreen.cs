using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MGPkmnLibrary;
using MGPkmnLibrary.Controls;
using MGPkmnLibrary.SpriteClasses;
using MGPkmnLibrary.WorldClasses;
using MGPkmnLibrary.CharacterClasses;
using PkmnEngine.Components;

namespace PkmnEngine.GameScreens
{
    /* The CharacterSelectScreen is the screen where the player chooses whether they want to play as the male or female character. */
    public class CharacterSelectScreen : BaseGameState
    {
        /* There are three controls on the screen. The characterSelector is a LeftRightSelector uses to choose between female or male.
         * The backgroundImage is the PictureBox holding the image in the background of the screen.
         * The characterImage shows a preview of the art of the currently selected character. */
        LeftRightSelector characterSelector;
        PictureBox backgroundImage;
        PictureBox characterImage;

        /* The characterImages array stores the two art preview images of the male and female player character.
         * The characterItems array stores the two selections that will be available from the LeftRightSelector.
         * The SelectedCharacter property exposes the current string in characterItems that is selected. */
        Texture2D[] characterImages;
        string[] characterItems = { "Male", "Female" };
        public string SelectedCharacter
        {
            get { return characterSelector.SelectedItem; }
        }

        /* The constructor for the screen does nothing special outside of invoking its parent BaseGameState() constructor. */
        public CharacterSelectScreen(Game game, GameStateManager stateManager) : base(game, stateManager)
        {

        }

        /* The Initialize() function calls its parent function. */
        public override void Initialize()
        {
            base.Initialize();
        }

        /* The LoadContent() method calls the internal LoadImages() and CreateControls() methods to load the content. */
        protected override void LoadContent()
        {
            base.LoadContent();
            LoadImages();
            CreateControls();
        }

        /* The CharacterSelectScreen only needs have its ControlManager updated. */
        public override void Update(GameTime gameTime)
        {
            ControlManager.Update(gameTime, PlayerIndex.One);
            base.Update(gameTime);
        }

        /* The Draw() function for the screen follows a basic pattern used in other screens.
         * It calls Begin() on the game's SpriteBatch to prepare it to draw to the screen.
         * The function calls ControlManager.Draw() to draw all the controls to an internal buffer.
         * Finally, End() is called on the SpriteBatch to actually send the drawn buffer to the screen. */
        public override void Draw(GameTime gameTime)
        {
            GameRef.SpriteBatch.Begin();
            base.Draw(gameTime);
            ControlManager.Draw(GameRef.SpriteBatch);
            GameRef.SpriteBatch.End();
        }

        /* CreateControls() initializes every Control object required by the screen. */
        private void CreateControls()
        {
            /* First, the three textures to be used in the LeftRightSelector are loaded using Content.Load<T>(). */
            Texture2D leftTexture = Game.Content.Load<Texture2D>(@"GUI/leftArrowUp");
            Texture2D rightTexture = Game.Content.Load<Texture2D>(@"GUI/rightArrowUp");
            Texture2D stopTexture = Game.Content.Load<Texture2D>(@"GUI/stopBar");

            /* The background image is loaded in to a PictureBox object, with the screen rectangle as the destination rectangle.
             * The newly created control is added to the ControlManager (as all the other controls will be). */
            backgroundImage = new PictureBox(Game.Content.Load<Texture2D>(@"Backgrounds/startscreen"), GameRef.ScreenRectangle);
            ControlManager.Add(backgroundImage);

            /* The title label displays the text "Choose your character" on the screen.
             * A label's size is calculated using the SpriteFont.MeasureString() method.
             * Its position is centralized in the window by setting it to half the window width minus the text width. */
            Label title = new Label();
            title.Text = "Choose your character";
            title.Size = title.SpriteFont.MeasureString(title.Text);
            title.Position = new Vector2((GameRef.Window.ClientBounds.Width - title.Size.X) / 2, 50);
            ControlManager.Add(title);

            /* The characterSelector is the LeftRightSelector enabling a user to choose a character.
             * Its items are set to the characterItems array, with a maximum width of 125 pixels.
             * Its position is aligned with the title label but placed further down.
             * The selector's SelectionChanged event handler is set to call selectionChanged(). */
            characterSelector = new LeftRightSelector(leftTexture, rightTexture, stopTexture);
            characterSelector.SetItems(characterItems, 125);
            characterSelector.Position = new Vector2(title.Position.X, 200);
            characterSelector.SelectionChanged += new EventHandler(selectionChanged);
            ControlManager.Add(characterSelector);

            /* This LinkLabel will allow the user to finalize their choice of character and proceed to the next screen.
             * Its text is "Confirm" and its position is aligned to the title, but further down the screen.
             * Its Selected event handler is set to call confirmCharacter_Selected(). */
            LinkLabel confirmCharacter = new LinkLabel();
            confirmCharacter.Text = "Confirm";
            confirmCharacter.Position = new Vector2(title.Position.X, 300);
            confirmCharacter.Selected += new EventHandler(confirmCharacter_Selected);
            ControlManager.Add(confirmCharacter);

            /* The characterImage PictureBox is initialized with the first character image.
             * The source rectangle is the entire image, and the destination rectangle is the same size on the screen. */
            characterImage = new PictureBox(characterImages[0], new Rectangle(0, 0, 140, 300), new Rectangle(400, 150, 140, 300));
            ControlManager.Add(characterImage);

            /* NextControl() is called on the ControlManager to make sure it has a valid Control selected. */
            ControlManager.NextControl();
        }

        /* The LoadImages() function loads the two character images in to the engine.
         * It initializes the characterImages array and fills it with the two Texture2Ds loaded in. */
        private void LoadImages()
        {
            characterImages = new Texture2D[characterItems.Length];
            for (int i = 0; i < characterItems.Length; i++)
            {
                characterImages[i] = Game.Content.Load<Texture2D>(@"Tilesets/" + characterItems[i] + "Art");
            }
        }

        /* This function is called when the LeftRightSelector's selection is changed.
         * It updates the preview image to match the currently selected character. */
        void selectionChanged(object sender, EventArgs e)
        {
            characterImage.Image = characterImages[characterSelector.SelectedIndex];
        }

        /* This function is called when the confirmCharacter LinkLabel is selected. */
        void confirmCharacter_Selected(object sender, EventArgs e)
        {
            /* First, the input handler is flushed so the last input states match the current states.
             * This ensures nothing is marked as pressed or released while the transition occurs. */
            InputHandler.Flush();

            /* Next, the internal CreatePlayer() and CreateWorld() methods are called to initialize the player and world. */
            CreateWorld();
            CreatePlayer();

            /* Finally, the game state is changed and the engine moves on to the StarterSelectScreen. */
            Change(ChangeType.Change, GameRef.StarterSelectScreen);
        }

        /* This method creates the player in preparation for loading the world. */
        private void CreatePlayer()
        {
            /* The correct sprite sheet for the chosen character is loaded in using Content.Load<T>(). */
            Texture2D playerSpriteSheet = Game.Content.Load<Texture2D>(@"Tilesets/" + characterSelector.SelectedItem);

            /* The Dictionary of animations for the character sprite is initialized.
             * The four different animations, each three tiles long, are created separately and added with the correct AnimationKey.
             * Each animation is added to the Dictionary. */
            Dictionary<AnimationKey, Animation> animations = new Dictionary<AnimationKey, Animation>();
            Animation animation = new Animation(3, 16, 22, 0, 0);
            animations.Add(AnimationKey.Down, animation);
            animation = new Animation(3, 16, 22, 0, 22);
            animations.Add(AnimationKey.Left, animation);
            animation = new Animation(3, 16, 22, 0, 44);
            animations.Add(AnimationKey.Right, animation);
            animation = new Animation(3, 16, 22, 0, 66);
            animations.Add(AnimationKey.Up, animation);

            /* The player sprite itself is created using the sprite sheet, animation dictionary, and a default starting location of (1,1). */
            AnimatedSprite sprite = new AnimatedSprite(playerSpriteSheet, animations, new Point(1, 1));

            /* The Character object for the player is initialized with the sprite and the name "Player". */
            Character character = new Character(sprite, "Player");

            /* The Player property in the WorldScreen is created with the Character object and a reference to the game. */
            WorldScreen.Player = new Player(GameRef, character, characterSelector.SelectedItem, WorldScreen.World);
        }

        /* This method creates the WorldScreen's World. */
        private void CreateWorld()
        {
            /* The commented out block of code here creates a test Beery item and adds it to the level.
             * As it is not quite working and not in the user requirements, it is not being used. */
            /* Berry testBerry = new Berry(BerryType.Razz);
            BaseSprite testBaseSprite = new BaseSprite(itemTexture, new Rectangle(64, 32, 32, 32), new Point(10, 10));
            ItemSprite testItemSprite = new ItemSprite(testBaseSprite, testBerry);
            level.Items.Add(testItemSprite); */

            /* A new World object is created using references to the game and the window rectangle.
             * Its list of Levels is set to DataManager.Levels (the master Level list).
             * The starting level index is currently "Level 1" (the first area in the engine). */
            World world = new World(GameRef, GameRef.ScreenRectangle);
            world.Levels = DataManager.Levels;
            world.CurrentLevelIndex = "Level 1";

            /* The list of spawns for the first level is initialized.
             * In the future, spawns may be editable in the level editor, but it's not a part of the user requirements.
             * The two spawns added to the list mean that if a battle is triggered in the first level,
             * there is a 50% chance it will be a Lv2-3 Caterpie, and a 50% chance it will be a Lv2-3 Metapod. */
            List<PokemonSpawn> firstSpawns = new List<PokemonSpawn>();
            PokemonSpawn spawn = new PokemonSpawn(0.5, 10, 2, 3);
            firstSpawns.Add(spawn);
            spawn = new PokemonSpawn(0.5, 11, 2, 3);
            firstSpawns.Add(spawn);

            /* The second level's spawn list has two entries.
             * There is a three quarter chance of encountering a Pidgey between Lv4-5.
             * There is a quarter chance of encountering a Pidgeotto between Lv4-5. */
            List<PokemonSpawn> secondSpawns = new List<PokemonSpawn>();
            spawn = new PokemonSpawn(0.75, 16, 4, 5);
            secondSpawns.Add(spawn);
            spawn = new PokemonSpawn(0.25, 17, 4, 5);
            secondSpawns.Add(spawn);

            /* There are three PokemonSpawn objects in the third level's list.
             * 60% of the time, the player will run into a Lv6-7 Abra.
             * 30% of the time, the player will run into a Lv6-7 Kadabra.
             * 10% of the time, the player will run into a Lv6-7 Alakazam. */
            List<PokemonSpawn> thirdSpawns = new List<PokemonSpawn>();
            spawn = new PokemonSpawn(0.6, 63, 6, 7);
            thirdSpawns.Add(spawn);
            spawn = new PokemonSpawn(0.3, 64, 6, 7);
            thirdSpawns.Add(spawn);
            spawn = new PokemonSpawn(0.1, 65, 6, 7);
            thirdSpawns.Add(spawn);

            /* The spawns are added to the correct levels, as well as an encounter rate of VeryCommon.
             * The WorldScreen.World is then set to the World that was just created. */
            world.Levels["Level 1"].GrassSpawns = firstSpawns;
            world.Levels["Level 1"].GrassEncounterRate = EncounterRate.VeryCommon;
            world.Levels["Level 2"].GrassSpawns = secondSpawns;
            world.Levels["Level 2"].GrassEncounterRate = EncounterRate.VeryCommon;
            world.Levels["Level 3"].GrassSpawns = thirdSpawns;
            world.Levels["Level 3"].GrassEncounterRate = EncounterRate.VeryCommon;
            WorldScreen.World = world;
        }
    }
}