using System.Collections;
using UnityEngine;

namespace PointClick
{
    public class NumPadButtonController : MonoBehaviour, IInteractable
    {
        [Header("Meine Zahl (0-9) -1 = ENTER")]
        [SerializeField] private int number;
        [Header("Referenz auf das Raetsel")]
        [SerializeField] private ZahlenRaetsel raetsel;
        [Header("Referenz auf den Zahlentext")]
        [SerializeField] private TMPro.TextMeshPro numText;

        private Coroutine animationRoutine;
        private Vector3 normalPos;

        public void OnCursorEnter()
        {
            if (number < 0)
                Tooltip.Show($"Bestätigen");
            else
                Tooltip.Show("Wähle die " + number);
        }

        public void OnCursorExit()
        {
            Tooltip.Hide();
        }

        public void OnInteract()
        {
            if (number < 0)
                raetsel.Check();
            else
                raetsel.EnterNumber(number);
            if (animationRoutine != null) StopCoroutine(animationRoutine);
            animationRoutine = StartCoroutine(ButtonPressAnimation());
        }

        void Start()
        {
            normalPos = transform.localPosition;
            if (number < 0)
                numText.text = "OK";
            else if (number < 10)
                numText.text = number.ToString();
            else
                Debug.LogError("Invalid number on Num Pad Button: " + number);
        }

        IEnumerator ButtonPressAnimation()
        {
            float t = 0;
            float duration = 0.3f;
            Vector3 pressPos = new Vector3(normalPos.x, normalPos.y, 0.01f);
            while (t < duration / 2)
            {
                float normalized = t / duration;
                transform.localPosition = Vector3.Lerp(normalPos, pressPos, normalized);
                t += Time.deltaTime;
                yield return null;
            }
            t = 0;
            while (t < duration)
            {
                float normalized = t / duration;
                transform.localPosition = Vector3.Lerp(pressPos, normalPos, normalized);
                t += Time.deltaTime;
                yield return null;
            }
            animationRoutine = null;
        }
    }
}