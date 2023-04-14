using BlockPuzzle.Scripts.Runtime.configs;
using BlockPuzzle.Scripts.Runtime.persistence;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using Zenject;


namespace BlockPuzzle.Scripts.Runtime.ui {
	[RequireComponent(typeof(TMP_Text))]
	public class NextKeyPresenter : MonoBehaviour {
		private BlockPuzzleConfig  _config;
		private BlockPuzzleAttempt _attempt;

		private TMP_Text _textComponent;
		private string   _pattern;


		[Inject]
		[UsedImplicitly]
		private void SetDependencies (BlockPuzzleConfig config, BlockPuzzleAttempt attempt) {
			_config  = config;
			_attempt = attempt;
		}

		private void Awake () {
			_textComponent = GetComponent <TMP_Text>();
			_pattern       = _textComponent.text;

			_attempt.Score.Subscribe(Refresh);
		}

		private void Start () {
			Refresh(_attempt.Score.Value);
		}

		private void OnDestroy () {
			_attempt.Score.Unsubscribe(Refresh);
		}

		private void Refresh (ulong score) {
			ulong  nextKey = (_attempt.Score.Value / _config.ScorePerKey + 1) * _config.ScorePerKey;
			string text    = _pattern.Replace("@@@", nextKey.ToString());

			_textComponent.SetText(text);
		}
	}
}
