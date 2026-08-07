
using System.Collections.Generic;
using UnityEngine;

public class MauerGenerator : GeneratorBehaviour
{
    [Header("Das Wuerfel Prefab")]
    [SerializeField] private GameObject cubePrefab;
    [Header("Breite der Mauer entlang der X-Achse")]
    [SerializeField] private int breite;
    [Header("Hoehe der Mauer entlang der Y-Achse")]
    [SerializeField] private int hoehe;

    public Vector3 GetCubeSize()
    {
        if (cubePrefab.TryGetComponent<Renderer>(out var cube)) return cube.bounds.size;
        return Vector3.one;
    }

    public Vector3[] GetPositionsInWall()
    {
        List<Vector3> positions = new();
        Vector3 cubeSize = GetCubeSize();
        for (int x = 0; x < breite; x++)
            for (int y = 0; y < hoehe; y++)
            {
                Vector3 position = transform.position + new Vector3(x * cubeSize.x, y * cubeSize.y, 0);
                positions.Add(position);
            }
        return positions.ToArray();
    }

    public override void Generate()
    {
        Transform parent = Instantiate(gameObject, transform.parent).transform;
        Vector3 cubeSize = GetCubeSize();
        foreach (var position in GetPositionsInWall())
        {
            var cube = Instantiate(cubePrefab, parent);
            cube.transform.localPosition = position;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 cubeSize = GetCubeSize();
        UnityEditor.Handles.color = new Color(0, 1, 1, 0.1f);
        foreach (var position in GetPositionsInWall())
            UnityEditor.Handles.DrawWireCube(position, cubeSize);
    }
#endif
}
