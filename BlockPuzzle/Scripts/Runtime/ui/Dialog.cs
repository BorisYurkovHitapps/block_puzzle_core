using System;
using DG.Tweening;
using UnityEngine;


namespace BlockPuzzle.Scripts.Runtime.ui {
	[RequireComponent(typeof(CanvasGroup))]
	public class Dialog : MonoBehaviour {
		#region Set in Inspector
		[SerializeField, Min(0)] private float _fadeInDelay;
		[SerializeField, Min(0)] private float _fadeDuration = 0.25f;
		#endregion


		private CanvasGroup _canvasGroup;


		protected void Awake () {
			_canvasGroup = GetComponent <CanvasGroup>();
		}

		private void Start () {
			gameObject.SetActive(false);
		}

		public virtual void Show () {
			gameObject.SetActive(true);

			_canvasGroup.alpha        = 0;
			_canvasGroup.interactable = false;

			_canvasGroup.DOKill();
			_canvasGroup
				.DOFade(1, _fadeDuration)
				.SetDelay(_fadeInDelay)
				.OnStart(() => _canvasGroup.interactable = true)
				.SetLink(gameObject, LinkBehaviour.KillOnDestroy);
		}

		public void Hide (Action onComplete) {
			_canvasGroup.interactable = false;

			_canvasGroup.DOKill();
			_canvasGroup
				.DOFade(0, _fadeDuration)
				.OnComplete(() => {
					onComplete?.Invoke();
					gameObject.SetActive(false);
				})
				.SetLink(gameObject, LinkBehaviour.KillOnDestroy);
		}
	}
}
