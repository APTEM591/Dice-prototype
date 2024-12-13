using DiceRoll.Entities;
using DiceRoll.Entities.Player;
using DiceRoll.Map;
using DiceRoll.Settings;
using TMPro;
using TriInspector;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;
using VContainer.Unity;

namespace Entities
{
    public class EntityScope : LifetimeScope
    {
        [FormerlySerializedAs("_numberPrefab")] [SerializeField] private TextMeshPro numberPrefab;
        
        [Title("Settings")]
        [SerializeField] private EntitySettings[] enemies;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<EntityFactory>(Lifetime.Singleton).WithParameter(numberPrefab);
            builder.RegisterFactory<EntitySettings,EntityStats>(container => container.Resolve<EntityFactory>().Create, Lifetime.Singleton);
            
            builder.RegisterComponentInHierarchy<PlayerStats>().As<IPlayerInfo>();
        }
    }
}