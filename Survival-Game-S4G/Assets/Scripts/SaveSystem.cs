using UnityEngine;

/// <summary>
/// Saves and loads player state using PlayerPrefs.
/// </summary>
public class SaveSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Character player;

    [Header("Saved Parameters")]
    [SerializeField] private float defaultStamina = 20f;
    [SerializeField] private float defaultFear = 0f;
    [SerializeField] private float defaultHP = 20f;
    [SerializeField] private float defaultDash = 0f;
    [SerializeField] private float defaultTime = 120f;
    [SerializeField] private int defaultCollected = 0;

    private const string POSITION_X_KEY = "X";
    private const string POSITION_Y_KEY = "Y";
    private const string POSITION_Z_KEY = "Z";
    private const string STAMINA_KEY = "Stamina";
    private const string FEAR_KEY = "Fear";
    private const string HP_KEY = "HP";
    private const string DASH_KEY = "Dash";
    private const string TIME_KEY = "Time";
    private const string COLLECTED_KEY = "Collected";

    private float saveTimer;
    private const float saveInterval = 1f;

    private void Start()
    {
        // Find player if not assigned
        if (player == null)
        {
            player = FindFirstObjectByType<Character>();
            if (player == null)
            {
                Debug.LogError("Character not found in SaveSystem!", this);
                enabled = false;
                return;
            }
        }
        Load();
    }

    private void Update()
    {
        // Save state periodically
        saveTimer += Time.deltaTime;
        if (saveTimer >= saveInterval)
        {
            SaveCurrentState();
            saveTimer = 0f;
        }
    }

    /// <summary>
    /// Saves the current player state to PlayerPrefs.
    /// </summary>
    public void SaveCurrentState()
    {
        PlayerPrefs.SetFloat(POSITION_X_KEY, transform.position.x);
        PlayerPrefs.SetFloat(POSITION_Y_KEY, transform.position.y);
        PlayerPrefs.SetFloat(POSITION_Z_KEY, transform.position.z);
        PlayerPrefs.SetFloat(STAMINA_KEY, player.GetStamina());
        PlayerPrefs.SetFloat(FEAR_KEY, player.GetFear());
        PlayerPrefs.SetFloat(HP_KEY, player.GetHP());
        PlayerPrefs.SetFloat(DASH_KEY, player.GetDash());
        PlayerPrefs.SetFloat(TIME_KEY, player.GetTime());
        PlayerPrefs.SetInt(COLLECTED_KEY, player.GetCollected());
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Loads the saved player state from PlayerPrefs.
    /// </summary>
    private void Load()
    {
        Vector3 loadedPosition = transform.position;
        if (PlayerPrefs.HasKey(POSITION_X_KEY)) loadedPosition.x = PlayerPrefs.GetFloat(POSITION_X_KEY);
        if (PlayerPrefs.HasKey(POSITION_Y_KEY)) loadedPosition.y = PlayerPrefs.GetFloat(POSITION_Y_KEY);
        if (PlayerPrefs.HasKey(POSITION_Z_KEY)) loadedPosition.z = PlayerPrefs.GetFloat(POSITION_Z_KEY);
        transform.position = loadedPosition;

        if (player != null)
        {
            player.SetStamina(PlayerPrefs.HasKey(STAMINA_KEY) ? PlayerPrefs.GetFloat(STAMINA_KEY) : defaultStamina);
            player.SetFear(PlayerPrefs.HasKey(FEAR_KEY) ? PlayerPrefs.GetFloat(FEAR_KEY) : defaultFear);
            player.SetHP(PlayerPrefs.HasKey(HP_KEY) ? PlayerPrefs.GetFloat(HP_KEY) : defaultHP);
            player.SetDash(PlayerPrefs.HasKey(DASH_KEY) ? PlayerPrefs.GetFloat(DASH_KEY) : defaultDash);
            player.SetTime(PlayerPrefs.HasKey(TIME_KEY) ? PlayerPrefs.GetFloat(TIME_KEY) : defaultTime);
            player.SetCollected(PlayerPrefs.HasKey(COLLECTED_KEY) ? PlayerPrefs.GetInt(COLLECTED_KEY) : defaultCollected);
        }
    }
}