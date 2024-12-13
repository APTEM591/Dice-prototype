namespace DiceRoll.Battles
{
    public interface IDamageable
    {
        public bool IsAlive { get; }
        
        public void DoDamage(IDamageable target);
        public void TakeDamage(float damage, IDamageable source);
    }
}

