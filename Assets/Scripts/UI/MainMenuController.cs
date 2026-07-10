using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public GameObject loadingScreen;

    private bool loading = false;
    private int sceneIdx = 0;

    private void Update()
    {
        if (loadingScreen) loadingScreen.SetActive(loading);
    }

    public void OnMenuButtonClicked(int idx)
    {
        if (!loading)
        {
            loading = true;
            sceneIdx = idx;
            Invoke(nameof(LoadScene), 0.1f); // delay scene switch to enable loading screen first
        }
    }

    void LoadScene()
    {
        Debug.Log("LoadScene(): " + sceneIdx);
        SceneManager.LoadScene(sceneIdx);
        loading = false;
    }

    public void OnDeleteButtonClicked()
    {
        SaveDataHandler.Delete();
    }
}
