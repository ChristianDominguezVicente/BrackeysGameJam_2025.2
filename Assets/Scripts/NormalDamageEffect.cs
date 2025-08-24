using UnityEngine;

[CreateAssetMenu(fileName = "NormalDamageEffect", menuName = "Script of the normal damage")]
public class NormalDamageEffect : CardEffect
{
    public DamageType damageType = DamageType.Normal;
    public int amount = 0; // Valor que se cambia en el editor de unity

    public override void OnEffectActivated(GameObject target)
    {
        Debug.Log("CARTA DE NOMBRE " + this.name + " JUGADA");
    }
}
