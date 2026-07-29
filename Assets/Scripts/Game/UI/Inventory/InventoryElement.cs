using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI.Inventory
{
    [RequireComponent(typeof(CanvasGroup))]
    public class InventoryElement : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI amountText;
        [Space] 
        [SerializeField] private CanvasGroup canvasGroup;

        private string itemId;
        private int amount;
        private bool isDragging;
        private Canvas dragCanvas;
        private RectTransform dragIcon;

        public string ItemId => itemId;
        public int Amount => amount;

        public event Action<InventoryElement, PointerEventData> DragStarted;
        public event Action<InventoryElement, PointerEventData> Dragged;
        public event Action<InventoryElement, PointerEventData> DragEnded;

        public void SetElement(string itemId, Sprite icon, int amount)
        {
            this.icon.sprite = icon;
            this.itemId = itemId;
            this.amount = amount;

            UpdateText();
        }

        public void SetAmount(int amount)
        {
            this.amount = amount;
            
            UpdateText();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (amount <= 0 || icon == null || icon.sprite == null)
                return;

            isDragging = true;
            canvasGroup.alpha = 0.35f;
            CreateDragIcon();
            MoveDragIcon(eventData);
            DragStarted?.Invoke(this, eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!isDragging)
                return;

            MoveDragIcon(eventData);
            Dragged?.Invoke(this, eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!isDragging)
                return;

            try
            {
                DragEnded?.Invoke(this, eventData);
            }
            finally
            {
                ResetDrag();
            }
        }

        private void CreateDragIcon()
        {
            dragCanvas = GetComponentInParent<Canvas>()?.rootCanvas;
            if (dragCanvas == null)
                return;

            var dragObject = new GameObject("Dragged Seed", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            dragIcon = dragObject.GetComponent<RectTransform>();
            dragIcon.SetParent(dragCanvas.transform, false);
            dragIcon.SetAsLastSibling();
            dragIcon.sizeDelta = icon.rectTransform.rect.size;

            var dragImage = dragObject.GetComponent<Image>();
            dragImage.sprite = icon.sprite;
            dragImage.preserveAspect = true;
            dragImage.raycastTarget = false;
            dragImage.color = new Color(1f, 1f, 1f, 0.9f);
        }

        private void MoveDragIcon(PointerEventData eventData)
        {
            if (dragIcon == null || dragCanvas == null)
                return;

            var canvasRect = (RectTransform)dragCanvas.transform;
            var eventCamera = dragCanvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : dragCanvas.worldCamera;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvasRect, eventData.position, eventCamera, out var localPoint))
            {
                dragIcon.anchoredPosition = localPoint;
            }
        }

        private void ResetDrag()
        {
            if (dragIcon != null)
                Destroy(dragIcon.gameObject);

            dragIcon = null;
            dragCanvas = null;
            isDragging = false;

            if (canvasGroup != null)
                canvasGroup.alpha = 1f;
        }

        private void UpdateText() => 
            amountText.text = $"x{amount}";
    }
}
