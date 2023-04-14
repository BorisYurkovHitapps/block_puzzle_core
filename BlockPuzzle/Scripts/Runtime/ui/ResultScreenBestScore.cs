using BlockPuzzle.Scripts.Runtime.gameplay.turnProcessing;
using UnityEngine;


namespace BlockPuzzle.Scripts.Runtime.ui {
	public class ResultScreenBestScore : ResultScreen {
		protected override void Awake () {
			CanvasGroup = GetComponent <CanvasGroup>();

			gameObject.SetActive(false);

			TurnProcessor.AttemptFinishedWithBestScore += Show;
		}

		protected override void OnDestroy () {
			TurnProcessor.AttemptFinishedWithBestScore -= Show;
		}
	}
}
