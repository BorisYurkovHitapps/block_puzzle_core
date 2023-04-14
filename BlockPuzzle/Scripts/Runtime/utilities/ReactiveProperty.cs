using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;


namespace BlockPuzzle.Scripts.Runtime.utilities {
	[Serializable]
	public class ReactiveProperty<T> where T : struct {
		private event Action <T> Listeners;

		[JsonProperty("value")]
		private T _value;

		[JsonIgnore]
		public T Value {
			get => _value;
			set {
				bool equals = EqualityComparer <T>.Default.Equals(_value, value);

				if (equals)
					return;

				_value = value;

				Listeners?.Invoke(_value);
			}
		}


		public ReactiveProperty () {}

		public ReactiveProperty (T value) {
			_value = value;
		}

		public void Subscribe (Action <T> callback) {
			if (Listeners != null && Listeners.GetInvocationList().Contains(callback)) {
				Debug.LogWarning($"'{callback.Method.Name}' is already registered.");
				return;
			}

			Listeners += callback;
		}

		public void Unsubscribe (Action <T> callback) {
			if (Listeners == null || Listeners.GetInvocationList().Contains(callback) == false) {
				Debug.LogWarning($"'{callback.Method.Name}' is not registered.");
				return;
			}

			Listeners -= callback;
		}
	}
}
