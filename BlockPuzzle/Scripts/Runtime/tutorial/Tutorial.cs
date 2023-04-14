using System;
using System.Threading;
using BlockPuzzle.Scripts.Runtime.factories;
using BlockPuzzle.Scripts.Runtime.persistence;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;


namespace BlockPuzzle.Scripts.Runtime.tutorial {
	public class Tutorial : MonoBehaviour {
		#region Set in Inspector
		[SerializeField] private Hand                   _hand;
		[SerializeField] private TutorialCompleteDialog _tutorialCompleteDialog;
		#endregion Set in Inspector


		public static event Action Started;
		public static event Action Ended;

		private GenericFactory      _factory;
		private BlockPuzzleUserData _userData;
		private BlockPuzzleAttempt  _attempt;


		[Inject]
		[UsedImplicitly]
		private void SetDependencies (
			BlockPuzzleUserData userData,
			BlockPuzzleAttempt attempt,
			GenericFactory genericFactory
		) {
			_userData = userData;
			_attempt  = attempt;
			_factory  = genericFactory;
		}

		private void Awake () {
			_hand.gameObject.SetActive(false);
		}

		public async UniTaskVoid StartSequenceAsync () {
			TutorialSequence  sequence          = _factory.Create <TutorialSequence>();
			CancellationToken cancellationToken = gameObject.GetCancellationTokenOnDestroy();

			Started?.Invoke();

			sequence.StepChanged += RestartHand;

			_hand.gameObject.SetActive(true);
			_hand.FadeIn();

			await sequence.StartAsync(cancellationToken);

			_hand.FadeOut();

			await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: cancellationToken);

			sequence.StepChanged -= RestartHand;
			_hand.gameObject.SetActive(false);

			_userData.SetTutorialFinished();
			_attempt.Reset();

			_tutorialCompleteDialog.Show();

			await UniTask.WaitUntil(
				() => _tutorialCompleteDialog.IsClosed,
				cancellationToken: cancellationToken
			);

			Ended?.Invoke();
		}

		private void RestartHand () {
			_hand.PlayAnimation();
		}
	}
}
