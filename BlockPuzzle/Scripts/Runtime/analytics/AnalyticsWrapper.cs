using JetBrains.Annotations;

namespace BlockPuzzle.Scripts.Runtime.analytics 
{
	[UsedImplicitly]
	public class AnalyticsWrapper 
	{
		private readonly IAnalyticsManager _analytics;

		public AnalyticsWrapper (IAnalyticsManager analytics) 
		{
			_analytics = analytics;
		}

		public void game_start_level () 
		{
			_analytics.block_puzzle_game_start_level();
		}

		public void game_complete_level (ulong keysGained, ulong time) 
		{
			_analytics.block_puzzle_game_complete_level((int) keysGained, (int) time);
		}

		public void tutorial_complete_step (string stepLiteral) 
		{
			_analytics.block_puzzle_tutorial_complete_step(stepLiteral);
		}
	}
}