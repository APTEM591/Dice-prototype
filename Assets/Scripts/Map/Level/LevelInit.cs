using System;
using System.Runtime.InteropServices;
using DiceRoll.Entities;
using DiceRoll.Settings;
using TriInspector;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace DiceRoll.Map.Level
{
    public class LevelInit : MonoBehaviour
    {
        [SerializeField] private EntitySettings playerSettings;
        
        private MapInfo _mapInfo;
        private EntityFactory _entityFactory;
        
        [Inject]
        private void Construct(MapInfo mapInfo, EntityFactory entityFactory)
        {
            _mapInfo = mapInfo;
            _entityFactory = entityFactory;
            
            Spawn(playerSettings, new Vector2Int(2, _mapInfo.Height / 2));
        }

        [Button]
        private void Spawn(EntitySettings entitySettings, Vector2Int position)
        {
            if(position.x < 0 || position.x >= _mapInfo.Width || position.y < 0 || position.y >= _mapInfo.Height)
                throw new ArgumentOutOfRangeException(nameof(position), position, "Position is out of map bounds");
            
            var entity = _entityFactory.Create(entitySettings);
            entity.GetComponent<EntityMove>().ForceMove(position);
        }
    }
}