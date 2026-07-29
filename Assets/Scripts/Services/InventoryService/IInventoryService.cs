using System;
using System.Collections.Generic;
using Data;

namespace Services.InventoryService
{
    public interface IInventoryService : IService
    {
        event Action Changed;


        void Initialize();
        int GetAmount(string itemId);
        void Add(string itemId, int amount);
        bool TryRemove(string itemId, int amount);
        IReadOnlyList<ItemPrivateScheme> GetAllItems();
    }
}
