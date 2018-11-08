namespace MGPkmnLibrary.TileEngine
{
    /* This is a small class representing Tiles in the Engine.
     * It could have been implemented as a struct, but I wanted to be able to give Tiles methods if they were ever needed. */
    public class Tile
    {
        /* A Tile has four fields. The tileIndex field is the location of the tile's texture in its tileset.
         * For example, a Tile with a tileIndex of 1 would be the second tile in the tileset, since it starts at 0.
         * The tileset field is the index of the tileset that the Tile's texture can be found in.
         * The solid field indicates whether the tile can be walked through or not.
         * The spawn field determines if there's a chance for wild Pokemon to appear when you step on the Tile. */
        int tileIndex;
        int tileset;
        bool solid;
        bool spawn;

        public int TileIndex
        {
            get { return tileIndex; }
            set { tileIndex = value; }
        }
        public int Tileset
        {
            get { return tileset; }
            set { tileset = value; }
        }
        public bool Solid
        {
            get { return solid; }
            set { solid = value; }
        }
        public bool Spawn
        {
            get { return spawn; }
            set { spawn = value; }
        }

        /* There are two constructors for a Tile. Both take a tileIndex and a tileset, but only one takes the spawn and solid bits.
         * When the spawn and solid bits are not passed in, they are set as false by default.
         * The constructors set the four fields using the parameters passed in. */
        public Tile(int tileIndex, int tileset)
        {
            TileIndex = tileIndex;
            Tileset = tileset;
            solid = false;
            spawn = false;
        }
        public Tile(int tileIndex, int tileset, bool solid, bool spawn)
        {
            TileIndex = tileIndex;
            Tileset = tileset;
            Solid = solid;
            Spawn = spawn;
        }
    }

    /* A WarpTile is the same as a regular Tile and inherits from it.
     * When a player stands on a WarpTile, they are sent to the destination level at the destination coordinates.
     * This means the player can travel between levels by using WarpTiles.
     * It has three extra properties: the index of the destination level, and the coordinates in the destination level. */
    public class WarpTile : Tile
    {
        string destinationIndex;
        public string DestinationIndex
        {
            get { return destinationIndex; }
            set { destinationIndex = value; }
        }
        int destX;
        public int DestX
        {
            get { return destX; }
            set { destX = value; }
        }
        int destY;
        public int DestY
        {
            get { return destY; }
            set { destY = value; }
        }

        public WarpTile(int tileIndex, int tileset, string index, int destX, int destY) : base(tileIndex, tileset)
        {
            destinationIndex = index;
            this.destX = destX;
            this.destY = destY;
        }
    }
}
