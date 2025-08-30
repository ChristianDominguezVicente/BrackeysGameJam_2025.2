using UnityEngine;

[CreateAssetMenu(fileName = "BiteAttack", menuName = "Scriptable Objects/EnemyAttack/BiteAttack")]
public class BiteAttack : EnemyAttack
{
    [Header("Attack Stats")]
    [SerializeField] private DamageType damageType = DamageType.Normal;
    [SerializeField] private int baseAttack;
    [SerializeField] private int chanceToHit;

    public override void OnAttackActivated(IHittable target, int damage)
    {
        Debug.Log("Ataque de mordisco realizado");

        if (chanceToHit >= Random.Range(0, 100))
            target.TakeDamage(damage + baseAttack, DamageType.Normal);
        else
            Debug.Log("El enemigo falló su ataque");
    }
}
