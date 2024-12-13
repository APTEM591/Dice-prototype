using System.Linq;
using DiceRoll.Battles;
using DiceRoll.Entities.Player;
using DiceRoll.Map;
using Entities;
using Entities.Base;
using Extensions;
using UnityEngine;
using VContainer;

namespace DiceRoll.Entities.Lancer
{
    public class LancerThrust : EntityMove, IEnemyAbility
    {
        public int Priority => 2;
        public AbilitiesStack Stack { get; set; }
        
        private IPlayerInfo _playerInfo;
        private StepProcessor _stepProcessor;
        private MarkHandler _markHandler;
        
        private int _stepOnAttack = -1;
        private int currentStep = 0;
        private bool _isPrepared;

        private Vector2Int[] _attackMarks;
        
        [Inject]
        private void Construct(IPlayerInfo playerInfo, StepProcessor stepProcessor, MarkHandler markHandler)
        {
            _playerInfo = playerInfo;
            _stepProcessor = stepProcessor;
            _markHandler = markHandler;
            
            _stepProcessor.AddStepListener(OnStep);
        }
        
        private void OnStep(int step) 
            => currentStep = step;

        public bool CanUseAbility()
        {
            return _isPrepared || 
                   ((_playerInfo.Position - Stats.MapPosition).magnitude <= 2 
                    && currentStep != _stepOnAttack+1);
        }
        
        public void UseAbility()
        {
            if(!_isPrepared)
            {
                var direction = (_playerInfo.Position - Stats.MapPosition).Normalize();
                
                _isPrepared = true;
                _attackMarks = new[]
                {
                    Stats.MapPosition + direction,
                    Stats.MapPosition + direction * 2
                };
                
                _markHandler.AddMarks(_attackMarks);
                return;
            }
            
            _markHandler.ResetMarks();
            _isPrepared = false;
            _stepOnAttack = currentStep;

            bool isCloseEnough = false;
            //Getting all enemies in the direction and getting the closest one
            var entity = MapInfo.Entities.Where(e =>
            {
                if(e == gameObject)
                    return false;
                
                var entityPos = e.GetComponent<EntityMove>().MapPosition;
                Debug.Log($"[LancerThrust] Checking at position {_attackMarks[0]}, entity at {entityPos}: {entityPos == _attackMarks[0]}");
                Debug.Log($"[LancerThrust] Checking at position {_attackMarks[1]}, entity at {entityPos}: {entityPos == _attackMarks[1]}");
                isCloseEnough = entityPos == _attackMarks[0];
                return entityPos == _attackMarks[0] || entityPos == _attackMarks[1];
            }).FirstOrDefault();
            
            if(!isCloseEnough)
                TryMove(_attackMarks[0] - Stats.MapPosition);
            
            if (entity == default)
            {
                Debug.Log("[LancerThrust] No entity found in the direction");
                return;
            }
            
            ((IDamageable)Stats).DoDamage(entity.GetComponent<IDamageable>());
        }
        
    }
}