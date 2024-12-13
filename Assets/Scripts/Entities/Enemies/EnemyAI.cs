using System;
using System.Linq;
using DiceRoll.Battles;

namespace Entities.Base
{
    public class EnemyAI : IDisposable
    {
        private IEnemyAbility[] _abilities;
        private StepProcessor _stepProcessor;
        private AbilitiesStack _stack;

        public EnemyAI(IEnemyAbility[] abilities, StepProcessor stepProcessor)
        {
            _abilities = abilities.OrderBy(ability => ability.Priority).ToArray();
            _stepProcessor = stepProcessor;
            
            _stack = new AbilitiesStack(3);
            foreach (var ability in _abilities)
                ability.Stack = _stack;
            
            _stepProcessor.AddStepListener(_ => MakeDecision());
        }
        
        ~EnemyAI()
        {
            ReleaseUnmanagedResources();
        }

        private void MakeDecision()
        {
            foreach (var ability in _abilities)
            {
                if (!ability.CanUseAbility()) 
                    continue;
                
                ability.UseAbility();
                _stack.AddAbility(ability);
                return;
            }
        }

        private void ReleaseUnmanagedResources()
        {
            _stepProcessor.RemoveStepListener(_ => MakeDecision());
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }
    }
}