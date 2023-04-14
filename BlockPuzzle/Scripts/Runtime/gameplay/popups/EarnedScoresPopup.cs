using System.Linq;
using BlockPuzzle.Scripts.Runtime.gameplay.turnProcessing;
using DG.Tweening;
using TMPro;
using UnityEngine;


namespace BlockPuzzle.Scripts.Runtime.gameplay.popups {
	public class EarnedScoresPopup : MonoBehaviour {
		private RectTransform   _rectTransform;
		private CanvasGroup     _canvasGroup;
		private TextMeshProUGUI _scorePresenter;
		private Sequence        _showSequence;


		private void Awake () {
			_rectTransform  = (RectTransform)transform;
			_scorePresenter = GetComponent <TextMeshProUGUI>();
			_canvasGroup    = GetComponent <CanvasGroup>();

			_canvasGroup.alpha = 0;

			TurnProcessor.SingleLineAssembled += Show;
		}

		private void OnDestroy () {
			TurnProcessor.SingleLineAssembled -= Show;
		}


		private void Show (Vector3[] cellPositions, ulong earnedScore) {
			Vector3 averagePosition = cellPositions.Aggregate(Vector3.zero, (sum, v) => sum + v) / cellPositions.Length;

			_rectTransform.position   = averagePosition;
			_rectTransform.localScale = Vector3.zero;

			_scorePresenter.SetText($"+{earnedScore.ToString()}");
			_canvasGroup.alpha = 0;

			_showSequence?.Kill();
			_showSequence = DOTween.Sequence()
			                       .Append(_rectTransform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutElastic))
			                       .Join(_canvasGroup.DOFade(1, 0.5f))
			                       .Append(_rectTransform.DOLocalMoveY(_rectTransform.localPosition.y + 400, 0.75f)
			                                             .SetEase(Ease.OutCubic))
			                       .Join(_canvasGroup.DOFade(0, 0.5f).SetEase(Ease.OutCubic))
			                       .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
		}
	}
}
