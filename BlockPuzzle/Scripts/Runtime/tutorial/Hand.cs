using BlockPuzzle.Scripts.Runtime.gameplay;
using BlockPuzzle.Scripts.Runtime.gameplay.board;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;


namespace BlockPuzzle.Scripts.Runtime.tutorial {
	[RequireComponent(typeof(CanvasGroup))]
	public class Hand : MonoBehaviour {
		#region Set in Inspector
		[SerializeField] private float _fadeSpeed = 3f;
		#endregion Set in Inspector

		private CanvasGroup _canvasGroup;

		private Tween _fadeTween;
		private Tween _moveTween;

		private Roster _roster;
		private Board  _board;


		[Inject]
		[UsedImplicitly]
		private void SetDependencies (BlockPuzzleGameHandler gameHandler) {
			_board  = gameHandler.Board;
			_roster = gameHandler.Roster;
		}

		private void Awake () {
			_canvasGroup       = GetComponent <CanvasGroup>();
			_canvasGroup.alpha = 0;

			_moveTween = DOTween.Sequence()
			                    .AppendCallback(() => transform.position = _roster.transform.position)
			                    .AppendInterval(0.75f)
			                    .Append(_canvasGroup.DOFade(1, 0.25f).From(0))
			                    .Append(transform.DOMove(_board.transform.position, 1.25f))
			                    .Append(_canvasGroup.DOFade(0, 0.25f))
			                    .SetLoops(-1)
			                    .SetAutoKill(false)
			                    .Pause()
			                    .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
		}

		private void OnEnable () {
			Shape.Grabbed += FadeOut;
			Shape.PutBack += PlayAnimation;
		}

		private void OnDisable () {
			Shape.Grabbed -= FadeOut;
			Shape.PutBack -= PlayAnimation;
		}

		private void FadeOut (Shape _) => FadeOut();
		private void PlayAnimation (Shape _) => PlayAnimation();

		public void FadeIn () {
			_moveTween.Pause();

			_fadeTween?.Kill();
			_fadeTween = _canvasGroup.DOFade(1, 1)
			                         .SetSpeedBased(true)
			                         .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
		}

		public void FadeOut () {
			_moveTween.Pause();

			_fadeTween?.Kill();
			_fadeTween = _canvasGroup.DOFade(0, _fadeSpeed)
			                         .SetSpeedBased(true)
			                         .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
		}

		public void PlayAnimation () {
			_fadeTween?.Kill();

			_moveTween.Restart();
		}
	}
}
