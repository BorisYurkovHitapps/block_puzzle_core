// ReSharper disable InconsistentNaming

using System;
using System.Collections.Generic;
using BlockPuzzle.Scripts.Runtime.extensions;
using BlockPuzzle.Scripts.Runtime.utilities;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;


namespace BlockPuzzle.Scripts.Runtime.gameplay {
	[CreateAssetMenu(menuName = ProjectUtils.GameTitle + "/Shape Pack")]
	[Serializable]
	public class ShapePatternPack : ScriptableObject {
		[SerializeField]
		[JsonProperty("shapes")]
		private List <ShapePattern> _shapes = new List <ShapePattern>();

		[JsonIgnore] public int Count => _shapes.Count;


		public ShapePattern GetShapePatternAt (int index) {
			return _shapes[index];
		}

		public void Add (ShapePattern pattern) {
			_shapes.Add(pattern);
		}

		#if UNITY_EDITOR
		[ContextMenu("Copy to Clipboard")]
		public void CopyToClipboard () => GUIUtility.systemCopyBuffer = this.ToJson();
		#endif
	}
}
