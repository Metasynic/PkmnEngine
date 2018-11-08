using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MGPkmnLibrary;
using MGPkmnLibrary.Controls;
using MGPkmnLibrary.BattleClasses;

namespace PkmnEngine.GameScreens
{
    /* The SwitchScreen is displayed during a battle when a player Pokemon faints, or the user chooses to switch.
     * It allows the user to choose a new Pokemon to send out in the battle. */
    public class SwitchScreen : BaseGameState
    {
        /* The selectedIndex variable holds the number representing the currently selected Pokemon in the team. */
        int selectedIndex;
        public int SelectedIndex
        {
            get { return selectedIndex; }
        }

        /* There is a PictureBox for the background of the screen, and an array of images for the sprites of each Pokemon.
         * An array of LinkLabels holds the selection buttons for each Pokemon.
         * The detailPreviews array holds Labels for the level and HP of each of the Pokemon. */
        PictureBox backgroundImage;
        PictureBox[] spritePreviews;
        LinkLabel[] namePreviews;
        Label[] detailPreviews;

        /* Like all the other screen constructors, this one is empty. */
        public SwitchScreen(Game game, GameStateManager manager) : base(game, manager)
        {
            
        }

        /* Initialize() calls its parent as per usual. */
        public override void Initialize()
        {
            base.Initialize();
        }

        /* The LoadContent() function calls its parent and then the CreateControls() method. */
        protected override void LoadContent()
        {
            base.LoadContent();
            CreateControls();
        }

        /* The Update() function handles input for the screen. It starts by updating all the controls on the screen through the ControlManager. */
        public override void Update(GameTime gameTime)
        {
            ControlManager.Update(gameTime, playerIndexInControl);

            /* If the Up Arrow key is released, then the selected control is going to move up.
             * This means selectedIndex needs to decrease, unless it's the first item, in which case it should wrap around to the end of the list.
             * If the selectedIndex is less than one, the index will be set to the Pokemon sprite list length minus one.
             * Otherwise it's just decremented normally. */
            if (InputHandler.KeyReleased(Keys.Up))
            {
                if (selectedIndex <= 0)
                    selectedIndex = spritePreviews.Length - 1;
                else
                    selectedIndex--;
            }

            /* Similarly, if the DownArrow is released, the selected control is going to move down.
             * If the selected index is at the end of the sprite list, then it wraps around to the beginning and is set to zero.7
             * Otherwise, it increments to the next item. */
            else if (InputHandler.KeyReleased(Keys.Down))
            {
                if (selectedIndex >= spritePreviews.Length - 1)
                    selectedIndex = 0;
                else
                    selectedIndex++;
            }

            /* Finally, the parent Update() function is called. */
            base.Update(gameTime);
        }

        /* The Draw() function begins the SpriteBatch with settings that optimize drawing of low-resolution sprites.
         * It calls the parent Draw() function and the ControlManager's Draw() to render all the controls.
         * The SpriteBatch is then ended, which sends the drawn graphics objects to the screen. */
        public override void Draw(GameTime gameTime)
        {
            GameRef.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
            base.Draw(gameTime);
            ControlManager.Draw(GameRef.SpriteBatch);
            GameRef.SpriteBatch.End();
        }

