using UnityEngine;

public abstract class Enemy : MonoBehaviour, IHittable
{
    [Header("Enemy stats")]
    [SerializeField] protected EnemyStats enemyStats;

    protected int health;

    public int Health { get { return health; } }

    public abstract void Die();

    void Start()
    {
        this.health = enemyStats.health;
    }

    public virtual void TakeDamage(int amount, DamageType dt)
    {
        int baseDamage = amount;

        switch (dt)
        {
            default:
                break;
        }

        this.health -= baseDamage;

        Debug.Log($"Me como daño {amount} para una vida total de {health}");

        if (health <= 0)
            Die();
    }
}
