namespace BlockPuzzle.Scripts.Runtime.ui.safeArea {
	public enum ScreenEdge {
		Top         = 1 << 0,
		Bottom      = 1 << 1,
		Left        = 1 << 2,
		Right       = 1 << 3,
		TopLeft     = Top | Left,
		TopRight    = Top | Right,
		BottomLeft  = Bottom | Left,
		BottomRight = Bottom | Right
	}
}
