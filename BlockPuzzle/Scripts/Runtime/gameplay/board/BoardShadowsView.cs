using System.Collections.Generic;
using System.Linq;
using BlockPuzzle.Scripts.Runtime.extensions;
using BlockPuzzle.Scripts.Runtime.utilities;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Tilemaps;


namespace BlockPuzzle.Scripts.Runtime.gameplay.board {
	public class BoardShadowsView : MonoBehaviour {
		#region Set in Inspector
		[SerializeField]         private TileBase _tile;
		[SerializeField, Min(0)] private float    _animationDuration = 0.075f;
		#endregion Set in Inspector

		private Tilemap _tilemap;

		private readonly HashSet <Coord> _shadowedCoords = new HashSet <Coord>();


		private void Awake () {
			_tilemap = GetComponent <Tilemap>();
		}

		public void Initialize (Vector2Int size) {
			_tilemap.ClearAllTiles();
			_tilemap.Fill(size, _tile, ColorUtils.TransparentWhite);
		}

		public void Clear () {
			foreach (Coord coord in _shadowedCoords.ToArray())
				ClearShadow(coord);

			_shadowedCoords.Clear();
		}

		public void ClearInstant () {
			foreach (Coord coord in _shadowedCoords.ToArray())
				ClearShadowInstant(coord);
		}

		public void Drop (HashSet <Coord> coords) {
			if (_shadowedCoords.SetEquals(coords))
				return;

			Coord[] coordsToDropShadow  = coords.Except(_shadowedCoords).ToArray();
			Coord[] coordsToClearShadow = _shadowedCoords.Except(coords).ToArray();

			foreach (Coord coord in coordsToDropShadow)
				DropShadow(coord);

			foreach (Coord coord in coordsToClearShadow)
				ClearShadow(coord);
		}

		private void DropShadow (Coord coord) {
			_shadowedCoords.Add(coord);

			Animate(coord, Color.white);
		}

		private void ClearShadow (Coord coord) {
			_shadowedCoords.Remove(coord);

			Animate(coord, ColorUtils.TransparentWhite);
		}

		private void ClearShadowInstant (Coord coord) {
			_shadowedCoords.Remove(coord);

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
