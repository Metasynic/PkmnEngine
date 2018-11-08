using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MGPkmnLibrary;
using MGPkmnLibrary.BattleClasses;
using MGPkmnLibrary.PokemonClasses;
using MGPkmnLibrary.Controls;

namespace PkmnEngine.GameScreens
{
    /* The BattleScreen is the game state where battles with wild Pokemon or trainers take place.
     * It's probably the most complex screen in the game. */
    public class BattleScreen : BaseGameState
    {
        /* The BattleScreen contains a large number of fields, which are almost all private.
         * The screen's Battle object is the only one that is exposed, so it can be viewed by the WorldScreen.
         * A Battle object is used to store the current battle happening on the screen.
         * If a client-server model was ever implemented on this engine (NB: not happening),
         * The BattleScreen could be seen as the "client" and the Battle object as the "server". */
        Battle battle;
        public Battle Battle
        {
            get { return battle; }
        }

        /* A BattleLog object is the control that handles all the text being displayed at the bottom of the screen.
         * It informs the user what moves have been used, and how much damage has been done.
         * There is a PictureBox to hold the background image of the BattleScreen.
         * Since the background images are loaded as one single sheet, a Texture2D field is used to hold the whole thing.
         * An array of Rectangles holds the coordinates and dimensions of each individual background on the sheet.
         * The backgroundIndex voice holds the index of the current background being used from the sheet. */
        BattleLog log;
        PictureBox background;
        Texture2D backgroundSheet;
        Rectangle[] backgroundRects;
        byte backgroundIndex;

        /* A FourButton control is used as the player's main menu. The player can use the control to make decisions about what to do in the battle.
         * Two PictureBox objects hold the sprites for the player and opponent's current Pokemon.
         * There are six Labels which display the name, level and health of the player and opponent's Pokemon. */
        FourButton mainMenu;
        PictureBox playerPokemon;
        PictureBox opponentPokemon;
        Label opponentName;
        Label opponentLevel;
        Label opponentHealth;
        Label playerName;
        Label playerLevel;
        Label playerHealth;

        /* These boolean values are all flags used to show whether the screen needs to do something special.
         * The waitingToPop bool means that the battle has ended, so once the BattleLog has finished displaying the text in its list,
         * this BattleScreen should pop and return to the WorldScreen.
         * The switchCompleted bool is set to true when the player has chosen the Pokemon they want to switch out to, meaning the switch needs to be made.
         * The faintCompleted bool is set as true when a player Pokemon has fainted, and they have chosen the next one to send in, so the new Pokemon needs to enter the battle.
         * The executing bool is true when the BattleScreen is currently executing the list of moves for the current turn.
         * The nextMove bool is true when the current turn has finished and the BattleLog has finished displaying the output from that turn. */
        bool waitingToPop;
        bool switchCompleted;
        bool faintCompleted;
        bool executing;
        bool nextMove;

        /* There is nothing special happening in the BattleScreen constructor. */
        public BattleScreen(Game game, GameStateManager manager) : base(game, manager)
        {

        }

        /* The Initialize() function calls its parent. */
        public override void Initialize()
        {
            base.Initialize();
        }

        /* LoadContent() calls its parent function and the private CreateControls() method, which sets up the components on the screen. */
        protected override void LoadContent()
        {
            base.LoadContent();
            CreateControls();
        }

