using UnityEngine;

[CreateAssetMenu(fileName = "NormalDamageEffect", menuName = "Script of the normal damage")]
public class NormalDamageEffect : CardEffect
{
    public DamageType damageType = DamageType.Normal;

    public override void OnEffectActivated(IHittable target, int damage)
    {
        Debug.Log("CARTA DE NOMBRE " + this.name + " JUGADA");

        target.TakeDamage(damage, damageType);
    }
}
