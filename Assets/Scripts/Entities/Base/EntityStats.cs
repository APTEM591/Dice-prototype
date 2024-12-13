using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DiceRoll.Battles;
using Entities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using VContainer;
using Random = UnityEngine.Random;

namespace DiceRoll.Entities
{
    public class EntityStats : MonoBehaviour, IStatsHolder, IDamageable//, IBuffable
    {
        public int Number { get; set; } = 0;
        public Vector2Int MapPosition { get; internal set; }
        
        public bool IsAlive => Stats[StatType.HP].Value > 0;
        
        /// <summary>
        /// Player stats
        /// </summary>
        public Dictionary<StatType, Stat> Stats { get; private set; } = new ();

        /// <summary>
        /// Buffs applied to player
        /// </summary>
        //public Dictionary<Buff, BuffTimer> buffs { get; } = new Dictionary<Buff, BuffTimer>();
        //[SerializeField] private BuffList buffList;

        //[SerializeField] private TreasuresUI treasuresUI;

        //[SerializeField] private HealingFlask flask;
        
        //public Ability[] Abilities => abilities;
        //[SerializeField] private Ability[] abilities;

        [Serializable]
        public class StatChangedEvent : UnityEvent<Stat[]> { }
        public StatChangedEvent _OnStatChange = new StatChangedEvent();

        /// <summary>
        /// Treasures owned by player
        /// </summary>
       // private List<Treasure> treasures = new List<Treasure>();

        [Inject]
        private void Construct()
        {
            OnStatChange(Stats.Values.ToArray());

            foreach (var module in GetComponents<IEntityModule>())
            {
                Debug.Log(module);
                module.Stats = this;
            }
        }

        public void ApplyStats(Stat[] stats)
        {
            foreach (var stat in stats)
                Stats.Add(stat.Type, new Stat(this, stat.BaseValue, stat.Type));

            Stats[StatType.MaxHP].OnValueChange.AddListener((val,_)=>Stats[StatType.HP].ClampValue(val));
            Stats[StatType.MaxHP].OnValueChange.Invoke(Stats[StatType.HP].BaseValue, 0);
        }

        public void OnStatChange(Stat[] newStats)
        {
            _OnStatChange.Invoke(newStats);
        }

        // public void AddTreasure(Treasure treasure)
        // {
        //     treasures.Add(treasure);
        //     treasuresUI.UpdateList(treasures.ToArray());
        // }

        //---------Damaging--------- 
        void IDamageable.DoDamage(IDamageable target)
        {
            var damage = Stats[StatType.Damage].Value;
            
            target.TakeDamage(damage, this);
        }

        void IDamageable.TakeDamage(float damage, IDamageable source)
        {
            //damage = Mathf.Clamp(damage - Stats[StatType.Armor].Value, 0, float.MaxValue);

            //Using flask if needed
            // var remainingHP = Stats[StatType.HP].BaseValue - damage;
            // if (remainingHP <= 0)
            // {
            //     flask.TryRevive(this);
            // }
            // else if (remainingHP < Stats[StatType.MaxHP].Value / 2f)
            // {
            //     float healEfficiency = Stats[StatType.HealEfficiency].Value;
            //     flask.TryUseFlask(this, healEfficiency);
            // }
            

            Stats[StatType.HP].ChangeBase(-damage);
            if(!IsAlive)
                Destroy(gameObject);
            
            EmitDamageEffect((int)damage);
            
            Debug.Log($"{gameObject.name} taken {damage} damage by {((MonoBehaviour)source).gameObject.name}");
        }

        private void EmitDamageEffect(int damage)
        {
            //Create a text object with damage value which jumps with DOTween animation and disappears
            
            var go = new GameObject("DamageText")
            {
                transform =
                {
                    position = transform.position,
                    rotation = Quaternion.Euler(90, 0, 0)
                }
            };
            var text = go.AddComponent<TextMeshPro>();
            text.text = damage.ToString();
            text.fontSize = 4;
            text.sortingOrder = 2;
            text.color = Color.red;
            text.alignment = TextAlignmentOptions.Center;
            
            //Jump to random direction
            go.transform.DOJump(go.transform.position 
                                + new Vector3(Random.Range(-1f, 1f), 1, Random.Range(-1f, 1f)), 0.2f, 1, 0.5f)
                .OnComplete(() => Destroy(go));
        }

        //---------Controlling buffs--------- 
        // public void UpdateBuffTimer()
        // {
        //     foreach (var buff in buffs.Values.ToList())
        //     {
        //         buff.DoTick();
        //         if (buff.IsGone)
        //             buffs.Remove(buff.Buff);
        //     }
        //
        //     UpdateBuffUI();
        // }
        //
        // public void AddBuff(BuffTimer buff)
        // {
        //     if(buffs.ContainsKey(buff.Buff))
        //     {
        //         buffs[buff.Buff].IncreaseTimer(buff.Buff.Duration);
        //     }
        //     else buffs.Add(buff.Buff, buff);
        //
        //     UpdateBuffUI();
        // }
        //
        // private void UpdateBuffUI()
        // {
        //     buffList.UpdateList(buffs.Values.ToArray());
        // }

        private void OnDestroy()
        {
            Stats[StatType.HP].ChangeBase(0);
        }
    }


}