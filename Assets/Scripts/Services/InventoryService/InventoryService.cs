using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Data;
using Services.LogService;
using Services.PrivateModelProvider;

namespace Services.InventoryService
{
    public class InventoryService : IInventoryService
    {
        private readonly ILogService logService;
        private readonly IPrivateModelProvider privateModelProvider;

        private InventoryPrivateModel model;
        
        public event Action Changed;

        public InventoryService(IPrivateModelProvider privateModelProvider, ILogService logService)
        {
            this.privateModelProvider = privateModelProvider;
            this.logService = logService;
        }

        public void Initialize() =>
            model = privateModelProvider.GetModel<InventoryPrivateModel>();

        public IReadOnlyList<ItemPrivateScheme> GetAllItems() => 
            model.Schemes;

        public int GetAmount(string itemId)
        {
            if (string.IsNullOrEmpty(itemId))
            {
                logService.LogError("InventoryService: ItemId can't be empty", LogCategory.Service);
                return 0;
            }
            
            return model.GetScheme(itemId).Amount;
        }

        public void Add(string itemId, int amount)
        {
            if (string.IsNullOrEmpty(itemId))
            {
                logService.LogError("InventoryService: ItemId can't be empty", LogCategory.Service);
                return;
            }
            
            if (amount <= 0)
            {
                logService.LogError("InventoryService: Amount can be less or equal then 0 ", LogCategory.Service);
                return;
            }
            
            model.GetScheme(itemId).IncreaseAmount(amount);
            SaveAndNotify();
        }

        public bool TryRemove(string itemId, int amount)
        {
            
            if (string.IsNullOrEmpty(itemId))
            {
                logService.LogError("InventoryService: ItemId can't be empty ", LogCategory.Service);
                return false;
            }
            
            if (amount <= 0)
            {
                logService.LogError("InventoryService: Amount can be less or equal then 0", LogCategory.Service);
                return false;
            }

            ItemPrivateScheme item = model.GetScheme(itemId);

            if (item.Amount < amount)
                return false;

            item.DecreaseAmount(amount);
            
            SaveAndNotify();
            return true;
        }

        private void SaveAndNotify()
        {
            privateModelProvider.SaveModel<InventoryPrivateModel>().Forget();
            Changed?.Invoke();
        }
    }
}
