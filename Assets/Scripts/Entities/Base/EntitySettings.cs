using DiceRoll.Battles;
using DiceRoll.Entities;
using UnityEngine;

namespace DiceRoll.Settings
{
    [CreateAssetMenu(fileName = "EntitySettings", menuName = "DiceRoll/Settings/EntitySettings")]
    public class EntitySettings : ScriptableObject
    {
        [field: SerializeField] public EntityStats EntityPrefab { get; private set; }
        [field: SerializeField] public Stat[] Stats { get; private set; }
    }
}