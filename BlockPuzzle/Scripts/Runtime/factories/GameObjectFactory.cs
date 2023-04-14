// ReSharper disable UnusedMember.Global

using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;


namespace BlockPuzzle.Scripts.Runtime.factories {
	[UsedImplicitly]
	public class GameObjectFactory {
		private readonly DiContainer _container;

		private readonly Dictionary <string, GameObject> _cachedAddressables;


		private GameObject this [string addressableKey] {
			get {
				if (_cachedAddressables.TryGetValue(addressableKey, out GameObject prototype))
					return prototype;

				prototype = LoadAsset <GameObject>(addressableKey);

				_cachedAddressables[addressableKey] = prototype;

				return prototype;
			}
		}

		public GameObjectFactory (DiContainer container) {
			_cachedAddressables = new Dictionary <string, GameObject>();
			_container          = container;
		}

		public GameObject Create (GameObject prototype) {
			return _container.InstantiatePrefab(prototype);
		}

		public GameObject Create (GameObject prototype, Transform parent) {
			return _container.InstantiatePrefab(prototype, parent);
		}

		public GameObject Create (GameObject prototype, Vector3 position, Quaternion rotation, Transform parent) {
			return _container.InstantiatePrefab(prototype, position, rotation, parent);
		}

		public TComponent Create<TComponent> (GameObject prototype) where TComponent : Component {
			return _container.InstantiatePrefabForComponent <TComponent>(prototype);
		}

		public TComponent Create<TComponent> (GameObject prototype, Transform parent) where TComponent : Component {
			return _container.InstantiatePrefabForComponent <TComponent>(prototype, parent);
		}

		public TComponent Create<TComponent> (GameObject prototype, Vector3 position, Quaternion rotation, Transform parent) where TComponent : Component {
			return _container.InstantiatePrefabForComponent <TComponent>(prototype, position, rotation, parent);
		}

		public TComponent Create<TComponent> (TComponent prototype) where TComponent : Component {
			return _container.InstantiatePrefabForComponent <TComponent>(prototype);
		}

		public TComponent Create<TComponent> (TComponent prototype, Transform parent) where TComponent : Component {
			return _container.InstantiatePrefabForComponent <TComponent>(prototype, parent);
		}

		public TComponent Create<TComponent> (TComponent prototype, Vector3 position, Quaternion rotation, Transform parent) where TComponent : Component {
			return _container.InstantiatePrefabForComponent <TComponent>(prototype, position, rotation, parent);
		}

		public GameObject Create (string addressableName) {
			return _container.InstantiatePrefab(this[addressableName]);
		}

		public GameObject Create (string addressableName, Transform parent) {
			return _container.InstantiatePrefab(this[addressableName], parent);
		}

		public GameObject Create (string addressableName, Vector3 position, Quaternion rotation, Transform parent) {
			return _container.InstantiatePrefab(this[addressableName], position, rotation, parent);
		}

		public TComponent Create<TComponent> (string addressableName) where TComponent : Component {
			GameObject prototype = this[addressableName];
			TComponent component = _container.InstantiatePrefabForComponent <TComponent>(prototype);
			component.gameObject.name = prototype.name;

			return component;
		}

		public TComponent Create<TComponent> (string addressableName, Transform parent) where TComponent : Component {
			GameObject prototype = this[addressableName];
			TComponent component = _container.InstantiatePrefabForComponent <TComponent>(prototype, parent);
			component.gameObject.name = prototype.name;

			return component;
		}

		public TComponent Create<TComponent> (string addressableName, Vector3 position, Quaternion rotation, Transform parent) where TComponent : Component {
			GameObject prototype = this[addressableName];
			TComponent component = _container.InstantiatePrefabForComponent <TComponent>(prototype, position, rotation, parent);
			component.gameObject.name = prototype.name;

			return component;
		}


		private static T LoadAsset<T> (string key) where T : Object {
			return UnityEngine.AddressableAssets.Addressables
			                  .LoadAssetAsync <T>(key)
			                  .WaitForCompletion();
		}
	}
}
