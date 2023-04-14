using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Audio;
using BlockPuzzle.Scripts.Runtime.extensions;
using BlockPuzzle.Scripts.Runtime.gameplay.turnProcessing;
using BlockPuzzle.Scripts.Runtime.persistence;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;


namespace BlockPuzzle.Scripts.Runtime.gameplay.board {
	public class Board : MonoBehaviour {
		#region Set in Inspector
		[SerializeField] private Vector2Int _size;

		[Space]
		[SerializeField] private Grid _grid;
		[SerializeField] private Camera _snapshotCamera;

		[Header("Views")]
		[SerializeField] private BoardBackgroundView _backgroundView;
		[SerializeField] private BoardShadowsView    _shadowsView;
		[SerializeField] private BoardCellsView      _cellsView;
		[SerializeField] private BoardHighlightsView _highlightsView;
		[SerializeField] private BoardMaskView       _boardMaskView;

		[Space]
		[SerializeField] private AudioClip _mergeSfx;
		#endregion

		public static event Action <Shape> ShapePlaced;


		public int Width  => _size.x;
		public int Height => _size.y;

		private BoardState      _state;
		private HashSet <Coord> _cachedShapeHoveredCoords = new HashSet <Coord>();

		private BlockPuzzleAttempt _attempt;


		[Inject]
		[UsedImplicitly]
		private void SetDependencies (BlockPuzzleAttempt attempt) {
			_attempt = attempt;
		}

		private void Awake () {
			TryApplyAttemptState();

			Shape.Moved += OnShapeMoved;

			_attempt.StoreRequested += OnStoreAttemptRequested;
		}

		private void OnDestroy () {
			Shape.Moved -= OnShapeMoved;

			_attempt.StoreRequested -= OnStoreAttemptRequested;
		}

		private void Start () {
			Draw();
		}

		private void TryApplyAttemptState () {
			ApplyState(
				_attempt.TryGetBoardState(out BoardState boardState)
					? boardState
					: new BoardState(_size.x, _size.y)
			);
		}

		public void ApplyState (BoardState boardState) {
			_state = boardState;
		}

		public void Draw () {
			_backgroundView.Initialize(_size);
			_shadowsView.Initialize(_size);
			_highlightsView.Initialize(_size);
			_boardMaskView.Initialize(_size);

			_cellsView.Clear();
			_cellsView.Draw(_state.GetOccupiedCoords());

			AlignGrid();
		}

		public void Refresh () {
			TryApplyAttemptState();
			Draw();
		}

		private void AlignGrid () {
			_grid.transform.localPosition = -new Vector3(_size.x, _size.y) / 2.0f;
		}

		private void OnShapeMoved (Shape shape) {
			bool canBePlaced = IsHoveringVacantCoords(shape, out HashSet <Coord> placement);

			if (placement.SequenceEqual(_cachedShapeHoveredCoords))
				return;

			_cachedShapeHoveredCoords = placement;

			if (canBePlaced) {
				HashSet <Coord> lineCoords = GetPotentialLinesCoords(placement);
				_highlightsView.Highlight(lineCoords);
				_shadowsView.Drop(_cachedShapeHoveredCoords);
				return;
			}

			_shadowsView.Clear();
			_highlightsView.UnhighlightAll();
		}

		private bool IsHoveringVacantCoords (Shape shape, out HashSet <Coord> hoveredCoords) {
			hoveredCoords = new HashSet <Coord>();
			bool result = true;

			foreach (ShapeBlock block in shape.Blocks) {
				Vector3 blockPosition = block.WorldPosition;
				Coord   hoveredCoord  = WorldToCell(blockPosition);

				if (_state.Contains(hoveredCoord) && _state.IsVacant(hoveredCoord)) {
					hoveredCoords.Add(hoveredCoord);
					continue;
				}

				result = false;
				hoveredCoords.Clear();
				break;
			}

			return result;
		}

		private HashSet <Coord> GetPotentialLinesCoords (HashSet <Coord> shapeHoveredCoords) {
			HashSet <Coord> result = new HashSet <Coord>();

			foreach (Coord coord in shapeHoveredCoords) {
				Coord[] row = _state.GetRowCoordsAt(coord.Y)
				                    .Except(shapeHoveredCoords)
				                    .ToArray();

				Coord[] column = _state.GetColumnCoordsAt(coord.X)
				                       .Except(shapeHoveredCoords)
				                       .ToArray();

				if (_state.AreOccupied(row))
					result.UnionWith(row);

				if (_state.AreOccupied(column))
					result.UnionWith(column);
			}

			return result;
		}

		public Vector3 GetCellCenterWorld (Coord coord) {
			return _grid.GetCellCenterWorld(coord);
		}

