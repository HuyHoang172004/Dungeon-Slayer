using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour {

    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float jumpHeight = 22f;
  
    [Header("UI & References")]
    public GameObject row;
    public GameObject menu;
    public GameObject stats;
    public GameObject controllerMobile;
    public ParticleSystem dust;

    [Header("Combat Settings")]
    public int damagePatrols = 25;
    private float attackRate = 0.6f;
    private float nextAttackTime = 0.5f;

    // State Variables
    private bool canJump;
    private bool canDoubleJump;
    private bool facingLeft = false;
    private bool facingRight = false;
    private bool isDead = false;

    // Cached Components
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private PlayerControllerUP playerControllerUp;
    private Stats playerStats;

    private void Awake() {
        jumpHeight = 22f;
        if (moveSpeed < 8f) moveSpeed = 8f;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerControllerUp = GetComponentInChildren<PlayerControllerUP>();
        playerStats = GetComponentInChildren<Stats>();
    }

    private void Start() {
        if (GameProgress.TryGetSavedPlayerPosition(out Vector3 savedPosition)) {
            transform.position = savedPosition;
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void Update() {
        if (isDead) 
            return;

        HandleMenuInput();
        HandleJumpInput();
        HandleMovementInput();
    }

    private void HandleMenuInput() {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton7)) {
            if (menu != null && stats != null) {
                bool isMenuActive = menu.activeSelf;
                menu.SetActive(!isMenuActive);
                stats.SetActive(isMenuActive);
                Time.timeScale = isMenuActive ? 1f : 0f;
            }
        }
    }

    private void HandleJumpInput() {
        if (playerControllerUp != null) {
            canJump = playerControllerUp.getJump();
            canDoubleJump = playerControllerUp.getDoubleJump();
        }

        // KeyCode.Space for Windows/Mac, JoystickButton0 for Xbox 'A' / PS 'Cross'
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0)) {
            if (canJump) {
                PerformJump();
                if (playerControllerUp != null) playerControllerUp.setJump(false);
            } else if (canDoubleJump) {
                PerformJump();
                if (playerControllerUp != null) playerControllerUp.setDoubleJump(false);
                canDoubleJump = false;
            }
        }
    }

    private void PerformJump() {
        CreateDust();
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpHeight);
    }

    private void HandleMovementInput() {
        // Native Unity axis seamlessly handles Keyboard (WASD/Arrows) and Gamepad Analog Stick 
        float moveDirection = Input.GetAxisRaw("Horizontal");

        // We only enforce velocity if keyboard/gamepad is being touched. 
        // This prevents overriding the Android On-Screen Joystick's velocity instructions.
        if (Mathf.Abs(moveDirection) > 0.1f) {
            CreateDust();
            rb.linearVelocity = new Vector2(moveDirection * moveSpeed, rb.linearVelocity.y);
            
            bool isMovingLeft = moveDirection < 0;
            spriteRenderer.flipX = isMovingLeft;
            HandleFacingDirection(isMovingLeft);
        }

        // Animate based on raw velocity to guarantee perfect UI sync across Android/Console/Windows
        anim.SetBool("moving", Mathf.Abs(rb.linearVelocity.x) > 0.1f);
    }

    private void HandleFacingDirection(bool isMovingLeft) {
        if (isMovingLeft) {
            facingLeft = true;
            if (facingRight && row != null) {
                row.transform.Rotate(0f, 180f, 0f);
                facingRight = false;
            }
        } else {
            facingRight = true;
            if (facingLeft && row != null) {
                row.transform.Rotate(0f, 180f, 0f);
                facingLeft = false;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.transform.CompareTag("Patrols")) {
            if (Time.time >= nextAttackTime && playerStats != null) {
                MinibossController miniboss = collision.transform.GetComponent<MinibossController>();
                int contactDamage = miniboss != null
                    ? Mathf.RoundToInt(damagePatrols * miniboss.PatrolDamageMultiplier)
                    : damagePatrols;
                playerStats.takeDamage(contactDamage);
                nextAttackTime = Time.time + attackRate;
            }
            rb.AddForce(new Vector2(10f, 10f), ForceMode2D.Impulse);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        string colTag = collision.gameObject.tag;
        if (colTag == "Coins" || colTag == "Gems" || colTag == "Stars" || 
            colTag == "Heart" || colTag == "Sword" || colTag == "Shield") {
            Destroy(collision.gameObject);
        }
    }

    public void reSpawn() {
        GameObject spawnPoint = GameObject.FindWithTag("ReSpawn");
        if (spawnPoint != null) {
            transform.position = spawnPoint.transform.position;
            GameProgress.SavePlayerPosition(transform.position);
        }
    }

    private void OnApplicationPause(bool paused) {
        if (paused) {
            GameProgress.SavePlayerPosition(transform.position);
        }
    }

    private void OnApplicationQuit() {
        GameProgress.SavePlayerPosition(transform.position);
    }

    public void destroy() {
        Destroy(gameObject, 5.0f);
        isDead = true;
    }

    public bool isDeadStatus() {
        isDead = true;
        return isDead;
    }

    private void CreateDust() {
        if (dust != null) {
            dust.Play();
        }
    }
}
