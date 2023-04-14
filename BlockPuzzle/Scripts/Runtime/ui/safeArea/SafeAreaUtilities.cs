using UnityEngine;


namespace BlockPuzzle.Scripts.Runtime.ui.safeArea {
	public static class SafeAreaUtilities {
		public static int TopMargin {
			get {
				Rect safeArea   = Screen.safeArea;
				Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);

				return (int)(screenRect.height - safeArea.height - safeArea.y);
			}
		}

		public static int BottomMargin => (int)Screen.safeArea.y;

		public static int LeftMargin => (int)Screen.safeArea.x;

		public static int RightMargin {
			get {
				Rect safeArea   = Screen.safeArea;
				Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);

				return (int)(screenRect.width - safeArea.width - safeArea.x);
			}
		}

		public static bool Contains (Vector2 point) {
			return Screen.safeArea.Contains(point);
		}
	}
}
