namespace BlockPuzzle.Scripts.Runtime.gameplay.turnProcessing {
	public readonly struct AttemptResult {
		public readonly ulong EarnedScore;
		public readonly ulong EarnedKeys;


		public AttemptResult (ulong earnedScore, ulong earnedKeys) {
			EarnedKeys  = earnedKeys;
			EarnedScore = earnedScore;
		}
	}
}
