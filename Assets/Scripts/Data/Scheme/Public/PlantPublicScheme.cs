using System;
using Services.RewardService;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class PlantPublicScheme : PublicScheme
    {
        [SerializeField] private string name;
        
        [SerializeField] private Sprite imagePlant;
        [SerializeField] private Sprite imageSprout;

        [SerializeField] private float durationGrow;
        [SerializeField] private RewardConfig sellReward;

        public override string ID => name;

        public float DurationGrow => durationGrow;

        public Sprite ImagePlant => imagePlant;
        public Sprite ImageSprout => imageSprout;
        public RewardConfig SellReward => sellReward;
    }
}