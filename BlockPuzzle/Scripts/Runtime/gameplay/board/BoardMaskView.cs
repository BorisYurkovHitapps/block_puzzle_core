using System.Collections.Generic;
using System.Linq;
using BlockPuzzle.Scripts.Runtime.extensions;
using BlockPuzzle.Scripts.Runtime.utilities;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Tilemaps;


namespace BlockPuzzle.Scripts.Runtime.gameplay.board {
	[RequireComponent(typeof(Tilemap))]
	public class BoardMaskView : MonoBehaviour {
		#region Set in Inspector
		[SerializeField]         private TileBase _tile;
		[SerializeField, Min(0)] private float    _animationDuration = 0.075f;
		#endregion Set in Inspector

		private Tilemap _tilemap;

		private readonly HashSet <Coord> _maskedCoords = new HashSet <Coord>();


		private void Awake () {
			_tilemap = GetComponent <Tilemap>();
		}

		public void Initialize (Vector2Int size) {
			_tilemap.ClearAllTiles();
			_tilemap.Fill(size, _tile, ColorUtils.TransparentWhite);
		}

		public void UnmaskAll () {
			foreach (Coord coord in _maskedCoords.ToArray())
				Unmask(coord);
		}

		public void Mask (HashSet <Coord> coords) {
			if (_maskedCoords.SetEquals(coords))
				return;

			Coord[] coordsToMask   = coords.Except(_maskedCoords).ToArray();
			Coord[] coordsToUnmask = _maskedCoords.Except(coords).ToArray();

			foreach (Coord coord in coordsToMask)
				Mask(coord);

			foreach (Coord coord in coordsToUnmask)
				Unmask(coord);
		}

		private void Mask (Coord coord) {
			_maskedCoords.Add(coord);

			Animate(coord, Color.white);
		}

		private void Unmask (Coord coord) {
			_maskedCoords.Remove(coord);

			_tilemap.SetColor(coord, ColorUtils.TransparentWhite);
		}

		private void Animate (Coord coord, Color to) {
			DOTween.To(
				       () => _tilemap.GetColor(coord),
				       color => _tilemap.SetColor(coord, color),
				       to,
				       _animationDuration
			       )
			       .SetEase(Ease.InQuad)
			       .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
		}
	}
}
