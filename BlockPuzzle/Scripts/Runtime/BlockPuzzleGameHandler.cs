using BlockPuzzle.Scripts.Runtime.analytics;
using BlockPuzzle.Scripts.Runtime.gameplay;
using BlockPuzzle.Scripts.Runtime.gameplay.board;
using BlockPuzzle.Scripts.Runtime.persistence;
using Flime.Core.Platform.Interfaces;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;


namespace BlockPuzzle.Scripts.Runtime {
	public class BlockPuzzleGameHandler : MonoBehaviour {
		#region Set in Inspector
		[SerializeField] private Camera            _camera;
		[SerializeField] private Board             _board;
		[SerializeField] private Roster            _roster;
		[SerializeField] private tutorial.Tutorial _tutorial;
		#endregion Set in Inspector

		public Camera Camera => _camera;
		public Board  Board  => _board;
		public Roster Roster => _roster;

		private BlockPuzzleUserData _userData;
		private BlockPuzzleAttempt  _attempt;
		private IAds                _ads;
		private AnalyticsWrapper    _analyticsWrapper;


		[Inject]
		[UsedImplicitly]
		private void SetDependencies (
			BlockPuzzleUserData userData,
			BlockPuzzleAttempt attempt,
			IAds ads,
			AnalyticsWrapper analyticsWrapper
		) {
			_userData         = userData;
			_attempt          = attempt;
			_ads              = ads;
			_analyticsWrapper = analyticsWrapper;
		}

		private void Start () {
			gameObject.SetActive(false);
		}

		public void StartGame () {
			void StartGameInternal () {
				_analyticsWrapper.game_start_level();
				_ads.DisplayBanner();

				gameObject.SetActive(true);

				if (_userData.IsTutorialFinished == false) 
					_tutorial.StartSequenceAsync().Forget();

				_attempt.StartTimeTracking();
			}

			_ads.ShowInterstitial(
				StartGameInternal,
				error => StartGameInternal(),
				"block_puzzle" // TODO what
			);
		}

		public void StopGame () {
			_ads.HideBanner();

			_attempt.StopTimeTracking();
			_board.Refresh();
			_roster.Refresh();

			gameObject.SetActive(false);
		}

		public void ClearSaveData () {
			_userData.Reset();
			_attempt.Reset();
		}
	}
}
