using System;
using BlockPuzzle.Scripts.Runtime.analytics;
using BlockPuzzle.Scripts.Runtime.gameplay;
using BlockPuzzle.Scripts.Runtime.gameplay.board;
using BlockPuzzle.Scripts.Runtime.persistence;
using BlockPuzzle.Scripts.Runtime.ui;
using JetBrains.Annotations;
using Services;
using UI.Screens;
using UnityEngine;
using Zenject;

namespace BlockPuzzle.Scripts.Runtime {
	public class BlockPuzzleGameHandler : MonoBehaviour {
		#region Set in Inspector
		[SerializeField] private Camera            _camera;
		[SerializeField] private Board             _board;
		[SerializeField] private Roster            _roster;
		[SerializeField] private tutorial.Tutorial _tutorial;
		[SerializeField] private PauseDialog 	   _pauseDialog;
		#endregion Set in Inspector

		public static event Action OnStopGame 	  = delegate {};
		
		public Camera Camera => _camera;
		public Board  Board  => _board;
		public Roster Roster => _roster;

		private BlockPuzzleUserData _userData;
		private BlockPuzzleAttempt  _attempt;
		private AnalyticsWrapper    _analyticsWrapper;
		private IAdManager 			_adManager;
		private IMainMediator       _mediator;

		public bool GameActive => gameObject.activeSelf;

		[Inject]
		[UsedImplicitly]
		private void SetDependencies (
			BlockPuzzleUserData userData,
			BlockPuzzleAttempt  attempt,
			AnalyticsWrapper 	analyticsWrapper,
			IAdManager 			adManager,
			IMainMediator		mediator
		) {
			_userData         = userData;
			_attempt          = attempt;
			_analyticsWrapper = analyticsWrapper;
			_adManager 		  = adManager;
			_mediator 		  = mediator;
		}

		private void Start () {
			gameObject.SetActive(false);
		}

		public void StartGame (AdPlacementName placement) {
			void StartGameInternal ()
			{
				if (_userData.IsTutorialFinished)
					_analyticsWrapper.game_start_level();
				
				_adManager.DisplayBanner();

				gameObject.SetActive(true);

				if (_userData.IsTutorialFinished == false) 
					_tutorial.StartSequenceAsync().Forget();

				_attempt.StartTimeTracking();
			}
			
			_adManager.ShowInterstitial(placement, StartGameInternal);
		}

		public void StopGame () {
			OnStopGame.Invoke();

			_attempt.StopTimeTracking();
			_board.Refresh();
			_roster.Refresh();

			gameObject.SetActive(false);
		}

		public void ClearSaveData () {
			_userData.Reset();
			_attempt.Reset();
		}
		
		public bool IsExistsSavedGame () {
			return _attempt.IsExistsSavedGame();
		}

		public void GoToMainMenu(int keys_gained = 0)
		{
			StopGame();
			_mediator.ShowMainMenu(new MainMenuData {keys_gained = keys_gained});
		}
		
		public bool IsOpenedPauseDialog()
		{
			return _pauseDialog.gameObject.activeSelf;
		}

		public void ClosePauseDialog()
		{
			_pauseDialog.Close();
		}
	}
}