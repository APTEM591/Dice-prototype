using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DiceRoll.Battles;
using DiceRoll.Map;
using Entities;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DiceRoll.Entities.Player
{
    public class PlayerMove : EntityMove
    {
        [SerializeField] private Transform player;
        [SerializeField] private MeshRenderer dice;
        [SerializeField] private LayerMask enemyLayer;

        [SerializeField] private int rotationSmoothness = 16;

        private float _stepSize;
        private float _diff;

        private Vector2 _prevDirection = Vector2.right;

        private Sequence _sequence;

        private StepProcessor _stepProcessor;
        
        private PlayerStats PlayerStats => Stats as PlayerStats;
        private int NextNumber
        {
            get => PlayerStats.NextNumber;
            set => PlayerStats.NextNumber = value;
        }

        [Inject]
        private void Construct(StepProcessor stepProcessor)
        {
            _stepProcessor = stepProcessor;
        }

        protected void Start()
        {
            //Calculating width of the dice
            _stepSize = dice.bounds.size.x;
            _diff = (player.localPosition).magnitude;
        }

        public void HandleMovement(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Performed || _sequence is { active: true }) return;

            var direction = Vector2Int.RoundToInt(context.ReadValue<Vector2>().normalized);

            if (direction != _prevDirection)
            {
                Rotate(direction);
                return;
            }

            if (!TryMove(direction))
                return;

            //rolling dice
            _sequence = DOTween.Sequence()
                .Join(dice.transform.DOBlendableRotateBy(new Vector3(direction.y, 0, -direction.x) * 90,
                    0.5f, RotateMode.FastBeyond360))
                .OnComplete(() =>
                {
                    UpdateNextNumber();
                    TryDamagePhysic(direction);
                    TryDamageNumber();
                });

            //MapInfo.UpdatePlayerInfo(MapPosition, _nextNumber);
            _stepProcessor.ProcessStep();
        }

        private void TryDamagePhysic(Vector2Int direction)
        {
            if (!HasEntityAt(MapPosition, enemyLayer, out var entity))
                return;

            ((IDamageable)PlayerStats).TakeDamage(1, entity);
            entity.TakeDamage(1, PlayerStats);
            if (entity.IsAlive)
            {
                TryMove(-direction);

                _sequence = DOTween.Sequence()
                    .Join(dice.transform.DOBlendableRotateBy(new Vector3(direction.y, 0, -direction.x) * -90,
                    0.5f, RotateMode.FastBeyond360));
            }
        }
        
        private bool HasEntityAt(Vector2Int position, LayerMask layer, out IDamageable entity)
        {
            entity = null;

            entity = MapInfo.Entities.Find(e =>
            {
                var stats = e.GetComponent<EntityStats>();
                return stats.MapPosition == position && stats.gameObject.layer == layer;
            })?.GetComponent<IDamageable>();
            
            return entity != null;
        }

        private void TryDamageNumber()
        {
            Transform pick = null;
            for (int i = 0; i < dice.transform.childCount; i++)
            {
                var current = dice.transform.GetChild(i);
                if (pick == null || current.position.y > pick.transform.position.y)
                    pick = current;
            }

            var number = int.Parse(pick.name);
            Debug.Log($"Picked number: {number}");

            var entities = GetEntitiesWithNumber(number);
            Debug.Log($"Entities with number {number}: {entities.Count}");
            
             foreach (var entity in MapInfo.Entities)
                 Debug.Log($"Entity {entity.name} has number {entity.GetComponent<EntityStats>().Number}");
                
            
            foreach (var entity in entities)
                ((IDamageable)PlayerStats).DoDamage(entity);
        }
        
        private List<IDamageable> GetEntitiesWithNumber(int number)
        {
            return MapInfo.Entities
                .ConvertAll(e => e.GetComponent<EntityStats>())
                .Where(entity => entity.Number == number)
                .Cast<IDamageable>()
                .ToList();
        }
        
        private void UpdateNextNumber()
        {
            var axis = _prevDirection;
            int axisIndex = axis.x == 0 ? 2 : 0;
            
            Transform pick = null;
            //Find next number on the dice by axis
            for (int i = 0; i < dice.transform.childCount; i++)
            {
                var current = dice.transform.GetChild(i);
                if (pick == null || (axis[axisIndex == 2 ? 1 : 0] > 0 
                        ? current.position[axisIndex] < pick.position[axisIndex] 
                        : current.position[axisIndex] > pick.position[axisIndex]))
                    pick = current;
            }
            
            NextNumber = int.Parse(pick.name);
            Debug.Log($"Next number: {NextNumber}");
        }

        private void Rotate(Vector2 direction)
        {
            var playerPoints = new Vector3[rotationSmoothness + 1];
            var dicePoints = new Vector3[rotationSmoothness + 1];

            var startAngle = Mathf.Atan2(_prevDirection.y, _prevDirection.x) * Mathf.Rad2Deg;
            var endAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            //Debug.Log($"Start: {startAngle} End: {endAngle}");

            //Move around the dice to needed position from point to point
            for (var i = 0; i <= rotationSmoothness; i++)
            {
                var angle = Mathf.Lerp(startAngle, endAngle, i / (float)rotationSmoothness);
                //Debug.Log(angle);
                var x = Mathf.Cos(angle * Mathf.Deg2Rad) * _diff;
                var z = Mathf.Sin(angle * Mathf.Deg2Rad) * _diff;
                playerPoints[i] = -new Vector3(x, player.localPosition.y, z);
                dicePoints[i] = new Vector3(x, dice.transform.localPosition.y, z);
            }

            _sequence = DOTween.Sequence()
                .Append(player.DOLocalPath(playerPoints, 0.5f))
                .Join(dice.transform.DOLocalPath(dicePoints, 0.5f));
            
            _prevDirection = direction;
            UpdateNextNumber();
            
            _stepProcessor.ProcessStep();
        }
        
        private void OnDrawGizmosSelected()
        {
            
            for (int i = 0; i < dice.transform.childCount; i++)
            {
                var child = dice.transform.GetChild(i);
                Handles.Label(child.position, child.name, new GUIStyle
                {
                    normal = {textColor = Color.red},
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 16
                });
            }
        }
    }
}