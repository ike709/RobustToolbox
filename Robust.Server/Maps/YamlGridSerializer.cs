﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Robust.Shared.Interfaces.Map;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Utility;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;

namespace Robust.Server.Maps
{
    public static class YamlGridSerializer
    {
        public static YamlMappingNode SerializeGrid(IMapGrid mapGrid)
        {
            var grid = (IMapGridInternal) mapGrid;

            var gridn = new YamlMappingNode();
            var info = new YamlMappingNode();
            var chunkSeq = new YamlSequenceNode();

            gridn.Add("settings", info);
            gridn.Add("chunks", chunkSeq);

            info.Add("chunksize", grid.ChunkSize.ToString(CultureInfo.InvariantCulture));
            info.Add("tilesize", grid.TileSize.ToString(CultureInfo.InvariantCulture));
            info.Add("snapsize", grid.SnapSize.ToString(CultureInfo.InvariantCulture));
            info.Add("worldpos", $"{grid.WorldPosition.X},{grid.WorldPosition.Y}");

            var chunks = grid.GetMapChunks();
            foreach (var chunk in chunks)
            {
                var chunkNode = SerializeChunk(chunk.Value);
                chunkSeq.Add(chunkNode);
            }

            return gridn;
        }

        private static YamlNode SerializeChunk(IMapChunk chunk)
        {
            var root = new YamlMappingNode();
            var value = new YamlScalarNode($"{chunk.X},{chunk.Y}");
            value.Style = ScalarStyle.DoubleQuoted;
            root.Add("ind", value);

            var gridNode = new YamlScalarNode();
            root.Add("tiles", gridNode);

            gridNode.Value = SerializeTiles(chunk);

            return root;
        }

        private static string SerializeTiles(IMapChunk chunk)
        {
            // number of bytes written per tile, because sizeof(Tile) is useless.
            const int structSize = 4;

            var nTiles = chunk.ChunkSize * chunk.ChunkSize * structSize;
            var barr = new byte[nTiles];

            using (var stream = new MemoryStream(barr))
            using (var writer = new BinaryWriter(stream))
            {
                for (ushort y = 0; y < chunk.ChunkSize; y++)
                for (ushort x = 0; x < chunk.ChunkSize; x++)
                {
                    var tile = chunk.GetTile(x, y);
                    writer.Write(tile.TypeId);
                    writer.Write(tile.Data);
                }
            }

            return Convert.ToBase64String(barr);
        }

        public static void DeserializeGrid(IMapManager mapMan, IMap map, ref GridId? gridId, YamlMappingNode info, YamlSequenceNode chunks, IReadOnlyDictionary<ushort, string> tileDefMapping, ITileDefinitionManager tileDefinitionManager)
        {
            ushort csz = 0;
            ushort tsz = 0;
            float sgsz = 0.0f;
            var worldPos = Vector2.Zero;

            foreach (var kvInfo in info)
            {
                var key = kvInfo.Key.ToString();
                var val = kvInfo.Value.ToString();
                if (key == "chunksize")
                    csz = ushort.Parse(val);
                else if (key == "tilesize")
                    tsz = ushort.Parse(val);
                else if (key == "snapsize")
                    sgsz = float.Parse(val);
                else if (key == "worldpos")
                    worldPos = kvInfo.Value.AsVector2();
            }

            var grid = map.CreateGrid(gridId, csz, sgsz);

            gridId = grid.Index;

            foreach (YamlMappingNode chunkNode in chunks.Cast<YamlMappingNode>())
            {
                DeserializeChunk(mapMan, grid, chunkNode, tileDefMapping, tileDefinitionManager);
            }
        }

        private static void DeserializeChunk(IMapManager mapMan, IMapGrid grid, YamlMappingNode chunk, IReadOnlyDictionary<ushort, string> tileDefMapping, ITileDefinitionManager tileDefinitionManager)
        {
            var indNode = chunk["ind"];
            var tileNode = chunk["tiles"];

            var (chunkOffsetX, chunkOffsetY) = indNode.AsVector2i() * grid.ChunkSize;
            var tileBytes = Convert.FromBase64String(tileNode.ToString());

            using (var stream = new MemoryStream(tileBytes))
            using (var reader = new BinaryReader(stream))
            {
                mapMan.SuppressOnTileChanged = true;

                for (var y = 0; y < grid.ChunkSize; y++)
                for (var x = 0; x < grid.ChunkSize; x++)
                {
                    var id = reader.ReadUInt16();
                    var data = reader.ReadUInt16();

                    var defName = tileDefMapping[id];
                    id = tileDefinitionManager[defName].TileId;

                    var tile = new Tile(id, data);
                    grid.SetTile(new MapIndices(chunkOffsetX + x, chunkOffsetY + y), tile);
                }

                mapMan.SuppressOnTileChanged = false;
            }
        }
    }
}
