using System;
using System.Collections.Generic;
using System.Threading;
using BlockPuzzle.Scripts.Runtime.analytics;
using BlockPuzzle.Scripts.Runtime.factories;
using BlockPuzzle.Scripts.Runtime.gameplay;
using BlockPuzzle.Scripts.Runtime.gameplay.board;
using BlockPuzzle.Scripts.Runtime.persistence;
using Cysharp.Threading.Tasks;
using UnityEngine;


namespace BlockPuzzle.Scripts.Runtime.tutorial {
	public class TutorialSequence {
		private readonly Queue <TutorialStep> _steps = new Queue <TutorialStep>();

		public Action StepChanged;

		private readonly GenericFactory   _factory;
		private readonly AnalyticsWrapper _analytics;


		public TutorialSequence (GenericFactory factory, AnalyticsWrapper analytics) {
			_factory   = factory;
			_analytics = analytics;
		}

		public async UniTask StartAsync (CancellationToken cancellationToken = default) {
			InitializeSteps();

			Debug.Log("Tutorial Sequence Started");

			int stepCounter = 0;

			await UniTask.Yield(cancellationToken);

			while (_steps.Count > 0) {
				Debug.Log($"Tutorial Step: {++stepCounter}");
				TutorialStep step = _steps.Dequeue();

				StepChanged?.Invoke();

				await step.RunAsync(cancellationToken);
				
				_analytics.tutorial_complete_step(step.Literal);
			}

			Debug.Log("Tutorial Sequence Ended");
		}

		private TutorialStep CreateStep () {
			TutorialStep step = _factory.Create <TutorialStep>();
			_steps.Enqueue(step);
			return step;
		}

		private void InitializeSteps () {
			const float  stepDelay        = 0.45f;
			const string analyticsLiteral = "block_puzzle_tutorial_";

			CreateStep()
				.WithAnalyticsLiteral($"{analyticsLiteral}1")
				.WithBoardState(new BoardState(9, 9, new byte[] {
					9, 9, 9, 9, 1, 9, 9, 9, 9,
					9, 9, 9, 9, 1, 9, 9, 9, 9,
					9, 9, 9, 9, 1, 9, 9, 9, 9,
					9, 9, 9, 9, 0, 9, 9, 9, 9,
					9, 9, 9, 9, 0, 9, 9, 9, 9,
					9, 9, 9, 9, 0, 9, 9, 9, 9,
					9, 9, 9, 9, 1, 9, 9, 9, 9,
					9, 9, 9, 9, 1, 9, 9, 9, 9,
					9, 9, 9, 9, 1, 9, 9, 9, 9
				}))
				.WithRosterState(new RosterState(new[] {
					null,
					new ShapePattern(new[] {
						new Coord(0, 0),
						new Coord(0, 1),
						new Coord(0, 2)
					}),
					null
				}))
				.WithMask(new HashSet <Coord>() {
					new Coord(4, 3),
					new Coord(4, 4),
					new Coord(4, 5),
				})
				.WithEndCondition(TutorialStepEndCondition.ShapePlaced);

			CreateStep()
				.WithDelay(stepDelay)
				.WithAnalyticsLiteral($"{analyticsLiteral}2")
				.WithBoardState(new BoardState(9, 9, new byte[] {
					9, 9, 9, 9, 9, 9, 9, 9, 9,
					9, 9, 9, 9, 9, 9, 9, 9, 9,
					9, 9, 9, 9, 9, 9, 9, 9, 9,
					9, 9, 9, 9, 9, 9, 9, 9, 9,
					1, 1, 1, 0, 0, 0, 1, 1, 1,
					9, 9, 9, 9, 9, 9, 9, 9, 9,
					9, 9, 9, 9, 9, 9, 9, 9, 9,
					9, 9, 9, 9, 9, 9, 9, 9, 9,
					9, 9, 9, 9, 9, 9, 9, 9, 9
				}))
				.WithRosterState(new RosterState(new[] {
					null,
					new ShapePattern(new[] {
						new Coord(0, 0),
						new Coord(1, 0),
						new Coord(2, 0)
					}),
					null
				}))
				.WithMask(new HashSet <Coord>() {
					new Coord(3, 4),
					new Coord(4, 4),
					new Coord(5, 4),
				})
				.WithEndCondition(TutorialStepEndCondition.ShapePlaced);

			CreateStep()
				.WithDelay(stepDelay)
				.WithAnalyticsLiteral($"{analyticsLiteral}3")
				.WithBoardState(new BoardState(9, 9, new byte[] {
					9, 9, 9, 9, 1, 9, 9, 9, 9,
					9, 9, 9, 9, 1, 9, 9, 9, 9,
					9, 9, 9, 9, 1, 9, 9, 9, 9,
					9, 9, 9, 9, 0, 9, 9, 9, 9,
					1, 1, 1, 0, 0, 0, 1, 1, 1,
					9, 9, 9, 9, 0, 9, 9, 9, 9,
					9, 9, 9, 9, 1, 9, 9, 9, 9,
					9, 9, 9, 9, 1, 9, 9, 9, 9,
					9, 9, 9, 9, 1, 9, 9, 9, 9
				}))
				.WithRosterState(new RosterState(new[] {
					null,
					new ShapePattern(new[] {
						new Coord(1, 0),
						new Coord(0, 1),
						new Coord(1, 1),
						new Coord(2, 1),
						new Coord(1, 2)
					}),
					null
				}))
				.WithMask(new HashSet <Coord>() {
					new Coord(4, 3),
					new Coord(3, 4),
					new Coord(4, 4),
					new Coord(5, 4),
					new Coord(4, 5),
				})
				.WithEndCondition(TutorialStepEndCondition.ShapePlaced);
		}
	}
}
