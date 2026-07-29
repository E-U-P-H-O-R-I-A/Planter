using System.Collections.Generic;
using System.Linq;
using Data;
using Services.InventoryService;
using Services.PublicModelProvider;
using UnityEngine;
using VContainer;

namespace Game.UI.Inventory
{
    public class InventoryPanel : MonoBehaviour
    {
        [SerializeField] private Transform content;
        [SerializeField] private InventoryElement prefab;

        private IPublicModelProvider publicModelProvider;
        private IInventoryService inventoryService;
        private SeedPublicModel seedModel;

        private readonly List<InventoryElement> elements = new();

        [Inject]
        public void Construct(IInventoryService inventoryService, IPublicModelProvider publicModelProvider)
        {
            this.publicModelProvider = publicModelProvider;
            this.inventoryService = inventoryService;
        }

        public void Initialize()
        {
            seedModel = publicModelProvider.GetModel<SeedPublicModel>();

            inventoryService.Changed += Refresh;

            Refresh();
        }

        public void Release()
        {
            inventoryService.Changed -= Refresh;

            foreach (var element in elements)
                element.Clicked -= RemoveItem;
        }
        
        private void Refresh()
        {
            foreach (var item in inventoryService.GetAllItems())
            {
                InventoryElement element = FindElement(item.ID);
                
                if (element != null)
                {
                    UpdateElement(element, item.Amount);
                    continue;
                }

                if (item.Amount <= 0)
                    continue;

                CreateElement(item);
            }
        }

        private void CreateElement(ItemPrivateScheme item)
        {
            var publicItem = GetItemPublicScheme(item.ID);
            
            if (publicItem == null)
                return;

            var element = Instantiate(prefab, content);
            element.SetElement(item.ID, publicItem.Icon, item.Amount);
            element.Clicked += RemoveItem;
            element.gameObject.SetActive(true);
            elements.Add(element);
        }

        private void RemoveItem(string itemId) =>
            inventoryService.TryRemove(itemId, 1);

        private InventoryElement FindElement(string itemId) =>
            elements.FirstOrDefault(element => element.ItemId == itemId);

        private void UpdateElement(InventoryElement element, int amount)
        {
            if (element.Amount == amount)
                return;

            element.SetAmount(amount);
            element.gameObject.SetActive(amount > 0);
        }

        private SeedPublicScheme GetItemPublicScheme(string itemId)
        {
            SeedPublicScheme publicItem = seedModel.GetScheme(itemId);
            
            if (publicItem == null)
            {
                Debug.LogWarning($"Seed config with ID '{itemId}' was not found.", this);
                return null;
            }

            return publicItem;
        }
    }
}
