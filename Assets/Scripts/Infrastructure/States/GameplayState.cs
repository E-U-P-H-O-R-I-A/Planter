using CodeBase.Infrastructure.AssetManagement;
using Cysharp.Threading.Tasks;
using Data;
using Game;
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
        private readonly IInventoryService inventoryService;
        private readonly IPublicModelProvider publicModelProvider;
        private readonly ISceneProvider sceneProvider;
        private readonly ILogService logService;
        
        private InventoryPanel inventoryPanel;

        public GameplayState(ILogService logService, ISceneProvider sceneProvider, ILoadingCurtain loadingCurtain,
            IInventoryService inventoryService, IPublicModelProvider publicModelProvider)
        {
            this.loadingCurtain = loadingCurtain;
            this.inventoryService = inventoryService;
            this.publicModelProvider = publicModelProvider;
            this.sceneProvider = sceneProvider;
            this.logService = logService;
        }

        public async UniTask Enter()
        {
            logService.Log("GamePlayState Enter", LogCategory.Infrastructure);
            
            var loadSceneTask = sceneProvider.Load(AssetsPath.GAMEPLAY_SCENE);
            await loadingCurtain.AnimatePhase(loadSceneTask, 0.90f);
            
            Resolve();
            
            AddStartingSeeds();
            inventoryPanel.Initialize();
            
            await loadingCurtain.Finish();

            loadingCurtain.Hide();
        }
        
        public async UniTask Exit()
        {
            inventoryPanel.Release();
            
            logService.Log("GamePlayState Exit", LogCategory.Infrastructure);
        }
        
        private void Resolve()
        {
            var gameplayScope = LifetimeScope.Find<GameplayLifeTimeScope>();
            
            inventoryPanel = gameplayScope.Container.Resolve<InventoryPanel>();
        }

        private void AddStartingSeeds()
        {
            var seedModel = publicModelProvider.GetModel<SeedPublicModel>();

            foreach (var seed in seedModel.Schemes)
                inventoryService.Add(seed.ID, 1);
        }
    }
}
