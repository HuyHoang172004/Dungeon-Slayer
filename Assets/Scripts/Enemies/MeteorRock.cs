using UnityEngine;

/// <summary>
/// Falling rock used by the final Wizard's phase 3 meteor shower.
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class MeteorRock : MonoBehaviour
{
    [SerializeField] private float fallSpeed = 5.5f;
    [SerializeField] private int damage = 35;
    [SerializeField] private float maximumLifetime = 4f;

    private Rigidbody2D body;
    private float minimumTerrainImpactY = float.NegativeInfinity;
    private float lifeTimer;
    private bool hasImpacted;

    public void Initialize(float terrainImpactY)
    {
        minimumTerrainImpactY = terrainImpactY;
    }

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Kinematic;
        body.gravityScale = 0f;
        body.freezeRotation = true;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        CircleCollider2D hitbox = GetComponent<CircleCollider2D>();
        hitbox.isTrigger = true;
        hitbox.radius = 1.2f;
    }

    private void FixedUpdate()
    {
        if (hasImpacted) return;

        lifeTimer += Time.fixedDeltaTime;
        body.MovePosition(body.position + Vector2.down * (fallSpeed * Time.fixedDeltaTime));
        if (lifeTimer >= maximumLifetime || transform.position.y < -30f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasImpacted || collision == null) return;

        bool isPlayer = collision.CompareTag("Player") ||
            collision.transform.root.CompareTag("Player");
        if (isPlayer)
        {
            Stats stats = collision.GetComponentInParent<Stats>();
            if (stats != null) stats.takeDamage(damage);
            Impact();
            return;
        }

        // The final arena has solid ceiling colliders above the camera. Ignore
        // those and only accept terrain impacts once the rock has descended to
        // the player's floor area, otherwise it disappears before being seen.
        if (!collision.isTrigger && transform.position.y <= minimumTerrainImpactY)
        {
            Impact();
        }
    }

    private void Impact()
    {
        if (hasImpacted) return;
        hasImpacted = true;
        if (ImpactFX.instance != null) ImpactFX.instance.CameraShake();
        Destroy(gameObject);
    }
}
