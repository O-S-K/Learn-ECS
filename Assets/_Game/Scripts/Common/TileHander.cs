using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System;

public class TileHandler
{
    private Dictionary<string, TiledMap> _tiledMaps;
    private Dictionary<string, Dictionary<int, TiledTileset>> _tileSets;

    /// <summary>
    /// A dictionary of all obstacles in every loaded map.
    /// </summary>
    public Dictionary<string, Dictionary<string, List<Rectangle>>> objects { get; private set; }

    /// <summary>
    /// Initializes a new instance of the TileHandler class.
    /// </summary>
    public TileHandler()
    {
        _tiledMaps = new Dictionary<string, TiledMap>();
        _tileSets = new Dictionary<string, Dictionary<int, TiledTileset>>();
        objects = new Dictionary<string, Dictionary<string, List<Rectangle>>>();
    }

    /// <summary>
    /// Loads a map and its associated tileset texture.
    /// </summary>
    /// <param name="pathToMap">The path to the Tiled map file.</param>
    /// <param name="pathToFolder">The path to the folder containing the tileset texture.</param>
    /// <param name="levelID">The name to give the loaded map.</param>
    /// <param name="textureName">The name to give the loaded tileset texture.</param>
    public void Load(string pathToMap, string pathToFolder, string levelID)
    {
        try
        {
            TiledMap map = new TiledMap(pathToMap);
            var tilesets = map.GetTiledTilesets(pathToFolder);

            _tiledMaps[levelID] = map ?? throw new Exception("Map file was not found or could not be loaded.");
            _tileSets[levelID] = tilesets ?? throw new Exception("Tileset could not be loaded.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while loading the map or its tilesets: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Retrieves a map given its LevelID.
    /// </summary>
    /// <param name="level">The LevelID of the map to retrieve.</param>
    /// <returns>The loaded map, or null if the map was not found.</returns>
    public TiledMap GetMap(LevelID level)
    {
        return _tiledMaps[level.ToString()];
    }

    /// <summary>
    /// Helper method for getting object representing obstacles in a map method below. 
    /// Creates rectangles representing the objects of a specified layer.
    /// </summary>
    /// <param name="layer">The layer to create bounds for.</param>
    /// <returns>A list of rectangles representing the layer's bounds.</returns>
    public List<Rectangle> GetLayerObjects(TiledLayer layer)
    {
        List<Rectangle> layerBounds = new List<Rectangle>();

        foreach (var obj in layer.objects)
        {
            int objX = (int)obj.x;
            int objY = (int)obj.y;
            int objWidth = (int)obj.width;
            int objHeight = (int)obj.height;
            layerBounds.Add(new Rectangle(objX, objY, objWidth, objHeight));
        }

        return layerBounds;
    }

    /// <summary>
    /// Creates rectangles that bound every obstacle object on every layer in every loaded Level. 
    /// Useful for collision detection with Rectangle.Intersects() method.
    /// </summary>
    public void GetLayersObstaclesInMap()
    {
        foreach (string mapName in _tiledMaps.Keys)
        {
            Dictionary<string, List<Rectangle>> layerBoundsMap = new Dictionary<string, List<Rectangle>>();
            foreach (var layer in _tiledMaps[mapName].Layers)
            {
                string layerName = layer.name;
                if (layer.type != TiledLayerType.ObjectLayer || !GameConstants.OBSTACLES.Contains(layer.name))
                {
                    continue;
                }
                List<Rectangle> layerBounds = GetLayerObjects(layer);
                if (!layerBoundsMap.ContainsKey(layerName))
                {
                    layerBoundsMap[layerName] = new List<Rectangle>();
                }
                layerBoundsMap[layerName].AddRange(layerBounds);
            }
            objects[mapName] = layerBoundsMap;
        }
    }

    /// <summary>
    /// Draws every tile in the "TileLayer" type layers of the specified map.
    /// </summary>
    /// <param name="LevelID">The name of the map to draw.</param>
    /// <param name="spriteBatch">The SpriteBatch object to use for rendering.</param>
    public void Draw(string LevelID, SpriteBatch spriteBatch)
    {
        TiledMap map = _tiledMaps[LevelID];
        var tilesets = _tileSets[LevelID];

        foreach (var layer in map.Layers)
        {
            if (layer.type != TiledLayerType.TileLayer)
            {
                continue;
            }

            for (int y = 0; y < layer.height; y++)
            {
                for (int x = 0; x < layer.width; x++)
                {
                    // Assuming the default render order is used which is from right to bottom
                    var index = (y * layer.width) + x;
                    var gid = layer.data[index];
                    var tileX = x * map.TileWidth;
                    var tileY = y * map.TileHeight;
                    // Gid 0 is used to tell there is no tile set
                    if (gid == 0)
                    {
                        continue;
                    }

                    // Helper method to fetch the right TieldMapTileset instance.
                    // This is a connection object Tiled uses for linking the correct tileset to the gid value using the firstgid property.
                    var mapTileset = map.GetTiledMapTileset(gid);

                    // Retrieve the actual tileset based on the firstgid property of the connection object we retrieved just now
                    var tileset = tilesets[mapTileset.firstgid];
                    //Console.WriteLine("name: " + tileset.Name);  //Debug message

                    // Use the connection object as well as the tileset to figure out the source rectangle.
                    var rect = map.GetSourceRect(mapTileset, tileset, gid);
                    // Create destination and source rectangles
                    var source = new Rectangle(rect.x, rect.y, rect.width, rect.height);
                    var destination = new Rectangle(tileX, tileY, map.TileWidth, map.TileHeight);
                    // Retrieve texture used in the tileset
                    Enum.TryParse(tileset.Name, out TiledTexture textureName);
                    var tilesetTexture = Loader.GetTexture(textureName);
                    // Draw tile
                    spriteBatch.Draw(tilesetTexture, destination, source, Color.White);
                }
            }
        }
    }
