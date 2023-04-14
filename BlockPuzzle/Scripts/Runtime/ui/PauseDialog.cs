using BlockPuzzle.Scripts.Runtime.persistence;
using JetBrains.Annotations;
using Services;
using Zenject;


namespace BlockPuzzle.Scripts.Runtime.ui {
	public class PauseDialog : Dialog {
		private BlockPuzzleGameHandler _gameHandler;
		private BlockPuzzleAttempt     _attempt;
		private IMainMediator          _mediator;


		[Inject]
		[UsedImplicitly]
		private void SetDependencies (BlockPuzzleGameHandler gameHandler, BlockPuzzleAttempt attempt, IMainMediator mediator) {
			_gameHandler = gameHandler;
			_attempt     = attempt;
			_mediator    = mediator;
		}

		public void GoToMainMenu () {
			_gameHandler.GoToMainMenu();

			Hide(null);
		}

		public void GoToSettings () {
			_mediator.ShowSettings();

			Hide(null);
		}


		public override void Show () {
			_attempt.StopTimeTracking();

			base.Show();
		}

		public void Close () {
			_attempt.StartTimeTracking();

			Hide(null);
		}
	}
}
