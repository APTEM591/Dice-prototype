using System;
using System.Collections.Generic;
using DiceRoll.Battles;
using DiceRoll.Entities.Player;
using DiceRoll.Map;
using Entities.Base;
using L1PathFinder;
using TriInspector;
using UnityEngine;
using VContainer;

namespace DiceRoll.Entities.Lancer
{
    public class LancerMove : EntityMove, IEnemyAbility
    {
        public int Priority => 3;
        public bool CanUseAbility() => true;
        public AbilitiesStack Stack { get; set; }

        private MapInfo _mapInfo;
        private IPlayerInfo _playerInfo;

        [ShowInInspector] private List<Point> _path;

        [Inject]
        private void Construct(MapInfo mapInfo, IPlayerInfo playerInfo)
        {
            _mapInfo = mapInfo;
            _playerInfo = playerInfo;

            Debug.Log("LancerMove Constructed");
        }


        public void UseAbility()
        {
            UpdatePath();
            if (_path.Count == 0)
                return;

            var nextPos = new Vector2Int(_path[^1].X, _path[^1].Y);
            var direction = nextPos - MapPosition;
            if (direction.magnitude <= 2 && _path.Count == 1)
                return;
            TryMove(Vector2Int.RoundToInt(new Vector2(direction.x, direction.y).normalized));
        }

        private void UpdatePath()
        {
            _path = new();
            L1PathPlanner.CreatePlanner(new int[_mapInfo.Width, _mapInfo.Height])
                .Search(new Point(MapPosition.x, MapPosition.y)
                    , new Point(_playerInfo.Position.x, _playerInfo.Position.y)
                    , out _path);

            _path.Remove(_path[^1]);
        }

        private void OnDrawGizmosSelected()
        {
            if (_path == null)
                return;

            Gizmos.color = Color.red;
            foreach (var point in _path)
            {
                var worldPos = _mapInfo.MapToWorld(new Vector2Int(point.X, point.Y));
                Gizmos.DrawSphere(worldPos, 0.1f);
            }
        }
    }
}