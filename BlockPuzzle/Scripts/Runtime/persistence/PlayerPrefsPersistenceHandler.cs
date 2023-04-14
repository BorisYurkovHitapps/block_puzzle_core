using BlockPuzzle.Scripts.Runtime.extensions;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;


namespace BlockPuzzle.Scripts.Runtime.persistence {
	[UsedImplicitly]
	public class PlayerPrefsPersistenceHandler<T> : IPersistenceHandler {
		private readonly T      _target;
		private readonly string _key;


		public PlayerPrefsPersistenceHandler (T target, string key) {
			_target = target;
			_key    = key;
		}

		public void Save () {
			PlayerPrefs.SetString(_key, _target.ToJson());
		}

		public bool TryLoad () {
			if (PlayerPrefs.HasKey(_key) == false)
				return false;

			string json = PlayerPrefs.GetString(_key);

			JsonConvert.PopulateObject(json, _target);

			return true;
		}

		public override string ToString () {
			return _target.ToJson();
		}
	}
}
