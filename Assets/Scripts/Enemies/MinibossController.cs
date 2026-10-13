using System.Collections;
using UnityEngine;

/// <summary>
/// Nâng một Enemy thông thường thành miniboss. Khi còn ít máu, miniboss tăng sức mạnh
/// và gọi bốn phân thân. Phân thân không thể tự gọi thêm phân thân.
/// </summary>
public class MinibossController : MonoBehaviour
{
    [SerializeField] private float phaseTwoThreshold = 0.45f;
    [SerializeField] private float phaseTwoSpeedMultiplier = 1.55f;
    [SerializeField] private int phaseTwoDamageMultiplier = 2;

    private Enemy enemy;
    private BossWeapon weapon;
    private Patrol patrol;
    private SpriteRenderer spriteRenderer;
    private int startingHealth;
    private bool phaseTwo;
    private bool isClone;
    private bool isPatrolGuardian;
    private bool isVampireGuardian;
    private bool phaseThree;
    private Vector3 guardianHome;
    private Vector3 originalScale;
    private Transform player;
    private float nextPhaseThreeTeleport;
    private float nextPatrolDash;
    private bool patrolDashRunning;
    private float cloneProtectionUntil;
    private float nextVampireEvade;
    private float vampireRecoveryUntil;

    public float MovementMultiplier => isVampireGuardian && Time.time < vampireRecoveryUntil
        ? 0f
        : isVampireGuardian && phaseThree ? 3.2f : phaseTwo ? phaseTwoSpeedMultiplier : 1f;
    // Map 2's Bee is deliberately stronger than the Map 1 Knight guardian:
    // it trades the Knight's clones for much faster, harder contact attacks.
    public float PatrolDamageMultiplier => isPatrolGuardian && phaseTwo ? 3f : 1f;
    public bool IsVampireGuardian => isVampireGuardian;
    public float AttackRangeMultiplier => isVampireGuardian && phaseThree ? 1.35f : 1f;

    public int ModifyIncomingDamage(int damage)
    {
        if (!isClone && !isPatrolGuardian && !isVampireGuardian && Time.time < cloneProtectionUntil)
        {
            return Mathf.Max(1, Mathf.CeilToInt(damage * 0.5f));
        }
        return damage;
    }

    public void ConfigureGuardian()
    {
        CacheComponents();
        startingHealth = enemy != null ? enemy.health : 1;
        isPatrolGuardian = CompareTag("Patrols");
        isVampireGuardian = name.StartsWith("Vampire");
        // The prefab's original 200 damage can one-shot the 200 HP player.
        // Use a guardian baseline that still increases clearly through phases.
        if (isVampireGuardian && weapon != null)
        {
            weapon.attackDamage = 90;
        }
        guardianHome = transform.position;
        originalScale = transform.localScale;
        PlayerController playerController = FindAnyObjectByType<PlayerController>();
        player = playerController != null ? playerController.transform : null;
    }

    private void LateUpdate()
    {
        // The Map 2 Bee may patrol, but must remain in the final arena instead
        // of wandering across the map or disappearing from the fight.
        if (isPatrolGuardian && Mathf.Abs(transform.position.x - guardianHome.x) > 4f)
        {
            Vector3 position = transform.position;
            position.x = guardianHome.x;
            transform.position = position;
        }

        if (isPatrolGuardian && phaseTwo && !patrolDashRunning && Time.time >= nextPatrolDash)
        {
            StartCoroutine(PatrolDashAttack());
        }

        if (isVampireGuardian && phaseThree && Time.time >= nextPhaseThreeTeleport)
        {
            TeleportBehindPlayer(2.2f);
            nextPhaseThreeTeleport = Time.time + 2.0f;
        }
    }

