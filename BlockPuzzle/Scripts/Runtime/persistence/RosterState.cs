using System;
using System.Collections.Generic;
using System.Linq;
using BlockPuzzle.Scripts.Runtime.gameplay;
using Newtonsoft.Json;


namespace BlockPuzzle.Scripts.Runtime.persistence {
	[Serializable]
	public class RosterState {
		[JsonProperty("shapes")]
		private List <List <Coord>> _shapes;


		[JsonConstructor]
		private RosterState () {}

		public RosterState (IEnumerable <Shape> shapes) {
			_shapes = new List <List <Coord>>();

			foreach (Shape shape in shapes) {
				if (shape == null) {
					_shapes.Add(null);
					continue;
				}

				_shapes.Add(shape.Coords.ToList());
			}
		}

		public RosterState (IEnumerable <ShapePattern> shapePatterns) {
			_shapes = new List <List <Coord>>();

			foreach (ShapePattern pattern in shapePatterns) {
				if (pattern == null) {
					_shapes.Add(null);
					continue;
				}

				_shapes.Add(pattern.Coords.ToList());
			}
		}

		public IList <ShapePattern> GetShapePatterns () {
			return _shapes.Select(coords => coords == null ? null : new ShapePattern(coords))
			              .ToList();
		}
	}
}
