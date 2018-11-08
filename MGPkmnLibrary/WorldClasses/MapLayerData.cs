namespace MGPkmnLibrary.WorldClasses
{
    /* This struct for a Tile is used by the MapLayerData class only and is not a replacement for the TileEngine.Tile class.
     * It is used here so that a newly created Tile will be a value type (if the class was used, it'd be a reference type).
     * This means when a new Tile is created in this class, it automatically has null values instead of requiring initialization. */
    public struct Tile
    {
        /* A Tile struct has the same four fields as the TileEngine.Tile class. */
        public int TileIndex;
        public int TilesetIndex;
        public bool Solid;
        public bool Spawn;
        public Tile(int tileindex, int tilesetindex)
        {
            TileIndex = tileindex;
            TilesetIndex = tilesetindex;
            Solid = false;
            Spawn = false;
        }

        /* The struct's constructor sets the four fields using the parameters passed in. */
        public Tile(int tileindex, int tilesetindex, bool solid, bool spawn)
        {
            TileIndex = tileindex;
            TilesetIndex = tilesetindex;
            Solid = solid;
            Spawn = spawn;
        }
    }

    /* The MapLayerData class is a serializable MapLayer. It is used in the Level Editor to save and load individual layers. */
    public class MapLayerData
    {
        /* MapLayerName is the unique name of the MapLayer.
         * Width and Height are the width and height in tiles of the layer.
         * The Layer array will be filled with all the Tiles in the MapLayer. */
        public string MapLayerName;
        public int Width;
        public int Height;
        public Tile[] Layer;

        /* The private constructor is used when deserializing a MapLayerData from XML. */
        private MapLayerData()
        {

        }

        /* The regular constructor takes the name, width, and height of the layer.
         * The Layer array is then generated with a length of (width times height).
         * This means the single dimensional array can hold all the tiles.
         * A single dimensional array must be used, since a 2+ dimensional array cannot be serialized.
         * Since Tile is a value type in this scope, all the new Tile objects in the array have values rather than being null references. */
        public MapLayerData(string name, int width, int height)
        {
            MapLayerName = name;
            Width = width;
            Height = height;
            Layer = new Tile[Height * Width];
        }

        /* There is another overload for the MapLayerData constructor, taking tileIndex and tilesetIndex values.
         * If these values are passed in, the new layer will be filled with Tiles with these values.
         * The constructor does this by creating a new Tile with the tileIndex and tilesetIndex passed in,
         * and then copies that Tile into every space in the array using SetTile(). */
        public MapLayerData(string name, int width, int height, int tileIndex, int tilesetIndex)
        {
            MapLayerName = name;
            Width = width;
            Height = height;
            Layer = new Tile[Height * Width];
            Tile tile = new Tile(tileIndex, tilesetIndex);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < height; x++)
                {
                    SetTile(x, y, tile);
                }
            }
        }

        /* There are two SetTile() functions. One takes the components to make a Tile, and one takes a Tile itself.
         * In both cases, since the array is arranged as one row after another,
         * the index to save the Tile to is found by multiplying y (the row index) by the row Width, and adding x (the column index).
         * In the first overload, a new Tile is generated using the properties passed in.
         * In the second overload, the Tile is set directly. */
        public void SetTile(int x, int y, int tileIndex, int tilesetIndex, bool solid, bool spawn)
        {
            Layer[y * Width + x] = new Tile(tileIndex, tilesetIndex, solid, spawn);
        }
        public void SetTile(int x, int y, Tile tile)
        {
            Layer[y * Width + x] = tile;
        }

        /* GetTile() finds the correct array index the same way as SetTile(), but it returns the Tile instead of setting it. */
        public Tile GetTile(int x, int y)
        {
            return Layer[y * Width + x];
        }
    }
}
