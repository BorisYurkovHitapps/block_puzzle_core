// ReSharper disable UnusedMember.Global

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;


namespace BlockPuzzle.Scripts.Runtime.gameplay {
	[Serializable]
	public struct Coord : IEquatable <Coord> {
		public static readonly Coord Zero  = new Coord(0,  0);
		public static readonly Coord One   = new Coord(1,  1);
		public static readonly Coord Up    = new Coord(0,  1);
		public static readonly Coord Down  = new Coord(0,  -1);
		public static readonly Coord Left  = new Coord(-1, 0);
		public static readonly Coord Right = new Coord(1,  0);


		[SerializeField, JsonProperty("x")] private int _x;
		[SerializeField, JsonProperty("y")] private int _y;

		[JsonIgnore] public int X => _x;
		[JsonIgnore] public int Y => _y;

		public Coord (int x, int y) {
			_x = x;
			_y = y;
		}

		public Coord (Vector2Int vector) : this(vector.x, vector.y) {}

		public Coord (Vector3Int vector) : this(vector.x, vector.y) {}

		public static Coord operator + (Coord a, Coord b) {
			return new Coord(a._x + b._x, a._y + b._y);
		}

		public static Coord operator - (Coord a, Coord b) {
			return new Coord(a._x - b._x, a._y - b._y);
		}

		public static Vector2 operator * (Vector2 vector, Coord coord) {
			return new Vector2(vector.x * coord._x, vector.y * coord._y);
		}

		public static Vector2 operator * (Coord coord, Vector2 vector) {
			return new Vector2(coord._x * vector.x, coord._y * vector.y);
		}

		public bool IsInRange (int xMin, int yMin, int xMax, int yMax, bool includeMax = true) {
			bool xInRange = _x >= xMin && _x < (includeMax ? xMax + 1 : xMax);
			bool yInRange = _y >= yMin && _y < (includeMax ? yMax + 1 : yMax);

			return xInRange && yInRange;
		}

		public bool IsAdjacentTo (Coord other) {
			int deltaX = Math.Abs(_x - other._x);
			int deltaY = Math.Abs(_y - other._y);

			return (deltaX == 1 && deltaY == 0) || (deltaX == 0 && deltaY == 1);
		}

		public Vector2 ToVector2 () {
			return new Vector2(_x, _y);
		}

		public Vector2Int ToVector2Int () {
			return new Vector2Int(_x, _y);
		}

		public Vector3 ToVector3 () {
			return new Vector3(_x, _y);
		}

		public Vector3Int ToVector3Int () {
			return new Vector3Int(_x, _y, 0);
		}

		public Coord Offset (int x, int y) {
			return new Coord(_x + x, _y + y);
		}

		public IEnumerable <Coord> GetNeighbors (bool includeOrthogonal = false) {
			yield return this + Left;
			yield return this + Right;
			yield return this + Up;
			yield return this + Down;

			if (includeOrthogonal == false)
				yield break;

			yield return this + Left + Down;
			yield return this + Right + Down;
			yield return this + Left + Up;
			yield return this + Right + Up;
		}

		public IEnumerable <Coord> LineTo (Coord target) {
			int currentX = _x;
			int currentY = _y;

			int deltaX = Mathf.Abs(target._x - _x);
			int deltaY = Mathf.Abs(target._y - _y);

			int stepX = _x < target._x ? 1 : -1;
			int stepY = _y < target._y ? 1 : -1;

			int error = deltaX - deltaY;


			while (true) {
				yield return new Coord(currentX, currentY);

				if (currentX == target._x && currentY == target._y)
					yield break;

				int doubleError = error * 2;

				if (doubleError > -deltaY) {
					error    -= deltaY;
					currentX += stepX;
				}

				if (doubleError >= deltaX)
					continue;

				error    += deltaX;
				currentY += stepY;
			}
		}

		public int ManhattanDistanceTo (Coord other) {
			return Mathf.Abs(other._x - _x) + Mathf.Abs(other._y - _y);
		}

		public bool Equals (Coord other) {
			return _x == other._x && _y == other._y;
		}

		public override bool Equals (object obj) {
			return obj is Coord other && Equals(other);
		}

		public override string ToString () {
			return $"{_x} {_y}";
		}

		public override int GetHashCode () {
			unchecked {
				return (_x * 397) ^ _y;
			}
		}

		public static Coord Random (Coord min, Coord max, bool includeMax = true) {
			return Random(min._x, max._x, min._y, max._y, includeMax);
		}

		public static Coord Random (int xMin, int xMax, int yMin, int yMax, bool includeMax = true) {
			int x = UnityEngine.Random.Range(xMin, includeMax ? xMax + 1 : xMax);
			int y = UnityEngine.Random.Range(yMin, includeMax ? yMax + 1 : yMax);
			return new Coord(x, y);
		}
	}
}
