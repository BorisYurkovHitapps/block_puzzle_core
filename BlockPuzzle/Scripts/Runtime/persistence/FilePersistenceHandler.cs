using System.IO;
using BlockPuzzle.Scripts.Runtime.extensions;
using JetBrains.Annotations;
using Newtonsoft.Json;


namespace BlockPuzzle.Scripts.Runtime.persistence {
	[UsedImplicitly]
	public class FilePersistenceHandler<T> : IPersistenceHandler {
		private readonly T      _target;
		private readonly string _filePath;


		public FilePersistenceHandler (T target, string filePath) {
			_filePath = filePath;
			_target   = target;
		}

		public void Save () {
			File.WriteAllText(_filePath, _target.ToJson());
		}

		public bool TryLoad () {
			if (File.Exists(_filePath) == false)
				return false;

			string json = File.ReadAllText(_filePath);

			JsonConvert.PopulateObject(json, _target);

			return true;
		}

		public override string ToString () {
			return _target.ToJson();
		}
	}
}
