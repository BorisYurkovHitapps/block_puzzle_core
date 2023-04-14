using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;


namespace BlockPuzzle.Scripts.Runtime.gameplay {
	[UsedImplicitly]
	public class ShapePatternProvider : IInitializable {
		private ShapePatternPack _shapePatternPack;

		private readonly List <int> _shapeWeights = new List <int>();


		[Inject]
		private void SetDependencies (ShapePatternPack shapePatternPack) {
			_shapePatternPack = shapePatternPack;
		}

		public void Initialize () {
			_shapeWeights.Clear();

			for (int i = 0; i < _shapePatternPack.Count; i++) {
				ShapePattern pattern = _shapePatternPack.GetShapePatternAt(i);
				_shapeWeights.AddRange(Enumerable.Repeat(i, pattern.Weight));
			}
		}

		public ShapePattern GetRandomShapePattern () {
			if (_shapeWeights.Count == 0)
				Debug.LogError("No Shapes Found!");

			int index = _shapeWeights[Random.Range(0, _shapeWeights.Count)];

			return _shapePatternPack.GetShapePatternAt(index);
		}
	}
}
