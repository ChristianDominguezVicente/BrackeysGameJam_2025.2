using UnityEngine;

[CreateAssetMenu(fileName = "NormalDamageEffect", menuName = "Scriptable Objects/Effect/Normal damage effect")]
public class NormalDamageEffect : CardEffect
{
    public DamageType damageType = DamageType.Normal;

    public override void OnEffectActivated(IHittable target, int damage)
    {
        Debug.Log("CARTA DE NOMBRE " + this.name + " JUGADA");

        target.TakeDamage(damage, damageType);
    }
}
