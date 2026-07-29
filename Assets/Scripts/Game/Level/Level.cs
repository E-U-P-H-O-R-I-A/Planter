using System.Collections.Generic;
using Game.UI.Inventory;
using Game.UI.Shop;
using Services.WindowsService;
using UnityEngine;
using VContainer;

namespace Game.Level
{
    public class Level : MonoBehaviour
    {
        [SerializeField] private InventoryPanel inventory;
        [SerializeField] private ShopButton shopButton;
        [Space]
        [SerializeField] private List<Pot> pots;
        
        public void Initialize()
        {
            InitializeUI();
            InitializePots();
        }

        public void Release()
        {
            ReleaseUI();
            ReleasePots();
        }

        private void ReleaseUI()
        {
            inventory.Release();
            shopButton.Release();
        }

        private void InitializeUI()
        {
            inventory.Initialize();
            shopButton.Initialize();
        }

        private void ReleasePots()
        {
            for (int index = 0; index < pots.Count; index++) 
                pots[index].Release();
        }

        private void InitializePots()
        {
            for (int index = 0; index < pots.Count; index++) 
                pots[index].Initialize(index.ToString());
        }
    }
}
