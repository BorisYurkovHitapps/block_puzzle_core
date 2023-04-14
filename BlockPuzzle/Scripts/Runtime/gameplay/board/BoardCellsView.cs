using System.Collections.Generic;
using System.Threading;
using BlockPuzzle.Scripts.Runtime.extensions;
using BlockPuzzle.Scripts.Runtime.utilities;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Tilemaps;


namespace BlockPuzzle.Scripts.Runtime.gameplay.board {
	public class BoardCellsView : MonoBehaviour {
		#region Set in Inspector
		[SerializeField]         private TileBase _tile;
		[SerializeField, Min(0)] private float    _animationDuration = 0.075f;
		#endregion Set in Inspector

		private Tilemap    _tilemap;
		private Vector2Int _size;


		private void Awake () {
			_tilemap = GetComponent <Tilemap>();
		}

		public void Clear () {
			_tilemap.ClearAllTiles();
		}
		
		public void Draw (IEnumerable <Coord> coords) {
			foreach (Coord coord in coords) {
				_tilemap.SetColor(coord, Color.white);
				_tilemap.SetTile(coord, _tile);
			}
		}

		public async UniTask ClearRangeAsync (IEnumerable <Coord> coords, CancellationToken cancellationToken = default) {
			await UniTask.WhenAll(coords.Select(coord => ClearAsync(coord, cancellationToken)));
		}

		private async UniTask ClearAsync (Coord coord, CancellationToken cancellationToken = default) {
			await DOTween.To(
				             () => _tilemap.GetColor(coord),
				             color => _tilemap.SetColor(coord, color),
				             ColorUtils.TransparentWhite,
				             _animationDuration
			             )
			             .SetEase(Ease.InQuad)
			             .SetLink(gameObject, LinkBehaviour.KillOnDestroy)
			             .WithCancellation(cancellationToken);
		}
	}
}
