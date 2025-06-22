using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages main menu interactions.
/// </summary>
public class MenuManager : MonoBehaviour
{
    /// <summary>
    /// Starts the game by loading the game scene.
    /// </summary>
    public void Play()
    {
        SceneManager.LoadScene("Game");
    }

    /// <summary>
    /// Quits the application.
    /// </summary>
    public void Exit()
    {
        Application.Quit();
    }
}