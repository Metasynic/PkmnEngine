using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MGPkmnLibrary.PokemonClasses;
using MGPkmnLibrary.WorldClasses;
using MGPkmnLibrary.TileEngine;

namespace PkmnEngine
{
    /* This class handles the loading of large quantities of content used throughout the engine.
     * The class also makes the loaded content available through public properties. */
    public class DataManager
    {
        /* There are six dictionaries of data currently in the manager.
         * The pokemonSpecies dictionary stores the PokemonData objects for each Pokemon in the game. The key of the dictionary is the Pokemon's ID.
         * The moves dictionary stores all the Move objects for each Move in the game. The key in the dictionary is the name of the move.
         * The trainers dictionary stores every NPC trainer in the game. The key of the dictionary is the Trainer's name.
         * The levels dictionary stores all the levels in the game to be used in creating the WorldScreen.world object.
         * The remaining dictionaries store Texture2D objects containing the sprites of the Pokemon in the engine.
         * Currently, there are two sprite dictionaries: one for sprites of the front, and one for sprites of the back of the Pokemon. */
        static Dictionary<ushort, PokemonData> pokemonSpecies = new Dictionary<ushort, PokemonData>();
        static Dictionary<string, Move> moves = new Dictionary<string, Move>();
        static Dictionary<string, Trainer> trainers = new Dictionary<string, Trainer>();
        static Dictionary<string, Level> levels = new Dictionary<string, Level>();
        static Dictionary<ushort, Texture2D> pkmnFrontSprites = new Dictionary<ushort, Texture2D>();
        static Dictionary<ushort, Texture2D> pkmnBackSprites = new Dictionary<ushort, Texture2D>();

        public static Dictionary<ushort, PokemonData> PokemonSpecies
        {
            get { return pokemonSpecies; }
        }
        public static Dictionary<string, Move> Moves
        {
            get { return moves; }
        }
        public static Dictionary<string, Trainer> Trainers
        {
            get { return trainers; }
        }
        public static Dictionary<string, Level> Levels
        {
            get { return levels; }
        }
        public static Dictionary<ushort, Texture2D> PkmnFrontSprites
        {
            get { return pkmnFrontSprites; }
        }
        public static Dictionary<ushort, Texture2D> PkmnBackSprites
        {
            get { return pkmnBackSprites; }
        }

        /* The following methods are for reading in the content at runtime, and all follow a similar pattern.
         * The pattern will be explained here, and major variations in the pattern will be explained below in the code.
         * The engine's ContentManager is passed in so that it can be used to load the data.
         * First, a list of all the files in the relevant content folder is obtained, using Directroy.GetFiles().
         * The "*.xnb" parameter is passed in so that the array will only contain the paths of Monogame Content files.
         * Next, a foreach loop iterates through the paths in the array.
         * In each case, another string is created from the path, without the "Content/", and without the file extension at the end.
         * This is because the base folder for the Content.Load<T>() method is the "Content" folder already.
         * The file extension is removed because Monogame requires content filenames to be passed in without the .xnb extension.
         * Finally, the content file is loaded into the game using Content.Load<T>(), and added to the relevant dictionary. */
        public static void ReadPokemonData(ContentManager Content)
        {
            string[] filenames = Directory.GetFiles(@"Content/GameData/PokemonData", "*.xnb");
            foreach(string filename in filenames)
            {
                string name = @"GameData/PokemonData/" + Path.GetFileNameWithoutExtension(filename);
                PokemonData pokemonData = Content.Load<PokemonData>(name);
                pokemonSpecies.Add(pokemonData.ID, pokemonData);
            }
        }

        public static void ReadMoveData(ContentManager Content)
        {
            string[] filenames = Directory.GetFiles(@"Content/GameData/MoveData", "*.xnb");
            foreach(string filename in filenames)
            {
                string name = @"GameData/MoveData/" + Path.GetFileNameWithoutExtension(filename);
                Move move = Content.Load<Move>(name);
                moves.Add(move.Name, move);
            }
        }

        public static void ReadTrainerData(ContentManager Content)
        {
            string[] filenames = Directory.GetFiles(@"Content/GameData/TrainerData", "*.xnb");
            foreach(string filename in filenames)
            {
                string name = @"GameData/TrainerData/" + Path.GetFileNameWithoutExtension(filename);
                Trainer trainer = Content.Load<Trainer>(name);
                trainers.Add(trainer.TrainerName, trainer);
            }
        }

