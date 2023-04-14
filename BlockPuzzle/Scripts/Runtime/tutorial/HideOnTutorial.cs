using UnityEngine;


namespace BlockPuzzle.Scripts.Runtime.tutorial {
	[DisallowMultipleComponent]
	public class HideOnTutorial : MonoBehaviour {
		private void Awake () {
			Tutorial.Started += OnTutorialSequenceStarted;
			Tutorial.Ended   += OnTutorialSequenceEnded;
		}

		private void OnDestroy () {
			Tutorial.Started -= OnTutorialSequenceStarted;
			Tutorial.Ended   -= OnTutorialSequenceEnded;
		}

		private void OnTutorialSequenceStarted () {
			gameObject.SetActive(false);
		}

		private void OnTutorialSequenceEnded () {
			gameObject.SetActive(true);
		}
	}
}
