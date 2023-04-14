using System;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;


namespace BlockPuzzle.Scripts.Runtime.services.androidBackButtonHandler {
	[UsedImplicitly]
	public class AndroidBackButtonHandler : ITickable {
		private Action _listener;
		private bool   _isTrackingSuspended;


		public void Track (Action callback) => _listener = callback;
		public void StopTracking () => _listener = null;

		public void Suspend () => _isTrackingSuspended = true;
		public void Resume () => _isTrackingSuspended = false;

		public void Tick () {
			if (_isTrackingSuspended || Input.GetKeyDown(KeyCode.Escape) == false)
				return;

			if (Application.isEditor)
				Debug.Log("<color=#00FFFF>Back Button pressed</color>");

			_listener?.Invoke();
		}
	}
}
