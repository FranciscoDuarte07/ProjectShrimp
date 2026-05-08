using UnityEngine;
using System.Collections;

public class AfterimageEffect : MonoBehaviour
{
    private SpriteRenderer sprite;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    public void Initialize(float fadeTime)
    {
        StartCoroutine(FadeOut(fadeTime));
    }

    private IEnumerator FadeOut(float duration)
    {
        Color startColor = sprite.color;
        startColor.a = 0.6f;
        sprite.color = startColor;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(0.6f, 0f, elapsed / duration);
            Color c = sprite.color;
            c.a = alpha;
            sprite.color = c;
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
