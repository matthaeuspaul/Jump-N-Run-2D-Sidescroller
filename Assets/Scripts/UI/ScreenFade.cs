using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFade : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeSpeed = 1f;
    [SerializeField] private Color fadeColor = Color.black;

    private Coroutine currentFade;

    private void Awake()
    {
        // Check if fade image is assigned
        if (fadeImage == null)
        {
            Debug.LogError("[ScreenFade] Fade Image is not assigned! Please assign it in the Inspector.");
            return;
        }

        // Start with transparent (no fade)
        SetAlpha(0f);
        fadeImage.gameObject.SetActive(true);
    }

    public IEnumerator FadeOut(float duration = -1f)
    {
        if (duration < 0) duration = 1f / fadeSpeed;

        if (currentFade != null)
            StopCoroutine(currentFade);

        fadeImage.gameObject.SetActive(true);

        float elapsed = 0f;
        Color color = fadeImage.color;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float alpha = Mathf.Clamp01(elapsed / duration);
            color.a = alpha;
            fadeImage.color = color;
            yield return null;
        }

        // Ensure fully faded
        color.a = 1f;
        fadeImage.color = color;
    }

    /// <summary>
    /// Fade from black (or fadeColor) to transparent
    /// </summary>
    public IEnumerator FadeIn(float duration = -1f)
    {
        if (duration < 0) duration = 1f / fadeSpeed;

        if (currentFade != null)
            StopCoroutine(currentFade);

        fadeImage.gameObject.SetActive(true);

        float elapsed = 0f;
        Color color = fadeImage.color;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float alpha = 1f - Mathf.Clamp01(elapsed / duration);
            color.a = alpha;
            fadeImage.color = color;
            yield return null;
        }

        // Ensure fully transparent
        color.a = 0f;
        fadeImage.color = color;
        fadeImage.gameObject.SetActive(false);
    }

    public void SetAlpha(float alpha)
    {
        Color color = fadeImage.color;
        color.a = Mathf.Clamp01(alpha);
        fadeImage.color = color;

        fadeImage.gameObject.SetActive(alpha > 0f);
    }

    public IEnumerator FadeOutAndIn(float outDuration = -1f, float waitDuration = 0.2f, float inDuration = -1f)
    {
        yield return FadeOut(outDuration);
        yield return new WaitForSecondsRealtime(waitDuration);
        yield return FadeIn(inDuration);
    }
}