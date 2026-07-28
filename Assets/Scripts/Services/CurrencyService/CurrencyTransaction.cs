using System;
using Data;

namespace Services.CurrencyService
{
    [Serializable]
    public struct CurrencyTransaction
    {
        public CurrencyType type;
        public int amount;
    }
}