using System;
using System.IO;
using BlockPuzzle.Scripts.Runtime.gameplay.board;
using BlockPuzzle.Scripts.Runtime.utilities;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using Zenject;


namespace BlockPuzzle.Scripts.Runtime.persistence {
	public class BlockPuzzleAttempt : ITickable {
		[UsedImplicitly] public const  string PlayerPrefsKey = "block_puzzle_attempt";
		[UsedImplicitly] public static string FilePath => Path.Combine(Application.persistentDataPath, $"{PlayerPrefsKey}.json");

		public event Action StoreRequested;


		[JsonProperty("time")]
		private double _time;

		[JsonProperty("board")]
		private BoardState _boardState;

		[JsonProperty("roster")]
		private RosterState _rosterState;

		[JsonProperty("score")]
		public ReactiveProperty <ulong> Score {get; private set;} = new ReactiveProperty <ulong>();


		private bool _isTrackingTime;

		private readonly IPersistenceHandler _persistenceHandler;

		[JsonIgnore] public ulong Time => (ulong)Math.Ceiling(_time);


		public BlockPuzzleAttempt () {
			// TODO выбери нужный
			_persistenceHandler = new PlayerPrefsPersistenceHandler <BlockPuzzleAttempt>(this, PlayerPrefsKey);
			// _persistenceHandler = new FilePersistenceHandler <BlockPuzzleAttemptData>(this, FilePath);

			if (_persistenceHandler.TryLoad() == false)
				Debug.Log($"{nameof(BlockPuzzleAttempt)}: persistent data not found.");
		}

		public void Store () {
			StoreRequested?.Invoke();

			_persistenceHandler.Save();

			// Debug.Log(_attemptData.ToJson());
			// GUIUtility.systemCopyBuffer = _attemptData.ToJson();
		}

		public void Reset () {
			_time           = default;
			_boardState     = default;
			_rosterState    = default;
			_isTrackingTime = default;

			Score.Value = default;

			_persistenceHandler.Save();
		}

		public void SetRosterState (RosterState rosterState) {
			_rosterState = rosterState;
		}

		public void SetBoardState (BoardState boardState) {
			_boardState = boardState;
		}

		public bool TryGetRosterState (out RosterState rosterState) {
			rosterState = _rosterState;
			return rosterState != null;
		}

		public bool TryGetBoardState (out BoardState boardState) {
			boardState = _boardState;
			return _boardState != null;
		}

		public void StartTimeTracking () {
			_isTrackingTime = true;
		}

		public void StopTimeTracking () {
			_isTrackingTime = false;
		}

		public void Tick () {
			if (_isTrackingTime == false)
				return;

			_time += UnityEngine.Time.deltaTime;
		}

		public bool IsExistsSavedGame () {
			return _boardState != null;
		}
	}
}
