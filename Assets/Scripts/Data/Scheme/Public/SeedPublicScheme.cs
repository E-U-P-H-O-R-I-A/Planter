using System;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class SeedPublicScheme : PublicScheme
    {
        [SerializeField] private string name;
        
        [SerializeField] private Sprite icon;
        
        [SerializeField] private int price;
        [SerializeField] private string plantID;

        public override string ID => name;

        public int Price => price;
        public string PlantID => plantID;
        
        public Sprite Icon => icon;
    }
}