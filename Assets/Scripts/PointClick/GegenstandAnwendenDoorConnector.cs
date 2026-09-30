using PointClick;
using UnityEngine;

public class GegenstandAnwendenDoorConnector : MonoBehaviour
{
    private DoorController door;
    private GegenstandAnwenden gegenstandAnwenden;
    void Start()
    {
        door = GetComponent<DoorController>();
        gegenstandAnwenden = GetComponent<GegenstandAnwenden>();
        gegenstandAnwenden.ZustandGeandert += GegenstandAnwenden_ZustandGeandert;
        // Initial Call
        GegenstandAnwenden_ZustandGeandert(gegenstandAnwenden.Zustand);
    }

    private void GegenstandAnwenden_ZustandGeandert(int zustand)
    {
        door.Open = zustand > 0;
    }
}
