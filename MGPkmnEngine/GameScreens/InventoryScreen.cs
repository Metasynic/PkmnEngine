using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MGPkmnLibrary;
using MGPkmnLibrary.Controls;
using MGPkmnLibrary.PokemonClasses;

namespace PkmnEngine.GameScreens
{
    /* The InventoryScreen allows the user to view the Pokemon currently in their team. */
    public class InventoryScreen : BaseGameState
    {
        /* There are a lot of Controls in the inventory screen.
         * The pokemonSelector is the LeftRightSelector used to scroll between the Pokemon in the team.
         * The pokemonNames array contains the names of the Pokemon in the team.
         * The typeRects field is an array containing 18 Rectangles corresponding to the positions of each type sprite on the typeSheet.
         * The genderRects field is similar, having 3 Rectangles with the positions of each gender sprite on the genderSheet. */
        LeftRightSelector pokemonSelector;
        string[] pokemonNames;
        Rectangle[] typeRects;
        Rectangle[] genderRects;

        /* The backgroundImage is a PictureBox for the image drawn in the background.
         * Four other PictureBoxes are used to display the sprite of the Pokemon, its types, and its gender. */
        PictureBox backgroundImage;
        PictureBox pkmnSprite;
        PictureBox typeSprite;
        PictureBox secondTypeSprite;
        PictureBox genderSprite;

        /* These two Texture2Ds are the two sprite sheets containing all the types and genders that can be drawn. */
        Texture2D typeSheet;
        Texture2D genderSheet;

        /* These labels display information about the Pokemon currently being viewed.
         * In order, they are the name, level, HP (health), attack, defence, special attack, special defence, speed and XP of the Pokemon. */
        Label name;
        Label level;
        Label hp;
        Label atk;
        Label def;
        Label spatk;
        Label spdef;
        Label spd;
        Label xp;

        /* This LinkLabel allows the saving of the game to an external binary file. */
        LinkLabel saveLabel;

        public int SelectedPokemon
        {
            get { return pokemonSelector.SelectedIndex; }
        }

        /* The InventoryScreen has no special constructor and just invokes the BaseGameState constructor. */
        public InventoryScreen(Game game, GameStateManager manager) : base(game, manager)
        {

        }

        /* As always, Initialize() just calls its parent. */
        public override void Initialize()
        {
            base.Initialize();
        }

        /* The LoadContent() method calls the LoadImages() and CreateControls() functions. */
        protected override void LoadContent()
        {
            base.LoadContent();
            LoadImages();
            CreateControls();
        }

        /* The Update() functon for the InventoryScreen updates the ControlManager, and handles input to switch back to the WorldScreen. */
        public override void Update(GameTime gameTime)
        {
            ControlManager.Update(gameTime, PlayerIndex.One);

            /* If the "I" key or Start button is released, then the InventoryScreen is popped off the stack of states.
             * The engine should then return to the WorldScreen. */
            if (InputHandler.KeyReleased(Keys.I) || InputHandler.ButtonReleased(Buttons.Start, PlayerIndex.One))
            {
                Change(ChangeType.Pop, null);
            }

            /* The parent Update() function is also called. */
            base.Update(gameTime);
        }

        /* Draw() follows a similar pattern to Draw() functions in other screens. */
        public override void Draw(GameTime gameTime)
        {
            /* The Begin() method is called with a set of arguments that display small sprites clearly when scaled up.
             * This ensures that the sprites are not blurred by anti-aliasing. */
            GameRef.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
            base.Draw(gameTime);

            /* All the controls are drawn to a buffer using the SpriteBatch. */
            ControlManager.Draw(GameRef.SpriteBatch);

            /* Finally, the contents of the buffer are sent to the screen. */
            GameRef.SpriteBatch.End();
        }

