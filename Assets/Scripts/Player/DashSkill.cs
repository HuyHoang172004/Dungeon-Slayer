using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class DashSkill : MonoBehaviour {
    
    [Header("Dash Settings")]
    public float dashForce = 25f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1.0f;
    
    [Header("Effects")]
    public GameObject afterimagePrefab; // An empty GameObject with SpriteRenderer & Afterimage script
    public float afterimageSpawnRate = 0.05f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool isDashing = false;
    private float lastDashTime = -100f;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update() {
        if (isDashing || Time.time < lastDashTime + dashCooldown) return;

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.JoystickButton5)) {
            StartCoroutine(PerformDash());
        }
    }

    public void DashButton() {
        if (!isDashing && Time.time >= lastDashTime + dashCooldown) {
            StartCoroutine(PerformDash());
        }
    }

    private IEnumerator PerformDash() {
        isDashing = true;
        lastDashTime = Time.time;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        float dashDirection = (spriteRenderer != null && spriteRenderer.flipX) ? -1f : 1f;
        
        if (Mathf.Abs(rb.linearVelocity.x) > 0.1f) {
            dashDirection = Mathf.Sign(rb.linearVelocity.x);
        }

        rb.linearVelocity = new Vector2(dashDirection * dashForce, 0f);

        float elapsed = 0f;
        float nextImageTime = 0f;

        while (elapsed < dashDuration) {
            elapsed += Time.deltaTime;

            if (afterimagePrefab != null && elapsed >= nextImageTime) {
                SpawnAfterimage();
                nextImageTime = elapsed + afterimageSpawnRate;
            }

            yield return null;
        }

        rb.gravityScale = originalGravity;
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        isDashing = false;
    }

    private void SpawnAfterimage() {
        GameObject shadow = Instantiate(afterimagePrefab, transform.position, transform.rotation);
        SpriteRenderer shadowSprite = shadow.GetComponent<SpriteRenderer>();
        
        if (shadowSprite != null && spriteRenderer != null) {
            shadowSprite.sprite = spriteRenderer.sprite;
            shadowSprite.flipX = spriteRenderer.flipX;
        }
    }
}
