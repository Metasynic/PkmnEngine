using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MGPkmnLibrary;
using MGPkmnLibrary.TileEngine;
using MGPkmnLibrary.WorldClasses;
using PkmnEngine.Components;
using MGPkmnLibrary.BattleClasses;
using MGPkmnLibrary.PokemonClasses;

namespace PkmnEngine.GameScreens
{
    /* The WorldScreen is one of the main screens in the engine. It is the state displayed when the player is walking around in a level. */
    public class WorldScreen : BaseGameState
    {
        /* The Engine defines the size of a tile in the World. It is used across the different Levels in the World. */
        Engine engine = new Engine(16, 16);

        /* The static world and player fields store the World and Player that get drawn on the WorldScreen. */
        static World world;
        static Player player;
        public static World World
        {
            get { return world; }
            set { world = value; }
        }
        public static Player Player
        {
            get { return player; }
            set { player = value; }
        }

        /* The constructor passes the parameters into the parent BaseGameState() constructor. */
        public WorldScreen(Game game, GameStateManager manager) : base(game, manager)
        {

        }

        /* The Initialize() function calls the parent function. */
        public override void Initialize()
        {
            base.Initialize();
        }

        /* The LoadContent() function also only calls its parent, since the creation of the world and player are done in the CharacterSelectScreen. */
        protected override void LoadContent()
        {
            base.LoadContent();
        }

        /* The Update() function handles everything that needs doing every frame in the WorldScreen. */
        public override void Update(GameTime gameTime)
        {            
            /* The Player and World are both updated. These handle camera and sprite movement as well as updating the current Level object. */
            player.Update(gameTime);
            world.Update(gameTime);

            /* If the player releases the "I" key, or the start button on a gamepad, the InventoryScreen is pushed on to the state stack.
             * UpdateNames() is also called so that the list of Pokemon in the InventoryScreen matches the player's Pokemon. */
            if (InputHandler.KeyReleased(Keys.I) || InputHandler.ButtonReleased(Buttons.Start, PlayerIndex.One)) {
                GameRef.InventoryScreen.UpdateNames();
                Change(ChangeType.Push, GameRef.InventoryScreen);
            }

            base.Update(gameTime);

            /* If the Player's StepCheck bit is true, this means the player has just completed a step.
             * Any actions that take place after taking a step, such as checking for wild Pokemon or a warp tile, need to happen.
             * StepCheck is then set to false again so it doesn't do everything again next frame. */
            if (Player.StepCheck)
            {
                Player.StepCheck = false;
                CheckSpawn();
                CheckWarp();
            }
        }

        /* Drawing the World means drawing the current Level, the player, and the current dialogue (if applicable). */
        public override void Draw(GameTime gameTime)
        {
            /* The SpriteBatch.Begin() function is called to initialize a draw buffer with a set of arguments that optimize the drawing of low-resolution sprites without antialiasing.
             * The Camera's Transformation matrix is also passed in so that the camera always stays at (0,0) inside the window. */
            GameRef.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, player.Camera.Transformation);
            base.Draw(gameTime);

            /* The current Level is drawn using world.DrawLevel() and passing in the SpriteBatch and Camera.
             * The player gets drawn using player.Draw() and passing in the SpriteBatch. */
            world.DrawLevel(gameTime, GameRef.SpriteBatch, player.Camera);
            player.Draw(gameTime, GameRef.SpriteBatch);

            /* Calling SpriteBatch.End() sends the contents of the draw buffer to the actual screen. */
            GameRef.SpriteBatch.End();
        }
        
