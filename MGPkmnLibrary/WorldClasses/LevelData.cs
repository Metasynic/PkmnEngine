using System.Collections.Generic;

namespace MGPkmnLibrary.WorldClasses
{
    /* A LevelData is a serializable version of a Level, containing its attributes other than the TileMap.
     * The TileMap is serialized separately, and identified by the MapName field.
     * This needs extending to include the items on the map. */
    public class LevelData
    {
        /* These five fields store the information about the Level.
         * LevelName and MapName are the unique names of the Level and Map.
         * MapWith and MapHeight are self-explanatory.
         * The CharacterNames array holds all the identifiers for the NPCs on the map. */
        public string LevelName;
        public string MapName;
        public int MapWidth;
        public int MapHeight;
        public string[] CharacterNames;

        /* The private constructor allows the object to be deserialized from XML. */
        private LevelData()
        {

        }

        /* The public constructor for a LevelData takes parameters for all its fields. */
        public LevelData(string levelName, string mapName, int mapWidth, int mapHeight, List<string> characterNames)
        {
            LevelName = levelName;
            MapName = mapName;
            MapWidth = mapWidth;
            MapHeight = mapHeight;
            CharacterNames = characterNames.ToArray();
        }
    }
}
