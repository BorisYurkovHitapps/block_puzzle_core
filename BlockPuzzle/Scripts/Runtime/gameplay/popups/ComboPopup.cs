using BlockPuzzle.Scripts.Runtime.gameplay.turnProcessing;


namespace BlockPuzzle.Scripts.Runtime.gameplay.popups {
	public class ComboPopup : BoardPopup {
		protected override void Start () {
			base.Start();

			TurnProcessor.ComboPerformed += Show;
		}

		private void OnDestroy () {
			TurnProcessor.ComboPerformed -= Show;
		}
	}
}
