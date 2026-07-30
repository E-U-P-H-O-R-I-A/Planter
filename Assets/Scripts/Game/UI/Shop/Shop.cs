using System.Linq;
using Cysharp.Threading.Tasks;
using Data;
using Services.CurrencyService;
using Services.InventoryService;
using Services.PublicModelProvider;
using Services.WindowsService.Windows;
using UnityEngine;
using VContainer;

namespace Game.UI.Shop
{
    public sealed class Shop : BaseWindow<BaseWindowParams>
    {
        [SerializeField] private Transform content;
        [SerializeField] private ShopElement shopElementPrefab;
        [SerializeField] private CloseElement closeElementPrefab;
        [Space]
        [SerializeField, Min(0)] private int lockedElementCount = 5;

        private IPublicModelProvider publicModelProvider;
        private IInventoryService inventoryService;
        private ICurrencyService currencyService;
        private SeedPublicModel seedModel;
        private PlantPublicModel plantModel;

        public override WindowType Type => WindowType.Shop;

        [Inject]
        public void Construct(IPublicModelProvider publicModelProvider, IInventoryService inventoryService,
            ICurrencyService currencyService)
        {
            this.publicModelProvider = publicModelProvider;
            this.inventoryService = inventoryService;
            this.currencyService = currencyService;
        }

        protected override UniTask OnBeforeOpen(BaseWindowParams payload)
        {
            if (seedModel == null && publicModelProvider != null)
                seedModel = publicModelProvider.GetModel<SeedPublicModel>();

            if (plantModel == null && publicModelProvider != null)
                plantModel = publicModelProvider.GetModel<PlantPublicModel>();

            RebuildContent();
            
            return UniTask.CompletedTask;
        }

        protected override UniTask OnAfterOpened(BaseWindowParams payload)
        {
            currencyService.Changed -= OnCurrencyChanged;
            currencyService.Changed += OnCurrencyChanged;
            RefreshPurchaseButtons();

            return UniTask.CompletedTask;
        }

        protected override UniTask OnAfterClosed()
        {
            currencyService.Changed -= OnCurrencyChanged;
            return UniTask.CompletedTask;
        }

        private void OnCurrencyChanged(CurrencyType type, int amount)
        {
            if (type == CurrencyType.Soft)
                RefreshPurchaseButtons();
        }

        private void RefreshPurchaseButtons()
        {
            if (content == null)
                return;

            foreach (ShopElement element in content.GetComponentsInChildren<ShopElement>())
                element.RefreshPurchaseButton();
        }

        private void RebuildContent()
        {
            if (content == null)
            {
                Debug.LogError($"[{name}] Shop content is not assigned.", this);
                return;
            }

            DisableExistingElements();

            int shopElementIndex = 0;

            if (seedModel?.Schemes != null)
            {
                foreach (var seed in seedModel.Schemes)
                {
                    if (seed == null)
                        continue;

                    ShopElement element = GetOrCreateShopElement(seed.ID);
                    if (element == null)
                        break;

                    PlantPublicScheme plant = plantModel?.GetScheme(seed.PlantID);
                    element.Initialize(seed, plant, inventoryService, currencyService);
                    element.gameObject.SetActive(true);
                    element.transform.SetSiblingIndex(shopElementIndex++);
                }
            }

            for (int index = 0; index < lockedElementCount; index++)
            {
                CloseElement element = GetOrCreateCloseElement(index);
                if (element == null)
                    break;

                element.gameObject.SetActive(true);
                element.transform.SetSiblingIndex(shopElementIndex + index);
            }
        }

        private void DisableExistingElements()
        {
            foreach (ShopElement element in content.GetComponentsInChildren<ShopElement>(true))
                element.gameObject.SetActive(false);

            foreach (CloseElement element in content.GetComponentsInChildren<CloseElement>(true))
                element.gameObject.SetActive(false);
        }

        private ShopElement GetOrCreateShopElement(string seedId)
        {
            ShopElement[] existingElements = content.GetComponentsInChildren<ShopElement>(true);
            ShopElement element = existingElements.FirstOrDefault(item => item.SeedId == seedId)
                                  ?? existingElements.FirstOrDefault(item => string.IsNullOrEmpty(item.SeedId));

            if (element != null)
                return element;

            if (shopElementPrefab == null)
            {
                Debug.LogError($"[{name}] Shop element prefab is not assigned.", this);
                return null;
            }

            return Instantiate(shopElementPrefab, content);
        }

        private CloseElement GetOrCreateCloseElement(int index)
        {
            CloseElement[] existingElements = content.GetComponentsInChildren<CloseElement>(true);

            if (index < existingElements.Length)
                return existingElements[index];

            if (closeElementPrefab == null)
            {
                Debug.LogError($"[{name}] Close element prefab is not assigned.", this);
                return null;
            }

            return Instantiate(closeElementPrefab, content);
        }

        private void OnDestroy()
        {
            if (currencyService != null)
                currencyService.Changed -= OnCurrencyChanged;
        }
    }
}
