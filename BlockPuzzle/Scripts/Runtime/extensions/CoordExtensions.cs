using BlockPuzzle.Scripts.Runtime.gameplay;
using UnityEngine;
using UnityEngine.Tilemaps;


namespace BlockPuzzle.Scripts.Runtime.extensions {
	public static class CoordExtensions {
		public static void SetTile (this Tilemap self, Coord coord, TileBase tile) {
			self.SetTile(coord.ToVector3Int(), tile);
		}

		public static void SetColor (this Tilemap self, Coord coord, Color color) {
			self.SetColor(coord.ToVector3Int(), color);
		}

		public static Color GetColor (this Tilemap self, Coord coord) {
			return self.GetColor(coord.ToVector3Int());
		}

		public static Vector3 GetCellCenterWorld (this Grid self, Coord coord) {
			return self.GetCellCenterWorld(coord.ToVector3Int());
		}

		public static Coord LocalToCellCoord (this Grid self, Vector3 position) {
			return new Coord(self.LocalToCell(position));
		}
	}
}