		private Coord WorldToCell (Vector3 position) {
			position = _grid.transform.InverseTransformPoint(position);
			return _grid.LocalToCellCoord(position);
		}

		private Vector3 CalculatePlacementWorldPosition (HashSet <Coord> placement) {
			float minX = Mathf.Infinity;
			float maxX = Mathf.NegativeInfinity;
			float minY = Mathf.Infinity;
			float maxY = Mathf.NegativeInfinity;

			foreach (Coord coord in placement) {
				Vector3 cellCenterWorld = GetCellCenterWorld(coord);

				minX = Mathf.Min(minX, cellCenterWorld.x);
				maxX = Mathf.Max(maxX, cellCenterWorld.x);
				minY = Mathf.Min(minY, cellCenterWorld.y);
				maxY = Mathf.Max(maxY, cellCenterWorld.y);
			}

			Vector3 midPoint = new Vector3((minX + maxX) / 2f, (minY + maxY) / 2f, 0);

			return midPoint;
		}

		public async UniTask <ShapePlacementResult> TryPlaceShapeAsync (Shape shape, CancellationToken cancellationToken = default) {
			bool canPlace = IsHoveringVacantCoords(shape, out HashSet <Coord> hoveredCoords);

			if (canPlace) {
				await PlaceAsync(shape, hoveredCoords, cancellationToken);
			} else {
				await shape.PutBackAsync(cancellationToken);
			}

			return new ShapePlacementResult(canPlace, hoveredCoords);
		}

		private async UniTask PlaceAsync (Shape shape, HashSet <Coord> placement, CancellationToken cancellationToken = default) {
			_state.OccupyCoords(placement);

			Vector2 placementWorldPosition = CalculatePlacementWorldPosition(placement);

			_highlightsView.UnhighlightAll();

			await shape.LandAsync(placementWorldPosition, cancellationToken);

			_shadowsView.ClearInstant();
			_cellsView.Draw(placement);

			ShapePlaced?.Invoke(shape);
		}

		public bool HasAvailableSpaceFor (Shape shape) {
			return _state.HasVacantPlaceFor(shape);
		}

		public async UniTask <ClearLinesResult> TryClearLinesAsync (ShapePlacementResult shapePlacementResult, CancellationToken cancellationToken) {
			HashSet <int> rowIndices    = new HashSet <int>();
			HashSet <int> columnIndices = new HashSet <int>();

			HashSet <Coord> rowsCoords    = new HashSet <Coord>();
			HashSet <Coord> columnsCoords = new HashSet <Coord>();

			int rowsAssembled    = 0;
			int columnsAssembled = 0;

			foreach (Coord coord in shapePlacementResult.OccupiedCoords) {
				rowIndices.Add(coord.Y);
				columnIndices.Add(coord.X);
			}

			foreach (int index in rowIndices) {
				Coord[] row = _state.GetRowCoordsAt(index).ToArray();

				if (_state.AreOccupied(row) == false)
					continue;

				rowsAssembled++;
				rowsCoords.UnionWith(row);
			}

			foreach (int index in columnIndices) {
				Coord[] column = _state.GetColumnCoordsAt(index).ToArray();

				if (_state.AreOccupied(column) == false)
					continue;

				columnsAssembled++;
				columnsCoords.UnionWith(column);
			}

			_state.ReleaseCoords(columnsCoords);
			_state.ReleaseCoords(rowsCoords);

			if (rowsAssembled + columnsAssembled > 0)
				AudioManager.PlayOnce(_mergeSfx);

			await _cellsView.ClearRangeAsync(rowsCoords.Union(columnsCoords), cancellationToken);

			return new ClearLinesResult(
				rowsAssembled,
				columnsAssembled,
				rowsCoords,
				columnsCoords
			);
		}

		private void OnStoreAttemptRequested () {
			_attempt.SetBoardState(_state);
		}

		public Sprite TakeLevelSnapshot () {
			return _snapshotCamera.TakeSnapshot(512, 512);
		}

		private void OnDrawGizmos () {
			if (Application.isPlaying == false)
				return;

			Gizmos.color = new Color(1, 0, 1, 0.5f);
			for (int y = 0; y < _size.y; y++) {
				for (int x = 0; x < _size.x; x++) {
					Coord coord = new Coord(x, y);

					if (_state[coord] == BoardState.Vacant)
						continue;

					Vector3 cellCenterWorld = GetCellCenterWorld(coord);

					Gizmos.DrawWireCube(cellCenterWorld, Vector3.one);
					Gizmos.DrawCube(cellCenterWorld, Vector3.one);
				}
			}
		}

		public void ClearMask () {
			_boardMaskView.UnmaskAll();
		}

		public void Mask (HashSet <Coord> coords) {
			_boardMaskView.Mask(coords);
		}
	}
}
