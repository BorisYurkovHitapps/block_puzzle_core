namespace BlockPuzzle.Scripts.Runtime.gameplay.turnProcessing {
	public readonly struct ScoreCalculationResult {
		public readonly ulong Score;
		public readonly int   Streak;
		public readonly int   Combo;


		public ScoreCalculationResult (ulong score, int streak, int combo) {
			Score  = score;
			Streak = streak;
			Combo  = combo;
		}

		public static ScoreCalculationResult Empty () {
			return new ScoreCalculationResult();
		}
	}
}
