using UnityEngine;

public class PlayerCombat : MonoBehaviour {

    [Header("References")]
    public Animator animator;
    public Transform attackPoint;
    public LayerMask enemyLayers;
    public GameObject sound;

    [Header("Combat Settings")]
    public float attackRate = 0.6f;
    public float attackRange = 1f;

    private float nextAttackTime = 0.5f;
    private Stats playerStats;

    private void Awake() {
        if (animator == null) animator = GetComponent<Animator>();
        playerStats = GetComponent<Stats>();
    }

    private void Update() {
        if (Time.time >= nextAttackTime) {
            if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.JoystickButton2)) {
                PerformAttack();
            }
        }
    }

    public void AttackButton() {
        if (Time.time >= nextAttackTime) {
            PerformAttack();
        }
    }

    private void PerformAttack() {
        nextAttackTime = Time.time + (attackRate / attackRange);

        if (animator != null) {
            animator.SetTrigger("attack");
        }

        if (sound != null) {
            Instantiate(sound, transform.position, Quaternion.identity);
        }

        if (attackPoint == null) return;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        bool hitSuccessful = false;

        foreach (Collider2D enemyCollider in hitEnemies) {
            Enemy enemy = enemyCollider.GetComponent<Enemy>();
            if (enemy != null) {
                int damage = playerStats != null ? playerStats.getAttackDamage() : 100;
                enemy.TakeDamage(damage);

                if (playerStats != null && playerStats.getPower() != 4) {
                    playerStats.takePower(1);
                }
                
                hitSuccessful = true;
                break; 
            }
        }

        if (hitSuccessful && ImpactFX.instance != null) {
            ImpactFX.instance.TriggerHitImpact();
        }
    }

    private void OnDrawGizmosSelected() {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
