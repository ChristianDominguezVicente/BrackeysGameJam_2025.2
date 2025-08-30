using UnityEngine;

[CreateAssetMenu(fileName = "DefaultCard", menuName = "Scriptable Objects/Cards/DefaultCard")]
public class DefaultCard : Card
{
    public override void OnActivated(IHittable target)
    {
        if (this.effects != null)
            foreach (CardEffect effect in effects)
                effect.OnEffectActivated(target, this.damage);
    }
}
