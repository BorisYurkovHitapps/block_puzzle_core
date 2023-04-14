using System.Linq;
using System.Text;
using BlockPuzzle.Scripts.Runtime.persistence;
using Flime.Core.Platform.Interfaces;
using JetBrains.Annotations;
using UnityEngine;


namespace BlockPuzzle.Scripts.Runtime.analytics {
	[UsedImplicitly]
	public class AnalyticsWrapper {
		private const string LevelType = "block_puzzle";

		private readonly BlockPuzzleUserData _userData;
		private readonly IAnalyticsManager   _analytics;


		public AnalyticsWrapper (BlockPuzzleUserData userData, IAnalyticsManager analytics) {
			_analytics = analytics;
			_userData  = userData;
		}

		public void game_start_level () {
			const string eventName = nameof(game_start_level);

			ulong keys = _userData.Keys;


			SendEvent(
				eventName,
				LevelType,
				keys,
				_userData.AttemptsFinishedTotal,
				_userData.AttemptsFinishedCurrent
			);
		}

		public void game_complete_level (ulong keysGained, ulong time) {
			const string eventName = nameof(game_complete_level);

			SendEvent(
				eventName,
				LevelType,
				keysGained,
				time,
				_userData.AttemptsFinishedTotal,
				_userData.AttemptsFinishedCurrent
			);
		}

		public void tutorial_complete_step (string stepLiteral) {
			const string eventName = nameof(tutorial_complete_step);

			SendEvent(
				eventName,
				LevelType,
				stepLiteral
			);
		}


		private static void SendEvent (string eventName, params object[] @params) {
			if (Application.isEditor || Debug.isDebugBuild)
				PrintMessage(eventName, @params);
		}

		private static void PrintMessage (string eventName, object[] @params) {
			StringBuilder message = new StringBuilder();

			message.Append($"<color=#99CC66>{eventName}");

			if (@params.Any()) {
				foreach (object param in @params) {
					message.Append('\n');
					message.Append(param);
				}
			}

			message.Append("</color>");

			Debug.Log(message);
		}
	}
}
