using UnityEngine;

namespace Entities.Base
{
    public interface IEnemyAbility
    {
        public int Priority { get; }
        public AbilitiesStack Stack { get; set; }

        public bool CanUseAbility();
        public void UseAbility();
    }
}