using UnityEngine;

[CreateAssetMenu(fileName = "FearEffect", menuName = "Scriptable Objects/Effect/FearEffect")]
public class FearEffect : CardEffect
{
    public DamageType damageType = DamageType.Fear;

    public override void OnEffectActivated(IHittable target, int damage)
    {
        Debug.Log($"Efecto {damageType} JUGADO");

        target.TakeDamage(damage, damageType);
    }
}
