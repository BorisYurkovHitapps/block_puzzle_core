// ReSharper disable UnusedMember.Global

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Zenject;


namespace BlockPuzzle.Scripts.Runtime.factories {
	[UsedImplicitly]
	public class GenericFactory {
		private readonly DiContainer _container;


		public GenericFactory (DiContainer container) {
			_container = container;
		}

		public T Create<T> () where T : class {
			return _container.Instantiate <T>();
		}

		public T Create<T> (IEnumerable <object> extraArgs) where T : class {
			return _container.Instantiate <T>(extraArgs);
		}

		public object Create<T> (T type) where T : Type {
			return _container.Instantiate(type);
		}

		public object Create<T> (T type, IEnumerable <object> extraArgs) where T : Type {
			return _container.Instantiate(type, extraArgs);
		}
	}
}
