using Shooter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GegnerSpawner : MonoBehaviour
{
    [Header("Prefab für das Gegner Objekt")]
    [SerializeField] private DamageController gegnerPrefab;
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

    IEnumerator SpawnGegner()
    {
        yield return new WaitForSeconds(1);
        while (SpielerLebt())
        {
            waveNumber++;
            waveTimeRemain = timeBetweenWaves;
            int anz = waveNumber + 1;
            Debug.Log("Starte Welle " + waveNumber);
            for (int i = 0; i < anz && SpielerLebt(); i++)
            {
                Transform spawnPunkt = WaehleSpawnPunkt();
                DamageController gegner = Instantiate(gegnerPrefab, spawnPunkt.position, spawnPunkt.rotation);
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

    bool SpielerLebt() => Spieler.Instance != null && !Spieler.Instance.Leben.IsDead();

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
}
