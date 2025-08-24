using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "PunchCard", menuName = "Script of the Punch card")]
public class PunchCard : Card
{
    public override void OnActivated(GameObject target)
    {
        if (this.effects != null)
            foreach (CardEffect effect in effects)
                effect.OnEffectActivated(target);
    }
}
