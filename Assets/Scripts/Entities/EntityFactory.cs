using System.Collections.Generic;
using DiceRoll.Battles;
using DiceRoll.Entities.Player;
using DiceRoll.Map;
using DiceRoll.Settings;
using Entities.Base;
using TMPro;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace DiceRoll.Entities
{
    public class EntityFactory 
    {
        private readonly IObjectResolver _resolver;

        private readonly TextMeshPro _numberPrefab;
        
        private readonly MapInfo _mapInfo;
        
        private IPlayerInfo _playerInfo;
        
        [Inject]
        public EntityFactory(IObjectResolver resolver, TextMeshPro numberPrefab, MapInfo mapInfo)
        {
            _resolver = resolver;
            _numberPrefab = numberPrefab;
            _mapInfo = mapInfo;
            Debug.Log(_numberPrefab);
        }
        
        public EntityStats Create(EntitySettings settings)
        {
            var entity = _resolver.Instantiate(settings.EntityPrefab);
            entity.name = settings.EntityPrefab.name;
            entity.ApplyStats(settings.Stats);
            
            _mapInfo.AddEntity(entity.gameObject);

            if (entity.gameObject.layer == LayerMask.NameToLayer("Player")) 
                return entity;
            
            return SetupEnemy(entity);
        }
        
        private EntityStats SetupEnemy(EntityStats entity)
        {
            var number = Object.Instantiate(_numberPrefab, entity.transform);
                
            entity.Stats[StatType.HP].OnValueChange.AddListener((value, diff) =>
            {
                _playerInfo ??= _resolver.Resolve<IPlayerInfo>();
                    
                var excludeNum = _playerInfo.NextNumber;
                if (diff >= 0)
                    return;
                    
                //Pick number excluding the next number without loop
                List<int> possibleNumbers = new List<int>();
                for (int i = 1; i <= 6; i++)
                {
                    if (i == excludeNum)
                        continue;
                    possibleNumbers.Add(i);
                }
                entity.Number = possibleNumbers[Random.Range(0, possibleNumbers.Count)];
                number.text = entity.Number.ToString();
            });
            entity.Number = Random.Range(1, 7);
            number.text = entity.Number.ToString();
            
            var ai = new EnemyAI(entity.GetComponentsInChildren<IEnemyAbility>(), _resolver.Resolve<StepProcessor>());
            entity.Stats[StatType.HP].OnValueChange.AddListener((value, diff) =>
            {
                if(value > 0)
                    return;
                
                ai.Dispose();
                _mapInfo.RemoveEntity(entity.gameObject);
            });
            
            return entity;
        }
    }
}