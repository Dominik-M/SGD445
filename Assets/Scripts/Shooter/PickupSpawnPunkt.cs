using UnityEngine;

namespace Shooter
{
    public class PickupSpawnPunkt : MonoBehaviour
    {
        [Header("Zeit bis ein neues Pickup erscheint nach dem Aufsammeln")]
        [SerializeField] private float respawnZeit = 15f;
        public float RespawnZeit => respawnZeit;

        [Header("Prefab für das Pickup Objekt")]
        [SerializeField] private GameObject pickupPrefab;
        public GameObject PickupPrefab => pickupPrefab;

        private GameObject pickupObjekt;
        private float timeToSpawn = 0;

        private void Update()
        {
            // Der Vergleich Operator ist bei GameObjects überladen und prüft auch
            // ob das Objekt zerstört wurde, was beim Aufsammeln der Fall ist
            if (pickupObjekt == null)
            {
                timeToSpawn -= Time.deltaTime;
                if (timeToSpawn <= 0)
                    SpawnPickup();
            }
        }

        public virtual GameObject SpawnPickup()
        {
            pickupObjekt = Instantiate(pickupPrefab.gameObject, transform);
            // Pickup Position leicht über dem Spawn Punkt setzen
            pickupObjekt.transform.localPosition = new Vector3(0, 1, 0);
            timeToSpawn = respawnZeit;
            return pickupObjekt;
        }
    }
}