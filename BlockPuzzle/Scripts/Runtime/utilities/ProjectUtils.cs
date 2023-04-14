// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global


using UnityEngine;


namespace BlockPuzzle.Scripts.Runtime.utilities {
	public static class ProjectUtils {
		public const string GameTitle = "Block Puzzle";

		public static bool CanCheat => Application.isEditor || Debug.isDebugBuild;
	}
}
