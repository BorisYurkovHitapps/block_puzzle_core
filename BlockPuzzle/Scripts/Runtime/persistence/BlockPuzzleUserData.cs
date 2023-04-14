using System;
using System.IO;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;


namespace BlockPuzzle.Scripts.Runtime.persistence {
	public class BlockPuzzleUserData {
		[UsedImplicitly] public const  string PlayerPrefsKey = "block_puzzle_save_data";
		[UsedImplicitly] public static string FilePath => Path.Combine(Application.persistentDataPath, $"{PlayerPrefsKey}.json");

		public static event Action OnKeyAdded   = delegate {};
		public static event Action OnKeyRemoved = delegate {};


		[JsonProperty("tutorial_finished")]
		private bool _isTutorialFinished;


		[JsonProperty("keys")]
		private ulong _keys;

		[JsonProperty("best_score")]
		private ulong _bestScore;

		[JsonProperty("attempts_finished_total")]
		private ulong _attemptsFinishedTotal;

		[JsonProperty("attempts_finished_current")]
		private ulong _attemptsFinishedCurrent;


		private readonly IPersistenceHandler _persistenceHandler;


		[JsonIgnore] public ulong Keys                    => _keys;
		[JsonIgnore] public ulong BestScore               => _bestScore;
		[JsonIgnore] public ulong AttemptsFinishedTotal   => _attemptsFinishedTotal;
		[JsonIgnore] public ulong AttemptsFinishedCurrent => _attemptsFinishedCurrent;
		[JsonIgnore] public bool  IsTutorialFinished      => _isTutorialFinished;


		public BlockPuzzleUserData () {
			// TODO выбери нужный
			_persistenceHandler = new PlayerPrefsPersistenceHandler <BlockPuzzleUserData>(this, PlayerPrefsKey);
			// _persistenceHandler = new FilePersistenceHandler <BlockPuzzleUserData>(this, FilePath);

			if (_persistenceHandler.TryLoad() == false)
				Debug.Log($"{nameof(BlockPuzzleUserData)}: persistent data not found.");
		}

		public void Reset () {
			_isTutorialFinished      = default;
			_keys                    = default;
			_bestScore               = default;
			_attemptsFinishedTotal   = default;
			_attemptsFinishedCurrent = default;

			_persistenceHandler.Save();
		}

		public void GrantKeys (ulong count) {
			_keys += count;

			_persistenceHandler.Save();

			Debug.Log("-bp grant keys");
			OnKeyAdded.Invoke();
		}

		public bool TryUseKeys (ulong count) {
			if (_keys < count)
				return false;

			_keys -= (uint)count;

			ResetCurrentFinishedAttempts();

			_persistenceHandler.Save();

			OnKeyRemoved.Invoke();

			return true;
		}

		public void IncrementFinishedAttempts () {
			_attemptsFinishedTotal++;
			_attemptsFinishedCurrent++;

			_persistenceHandler.Save();
		}

		public void ResetCurrentFinishedAttempts () {
			_attemptsFinishedCurrent = 0;

			_persistenceHandler.Save();
		}

		public bool TryUpdateBestScore (ulong earnedScore) {
			if (_bestScore >= earnedScore)
				return false;

			_bestScore = earnedScore;
			_persistenceHandler.Save();

			return true;
		}

		public void SetTutorialFinished () {
			_isTutorialFinished = true;
			
			_persistenceHandler.Save();
		}
	}
}
