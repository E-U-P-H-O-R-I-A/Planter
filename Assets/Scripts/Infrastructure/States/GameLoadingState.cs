using Cysharp.Threading.Tasks;
using Services.AssetProvider;
using Services.CurrencyService;
using Services.InputService;
using Services.LogService;
using Services.PrivateModelProvider;
using Services.PublicModelProvider;
using Services.WindowsService;
using Utility.LoadingCurtain;
using Utility.StateMachine;

namespace Infrastructure.States
{
    public class GameLoadingState : IState
    {
        private readonly IPrivateModelProvider privateModelProvider;
        private readonly IPublicModelProvider publicModelProvider;
        private readonly GameStateMachine gameStateMachine;
        private readonly ICurrencyService currencyService;
        private readonly IAssetsProvider assetsProvider;
        private readonly ILoadingCurtain loadingCurtain;
        private readonly IWindowService windowService;
        private readonly IInputService inputService;
        private readonly ILogService logService;

        public GameLoadingState(GameStateMachine gameStateMachine, ILogService logService, IPublicModelProvider publicModelProvider,
            IPrivateModelProvider privateModelProvider, ILoadingCurtain loadingCurtain, ICurrencyService currencyService, 
            IWindowService windowService, IAssetsProvider assetsProvider, IInputService inputService)
        {
            this.inputService = inputService;
            this.privateModelProvider = privateModelProvider;
            this.publicModelProvider = publicModelProvider;
            this.gameStateMachine = gameStateMachine;
            this.currencyService = currencyService;
            this.assetsProvider = assetsProvider;
            this.loadingCurtain = loadingCurtain;
            this.windowService = windowService;
            this.logService = logService;
        }
        
        public async UniTask Enter()
        {
            logService.Log("GameLoadingState Enter", LogCategory.Infrastructure);
            
            loadingCurtain.Show();
            
            var assetProviderTask = assetsProvider.Initialize();
            await loadingCurtain.AnimatePhase(assetProviderTask, 0.20f);
            
            var publicDataTask = publicModelProvider.Initialize();
            await loadingCurtain.AnimatePhase(publicDataTask, 0.50f);
            
            var privateDataTask = privateModelProvider.Initizele();
            await loadingCurtain.AnimatePhase(privateDataTask, 0.70f);
            
            currencyService.Initialize();
            windowService.Initialize();
            inputService.Initialize();

            gameStateMachine.Enter<GameplayState>();
        }

        public async UniTask Exit()
        {
            logService.Log("GameLoadingState Exit", LogCategory.Infrastructure);
        }
    }
}
