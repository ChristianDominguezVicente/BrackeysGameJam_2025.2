using UnityEngine;

public enum DamageType
{
    Normal,
    Bleed,
    Fear,
    Numbing,
    DamageX2,
    PissOff,
    SecondHit,
    Fail,
    SelfDamage
}

public interface IHittable
{
    void TakeDamage(int amount, DamageType dt);
    bool HasStatusEffect(StatusEffect status);
    void AddStatus(StatusEffect status);
    void RemoveStatus(StatusEffect status);
}
