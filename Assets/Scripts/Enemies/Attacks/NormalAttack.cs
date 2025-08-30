using UnityEngine;

[CreateAssetMenu(fileName = "NormalAttack", menuName = "Scriptable Objects/EnemyAttack/NormalAttack")]
public class NormalAttack : EnemyAttack
{
    [Header("Attack Stats")]
    [SerializeField] private DamageType damageType = DamageType.Normal;
    [SerializeField] private int baseAttack;
    [SerializeField] private int chanceToHit;
    [SerializeField] private string attackName;
    [SerializeField] private int chanceToUse;

    public int ChanceToUse { get { return chanceToUse; } }

    public override void OnAttackActivated(IHittable target, int damage)
    {
        Debug.Log("Ataque de mordisco realizado");

        if (chanceToHit >= Random.Range(0, 100))
            target.TakeDamage(damage + baseAttack, damageType);
        else
            Debug.Log("El enemigo falló su ataque");
    }
}