        /* CreateControls() is an internal method that creates the objects on the screen. */
        private void CreateControls()
        {
            /* The backgroundImage is constructed using a texture loaded in with Content.Load<T>(), and added to the ControlManager. */
            backgroundImage = new PictureBox(Game.Content.Load<Texture2D>(@"Backgrounds/startscreen"), GameRef.ScreenRectangle);
            ControlManager.Add(backgroundImage);

            /* Three arrays for sprites, names, and other details are initialized with a length the same as the player's team.
             * A local variable, currentPokemon, is declared to hold the current Pokemon being processed. */
            spritePreviews = new PictureBox[GameRef.BattleScreen.Battle.PlayerTeam.Count];
            namePreviews = new LinkLabel[GameRef.BattleScreen.Battle.PlayerTeam.Count];
            detailPreviews = new Label[GameRef.BattleScreen.Battle.PlayerTeam.Count];
            PokemonInBattle currentPokemon;
            for (int i = 0; i < spritePreviews.Length; i++)
            {
                /* The currentPokemon variable is set with a reference to the current Pokemon in the player team during this iteration of the loop.
                 * The spritePreview for the current Pokemon is constructed using the front sprite for the Pokemon with currentPokemon's ID.
                 * The namePreview is set as a new LinkLabel with a position aligned to the sprite preview.
                 * Its Selected event handler is connected to the OnPokemonSelected function. 
                 * Finally, the detailPreview for the current Pokemon is set with the current level and HP of the Pokemon.
                 * Its position is aligned with the sprite and name previews. */
                currentPokemon = GameRef.BattleScreen.Battle.PlayerTeam[i];
                spritePreviews[i] = new PictureBox(DataManager.PkmnFrontSprites[currentPokemon.PokemonID], new Rectangle(20, (i * 50) + 10, 48, 48));
                ControlManager.Add(spritePreviews[i]);
                namePreviews[i] = new LinkLabel();
                namePreviews[i].Text = currentPokemon.Nickname;
                namePreviews[i].Size = namePreviews[i].SpriteFont.MeasureString(namePreviews[i].Text);
                namePreviews[i].Position = new Vector2(100, (i * 50) + 10);
                namePreviews[i].Selected += new EventHandler(OnPokemonSelected);
                ControlManager.Add(namePreviews[i]);
                detailPreviews[i] = new Label();
                detailPreviews[i].Text = "Lv " + currentPokemon.Level + "    HP: " + currentPokemon.Health.CurrentValue + "/" + currentPokemon.Health.MaximumValue;
                detailPreviews[i].Size = detailPreviews[i].SpriteFont.MeasureString(detailPreviews[i].Text);
                detailPreviews[i].Position = new Vector2(300, (i * 50) + 10);
                ControlManager.Add(detailPreviews[i]);

                /* If the current Pokemon is fainted, its name preview will be disabled so that it can't be sent out into battle.
                 * NextControl() is then called so that the selection moves to the next enabled control.
                 * Otherwise, the name preview will be enabled. */
                if (currentPokemon.Fainted)
                {
                    namePreviews[i].Enabled = false;
                    ControlManager.NextControl();
                }
                else
                {
                    namePreviews[i].Enabled = true;
                }
            }
            
            /* NextControl() is called again to ensure that a valid control is selected, even if none of them were disabled previously. */
            ControlManager.NextControl();

            /* This block sets the initial selectedIndex for the SwitchScreen.
             * It starts at the current selectedIndex, and checks each item in the list.
             * The modulo operator is used to ensure that an item in the list is checked, even when j is larger than the list length.
             * It sets the selectedIndex to the next namePreview that's enabled. This avoids the selectedIndex trying to select a disabled LinkLabel. */
            for (int j = selectedIndex; j < selectedIndex + namePreviews.Length; j++)
            {
                if (namePreviews[j % namePreviews.Length].Enabled)
                {
                    selectedIndex = j % namePreviews.Length;
                    break;
                }
            }
        }

        /* This function, as the name implies, resets the SwitchScreen to update every control so it represents the current state of the player team.
         * It gets called every time the screen is pushed onto the GameStateManager stack, so that it always has the latest information. */
        public void Reset()
        {
            /* The engine checks if any of the preview arrays are null. If they are, the code is skipped so it doesn't cause a null reference. */
            if (spritePreviews != null && namePreviews != null && detailPreviews != null)
            {
                /* Similarly to in CreateControls(), this is a temporary variable to hold the current Pokemon being dealt with.
                 * A for loop iterates through the spritePreviews array. */
                PokemonInBattle currentPokemon;
                for(int i = 0; i < spritePreviews.Length; i++)
                {
                    /* A reference to the current Pokemon being processed by the loop is set into the temporary variable.
                     * Each image/text field in the preview for the Pokemon is updated to its current value.
                     * If the Pokemon is fainted, its LinkLabel will be disabled, and vice versa. */
                    currentPokemon = GameRef.BattleScreen.Battle.PlayerTeam[i];
                    spritePreviews[i].Image = DataManager.PkmnFrontSprites[currentPokemon.PokemonID];
                    namePreviews[i].Text = currentPokemon.Nickname;
                    detailPreviews[i].Text = "Lv " + currentPokemon.Level + "    HP: " + currentPokemon.Health.CurrentValue + "/" + currentPokemon.Health.MaximumValue;
                    if (currentPokemon.Fainted)
                    {
                        namePreviews[i].Enabled = false;
                    }
                    else
                    {
                        namePreviews[i].Enabled = true;
                    }
                }

                /* The ControlManager goes to the next valid control to make sure that a disabled control isn't selected. */
                ControlManager.NextControl();

                /* This block does the same thing it did in CreateControls(). It sets the selectedIndex to the first enabled control it can find. */
                for (int j = selectedIndex; j < selectedIndex + namePreviews.Length; j++)
                {
                    if (namePreviews[j % namePreviews.Length].Enabled)
                    {
                        selectedIndex = j % namePreviews.Length;
                        break;
                    }
                }
            }
        }

        /* This function is run when a Pokemon's LinkLabel is selected. It pops the SwitchScreen off the stack so the engine will return to the BattleScreen. */
        void OnPokemonSelected(object sender, EventArgs e)
        {
            Change(ChangeType.Pop, null);
        }
    }
}
