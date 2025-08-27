using System.Collections.Generic;
using UnityEngine;

public class EnragedDog : Enemy
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

    public override StatusEffect HandleStatusEffects(int playerLife)
    {
        foreach (StatusEffect se in statusEffects)
        {
            switch (se)
            {
                case StatusEffect.Numb:
                    return StatusEffect.Numb;
            }
        }

        return StatusEffect.None;
    }
}
