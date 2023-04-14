using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace BlockPuzzle.Scripts.Runtime.ui {
	[RequireComponent(typeof(Button))]
	public class MainMenuButton : MonoBehaviour {
		private Button _button;

		private BlockPuzzleGameHandler _gameHandler;

		[Inject]
		[UsedImplicitly]
		private void SetDependencies (BlockPuzzleGameHandler gameHandler) {
			_gameHandler = gameHandler;
		}

		private void Awake () {
			_button = GetComponent <Button>();

			_button.onClick.AddListener(OnClick);
		}

		private void OnClick () {
			_gameHandler.GoToMainMenu();
		}
	}
}