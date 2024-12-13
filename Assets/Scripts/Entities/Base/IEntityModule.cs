using DiceRoll.Entities;

namespace Entities
{
    public interface IEntityModule
    {
        public EntityStats Stats { get; protected internal set; }
    }
}