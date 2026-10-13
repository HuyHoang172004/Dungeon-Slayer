using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Three-phase encounter AI used only by the final WizardBoss.
/// </summary>
[RequireComponent(typeof(Enemy))]
public class FinalWizardBossAI : MonoBehaviour
{
    private Enemy enemy;
    private MageWeapon mageWeapon;
    private SpriteRenderer spriteRenderer;
    private Transform player;
    private Vector3 originalScale;
    private int maxHealth;
    private int lastHealth;
    private int phase = 1;
    private float nextFanTime;
    private float nextReactiveBlink;
    private bool phaseThreeLoopRunning;
    [SerializeField] private Sprite meteorRockSprite;
    private Sprite warningSprite;

    private void Start()
    {
        enemy = GetComponent<Enemy>();
        mageWeapon = GetComponent<MageWeapon>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject != null ? playerObject.transform : null;
        originalScale = transform.localScale;
        maxHealth = Mathf.Max(1, enemy.health);
        lastHealth = enemy.health;
    }

    private void Update()
    {
        if (enemy == null || enemy.health <= 0) return;
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            player = playerObject != null ? playerObject.transform : null;
            if (player == null) return;
        }

        UpdatePhase();

        // A successful sword swing or arrow hit makes later phases react,
        // rather than letting the player repeat the same attack safely.
        if (enemy.health < lastHealth && phase >= 2)
        {
            if (phase == 2) BlinkAway();
            else if (Time.time >= nextReactiveBlink)
            {
                BlinkBehindPlayer();
                nextReactiveBlink = Time.time + 2.2f;
            }
        }
        lastHealth = enemy.health;

        if (phase == 2 && Time.time >= nextFanTime)
        {
            FireFan();
            nextFanTime = Time.time + 3.2f;
        }

