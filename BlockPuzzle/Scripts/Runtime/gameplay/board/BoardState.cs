using System;
using System.Collections.Generic;
using System.Linq;
using BlockPuzzle.Scripts.Runtime.extensions;
using Newtonsoft.Json;


namespace BlockPuzzle.Scripts.Runtime.gameplay.board {
	[Serializable]
	public class BoardState {
		public const byte Vacant   = 0;
		public const byte Occupied = 1;


		[JsonProperty("width")]
		private readonly int _width;

		[JsonProperty("height")]
		private readonly int _height;

		private byte[] _state;


		[JsonProperty("state")]
		private byte[] CompressedState {
			get => _state.Compressed();
			set => _state = value.Decompressed();
		}

		[JsonConstructor]
		public BoardState (int width, int height) {
			_width  = width;
			_height = height;

			_state = new byte[width * height];
			_state.Fill(Vacant);
		}

		public BoardState (int width, int height, byte[] state) {
			_width  = width;
			_height = height;

			_state = state;
		}

		public byte this [Coord coord] {
			get => _state[GetIndex(coord.X, coord.Y)];
			set => _state[GetIndex(coord.X, coord.Y)] = value;
		}

		private int GetIndex (int x, int y) {
			return x + y * _width;
		}

		public bool Contains (Coord coord) {
			bool xInRange = coord.X >= 0 && coord.X < _width;
			bool yInRange = coord.Y >= 0 && coord.Y < _height;

			return xInRange && yInRange;
		}

		private bool CanPlace (Shape shape, int xOffset = 0, int yOffset = 0) {
			foreach (Coord coord in shape.Coords) {
				Coord coordWithOffset = coord.Offset(xOffset, yOffset);

				if (Contains(coordWithOffset) == false)
					return false;

				if (IsVacant(coordWithOffset) == false)
					return false;
			}

			return true;
		}

		public bool HasVacantPlaceFor (Shape shape) {
			for (int yOffset = 0; yOffset <= _height - shape.Height; yOffset++) {
				for (int xOffset = 0; xOffset <= _width - shape.Width; xOffset++) {
					if (CanPlace(shape, xOffset, yOffset))
						return true;
				}
			}

			return false;
		}

		public bool IsVacant (Coord coord) {
			return _state[GetIndex(coord.X, coord.Y)] == Vacant;
		}

		private bool IsOccupied (Coord coord) {
			return _state[GetIndex(coord.X, coord.Y)] == Occupied;
		}

		public bool AreOccupied (IEnumerable <Coord> coords) {
			return coords.All(IsOccupied);
		}

		public IEnumerable <Coord> GetRowCoordsAt (int index) {
			Coord start = new Coord(0,          index);
			Coord end   = new Coord(_width - 1, index);

			return start.LineTo(end);
		}

		public IEnumerable <Coord> GetColumnCoordsAt (int index) {
			Coord start = new Coord(index, 0);
			Coord end   = new Coord(index, _height - 1);

			return start.LineTo(end);
		}

		public IEnumerable <Coord> GetOccupiedCoords () {
			HashSet <Coord> result = new HashSet <Coord>();

			for (int y = 0; y < _height; y++) {
				for (int x = 0; x < _width; x++) {
					Coord coord = new Coord(x, y);

					if (IsOccupied(coord))
						result.Add(coord);
				}
			}

			return result;
		}

		public void OccupyCoords (IEnumerable <Coord> coords) {
			foreach (Coord coord in coords)
				this[coord] = Occupied;
		}

		public void ReleaseCoords (IEnumerable <Coord> coords) {
			foreach (Coord coord in coords)
				this[coord] = Vacant;
		}
	}
}
