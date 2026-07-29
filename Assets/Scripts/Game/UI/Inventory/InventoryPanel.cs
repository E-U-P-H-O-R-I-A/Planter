using System.Collections.Generic;
using System.Linq;
using Data;
using Game.Level;
using Services.InventoryService;
using Services.PublicModelProvider;
using UnityEngine;
using UnityEngine.EventSystems;
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
        private Camera gameplayCamera;
        private InventoryElement draggedElement;
        private Pot draggedPot;

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
            gameplayCamera = Camera.main;

            inventoryService.Changed += Refresh;

            Refresh();
        }

        public void Release()
        {
            inventoryService.Changed -= Refresh;

            foreach (var element in elements)
            {
                element.DragStarted -= StartDrag;
                element.Dragged -= Drag;
                element.DragEnded -= EndDrag;
            }

            draggedElement = null;
            draggedPot = null;
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
            element.DragStarted += StartDrag;
            element.Dragged += Drag;
            element.DragEnded += EndDrag;
            element.gameObject.SetActive(true);
            elements.Add(element);
        }

        private InventoryElement FindElement(string itemId) =>
            elements.FirstOrDefault(element => element.ItemId == itemId);

        private void UpdateElement(InventoryElement element, int amount)
        {
            if (element.Amount == amount)
                return;

            element.SetAmount(amount);
            element.gameObject.SetActive(amount > 0);
        }

        private void StartDrag(InventoryElement element, PointerEventData eventData)
        {
            draggedElement = element;
            UpdateDraggedPot(eventData.position);
        }

        private void Drag(InventoryElement element, PointerEventData eventData)
        {
            if (element == draggedElement)
                UpdateDraggedPot(eventData.position);
        }

        private void EndDrag(InventoryElement element, PointerEventData eventData)
        {
            if (element != draggedElement)
                return;

            var pot = FindPot(eventData.position);
            var seed = GetItemPublicScheme(element.ItemId);

            if (pot != null && seed != null && pot.IsEmpty)
            {
                if (inventoryService.TryRemove(element.ItemId, 1))
                {
                    if (!pot.Plant(seed.PlantID))
                        inventoryService.Add(element.ItemId, 1);
                }
            }

            draggedPot = null;
            draggedElement = null;
        }

        private void UpdateDraggedPot(Vector2 screenPosition)
        {
            var pot = FindPot(screenPosition);

            if (draggedPot == pot)
                return;

            draggedPot = pot;
        }

        private Pot FindPot(Vector2 screenPosition)
        {
            var camera = gameplayCamera != null ? gameplayCamera : Camera.main;
            if (camera == null)
                return null;

            var worldPosition = camera.ScreenToWorldPoint(screenPosition);
            foreach (var collider in Physics2D.OverlapPointAll(worldPosition))
            {
                var pot = collider.GetComponentInParent<Pot>();
                if (pot != null)
                    return pot;
            }

            return null;
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
