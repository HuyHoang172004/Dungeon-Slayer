using UnityEngine;

public class Bow : MonoBehaviour {

    [Header("References")]
    public Transform bow;
    public Animator animator;
    public GameObject arrowPrefab;
    public GameObject sound;

    private Stats playerStats;

    private void Awake() {
        if (animator == null) animator = GetComponent<Animator>();
        playerStats = GetComponentInParent<Stats>();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.JoystickButton3)) {
            FireBow();
        }
    }

    public void AttackButton() {
        FireBow();
    }

    private void FireBow() {
        if (animator != null) {
            animator.SetTrigger("attackBow");
        }
    }

    // Called by Animation Event half-way through the Bow drawing animation
    public void ShootArrow() {
        if (arrowPrefab != null && bow != null) {
            Instantiate(arrowPrefab, bow.position, bow.rotation);
        }

        if (sound != null) {
            Instantiate(sound, transform.position, Quaternion.identity);
        }
    }
}
