using JetBrains.Annotations;
using Services;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


namespace BlockPuzzle.Scripts.Runtime.ui {
	[RequireComponent(typeof(Button))]
	public class MainMenuButton : MonoBehaviour {
		private Button _button;

		private BlockPuzzleGameHandler _gameHandler;
		private IMainMediator          _mediator;


		[Inject]
		[UsedImplicitly]
		private void SetDependencies (BlockPuzzleGameHandler gameHandler, IMainMediator mediator) {
			_gameHandler = gameHandler;
			_mediator    = mediator;
		}

		private void Awake () {
			_button = GetComponent <Button>();

			_button.onClick.AddListener(OnClick);
		}

		private void OnClick () {
			_gameHandler.StopGame();
			_mediator.ShowMainMenu();
		}
	}
}
