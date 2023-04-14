using System;
using System.Collections.Generic;
using System.Threading;
using Audio;
using BlockPuzzle.Scripts.Runtime.extensions;
using BlockPuzzle.Scripts.Runtime.ui.safeArea;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using Zenject;


namespace BlockPuzzle.Scripts.Runtime.gameplay {
	[RequireComponent(typeof(BoxCollider2D))]
	public class Shape : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
		public const int MaxSize = 5;

		public static event Action <Shape> Grabbed;
		public static event Action <Shape> Moved;
		public static event Action <Shape> Released;
		public static event Action <Shape> PutBack;

		#region Set in Inspector
		[SerializeField] private ShapeBlock _shapeBlockPrototype;

		[Space]
		[SerializeField] private Transform _pivot;
		[SerializeField] private Transform    _blockGroup;
		[SerializeField] private SortingGroup _sortingGroup;

		[Space]
		[SerializeField] private float _animationDuration = 0.1f;
		[SerializeField] private float _dragYOffset;

		[Header("Sfx")]
		[SerializeField] private AudioClip _pickUpSfx;
		[SerializeField] private AudioClip _placeSfx;
		[SerializeField] private AudioClip _putBackSfx;
		#endregion

		private BoxCollider2D _collider;

		private Vector3 _initialPosition;
		private Vector3 _targetPosition;
		private Vector3 _initialScale;
		private bool    _isDragged;
		private bool    _canScan;


		private readonly HashSet <Coord>      _coords = new HashSet <Coord>();
		private readonly HashSet <ShapeBlock> _blocks = new HashSet <ShapeBlock>();

		private Tween    _transformTween;
		private Sequence _pivotSequence;

		private bool IsInteractable {
			get => _collider.enabled;
			set => _collider.enabled = value;
		}

		public int Width  {get; private set;}
		public int Height {get; private set;}

		public IEnumerable <Coord>      Coords => _coords;
		public IEnumerable <ShapeBlock> Blocks => _blocks;

		private Camera _camera;


		[Inject]
		[UsedImplicitly]
		private void SetDependencies (Camera camera) {
			_camera = camera;
		}

		private void Awake () {
			_collider = GetComponent <BoxCollider2D>();

			_isDragged = false;
		}

		private void Update () {
			if (_isDragged == false)
				return;

			if (_canScan)
				Moved?.Invoke(this);

			UpdatePosition();
		}

		public void AssignPattern (ShapePattern pattern) {
			CalculateCoords(pattern);
			CalculateSize();
			SpawnBlocks();
		}

		public void SetPosition (Vector3 position) {
			_initialPosition   = position;
			_targetPosition    = position;
			transform.position = position;
		}

		public void SetScale (float scale) {
			_initialScale     = scale.ToVector3();
			_pivot.localScale = _initialScale;

			_collider.size = (MaxSize * scale).ToVector3();
		}

		public async UniTask PopInAsync (CancellationToken cancellationToken) {
			const float duration = 0.5f;

			HashSet <UniTask> tasks = new HashSet <UniTask>();

			UniTask scaleTask = _blockGroup.DOScale(Vector3.one, duration)
			                               .From(Vector3.zero)
			                               .SetEase(Ease.OutElastic, 2.0f, 1.0f)
			                               .SetLink(gameObject, LinkBehaviour.KillOnDestroy)
			                               .ToUniTask(cancellationToken: cancellationToken);

			tasks.Add(scaleTask);

			foreach (ShapeBlock block in _blocks)
				tasks.Add(block.FadeIn(duration).ToUniTask(cancellationToken: cancellationToken));

			await UniTask.WhenAll(tasks);
		}

		public async UniTask LandAsync (Vector3 position, CancellationToken cancellationToken = default) {
			_transformTween?.Kill();
			_pivotSequence?.Kill();

			HashSet <UniTask> tasks = new HashSet <UniTask>();

			UniTask pivotMoveTask = _pivot.DOLocalMoveY(0, _animationDuration)
			                              .SetLink(gameObject, LinkBehaviour.KillOnDestroy)
			                              .WithCancellation(cancellationToken);

			UniTask transformMoveTask = transform.DOMove(position, _animationDuration)
			                                     .SetLink(gameObject, LinkBehaviour.KillOnDestroy)
			                                     .WithCancellation(cancellationToken);

			tasks.Add(pivotMoveTask);
			tasks.Add(transformMoveTask);

			foreach (ShapeBlock block in _blocks)
				tasks.Add(block.RestoreSize(_animationDuration).WithCancellation(cancellationToken));

			AudioManager.PlayOnce(_placeSfx);

			await UniTask.WhenAll(tasks);

			Destroy(gameObject);
		}

