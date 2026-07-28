using System;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class ItemPrivateScheme : PrivateScheme
    {
        [SerializeField] private string itemID;
        [SerializeField] private int amount;

        public override string ID => itemID;
        public int Amount => amount;
        
        public ItemPrivateScheme(string itemID, int amount = 0)
        {
            this.itemID = itemID;
            this.amount = amount;
        }
        
        public void IncreaseAmount(int amount)
        {
            this.amount += amount;
        }

        public void DecreaseAmount(int amount)
        {
            this.amount -= amount;
        }
    }
}