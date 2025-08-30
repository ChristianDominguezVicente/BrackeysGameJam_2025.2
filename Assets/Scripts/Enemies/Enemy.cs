using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour, IHittable
{
    [Header("Enemy stats")]
    [SerializeField] protected EnemyStats enemyStats;

    public string EnemyName { get { return enemyStats.name; } }
    public EnemyType EnemyType { get { return enemyStats.enemyType; } }

    protected int health;
    protected List<StatusEffect> statusEffects;

    public int Health { get { return health; } }

    public abstract void Die();
    public virtual StatusEffect HandleStatusEffects(int playerLife)
    {
        foreach (StatusEffect se in statusEffects)
        {

            switch (se)
            {
                case StatusEffect.Bleeding:
                    TakeDamage(1, DamageType.Bleed);
                    break;

                case StatusEffect.Numb:
                    return StatusEffect.Numb;

                case StatusEffect.Torment:
                    if (playerLife > this.health)
                        return StatusEffect.Torment;

                    break;

                default:
                    break;
            }

            if (this.health <= 0)
                return StatusEffect.Death;
        }

        return StatusEffect.None;
    }

    void Start()
    {
    }

    public virtual void TakeDamage(int amount, DamageType dt)
    {
        int baseDamage = amount;

        switch (dt)
        {
            case DamageType.Bleed:
                AddStatus(StatusEffect.Bleeding);
                break;

            case DamageType.Fear:
                AddStatus(StatusEffect.Torment);
                break;

            case DamageType.Numbing:
                AddStatus(StatusEffect.Numb);
                break;

            default:
                break;
        }

        this.health -= baseDamage;

        Debug.Log($"Me como daño {amount} para una vida total de {health}");

        if (health <= 0)
            Die();
    }

    public virtual void Attack(IHittable target)
    {
        EnemyAttack ea = GetRandomAttack();

        if (ea != null)
        {
            ea.OnAttackActivated(target, enemyStats.damage);
        }
    }

    private EnemyAttack GetRandomAttack()
    {
        if (enemyStats.attacks != null && enemyStats.attacks.Count > 0)
        {
            return enemyStats.attacks[Random.Range(0, enemyStats.attacks.Count)];
        }

        return null;
    }

    public bool HasStatusEffect(StatusEffect status)
    {
        return statusEffects.Contains(status);
    }

    public virtual void AddStatus(StatusEffect status)
    {
        if (HasStatusEffect(status))
            return;

        statusEffects.Add(status);
    }

    public virtual void RemoveStatus(StatusEffect status)
    {
        if (!HasStatusEffect(status))
            return;

        statusEffects.Remove(status);
    }
}
