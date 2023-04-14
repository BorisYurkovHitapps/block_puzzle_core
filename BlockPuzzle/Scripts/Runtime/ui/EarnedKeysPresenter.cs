using BlockPuzzle.Scripts.Runtime.configs;
using BlockPuzzle.Scripts.Runtime.persistence;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using Zenject;


namespace BlockPuzzle.Scripts.Runtime.ui {
	[RequireComponent(typeof(TMP_Text))]
	public class EarnedKeysPresenter : MonoBehaviour {
		private TMP_Text _keysPresenter;

		private BlockPuzzleConfig  _config;
		private BlockPuzzleAttempt _attempt;


		[Inject]
		[UsedImplicitly]
		private void SetDependencies (BlockPuzzleConfig config, BlockPuzzleAttempt attempt) {
			_config  = config;
			_attempt = attempt;
		}

		private void Awake () {
			_keysPresenter = GetComponent <TMP_Text>();

			_attempt.Score.Subscribe(Refresh);
		}

		private void Start () {
			Refresh(_attempt.Score.Value);
		}

		private void OnDestroy () {
			_attempt.Score.Unsubscribe(Refresh);
		}

		private void Refresh (ulong score) {
			_keysPresenter.SetText((score / _config.ScorePerKey).ToString());
		}
	}
}
