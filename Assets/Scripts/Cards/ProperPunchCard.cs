using UnityEngine;

[CreateAssetMenu(fileName = "ProperPunchCard", menuName = "Scriptable Objects/Cards/ProperPunchCard")]
public class ProperPunchCard : Card
{
    public override void OnActivated(IHittable target)
    {
        if (this.effects != null)
            foreach (CardEffect effect in effects)
                effect.OnEffectActivated(target, this.damage);
    }
}
