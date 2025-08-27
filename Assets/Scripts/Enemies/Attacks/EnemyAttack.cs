using UnityEngine;

[CreateAssetMenu(fileName = "EnemyAttack", menuName = "Scriptable Objects/EnemyAttack")]
public abstract class EnemyAttack : ScriptableObject
{
    public abstract void OnAttackActivated(IHittable target, int damage);
}