        /* The Update() function handles different scenarios when different flags are true. */
        public override void Update(GameTime gameTime)
        {
            /* First, the labels on the screen are updated. This means changes to health or level are visible to the user. */
            UpdateLabels();

            /* If the user has just chosen the Pokemon they would like to switch in, the flag is set to false.
             * A new TurnSwitch is then added to the turn list, using the current player and Pokemon that was selected.
             * Now that the player turn has been created, DoRound() is called to do everything else necessary for the round to start. */
            if (switchCompleted)
            {
                switchCompleted = false;
                battle.Turns.Add(new TurnSwitch(battle.CurrentPlayer, battle.PlayerTeam[GameRef.SwitchScreen.SelectedIndex], battle, (byte)GameRef.SwitchScreen.SelectedIndex));
                DoRound();
            }

            /* If a player Pokemon has just fainted, and the user has just chosen a new Pokemon to send in, the flag is set to false.
             * Next, all the turns where the fainted Pokemon is the user are removed from the turn list.
             * This means that a fainted Pokemon should not be able to execute a turn.
             * The turn list is iterated through backwards, as changes are being made to the list while iterating through it.
             * A new TurnSwitch is created using the current player and the newly selected Pokemon.
             * The new switch turn is immediately executed, as it doesn't count as a part of the round. */
            if (faintCompleted)
            {
                faintCompleted = false;
                for(int i = battle.Turns.Count - 1; i >= 0; i--)
                {
                    if (battle.Turns[i].User == battle.CurrentPlayer)
                        battle.Turns.Remove(battle.Turns[i]);
                }
                TurnSwitch faintSwitch = new TurnSwitch(battle.CurrentPlayer, battle.PlayerTeam[GameRef.SwitchScreen.SelectedIndex], battle, (byte)GameRef.SwitchScreen.SelectedIndex);
                faintSwitch.Execute();
            }

            /* If the battle's OpenSwitchMenu flag is true, then the SwitchScreen is reset so it has the updated values for the Pokemon on its list.
             * The SwitchScreen is pushed onto the top of the GameStateManager stack, so that it is displayed on the next frame.
             * This will allow the player to choose the next Pokemon they want to send out.
             * The faintCompleted flag is set to true so that once the SwitchScreen pops, the BattleScreen will send the chosen Pokemon out into battle.
             * Finally, the battle's OpenSwitchMenu flag is set to false as it's done its job. */
            if (battle.OpenSwitchMenu && log.FinishedEmptying)
            {
                GameRef.SwitchScreen.Reset();
                Change(ChangeType.Push, GameRef.SwitchScreen);
                faintCompleted = true;
                battle.OpenSwitchMenu = false;
            }

            /* If the battle's EmptyLog flag is true, then the BattleLog control's Empty() function is called,
             * which starts the process of displaying the text buffered in the log onto the screen.
             * The flag is then set to false so that this block doesn't execute again next frame. */
            if (battle.EmptyLog)
            {
                log.Empty(battle.LogList);
                battle.EmptyLog = false;
            }

            /* If the battle's BattleFinished flag is true, then the player's PokemonInBattle from this screen need to be transferred back to the WorldScreen.
             * This is necessary so that changes to health and level will carry over after the battle, instead of being reset to how they were before.
             * The waitingToPop flag is set to true, so that the BattleScreen pops on the next game loop iteration.
             * The WorldScreen's Player's Team is then reset and filled with the BattleScreen's Player's Team, using InitTeam() and AddToTeam(). */
            if (battle.BattleFinished)
            {
                waitingToPop = true;
                WorldScreen.Player.InitTeam(battle.PlayerTeam[0]);
                for (int i = 1; i < battle.PlayerTeam.Count; i++)
                {
                    WorldScreen.Player.AddToTeam(battle.PlayerTeam[i]);
                }
            }

            /* This is where the flag system gets a bit complicated.
             * If the executing and nextMove flags are true, then a turn has just finished.
             * The engine checks if there are any more turns left to execute, by checking the list's Count.
             * If there are moves left, the ExecuteNextTurn() function kickstarts the next turn in the list.
             * The turn that has just been executed is then removed from the list since it isn't needed any more.
             * In this way, the list will gradually empty as the turns are executed.
             * If there are no turns left in the list, the executing flag is set to false, since there is nothing to execute.
             * In both cases, the nextMove flag is set to false, as the next move no longer needs to be checked. */
            if (executing && nextMove)
            {
                if (battle.Turns.Count > 0)
                {
                    battle.ExecuteNextTurn();
                    battle.Turns.Remove(battle.Turns[0]);
                }
                else
                {
                    executing = false;
                }
                nextMove = false;
            }

            /* If the BattleLog's FinishedEmptying flag is true, it's time to check if another move can be executed.
             * This means that the nextMove flag will be set to true if the round is still executing.
             * Either way, ResetBattleScreen() is called to update the sprite images in case a switch has happened,
             * and to reset the menu so that it is ready to be used to get the user's next turn.
             * The log's FinishedEmptying flag is set back to false. The log's list is also cleared.
             * Finally, if the waitingToPop flag is true, it is set back to false and the BattleScreen is popped off the stack. */
            if (log.FinishedEmptying)
            {
                if (executing)
                {
                    nextMove = true;
                }
                ResetBattleScreen();
                log.FinishedEmptying = false;
                battle.LogList.Clear();
                if (waitingToPop)
                {
                    waitingToPop = false;
                    Change(ChangeType.Pop, null);
                }
            }

            /* The ControlManager and battle are also updated during the BattleScreen updating. */
            ControlManager.Update(gameTime, PlayerIndex.One);
            battle.Update(gameTime);

            /* If the user presses the backspace key, and the BattleLog isn't in the middle of displaying anything, the menu will be reset.
             * This essentially means the user can cancel their choice if necessary.
             * Finally, the parent function is called. */
            if (InputHandler.KeyReleased(Keys.Back) && !log.Emptying)
            {
                ResetBattleScreen();
            }
            base.Update(gameTime);
        }

