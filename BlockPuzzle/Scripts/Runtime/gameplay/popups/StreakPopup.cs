using BlockPuzzle.Scripts.Runtime.gameplay.turnProcessing;
using Zenject;


namespace BlockPuzzle.Scripts.Runtime.gameplay.popups {
	public class StreakPopup : BoardPopup {
		private SignalBus _signalBus;


		protected override void Start () {
			base.Start();

			TurnProcessor.StreakPerformed += Show;
		}

		private void OnDestroy () {
			TurnProcessor.StreakPerformed -= Show;
		}
	}
}