    public void EvaluatePhase(int remainingHealth)
    {
        if (isClone || startingHealth <= 0 || remainingHealth <= 0)
        {
            return;
        }

        if (isVampireGuardian)
        {
            EvaluateVampirePhases(remainingHealth);
            return;
        }

        float threshold = isPatrolGuardian ? 0.50f : phaseTwoThreshold;
        if (phaseTwo || remainingHealth > startingHealth * threshold) return;

        phaseTwo = true;
        if (weapon != null)
        {
            weapon.attackDamage *= phaseTwoDamageMultiplier;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(0.72f, 0.15f, 1f);
        }

        if (isPatrolGuardian)
        {
            // Map 2's flying Bee becomes enraged instead of copying the Map 1 clone mechanic.
            if (patrol != null)
            {
                patrol.speed *= 2.8f;
            }

            if (spriteRenderer != null)
            {
                spriteRenderer.color = new Color(1f, 0.42f, 0.05f);
            }

            transform.localScale *= 1.3f;
            nextPatrolDash = Time.time + 0.9f;
            Debug.Log("Bee Guardian entered Stinger Rage: speed and contact damage increased.");
            return;
        }

        cloneProtectionUntil = Time.time + 4f;
        SpawnClones();
        Debug.Log("Kẻ Canh Giữ đã vào Phase 2: triệu hồi bốn phân thân!");
    }

    private void EvaluateVampirePhases(int remainingHealth)
    {
        if (!phaseTwo && remainingHealth <= startingHealth * 0.60f)
        {
            phaseTwo = true;
            transform.localScale = originalScale * 1.45f;
            if (weapon != null) weapon.attackDamage = Mathf.RoundToInt(weapon.attackDamage * 1.35f);
            if (spriteRenderer != null) spriteRenderer.color = new Color(0.82f, 0.18f, 1f);
            TeleportEvade(3f);
            return;
        }

        if (phaseTwo && !phaseThree && remainingHealth <= startingHealth * 0.25f)
        {
            phaseThree = true;
            transform.localScale = originalScale * 0.68f;
            if (weapon != null) weapon.attackDamage = Mathf.RoundToInt(weapon.attackDamage * 1.2f);
            if (spriteRenderer != null) spriteRenderer.color = new Color(0.25f, 0.9f, 1f);
            nextPhaseThreeTeleport = Time.time + 1.15f;
            TeleportBehindPlayer(2.1f);
            return;
        }

        // Dodging every successful hit felt unfair. Later phases now use a
        // readable chance plus cooldown, so good attacks can still connect.
        if (phaseThree) TryVampireEvade(0.70f, 0.70f);
        else if (phaseTwo) TryVampireEvade(0.55f, 1.0f);
    }

    private void TryVampireEvade(float chance, float cooldown)
    {
        if (Time.time < nextVampireEvade || Random.value > chance) return;
        nextVampireEvade = Time.time + cooldown;
        TeleportEvade(3f);
    }

    private bool TryGetPlayer()
    {
        if (player == null)
        {
            PlayerController playerController = FindAnyObjectByType<PlayerController>();
            player = playerController != null ? playerController.transform : null;
        }
        return player != null;
    }

    // Used in Phase 2 after an arrow or sword connects: the boss vanishes out
    // of the player's current attack line before the next strike can land.
    private void TeleportEvade(float distance)
    {
        if (!TryGetPlayer()) return;

        CreateAfterImage();
        float direction = Mathf.Sign(transform.position.x - player.position.x);
        if (Mathf.Approximately(direction, 0f)) direction = Random.value < 0.5f ? -1f : 1f;

        Vector3 position = transform.position;
        position.x = player.position.x + direction * distance;
        transform.position = position;
    }

    // Used in Phase 3: crosses through the player and reappears at their back
    // so the next melee attack comes from the opposite side.
    private void TeleportBehindPlayer(float distance)
    {
        if (!TryGetPlayer()) return;

        CreateAfterImage();
        float fromBoss = Mathf.Sign(transform.position.x - player.position.x);
        if (Mathf.Approximately(fromBoss, 0f)) fromBoss = Random.value < 0.5f ? -1f : 1f;

        Vector3 position = transform.position;
        position.x = player.position.x - fromBoss * distance;
        transform.position = position;
        vampireRecoveryUntil = Time.time + 0.4f;
    }

    private void CreateAfterImage()
    {
        if (spriteRenderer == null || spriteRenderer.sprite == null) return;

        GameObject afterImage = new GameObject("Vampire Blink Afterimage");
        afterImage.transform.position = transform.position;
        afterImage.transform.localScale = transform.lossyScale;
        SpriteRenderer renderer = afterImage.AddComponent<SpriteRenderer>();
        renderer.sprite = spriteRenderer.sprite;
        renderer.flipX = spriteRenderer.flipX;
        renderer.sortingLayerID = spriteRenderer.sortingLayerID;
        renderer.sortingOrder = spriteRenderer.sortingOrder - 1;
        renderer.color = new Color(0.25f, 0.9f, 1f, 0.55f);
        Destroy(afterImage, 0.22f);
    }

