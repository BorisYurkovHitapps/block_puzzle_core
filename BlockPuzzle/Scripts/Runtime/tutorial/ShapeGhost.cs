using DG.Tweening;
using UnityEngine;


// TODO TUTORIAL
namespace BlockPuzzle.Scripts.Runtime.tutorial {
	public class ShapeGhost : MonoBehaviour {
		#region Set in Inspector
		[SerializeField] private float _maxOpacity = 0.5f;
		[SerializeField] private float _fadeDuration = 0.5f;
		#endregion


		private Renderer[]            _renderers;
		private MaterialPropertyBlock _propertyBlock;

		private static readonly int Alpha = Shader.PropertyToID("_Alpha");

		private Tweener _tween;


		private void Awake () {
			_renderers     = GetComponentsInChildren <Renderer>();
			_propertyBlock = new MaterialPropertyBlock();

			UpdateOpacity(0);
		}

		private void UpdateOpacity (float value) {
			_propertyBlock.SetFloat(Alpha, value);

			foreach (Renderer r in _renderers)
				r.SetPropertyBlock(_propertyBlock);
		}

		public void FadeIn () {
			_tween?.Kill();
			_tween = DOTween.To(UpdateOpacity, 0, _maxOpacity, _fadeDuration)
			                 .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
		}

		public void FadeOut () {
			_tween?.Kill();
			DOTween.To(UpdateOpacity, _propertyBlock.GetFloat(Alpha), 0, _fadeDuration)
			       .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
		}
	}
}
