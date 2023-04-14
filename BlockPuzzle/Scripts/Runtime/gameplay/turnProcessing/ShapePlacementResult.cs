using System.Collections.Generic;


namespace BlockPuzzle.Scripts.Runtime.gameplay.turnProcessing {
	public sealed class ShapePlacementResult {
		public bool            IsPlaced       {get;}
		public HashSet <Coord> OccupiedCoords {get;}


		public ShapePlacementResult (bool isPlaced, HashSet <Coord> occupiedCoords) {
			OccupiedCoords = occupiedCoords;
			IsPlaced       = isPlaced;
		}
	}
}
