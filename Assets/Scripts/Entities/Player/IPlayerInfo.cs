using UnityEngine;

namespace DiceRoll.Entities.Player
{
    public interface IPlayerInfo
    {
        public Vector2Int Position { get; }
        public int NextNumber { get; }
    }
}