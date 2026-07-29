using Data;
using Services.WindowsService;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Game.UI.Shop
{
    public sealed class ShopButton : MonoBehaviour
    {
        [SerializeField] private Button button;

        private IWindowService windowService;

        [Inject]
        public void Construct(IWindowService windowService) => 
            this.windowService = windowService;

        public void Initialize()
        {
            button.onClick.RemoveListener(OpenShop);
            button.onClick.AddListener(OpenShop);
        }

        public void Release() => 
            button.onClick.RemoveListener(OpenShop);

        private void OpenShop() =>
            windowService.OpenWindow(WindowType.Shop);
    }
}
