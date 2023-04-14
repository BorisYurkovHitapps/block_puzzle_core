using DG.Tweening;
using TMPro;
using UnityEngine;
using Zenject;


// TODO ***
namespace BlockPuzzle.Scripts.Runtime.gameplay.popups {
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(CanvasGroup))]
	public abstract class BoardPopup : MonoBehaviour {
		#region Set in Inspector
		[SerializeField] private TextMeshProUGUI _popupTextPresenter;

		[Header("Sfx")]
		[SerializeField] private AudioClip _popupSfx;
		#endregion


		// private Tutorial     _tutorial; // TODO what

		[Inject]
		private void SetDependencies (
			// ,
			// Tutorial tutorial
		) {
			// _tutorial     = tutorial; // TODO -?
		}

		private RectTransform _rectTransform;
		private CanvasGroup   _canvasGroup;
		private Vector2       _initialPosition;
		private Sequence      _showSequence;


		private void Awake () {
			_rectTransform = GetComponent <RectTransform>();
			_canvasGroup   = GetComponent <CanvasGroup>();

			_canvasGroup.alpha = 0;
		}

		protected virtual void Start () {
			_canvasGroup.alpha = 0;
			_initialPosition   = _rectTransform.anchoredPosition;
		}

		protected void Show (int combo) {
			// if (_tutorial.IsRunning) return; // TODO what

			_popupTextPresenter.SetText($"x{combo.ToString()}");

			_rectTransform.localScale       = Vector3.zero;
			_rectTransform.anchoredPosition = _initialPosition;

			_canvasGroup.alpha = 0;

			const float duration = 0.5f;

			_showSequence?.Kill();
			_showSequence = DOTween.Sequence()
			                       .Append(_rectTransform.DOScale(Vector3.one, duration).SetEase(Ease.OutElastic, 0.5f, 2))
			                       .Join(_canvasGroup.DOFade(1, duration))
			                       .AppendInterval(0.25f)
			                       .Append(_rectTransform.DOLocalMoveY(_rectTransform.localPosition.y + 400, 0.75f)
			                                             .SetEase(Ease.OutCubic))
			                       .Join(_canvasGroup.DOFade(0, duration).SetEase(Ease.OutCubic))
			                       .SetLink(gameObject, LinkBehaviour.KillOnDestroy);

			// _audioService.PlaySfx(_popupSfx); // TODO what
		}
	}
}
