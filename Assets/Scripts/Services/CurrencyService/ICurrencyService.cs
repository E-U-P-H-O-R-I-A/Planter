using System;
using Data;

namespace Services.CurrencyService
{
    public interface ICurrencyService
    {
        event Action<CurrencyType, int> Changed;

        void Initialize();
        
        int GetAmountCurrency(CurrencyType currencyType);
        void IncreaseCurrency(CurrencyTransaction transaction);
        bool IsEnoughCurrency(CurrencyTransaction transaction);
        bool DecreaseCurrency(CurrencyTransaction transaction);
    }
}
