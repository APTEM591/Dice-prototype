using DiceRoll.Map;
using UnityEngine;
using VContainer;

namespace DiceRoll.Entities.Player
{
    public class PlayerStats : EntityStats, IPlayerInfo
    {
        public Vector2Int Position => MapPosition;
        public int NextNumber { get; internal set; }
    }
}