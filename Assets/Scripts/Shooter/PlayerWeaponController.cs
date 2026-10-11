using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Shooter
{
    public class PlayerWeaponController : MonoBehaviour
    {
        [Header("Inputs")]
        [SerializeField] private InputActionReference FireInput;
        [SerializeField] private InputActionReference ReloadInput;
        [SerializeField] private InputActionReference NextInput;
        [SerializeField] private InputActionReference PrevInput;

        [Header("Meine Kamera")]
        [SerializeField] Camera spielerKamera;
        [Header("Mein Zielobjekt")]
        [SerializeField] private Transform ziel;
        [Header("Laenge der Waffenwechsel Animation")]
        [SerializeField] private float waffenWechselZeit = 0.5f;
        [Header("Alle meine Waffen")]
        [SerializeField] private WeaponController[] alleWaffen;

        private int aktuelleWaffeIndex = 0;
        private bool amWechseln = false;
        public WeaponController AktuelleWaffe => alleWaffen[aktuelleWaffeIndex];
        private DamageController mDC;

        // ── Lifecycle ─────────────────────────────────────────────────────────────
        void OnEnable()
        {
            FireInput.action.Enable();
            ReloadInput.action.Enable();
            PrevInput.action.Enable();
            NextInput.action.Enable();
        }

        void OnDisable()
        {
            FireInput.action.Disable();
            ReloadInput.action.Disable();
            PrevInput.action.Disable();
            NextInput.action.Disable();
        }

        void Update()
        {
            // Ziel Position setzen
            RaycastHit hit;
            if (Physics.Raycast(spielerKamera.transform.position, spielerKamera.transform.forward, out hit, 100))
            {
                ziel.position = hit.point;
            }
            else
            {
                // Nichts getroffen, einfach irgendwo mittig weit entfernt setzen
                ziel.localPosition = new Vector3(0, 0, 100);
            }

            // Inputs ignorieren während Waffenwechsel
            if (amWechseln) return;

            // Alle Inputs einsammeln
            bool fire = FireInput.action.IsPressed();
            bool reload = ReloadInput.action.IsPressed();
            bool prev = PrevInput.action.IsPressed();
            bool next = NextInput.action.IsPressed();

            AktuelleWaffe.Firing = fire;
            if (prev) PrevWpn();
            else if (next) NextWpn();
        }

        void Start()
        {
            mDC = GetComponent<DamageController>();
            foreach (var wpn in alleWaffen)
            {
                wpn.Target = ziel;
            }
            // Startwaffe Munition geben
            AktuelleWaffe.AddMunition(32);
        }

        void PrevWpn()
        {
            int neuIndex = aktuelleWaffeIndex - 1;
            if (neuIndex < 0) neuIndex = alleWaffen.Length - 1;
            // TODO noch prüfen ob die neue Waffe freigeschaltet ist
            WechselWaffe(neuIndex);
        }

        void NextWpn()
        {
            int neuIndex = aktuelleWaffeIndex + 1;
            if (neuIndex >= alleWaffen.Length) neuIndex = 0;
            // TODO noch prüfen ob die neue Waffe freigeschaltet ist
            WechselWaffe(neuIndex);
        }

        public void WechselWaffe(int index)
        {
            if (index < 0 || index >= alleWaffen.Length) return;
            if (index == aktuelleWaffeIndex) return;
            if (amWechseln) return;
            // Schlossvariable synchron setzen
            amWechseln = true;
            // Schießen abbrechen
            AktuelleWaffe.Firing = false;
            StartCoroutine(WaffenWechselRoutine(aktuelleWaffeIndex, index));
            aktuelleWaffeIndex = index;
        }

        IEnumerator WaffenWechselRoutine(int altIndex, int neuIndex)
        {
            // Objekte und Positionen definieren
            Transform alt = alleWaffen[altIndex].transform;
            Transform neu = alleWaffen[neuIndex].transform;
            Vector3 offPosition = new Vector3(0, -0.4f, 0);
            Vector3 onPosition = Vector3.zero;

            // Erst alte Waffe runter animieren
            float t = 0;
            float duration = waffenWechselZeit / 2;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                float normalized = t / duration;
                alt.localPosition = Vector3.Lerp(onPosition, offPosition, normalized);
                yield return null;
            }
            // Objekte umschalten
            alt.gameObject.SetActive(false);
            neu.gameObject.SetActive(true);
            // Dann neue Waffe hoch animieren
            t = 0;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                float normalized = t / duration;
                neu.localPosition = Vector3.Lerp(offPosition, onPosition, normalized);
                yield return null;
            }

            // Routine beendet, Schlossvariable freigeben
            amWechseln = false;
        }
    }
}