        /* The CheckSpawn() function checks if a wild Pokemon battle will be triggered on this frame.
         * It is only called when the player's StepCheck bit is true, so it is only called once every step. */
        private void CheckSpawn()
        {
            /* This Random object will be needed for a lot of things later. */
            Random r = new Random();

            /* Each MapLayer in the map needs to be checked for a Tile with its Spawn bit set to true. */
            foreach (MapLayer layer in world.CurrentMap.MapLayers)
            {
                /* If tile that the player is standing on in the layer has its Spawn bit set to true, a battle may happen. */
                if (layer.GetTile(player.Sprite.TileLocation.X, player.Sprite.TileLocation.Y).Spawn)
                {
                    /* This condition determines whether an actual battle happens or not.
                     * If a random double between 0 and 187.5 is less than the level's grass encounter rate, then a battle will happen.
                     * The most likely a battle can be to happen is a VeryCommon encounter rate, which is 10.0.
                     * With a VeryCommon encounter rate, the chance of a battle occurring is 10/187.5.
                     * The least likely a battle can be to happen is with a VeryRare encounter rate, which has a chance of 1.25/187.5.
                     * Note the the encounter rate is not how likely it is for a certain Pokemon to be battled - it is how likely that a battle will happen at all. */
                    if ((r.NextDouble() * 187.5) < World.CurrentLevel.GrassEncounterRate)
                    {
                        /* A new array of ProportionValue<PokemonSpawn> objects is created with the length of the GrassSpawns in the current level.
                         * GrassSpawns is a list of PokemonSpawn objects for the level's grass, stating what Pokemon at what level can be encountered.
                         * The spawns array is filled with ProportionValues created using the PokemonSpawn's Percentage property, and the PokemonSpawn object itself. */
                        ProportionValue<PokemonSpawn>[] spawns = new ProportionValue<PokemonSpawn>[World.CurrentLevel.GrassSpawns.Count];
                        for(int i = 0; i < spawns.Length; i++) // NOTE TO SELF: IT WOULD BE MORE EFFICIENT TO CREATE THE ARRAY IN A HIGHER SCOPE, SINCE IT IS PART OF THE LEVEL, BUT THIS WILL DO FOR NOW.
                        {
                            spawns[i] = ProportionValue.Create(World.CurrentLevel.GrassSpawns[i].Percentage, World.CurrentLevel.GrassSpawns[i]);
                        }

                        /* The encounter for the spawn is found proportionally from the array using ChooseByRandom().
                         * A new Pokemon object is created using the encounter data's ID, gender, and minimum/maximum levels.
                         * The wild Pokemon's level is chosen randomly between the minimum and maximum level in the selected PokemonSpawn.
                         * The moveset is currently just "Scratch". Although dynamic moveset generation is not part of the user requirements,
                         * I could add it if I have time by choosing the last four moves that the species can learn before it reaches its level. */
                        PokemonSpawn encounter = spawns.ChooseByRandom();
                        Pokemon p = new Pokemon(DataManager.PokemonSpecies[encounter.ID], Pokemon.GetGender(DataManager.PokemonSpecies[encounter.ID].GenderRatio), new[] { DataManager.Moves["Scratch"], null, null, null }, (byte)PkmnUtils.RandomInclusive(encounter.MinLevel, encounter.MaxLevel), null);

                        /* A wild battle is essentially a fight against an enemy team of one Pokemon.
                         * The BattleScreen.InitBattle() is called, which sends the player and enemy to the screen and sets the battle background index to zero.
                         * Finally, the BattleScreen is pushed on to the state stack. */
                        GameRef.BattleScreen.InitBattle(player.Team, p, 0);
                        Change(ChangeType.Push, GameRef.BattleScreen);
                    }
                }
            }
        }

        /* CheckWarp() goes through each layer in the map, and works out whether the current Tile the player is on is a WarpTile.
         * If it's a WarpTile, the CurrentLevelIndex for the world is changed to the WarpTile's DestinationIndex.
         * This means the player will effectively be warped to another level. */
        private void CheckWarp()
        {
            foreach(MapLayer layer in world.CurrentMap.MapLayers)
            {
                if (layer.GetTile(player.TilePosition.X, player.TilePosition.Y).GetType() == typeof(WarpTile))
                {
                    world.CurrentLevelIndex = ((WarpTile)(layer.GetTile(player.TilePosition.X, player.TilePosition.Y))).DestinationIndex;
                }
            }
        }

        /* This function is called immediately after a battle. It creates a new list of PokemonInBattle objects (pullTeam) containing the player's team from the BattleScreen.
         * The WorldScreen's player team is then filled with the pullTeam items, using InitTeam() then iterative calls to AddToTeam(). */
        private void PullPokemon()
        {
            List<PokemonInBattle> pullTeam = GameRef.BattleScreen.Battle.PlayerTeam;
            player.InitTeam(pullTeam[0]);
            for(int i = 1; i < pullTeam.Count; i++)
            {
                player.AddToTeam(pullTeam[i]);
            }
        }
    }
}