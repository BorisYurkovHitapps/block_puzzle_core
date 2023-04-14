using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BlockPuzzle.Scripts.Runtime.extensions;
using BlockPuzzle.Scripts.Runtime.factories;
using BlockPuzzle.Scripts.Runtime.gameplay.board;
using BlockPuzzle.Scripts.Runtime.persistence;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Services.EventSystemHandler;
using UnityEngine;
using Zenject;


namespace BlockPuzzle.Scripts.Runtime.gameplay {
	public class Roster : MonoBehaviour {
		#region Set in Inspector
		[SerializeField] private Board _board;
		[SerializeField] private Shape _shapePrototype;

		[Space]
		[SerializeField, Min(1)] private int _shapeCount;
		[SerializeField, Min(0)]      private float _shapeSpacing;
		[SerializeField, Range(0, 1)] private float _shapeScale;
		#endregion

		private Shape[] _shapes;

		private bool _suspendRefreshForTutorial;

		private ShapePatternProvider _shapePatternProvider;
		private GameObjectFactory    _shapeFactory;
		private BlockPuzzleAttempt   _attempt;
		private IEventSystemHandler  _eventSystemHandler;


		private bool                IsEmpty => _shapes.All(shape => shape == null);
		public  IEnumerable <Shape> Shapes  => _shapes.Where(shape => shape != null);


		[Inject]
		[UsedImplicitly]
		private void SetDependencies (
			ShapePatternProvider shapePatternProvider,
			GameObjectFactory factory,
			BlockPuzzleAttempt attempt,
			IEventSystemHandler eventSystemHandler
		) {
			_eventSystemHandler   = eventSystemHandler;
			_shapePatternProvider = shapePatternProvider;
			_shapeFactory         = factory;
			_attempt              = attempt;
		}

		private void Awake () {
			_shapes = new Shape[_shapeCount];

			_attempt.StoreRequested += OnStoreAttemptRequested;

			tutorial.Tutorial.Started += OnTutorialSequenceStarted;
			tutorial.Tutorial.Ended   += OnTutorialSequenceEnded;
		}

		private void Start () {
			TryApplyAttemptState();
		}

		private void TryApplyAttemptState () {
			if (_attempt.TryGetRosterState(out RosterState rosterState)) {
				ApplyStateAsync(rosterState).Forget();
			} else {
				RefillAsync(gameObject.GetCancellationTokenOnDestroy()).Forget();
			}
		}

		private void OnDestroy () {
			_attempt.StoreRequested -= OnStoreAttemptRequested;

			tutorial.Tutorial.Started -= OnTutorialSequenceStarted;
			tutorial.Tutorial.Ended   -= OnTutorialSequenceEnded;
		}

		private void OnTutorialSequenceStarted () {
			_suspendRefreshForTutorial = true;
		}

		private void OnTutorialSequenceEnded () {
			_suspendRefreshForTutorial = false;
			
			RefillAsync(gameObject.GetCancellationTokenOnDestroy()).Forget();
		}

		public async UniTaskVoid ApplyStateAsync (RosterState rosterState, CancellationToken cancellationToken = default) {
			_eventSystemHandler.DisableInput();

			HashSet <UniTask> tasks = new HashSet <UniTask>();

			IList <ShapePattern> shapePatterns = rosterState.GetShapePatterns();

			for (int i = 0; i < shapePatterns.Count; i++) {
				ShapePattern shapePattern = shapePatterns[i];

				if (shapePattern == null)
					continue;

				tasks.Add(SpawnShapeAsync(i, shapePattern, cancellationToken));
			}

			await UniTask.WhenAll(tasks);

			_eventSystemHandler.EnableInput();
		}

		public async UniTask RefillAsync (CancellationToken cancellationToken = default) {
			if (_suspendRefreshForTutorial)
				return;

			if (IsEmpty == false)
				return;

			HashSet <UniTask> tasks = new HashSet <UniTask>();

			for (int shapeIndex = 0; shapeIndex < _shapes.Length; shapeIndex++) {
				Shape shape = _shapes[shapeIndex];

				if (shape != null)
					continue;

				tasks.Add(SpawnRandomShapeAsync(shapeIndex, cancellationToken));
			}

			await UniTask.WhenAll(tasks);
		}

		private async UniTask SpawnShapeAsync (int shapeIndex, ShapePattern pattern, CancellationToken cancellationToken = default) {
			Vector3 position = CalculateShapePosition(shapeIndex);

			Shape shape = _shapeFactory.Create(_shapePrototype, transform);
			shape.SetScale(_shapeScale);
			shape.SetPosition(position);
			shape.AssignPattern(pattern);

			_shapes[shapeIndex] = shape;

			if (_board.HasAvailableSpaceFor(shape) == false)
				shape.TurnOff();

			await shape.PopInAsync(cancellationToken);
		}

		private async UniTask SpawnRandomShapeAsync (int shapeIndex, CancellationToken cancellationToken = default) {
			await SpawnShapeAsync(
				shapeIndex,
				_shapePatternProvider.GetRandomShapePattern(),
				cancellationToken
			);
		}

		private Vector3 CalculateShapePosition (int shapeIndex) {
			float halfWidth = _shapeSpacing * (_shapeCount - 1) / 2.0f;
			float offset    = _shapeSpacing * shapeIndex - halfWidth;

			Vector3 position = transform.position;
			position.x += offset;

			return position;
		}

		public void RemoveShape (Shape shape) {
			int shapeIndex = Array.IndexOf(_shapes, shape);
			_shapes[shapeIndex] = null;
		}

		private void OnStoreAttemptRequested () {
			RosterState rosterState = IsEmpty ? null : new RosterState(_shapes);
			_attempt.SetRosterState(rosterState);
		}

		public void Refresh () {
			Clear();
			TryApplyAttemptState();
		}

		public void Clear () {
			for (var i = 0; i < _shapes.Length; i++) {
				Shape shape = _shapes[i];
				_shapes[i] = null;

				if (shape == null)
					continue;

				Destroy(shape.gameObject);
			}
		}


		private void OnDrawGizmos () {
			Gizmos.color = Color.green;

			for (int i = 0; i < _shapeCount; i++) {
				Vector3 position = CalculateShapePosition(i);
				Gizmos.DrawWireCube(position, (Shape.MaxSize * _shapeScale).ToVector3());
			}
		}
	}
}
