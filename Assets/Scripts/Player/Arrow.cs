using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Arrow : MonoBehaviour {

    public float speed = 20f;
    public int damage = 200;
    public float timeToLive = 3.0f;
    
    private Rigidbody2D rb;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start() {
        if (rb != null) {
            rb.linearVelocity = transform.right * speed;
        }
        
        // Destroy the arrow after a few seconds so it doesn't cause a memory leak
        Destroy(gameObject, timeToLive);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Enemies") || collision.gameObject.CompareTag("Boss")) {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null) {
                // Calculate critical damage scaling
                int attackDamage = Stats.instance != null ? Stats.instance.getAttackDamage() : 100;
                damage = Mathf.FloorToInt(attackDamage + (0.4f * attackDamage));
                
                enemy.TakeDamage(damage);
            }
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Ground")) {
            Destroy(gameObject);
        }
    }
}
