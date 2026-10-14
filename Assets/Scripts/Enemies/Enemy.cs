using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Enemy : MonoBehaviour {

    [Header("Stats")]
    public int health = 500;
    public float timeDestroy = 1.5f;
    public Animator animator;
    public GameObject deathSound;

    [Header("Loot Drops")]
    public GameObject coins;
    public GameObject hearts;
    public GameObject sword;
    public GameObject shield;

    public int maxCoins = 5;
    public int maxHearts = 3;
    public int maxSwords = 2;
    public int maxShields = 2;

    private Rigidbody2D rb;
    private Collider2D col;
    private bool isDead = false;

    private void Awake() {
        if (animator == null) animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    public void TakeDamage(int damage) {
        if (isDead) return;

        MinibossController miniboss = GetComponent<MinibossController>();
        if (miniboss != null)
        {
            damage = miniboss.ModifyIncomingDamage(damage);
        }

        health -= damage;

        if (miniboss != null)
        {
            miniboss.EvaluatePhase(health);
        }

        if (ImpactFX.instance != null) {
            ImpactFX.instance.SpawnDamagePopup(transform.position, damage);
        }

        if (animator != null) {
            animator.SetTrigger("hurt");
        }

        if (health <= 0) {
            FinalWizardBossAI finalBoss = GetComponent<FinalWizardBossAI>();
            if (finalBoss != null && finalBoss.BeginDefeatSequence()) {
                health = 0;
                isDead = true;
                return;
            }

            Die();
        }
    }

    private void Die() {
        if (isDead) return;
        isDead = true;

        if (rb != null) {
            rb.constraints = RigidbodyConstraints2D.FreezePositionY;
        }

        DropLoot();

        if (animator != null) {
            animator.SetBool("isDead", true);
        }

        if (ScoreManager.instance != null) {
            ScoreManager.instance.ChangeScore(100);
        }

        if (col != null) col.enabled = false;
        this.enabled = false;

        if (deathSound != null) {
            Instantiate(deathSound, transform.position, Quaternion.identity);
        }

        Destroy(gameObject, timeDestroy);
    }

    private void DropLoot() {
        SpawnItem(coins, Random.Range(1, maxCoins));
        SpawnItem(hearts, Random.Range(0, maxHearts));
        SpawnItem(sword,  Random.Range(0, maxSwords));
        SpawnItem(shield, Random.Range(0, maxShields));
    }

    private void SpawnItem(GameObject itemPrefab, int amount) {
        if (itemPrefab == null || amount <= 0) return;

        for (int i = 0; i < amount; i++) {
            // Adds a small random spread so loot doesn't stack perfectly on top of each other
            float randomXOffset = Random.Range(-1.5f, 1.5f);
            float randomYOffset = Random.Range(1.0f, 2.5f);
            
            Vector3 spawnPosition = transform.position + new Vector3(randomXOffset, randomYOffset, 0);
            Instantiate(itemPrefab, spawnPosition, Quaternion.identity);
        }
    }
}
