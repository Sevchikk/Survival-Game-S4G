using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages in-game UI buttons and scene transitions.
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject panelSettings;

    private void Start()
    {
        // Initialize settings panel
        panelSettings.SetActive(false);
    }

    /// <summary>
    /// Loads the main menu scene.
    /// </summary>
    public void Back()
    {
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Quits the application.
    /// </summary>
    public void Exit()
    {
        Application.Quit();
    }

    /// <summary>
    /// Toggles the settings panel.
    /// </summary>
    public void Settings()
    {
        panelSettings.SetActive(!panelSettings.activeSelf);
    }
}