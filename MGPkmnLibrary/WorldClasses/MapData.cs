using System.Collections.Generic;
using MGPkmnLibrary.TileEngine;
using Microsoft.Xna.Framework.Content;

namespace MGPkmnLibrary.WorldClasses
{
    /* A MapData is a serializable TileMap. It is used in the Level Editor. */
    public class MapData
    {
        /* MapName contains the unique name identifier for the TileMap being serialized.
         * Layers is an array containing all the MapLayers in the TileMap.
         * Tilesets is an array containing all the Tilesets in the TileMap. */
        public string MapName;
        public MapLayerData[] Layers;
        public TilesetData[] Tilesets;

        /* There are also fields for an AnimatedMapLayer and an AnimatedTileset.
         * Both are marked as Optional for the ContentSerializer, meaning they don't have to be present in the XML. */
        [ContentSerializer(Optional = true)]
        public AnimatedMapLayer AnimatedLayer;

        [ContentSerializer(Optional = true)]
        public AnimatedTilesetData AnimatedTileset;

        /* The private constructor is for deserializing a MapData from XML. */
        private MapData()
        {

        }

        /* The public constructor for a MapData takes the name, layers, tilesets, and the animated layer and tileset. */
        public MapData(string mapName, List<MapLayerData> layers, AnimatedMapLayer animatedLayer, List<TilesetData> tilesets, AnimatedTilesetData animatedTileset)
        {
            MapName = mapName;
            Layers = layers.ToArray();
            AnimatedLayer = animatedLayer;
            Tilesets = tilesets.ToArray();
            AnimatedTileset = animatedTileset;
        }
    }
}
