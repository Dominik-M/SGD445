using System.Collections;
using UnityEngine;

namespace PointClick
{
    public class NumPadButtonController : MonoBehaviour, IInteractable
    {
        [Header("Meine Ziffer (0-9) -1 = ENTER")]
        [SerializeField] private int digit;
        [Header("Referenz auf das Raetsel")]
        [SerializeField] private ZahlenRaetsel raetsel;
        [Header("Referenz auf den Zahlentext")]
        [SerializeField] private TMPro.TextMeshPro numText;

        // Definiere Buttons mit Ziffer kleiner 0 als der "OK" Button
        bool IsConfirmButton => digit < 0;

        private Coroutine animationRoutine;
        private Vector3 normalPos;

        public void OnCursorEnter()
        {
            if (IsConfirmButton)
                Tooltip.Show($"Bestätigen");
            else
                Tooltip.Show("Wähle die " + digit);
        }

        public void OnCursorExit()
        {
            Tooltip.Hide();
        }

        public void OnInteract()
        {
            if (IsConfirmButton)
                raetsel.Check();
            else
                raetsel.EnterNumber(digit);
            if (animationRoutine != null) StopCoroutine(animationRoutine);
            animationRoutine = StartCoroutine(ButtonPressAnimation());
        }

        void Start()
        {
            normalPos = transform.localPosition;
            if (IsConfirmButton)
                numText.text = "OK";
            else if (digit < 10)
                numText.text = digit.ToString();
            else
                Debug.LogError("Invalid number on Num Pad Button: " + digit);
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