using PointClick;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSwitchTrigger : MonoBehaviour
{
    public int sceneBuildIndex;
    public Vector3 resetPosition;
    public GameObject loadingScreen;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Spieler betritt LevelSwitchTrigger index=" + sceneBuildIndex);
            loadingScreen.SetActive(true);
            // Den Rücksetzpunkt setzen und Welt Daten speichern
            Inventar.Instance.Player.transform.position = resetPosition;
            Inventar.Instance.SaveAllData();
            // Szene verzögert laden, damit der Ladescreen vorher zu sehen ist
            Invoke(nameof(LoadScene), 0.5f);
        }
    }

    private void LoadScene()
    {
        SceneManager.LoadScene(sceneBuildIndex);
    }
}
