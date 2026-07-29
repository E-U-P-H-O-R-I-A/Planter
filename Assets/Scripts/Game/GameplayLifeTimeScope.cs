using Game.UI.Inventory;
using Infrastructure;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game
{
    public class GameplayLifeTimeScope : SceneLifetimeScope
    {
        [Space]
        [SerializeField] private InventoryPanel inventory;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(inventory);
        }
    }
}