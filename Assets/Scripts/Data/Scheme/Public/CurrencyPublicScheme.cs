using System;
using UnityEngine;

namespace Data
{
    public enum CurrencyType
    {
        Soft = 0,
        Hard = 1,
    }
    
    [Serializable]
    public class CurrencyPublicScheme : PublicScheme
    {
        [SerializeField] private CurrencyType type;
        [SerializeField] private Sprite sprite;
        [SerializeField, Min(0)] private int startValue;
        
        public CurrencyType Type => type;
        public Sprite Sprite => sprite;
        public int StartValue => startValue;
        public override string ID => type.ToString();
    }
}