        /* The Draw() function is far simpler than the Update() function.
         * The SpriteBatch is called to start using settings that display the sprites without antialiasing.
         * After that, the parent Draw() function is called, and the ControlManager is drawn.
         * Finally, the SpriteBatch ends, which flushes the drawn sprites out of the SpriteBatch's internal buffer to the screen. */
        public override void Draw(GameTime gameTime)
        {
            GameRef.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
            base.Draw(gameTime);
            ControlManager.Draw(GameRef.SpriteBatch);
            GameRef.SpriteBatch.End();
        }

        /* The CreateControls() function sets up the screen with everything it needs to display properly. */
        private void CreateControls()
        {
            /* First, the background texture sheet is loaded in using Content.Load<T>().
             * Then, a new array is made to hold the Rectangle of each individual background on the sheet.
             * The array is filled with the correct Rectangles, which are each 240x112 pixels. 
             * As there are three backgrounds per row on the sheet, the x coordinate is found using modulo function and the y coordinate by floor division. */
            backgroundSheet = Game.Content.Load<Texture2D>(@"Backgrounds/battlescreen");
            backgroundRects = new Rectangle[12];
            for (int i = 0; i < backgroundRects.Length; i++)
            {
                int x = (i % 3) * 240;
                int y = (i / 3) * 112;
                backgroundRects[i] = new Rectangle(x, y, 240, 112);
            }

            /* The background, player Pokemon, and opponent Pokemon picture boxes are constructed with the images corresponding to the first Pokemon in their team. */
            background = new PictureBox(backgroundSheet, new Rectangle(0, 0, 640, 300));
            ControlManager.Add(background);

            playerPokemon = new PictureBox(DataManager.PkmnBackSprites[battle.CurrentPlayer.PokemonID], new Rectangle(0, 92, 288, 288));
            ControlManager.Add(playerPokemon);

            opponentPokemon = new PictureBox(DataManager.PkmnFrontSprites[battle.CurrentOpponent.PokemonID], new Rectangle(325, 0, 288, 288));
            ControlManager.Add(opponentPokemon);

            /* The FourButton menu is initialized with the FourButton background, and its position and size are set to clamp it to the bottom of the screen.
             * It has the BattleFont and is added to the ControlManager. */
            mainMenu = new FourButton(Game.Content.Load<Texture2D>(@"Backgrounds/fourButton"));
            mainMenu.Position = new Vector2(0, 300);
            mainMenu.Size = new Vector2(GameRef.ScreenRectangle.Width, GameRef.ScreenRectangle.Height - mainMenu.Position.Y);
            mainMenu.SpriteFont = Game.Content.Load<SpriteFont>(@"Fonts/BattleFont");
            ControlManager.Add(mainMenu);

            /* The BattleLog control also uses the FourButton background, and is passed a reference to the battle's log along with 0.1 seconds as the character interval.
             * As it covers the main menu, it has the same size, position, and font. */
            log = new BattleLog(Game.Content.Load<Texture2D>(@"Backgrounds/fourButton"), battle.LogList, 0.1);
            log.Position = mainMenu.Position;
            log.Size = mainMenu.Size;
            log.SpriteFont = mainMenu.SpriteFont;
            ControlManager.Add(log);

            /* Each of the labels on the screen is constructed, set with its position, and added to the ControlManager so it gets updated and drawn. */
            opponentName = new Label();
            opponentName.Position = new Vector2(0, 0);
            ControlManager.Add(opponentName);

            opponentLevel = new Label();
            opponentLevel.Position = new Vector2(0, 20);
            ControlManager.Add(opponentLevel);

            opponentHealth = new Label();
            opponentHealth.Position = new Vector2(0, 40);
            ControlManager.Add(opponentHealth);

            playerName = new Label();
            playerName.Position = new Vector2(400, 200);
            ControlManager.Add(playerName);

            playerLevel = new Label();
            playerLevel.Position = new Vector2(400, 220);
            ControlManager.Add(playerLevel);

            playerHealth = new Label();
            playerHealth.Position = new Vector2(400, 240);
            ControlManager.Add(playerHealth);

            /* Finally, ResetBattleScreen() is called to set the menu up with the initial options and the events attached to each option.
             * UpdateLabels() will set the correct text into each of the labels on the screen. */
            ResetBattleScreen();
            UpdateLabels();
        }

