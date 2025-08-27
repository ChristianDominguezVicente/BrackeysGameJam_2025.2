using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "Enemy", menuName = "Scriptable Objects/Enemy")]
public class EnemyStats : ScriptableObject
{
    public string enemyName;
    public int health;
    public int damage;
    public List<EnemyAttack> attacks;
}
