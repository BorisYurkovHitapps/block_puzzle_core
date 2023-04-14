using System;
using System.Collections.Generic;
using System.Threading;
using BlockPuzzle.Scripts.Runtime.gameplay;
using BlockPuzzle.Scripts.Runtime.gameplay.board;
using BlockPuzzle.Scripts.Runtime.persistence;
using Cysharp.Threading.Tasks;
using Services.EventSystemHandler;


namespace BlockPuzzle.Scripts.Runtime.tutorial {
	public class TutorialStep {
		private Action _onStart;
		private Action _onComplete;

		private float? _delay;
		private bool   _isRunning;

		private TutorialStepEndCondition _endCondition = TutorialStepEndCondition.Immediate;

		private readonly Board  _board;
		private readonly Roster _roster;

		private readonly IEventSystemHandler _eventSystemHandler;


		public TutorialStep (Board board, Roster roster, IEventSystemHandler eventSystemHandler) {
			_board              = board;
			_roster             = roster;
			_eventSystemHandler = eventSystemHandler;
		}

		public string Literal {get; private set;}

		private async UniTask DelayAsync (CancellationToken cancellationToken = default) {
			if (_delay == null)
				return;

			_eventSystemHandler.DisableInput();

			await UniTask.Delay(TimeSpan.FromSeconds(_delay.Value), cancellationToken: cancellationToken);

			_eventSystemHandler.EnableInput();
		}

		public async UniTask RunAsync (CancellationToken cancellationToken = default) {
			ApplyEndCondition();

			await DelayAsync(cancellationToken);

			_onStart?.Invoke();

			_isRunning = true;

			await UniTask.WaitUntil(() => _isRunning == false, cancellationToken: cancellationToken);

			_onComplete?.Invoke();
		}

		private void ApplyEndCondition () {
			switch (_endCondition) {
				case TutorialStepEndCondition.Immediate:
					_onStart += Stop;
					break;
				case TutorialStepEndCondition.ShapePlaced:
					_onStart    += () => Board.ShapePlaced += Stop;
					_onComplete -= () => Board.ShapePlaced -= Stop;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void Stop () => _isRunning = false;

		private void Stop (Shape shape) => Stop();

		public TutorialStep WithDelay (float delay) {
			_delay = delay;

			return this;
		}

		public TutorialStep WithBoardState (BoardState boardState) {
			_onStart += () => {
				_board.ApplyState(boardState);
				_board.Draw();
			};

			_onComplete += _board.Refresh;

			return this;
		}

		public TutorialStep WithRosterState (RosterState rosterState) {
			_onStart += () => {
				_roster.Clear();
				_roster.ApplyStateAsync(rosterState).Forget();
			};

			return this;
		}

		public TutorialStep WithMask (HashSet <Coord> coords) {
			_onStart += () => {
				_board.ClearMask();
				_board.Mask(coords);
			};

			_onComplete += _board.ClearMask;

			return this;
		}

		public TutorialStep WithAnalyticsLiteral (string literal) {
			Literal = literal;

			return this;
		}

		public TutorialStep WithEndCondition (TutorialStepEndCondition condition) {
			_endCondition = condition;

			return this;
		}
	}
}
