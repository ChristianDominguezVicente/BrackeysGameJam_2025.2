using UnityEngine;

[CreateAssetMenu(fileName = "PunchCard", menuName = "Scriptable Objects/Cards/PunchCard")]
public class PunchCard : Card
{
    public override void OnActivated(IHittable target)
    {
        if (this.effects != null)
            foreach (CardEffect effect in effects)
                effect.OnEffectActivated(target, this.damage);
    }
}
