using System;
using Audio;
using BlockPuzzle.Scripts.Runtime.gameplay.turnProcessing;
using BlockPuzzle.Scripts.Runtime.persistence;
using DG.Tweening;
using Flime.Core.Platform.Interfaces;
using JetBrains.Annotations;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


namespace BlockPuzzle.Scripts.Runtime.ui {
	[RequireComponent(typeof(CanvasGroup))]
	public class ResultScreen : MonoBehaviour {
		#region Set in Inspector
		[SerializeField] private Image    _levelPreviewPresenter;
		[SerializeField] private TMP_Text _earnedScorePresenter;
		[SerializeField] private TMP_Text _bestScorePresenter;
		[SerializeField] private TMP_Text _earnedKeysPresenter;

		[Space]
		[SerializeField] private AudioClip _sfx;
		#endregion Set in Inspector

		private BlockPuzzleUserData    _userData;
		private BlockPuzzleGameHandler _gameHandler;
		private IMainMediator          _mediator;
		private IAds                   _ads;

		protected CanvasGroup CanvasGroup;


		[Inject]
		[UsedImplicitly]
		private void SetDependencies (
			BlockPuzzleGameHandler gameHandler,
			BlockPuzzleUserData userData,
			IMainMediator mediator,
			IAds ads
		) {
			_mediator    = mediator;
			_gameHandler = gameHandler;
			_userData    = userData;
			_ads         = ads;
		}

		protected virtual void Awake () {
			CanvasGroup = GetComponent <CanvasGroup>();

			gameObject.SetActive(false);

			TurnProcessor.AttemptFinishedWithoutBestScore += Show;
		}

		protected virtual void OnDestroy () {
			TurnProcessor.AttemptFinishedWithoutBestScore -= Show;
		}

		protected void Show (AttemptResult attemptResult) {
			_ads.HideBanner();

			gameObject.SetActive(true);

			_earnedScorePresenter.SetText(attemptResult.EarnedScore.ToString());
			_bestScorePresenter.SetText(_userData.BestScore.ToString());
			_earnedKeysPresenter.SetText(attemptResult.EarnedKeys.ToString());

			_levelPreviewPresenter.sprite = _gameHandler.Board.TakeLevelSnapshot();


			CanvasGroup.DOKill();
			CanvasGroup.DOFade(1, 1.5f)
			           .From(0)
			           .SetLink(gameObject, LinkBehaviour.KillOnDestroy);

			AudioManager.PlayOnce(_sfx);
		}

		private void Hide (Action onComplete = null) {
			CanvasGroup.DOKill();
			CanvasGroup.DOFade(0, 1.5f)
			           .OnComplete(() => {
				           gameObject.SetActive(false);
				           onComplete?.Invoke();
			           })
			           .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
		}

		public void GoToMainMenu () {
			void NewFunction () {
				_gameHandler.StopGame();

				Hide();

				_mediator.ShowMainMenu();
			}

			_ads.ShowInterstitial(
				NewFunction,
				error => NewFunction(),
				"block_puzzle" // TODO what
			);
		}

		public void GetMoreKeys () {
			_gameHandler.StopGame();

			Hide();

			_gameHandler.StartGame();
		}
	}
}
