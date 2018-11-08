using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MGPkmnLibrary.TileEngine
{
    /* This class represents an entire map in the game. A map is, for example, a room or area of the world. */
    public class TileMap
    {
        /* A TileMap has a List<Tileset> containing all the tilesets used in the map, and a List<InterfaceLayer>,
         * which contains all of the MapLayers. There is also a field for an animated tileset and animated layer.
         * The animated fields are not yet implemented and not a part of the user requirements. */
        List<Tileset> tilesets;
        AnimatedTileset animatedTileset;
        List<InterfaceLayer> mapLayers;
        AnimatedMapLayer animatedLayer;
        public List<Tileset> Tilesets
        {
            get { return tilesets; }
        }
        public List<InterfaceLayer> MapLayers
        {
            get { return mapLayers; }
        }
        public AnimatedTileset AnimatedTileset
        {
            get { return animatedTileset; }
        }
        public AnimatedMapLayer AnimatedLayer
        {
            get { return animatedLayer; }
        }

        /* These two static fields store the width and height of the map in tiles, using the width and height of the first MapLayer.
         * The width and height in pixels is then exposed by multiplying them by the tile width/height. */
        int widthInTiles
        {
            get { return ((MapLayer)mapLayers[0]).Width; }
        }
        int heightInTiles
        {
            get { return ((MapLayer)mapLayers[0]).Height; }
        }
        public int WidthInPixels
        {
            get { return widthInTiles * Engine.TileWidth; }
        }
        public int HeightInPixels
        {
            get { return heightInTiles * Engine.TileHeight; }
        }

        /* The TileMap class has two constructors. The first one takes tilesets and map layers, and sets them all correctly. */
        public TileMap(List<Tileset> tilesets, AnimatedTileset animatedTileset, List<InterfaceLayer> mapLayers, AnimatedMapLayer animatedLayer)
        {
            this.tilesets = tilesets;
            this.mapLayers = mapLayers;
            this.animatedTileset = animatedTileset;
            this.animatedLayer = animatedLayer;
        }

        /* The second constructor only takes one map layer instead of a list of them.
         * It does the same as the first constructor, except the mapLayers list is initialized, and the first layer is added from there. */
        public TileMap(Tileset tileset, AnimatedTileset animatedTileset, MapLayer layer)
        {
            tilesets = new List<Tileset>();
            tilesets.Add(tileset);
            this.animatedTileset = animatedTileset;
            animatedLayer = new AnimatedMapLayer();
            mapLayers = new List<InterfaceLayer>();
            mapLayers.Add(layer);
        }

        /* AddLayer() simply takes an InterfaceLayer object and checks it can be added to the list of layers.
         * It does this by checking if the layer's width and height are the same as the map's width and height.
         * If they are not the same, the layer cannot be added, as all the layers must match the map's size.
         * Otherwise, the new layer is added to the list. */
        public void AddLayer(InterfaceLayer layer)
        {
            if (layer is MapLayer)
            {
                if (((MapLayer)layer).Width != widthInTiles && ((MapLayer)layer).Height != heightInTiles)
                {
                    throw new Exception("Map Layer Size Exception while adding new Layer");
                }
            }
            mapLayers.Add(layer);
        }

        /* When a TileMap is updated, all the layers inside the map must be updated.
         * Every layer in the mapLayers list is updated, as well as the animated layer. */
        public void Update(GameTime gameTime)
        {
            foreach(InterfaceLayer mapLayer in mapLayers)
            {
                mapLayer.Update(gameTime);
            }
            animatedLayer.Update(gameTime);
        }

        /* Much like the Update() function, the Draw() function also needs to update every layer in the map.
         * The function iterates through every layer and updates it.
         * If the animatedTileset field is initialized, then the animatedLayer will also be drawn.
         * It's important that the animatedTileset is initialized, otherwise the animatedLayer could try to
         * reference a null animatedTileset when it draws, which would cause a crash. */
        public void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            foreach(MapLayer mapLayer in mapLayers)
            {
                mapLayer.Draw(spriteBatch, camera, tilesets);
            }
            if (animatedTileset != null)
                animatedLayer.Draw(spriteBatch, animatedTileset);
        }

        /* This function simply adds a new Tileset object to the TileMap's list of tilesets. */
        public void AddTileset(Tileset tileset)
        {
            tilesets.Add(tileset);
        }
    }
}
