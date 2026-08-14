using UnityEngine;
using System.Collections;

public class Popup : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float fadeDuration = 0.3f;  // Zeit für Fade-in/Fade-out
    [SerializeField] private Vector3 inactiveScale = new Vector3(0.9f, 0.9f, 0.9f);
    [SerializeField] private Vector3 activeScale = new Vector3(1, 1, 1);
    [Header("Display time in seconds - negative values are infinite")]
    public float displayTime = 2f;     // Wie lange sichtbar bleiben
    [Header("UI References")]
    [SerializeField] protected TMPro.TextMeshProUGUI title;

    private string titletext;

    private CanvasGroup canvasGroup;
    private Coroutine fadeRoutine;

    public string Titletext
    {
        get => titletext; set
        {
            titletext = value;
            if (title != null) title.text = titletext;
        }
    }

    public virtual void OnEnable()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        if (title != null) title.text = titletext;

        // unsichtbar starten
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }

    public void Hide()
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeOutRoutine());
        canvasGroup.blocksRaycasts = false;
    }

    public void Show()
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        if (displayTime > 0)
        {
            fadeRoutine = StartCoroutine(FadeInOutRoutine());
        }
        else
        {
            fadeRoutine = StartCoroutine(FadeInRoutine());
        }
        canvasGroup.blocksRaycasts = true;
    }

    public void Show(float delay)
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        if (displayTime > 0)
        {
            fadeRoutine = StartCoroutine(FadeInOutRoutineDelayed(delay));
        }
        else
        {
            fadeRoutine = StartCoroutine(FadeInRoutineDelayed(delay));
        }
        canvasGroup.blocksRaycasts = true;
    }

    private IEnumerator FadeInOutRoutineDelayed(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        yield return FadeInOutRoutine();
    }
    private IEnumerator FadeInRoutineDelayed(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        yield return FadeInRoutine();
    }

    private IEnumerator FadeInRoutine()
    {
        // Fade-in and Scale-in
        yield return FadeAndScale(0f, 1f, inactiveScale, activeScale, fadeDuration);
    }

    private IEnumerator FadeOutRoutine()
    {
        // Fade-out and Scale-out
        yield return FadeAndScale(1.0f, 0.0f, activeScale, inactiveScale, fadeDuration);
        Destroy(gameObject);
    }

    private IEnumerator FadeInOutRoutine()
    {
        // Fade-in and Scale-in
        yield return FadeInRoutine();

        // Kurze Zeit stehen bleiben
        yield return new WaitForSecondsRealtime(displayTime);

        // Fade-out and Scale-out
        yield return FadeOutRoutine();
    }

    private IEnumerator FadeAndScale(float fromAlpha, float toAlpha, Vector3 fromScale, Vector3 toScale, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float normalized = t / duration;
            canvasGroup.alpha = Mathf.Lerp(fromAlpha, toAlpha, normalized);
            transform.localScale = Vector3.Lerp(fromScale, toScale, normalized);
            yield return null;
        }
        canvasGroup.alpha = toAlpha;
        transform.localScale = toScale;
    }
}
