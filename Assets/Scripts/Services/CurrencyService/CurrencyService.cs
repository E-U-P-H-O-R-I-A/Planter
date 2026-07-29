using System;
using Data;
using Services.PrivateModelProvider;
using Services.PublicModelProvider;

namespace Services.CurrencyService
{
    public class CurrencyService : ICurrencyService
    {
        private readonly IPrivateModelProvider privateModelProvider;
        private readonly IPublicModelProvider publicModelProvider;
        
        private CurrencyPrivateModel currencyPrivateModel;

        public event Action<CurrencyType, int> Changed;

        public CurrencyService(IPrivateModelProvider privateModelProvider, IPublicModelProvider publicModelProvider)
        {
            this.privateModelProvider = privateModelProvider;
            this.publicModelProvider = publicModelProvider;
        }

        public void Initialize()
        {
            currencyPrivateModel = privateModelProvider.GetModel<CurrencyPrivateModel>();

            CurrencyPublicModel publicModel = publicModelProvider.GetModel<CurrencyPublicModel>();
            if (publicModel?.Schemes == null)
                return;

            foreach (CurrencyPublicScheme scheme in publicModel.Schemes)
            {
                if (scheme != null)
                    currencyPrivateModel.SetStartValue(scheme.Type, scheme.StartValue);
            }
        }

        public int GetAmountCurrency(CurrencyType currencyType) => 
            GetScheme(currencyType).Value;

        public bool IsEnoughCurrency(CurrencyTransaction transaction) => 
            GetScheme(transaction.type).IsEnoughCurrency(transaction.amount);

        public void IncreaseCurrency(CurrencyTransaction transaction)
        {
            GetScheme(transaction.type).IncreaseCurrency(transaction.amount);
            Save();
            NotifyChanged(transaction.type);
        }

        public bool DecreaseCurrency(CurrencyTransaction transaction)
        {
            if (!IsEnoughCurrency(transaction))
                return false;
            
            GetScheme(transaction.type).DecreaseCurrency(transaction.amount);
            Save();
            NotifyChanged(transaction.type);

            return true;
        }

        private void Save() => 
            privateModelProvider.SaveModel<CurrencyPrivateModel>();

        private void NotifyChanged(CurrencyType currencyType) =>
            Changed?.Invoke(currencyType, GetAmountCurrency(currencyType));

        private CurrencyPrivateScheme GetScheme(CurrencyType currencyType) => 
            currencyPrivateModel.GetScheme(currencyType.ToString());
    }
}
