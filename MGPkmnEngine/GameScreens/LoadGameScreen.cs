using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MGPkmnLibrary;
using MGPkmnLibrary.Controls;
using MGPkmnLibrary.SpriteClasses;
using MGPkmnLibrary.PokemonClasses;
using MGPkmnLibrary.WorldClasses;
using MGPkmnLibrary.CharacterClasses;
using PkmnEngine.Components;

namespace PkmnEngine.GameScreens
{
    /* The LoadGameScreen allows the user to load a previously saved game. */
    public class LoadGameScreen : BaseGameState
    {   
        /* The backgroundImage is the picture drawn first that fills the screen.
         * The loadListBox is a ListBox control which contains a list of the loadable saves.
         * Two LinkLabels allow the user to enter the loadListBox, or go back to the previous screen. */
        PictureBox backgroundImage;
        ListBox loadListBox;
        LinkLabel loadLinkLabel;
        LinkLabel backLinkLabel;

        /* The LoadGameScreen has nothing interesting happening in its constructor. */
        public LoadGameScreen(Game game, GameStateManager manager) : base(game, manager)
        {

        }

        /* The LoadContent() method creates all the controls for the LoadGameScreen. */
        protected override void LoadContent()
        {
            base.LoadContent();

            /* The backgroundImage is loaded in with Content.Load<T>() with the window as the destination rectangle.
             * Each control is added to the ControlManager so it gets updated and drawn. */
            backgroundImage = new PictureBox(Game.Content.Load<Texture2D>(@"Backgrounds/startscreen"), GameRef.ScreenRectangle);
            ControlManager.Add(backgroundImage);

            /* The two Linklabels are created and set with the appropriate text.
             * Their positions are aligned and spaced using SpriteFont.LineSpacing.
             * Both event handlers are wired to loadLinklabel_Selected() and backLinkLabel_Selected() respectively. */
            loadLinkLabel = new LinkLabel();
            loadLinkLabel.Text = "Select Game";
            loadLinkLabel.Position = new Vector2(10, 20);
            loadLinkLabel.Selected += new EventHandler(loadLinkLabel_Selected);
            ControlManager.Add(loadLinkLabel);
            backLinkLabel = new LinkLabel();
            backLinkLabel.Text = "Back";
            backLinkLabel.Position = new Vector2(10, 20 + backLinkLabel.SpriteFont.LineSpacing);
            backLinkLabel.Selected += new EventHandler(backLinkLabel_Selected);
            ControlManager.Add(backLinkLabel);

            /* The loadListBox is initialized with its background image and arrow image.
             * Its position is set to the right hand side of the screen.
             * Selecting the confirm part of the ListBox is wired to the loadListBox_Selected() function.
             * Exiting the ListBox is wired to the loadListBox_Leave() function.
             * An array of the items that can be loaded is created using Directory.GetFiles(). 
             * Each of the names is then added to the ListBox's items.
             * Finally, the ListBox is added to the ControlManager. */
            loadListBox = new ListBox(Game.Content.Load<Texture2D>(@"GUI/listBoxImage"), Game.Content.Load<Texture2D>(@"GUI/rightArrowUp"));
            loadListBox.Position = new Vector2(200, 100);
            loadListBox.Selected += new EventHandler(loadListBox_Selected);
            loadListBox.Leave += new EventHandler(loadListBox_Leave);
            string[] filenames = Directory.GetFiles(@"../../../../../Saves/", "*.pks", SearchOption.TopDirectoryOnly);
            foreach(string filename in filenames)
            {
                loadListBox.Items.Add(Path.GetFileNameWithoutExtension(filename));
            }
            ControlManager.Add(loadListBox);

            /* The NextControl() function is called on the ControlManager to make sure a Control is selected. */
            ControlManager.NextControl();
        }

        /* The Update() function just updates the ControlManager. */
        public override void Update(GameTime gameTime)
        {
            ControlManager.Update(gameTime, PlayerIndex.One);
            base.Update(gameTime);
        }

        /* Much like other Draw() functions for minor screens, the engine's SpriteBatch is initialized.
         * The parent Draw() function is called, followed by drawing the ControlManager (and thus all the Controls).
         * Last, the internal buffer is flushed to the screen by calling End(). */
        public override void Draw(GameTime gameTime)
        {
            GameRef.SpriteBatch.Begin();
            base.Draw(gameTime);
            ControlManager.Draw(GameRef.SpriteBatch);
            GameRef.SpriteBatch.End();
        }

