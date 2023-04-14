using DG.Tweening;
using UnityEngine;


namespace BlockPuzzle.Scripts.Runtime.gameplay {
	public class ShapeBlock : MonoBehaviour {
		#region Set in Inspector
		[SerializeField] private Transform      _graphicsTransform;
		[SerializeField] private SpriteRenderer _spriteRenderer;

		[Space]
		[SerializeField, Range(0, 1f)] private float _shrinkFactor;

		[Header("Sprites")]
		[SerializeField] private Sprite _defaultSprite;
		[SerializeField] private Sprite _grayscaleSprite;
		#endregion


		private Transform _transform;
		private Vector3   _shrinkSize;

		public Vector3 WorldPosition => _transform.position;


		private void Awake () {
			_transform  = GetComponent <Transform>();
			_shrinkSize = Vector3.one * _shrinkFactor;
		}

		public void Shrink (float duration) {
			_graphicsTransform.DOKill();

			_graphicsTransform
				.DOScale(_shrinkSize, duration)
				.SetLink(gameObject, LinkBehaviour.KillOnDestroy);
		}

		public Tween RestoreSize (float duration) {
			_graphicsTransform.DOKill();

			return _graphicsTransform
			       .DOScale(Vector3.one, duration)
			       .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
		}

		public Tween FadeIn (float duration) {
			_spriteRenderer.DOKill();
			return _spriteRenderer.DOFade(1, duration)
			                      .From(0)
			                      .SetEase(Ease.OutSine)
			                      .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
		}

		public void SetDefault () {
			_spriteRenderer.sprite = _defaultSprite;
		}

		public void SetGrayscale () {
			_spriteRenderer.sprite = _grayscaleSprite;
		}
	}
}
