using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MGPkmnLibrary.TileEngine;
using MGPkmnLibrary.ItemClasses;

namespace MGPkmnLibrary.WorldClasses
{
    /* An EncounterRate is a number representing how likely it is that a player will enter a wild battle when taking a step on a Spawn tile.
     * This static class and its properties are used to reference the different possible numbers using names, similar to an enum.
     * I could have used the numbers themselves when defining the EncounterRate for a level, but it's much easier to use these identifiers. */
    public static class EncounterRate
    {
        public static double VeryCommon = 10.0;
        public static double Common = 8.5;
        public static double SemiRare = 6.75;
        public static double Rare = 3.33;
        public static double VeryRare = 1.25;
    }

    /* A PokemonSpawn is a possibility for what can be encountered in a wild battle in a level.
     * A Level has a list of these structs, which is used to choose the properties of a Pokemon that is spawned in a wild battle. */
    public struct PokemonSpawn
    {
        /* The ID is the species ID for the Pokemon. For example, Charizard is #6.
         * MinLevel and MaxLevel are the floor and ceiling for the possible range of levels the Pokemon could have.
         * For instance, a PokemonSpawn with a MinLevel of 4 and a MaxLevel of 8 will spawn a Pokemon with a level anywhere between 4 and 8.
         * Percentage represents the percentage chance that this PokemonSpawn will be used to generate an encounter. */
        public ushort ID;
        public byte MinLevel;
        public byte MaxLevel;
        public double Percentage;

        /* The consructor for a PokemonSpawn is nothing special, it just takes the values as parameters and sets them all. */
        public PokemonSpawn(double percentage, ushort id, byte minLevel, byte maxLevel)
        {
            Percentage = percentage;
            ID = id;
            MinLevel = minLevel;
            MaxLevel = maxLevel;
        }
    }

    /* The World class is one of the more important classes in the engine. The engine only has one World, and that World contains every Level.
     * It inherits from DrawableGameComponent because it needs to be drawn to the screen. */
    public class World : DrawableGameComponent
    {
        /* The screenRect field holds a reference to the Rectangle representing the screen. */
        Rectangle screenRect;
        public Rectangle ScreenRectangle
        {
            get { return screenRect; }
        }

        /* The ItemManager holds the master list of items in the world.
         * It is not currently used and will probably be moved into the DataManager class. */
        ItemManager itemManager = new ItemManager();

        /* The levels Dictionary contains all the Levels in the world. The name of the Level is used as the key in the Dictionary.
         * The currentLevelIndex field stores the name of the current Level being updated and drawn.
         * Only one Level can be active at a time. There is a bit of error handling to check if the Level is in the Dictionary. */
        private Dictionary<string, Level> levels = new Dictionary<string, Level>();
        string currentLevelIndex;
        public Dictionary<string, Level> Levels
        {
            get { return levels; }
            set { levels = value; }
        }
        public string CurrentLevelIndex
        {
            get { return currentLevelIndex; }
            set
            {
                if (levels[value] == null)
                    throw new Exception("Level reference is null.");
                currentLevelIndex = value;
            }
        }

        /* These two properties helpfully expose the currently active Level, and the currently active Level's TileMap. */
        public Level CurrentLevel
        {
            get { return levels[currentLevelIndex]; }
        }
        public TileMap CurrentMap
        {
            get { return levels[currentLevelIndex].Map; }
        }

        /* The constructor for the world is surprisingly simple, as the levels Dictionary is set independently.
         * It stores the reference to the screen rectangle in the screenRect field, and sets the initial level name to an empty string.
         * It also passes a reference to the game through to the parent DrawableGameComponent constructor. */
        public World(Game game, Rectangle screenRectangle) : base(game)
        {
            screenRect = screenRectangle;
            currentLevelIndex = "";
        }

        /* Since only one Level is active at a time, updating the World only involves updating the current Level. */
        public override void Update(GameTime gameTime)
        {
            CurrentLevel.Update(gameTime);
        }

        /* The Draw() function doesn't really do anything at the moment. Drawing the World means drawing the active Level.
         * However, drawing a Level requires a SpriteBatch and Camera, which cannot be passed in to this override of Draw().
         * In order to get around this, a separate function, DrawLevel(), draws the current Level with a SpriteBatch and Camera passed in.
         * The regular World.Draw() function just calls its parent function. */
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
        public void DrawLevel(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
        {
            CurrentLevel.Draw(gameTime, spriteBatch, camera);
        }
    }
}
