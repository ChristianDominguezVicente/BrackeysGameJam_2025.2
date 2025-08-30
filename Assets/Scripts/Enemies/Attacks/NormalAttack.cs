using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NormalAttack", menuName = "Scriptable Objects/EnemyAttack/NormalAttack")]
public class NormalAttack : EnemyAttack
{
    [Header("Attack Stats")]
    [SerializeField] private List<DamageType> damageTypes;
    [SerializeField] private int baseAttack;
    [SerializeField] private int chanceToHit;
    [SerializeField] private string attackName;
    [SerializeField] private int chanceToUse;

    public int ChanceToUse { get { return chanceToUse; } }

    public override void OnAttackActivated(IHittable target, int damage)
    {
        Debug.Log($"Ataque {attackName} realizado");

        int damageToHit = baseAttack;

        if (damage == 0)
        {
            damageToHit /= 2;
        }

        if (chanceToHit >= Random.Range(0, 100))
        {
            foreach (DamageType dmg in damageTypes)
            {
                target.TakeDamage(damageToHit, dmg);
                damageToHit *= 0;
            }
        }
        else
        {
            Debug.Log("El enemigo falló su ataque");
        }
    }
}
