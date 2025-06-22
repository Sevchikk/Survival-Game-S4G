using UnityEngine;
using System.Collections;

/// <summary>
/// Controls enemy AI behavior, including patrolling, chasing, and attacking the player.
/// </summary>
[RequireComponent(typeof(CharacterController), typeof(Animator))]
public class Enemy : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField, Range(0f, 20f)] private float sightRadius = 13.9f;
    [SerializeField, Range(0f, 5f)] private float attackRadius = 2f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private float searchTime = 11f;
    [SerializeField] private float searchSpeed = 2f;
    [SerializeField] private LayerMask boundaryLayer;

    [Header("Attack Settings")]
    [SerializeField] private float intimidationDamage = 3f;
    [SerializeField] private float attackDamage = 3f;
    [SerializeField] private GameObject damageZone;
    [SerializeField] private float attackCooldown = 2f;

    [Header("References")]
    [SerializeField] private Transform player;

    private CharacterController characterController;
    private Animator animator;
    private Character playerCharacter;
    private Vector3 velocity;
    private bool isPlayerInRange;
    private bool isPlayerInAttackRange;
    private bool isAttacking;
    public bool isSearching;
    private Vector3 searchDirection;
    private float searchStartTime = -1f;

    private void Start()
    {
        // Cache components
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        // Validate components
        if (characterController == null || animator == null)
        {
            Debug.LogError("Missing CharacterController or Animator on Enemy!", this);
            enabled = false;
            return;
        }

        // Disable damage zone if assigned
        if (damageZone != null)
        {
            damageZone.SetActive(false);
        }

        // Find player if not assigned
        if (player == null)
        {
            Debug.LogWarning("Player reference is missing, attempting to find...", this);
            player = FindFirstObjectByType<Character>()?.transform;
            if (player != null)
            {
                playerCharacter = player.GetComponent<Character>();
                if (playerCharacter == null)
                {
                    Debug.LogWarning("Character component not found on player!", this);
                }
            }
            else
            {
                Debug.LogError("Could not find player in scene!", this);
                enabled = false;
                return;
            }
        }
    }

    private void Update()
    {
        if (player == null || playerCharacter.GetHP() <= 0)
        {
            Debug.LogWarning("Player reference is missing or player is dead!", this);
            return;
        }

        // Check player proximity
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        isPlayerInRange = distanceToPlayer <= sightRadius;
        isPlayerInAttackRange = distanceToPlayer <= attackRadius;

        // Start searching if player is out of sight
        if (!isPlayerInRange && !isSearching)
        {
            StartCoroutine(SearchPlayer());
        }
        // Stop searching if player is back in range
        else if (isPlayerInRange && isSearching)
        {
            isSearching = false;
            searchStartTime = -1f;
        }

        // Update enemy behavior
        HandleMovement();
        HandleGravity();
        HandleAttack();
    }

    /// <summary>
    /// Handles enemy movement based on state (searching or chasing).
    /// </summary>
    private void HandleMovement()
    {
        if (isSearching)
        {
            Vector3 moveDirection = searchDirection;
            // Reverse direction if hitting a boundary
            if (Physics.Raycast(transform.position, moveDirection, 1f, boundaryLayer))
            {
                searchDirection = -searchDirection;
            }
            characterController.Move(moveDirection * searchSpeed * Time.deltaTime);
            UpdateAnimator(moveDirection, true);
        }
        else if (isPlayerInRange && !isPlayerInAttackRange)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            directionToPlayer.y = 0;
            characterController.Move(directionToPlayer * moveSpeed * Time.deltaTime);
            UpdateAnimator(directionToPlayer, false);
        }
        else
        {
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsSearching", false);
        }
    }

    /// <summary>
    /// Updates animator states and rotates enemy.
    /// </summary>
    private void UpdateAnimator(Vector3 moveDirection, bool isSearchingState)
    {
        if (moveDirection != Vector3.zero)
        {
            animator.SetBool("IsWalking", true);
            animator.SetBool("IsSearching", isSearchingState);
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Applies gravity to the enemy.
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
    /// Initiates an attack when the player is in range.
    /// </summary>
    private void HandleAttack()
    {
        if (isPlayerInAttackRange && !isAttacking && playerCharacter.GetHP() > 0)
        {
            StartCoroutine(Attack());
        }
    }

    /// <summary>
    /// Makes the enemy patrol randomly for a set duration.
    /// </summary>
    private IEnumerator SearchPlayer()
    {
        isSearching = true;
        searchStartTime = Time.time;
        searchDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;

        yield return new WaitForSeconds(searchTime);
        if (isSearching)
        {
            Destroy(gameObject);
        }
        isSearching = false;
        searchStartTime = -1f;
    }

    /// <summary>
    /// Activates the damage zone during an attack.
    /// </summary>
    public void EnableDamageZone()
    {
        if (damageZone != null)
            damageZone.SetActive(true);
    }

    /// <summary>
    /// Deactivates the damage zone after an attack.
    /// </summary>
    public void DisableDamageZone()
    {
        if (damageZone != null)
            damageZone.SetActive(false);
    }

    /// <summary>
    /// Performs an attack animation and activates damage zone.
    /// </summary>
    private IEnumerator Attack()
    {
        isSearching = false;
        animator.SetBool("IsWalking", false);
        isAttacking = true;
        animator.SetBool("IsAttacking", true);

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        directionToPlayer.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        while (Quaternion.Angle(transform.rotation, targetRotation) > 1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            yield return null;
        }

        if (damageZone != null)
        {
            damageZone.SetActive(true);
            yield return new WaitForSeconds(1.07f);
            damageZone.SetActive(false);
        }

        animator.SetBool("IsAttacking", false);
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    /// <summary>
    /// Draws sight and attack radii in the editor for debugging.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }

    /// <summary>
    /// Gets the attack damage value.
    /// </summary>
    public float GetAttackDamage() => attackDamage;

    /// <summary>
    /// Gets the intimidation damage value.
    /// </summary>
    public float GetIntimidation() => intimidationDamage;

    /// <summary>
    /// Notifies the spawner when the enemy is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        EnemySpawner spawner = FindFirstObjectByType<EnemySpawner>();
        if (spawner != null)
        {
            spawner.OnEnemyDestroyed(gameObject);
        }
    }
}