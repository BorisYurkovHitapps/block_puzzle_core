using System.Collections.Generic;
using UnityEngine;


namespace BlockPuzzle.Scripts.Runtime.tutorial {
	public class MaskState {
		public readonly Color Color = new Color(0.36f, 0.36f, 0.36f);

		public List <Vector2Int> IgnoreCells;
	}
}