		public async UniTask PutBackAsync (CancellationToken cancellationToken = default) {
			_canScan = false;

			_transformTween?.Kill();
			_pivotSequence?.Kill();

			await DOTween.Sequence()
			             .OnStart(() => _blocks.ForEach(_ => _.RestoreSize(_animationDuration)))
			             .Append(transform.DOMove(_initialPosition, _animationDuration))
			             .Join(_pivot.DOLocalMoveY(0, _animationDuration))
			             .Join(_pivot.DOScale(_initialScale, _animationDuration))
			             .OnComplete(() => {
				             IsInteractable             = true;
				             _sortingGroup.sortingOrder = 0;

				             PutBack?.Invoke(this);
			             })
			             .SetLink(gameObject, LinkBehaviour.KillOnDestroy)
			             .WithCancellation(cancellationToken);

			AudioManager.PlayOnce(_putBackSfx);
		}

		public void OnBeginDrag (PointerEventData eventData) {
			if (IsInteractable == false)
				return;

			_isDragged                 = true;
			IsInteractable             = false;
			_sortingGroup.sortingOrder = short.MaxValue;

			Grabbed?.Invoke(this);

			_pivotSequence.Kill();
			_pivotSequence = DOTween.Sequence()
			                        .Join(_pivot.DOScale(Vector3.one, _animationDuration))
			                        .Join(_pivot.DOLocalMoveY((float)Height / 2 + _dragYOffset, _animationDuration))
			                        .OnComplete(() => _canScan = true)
			                        .SetLink(gameObject, LinkBehaviour.KillOnDestroy);

			foreach (ShapeBlock block in _blocks)
				block.Shrink(_animationDuration);

			AudioManager.PlayOnce(_pickUpSfx);
		}

		public void OnDrag (PointerEventData eventData) {
			Vector3 position = _camera.ScreenToWorldPoint(Input.mousePosition);
			_targetPosition = new Vector3(position.x, position.y, transform.position.z);
		}

		public void OnEndDrag (PointerEventData eventData) {
			_canScan   = false;
			_isDragged = false;

			Released?.Invoke(this);
		}

		private void CalculateCoords (ShapePattern pattern) {
			Coord offset = pattern.Min();

			foreach (Coord coord in pattern.Coords)
				_coords.Add(coord - offset);
		}

		private void CalculateSize () {
			int maxX = int.MinValue;
			int maxY = int.MinValue;

			foreach (Coord coord in _coords) {
				maxX = Mathf.Max(maxX, coord.X);
				maxY = Mathf.Max(maxY, coord.Y);
			}

			Width  = maxX + 1;
			Height = maxY + 1;
		}

		private void SpawnBlocks () {
			foreach (Coord coord in _coords) {
				ShapeBlock block    = Instantiate(_shapeBlockPrototype, _blockGroup);
				Vector3    position = new Vector3(coord.X, coord.Y);

				position.x -= (Width - 1) / 2.0f;
				position.y -= (Height - 1) / 2.0f;

				block.transform.localPosition = position;
				_blocks.Add(block);
			}
		}

		private void UpdatePosition () {
			Vector3 screenPoint = _camera.WorldToScreenPoint(_targetPosition);

			if (SafeAreaUtilities.Contains(screenPoint) == false) {
				OnEndDrag(null);
				PutBackAsync().Forget();
				return;
			}

			Vector3 position = Vector3.Lerp(transform.position, _targetPosition, 25f * Time.deltaTime);

			transform.position = position;
		}

		private void OnApplicationFocus (bool hasFocus) {
			if (hasFocus)
				return;

			if (_isDragged == false)
				return;

			OnEndDrag(null);
			PutBackAsync().Forget();
		}

		public void TurnOn () {
			IsInteractable = true;

			foreach (ShapeBlock block in _blocks)
				block.SetDefault();
		}

		public void TurnOff () {
			IsInteractable = false;

			foreach (ShapeBlock block in _blocks)
				block.SetGrayscale();
		}
	}
}
