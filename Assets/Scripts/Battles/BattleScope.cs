//using DiceRoll.Entities;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;
using VContainer.Unity;

namespace DiceRoll.Battles
{
    public class BattleScope : LifetimeScope
    {
        [SerializeField] private Transform markPrefab;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<StepProcessor>(Lifetime.Singleton);
            builder.Register<MarkHandler>(Lifetime.Transient).WithParameter(markPrefab);
        }
    }
}