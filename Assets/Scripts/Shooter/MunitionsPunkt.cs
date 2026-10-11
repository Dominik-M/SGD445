using UnityEngine;

namespace Shooter
{
    public class MunitionsPunkt : PickupSpawnPunkt
    {
        [Header("Menge an Munition, die das Pickup geben soll")]
        [SerializeField] private int munitionsZuwachs = 8;

        [Header("Typ der Munition, die das Pickup geben soll")]
        [SerializeField] private Munition munition;

        public override GameObject SpawnPickup()
        {
            // Basismethode aufrufen zum Erzeugen des Objekts
            var pickupObjekt = base.SpawnPickup();
            // Hier setzen wir noch zusätzlich die Munitions Parameter
            MunitionPickUp muni = pickupObjekt.GetComponent<MunitionPickUp>();
            if (muni == null)
                muni = pickupObjekt.AddComponent<MunitionPickUp>();
            muni.munition = munition;
            muni.munitionsZuwachs = munitionsZuwachs;
            return pickupObjekt;
        }
    }
}