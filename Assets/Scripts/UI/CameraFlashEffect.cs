using System.Collections;
using System.Timers;
using UnityEngine;
using UnityEngine.UI;

public class CameraShakeEffect : MonoBehaviour
{
    [SerializeField] private float effectDuration = 1.0f;
    [SerializeField] private float flashInterval = 0.2f;
    [SerializeField] private Image damageImage;
    private Player player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        player = TurnManager.tm.Player;

        if (player != null)
        {
            player.OnDamageTaken += DamageScreen;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDestroy()
    {
        if (player != null)
        {
            player.OnDamageTaken -= DamageScreen;
        }
    }

    private void DamageScreen(int amount)
    {
        if (amount <= 0) return;

        StartCoroutine(FlashRedDamage(effectDuration, 0));

    }

    private IEnumerator FlashRedDamage(float duration, float magnitude)
    {
        damageImage.gameObject.SetActive(true);

        Color original = damageImage.color;

        float passedTime = 0f;

        float alpha = original.a;

        while (passedTime <= effectDuration)
        {
            damageImage.color = new Color(original.r, original.g, original.b, alpha);

            if (alpha != 0)
                alpha = 0;
            else
                alpha = original.a;

            yield return new WaitForSeconds(flashInterval);
            passedTime += flashInterval;
            Debug.Log("TIEMPO PASADO " + passedTime + " DE " + effectDuration);
        }

        Debug.Log("TIEMPO PASADO FINAL" + passedTime + " DE " + effectDuration);

        damageImage.color = original;
        damageImage.gameObject.SetActive(false);
    }
}
