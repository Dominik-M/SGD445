using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;

public class CreditsSceneController : MonoBehaviour
{
    public GameObject[] elements;
    public float timeBetweenElements = 2;
    public float fadeTime = 0.5f;
    public int menuSceneIndex = 0;

    private System.IDisposable _listener;
    private Coroutine creditsRoutine;
    private float startTime;

    private void Start()
    {
        startTime = Time.realtimeSinceStartup;
        creditsRoutine = StartCoroutine(PlayCredits());
    }

    private void OnEnable()
    {
        _listener = InputSystem.onAnyButtonPress
            .Call(control => Continue(control));
    }

    private void OnDisable()
    {
        _listener?.Dispose();
        if (creditsRoutine != null)
            StopCoroutine(creditsRoutine);
    }

    private void Continue(InputControl control)
    {
        Debug.Log($"Taste gedrückt: {control.displayName} ({control.device.displayName})");
        if (Time.realtimeSinceStartup < startTime + 5)
        {
            Debug.Log("Schnelle Eingaben entprellt");
            return;
        }
        SceneManager.LoadScene(menuSceneIndex);
    }

    void HideAll()
    {
        foreach (var element in elements)
        {
            if (element != null) element.SetActive(false);
        }
    }

    private IEnumerator PlayCredits()
    {
        HideAll();
        foreach (var element in elements)
        {
            yield return new WaitForSeconds(timeBetweenElements);
            if (element != null)
            {
                element.SetActive(true);
                CanvasGroup canvasGroup = element.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = element.AddComponent<CanvasGroup>();
                }
                yield return FadeIn(canvasGroup);
            }
        }
    }
    private IEnumerator FadeIn(CanvasGroup canvasGroup)
    {
        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.unscaledDeltaTime;
            float normalized = t / fadeTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, normalized);
            yield return null;
        }
        canvasGroup.alpha = 1;
    }
}
