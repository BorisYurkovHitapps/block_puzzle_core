using System;
using BlockPuzzle.Scripts.Runtime.extensions;
using BlockPuzzle.Scripts.Runtime.utilities;
using Newtonsoft.Json;
using UnityEngine;


namespace BlockPuzzle.Scripts.Runtime.configs {
	[CreateAssetMenu(menuName = ProjectUtils.GameTitle + "/Block Puzzle Config")]
	[Serializable]
	public class BlockPuzzleConfig : ScriptableObject {
		#region Set in Inspector
		[SerializeField]
		[Min(1)]
		[JsonProperty("score_per_key")]
		private ulong _scorePerKey = 100;

		[SerializeField]
		[Min(1)]
		[JsonProperty("combo_score_multiplier")]
		private ulong _comboScoreMultiplier = 5;
		#endregion Set in Inspector


		[JsonIgnore] public ulong ScorePerKey          => _scorePerKey;
		[JsonIgnore] public ulong ComboScoreMultiplier => _comboScoreMultiplier;


		#if UNITY_EDITOR
		[ContextMenu("Copy to Clipboard")]
		public void CopyToClipboard () => GUIUtility.systemCopyBuffer = this.ToJson();
		#endif
	}
}
