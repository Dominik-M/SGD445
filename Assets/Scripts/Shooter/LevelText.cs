using UnityEngine;

namespace Shooter
{
    public class LevelText : MonoBehaviour
    {
        [Header("Referenz auf den Gegner Spawner")]
        [SerializeField] private GegnerSpawner spawner;

        private TMPro.TextMeshProUGUI text;
        void Start()
        {
            text = GetComponent<TMPro.TextMeshProUGUI>();
        }

        void Update()
        {
            text.text = $"Level {spawner.WaveNumber.ToString()}\r\n" +
                $"{spawner.GegnerAnzahl.ToString()} Gegner übrig\r\n" +
                $"Nächste Welle in {spawner.WaveTimeRemain.ToString("F1")}s";
        }
    }
}