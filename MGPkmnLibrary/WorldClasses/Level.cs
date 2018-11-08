using System.Collections.Generic;
using MGPkmnLibrary.ItemClasses;
using MGPkmnLibrary.CharacterClasses;
using MGPkmnLibrary.TileEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MGPkmnLibrary.WorldClasses
{
    /* A Level is everything in one area of the game. It contains a TileMap, a list of wild Pokemon spawns, NPCs, and items on the map. */
    public class Level
    {
        /* The level name is an identifier for the level, and it is used as the key in the world's master list of levels. */
        string levelName;
        public string LevelName
        {
            get { return levelName; }
        }

        /* This list stores all the possible PokemonSpawn objects that could be chosen for an encounter in that level.
         * The objects in this list determine the species and level of Pokemon that could appear in a wild battle. */
        List<PokemonSpawn> grassSpawns;
        public List<PokemonSpawn> GrassSpawns
        {
            get { return grassSpawns; }
            set { grassSpawns = value; }
        }

        /* The encounter rate is a measure of how likely it is that a wild battle will commence when taking a step
         * on to a Tile with its Spawn property set to true. */
        double grassEncounterRate;
        public double GrassEncounterRate
        {
            get { return grassEncounterRate; }
            set { grassEncounterRate = value; }
        } 

        /* This field stores the TileMap object associated with the level. This is mostly what gets drawn to the screen. */
        readonly TileMap map;
        public TileMap Map
        {
            get { return map; }
        }

        /* The two following lists store all the NPC characters and Items that appear in the level. */
        readonly List<Character> npcs;
        public List<Character> NPCs
        {
            get { return npcs; }
        }
        readonly List<ItemSprite> items;
        public List<ItemSprite> Items
        {
            get { return items; }
        }

        /* The constructor for a Level takes its essential components (TileMap and name) as parameters.
         * The lists of spawns, NPCs, and items are initialized. */
        public Level (TileMap tileMap, string name)
        {
            map = tileMap;
            npcs = new List<Character>();
            items = new List<ItemSprite>();
            grassSpawns = new List<PokemonSpawn>();
            levelName = name;
        }

        /* To update a Level, all the NPCs are updated, and all the items are updated.
         * Most importantly, the map itself is also updated. */
        public void Update(GameTime gameTime)
        {
            map.Update(gameTime);
            foreach (Character npc in npcs)
            {
                npc.Update(gameTime, null);
            }
            foreach(ItemSprite itemSprite in items)
            {
                itemSprite.Update(gameTime);
            }
        }

        /* The Draw() function draws the TileMap, all the NPCs in the list, and all the items. */
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
        {
            map.Draw(spriteBatch, camera);
            foreach(Character npc in npcs)
            {
                npc.Draw(gameTime, spriteBatch);
            }
            foreach(ItemSprite itemSprite in items)
            {
                itemSprite.Draw(gameTime, spriteBatch);
            }
        }
    }
}
