using UnityEngine;

[CreateAssetMenu(fileName = "Effect", menuName = "Scriptable Objects/Effect")]
public abstract class CardEffect : ScriptableObject
{
    public abstract void OnEffectActivated(IHittable target, int damage);
}
