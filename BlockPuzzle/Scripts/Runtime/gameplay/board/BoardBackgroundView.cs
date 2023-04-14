using BlockPuzzle.Scripts.Runtime.extensions;
using UnityEngine;
using UnityEngine.Tilemaps;


namespace BlockPuzzle.Scripts.Runtime.gameplay.board {
	[RequireComponent(typeof(Tilemap))]
	public class BoardBackgroundView : MonoBehaviour {
		#region Set in Inspector
		[SerializeField] private TileBase _tile;
		#endregion Set in Inspector

		private Tilemap _tilemap;


		private void Awake () {
			_tilemap = GetComponent <Tilemap>();
		}

		public void Initialize (Vector2Int size) {
			_tilemap.ClearAllTiles();
			_tilemap.Fill(size, _tile);
		}
	}
}
