using System;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class PlantPublicScheme : PublicScheme
    {
        [SerializeField] private string name;
        
        [SerializeField] private Sprite imagePlant;
        [SerializeField] private Sprite imageSprout;
        
        [SerializeField] private int costSell;
        [SerializeField] private float durationGrow;
        
        public override string ID => name;

        public int CostSell => costSell;
        public float DurationGrow => durationGrow;
        
        public Sprite ImagePlant => imagePlant;
        public Sprite ImageSprout => imageSprout;
    }
}