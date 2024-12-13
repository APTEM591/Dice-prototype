using System.Collections.Generic;
using System.Linq;
using TriInspector;
using UnityEngine;
using UnityEngine.Events;

namespace DiceRoll.Battles
{
    [System.Serializable]
    public enum StatType
    {
        MaxHP,
        HP,
        Regeneration,
        HealEfficiency,
        Damage,
        Armor,
        Strength,
        Agility,
        Wisdom,
        Treasures
    }

    [System.Serializable]
    [DeclareHorizontalGroup("horizontal")]
    public class Stat
    {
        public float Value { get; private set; }
        public float BaseValue => baseValue;

        public StatType Type => statType;

        /// <summary>
        /// Invokes when value of this stat changed. Return new value of stat and difference.
        /// </summary>
        public class ValueChangeEvent : UnityEvent<float, float> {}
        public ValueChangeEvent OnValueChange = new ();

        [HideLabel]
        [Group("horizontal")]
        [SerializeField] private StatType statType;

        [Group("horizontal")]
        [SerializeField] private float baseValue;

        private Dictionary<string, StatModifier> baseModifiers = new Dictionary<string, StatModifier>();

        private IStatsHolder statsObject;

        private float maxValue = float.MaxValue;

        public Stat(IStatsHolder refObject, float value, StatType type)
        {
            statType = type;
            baseValue = value;
            Value = baseValue;

            statsObject = refObject;
        }

        /// <summary>
        /// Change the base by a specific amount. Recommended to use AddModifier() instead.
        /// </summary>
        public void ChangeBase(float amount)
        {
            baseValue += amount;
            UpdateValue();
            statsObject.OnStatChange(statsObject.Stats.Values.ToArray());
        }

        public void AddModifier(string key, float value, ModifierType type)
        {
            var modifier = new StatModifier(value);
            baseModifiers.Add(key, modifier);

            UpdateValue();
        }

        public void RemoveModifier(string key)
        {
            baseModifiers.Remove(key);

            UpdateValue();
        }

        public void ClampValue(float maxVal) => maxValue = maxVal;

        private void UpdateValue()
        {
            var prevValue = Value;
            var newValue = baseValue + baseModifiers.Values.Sum(modifier => modifier.Value);

            Value = Mathf.Clamp(newValue, 0, maxValue);

            OnValueChange.Invoke(Value, newValue - prevValue);
            statsObject.OnStatChange(statsObject.Stats.Values.ToArray());
        }

    }

    public interface IStatsHolder
    {
        public Dictionary<StatType,Stat> Stats { get; }

        public void OnStatChange(Stat[] stats);
        public void ApplyStats(Stat[] stats);
    }

    public enum ModifierType
    {
        BaseAdditive,
        Multiplier,
        Additive
    }

    public class StatModifier
    {
        public float Value { get { return value; } }
        private float value;

        public StatModifier(float value)
        {
            this.value = value;
        }
    }
}

