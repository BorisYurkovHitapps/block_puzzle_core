using System.Collections.Generic;
using System.Linq;


namespace BlockPuzzle.Scripts.Runtime.gameplay.turnProcessing {
	public sealed class ClearLinesResult {
		public           int             RowsAssembled    {get;}
		public           int             ColumnsAssembled {get;}
		private readonly HashSet <Coord> _rowsCoords;
		private readonly HashSet <Coord> _columnsCoords;

		public int LinesAssembled => RowsAssembled + ColumnsAssembled;

		public IReadOnlyCollection <Coord> RowsCoords    => _rowsCoords;
		public IReadOnlyCollection <Coord> ColumnsCoords => _columnsCoords;

		public IEnumerable <Coord> ClearedCoords => _rowsCoords.Union(_columnsCoords);

		public ClearLinesResult (int rowsAssembled, int columnsAssembled, HashSet <Coord> rowsCoords, HashSet <Coord> columnsCoords) {
			_columnsCoords   = columnsCoords;
			_rowsCoords      = rowsCoords;
			ColumnsAssembled = columnsAssembled;
			RowsAssembled    = rowsAssembled;
		}
	}
}
