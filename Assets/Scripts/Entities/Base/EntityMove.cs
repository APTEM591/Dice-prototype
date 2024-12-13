using System;
using DG.Tweening;
using DiceRoll.Map;
using Entities;
using UnityEngine;
using VContainer;

namespace DiceRoll.Entities
{
    public class EntityMove : MonoBehaviour, IEntityModule
    {
        protected MapInfo MapInfo { get; private set; }

        protected EntityStats Stats { get; private set; }

        public Vector2Int MapPosition
        {
            get => Stats.MapPosition;
            private set => Stats.MapPosition = value;
        }

        EntityStats IEntityModule.Stats
        {
            get => Stats;
            set
            {
                Debug.Log(this + " Stats set");
                Stats = value;
            }
        }

        [Inject]
        private void Construct(MapInfo mapInfo)
        {
            MapInfo = mapInfo;
        }

        protected bool TryMove(Vector2Int direction)
        {
            var targetPos = MapPosition + direction;
            return TryMoveTo(targetPos);
        }
        
        protected bool TryMoveTo(Vector2Int targetPos)
        {
            if (targetPos.x < 0 || targetPos.x >= MapInfo.Width || targetPos.y < 0 || targetPos.y >= MapInfo.Height)
                return false;

            MapPosition = targetPos;
            transform.DOMove(MapInfo.MapToWorld(MapPosition), 0.5f);
            return true;
        }
        
        public void ForceMove(Vector2Int targetPos)
        {
            MapPosition = targetPos;
            transform.position = MapInfo.MapToWorld(MapPosition);
        }
    }
}