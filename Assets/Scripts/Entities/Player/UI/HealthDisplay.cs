using DiceRoll.Battles;
using DiceRoll.Entities.Player;
using UnityEngine;
using VContainer;

namespace Entities.Player.UI
{
    public class HealthDisplay : MonoBehaviour
    {
        [SerializeField] private Transform healthPrefab;
        [SerializeField] private Transform healthParent;
        
        [Inject]
        private void Construct(IPlayerInfo playerStats)
        {
            ((PlayerStats)playerStats).Stats[StatType.HP].OnValueChange.AddListener(UpdateHealth);
            UpdateHealth(((PlayerStats)playerStats).Stats[StatType.HP].Value, 0);
            //Debug.Log(((PlayerStats)playerStats).Stats[StatType.HP].Value);
        }


        private void UpdateHealth(float value, float diff)
        {
            foreach (Transform child in healthParent)
            {
                Destroy(child.gameObject);
            }
            
            for (int i = 0; i < value; i++)
            {
                var health = Instantiate(healthPrefab, healthParent);
            }
        }
    }
}