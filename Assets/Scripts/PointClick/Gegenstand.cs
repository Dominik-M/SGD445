using UnityEngine;

namespace PointClick
{
    [CreateAssetMenu(fileName = "Gegenstand", menuName = "Scriptable Objects/Gegenstand")]
    public class Gegenstand : ScriptableObject
    {
        public string bedingung;
        public int maxAnzahl;
        public Sprite icon;
        public GameObject prefab;

        public override string ToString()
        {
            return name;
        }

        public bool Equals(Gegenstand other)
        {
            return name.Equals(other.name);
        }
    }
}