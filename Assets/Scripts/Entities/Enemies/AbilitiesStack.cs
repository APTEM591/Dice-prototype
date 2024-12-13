using System.Collections.Generic;
using System.Linq;

namespace Entities.Base
{
    public class AbilitiesStack
    {
        public int MaxAmount { get; private set; }
        private Stack<IEnemyAbility> _abilities;
        
        public AbilitiesStack(int maxAmount)
        {
            MaxAmount = maxAmount;
            _abilities = new Stack<IEnemyAbility>(maxAmount);
        }
        
        public void AddAbility(IEnemyAbility ability)
        {
            if (_abilities.Count < MaxAmount)
            {
                _abilities.Push(ability);
            }
        }
        
        public IEnemyAbility[] GetLastAbilities(int amount)
        {
            var abilities = new IEnemyAbility[amount];
            for (var i = 0; i < amount; i++)
            {
                if (_abilities.Count == 0)
                    break;
                abilities[i] = _abilities.ElementAtOrDefault(i);
            }

            return abilities;
        }
    }
}