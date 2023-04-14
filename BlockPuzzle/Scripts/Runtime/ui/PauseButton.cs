using UnityEngine;
using UnityEngine.UI;


namespace BlockPuzzle.Scripts.Runtime.ui {
	[RequireComponent(typeof(Button))]
	public class PauseButton : MonoBehaviour {
		[SerializeField] private PauseDialog _pauseDialog;

		private Button _button;


		private void Awake () {
			_button = GetComponent <Button>();
			_button.onClick.AddListener(ShowPauseDialog);
		}

		private void ShowPauseDialog () {
			_pauseDialog.gameObject.SetActive(true);
			_pauseDialog.Show();
		}
	}
}
