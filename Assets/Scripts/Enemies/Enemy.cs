using UnityEngine;

public abstract class Enemy : MonoBehaviour, IHittable
{
    public EnemyStats enemyStats;

    public abstract void Die();

    public virtual void TakeDamage(int amount, DamageType dt)
    {
        int baseDamage = amount;

        switch (dt)
        {
            default:
                break;
        }

        this.enemyStats.health -= baseDamage;

        if (enemyStats.health <= 0)
            Die();
    }
}
