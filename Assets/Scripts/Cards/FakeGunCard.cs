using UnityEngine;

[CreateAssetMenu(fileName = "FakeGunCard", menuName = "Scriptable Objects/Cards/FakeGunCard")]
public class FakeGunCard : Card
{
    public override void OnActivated(IHittable target)
    {
        if (this.effects != null)
            foreach (CardEffect effect in effects)
                effect.OnEffectActivated(target, this.damage);
    }
}
