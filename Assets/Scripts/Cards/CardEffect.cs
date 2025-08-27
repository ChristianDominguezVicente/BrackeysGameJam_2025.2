using UnityEngine;

[CreateAssetMenu(fileName = "CardEffect", menuName = "Scriptable Objects/Effect/CardEffect")]
public abstract class CardEffect : ScriptableObject
{
    public abstract void OnEffectActivated(IHittable target, int damage);
}