        if (phase == 3 && !phaseThreeLoopRunning)
            StartCoroutine(PhaseThreeCombatLoop());
    }

    private void UpdatePhase()
    {
        if (phase == 1 && enemy.health <= maxHealth * 0.60f)
        {
            phase = 2;
            transform.localScale = originalScale * 1.2f;
            if (spriteRenderer != null) spriteRenderer.color = new Color(0.8f, 0.25f, 1f);
            BlinkBehindPlayer();
            nextFanTime = Time.time + 0.8f;
        }
        else if (phase == 2 && enemy.health <= maxHealth * 0.30f)
        {
            phase = 3;
            transform.localScale = originalScale * 0.82f;
            if (spriteRenderer != null) spriteRenderer.color = new Color(0.2f, 0.9f, 1f);
            nextReactiveBlink = Time.time + 1.2f;
            BlinkBehindPlayer();
            Debug.Log("Wizard Boss entered Phase 3: meteor shower enabled.");
        }
    }

    private void BlinkAway()
    {
        if (player == null) return;
        float direction = Mathf.Sign(transform.position.x - player.position.x);
        if (Mathf.Approximately(direction, 0f)) direction = Random.value < 0.5f ? -1f : 1f;
        BlinkTo(new Vector2(player.position.x + direction * 5f, transform.position.y));
    }

    private void BlinkBehindPlayer()
    {
        if (player == null) return;
        float side = Mathf.Sign(transform.position.x - player.position.x);
        if (Mathf.Approximately(side, 0f)) side = Random.value < 0.5f ? -1f : 1f;
        BlinkTo(new Vector2(player.position.x - side * 2.2f, transform.position.y));
    }

    private void BlinkTo(Vector2 position)
    {
        CreateAfterImage();
        transform.position = new Vector3(position.x, position.y, transform.position.z);
    }

    private void FireFan()
    {
        if (mageWeapon == null || mageWeapon.ballPrefab == null || player == null) return;
        Vector2 baseDirection = ((Vector2)player.position - (Vector2)transform.position).normalized;
        for (int i = -1; i <= 1; i++)
        {
            Vector2 direction = Quaternion.Euler(0f, 0f, i * 20f) * baseDirection;
            GameObject ball = Instantiate(mageWeapon.ballPrefab, transform.position, Quaternion.identity);
            MageBall mageBall = ball.GetComponent<MageBall>();
            if (mageBall != null)
            {
                mageBall.damage = 20;
                mageBall.SetDirection(direction);
            }
        }
    }

    private IEnumerator PhaseThreeCombatLoop()
    {
        phaseThreeLoopRunning = true;
        while (phase == 3 && enemy != null && enemy.health > 0)
        {
            // Warning -> meteor wave -> recovery -> fan -> blink -> recovery.
            // Only one major pattern runs at once, so every attack is dodgeable.
            yield return StartCoroutine(MeteorShower());
            yield return new WaitForSeconds(0.65f);
            FireFan();
            yield return new WaitForSeconds(0.9f);
            BlinkBehindPlayer();
            yield return new WaitForSeconds(1.0f);
        }
        phaseThreeLoopRunning = false;
    }

    private IEnumerator MeteorShower()
    {
        Sprite rockSprite = GetMeteorRockSprite();
        if (player == null) yield break;

        List<float> impactX = new List<float>();
        float centerX = player.position.x;
        int safeStart = Random.Range(1, 7);
        for (int i = 0; i < 9; i++)
        {
            // Nine fixed lanes with two adjacent lanes omitted leave a clear
            // safe zone. The remaining seven lanes receive rocks.
            if (i == safeStart || i == safeStart + 1) continue;
            float x = centerX + (i - 4) * 1.8f;
            impactX.Add(x);
            CreateMeteorWarning(new Vector3(x, player.position.y + 1.35f, player.position.z));
        }

        // Give the player time to read the warning before the whole wave falls.
        yield return new WaitForSeconds(0.8f);
        foreach (float x in impactX)
        {
            GameObject rock = new GameObject("Wizard Meteor Rock");
            rock.layer = gameObject.layer;
            rock.transform.position = new Vector3(x, player.position.y + 4.8f, player.position.z);
            rock.transform.localScale = Vector3.one * 0.68f;
            SpriteRenderer renderer = rock.AddComponent<SpriteRenderer>();
            renderer.sprite = rockSprite;
            renderer.color = Color.white;
            renderer.sortingLayerID = spriteRenderer != null ? spriteRenderer.sortingLayerID : 0;
            renderer.sortingOrder = spriteRenderer != null
                ? Mathf.Max(spriteRenderer.sortingOrder + 10, 50)
                : 50;
            Rigidbody2D body = rock.AddComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Kinematic;
            body.gravityScale = 0f;
            body.freezeRotation = true;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            CircleCollider2D collider = rock.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = 1.2f;
            MeteorRock meteor = rock.AddComponent<MeteorRock>();
            meteor.Initialize(player.position.y + 0.75f);
        }

        if (ImpactFX.instance != null) ImpactFX.instance.CameraShake();
        Debug.Log("Wizard Boss: meteor shower launched (7 rocks).");
        yield return new WaitForSeconds(1.2f);
    }

    private Sprite GetMeteorRockSprite()
    {
        if (meteorRockSprite != null) return meteorRockSprite;

        meteorRockSprite = Resources.Load<Sprite>("Boss/MeteorRock-v2");
        if (meteorRockSprite != null) return meteorRockSprite;

        // Runtime fallback keeps the attack visible if Unity has not finished
        // importing the optional PNG yet.
        Texture2D texture = new Texture2D(16, 16, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;
        for (int y = 0; y < 16; y++)
        {
            for (int x = 0; x < 16; x++)
            {
                int distance = Mathf.Abs(x - 8) + Mathf.Abs(y - 8);
                texture.SetPixel(x, y, distance < 8
                    ? new Color(0.25f, 0.08f, 0.4f, 1f)
                    : Color.clear);
            }
        }
        texture.Apply();
        meteorRockSprite = Sprite.Create(texture, new Rect(0, 0, 16, 16),
            new Vector2(0.5f, 0.5f), 16f);
        Debug.LogWarning("MeteorRock.png chưa được Unity import; dùng sprite dự phòng.");
        return meteorRockSprite;
    }

    private void CreateMeteorWarning(Vector3 position)
    {
        GameObject warning = new GameObject("Meteor Warning");
        warning.layer = gameObject.layer;
        warning.transform.position = position;
        if (warningSprite == null)
        {
            Texture2D texture = new Texture2D(8, 12, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Point;
            for (int y = 0; y < 12; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    bool mark = (y >= 2 && y <= 8 && x >= 3 && x <= 4) ||
                        (y >= 9 && x >= 3 && x <= 4);
                    texture.SetPixel(x, y, mark ? Color.red : Color.clear);
                }
            }
            texture.Apply();
            warningSprite = Sprite.Create(texture, new Rect(0, 0, 8, 12),
                new Vector2(0.5f, 0.5f), 8f);
        }
        SpriteRenderer renderer = warning.AddComponent<SpriteRenderer>();
        renderer.sprite = warningSprite;
        renderer.sortingLayerID = spriteRenderer != null ? spriteRenderer.sortingLayerID : 0;
        renderer.sortingOrder = spriteRenderer != null ? spriteRenderer.sortingOrder + 4 : 30;
        warning.transform.localScale = Vector3.one * 2.5f;
        Destroy(warning, 0.8f);
    }

    private void CreateAfterImage()
    {
        if (spriteRenderer == null || spriteRenderer.sprite == null) return;
        GameObject afterImage = new GameObject("Wizard Blink Afterimage");
        afterImage.layer = gameObject.layer;
        afterImage.transform.position = transform.position;
        afterImage.transform.localScale = transform.lossyScale;
        SpriteRenderer renderer = afterImage.AddComponent<SpriteRenderer>();
        renderer.sprite = spriteRenderer.sprite;
        renderer.flipX = spriteRenderer.flipX;
        renderer.sortingLayerID = spriteRenderer.sortingLayerID;
        renderer.sortingOrder = spriteRenderer.sortingOrder - 1;
        renderer.color = new Color(0.35f, 0.85f, 1f, 0.6f);
        Destroy(afterImage, 0.2f);
    }
}
