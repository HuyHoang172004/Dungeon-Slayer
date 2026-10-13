using UnityEngine;

public class PlayerControllerUP : MonoBehaviour {

    public float moveSpeed;
    public float jumpHeight;
    public float groundCheckRadius;
    public Transform groundCheck;
    public LayerMask whatIsGround;
    public Vector3 Movement;
    
    private bool grounded;
    private bool canJump;
    private bool canDoubleJump;

    private Animator parentAnim;

    private void Awake() {
        parentAnim = GetComponentInParent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.transform.CompareTag("Ground") || collision.transform.CompareTag("Water")) {
            if (parentAnim != null) parentAnim.SetBool("jumping", false);
            canJump = true;
            canDoubleJump = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision) {
        if (collision.transform.CompareTag("Ground") || collision.transform.CompareTag("Water")) {
            if (parentAnim != null) parentAnim.SetBool("jumping", true);
        }
    }

    public bool getJump() {
        return canJump;
    }

    public bool getDoubleJump() {
        return canDoubleJump;
    }

    public void setJump(bool jump) {
        this.canJump = jump;
    }

    public void setDoubleJump(bool jump) {
        this.canDoubleJump = jump;
    }
}