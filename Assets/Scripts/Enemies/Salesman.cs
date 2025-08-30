using System.Collections.Generic;
using UnityEngine;

public class Salesman : Enemy
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.health = enemyStats.health;
        this.statusEffects = new List<StatusEffect>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Die()
    {
        Debug.Log("ME MORí");
        Destroy(gameObject);
    }
}
