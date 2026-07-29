using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI.Inventory
{
    public class InventoryElement : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI amountText;

        private string itemId;
        private int amount;

        public string ItemId => itemId;
        public int Amount => amount;

        public event Action<string> Clicked;

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

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                Clicked?.Invoke(itemId);
        }

        private void UpdateText() => 
            amountText.text = $"x{amount}";
    }
}
