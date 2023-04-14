using UnityEngine;


namespace BlockPuzzle.Scripts.Runtime.utilities {
	[ExecuteInEditMode]
	[RequireComponent(typeof(RectTransform))]
	public class RectDrawer : MonoBehaviour {
		#region Set in Inspector
		[SerializeField] private Color _color        = new Color(0.25f, 0.9f, 1f, 0.1f);
		[SerializeField] private bool  _whenSelected = true;
		#endregion Set in Inspector

		private RectTransform _rectTransform;


		private void Awake () {
			_rectTransform = (RectTransform)transform;
		}

		private void Start () {
			if (Application.isEditor == false)
				Destroy(this);
		}

		private void Draw () {
			Rect worldRect = GetWorldRect();

			Gizmos.color = _color;
			Gizmos.DrawWireCube(worldRect.center, worldRect.size);
			Gizmos.DrawCube(worldRect.center, worldRect.size);
		}

		private Vector3[] GetWorldCorners () {
			Vector3[] corners = new Vector3[4];
			_rectTransform.GetWorldCorners(corners);

			return corners;
		}

		private Rect GetWorldRect () {
			Vector3[] corners = GetWorldCorners();

			Bounds bounds = new Bounds(corners[0], Vector3.zero);
			bounds.Encapsulate(corners[2]);

			return new Rect(new Vector2(bounds.min.x, bounds.min.y), bounds.size);
		}

		private void OnDrawGizmos () {
			if (_whenSelected == false)
				Draw();
		}

		private void OnDrawGizmosSelected () {
			if (_whenSelected)
				Draw();
		}
	}
}
