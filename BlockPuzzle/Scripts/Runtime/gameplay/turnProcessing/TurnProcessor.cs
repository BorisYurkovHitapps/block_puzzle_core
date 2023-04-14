using System;
using System.Linq;
using System.Threading;
using BlockPuzzle.Scripts.Runtime.analytics;
using BlockPuzzle.Scripts.Runtime.configs;
using BlockPuzzle.Scripts.Runtime.gameplay.board;
using BlockPuzzle.Scripts.Runtime.persistence;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;
using IEventSystemHandler = Services.EventSystemHandler.IEventSystemHandler;


namespace BlockPuzzle.Scripts.Runtime.gameplay.turnProcessing {
	[UsedImplicitly]
	public class TurnProcessor : IInitializable, IDisposable {
		public static event Action <Vector3[], ulong> SingleLineAssembled;
		public static event Action <int>              StreakPerformed;
		public static event Action <int>              ComboPerformed;
		public static event Action <AttemptResult>    AttemptFinishedWithBestScore;
		public static event Action <AttemptResult>    AttemptFinishedWithoutBestScore;

		private bool _suspendFailCheckForTutorial;

		private readonly CancellationTokenSource _cancellationTokenSource;

		private readonly ScoreCalculator _scoreCalculator;

		private readonly BlockPuzzleConfig   _config;
		private readonly Roster              _roster;
		private readonly Board               _board;
		private readonly BlockPuzzleUserData _userData;
		private readonly BlockPuzzleAttempt  _attempt;
		private readonly AnalyticsWrapper    _analyticsWrapper;
		private readonly IEventSystemHandler _eventSystemHandler;


		private TurnProcessor (
			BlockPuzzleConfig config,
			Roster roster,
			Board board,
			BlockPuzzleUserData userData,
			BlockPuzzleAttempt attempt,
			AnalyticsWrapper analyticsWrapper,
			IEventSystemHandler eventSystemHandler
		) {
			_config             = config;
			_userData           = userData;
			_analyticsWrapper   = analyticsWrapper;
			_eventSystemHandler = eventSystemHandler;

			_roster  = roster;
			_board   = board;
			_attempt = attempt;

			_scoreCalculator = new ScoreCalculator(board, _config);

			_cancellationTokenSource = new CancellationTokenSource();
		}

		public void Initialize () {
			Shape.Released += OnShapeReleased;

			tutorial.Tutorial.Started += OnTutorialSequenceStarted;
			tutorial.Tutorial.Ended   += OnTutorialSequenceEnded;
		}

		public void Dispose () {
			Shape.Released -= OnShapeReleased;

			tutorial.Tutorial.Started -= OnTutorialSequenceStarted;
			tutorial.Tutorial.Ended   -= OnTutorialSequenceEnded;

			_cancellationTokenSource.Dispose();
		}

		private void OnTutorialSequenceStarted () {
			_suspendFailCheckForTutorial = true;
		}

		private void OnTutorialSequenceEnded () {
			_suspendFailCheckForTutorial = false;
		}

		private void OnShapeReleased (Shape shape) {
			ProcessTurnAsync(shape, _cancellationTokenSource.Token).Forget();
		}

		// TODO refactor
		// ReSharper disable once CognitiveComplexity
		private async UniTaskVoid ProcessTurnAsync (Shape shape, CancellationToken cancellationToken = default) {
			_eventSystemHandler.DisableInput();

			ShapePlacementResult shapePlacementResult = await _board.TryPlaceShapeAsync(shape, cancellationToken);

			if (shapePlacementResult.IsPlaced == false) {
				_eventSystemHandler.EnableInput();
				return;
			}

			ClearLinesResult       clearLinesResult       = await _board.TryClearLinesAsync(shapePlacementResult, cancellationToken);
			ScoreCalculationResult scoreCalculationResult = _scoreCalculator.Calculate(shapePlacementResult, clearLinesResult);

			if (clearLinesResult.LinesAssembled == 1) {
				if (scoreCalculationResult.Streak > 1) {
					StreakPerformed?.Invoke(scoreCalculationResult.Streak);
				} else {
					Vector3[] cellPositions = clearLinesResult.ClearedCoords
					                                          .Select(coord => _board.GetCellCenterWorld(coord))
					                                          .ToArray();

					SingleLineAssembled?.Invoke(cellPositions, scoreCalculationResult.Score);
				}
			} else if (clearLinesResult.LinesAssembled > 1) {
				ComboPerformed?.Invoke(clearLinesResult.LinesAssembled);
			}

			_attempt.Score.Value += scoreCalculationResult.Score;

			_roster.RemoveShape(shape);
			await _roster.RefillAsync(cancellationToken);

			bool hasAvailableTurns = false;

			foreach (Shape rosterShape in _roster.Shapes) {
				if (_board.HasAvailableSpaceFor(rosterShape)) {
					hasAvailableTurns = true;
					rosterShape.TurnOn();
				} else {
					rosterShape.TurnOff();
				}
			}

			if (hasAvailableTurns == false && _suspendFailCheckForTutorial == false) {
				bool  isBestScoreBeaten = _userData.TryUpdateBestScore(_attempt.Score.Value);
				ulong keysGained        = _attempt.Score.Value / _config.ScorePerKey;

				AttemptResult attemptResult = new AttemptResult(_attempt.Score.Value, keysGained);

				_userData.GrantKeys(keysGained);
				_userData.IncrementFinishedAttempts();

				_analyticsWrapper.game_complete_level(keysGained, _attempt.Time);

				_attempt.Reset();

				if (isBestScoreBeaten) {
					AttemptFinishedWithBestScore?.Invoke(attemptResult);
				} else {
					AttemptFinishedWithoutBestScore?.Invoke(attemptResult);
				}
			} else {
				_attempt.Store();
			}

			_eventSystemHandler.EnableInput();
		}
	}
}
