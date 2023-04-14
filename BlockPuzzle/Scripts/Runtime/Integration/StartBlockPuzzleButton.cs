using JetBrains.Annotations;
using Services;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


namespace BlockPuzzle.Scripts.Runtime.Integration {
	[RequireComponent(typeof(Button))]
	public class StartBlockPuzzleButton : MonoBehaviour {
		private Button                 _button;
		private BlockPuzzleGameHandler _gameHandler;

		private IMainMediator _mainMediator;


		[Inject]
		[UsedImplicitly]
		private void SetDependencies (BlockPuzzleGameHandler gameHandler, IMainMediator mainMediator) {
			_mainMediator = mainMediator;
			_gameHandler  = gameHandler;
		}

		private void Awake () {
			_button = GetComponent <Button>();

			_button.onClick.AddListener(OnClick);
		}

		private void OnClick () {
			_mainMediator.CloseMainMenu();
			_gameHandler.StartGame();
		}
	}
}
