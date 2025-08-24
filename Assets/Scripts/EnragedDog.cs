using UnityEngine;

public class EnragedDog : MonoBehaviour, IHittable
{
    public void TakeDamage(int amount, DamageType dt)
    {
        switch (dt)
        {
            default:
                Debug.Log("Me he comido " + amount + " de daño de tipo " + dt);
                break;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