        /* The LoadImages() method loads the two sprite sheets and generates Rectangle arrays containing the locations of individual sprites. */
        private void LoadImages()
        {
            /* The typeSheet is loaded into the game using Content.Load<T>().
             * Next, the typeRects array is initialized with a length of 18 (the number of types on the sheet).
             * The Rectangle on the sprite sheet for each sprite is calculated in a for loop.
             * Since the sprites are arranged in 6 rows and 3 columns, and each sprite is 105 by 40 pixels,
             * the x position of the sprite is calculated from the remainder of dividing the index by 3 (multiplied by the sprite width),
             * and the y position is found by using integer division by 3, which determines the row (and is multiplied by the sprite height). */
            typeSheet = Game.Content.Load<Texture2D>(@"Tilesets/TypeLabels");
            typeRects = new Rectangle[18];
            for (int i = 0; i < typeRects.Length; i++)
            {
                int x = (i % 3) * 105;
                int y = (i / 3) * 40;
                typeRects[i] = new Rectangle(x, y, 105, 40);
            }

            /* The gender sprites work in exactly the same way as the type sprites, except the sprites are 16 by 16 pixels.
             * In addition, there are only three sprites and they're all on the same row. */
            genderSheet = Game.Content.Load<Texture2D>(@"Tilesets/Genders");
            genderRects = new Rectangle[3];
            for (int i = 0; i < 3; i++)
            {
                int x = i * 16;
                genderRects[i] = new Rectangle(x, 0, 16, 16);
            }
        }

