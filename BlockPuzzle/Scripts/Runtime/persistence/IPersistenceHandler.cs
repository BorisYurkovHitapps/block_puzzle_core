namespace BlockPuzzle.Scripts.Runtime.persistence {
	public interface IPersistenceHandler {
		void Save ();
		bool TryLoad ();
	}
}
