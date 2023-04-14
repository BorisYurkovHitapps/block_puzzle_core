using BlockPuzzle.Scripts.Runtime.persistence;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using Zenject;


namespace BlockPuzzle.Scripts.Runtime.ui {
	[RequireComponent(typeof(TMP_Text))]
	public class BestScorePresenter : MonoBehaviour {
		private TMP_Text            _textComponent;
		private BlockPuzzleUserData _userData;


		[Inject]
		[UsedImplicitly]
		private void SetDependencies (BlockPuzzleUserData userData) {
			_userData = userData;
		}

		private void OnEnable () {
			SetBestScorePresenterText();
		}

		private void Awake () {
			_textComponent = GetComponent <TMP_Text>();
		}

		private void SetBestScorePresenterText () {
			ulong bestScore = _userData.BestScore;

			_textComponent.SetText(bestScore.ToString());
		}
	}
}
