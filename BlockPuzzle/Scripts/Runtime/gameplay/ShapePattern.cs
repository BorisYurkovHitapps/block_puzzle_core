using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;


namespace BlockPuzzle.Scripts.Runtime.gameplay {
	[Serializable]
	public class ShapePattern {
		[SerializeField]
		[JsonProperty("coords")] private List <Coord> _coords;

		[SerializeField]
		[Range(0, 1000)]
		[JsonProperty("weight")] private int _weight = 100;

		[JsonIgnore] public IList <Coord> Coords => _coords;
		[JsonIgnore] public int           Weight => _weight;


		public ShapePattern () {
			_coords = new List <Coord>();
		}

		public ShapePattern (IEnumerable <Coord> coords) {
			_coords = new List <Coord>(coords);
		}

		public Coord Min () {
			int minX = int.MaxValue;
			int minY = int.MaxValue;

			foreach (Coord coord in _coords) {
				minX = Mathf.Min(minX, coord.X);
				minY = Mathf.Min(minY, coord.Y);
			}

			return new Coord(minX, minY);
		}

		public bool Contains (Coord coord) {
			return _coords.Contains(coord);
		}

		public void Add (Coord coord) {
			_coords.Add(coord);
		}

		public void Remove (Coord coord) {
			_coords.Remove(coord);
		}
	}
}
