using UnityEngine;

[CreateAssetMenu(fileName = "BiteAttack", menuName = "Scriptable Objects/EnemyAttack/BiteAttack")]
public class BiteAttack : EnemyAttack
{
    public DamageType damageType = DamageType.Normal;
    public int baseAttack;

    public override void OnAttackActivated(IHittable target, int damage)
    {
        Debug.Log("Ataque de mordisco realizado");

        target.TakeDamage(damage + baseAttack, DamageType.Normal);
    }
}