        /* The CreateControls() method for the InventoryScreen is quite long but doesn't require much explanation. */
        private void CreateControls()
        {
            /* The three textures for the LeftRightSelector are loaded in. */
            Texture2D leftTexture = Game.Content.Load<Texture2D>(@"GUI/leftArrowUp");
            Texture2D rightTexture = Game.Content.Load<Texture2D>(@"GUI/rightArrowUp");
            Texture2D stopTexture = Game.Content.Load<Texture2D>(@"GUI/stopBar");

            /* The background image is loaded in with Content.Load<T>() and given the window as the destination rectangle.
             * Every Control is added to the ControlManager. */
            backgroundImage = new PictureBox(Game.Content.Load<Texture2D>(@"Backgrounds/startscreen"), GameRef.ScreenRectangle);
            ControlManager.Add(backgroundImage);

            /* Each sprite's PictureBox is constructed using the sprites applying to the first Pokemon in the Player's inventory.
             * In the cases of the Pokemon sprite, the image is obtained by finding it (by its ID) in the DataManager's master list of Pokemon sprites.
             * The type and gender sprites are found by loading in the whole sprite sheet, and setting the source rectangle using the array of source rectangles.
             * The second type sprite is not always used, since Pokemon can be single or dual type.
             * If the first Pokemon in the Player's team is single type, the second type sprite is initialized with a source rectangle of size zero.
             * Effectively, this means nothing will be drawn to the screen for the second type. */
            pkmnSprite = new PictureBox(DataManager.PkmnFrontSprites[WorldScreen.Player.Team[0].PokemonID], new Rectangle(20, 100, 288, 288));
            ControlManager.Add(pkmnSprite);
            typeSprite = new PictureBox(typeSheet, typeRects[(int)WorldScreen.Player.Team[0].Type[0]], new Rectangle(50, 400, 105, 40));
            ControlManager.Add(typeSprite);
            if (WorldScreen.Player.Team[0].Type.Count > 1)
                secondTypeSprite = new PictureBox(typeSheet, typeRects[(int)WorldScreen.Player.Team[0].Type[1]], new Rectangle(175, 400, 105, 40));
            else
                secondTypeSprite = new PictureBox(typeSheet, new Rectangle(), new Rectangle(175, 400, 105, 40));
            ControlManager.Add(secondTypeSprite);
            genderSprite = new PictureBox(genderSheet, genderRects[(int)WorldScreen.Player.Team[0].PokemonGender], new Rectangle(300, 380, 64, 64));
            ControlManager.Add(genderSprite);
            
            /* The pokemonSelector is created by passing in the three textures loaded in earlier.
             * The items for the pokemonSelector are set to the pokemonNames array with a maximum width of 175.
             * Its SelectionChanged event handler is wired to call pokemonSelector_SelectionChanged(). */
            pokemonSelector = new LeftRightSelector(leftTexture, rightTexture, stopTexture);
            pokemonSelector.SetItems(pokemonNames, 175);
            pokemonSelector.Position = new Vector2(300, 100);
            pokemonSelector.SelectionChanged += new EventHandler(pokemonSelector_SelectionChanged);
            ControlManager.Add(pokemonSelector);

            /* Each of the labels on the screen are constructed in the same way.
             * After initialization, the Text property is set using the properties of the current Pokemon.
             * Size is calculated using the SpriteFont.MeasureString() method.
             * Finally, the position is set to a Vector2. The last six labels (displaying level and stats) are aligned to x = 400. */
            name = new Label();
            name.Text = "Inventory";
            name.Size = name.SpriteFont.MeasureString(name.Text);
            name.Position = new Vector2((GameRef.Window.ClientBounds.Width - name.Size.X) / 2, 50);
            ControlManager.Add(name);
            level = new Label();
            level.Text = "Lv " + WorldScreen.Player.Team[0].Level;
            level.Size = level.SpriteFont.MeasureString(level.Text);
            level.Position = new Vector2(400, 150);
            ControlManager.Add(level);
            hp = new Label();
            hp.Text = "HP: " + WorldScreen.Player.Team[0].Health.CurrentValue + "/" + WorldScreen.Player.Team[0].Health.MaximumValue;
            hp.Size = hp.SpriteFont.MeasureString(hp.Text);
            hp.Position = new Vector2(400, 250);
            ControlManager.Add(hp);
            atk = new Label();
            atk.Text = "ATK: " + WorldScreen.Player.Team[0].Attack;
            atk.Size = atk.SpriteFont.MeasureString(atk.Text);
            atk.Position = new Vector2(400, 280);
            ControlManager.Add(atk);
            def = new Label();
            def.Text = "DEF: " + WorldScreen.Player.Team[0].Defence;
            def.Size = def.SpriteFont.MeasureString(def.Text);
            def.Position = new Vector2(400, 310);
            ControlManager.Add(def);
            spatk = new Label();
            spatk.Text = "SP. ATK: " + WorldScreen.Player.Team[0].SpecialAttack;
            spatk.Size = spatk.SpriteFont.MeasureString(spatk.Text);
            spatk.Position = new Vector2(400, 340);
            ControlManager.Add(spatk);
            spdef = new Label();
            spdef.Text = "SP. DEF: " + WorldScreen.Player.Team[0].SpecialDefence;
            spdef.Size = spdef.SpriteFont.MeasureString(spdef.Text);
            spdef.Position = new Vector2(400, 370);
            ControlManager.Add(spdef);
            spd = new Label();
            spd.Text = "SPD: " + WorldScreen.Player.Team[0].Speed;
            spd.Size = spd.SpriteFont.MeasureString(spd.Text);
            spd.Position = new Vector2(400, 400);
            ControlManager.Add(spd);
            xp = new Label();
            xp.Text = "XP: " + WorldScreen.Player.Team[0].XP + "/" + PkmnUtils.NextLevel(DataManager.PokemonSpecies[WorldScreen.Player.Team[0].PokemonID].GrowthRate, WorldScreen.Player.Team[0].Level);
            xp.Size = xp.SpriteFont.MeasureString(xp.Text);
            xp.Position = new Vector2(200, 430);
            ControlManager.Add(xp);

            /* The saveLabel is constructed and set with its text, size, and position. It is set with the saveLabel_Selected() function as its Selected event. */
            saveLabel = new LinkLabel();
            saveLabel.Text = "Save";
            saveLabel.Size = saveLabel.SpriteFont.MeasureString(saveLabel.Text);
            saveLabel.Position = new Vector2(400, 430);
            saveLabel.Selected += new EventHandler(saveLabel_Selected);
            ControlManager.Add(saveLabel);

            /* ControlManager.NextControl() is called to have a control already selected when the user enters the screen. */
            ControlManager.NextControl();
        }