        /* The first event handler function in the class is loadLinkLabel_Selected().
         * This function is called when the player presses "Load" to enter the ListBox.
         * It temporarily disables the ControlManager's input so the ListBox can handle its own input.
         * The focus is changed from the loadLinkLabel to the loadListBox. */
        void loadLinkLabel_Selected(object sender, EventArgs e)
        {
            ControlManager.AcceptsInput = false;
            loadLinkLabel.HasFocus = false;
            loadListBox.HasFocus = true;
        }

        /* The backLinklabel_Selected() function is called when the user presses "Back" to go to the previous screen.
         * It Pops the current LoadGameScreen off the stack, which will return it to the StartScreen. */
        void backLinkLabel_Selected(object sender, EventArgs e)
        {
            Change(ChangeType.Pop, null);
        }

        /* This function is triggered when the player chooses a saved game from the ListBox to be loaded.
         * Focus is returned from the loadListBox to the loadLinkLabel (although it doesn't really matter at this point - the state is about to change).
         * The ControlManager is allowed to accept input again so the user will be able to control the player.
         * Loading the selected save data is accomplished by converting the sender back into a ListBox, 
         * and loading the save file corresponding to the ListBox's selected item.
         * The CreatePlayer() and CreateWorld() functions are called to initialize the world and player.
         * Properties from the loaded data are passed into the functions, thus meaning the data is used to create the player and world.
         * Finally, the state is changed to the WorldScreen. */
        void loadListBox_Selected(object sender, EventArgs e)
        {
            loadLinkLabel.HasFocus = true;
            ControlManager.AcceptsInput = true;
            ListBox box = sender as ListBox;
            SaveData load = PkmnUtils.BinaryLoad<SaveData>(@"../../../../../Saves/" + box.SelectedItem + ".pks");
            CreateWorld(load.LevelID);
            CreatePlayer(load.Gender, load.TileX, load.TileY, load.Team);
            Change(ChangeType.Change, GameRef.WorldScreen);
        }

        /* When control leaves the ListBox, the loadLinkLabel has its focus restored, and the ControlManager accepts input again. */
        void loadListBox_Leave(object sender, EventArgs e)
        {
            loadLinkLabel.HasFocus = true;
            ControlManager.AcceptsInput = true;
        }

        /* The CreatePlayer() and CreateWorld() functions are identical to the ones in the CharacterSelectScreen.
         * Please refer to the CharacterSelectScreen source code for comments on how they work.
         * The only difference is that this CreatePlayer() function takes the gender, starting coordinates, and Pokemon team of the save to be loaded.
         * These parameters are then used when creating the player instead of the default values. */
        private void CreatePlayer(string gender, int startX, int startY, Pokemon[] team)
        {
            Texture2D playerSpriteSheet = Game.Content.Load<Texture2D>(@"Tilesets/" + gender);
            Dictionary<AnimationKey, Animation> animations = new Dictionary<AnimationKey, Animation>();
            Animation animation = new Animation(3, 16, 22, 0, 0);
            animations.Add(AnimationKey.Down, animation);
            animation = new Animation(3, 16, 22, 0, 22);
            animations.Add(AnimationKey.Left, animation);
            animation = new Animation(3, 16, 22, 0, 44);
            animations.Add(AnimationKey.Right, animation);
            animation = new Animation(3, 16, 22, 0, 66);
            animations.Add(AnimationKey.Up, animation);

            AnimatedSprite sprite = new AnimatedSprite(playerSpriteSheet, animations, new Point(startX, startY));
            Character character = new Character(sprite, "Player");
            WorldScreen.Player = new Player(GameRef, character, "", WorldScreen.World);

            WorldScreen.Player.InitTeam(team[0]);
            for (int i = 1; i < team.Length; i++)
            {
                WorldScreen.Player.AddToTeam(team[i]);
            }
        }

        /* CreateWorld() is the same as in CharacterSelectScreen, except it takes a string with the name of the current level.
         * This string is set as the world's current level index. */
        private void CreateWorld(string levelIndex)
        {
            World world = new World(GameRef, GameRef.ScreenRectangle);
            world.Levels = DataManager.Levels;
            world.CurrentLevelIndex = levelIndex;
            WorldScreen.World = world;
        }
    }
}
