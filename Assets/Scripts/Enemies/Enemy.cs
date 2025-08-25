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
                Debug.Log("Me he comido " + amount + " de daño de tipo " + dt);
                break;
        }

        this.enemyStats.health -= baseDamage;

        if (enemyStats.health <= 0)
            Die();
    }
}
