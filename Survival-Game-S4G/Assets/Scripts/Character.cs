using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Controls the player's behavior, including movement, stats, UI updates, and interactions.
/// </summary>
[RequireComponent(typeof(CharacterController), typeof(Animator))]
public class Character : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Image healthIcon;
    [SerializeField] private Image fearIcon;
    [SerializeField] private Image dashIcon;
    [SerializeField] private TMP_Text collectedItemsText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text gameOver;

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 10f;
    [SerializeField] private float runSpeed = 20f;
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashTime = 0.2f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Player Stats")]
    [SerializeField] private float health = 20f;
    [SerializeField] private float stamina = 20f;
    [SerializeField] private float fear = 0f;
    [SerializeField] private float dash = 0f;
    [SerializeField] private float runStaminaCost = 2f;
    [SerializeField] private float maxDash = 3f;
    [SerializeField] private float maxStamina = 20f;
    [SerializeField] private float maxHealth = 20f;
    [SerializeField] private float maxFear = 20f;
    [SerializeField] private Sprite[] healthSprites;
    [SerializeField] private Sprite[] fearSprites;
    [SerializeField] private Sprite[] dashSprites;
    [SerializeField] private float gameDuration = 120f;
    [SerializeField] private Vector3 respawnPoint = new Vector3(1f, 4f, 1f);
    [SerializeField] private SaveSystem saveSystem;

    public GameObject DeathPanel;
    public GameObject gameOverPanel;
    public int collectedItems = 0;

    private Vector3 velocity;
    private bool isDashing;
    private bool isRunning;
    private float timeRemaining;
    private float lastDamageTime;
    private bool isRegeneratingStamina;
    private Animator animator;
    private bool isDead;

    /// <summary>
    /// Gets the current fear value.
    /// </summary>
    public float GetFear() => fear;

    /// <summary>
    /// Sets the fear value and updates the UI.
    /// </summary>
    public void SetFear(float newFear)
    {
        fear = Mathf.Clamp(newFear, 0f, maxFear);
        UpdateFearIcon();
        lastDamageTime = Time.time;
    }

    /// <summary>
    /// Gets the current stamina value.
    /// </summary>
    public float GetStamina() => stamina;

    /// <summary>
    /// Sets the stamina value.
    /// </summary>
    public void SetStamina(float newStamina)
    {
        stamina = Mathf.Clamp(newStamina, 0f, maxStamina);
    }

    /// <summary>
    /// Gets the current health value.
    /// </summary>
    public float GetHP() => health;

    /// <summary>
    /// Sets the health value and updates the UI.
    /// </summary>
    public void SetHP(float newHP)
    {
        health = Mathf.Clamp(newHP, 0f, maxHealth);
        UpdateHealthIcon();
    }

    /// <summary>
    /// Gets the current dash count.
    /// </summary>
    public float GetDash() => dash;

    /// <summary>
    /// Sets the dash count and updates the UI.
    /// </summary>
    public void SetDash(float newDash)
    {
        dash = Mathf.Clamp(newDash, 0f, maxDash);
        UpdateDashIcon();
    }

    /// <summary>
    /// Gets the remaining game time.
    /// </summary>
    public float GetTime() => timeRemaining;

    /// <summary>
    /// Sets the remaining game time and updates the UI.
    /// </summary>
    public void SetTime(float newTime)
    {
        timeRemaining = Mathf.Clamp(newTime, 0f, gameDuration);
        UpdateTimerText();
    }

    /// <summary>
    /// Gets the number of collected items.
    /// </summary>
    public int GetCollected() => collectedItems;

    /// <summary>
    /// Sets the number of collected items and updates the UI.
    /// </summary>
    public void SetCollected(int newCollected)
    {
        collectedItems = Mathf.Max(0, newCollected);
        UpdateCollectedItemsText();
    }

    private void Start()
    {
        // Cache components
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        saveSystem = GetComponent<SaveSystem>();

        // Validate components
        if (characterController == null)
        {
            Debug.LogError("CharacterController component is missing on the player!", this);
            enabled = false;
            return;
        }
        if (animator == null)
        {
            Debug.LogError("Animator component is missing on the player!", this);
            enabled = false;
            return;
        }

        // Initialize state
        DeathPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        timeRemaining = gameDuration;
        lastDamageTime = Time.time;

        // Start coroutines
        StartCoroutine(GameTimer());
        StartCoroutine(FearReduction());

        // Update UI
        UpdateHealthIcon();
        UpdateFearIcon();
        UpdateDashIcon();
        UpdateCollectedItemsText();
    }

    private void Update()
    {
        // Handle core gameplay mechanics
        HandleMovement();
        HandleGravity();
        RegenerateStaminaIfNeeded();
        DeathMenu();
        UpdateTimerText();
        HandleFearDamage();
    }

    /// <summary>
    /// Applies damage over time when fear is at maximum.
    /// </summary>
    private void HandleFearDamage()
    {
        if (fear >= maxFear)
        {
            SetHP(health - 0.5f * Time.deltaTime);
        }
    }

    /// <summary>
    /// Reduces fear over time if no recent damage was taken.
    /// </summary>
    private IEnumerator FearReduction()
    {
        while (true)
        {
            if (Time.time - lastDamageTime >= 10f && fear > 0)
            {
                SetFear(fear - 1f);
            }
            yield return new WaitForSeconds(10f);
        }
    }

    /// <summary>
    /// Handles player movement, including walking, running, and dashing.
    /// </summary>
    private void HandleMovement()
    {
        Vector3 move = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        // Reset running state if no movement or stamina is depleted
        if (move == Vector3.zero || stamina == 0)
        {
            isRunning = false;
            animator.SetBool("IsRunning", false);
        }

        // Recharge dash when stamina is full
        if (stamina == maxStamina && dash != maxDash)
        {
            stamina = maxStamina * 0.25f;
            dash += 1;
            UpdateDashIcon();
        }

        // Initiate dash on Space key press
        if (Input.GetKeyDown(KeyCode.Space) && move != Vector3.zero && !isDashing && dash > 0)
        {
            StartCoroutine(Dash(move));
        }

        // Enable running with Left Shift
        isRunning = Input.GetKey(KeyCode.LeftShift) && move != Vector3.zero && stamina > 0 && !isDashing;

        // Update animator states
        animator.SetBool("IsRunning", isRunning);
        animator.SetBool("IsWalking", move != Vector3.zero && !isRunning && !isDashing);

        if (move != Vector3.zero)
        {
            // Apply movement based on speed
            float speed = isRunning ? runSpeed : walkSpeed;
            characterController.Move(move * speed * Time.deltaTime);

            // Consume stamina when running
            if (isRunning)
            {
                stamina -= runStaminaCost * Time.deltaTime;
                SetStamina(stamina);
            }

            // Rotate player to face movement direction
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }
    }

    /// <summary>
    /// Applies gravity to the player.
    /// </summary>
    private void HandleGravity()
    {
        if (!characterController.isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
        }
        else
        {
            velocity.y = -2f;
        }
        characterController.Move(velocity * Time.deltaTime);
    }

    /// <summary>
    /// Displays the death menu when health reaches zero.
    /// </summary>
    private void DeathMenu()
    {
        if (health <= 0 && !isDead)
        {
            Debug.Log("Player is dead!", this);
            DeathPanel.SetActive(true);
            isDead = true;
            enabled = false;
        }
    }

    /// <summary>
    /// Initiates stamina regeneration if needed.
    /// </summary>
    private void RegenerateStaminaIfNeeded()
    {
        if (stamina < maxStamina && !isRegeneratingStamina)
        {
            StartCoroutine(RegenerateStamina());
        }
    }

    /// <summary>
    /// Handles collisions with collectables and enemy damage zones.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Collectable"))
        {
            CollectItem(other.gameObject);
        }
        else if (other.CompareTag("EnemyDamageZone"))
        {
            Enemy enemy = other.gameObject.GetComponentInParent<Enemy>();
            if (enemy != null)
            {
                SetHP(health - enemy.GetAttackDamage());
                SetFear(fear + enemy.GetIntimidation());
                Debug.Log($"Player hit by {other.gameObject.name}! Damage: {enemy.GetAttackDamage()}, Health: {health}", this);
            }
            else
            {
                Debug.LogWarning("EnemyDamageZone detected, but no Enemy component found on " + other.gameObject.name, this);
            }
        }
    }

    /// <summary>
    /// Collects an item, increases health, and updates UI.
    /// </summary>
    private void CollectItem(GameObject item)
    {
        collectedItems++;
        SetHP(health + 0.5f);
        Destroy(item);
        UpdateCollectedItemsText();
    }

    /// <summary>
    /// Updates the collected items text in the UI.
    /// </summary>
    private void UpdateCollectedItemsText()
    {
        if (collectedItemsText != null)
        {
            collectedItemsText.text = "Collected: " + collectedItems;
        }
    }

    /// <summary>
    /// Updates the timer text in the UI.
    /// </summary>
    private void UpdateTimerText()
    {
        if (timerText != null)
        {
            timerText.text = "Time: " + Mathf.Ceil(timeRemaining).ToString();
        }
    }

    /// <summary>
    /// Manages the game timer and triggers game over when time runs out.
    /// </summary>
    private IEnumerator GameTimer()
    {
        while (timeRemaining > 0)
        {
            yield return new WaitForSeconds(1f);
            timeRemaining -= 1f;
        }
        EndGame();
    }

    /// <summary>
    /// Ends the game and displays the game over panel.
    /// </summary>
    private void EndGame()
    {
        enabled = false;
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            gameOver.text = "Game Over!\nCollected Items: " + collectedItems;
        }
    }

    /// <summary>
    /// Performs a dash movement in the specified direction.
    /// </summary>
    private IEnumerator Dash(Vector3 dashDirection)
    {
        if (isDashing) yield break;

        animator.SetBool("IsDashing", true);
        dash -= 1;
        isDashing = true;
        UpdateDashIcon();

        float startTime = Time.time;
        while (Time.time < startTime + dashTime)
        {
            characterController.Move(dashDirection * dashSpeed * Time.deltaTime);
            yield return null;
        }

        animator.SetBool("IsDashing", false);
        isDashing = false;
    }

    /// <summary>
    /// Regenerates stamina over time when not running.
    /// </summary>
    private IEnumerator RegenerateStamina()
    {
        isRegeneratingStamina = true;
        yield return new WaitForSeconds(3f);

        while (stamina < maxStamina)
        {
            stamina = Mathf.Min(stamina + 0.1f, maxStamina);
            SetStamina(stamina);
            UpdateDashIcon();
            yield return new WaitForSeconds(0.15f);
        }

        isRegeneratingStamina = false;
    }

    /// <summary>
    /// Respawns the player at the designated point with adjusted stats.
    /// </summary>
    public void RespawnPlayer()
    {
        if (!DeathPanel.activeSelf) return;

        // Reset position
        characterController.enabled = false;
        transform.position = respawnPoint;
        characterController.enabled = true;

        // Adjust stats
        collectedItems = Mathf.Max(0, collectedItems - 5);
        fear = Mathf.Min(20, fear + 3);
        SetDash(0);
        SetStamina(10);
        SetHP(maxHealth);
        UpdateCollectedItemsText();

        // Reset state
        DeathPanel.SetActive(false);
        isDead = false;
        enabled = true;

        // Reset animations
        animator.SetBool("IsWalking", false);
        animator.SetBool("IsRunning", false);
        animator.SetBool("IsDashing", false);
    }

    /// <summary>
    /// Returns to the main menu and resets the game state.
    /// </summary>
    public void MainMenu()
    {
        if (!gameOverPanel.activeSelf) return;

        // Save state if save system exists
        if (saveSystem != null)
        {
            saveSystem.SaveCurrentState();
        }

        // Reset position
        characterController.enabled = false;
        transform.position = respawnPoint;
        characterController.enabled = true;

        // Reset stats
        collectedItems = 0;
        fear = 0;
        timeRemaining = gameDuration;
        SetDash(0);
        SetStamina(10);
        SetHP(maxHealth);
        UpdateCollectedItemsText();

        // Reset state
        gameOverPanel.SetActive(false);
        isDead = false;

        // Save again if save system exists
        if (saveSystem != null)
        {
            saveSystem.SaveCurrentState();
        }

        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Updates the health icon based on current health.
    /// </summary>
    private void UpdateHealthIcon()
    {
        if (healthIcon != null && healthSprites != null && healthSprites.Length == 6)
        {
            float healthPercentage = (health / maxHealth) * 100;
            int index = healthPercentage >= 100 ? 0 :
                        healthPercentage >= 80 ? 1 :
                        healthPercentage >= 60 ? 2 :
                        healthPercentage >= 40 ? 3 :
                        healthPercentage > 0 ? 4 : 5;
            healthIcon.sprite = healthSprites[index];
        }
    }

    /// <summary>
    /// Updates the fear icon based on current fear.
    /// </summary>
    private void UpdateFearIcon()
    {
        if (fearIcon != null && fearSprites != null && fearSprites.Length == 5)
        {
            float fearPercentage = (fear / maxFear) * 100;
            int index = fearPercentage >= 100 ? 0 :
                        fearPercentage >= 75 ? 1 :
                        fearPercentage >= 50 ? 2 :
                        fearPercentage >= 25 ? 3 : 4;
            fearIcon.sprite = fearSprites[index];
        }
    }

    /// <summary>
    /// Updates the dash icon based on current dash count.
    /// </summary>
    private void UpdateDashIcon()
    {
        if (dashIcon != null && dashSprites != null && dashSprites.Length == 4)
        {
            int index = dash == 3 ? 0 :
                        dash >= 2 ? 1 :
                        dash >= 1 ? 2 : 3;
            dashIcon.sprite = dashSprites[index];
        }
    }
}