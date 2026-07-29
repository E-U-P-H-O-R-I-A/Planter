using System;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class PotPrivateScheme : PrivateScheme
    {
        [SerializeField] private string potID;
        [SerializeField] private string plantID;
        [SerializeField] private long plantedUtcTicks;

        public override string ID => potID;
        public string PlantID => plantID;
        public bool IsPlanted => !string.IsNullOrEmpty(plantID);
        public DateTime PlantedTime => plantedUtcTicks > 0
            ? new DateTime(plantedUtcTicks, DateTimeKind.Utc)
            : DateTime.MinValue;

        public PotPrivateScheme(string potID)
        {
            this.potID = potID;
        }

        public void Plant(string plantID)
        {
            this.plantID = plantID;
            plantedUtcTicks = DateTime.UtcNow.Ticks;
        }

        public void Clear()
        {
            plantID = string.Empty;
            plantedUtcTicks = 0;
        }
    }
}