    public void ConfigureClone(int cloneHealth)
    {
        isClone = true;
        phaseTwo = true;
        CacheComponents();

        if (enemy != null)
        {
            enemy.health = cloneHealth;
            enemy.maxCoins = 0;
            enemy.maxHearts = 0;
            enemy.maxSwords = 0;
            enemy.maxShields = 0;
        }

        if (weapon != null)
        {
            // Clone được tạo từ boss Phase 2 nên đã kế thừa damage buff.
            // Giảm mạnh để chúng gây áp lực bằng số lượng, không phải one-shot người chơi.
            weapon.attackDamage = Mathf.Max(5, Mathf.RoundToInt(weapon.attackDamage * 0.35f));
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(0.55f, 0.2f, 1f);
        }
    }

    private void SpawnClones()
    {
        int cloneHealth = Mathf.Max(120, startingHealth / 4);
        Vector2[] offsets =
        {
            new Vector2(-2.2f, 0.2f), new Vector2(2.2f, 0.2f),
            new Vector2(-1.1f, 1.1f), new Vector2(1.1f, 1.1f)
        };

        foreach (Vector2 offset in offsets)
        {
            GameObject clone = Instantiate(gameObject, (Vector2)transform.position + offset, transform.rotation);
            clone.name = "Phân thân Kẻ Canh Giữ";
            clone.GetComponent<MinibossController>().ConfigureClone(cloneHealth);
        }
    }

    private IEnumerator PatrolDashAttack()
    {
        if (!TryGetPlayer())
        {
            nextPatrolDash = Time.time + 1f;
            yield break;
        }

        patrolDashRunning = true;
        Vector3 start = transform.position;
        float side = Mathf.Sign(start.x - player.position.x);
        if (Mathf.Approximately(side, 0f)) side = Random.value < 0.5f ? -1f : 1f;
        Vector3 lockedPlayerPosition = player.position;
        Vector3 target = lockedPlayerPosition + Vector3.right * side * 0.55f;
        // The Bee normally patrols horizontally above the arena, but its
        // special dash must descend to the player's actual height.
        target.y = lockedPlayerPosition.y;
        target.z = start.z;

        // Telegraph the dash before moving so the player can react.
        Color warningColor = new Color(1f, 0.85f, 0.2f);
        Color rageColor = new Color(1f, 0.42f, 0.05f);
        float warningElapsed = 0f;
        while (warningElapsed < 0.45f)
        {
            warningElapsed += Time.deltaTime;
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Mathf.FloorToInt(warningElapsed * 14f) % 2 == 0
                    ? warningColor
                    : rageColor;
            }
            yield return null;
        }
        if (spriteRenderer != null) spriteRenderer.color = rageColor;

        // Short visible lunge instead of an instant hit.
        float elapsed = 0f;
        while (elapsed < 0.18f)
        {
            if (player == null) break;
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(start, target, elapsed / 0.18f);
            yield return null;
        }

        transform.position = target;
        PlayerController playerController = player != null
            ? player.GetComponent<PlayerController>()
            : null;
        Stats playerStats = player != null
            ? player.GetComponentInParent<Stats>()
            : null;
        if (playerStats != null)
        {
            int dashDamage = playerController != null
                ? Mathf.RoundToInt(playerController.damagePatrols * PatrolDamageMultiplier)
                : 75;
            playerStats.takeDamage(dashDamage);
        }

        yield return new WaitForSeconds(0.5f);
        if (player != null)
        {
            Vector3 retreat = player.position + Vector3.right * -side * 3.2f;
            retreat.y = guardianHome.y;
            retreat.z = start.z;
            transform.position = retreat;
        }

        patrolDashRunning = false;
        nextPatrolDash = Time.time + 2.7f;
    }

    private void CacheComponents()
    {
        if (enemy == null) enemy = GetComponent<Enemy>();
        if (weapon == null) weapon = GetComponent<BossWeapon>();
        if (patrol == null) patrol = GetComponent<Patrol>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
    }
}
