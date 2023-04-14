using BlockPuzzle.Scripts.Runtime.configs;
using BlockPuzzle.Scripts.Runtime.persistence;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


namespace BlockPuzzle.Scripts.Runtime.ui {
	[RequireComponent(typeof(Slider))]
	public class KeyProgressBar : MonoBehaviour {
		#region Set in Inspector
		[SerializeField] private Image _fill;
		[SerializeField] private float _animationDuration;
		#endregion Set in Inspector

		private Slider _slider;


		private BlockPuzzleAttempt _attempt;
		private BlockPuzzleConfig  _config;
		private Tween              _tween;


		[Inject]
		[UsedImplicitly]
		private void SetDependencies (BlockPuzzleAttempt attempt, BlockPuzzleConfig config) {
			_attempt = attempt;
			_config  = config;
		}

		private void Awake () {
			_slider = GetComponent <Slider>();

			_attempt.Score.Subscribe(OnScoreChanged);
		}

		private void OnEnable () {
			Refresh();
		}

		private void OnDestroy () {
			_attempt.Score.Unsubscribe(OnScoreChanged);
		}

		private void OnScoreChanged (ulong _) {
			Refresh();
		}

		private void Refresh () {
			if (_attempt.Score.Value % _config.ScorePerKey == 0) {
				_slider.value = 0;
				_fill.enabled = false;
				return;
			}

			_fill.enabled = true;

			float targetValue = (float)(_attempt.Score.Value % _config.ScorePerKey) / _config.ScorePerKey;

			if (targetValue < _slider.value)
				targetValue += 1;

			_tween.Kill();
			_tween = DOTween.To(
				() => _slider.value,
				value => _slider.value = Mathf.Repeat(value, 1.0f),
				targetValue,
				_animationDuration
			).SetLink(gameObject, LinkBehaviour.KillOnDestroy);
		}
	}
}
