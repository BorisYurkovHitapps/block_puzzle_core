using BlockPuzzle.Scripts.Runtime.analytics;
using BlockPuzzle.Scripts.Runtime.configs;
using BlockPuzzle.Scripts.Runtime.factories;
using BlockPuzzle.Scripts.Runtime.gameplay;
using BlockPuzzle.Scripts.Runtime.gameplay.board;
using BlockPuzzle.Scripts.Runtime.gameplay.turnProcessing;
using BlockPuzzle.Scripts.Runtime.persistence;
using UnityEngine;
using Zenject;


namespace BlockPuzzle.Scripts.Runtime.installers {
	public class BlockPuzzleInstaller : MonoInstaller {
		#region Set in Inspector
		[SerializeField] private BlockPuzzleConfig _blockPuzzleConfig;
		[SerializeField] private ShapePatternPack  _shapePatternPack;

		[Space]
		[SerializeField] private BlockPuzzleGameHandler _blockPuzzleGameHandler;
		#endregion Set in Inspector


		public override void InstallBindings () {
			InstallGameObjectFactory();
			InstallGenericFactory();

			InstallBlockPuzzleConfig();
			InstallShapePatternPack();
			InstallShapePatternProvider();

			InstallBlockPuzzleUserData();
			InstallBlockPuzzleAttempt();

			InstallAnalyticsWrapper();

			InstallBlockPuzzleGameHandler();

			InstallCamera();
			InstallBoard();
			InstallRoster();

			InstallTurnProcessor();
		}

		private void InstallAnalyticsWrapper () {
			Container.Bind <AnalyticsWrapper>()
			          .AsSingle();
		}

		private void InstallGameObjectFactory () {
			Container.Bind <GameObjectFactory>()
			          .AsSingle();
		}

		private void InstallGenericFactory () {
			Container.Bind <GenericFactory>()
			          .AsSingle();
		}

		private void InstallBlockPuzzleConfig () {
			Container.Bind <BlockPuzzleConfig>()
			          .FromInstance(_blockPuzzleConfig)
			          .AsSingle();
		}

		private void InstallShapePatternPack () {
			Container.BindInterfacesAndSelfTo <ShapePatternPack>()
			          .FromInstance(_shapePatternPack)
			          .AsSingle();
		}

		private void InstallShapePatternProvider () {
			Container.BindInterfacesAndSelfTo <ShapePatternProvider>()
			          .AsSingle();
		}

		private void InstallBlockPuzzleUserData () {
			Container.BindInterfacesAndSelfTo <BlockPuzzleUserData>()
			          .AsSingle()
			          .NonLazy();
		}

		private void InstallBlockPuzzleAttempt () {
			Container.BindInterfacesAndSelfTo <BlockPuzzleAttempt>()
			          .AsSingle()
			          .NonLazy();
		}

		private void InstallBlockPuzzleGameHandler () {
			Container.Bind <BlockPuzzleGameHandler>()
			          .FromComponentInNewPrefab(_blockPuzzleGameHandler)
			          .AsSingle();
		}

		private void InstallCamera () {
			BlockPuzzleGameHandler gameHandler = Container.Resolve <BlockPuzzleGameHandler>();

			Container.Bind <Camera>()
			          .FromInstance(gameHandler.Camera)
			          .AsSingle();
		}

		private void InstallBoard () {
			BlockPuzzleGameHandler gameHandler = Container.Resolve <BlockPuzzleGameHandler>();

			Container.Bind <Board>()
			          .FromInstance(gameHandler.Board)
			          .AsSingle();
		}

		private void InstallRoster () {
			BlockPuzzleGameHandler gameHandler = Container.Resolve <BlockPuzzleGameHandler>();

			Container.Bind <Roster>()
			          .FromInstance(gameHandler.Roster)
			          .AsSingle();
		}

		private void InstallTurnProcessor () {
			Container.BindInterfacesAndSelfTo <TurnProcessor>()
			          .AsSingle();
		}
	}
}
