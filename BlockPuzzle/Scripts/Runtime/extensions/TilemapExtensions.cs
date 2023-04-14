// ReSharper disable MemberCanBePrivate.Global

using BlockPuzzle.Scripts.Runtime.gameplay;
using UnityEngine;
using UnityEngine.Tilemaps;


namespace BlockPuzzle.Scripts.Runtime.extensions {
	public static class TilemapExtensions {
		public static void Fill (this Tilemap self, int xMin, int xMax, int yMin, int yMax, TileBase tile, Color? color = default) {
			self.ClearAllTiles();
			self.CompressBounds();

			color ??= Color.white;

			for (int y = yMin; y < yMax; y++) {
				for (int x = xMin; x < xMax; x++) {
					Coord coord = new Coord(x, y);
					self.SetTile(coord, tile);
					self.SetColor(coord, color.Value);
				}
			}
		}

		public static void Fill (this Tilemap self, Vector2Int size, TileBase tile, Color? color = default) {
			self.Fill(0, size.x, 0, size.y, tile, color);
		}
	}
}
