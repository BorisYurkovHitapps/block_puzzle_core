using BlockPuzzle.Scripts.Runtime.ui;


namespace BlockPuzzle.Scripts.Runtime.tutorial {
	public class TutorialCompleteDialog : Dialog {
		public bool IsClosed {get; private set;}


		public void Hide () {
			base.Hide(() => IsClosed = true);
		}
	}
}