        /* This is the more complex data reader. It reads in the levels from the LevelData and MapData objects in the content. */
        public static void ReadLevelData(ContentManager Content)
        {
            /* It starts off the same way as all the other data readers, collecting a list of filenames and loading the content into temporary variables. */
            string[] filenames = Directory.GetFiles(@"Content/GameData/LevelData", "*.xnb");
            foreach(string filename in filenames)
            {
                string name = @"GameData/LevelData/" + Path.GetFileNameWithoutExtension(filename);
                LevelData lvlData = Content.Load<LevelData>(name);
                string mapfilename = @"GameData/LevelData/MapData/" + lvlData.MapName;
                MapData mapData = Content.Load<MapData>(mapfilename);

                /* This part is where things get interesting. For each level, a new list of Tilesets is created.
                 * Each tileset image in the level is then loaded from the "Tilesets" folder.
                 * A new Tileset object is made using the image and information in the TilesetData objects in the MapData.
                 * Each Tileset is added to the list of Tilesets. */
                List<Tileset> tilesets = new List<Tileset>();
                foreach(TilesetData data in mapData.Tilesets)
                {
                    Texture2D image = Content.Load<Texture2D>(@"Tilesets/" + Path.GetFileNameWithoutExtension(data.TilesetImageName));
                    Tileset t = new Tileset(image, data.TilesWide, data.TilesHigh, data.TileWidthInPixels, data.TileHeightInPixels);
                    tilesets.Add(t);
                }
                
                /* The same happens with the list of MapLayers in the Level.
                 * It's somewhat neater here since the MapLayer class has a function to parse a MapLayer from a MapLayerData. */
                List<MapLayer> layers = new List<MapLayer>();
                foreach(MapLayerData data in mapData.Layers)
                {
                    layers.Add(MapLayer.FromMapLayerData(data));
                }

                /* Next, a new TileMap object is created for the Level, and the tilesets and layers are added to it. */
                TileMap map = new TileMap(tilesets[0], null, layers[0]);
                if (tilesets.Count > 1)
                {
                    for (int i = 1; i < tilesets.Count; i++)
                    {
                        map.AddTileset(tilesets[i]);
                    }
                }
                if (layers.Count > 1)
                {
                    for (int i = 1; i < layers.Count; i++)
                    {
                        map.AddLayer(layers[i]);
                    }
                }

                /* Finally, the Level object is initialized and added to the dictionary. */
                Level level = new Level(map, lvlData.LevelName);
                levels.Add(level.LevelName, level);
            }
        
            ((MapLayer)(levels["Level 1"].Map.MapLayers[0])).SetTile(18, 1, new WarpTile(((MapLayer)(levels["Level 1"].Map.MapLayers[0])).GetTile(18, 1).TileIndex, ((MapLayer)(levels["Level 1"].Map.MapLayers[0])).GetTile(18, 1).Tileset, "Level 2", 1, 1));
            ((MapLayer)(levels["Level 2"].Map.MapLayers[0])).SetTile(2, 1, new WarpTile(((MapLayer)(levels["Level 2"].Map.MapLayers[0])).GetTile(2, 1).TileIndex, ((MapLayer)(levels["Level 2"].Map.MapLayers[0])).GetTile(2, 1).Tileset, "Level 3", 1, 1));
            ((MapLayer)(levels["Level 3"].Map.MapLayers[0])).SetTile(5, 1, new WarpTile(((MapLayer)(levels["Level 3"].Map.MapLayers[0])).GetTile(5, 1).TileIndex, ((MapLayer)(levels["Level 3"].Map.MapLayers[0])).GetTile(5, 1).Tileset, "Level 1", 1, 1));
        }

        /* This function deviates slightly since it fills all the sprite dictionaries in one shot.
         * It just does everything twice using different content folders and dictionaries. */
        public static void ReadAllPkmnSprites(ContentManager Content)
        {
            string[] filenames = Directory.GetFiles(@"Content/Sprites/Front/", "*.xnb");
            foreach(string filename in filenames)
            {
                string name = @"Sprites/Front/" + Path.GetFileNameWithoutExtension(filename);
                Texture2D texture = Content.Load<Texture2D>(name);
                pkmnFrontSprites.Add(ushort.Parse(Path.GetFileNameWithoutExtension(filename)), texture);
            }

            filenames = Directory.GetFiles(@"Content/Sprites/Back/", "*.xnb");
            foreach(string filename in filenames)
            {
                string name = @"Sprites/Back/" + Path.GetFileNameWithoutExtension(filename);
                Texture2D texture = Content.Load<Texture2D>(name);
                pkmnBackSprites.Add(ushort.Parse(Path.GetFileNameWithoutExtension(filename)), texture);
            }
        }
    }
}
