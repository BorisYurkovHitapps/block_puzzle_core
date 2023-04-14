using BlockPuzzle.Scripts.Runtime.configs;
using BlockPuzzle.Scripts.Runtime.gameplay.board;


namespace BlockPuzzle.Scripts.Runtime.gameplay.turnProcessing {
	public class ScoreCalculator {
		private readonly int _boardWidth;
		private readonly int _boardHeight;

		private int _streak;

		private readonly BlockPuzzleConfig _config;


		public ScoreCalculator (Board board, BlockPuzzleConfig config) {
			_boardWidth  = board.Width;
			_boardHeight = board.Height;

			_config = config;
		}

		public ScoreCalculationResult Calculate (ShapePlacementResult shapePlacementResult, ClearLinesResult clearLinesResult) {
			if (shapePlacementResult.IsPlaced == false)
				return ScoreCalculationResult.Empty();

			int linesAssembled = clearLinesResult.LinesAssembled;

			UpdateStreak(linesAssembled);

			ulong score = 0;

			ApplyPlacedShapeScore(ref score, shapePlacementResult);
			ApplyClearedLinesScore(ref score, clearLinesResult);
			ApplyComboScore(ref score, linesAssembled, out int combo);

			return new ScoreCalculationResult(score, _streak, combo);
		}

		private void UpdateStreak (int linesAssembled) {
			if (linesAssembled == 0) {
				_streak = 0;
			} else {
				_streak += 1;
			}
		}

		private void ApplyPlacedShapeScore (ref ulong score, ShapePlacementResult shapePlacementResult) {
			score += (ulong)shapePlacementResult.OccupiedCoords.Count;
		}

		private void ApplyClearedLinesScore (ref ulong score, ClearLinesResult clearLinesResult) {
			score += (ulong)(clearLinesResult.RowsAssembled * _boardWidth);
			score += (ulong)(clearLinesResult.ColumnsAssembled * _boardHeight);
		}

		private void ApplyComboScore (ref ulong score, int linesAssembled, out int combo) {
			if (linesAssembled > 1) {
				score += (ulong)linesAssembled * _config.ComboScoreMultiplier;
				combo =  linesAssembled;
			} else {
				combo = 0;
			}
		}
	}
}
