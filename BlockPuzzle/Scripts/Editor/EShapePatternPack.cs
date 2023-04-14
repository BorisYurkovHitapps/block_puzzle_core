using BlockPuzzle.Scripts.Runtime.gameplay;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;


namespace BlockPuzzle.Scripts.Editor {
	[CustomEditor(typeof(ShapePatternPack))]
	public class EShapePatternPack : UnityEditor.Editor {
		private const int Size        = Shape.MaxSize;
		private const int CellSpacing = 4;
		private const int CellSize    = 16;

		private ShapePatternPack   _patternPack;
		private SerializedProperty _shapes;
		private ReorderableList    _stepList;


		private void OnEnable () {
			_patternPack = (ShapePatternPack)target;
			_shapes      = serializedObject.FindProperty("_shapes");

			CreateStepList();
		}

		private void CreateStepList () {
			_stepList = new ReorderableList(serializedObject, _shapes, true, false, true, true) {
				onAddCallback       = _ => OnAddShape(),
				onRemoveCallback    = OnShapeRemove,
				drawElementCallback = DrawElement,
				elementHeight       = CellSize * Size + CellSpacing * (Size - 1) + 16
			};
		}

		private void DrawElement (Rect rect, int index, bool active, bool focused) {
			ShapePattern pattern = _patternPack.GetShapePatternAt(index);

			for (int y = 0; y < Shape.MaxSize; y++) {
				for (int x = 0; x < Shape.MaxSize; x++) {
					Coord coord = new Coord(x, y);

					Rect position = new Rect(
						rect.x + (CellSize + CellSpacing) * coord.X + 8,
						rect.y + (CellSize + CellSpacing) * (Shape.MaxSize - coord.Y - 1) + 8,
						CellSize,
						CellSize
					);

					Texture2D texture = pattern.Contains(coord)
						? Texture2D.whiteTexture
						: Texture2D.grayTexture;

					GUI.DrawTexture(position, texture);

					HandleClick(position, index, coord);
				}
			}

			SerializedProperty element = _stepList.serializedProperty.GetArrayElementAtIndex(index);

			Rect propertyRect = new Rect(
				rect.x + CellSize * 5 + CellSpacing * 4 + 8 + 20,
				rect.y + 8,
				100,
				EditorGUIUtility.singleLineHeight);

			EditorGUIUtility.labelWidth = 50;

			EditorGUI.PropertyField(
				propertyRect,
				element.FindPropertyRelative("_weight"),
				new GUIContent("Weight"),
				false
			);
		}

		private void OnAddShape () {
			ShapePattern shapePattern = new ShapePattern();
			_patternPack.Add(shapePattern);
			EditorUtility.SetDirty(_patternPack);
		}

		private static void OnShapeRemove (ReorderableList list) {
			bool dialog = EditorUtility.DisplayDialog(
				"Warning!",
				"Remove Shape?",
				"Yes",
				"No"
			);

			if (dialog)
				ReorderableList.defaultBehaviours.DoRemoveButton(list);
		}

		public override void OnInspectorGUI () {
			serializedObject.Update();

			_stepList.DoLayoutList();

			if (serializedObject.ApplyModifiedProperties())
				EditorUtility.SetDirty(_patternPack);
		}

		private void HandleClick (Rect rect, int index, Coord coord) {
			if (Event.current.type != EventType.MouseDown)
				return;

			if (rect.Contains(Event.current.mousePosition) == false)
				return;

			ShapePattern pattern = _patternPack.GetShapePatternAt(index);

			if (pattern.Contains(coord)) {
				pattern.Remove(coord);
			} else {
				pattern.Add(coord);
			}

			GUI.changed = true;
			Event.current.Use();

			EditorUtility.SetDirty(_patternPack);
		}
	}
}
