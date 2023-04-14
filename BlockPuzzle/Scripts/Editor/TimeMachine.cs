using UnityEditor;
using UnityEngine;


namespace BlockPuzzle.Scripts.Editor {
	public class TimeMachine : EditorWindow {
		private const string EditorTimeScaleKey = "editor_time_scale";

		private static Texture _windowIcon;
		private static Texture _plusIcon;
		private static Texture _minusIcon;

		private static float _timeScale    = 1.0f;
		private static float _minTimeScale = 0.1f;

		private const float Increment = 0.1f;

		private readonly Vector2Int _buttonSize = new Vector2Int(60, 40);


		[MenuItem("Window/Time Machine")]
		public static void ShowWindow () {
			GetWindow <TimeMachine>().Show();
		}

		private void OnEnable () {
			LoadIcons();
			titleContent = new GUIContent("Time Machine") { image = _windowIcon };
		}

		[InitializeOnEnterPlayMode]
		private static void SetInitialTimeScale () {
			_timeScale = EditorPrefs.GetFloat(EditorTimeScaleKey);
			SetTimeScale(_timeScale);
		}

		private void OnGUI () {
			GUILayout.BeginVertical();

			GUILayout.BeginHorizontal();

			if (GUILayout.Button(_minusIcon, GUILayout.Width(_buttonSize.x), GUILayout.Height(_buttonSize.y)))
				SetTimeScale(_timeScale - Increment);

			if (GUILayout.Button("1:1", GUILayout.Width(_buttonSize.x), GUILayout.Height(_buttonSize.y)))
				SetTimeScale(1);

			if (GUILayout.Button(_plusIcon, GUILayout.Width(_buttonSize.x), GUILayout.Height(_buttonSize.y)))
				SetTimeScale(_timeScale + Increment);

			GUILayout.EndHorizontal();

			EditorGUI.BeginChangeCheck();

			_timeScale = EditorGUILayout.Slider(_timeScale, _minTimeScale, 2.0f);

			if (EditorGUI.EndChangeCheck())
				SetTimeScale(_timeScale);

			GUILayout.EndVertical();
		}

		private static void SetTimeScale (float value) {
			_timeScale = value;
			_timeScale = (float)System.Math.Round(_timeScale, 1);
			_timeScale = Mathf.Clamp(_timeScale, _minTimeScale, 2);

			Time.timeScale = _timeScale;

			EditorPrefs.SetFloat(EditorTimeScaleKey, _timeScale);
		}

		private static void LoadIcons () {
			_windowIcon ??= EditorGUIUtility.Load("d_UnityEditor.AnimationWindow") as Texture;
			_plusIcon   ??= EditorGUIUtility.Load("d_Toolbar Plus@2x") as Texture;
			_minusIcon  ??= EditorGUIUtility.Load("d_Toolbar Minus@2x") as Texture;
		}
	}
}
