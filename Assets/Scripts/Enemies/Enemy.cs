using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour, IHittable
{
    [Header("Enemy stats")]
    [SerializeField] protected EnemyStats enemyStats;

    [Header("Enemy configuration")]
    [SerializeField] protected float flashDuration = 0.2f;
    [SerializeField] protected float damageFlashDuration = 0.3f;
    [SerializeField] protected int damageFlashes = 3;

    private SpriteRenderer sr;

    public string EnemyName { get { return enemyStats.name; } }
    public EnemyType EnemyType { get { return enemyStats.enemyType; } }

    protected int health;
    protected List<StatusEffect> statusEffects;

    private Coroutine flashCoroutine;

    public int Health { get { return health; } }

    public abstract void Die();
    public virtual StatusEffect HandleStatusEffects(int playerLife)
    {
        foreach (StatusEffect se in statusEffects)
        {

            switch (se)
            {
                case StatusEffect.SecondHit:
                    TakeDamage(3, DamageType.Bleed);
                    AddStatus(StatusEffect.Bleeding);
                    break;
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

    void Awake()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();

        if (sr == null)
            Debug.LogError("NO SE ENCONTRO SPRITE RENDERER");
    }

    void Start()
    {

    }

    public virtual void TakeDamage(int amount, DamageType dt)
    {
        int baseDamage = amount;

        switch (dt)
        {
            case DamageType.DamageX2:
                AddStatus(StatusEffect.DamageX2);
                break;

            case DamageType.PissOff:
                AddStatus(StatusEffect.PissOff);
                break;

            case DamageType.SecondHit:
                AddStatus(StatusEffect.SecondHit);
                break;

            case DamageType.Fail:
                AddStatus(StatusEffect.Fail);
                break;

            case DamageType.SelfDamage:
                AddStatus(StatusEffect.SelfDamage);
                break;

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

        if (baseDamage > 0)
        {
            StartCoroutine(DamageFlashEffect());
        }

        Debug.Log($"Me como daño {amount} para una vida total de {health}");

        if (health <= 0)
            Die();
    }

    private IEnumerator DamageFlashEffect()
    {
        if (sr == null) yield break;

        for (int i = 0; i < damageFlashes; i++)
        {
            sr.color = Color.red;
            yield return new WaitForSeconds(damageFlashDuration);

            sr.color = Color.white;
            yield return new WaitForSeconds(damageFlashDuration);
        }

        sr.color = Color.white;
    }

    public virtual void Attack(IHittable target)
    {
        EnemyAttack ea = GetRandomAttack();

        int damage = 1;

        if (ea != null)
        {
            if (HasStatusEffect(StatusEffect.SelfDamage))
            {
                if (Random.Range(1, 100) < 50)
                {
                    target = this;
                }
            }

            if (HasStatusEffect(StatusEffect.PissOff))
            {
                damage = 0;
                RemoveStatus(StatusEffect.PissOff);
            }

            if (HasStatusEffect(StatusEffect.Fail))
            {
                RemoveStatus(StatusEffect.Fail);
                Debug.Log("Enemigo falla debido a estado");
                return;
            }

            ea.OnAttackActivated(target, damage);
        }
    }

    public void StartFlashing()
    {
        Debug.Log($"Seleccion empezada {sr} | {flashCoroutine}");

        if (sr == null) return;

        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        flashCoroutine = StartCoroutine(FlashEffect());
    }

    public void StopFlashing()
    {
        Debug.Log("Seleccion parada");

        if (sr == null) return;

        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            sr.color = Color.white;
        }
    }

    private IEnumerator FlashEffect()
    {
        Debug.Log("Efecto flash iniciado");

        while (true)
        {
            sr.color = Color.white;
            yield return new WaitForSeconds(flashDuration);

            sr.color = Color.green;
            yield return new WaitForSeconds(flashDuration);
        }
    }

    protected virtual EnemyAttack GetRandomAttack()
    {
        if (enemyStats.attacks != null && enemyStats.attacks.Count > 0)
        {
            int generatedNumber = Random.Range(0, 100);
            int baseChance = 0;

            foreach (NormalAttack attack in enemyStats.attacks)
            {
                baseChance += attack.ChanceToUse;

                if (generatedNumber <= baseChance)
                    return attack;
            }
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
