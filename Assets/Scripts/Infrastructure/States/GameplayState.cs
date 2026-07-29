using CodeBase.Infrastructure.AssetManagement;
using Cysharp.Threading.Tasks;
using Data;
using Game;
using Game.Level;
using Game.UI.Inventory;
using Services.InventoryService;
using Services.LogService;
using Services.PublicModelProvider;
using Services.SceneProvider;
using Utility.LoadingCurtain;
using Utility.StateMachine;
using VContainer;
using VContainer.Unity;

namespace Infrastructure.States
{
    public class GameplayState : IState
    {
        private readonly ILoadingCurtain loadingCurtain;
        private readonly ISceneProvider sceneProvider;
        private readonly ILogService logService;

        private Level level;

        public GameplayState(ILogService logService, ISceneProvider sceneProvider, ILoadingCurtain loadingCurtain)
        {
            this.loadingCurtain = loadingCurtain;
            this.sceneProvider = sceneProvider;
            this.logService = logService;
        }

        public async UniTask Enter()
        {
            logService.Log("GamePlayState Enter", LogCategory.Infrastructure);
            
            var loadSceneTask = sceneProvider.Load(AssetsPath.GAMEPLAY_SCENE);
            await loadingCurtain.AnimatePhase(loadSceneTask, 0.90f);
            
            Resolve();
            
            level.Initialize();

            await loadingCurtain.Finish();

            loadingCurtain.Hide();
        }
        
        public async UniTask Exit()
        {
            level.Release();
            
            logService.Log("GamePlayState Exit", LogCategory.Infrastructure);
        }
        
        private void Resolve()
        {
            var gameplayScope = LifetimeScope.Find<GameplayLifeTimeScope>();
            
            level = gameplayScope.Container.Resolve<Level>();
        }
    }
}
