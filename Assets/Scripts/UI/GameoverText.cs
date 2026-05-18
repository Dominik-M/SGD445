using UnityEngine;

public class GameoverText : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI text;

    void Update()
    {
        text.gameObject.SetActive(GameController.I.IsGameOver);
        text.text = GameController.I.IsGameComplete ? "Game Complete!" : "Game Over";
    }
}
