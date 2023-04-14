using BlockPuzzle.Scripts.Runtime.persistence;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


namespace BlockPuzzle.Scripts.Runtime.Integration {
	[RequireComponent(typeof(Button))]
	public class GrantKeysButton : MonoBehaviour {
		#region Set in Inspector
		[SerializeField] private ulong _count = 3;
		#endregion Set in Inspector


		private Button              _button;
		private BlockPuzzleUserData _userData;


		[Inject]
		[UsedImplicitly]
		private void SetDependencies (BlockPuzzleUserData userData) {
			_userData = userData;
		}

		private void Awake () {
			_button = GetComponent <Button>();

			_button.onClick.AddListener(OnClick);
		}

		private void OnClick () {
			_userData.GrantKeys(_count);
		}
	}
}
