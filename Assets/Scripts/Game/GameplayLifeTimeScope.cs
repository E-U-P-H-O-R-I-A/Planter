using Infrastructure;
using Game.UI.Currency;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game
{
    public class GameplayLifeTimeScope : SceneLifetimeScope
    {
        [Space]
        [SerializeField] private Level.Level level;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(level);
            builder.RegisterComponentInHierarchy<CurrencyPanel>();
        }
    }
}
