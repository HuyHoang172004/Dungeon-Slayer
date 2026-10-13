using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveByTouch : MonoBehaviour {

    public float moveSpeed;
    public float jumpHeight;

    public GameObject row;
    public GameObject menu;
    public GameObject stats;
    public GameObject mobileControls;

    public Joystick joystick;
    public ParticleSystem dust;

    private bool canJump;
    private bool canDoubleJump;
    private bool facingLeft = false;
    private bool facingRight = false;
    
    // Cached components
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private PlayerControllerUP playerControllerUp;
    private bool isMoving = false;

    // Start is called before the first frame update
    void Start() {
        jumpHeight = 22f;
        if (moveSpeed < 8f) moveSpeed = 8f;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerControllerUp = GetComponentInChildren<PlayerControllerUP>();
    }

    // Update is called once per frame
    void Update() {
        if (mobileControls != null && mobileControls.activeSelf) {
            canJump = playerControllerUp.getJump();
            canDoubleJump = playerControllerUp.getDoubleJump();

            float h = joystick.Horizontal;

            if (h < -0.1f) {
                if (!isMoving) { CreateDust(); isMoving = true; }
                rb.linearVelocity = new Vector2(-moveSpeed, rb.linearVelocity.y);
                anim.SetBool("moving", true);
                spriteRenderer.flipX = true;

                facingLeft = true;
                if (facingRight == true) {
                    row.transform.Rotate(0f, 180f, 0f);
                    facingRight = false;
                }
            }
            else if (h > 0.1f) {
                if (!isMoving) { CreateDust(); isMoving = true; }
                rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);
                anim.SetBool("moving", true);
                spriteRenderer.flipX = false;

                facingRight = true;
                if (facingLeft == true) {
                    row.transform.Rotate(0f, 180f, 0f);
                    facingLeft = false;
                }
            }
            // Remove running animation
            else {
                isMoving = false;
                anim.SetBool("moving", false);
                // Stop instantly when joystick is released to feel responsive
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            }
        }
    }

    public void Jump() {
        if (mobileControls != null && mobileControls.activeSelf) {
            if (canJump) {
                CreateDust();
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpHeight);
                playerControllerUp.setJump(false);
            } else if (canDoubleJump) {
                CreateDust();
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpHeight);
                playerControllerUp.setDoubleJump(false);
                canDoubleJump = false;
            }
        }
    }

    public void settings() {
        if (mobileControls != null && mobileControls.activeSelf) {
            if (!menu.activeSelf) {
                stats.SetActive(false);
                menu.SetActive(true);
                Time.timeScale = 0;
            } else {
                menu.SetActive(false);
                stats.SetActive(true);
                Time.timeScale = 1;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (mobileControls != null && mobileControls.activeSelf) {
            if (collision.gameObject.CompareTag("Chest")) {
                Chest chest = collision.gameObject.GetComponent<Chest>();
                if (chest != null) chest.openChest();
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (mobileControls != null && mobileControls.activeSelf) {
            if (collision.gameObject.CompareTag("Instructor")) {
                DialogueManager dialogue = collision.gameObject.GetComponent<DialogueManager>();
                if (dialogue != null) dialogue.DialogueMobile();
            }
        }
    }

    void CreateDust() {
        if (dust != null) {
            dust.Play();
        }
    }
}
