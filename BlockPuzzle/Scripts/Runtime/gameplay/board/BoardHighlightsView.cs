using System.Collections.Generic;
using System.Linq;
using BlockPuzzle.Scripts.Runtime.extensions;
using BlockPuzzle.Scripts.Runtime.utilities;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Tilemaps;


namespace BlockPuzzle.Scripts.Runtime.gameplay.board {
	[RequireComponent(typeof(Tilemap))]
	public class BoardHighlightsView : MonoBehaviour {
		#region Set in Inspector
		[SerializeField]         private TileBase _tile;
		[SerializeField, Min(0)] private float    _animationDuration = 0.075f;
		#endregion Set in Inspector

		private Tilemap _tilemap;

		private readonly HashSet <Coord> _highlightedCoords = new HashSet <Coord>();


		private void Awake () {
			_tilemap = GetComponent <Tilemap>();
		}

		public void Initialize (Vector2Int size) {
			_tilemap.ClearAllTiles();
			_tilemap.Fill(size, _tile, ColorUtils.TransparentWhite);
		}

		public void UnhighlightAll () {
			foreach (Coord coord in _highlightedCoords.ToArray())
				Unhighlight(coord);
		}

		public void Highlight (HashSet <Coord> coords) {
			if (_highlightedCoords.SetEquals(coords))
				return;

			Coord[] coordsToHighlight   = coords.Except(_highlightedCoords).ToArray();
			Coord[] coordsToUnhighlight = _highlightedCoords.Except(coords).ToArray();

			foreach (Coord coord in coordsToHighlight)
				Highlight(coord);

			foreach (Coord coord in coordsToUnhighlight)
				Unhighlight(coord);
		}

		private void Highlight (Coord coord) {
			_highlightedCoords.Add(coord);

			Animate(coord, Color.white);
		}

		private void Unhighlight (Coord coord) {
			_highlightedCoords.Remove(coord);

			Animate(coord, ColorUtils.TransparentWhite);
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
