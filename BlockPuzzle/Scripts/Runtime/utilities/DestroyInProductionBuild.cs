using UnityEngine;


namespace BlockPuzzle.Scripts.Runtime.utilities {
	public class DestroyInProductionBuild : MonoBehaviour {
		private void Awake () {
			if ((Application.isEditor && Debug.isDebugBuild) == false)
				Destroy(gameObject);
		}
	}
}
