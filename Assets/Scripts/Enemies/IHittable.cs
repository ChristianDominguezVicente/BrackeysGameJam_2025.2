using UnityEngine;

public enum DamageType
{ 
    Normal,
}

public interface IHittable
{
    void TakeDamage(int amount, DamageType dt);
}
