using UnityEngine;

namespace PointClick
{
    [CreateAssetMenu(fileName = "Gegenstand", menuName = "Scriptable Objects/Gegenstand")]
    public class Gegenstand : ScriptableObject
    {
        public string bedingung;
        public int anzahl;
        public int maxAnzahl;
        public Sprite icon;
        public GameObject prefab;

        public override string ToString()
        {
            if (anzahl > 1)
                return name + "\nx" + anzahl.ToString();
            else return name;
        }

        public bool Equals(Gegenstand other)
        {
            return name.Equals(other.name);
        }
    }
}