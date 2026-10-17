using Shooter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GegnerSpawner : MonoBehaviour
{
    [System.Serializable]
    public class GegnerTyp
    {
        public string name;
        public DamageController prefab;
        [Min(1)] public int kosten = 1;
        [Min(1)] public int ersteWelle = 1;   // ab dieser Welle verfügbar
        [Min(0)] public float gewicht = 1f;   // Häufigkeit bei der Auswahl
    }
    [Header("Gegnertypen")]
    [SerializeField] private List<GegnerTyp> gegnerTypen = new List<GegnerTyp>();
    [SerializeField] private int basisBudget = 3;
    [SerializeField] private float budgetProWelle = 2f;
    [SerializeField] private int maxGleichzeitig = 30;
    [Header("Gegner Spawn Positionen")]
    [SerializeField] private Transform[] spawnPunkte;
    [Header("Mindest Abstand zum Spieler")]
    [SerializeField] private float minAbstand = 15f;
    [Header("Spawn Balancing")]
    [SerializeField] private float timeBetweenWaves = 20f;
    [SerializeField] private float timeBetweenSpawns = 1f;

    private Coroutine spawnRoutine;
    private int waveNumber = 0;
    public int WaveNumber => waveNumber;
    private float waveTimeRemain;
    public float WaveTimeRemain => waveTimeRemain;
    private List<DamageController> gegnerSpawned = new List<DamageController>();
    public int GegnerAnzahl => gegnerSpawned.Count;

    private void Start()
    {
        spawnRoutine = StartCoroutine(SpawnGegner());
    }

    private void OnDisable()
    {
        if (spawnRoutine != null) StopCoroutine(spawnRoutine);
    }

    int WellenBudget(int welle) => basisBudget + Mathf.RoundToInt(welle * budgetProWelle);
    bool SpielerLebt() => Spieler.Instance != null && !Spieler.Instance.Leben.IsDead();

    IEnumerator SpawnGegner()
    {
        yield return new WaitForSeconds(1);
        while (SpielerLebt())
        {
            waveNumber++;
            waveTimeRemain = timeBetweenWaves;
            Debug.Log("Starte Welle " + waveNumber);
            int budget = WellenBudget(waveNumber);

            while (budget > 0 && SpielerLebt())
            {
                // Limit gleichzeitiger Gegner einhalten
                while (GegnerAnzahl >= maxGleichzeitig && SpielerLebt())
                    yield return null;

                GegnerTyp typ = WaehleTyp(budget, waveNumber);
                if (typ == null) break;           // nichts Passendes mehr im Budget

                budget -= typ.kosten;
                Transform spawnPunkt = WaehleSpawnPunkt();
                DamageController gegner = Instantiate(typ.prefab, spawnPunkt.position, spawnPunkt.rotation);
                gegnerSpawned.Add(gegner);
                gegner.OnDeath += () => gegnerSpawned.Remove(gegner);
                yield return new WaitForSeconds(timeBetweenSpawns);
            }

            while (waveTimeRemain > 0 && GegnerAnzahl > 0 && SpielerLebt())
            {
                yield return null;
                waveTimeRemain -= Time.deltaTime;
            }
        }
    }

    Transform WaehleSpawnPunkt()
    {
        Vector3 sp = Spieler.Instance.transform.position;
        float minSqr = minAbstand * minAbstand;

        // Zufällig durchprobieren, ersten ausreichend weit entfernten nehmen
        int start = Random.Range(0, spawnPunkte.Length);
        for (int i = 0; i < spawnPunkte.Length; i++)
        {
            Transform p = spawnPunkte[(start + i) % spawnPunkte.Length];
            if ((p.position - sp).sqrMagnitude >= minSqr)
                return p;
        }
        // Alle zu nah (Spieler steht mittendrin): irgendeinen nehmen
        return spawnPunkte[start];
    }

    GegnerTyp WaehleTyp(int budget, int welle)
    {
        List<GegnerTyp> verfuegbar = new List<GegnerTyp>();
        float summe = 0f;
        foreach (var t in gegnerTypen)
        {
            if (t.prefab == null || t.ersteWelle > welle || t.kosten > budget) continue;
            verfuegbar.Add(t);
            summe += t.gewicht;
        }
        if (verfuegbar.Count == 0) return null;

        float wurf = Random.value * summe;
        foreach (var t in verfuegbar)
        {
            wurf -= t.gewicht;
            if (wurf <= 0f) return t;
        }
        return verfuegbar[verfuegbar.Count - 1];
    }
}
