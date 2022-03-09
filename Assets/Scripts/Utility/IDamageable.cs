public interface IDamageable
{
    void TakeDamage(int damage, string attacker, string weaponSprite, bool canSelfDamage = false);
}
