using Data;
using Services.CurrencyService;
using Services.InventoryService;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Shop
{
    public sealed class ShopElement : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text title;
        [SerializeField] private TMP_Text price;
        [Space]
        [SerializeField] private Button purchaseButton;
        [SerializeField] private Sprite availableButtonSprite;
        [SerializeField] private Sprite unavailableButtonSprite;

        private IInventoryService inventoryService;
        private ICurrencyService currencyService;
        private SeedPublicScheme seed;
        
        public string SeedId => seed.ID;

        public void Initialize(SeedPublicScheme seed, IInventoryService inventoryService, ICurrencyService currencyService)
        {
            if (seed == null)
                return;
            
            this.seed = seed;
            this.inventoryService = inventoryService;
            this.currencyService = currencyService;
            
            purchaseButton.onClick.RemoveListener(Purchase);
            purchaseButton.onClick.AddListener(Purchase);
            
            icon.sprite = seed.Icon;
            title.text = seed.Name;
            price.text = seed.Price.ToString();

            UpdatePurchaseButton();
        }

        private void Purchase()
        {
            if (seed == null || inventoryService == null || currencyService == null)
                return;

            if (currencyService.DecreaseCurrency(GetPriceTransaction()))
                inventoryService.Add(seed.ID, 1);

            UpdatePurchaseButton();
        }

        private void UpdatePurchaseButton()
        {
            bool canPurchase = currencyService.IsEnoughCurrency(GetPriceTransaction());

            purchaseButton.interactable = canPurchase;
            purchaseButton.image.sprite = canPurchase
                    ? availableButtonSprite
                    : unavailableButtonSprite;
        }

        private CurrencyTransaction GetPriceTransaction() => new()
        {
            type = CurrencyType.Soft,
            amount = seed.Price
        };
    }
}
