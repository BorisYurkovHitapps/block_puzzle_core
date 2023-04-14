using BlockPuzzle.Scripts.Runtime.persistence;
using BlockPuzzle.Scripts.Runtime.utilities;
using UnityEditor;
using UnityEngine;


namespace BlockPuzzle.Scripts.Editor {
	public static class BlockPuzzleSaveDataEditorUtils {
		[MenuItem(ProjectUtils.GameTitle + "/Clear Save Data")]
		public static void ClearSaveData () {
			if (Application.isPlaying) {
				Debug.LogWarning("Cannot delete save data in Play Mode.");
				return;
			}


			BlockPuzzleUserData userData = new BlockPuzzleUserData();
			BlockPuzzleAttempt  attempt  = new BlockPuzzleAttempt();

			userData.Reset();
			attempt.Reset();

			Debug.Log("Block Puzzle User Data cleared.");
			Debug.Log("Block Puzzle Attempt Data cleared.");
		}
	}
}
