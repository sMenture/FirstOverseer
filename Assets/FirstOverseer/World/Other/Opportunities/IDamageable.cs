namespace FirstOverseer.World.Opportunities
{

    public interface IDamageable
    {
        void TakeDamage(float damageAmount);
        void Heal(float healAmount);
    }
}