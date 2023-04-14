using UnityEditor;
using UnityEngine;


namespace BlockPuzzle.Scripts.Editor {
	public static class BakePositionToAnchorPoints {
		[MenuItem("CONTEXT/RectTransform/Bake Position To Anchors")]
		private static void Do (MenuCommand command) {
			RectTransform rectTransform = (RectTransform)command.context;

			Canvas canvas = rectTransform.GetComponentInParent <Canvas>();

			if (canvas == null)
				return;

			Undo.RecordObject(rectTransform, "Bake Position To Anchors");

			RectTransform parent = rectTransform.parent.GetComponentInParent <RectTransform>();

			if (parent == null)
				return;

			Vector3 transformPoint = parent.InverseTransformPoint(rectTransform.position);

			Rect    parentRect = parent.rect;
			Vector2 anchorMin  = rectTransform.anchorMin;
			Vector2 anchorMax  = rectTransform.anchorMax;

			float anchorWidth  = anchorMax.x - anchorMin.x;
			float anchorHeight = anchorMax.y - anchorMin.y;

			float targetAnchorX = Mathf.InverseLerp(parentRect.x, parentRect.x + parentRect.width,  transformPoint.x);
			float targetAnchorY = Mathf.InverseLerp(parentRect.y, parentRect.y + parentRect.height, transformPoint.y);

			rectTransform.anchorMin = new Vector2(targetAnchorX - anchorWidth / 2, targetAnchorY - anchorHeight / 2);
			rectTransform.anchorMax = new Vector2(targetAnchorX + anchorWidth / 2, targetAnchorY + anchorHeight / 2);

			rectTransform.anchoredPosition = Vector2.zero;

			EditorUtility.SetDirty(rectTransform);
		}
	}
}
