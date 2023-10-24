using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Toggle aiToggle;

    private void Start()
    {
        aiToggle.isOn = PlayerPrefs.GetInt("AI", 1) == 1;
    }

    public void StartGame()
    {
        // Carga la escena por nombre; es más claro que por índice.
        SceneManager.LoadScene("Game"); // Cambia "GameScene" por el nombre de tu escena de juego si es diferente.
    }

    public void AiToggleChange()
    {
        PlayerPrefs.SetInt("AI", aiToggle.isOn ? 1 : 0);
    }
}
