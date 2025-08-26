using UnityEngine;

public class EnragedDog : Enemy
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.health = enemyStats.health;
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
