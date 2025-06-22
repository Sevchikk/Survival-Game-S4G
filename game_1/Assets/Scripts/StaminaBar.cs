using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Displays the player's stamina with a smooth transition effect.
/// </summary>
public class StaminaBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Character player;

    [Header("Stamina Settings")]
    [SerializeField] private float maxStamina = 20f;

    [Header("Animation Settings")]
    [SerializeField] private float transitionDuration = 0.2f;

    private Image staminaBar;
    private float targetFillAmount;
    private Coroutine transitionCoroutine;

    private void Start()
    {
        // Cache components
        staminaBar = GetComponent<Image>();
        if (staminaBar == null)
        {
            Debug.LogError("Image component missing on StaminaBar!", this);
            enabled = false;
            return;
        }

        // Find player if not assigned
        if (player == null)
        {
            player = FindFirstObjectByType<Character>();
            if (player == null)
            {
                Debug.LogError("Player not found in StaminaBar!", this);
                enabled = false;
                return;
            }
        }

        // Initialize UI
        targetFillAmount = player.GetStamina() / maxStamina;
        staminaBar.fillAmount = targetFillAmount;
    }

    private void Update()
    {
        UpdateStaminaBar();
    }

    /// <summary>
    /// Updates the stamina bar UI.
    /// </summary>
    private void UpdateStaminaBar()
    {
        if (player != null)
        {
            float currentStamina = player.GetStamina();
            float newFillAmount = currentStamina / maxStamina;

            if (!Mathf.Approximately(newFillAmount, targetFillAmount))
            {
                targetFillAmount = newFillAmount;
                if (transitionCoroutine != null)
                {
                    StopCoroutine(transitionCoroutine);
                }
                transitionCoroutine = StartCoroutine(SmoothTransition(newFillAmount));
            }
        }
    }

    /// <summary>
    /// Smoothly transitions the stamina bar to the target fill amount.
    /// </summary>
    private IEnumerator SmoothTransition(float targetFill)
    {
        float startFill = staminaBar.fillAmount;
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / transitionDuration;
            staminaBar.fillAmount = Mathf.Lerp(startFill, targetFill, t);
            yield return null;
        }

        staminaBar.fillAmount = targetFill;
        transitionCoroutine = null;
    }
}