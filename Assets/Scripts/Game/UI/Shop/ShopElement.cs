using System;
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
        [SerializeField] private TMP_Text growthTime;
        [Space]
        [SerializeField] private Button purchaseButton;
        [SerializeField] private Sprite availableButtonSprite;
        [SerializeField] private Sprite unavailableButtonSprite;

        private IInventoryService inventoryService;
        private ICurrencyService currencyService;
        private SeedPublicScheme seed;
        
        public string SeedId => seed?.ID;

        public void Initialize(SeedPublicScheme seed, PlantPublicScheme plant,
            IInventoryService inventoryService, ICurrencyService currencyService)
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
            if (growthTime != null)
                growthTime.text = plant == null ? "—" : FormatGrowthTime(plant.DurationGrow);

            RefreshPurchaseButton();
        }

        private static string FormatGrowthTime(float durationSeconds)
        {
            int totalSeconds = Mathf.Max(0, Mathf.CeilToInt(durationSeconds));
            TimeSpan duration = TimeSpan.FromSeconds(totalSeconds);

            if (duration.TotalHours >= 1)
                return duration.Minutes > 0
                    ? $"{(int)duration.TotalHours}h {duration.Minutes}min"
                    : $"{(int)duration.TotalHours}h";

            if (duration.TotalMinutes >= 1)
                return duration.Seconds > 0
                    ? $"{(int)duration.TotalMinutes}min {duration.Seconds}s"
                    : $"{(int)duration.TotalMinutes}min";

            return $"{duration.Seconds}s";
        }

        private void Purchase()
        {
            if (seed == null || inventoryService == null || currencyService == null)
                return;

            if (currencyService.DecreaseCurrency(GetPriceTransaction()))
                inventoryService.Add(seed.ID, 1);

            RefreshPurchaseButton();
        }

        public void RefreshPurchaseButton()
        {
            if (seed == null || currencyService == null || purchaseButton == null)
                return;

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