        /* This function updates the list of Pokemon to reflect the Player's team.
         * It is called whenever the user switches from the WorldScreen to the InventoryScreen.
         * It initializes the array with the same length as the team, then fills it with the nicknames of the team. */
        public void UpdateNames()
        {
            pokemonNames = new string[WorldScreen.Player.Team.Count(s => s != null)];
            for (int i = 0; i < pokemonNames.Length; i++)
            {
                pokemonNames[i] = WorldScreen.Player.Team[i].Nickname;
            }
        }

        /* This function is attached to the pokemonSelector.SelectionChanged event handler.
         * Therefore it will be triggered when the user scrolls from one Pokemon to another in the list. */
        void pokemonSelector_SelectionChanged(object sender, EventArgs e)
        {
            /* Firstly, the Pokemon sprite is changed so that it displays the sprite for the newly selected Pokemon.
             * It does this by getting the new sprite from the DataManager's PkmnFrontSprites dictionary. */
            pkmnSprite.Image = DataManager.PkmnFrontSprites[WorldScreen.Player.Team[SelectedPokemon].PokemonID];

            /* Next, the Text property of each label is changed to reflect the stats of the new Pokemon. */
            level.Text = "Lv " + WorldScreen.Player.Team[SelectedPokemon].Level;
            hp.Text = "HP: " + WorldScreen.Player.Team[SelectedPokemon].Health.CurrentValue + "/" + WorldScreen.Player.Team[SelectedPokemon].Health.MaximumValue;
            atk.Text = "ATK: " + WorldScreen.Player.Team[SelectedPokemon].Attack;
            def.Text = "DEF: " + WorldScreen.Player.Team[SelectedPokemon].Defence;
            spatk.Text = "SP. ATK: " + WorldScreen.Player.Team[SelectedPokemon].SpecialAttack;
            spdef.Text = "SP. DEF: " + WorldScreen.Player.Team[SelectedPokemon].SpecialDefence;
            spd.Text = "SPD: " + WorldScreen.Player.Team[SelectedPokemon].Speed;
            xp.Text = "XP: " + WorldScreen.Player.Team[SelectedPokemon].XP + "/" + PkmnUtils.NextLevel(DataManager.PokemonSpecies[WorldScreen.Player.Team[SelectedPokemon].PokemonID].GrowthRate, WorldScreen.Player.Team[SelectedPokemon].Level);

            /* Finally, the source rectangles of the type and gender sprites are changed to the rectangles for the new Pokemon's type(s) and gender.
             * If the Pokemon is dual type, the second type sprite is updated correctly.
             * If it's single type, the source rectangle of the second type sprite is changed to null so it doesn't draw anything. */
            typeSprite.SourceRectangle = typeRects[(int)WorldScreen.Player.Team[SelectedPokemon].Type[0]];
            if (WorldScreen.Player.Team[SelectedPokemon].Type.Count > 1)
                secondTypeSprite.SourceRectangle = typeRects[(int)WorldScreen.Player.Team[SelectedPokemon].Type[1]];
            else
                secondTypeSprite.SourceRectangle = new Rectangle();
            genderSprite.SourceRectangle = genderRects[(int)WorldScreen.Player.Team[SelectedPokemon].PokemonGender];
        }

        /* This function is called when the saveLabel is selected.
         * A new SaveData object is created using the current game, and saved externally using the PkmnUtils.BinarySave<T>() function.
         * The save file is named after the date and time when it was saved. */
        void saveLabel_Selected(object sender, EventArgs e)
        {
            SaveData saveData = new SaveData(WorldScreen.World.CurrentLevelIndex, WorldScreen.Player.TilePosition, (Pokemon[])WorldScreen.Player.Team.Clone(), WorldScreen.Player.Gender);
            PkmnUtils.BinarySave(saveData, DateTime.Now.ToString("dd-MM-yyyy_hh-mm-ss"));
        }
    }
}
