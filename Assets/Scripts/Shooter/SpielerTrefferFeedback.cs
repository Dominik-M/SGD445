using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Shooter
{
    public class SpielerTrefferFeedback : MonoBehaviour
    {
        public float maxAlpha = 0.4f;
        public float fadeDuration = 0.2f;
        public TMPro.TextMeshProUGUI deathDisplay;
        private Spieler spieler;

        private Image image;
        private CanvasGroup _canvasGroup;
        private Coroutine fadeRoutine;
        void Start()
        {
            spieler = Spieler.Instance;
            if (spieler != null)
            {
                var mDC = spieler.GetComponent<DamageController>();
                if (mDC != null)
                {
                    mDC.OnDamage += GetHit;
                    mDC.OnDeath += Die;
                }
            }
            image = GetComponent<Image>();
            _canvasGroup = GetComponent<CanvasGroup>();
            // unsichtbar starten
            _canvasGroup.alpha = 0;
            deathDisplay.gameObject.SetActive(false);
        }

        void GetHit(float dmg)
        {
            if (dmg == 0f) return;
            if (dmg > 0f) image.color = Color.red;
            if (dmg < 0f) image.color = Color.green;

            if (fadeRoutine != null) StopCoroutine(fadeRoutine);
            fadeRoutine = StartCoroutine(Fade(maxAlpha, 0, fadeDuration));
        }

        void Die()
        {
            if (fadeRoutine != null) StopCoroutine(fadeRoutine);
            _canvasGroup.alpha = maxAlpha;
            deathDisplay.gameObject.SetActive(true);
        }

        private IEnumerator Fade(float fromAlpha, float toAlpha, float duration)
        {
            float t = 0f;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                float normalized = t / duration;
                _canvasGroup.alpha = Mathf.Lerp(fromAlpha, toAlpha, normalized);
                yield return null;
            }
            _canvasGroup.alpha = toAlpha;
        }
    }
}