namespace MGPkmnLibrary.WorldClasses
{
    /* These two classes represent a serializable Tileset/AnimatedTileset.
     * They are used in the Level Editor to save and load individual Tilesets.
     * Structs could have been used here but I prefer to use a class so I can add methods if necessary. */
    public class TilesetData
    {
        /* A TilesetData has six fields which are the same as fields in the Tileset class.
         * TilesetName is the name of the Tileset.
         * TilesetImageName is the file path to the sprite sheet image.
         * TileWidthInPixels and TileHeightInPixels are the width/height of an individual sprite on the sheet.
         * TilesWide and TilesHigh is the width/height of the entire sheet in tiles. */
        public string TilesetName;
        public string TilesetImageName;
        public int TileWidthInPixels;
        public int TileHeightInPixels;
        public int TilesWide;
        public int TilesHigh;
    }

    public class AnimatedTilesetData
    {
        /* This class is the same as the TilesetData class, except that the TilesWide field is replaced by FrameCount.
         * It makes more sense to call it FrameCount, since in an AnimatedTileset, every row is one animation and every sprite is one frame. */
        public string TilesetName;
        public string TilesetImageName;
        public int TileWidthInPixels;
        public int TileHeightInPixels;
        public int FrameCount;
        public int TilesHigh;
    }
}
