﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    private float xAxis;
    public float jumpForce = 12f;
    private float jumpBufferCounter = 0;
    [SerializeField] private float jumpBufferFrames;
    private float coyoteTimeCounter = 0;
    [SerializeField] private float coyoteTime;
    private int airJumpCounter = 0;
    [SerializeField] private int maxAirJumps;

    [Header("Double Jump Settings")]
    [SerializeField] private bool hasDoubleJumpAbility = false; // Khả năng double jump

    [Header("Ground Check Settings")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckY = 0.2f;
    [SerializeField] private float groundCheckX = 0.5f;
    [SerializeField] private LayerMask whatIsGround;

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashTime = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private bool hasDashAbility = false; // Khả năng dash
    [Space(5)]

    [Header("Combat Settings")]
    public CombatManager combatManager; // Gán trong Inspector
    private Vector2 lastStepPosition;

    PlayerStateList pState;
    private Rigidbody2D rb;
    private Animator animator;
    private float gravity;
    private bool canDash = true;
    private bool dashed;
    private float dashDirection;

    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;
    public HealthBar healthBar;
    private bool isInvincible = false;
    [SerializeField] private float invincibilityDuration = 1.5f;
    [SerializeField] private float flashDelay = 0.1f;
    private SpriteRenderer spriteRenderer;

    [Header("Checkpoint Ground")]
    private Vector2 lastGroundedPosition;
    private Vector2 checkpointPosition;

    [Header("Death Animation")]
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float respawnDelay = 1.5f;
    private bool isDead = false;

    private bool wasGrounded = false;

    [Header("Slash Effect Settings")]
    public GameObject slashEffect; // Prefab hiệu ứng slash, gán trong Inspector
    [SerializeField] private float slashCooldown = 0.3f;
    [SerializeField] private float slashOffsetY = -0.5f;
    private float lastSlashTime = -10f;


    [Header("Recoil Settings")]
    [SerializeField] private float recoilForce = 15f; // Lực bật ngược khi va chạm dưới chân
    [SerializeField] private float recoilCooldown = 0.3f;
    [SerializeField] private float maxCheckDistance = 2f;
    [SerializeField] private float recoilBoxWidth = 1f; // Chiều rộng vùng recoil (thanh ngang)
    private float lastRecoilTime = -10f;
    [SerializeField] private LayerMask pogoMask;

    void Awake()
    {
        if (PlayerData.Instance != null)
        {
            maxHealth = PlayerData.Instance.maxHealth;
            currentHealth = PlayerData.Instance.currentHealth;
        }
    }
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        pState = GetComponent<PlayerStateList>();
        gravity = rb.gravityScale;

        if (PlayerData.Instance != null)
        {
            maxHealth = PlayerData.Instance.maxHealth;
            currentHealth = PlayerData.Instance.currentHealth;
            checkpointPosition = PlayerData.Instance.checkpointPosition;
            lastGroundedPosition = PlayerData.Instance.lastGroundedPosition;
            transform.position = PlayerData.Instance.lastGroundedPosition;
        }
        else
        {
            currentHealth = maxHealth;
            checkpointPosition = transform.position;
            lastGroundedPosition = transform.position;
        }
        if (currentHealth <= 0)
        {
            Die(); 
        }
        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth(currentHealth); // Đảm bảo gọi sau khi gán currentHealth
        lastStepPosition = transform.position;
    }

    void Update()
    {
        UpdateJumpVariables();
        GetInputs();
        if (pState.dashing) return;
        Move();
        Jump();
        UpdateAnimator();
        Flip();
        StartDash();
        TrackStep();
        TrackGroundedTransition();
        HandleDownSlash();
        HandleDownRecoil(); // Gọi hàm kiểm tra recoil
    }

    void GetInputs()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
    }

    void Move()
    {
        rb.velocity = new Vector2(walkSpeed * xAxis, rb.velocity.y);
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        if (PlayerData.Instance != null)
            PlayerData.Instance.currentHealth = currentHealth;

        healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        StartCoroutine(InvincibilityFlash());
    }
    public void SetCheckpoint(Vector2 position)
    {
        checkpointPosition = position;
        if (PlayerData.Instance != null)
        {
            PlayerData.Instance.checkpointPosition = position;
        }
        currentHealth = maxHealth;
        healthBar.SetHealth(currentHealth);
        Debug.Log("Checkpoint set at: " + position);
    }
    public Vector2 GetCheckpointPosition()
    {
        return checkpointPosition;
    }
    IEnumerator InvincibilityFlash()
    {
        isInvincible = true;

        float elapsed = 0f;
        while (elapsed < invincibilityDuration)
        {
            spriteRenderer.enabled = false; // ẩn
            yield return new WaitForSeconds(flashDelay);
            spriteRenderer.enabled = true;  // hiện
            yield return new WaitForSeconds(flashDelay);

            elapsed += flashDelay * 2;
        }

        isInvincible = false;
    }
    void StartDash()
    {
        if (Input.GetButtonDown("Dash") && hasDashAbility && canDash && !dashed)
        {
            // Lưu hướng dash dựa trên input hoặc hướng hiện tại
            if (xAxis != 0)
            {
                dashDirection = xAxis > 0 ? 1 : -1;
            }
            else
            {
                dashDirection = transform.localScale.x > 0 ? 1 : -1;
            }

            StartCoroutine(Dash());
            dashed = true;
        }

        // Reset dash khi chạm đất
        if (Grounded())
        {
            dashed = false;
        }
    }
    public void Die()
    {
        if (isDead) return;
        isDead = true;

        StartCoroutine(HandleDeath());
    }
    IEnumerator HandleDeath()
    {
        if (animator != null)
        {
            animator.SetTrigger("Dead");
        }

        yield return StartCoroutine(FadeToBlack());

        // Dịch chuyển về checkpointPosition (điểm checkpoint gần nhất)
        transform.position = checkpointPosition;

        currentHealth = maxHealth;
        healthBar.SetHealth(currentHealth);

        animator.Play("Idle");
        yield return new WaitForSeconds(respawnDelay);
        yield return StartCoroutine(FadeFromBlack());

        isDead = false;
    }

    IEnumerator FadeToBlack()
    {
        float time = 0f;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, time / fadeDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = 1f;
    }

    IEnumerator FadeFromBlack()
    {
        float time = 0f;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, time / fadeDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = 0f;
    }
    IEnumerator Dash()
    {
        canDash = false;
        pState.dashing = true;

        if (animator != null)
        {
            animator.ResetTrigger("Dashing");
            animator.SetTrigger("Dashing");
        }
        rb.gravityScale = 0;
        rb.velocity = new Vector2(dashDirection * dashSpeed, 0);

        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = gravity;
        pState.dashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    void Jump()
    {
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0)
        {
            pState.jumping = false;
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }

        if (!pState.jumping)
        {
            if (jumpBufferCounter > 0 && coyoteTimeCounter > 0)
            {
                pState.jumping = true;
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
            else if (!Grounded() && hasDoubleJumpAbility && airJumpCounter < maxAirJumps && Input.GetButtonDown("Jump"))
            {
                pState.jumping = true;
                airJumpCounter++;
                rb.velocity = new Vector3(rb.velocity.x, jumpForce);
            }
        }
    }

    public Vector2 GetLastGroundedPosition()
    {
        return lastGroundedPosition;
    }
    // Trả về RaycastHit2D? nếu chạm ground, null nếu không
    RaycastHit2D? GroundedHit()
    {
        RaycastHit2D hit1 = Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, whatIsGround);
        if (hit1.collider != null) return hit1;
        RaycastHit2D hit2 = Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround);
        if (hit2.collider != null) return hit2;
        RaycastHit2D hit3 = Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround);
        if (hit3.collider != null) return hit3;
        return null;
    }

    void UpdateJumpVariables()
    {
        var groundHit = GroundedHit();
        if (groundHit != null)
        {
            coyoteTimeCounter = coyoteTime;
            pState.jumping = false;
            airJumpCounter = 0;

            // Chỉ cập nhật lastGroundedPosition nếu chạm ground layer
            lastGroundedPosition = transform.position;
            if (PlayerData.Instance != null)
            {
                PlayerData.Instance.lastGroundedPosition = lastGroundedPosition;
            }
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferFrames;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime * 10;
        }
    }

    void Flip()
    {
        if (xAxis < 0)
        {
            transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
        else if (xAxis > 0)
        {
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
    }

    public bool Grounded()
    {
        return Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround);
    }

    void UpdateAnimator()
    {
        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));       
            animator.SetBool("IsGrounded", Grounded());             
            animator.SetFloat("VelocityY", rb.velocity.y);               
        }
    }

    void TrackStep()
    {
        if (combatManager == null) return;
        float stepDistance = 0.5f; // Độ dài 1 bước, có thể điều chỉnh
        if (Vector2.Distance((Vector2)transform.position, lastStepPosition) >= stepDistance)
        {
            combatManager.OnPlayerStep();
            lastStepPosition = transform.position;
        }
    }

    void TrackGroundedTransition()
    {
        bool nowGrounded = Grounded();
        if (!wasGrounded && nowGrounded)
        {
            // Vừa chạm đất
            if (combatManager != null)
                combatManager.NotifyPlayerGrounded();
        }
        wasGrounded = nowGrounded;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("DoubleJump"))
        {
            hasDoubleJumpAbility = true;
            Debug.Log("Double Jump ability unlocked!");
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Dash"))
        {
            hasDashAbility = true;
            Debug.Log("Dash ability unlocked!");
            Destroy(other.gameObject);
        }
    }

    public bool HasDoubleJumpAbility()
    {
        return hasDoubleJumpAbility;
    }

    public bool HasDashAbility()
    {
        return hasDashAbility;
    }

    public void ResetDoubleJumpAbility()
    {
        hasDoubleJumpAbility = false;
    }

    public void ResetDashAbility()
    {
        hasDashAbility = false;
    }

    public void ResetAllAbilities()
    {
        hasDoubleJumpAbility = false;
        hasDashAbility = false;
    }

    void HandleDownSlash()
    {
        if (rb.velocity.y < 0 && !Grounded())
        {
            if (Input.GetAxisRaw("Vertical") < 0 && Input.GetKeyDown(KeyCode.J))
            {
                if (Time.time - lastSlashTime >= slashCooldown)
                {
                    if (animator != null)
                    {
                        animator.SetTrigger("Pogo");
                    }
                    if (slashEffect != null)
                    {
                        Vector3 spawnPos = transform.position + Vector3.down * slashOffsetY;
                        Quaternion rotation = Quaternion.Euler(0, 0, -90);

                        GameObject slash = Instantiate(slashEffect, spawnPos, rotation);
                        slash.transform.localScale = Vector3.one;
                        Destroy(slash, 0.3f);
                        lastSlashTime = Time.time;
                    }
                }
            }
        }
    }

    // Hàm kiểm tra S+J và va chạm dưới chân để recoil
    void HandleDownRecoil()
    {
        if (Input.GetAxisRaw("Vertical") < 0 &&
            Input.GetKeyDown(KeyCode.J) &&
            rb.velocity.y < 0 &&
            Time.time - lastRecoilTime > recoilCooldown)
        {  
            Vector2 boxSize = new Vector2(recoilBoxWidth, maxCheckDistance);
            Vector2 boxCenter = (Vector2)groundCheckPoint.position + Vector2.down * (maxCheckDistance / 2f);

            Collider2D hit = Physics2D.OverlapBox(boxCenter, boxSize, 0f, pogoMask);

            if (hit != null)
            {
                rb.velocity = new Vector2(rb.velocity.x, recoilForce);
                lastRecoilTime = Time.time;
                airJumpCounter = 0;
                dashed = false; // Reset dash sau khi pogo
            }
        }
    }

#if UNITY_EDITOR
    // Vẽ gizmo để debug vùng va chạm recoil
    void OnDrawGizmosSelected()
    {
        if (groundCheckPoint == null) return;

        Vector2 boxSize = new Vector2(recoilBoxWidth, maxCheckDistance);
        Vector2 boxCenter = (Vector2)groundCheckPoint.position + Vector2.down * (maxCheckDistance / 2f);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(boxCenter, boxSize);
        {
            // GIZMO CHO VỊ TRÍ SLASH
            Gizmos.color = Color.red;

            // Vị trí spawn của slash giống như trong HandleDownSlash()
            Vector3 slashSpawnPos = transform.position + Vector3.down * slashOffsetY;
            Gizmos.DrawWireSphere(slashSpawnPos, 0.1f); // Vẽ hình cầu nhỏ để dễ thấy
        }
    }
#endif


    void OnDestroy()
    {
        if (PlayerData.Instance != null)
        {
            PlayerData.Instance.currentHealth = currentHealth;
            PlayerData.Instance.maxHealth = maxHealth;
            PlayerData.Instance.checkpointPosition = checkpointPosition;
            PlayerData.Instance.lastGroundedPosition = lastGroundedPosition;
        }
    }
}