        /* The InitBattle() function prepares the BattleScreen to start a new battle.
         * There are two overloads for the function - one takes a single opponent Pokemon, the other a team of opponents.
         * If there is only one opponent, it is converted into an array with a single item.
         * The array of opponents (or the single item array) is then passed into the Battle constructor along with the player's team.
         * A reference to the PokemonSpecies list is also passed in, and a bool representing whether this is an NPC trainer battle as opposed to a wild battle.
         * The background texture index is set to the index passed in.
         * On all but the first calling of this function, the backgroundRects array will have been initialized.
         * If the array has been created, the background's rectangle is updated to the correct one from the texture array.
         * ResetBattleScreen() and UpdateLabels() are then called to make sure the Pokemon sprites and labels are updated. */
        public void InitBattle(Pokemon[] playerTeam, Pokemon[] opponents, byte index)
        {
            battle = new Battle(playerTeam, opponents, DataManager.PokemonSpecies, true);
            backgroundIndex = index;
            if (backgroundRects != null)
            {
                background.SourceRectangle = backgroundRects[index];
                ResetBattleScreen();
                UpdateLabels();
            }
        }

        public void InitBattle(Pokemon[] playerTeam, Pokemon opponent, byte index)
        {
            Pokemon[] opponents = new Pokemon[1];
            opponents[0] = opponent;
            battle = new Battle(playerTeam, opponents, DataManager.PokemonSpecies, false);
            backgroundIndex = index;
            if (backgroundRects != null)
            {
                background.SourceRectangle = backgroundRects[index];
                ResetBattleScreen();
                UpdateLabels();
            }
        }

        /* This function is called when the "Run" button is chosen from the menu during a battle.
         * It adds one to the player Pokemon's RunCount, a value that affects its chances of running away.
         * A new TurnRun is added to the turn list, with the player Pokemon as the user and opponent Pokemon as the target, along with a reference to the Battle.
         * Finally, DoRound() is called to create the NPC turn and execute the round. */
        public void OnRunSelected(object sender, EventArgs e)
        {
            battle.CurrentPlayer.RunCount++;
            battle.Turns.Add(new TurnRun(battle.CurrentPlayer, battle.CurrentOpponent, battle));
            DoRound();
        }

        /* The OnFightSelected() function is called when the "Fight" button is chosen in the battle.
         * Its purpose is to change the menu so that it allows the user to choose a Move for their Pokemon to use. */
        public void OnFightSelected(object sender, EventArgs e)
        {
            /* Four strings are declared to hold the names of the moves.
             * For each of the strings, the player Pokemon's move array is checked to see if anything is there.
             * If it is, that name is copied into the correct variable.
             * Otherwise, the variable stays null as there is no move in that slot. */
            string firstMove, secondMove, thirdMove, fourthMove;

            if (battle.CurrentPlayer.Moves[0] != null)
                firstMove = battle.CurrentPlayer.Moves[0].Name;
            else
                firstMove = null;

            if (battle.CurrentPlayer.Moves[1] != null)
                secondMove = battle.CurrentPlayer.Moves[1].Name;
            else
                secondMove = null;

            if (battle.CurrentPlayer.Moves[2] != null)
                thirdMove = battle.CurrentPlayer.Moves[2].Name;
            else
                thirdMove = null;

            if (battle.CurrentPlayer.Moves[3] != null)
                fourthMove = battle.CurrentPlayer.Moves[3].Name;
            else
                fourthMove = null;

            /* The FourButton control text is reset by calling SetButtons() with the new strings.
             * All the event handlers are removed by ClearButtonEvents().
             * Finally, four new event handlers for executing the moves are attached to the buttons. */
            mainMenu.SetButtons(firstMove, secondMove, thirdMove, fourthMove);
            mainMenu.ClearButtonEvents();
            mainMenu.FirstButtonPressed += new EventHandler(OnFirstMoveUsed);
            mainMenu.SecondButtonPressed += new EventHandler(OnSecondMoveUsed);
            mainMenu.ThirdButtonPressed += new EventHandler(OnThirdMoveUsed);
            mainMenu.FourthButtonPressed += new EventHandler(OnFourthMoveUsed);
        }

        /* This function is for when the "Switch" button is chosen.
         * It resets the SwitchScreen so that it has the current HP and Level of the player's Pokemon team.
         * The SwitchScreen is then pushed onto the top of the GameStateManager stack.
         * The switchCompleted flag is set to true so that the BattleScreen knows to pull the chosen Pokemon from the SwitchScreen on the next game loop iteration. */
        public void OnSwitchSelected(object sender, EventArgs e)
        {
            GameRef.SwitchScreen.Reset();
            Change(ChangeType.Push, GameRef.SwitchScreen);
            switchCompleted = true;
        }

