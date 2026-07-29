using System;
using System.Collections.Generic;

namespace Data
{
    public class CurrencyPrivateModel : PrivateModel.Collection<CurrencyPrivateScheme>
    {
        private readonly Dictionary<CurrencyType, int> startValues = new();

        public void SetStartValue(CurrencyType type, int value) =>
            startValues[type] = value;

        protected override CurrencyPrivateScheme CreateSchemeById(string id)
        {
            if (!Enum.TryParse(id, out CurrencyType type))
                return null;

            int startValue = startValues.GetValueOrDefault(type);
            return new CurrencyPrivateScheme(type, startValue);
        }
    }
}
