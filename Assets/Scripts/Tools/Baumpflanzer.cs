
using UnityEngine;

public class Baumpflanzer : GeneratorBehaviour
{
    [Header("Die Baum ID im Terrain")]
    [SerializeField] private int baumNummer = 0;
    [Header("Abstand zwischen Bäumen")]
    [SerializeField] private int baumAbstand = 1;
    [Header("Breite der Baumreihe entlang der X-Achse")]
    [SerializeField] private int breite;
    [Header("Länge der Baumreihe entlang der Z-Achse")]
    [SerializeField] private int laenge;

    public override void Generate()
    {
        Terrain terrain = Terrain.activeTerrain;
        TerrainData terrainData = terrain.terrainData;
        Vector3 startPosition = new Vector3(
            transform.position.x / terrainData.size.x,
            transform.position.y / terrainData.size.y,
            transform.position.z / terrainData.size.z);
        for (int xi = 0; xi < breite; xi++)
        {
            for (int zi = 0; zi < laenge; zi++)
            {
                TreeInstance baum = terrainData.treeInstances[baumNummer];
                Vector3 position = startPosition;
                position.x += (0.5f + xi) * baumAbstand / terrainData.size.x;
                position.z += (0.5f + zi) * baumAbstand / terrainData.size.z;
                baum.position = position;
                terrain.AddTreeInstance(baum);
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 position = transform.position;
        Vector3 cubeSize = new Vector3(breite * baumAbstand, 1, laenge * baumAbstand);
        UnityEditor.Handles.DrawWireCube(position + cubeSize / 2, cubeSize);
    }
#endif
}