        /* The next four functions are triggered when one of the moves is chosen after selecting "Fight".
         * In all of them, a new TurnMove is added to the turn list, with the player Pokemon as the user and the opponent Pokemon as the target.
         * The name of the move is also passed in, and DoRound() is called to create an NPC move and start the round. */
        public void OnFirstMoveUsed(object sender, EventArgs e)
        {
            battle.Turns.Add(new TurnMove(battle.CurrentPlayer, battle.CurrentOpponent, battle, DataManager.Moves[((FourButton)sender).Labels[0]]));
            DoRound();
        }
        public void OnSecondMoveUsed(object sender, EventArgs e)
        {
            battle.Turns.Add(new TurnMove(battle.CurrentPlayer, battle.CurrentOpponent, battle, DataManager.Moves[((FourButton)sender).Labels[1]]));
            DoRound();
        }
        public void OnThirdMoveUsed(object sender, EventArgs e)
        {
            battle.Turns.Add(new TurnMove(battle.CurrentPlayer, battle.CurrentOpponent, battle, DataManager.Moves[((FourButton)sender).Labels[2]]));
            DoRound();
        }
        public void OnFourthMoveUsed(object sender, EventArgs e)
        {
            battle.Turns.Add(new TurnMove(battle.CurrentPlayer, battle.CurrentOpponent, battle, DataManager.Moves[((FourButton)sender).Labels[3]]));
            DoRound();
        }

        /* ResetBattleScreen() updates the background sprite and the Pokemon sprites to reflect the currently selected background/Pokemon.
         * The main menu is reset to its inital state, by changing the buttons and clearing the event handlers attached to them.
         * New event handlers are created for each menu option (The third button is not used as it would be "Item" which isn't implemented).
         * The ResetIndex() function is called to change the selected button back to the first one.
         * Finally, the menu is made enabled and visible. */
        private void ResetBattleScreen()
        {
            background.SourceRectangle = backgroundRects[backgroundIndex];
            playerPokemon.Image = DataManager.PkmnBackSprites[battle.CurrentPlayer.PokemonID];
            opponentPokemon.Image = DataManager.PkmnFrontSprites[battle.CurrentOpponent.PokemonID];

            mainMenu.SetButtons("Fight", "Pokemon", "Item", "Run");

            mainMenu.ClearButtonEvents();
            mainMenu.FirstButtonPressed += new EventHandler(OnFightSelected);
            mainMenu.SecondButtonPressed += new EventHandler(OnSwitchSelected);
            mainMenu.FourthButtonPressed += new EventHandler(OnRunSelected);
            mainMenu.ResetIndex();
            mainMenu.Enabled = true;
            mainMenu.Visible = true;
        }

        /* This method updates the text and size of every label to correspond with the newest version of the data they represent.
         * This was necessary because strings are a value type, not a reference type, so need updating manually. */
        public void UpdateLabels()
        {
            opponentName.Text = battle.CurrentOpponent.Nickname;
            opponentName.Size = opponentName.SpriteFont.MeasureString(opponentName.Text);
            opponentLevel.Text = "Lv " + battle.CurrentOpponent.Level;
            opponentLevel.Size = opponentLevel.SpriteFont.MeasureString(opponentLevel.Text);
            opponentHealth.Text = "HP: " + battle.CurrentOpponent.Health.CurrentValue + "/" + battle.CurrentOpponent.Health.MaximumValue;
            opponentHealth.Size = opponentHealth.SpriteFont.MeasureString(opponentHealth.Text);

            playerName.Text = battle.CurrentPlayer.Nickname;
            playerName.Size = playerName.SpriteFont.MeasureString(playerName.Text);
            playerLevel.Text = "Lv " + battle.CurrentPlayer.Level;
            playerLevel.Size = playerLevel.SpriteFont.MeasureString(playerLevel.Text);
            playerHealth.Text = "HP: " + battle.CurrentPlayer.Health.CurrentValue + "/" + battle.CurrentPlayer.Health.MaximumValue;
            playerHealth.Size = playerHealth.SpriteFont.MeasureString(playerHealth.Text);
        }

        /* DoRound() creates a new turn for the NPC by calling AddNpcTurn(), then orders the turns.
         * The nextMove and executing flags are set to true to mark the start of the round. */
        private void DoRound()
        {
            battle.AddNpcTurn(battle.CurrentOpponent, battle.CurrentPlayer);
            battle.OrderTurns();
            nextMove = true;
            executing = true;
        }
    }
}
