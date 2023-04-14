using UnityEngine;


namespace BlockPuzzle.Scripts.Runtime.extensions {
	public static class Vector3Extensions {
		public static Vector3 ToVector3 (this float value) {
			return new Vector3(value, value, value);
		}
	}
}
