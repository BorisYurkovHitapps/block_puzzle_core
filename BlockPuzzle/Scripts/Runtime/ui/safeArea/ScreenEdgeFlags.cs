using System;


namespace BlockPuzzle.Scripts.Runtime.ui.safeArea {
	[Flags]
	public enum ScreenEdgeFlags {
		None   = 0,
		Top    = 1 << 0,
		Bottom = 1 << 1,
		Left   = 1 << 2,
		Right  = 1 << 3
	}
}
