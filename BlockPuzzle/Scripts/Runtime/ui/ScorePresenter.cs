using BlockPuzzle.Scripts.Runtime.persistence;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Zenject;


namespace BlockPuzzle.Scripts.Runtime.ui {
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class ScorePresenter : MonoBehaviour {
		private TMP_Text _textComponent;


		private ulong _score;
		private float _presentedScore;
		private Tween _tween;


		private BlockPuzzleAttempt _attempt;


		[Inject]
		private void SetDependencies (BlockPuzzleAttempt attempt) {
			_attempt = attempt;
		}

		private void Awake () {
			_textComponent = GetComponent <TextMeshProUGUI>();

			_attempt.Score.Subscribe(OnScoresChanged);
		}

		private void Start () {
			UpdatePresenter(_attempt.Score.Value);
		}

		private void OnDestroy () {
			_attempt.Score.Unsubscribe(OnScoresChanged);
		}

		private void OnScoresChanged (ulong currentScores) {
			ulong delta = currentScores - _score;

			_score = currentScores;

			_tween?.Kill();
			_tween = DOTween
			         .To(UpdatePresenter, _presentedScore, _score, Mathf.Min(0.05f * delta, 0.5f))
			         .SetEase(Ease.InCubic)
			         .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
		}

		private void UpdatePresenter (float value) {
			if (value.Equals(0)) {
				_textComponent.SetText(string.Empty);
				return;
			}

			_presentedScore = value;
			_textComponent.SetText(Mathf.RoundToInt(value).ToString());
		}
	}
}
