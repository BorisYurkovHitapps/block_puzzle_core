using System;
using UnityEngine;


namespace BlockPuzzle.Scripts.Runtime.ui.safeArea {
	[DisallowMultipleComponent]
	[RequireComponent(typeof(RectTransform))]
	public class SafeAreaPosition : MonoBehaviour {
		[SerializeField] private ScreenEdge _edge = ScreenEdge.Top;

		private RectTransform _rectTransform;


		private void Awake () {
			_rectTransform = GetComponent <RectTransform>();
		}

		private void Start () {
			SetPosition();
		}

		private void SetPosition () {
			Vector2 position = _rectTransform.anchoredPosition;

			Vector2 top    = new Vector2(position.x,                                 position.y - SafeAreaUtilities.TopMargin);
			Vector2 bottom = new Vector2(position.x,                                 position.y + SafeAreaUtilities.BottomMargin);
			Vector2 left   = new Vector2(position.x + SafeAreaUtilities.LeftMargin,  position.y);
			Vector2 right  = new Vector2(position.x - SafeAreaUtilities.RightMargin, position.y);

			switch (_edge) {
				case ScreenEdge.Top:
					position = top;
					break;
				case ScreenEdge.Bottom:
					position = bottom;
					break;
				case ScreenEdge.Left:
					position = left;
					break;
				case ScreenEdge.Right:
					position = right;
					break;
				case ScreenEdge.TopLeft:
					position.x = left.x;
					position.y = top.y;
					break;
				case ScreenEdge.TopRight:
					position.x = right.x;
					position.y = top.y;
					break;
				case ScreenEdge.BottomLeft:
					position.x = left.x;
					position.y = bottom.y;
					break;
				case ScreenEdge.BottomRight:
					position.y = bottom.y;
					position.x = right.x;
					break;
				default:
					throw new NotSupportedException(nameof(_edge));
			}
			
			_rectTransform.anchoredPosition = position;
		}
	}
}
