using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Cổng chỉ hoạt động sau khi người chơi đã lấy Chìa Khóa Cổ của màn hiện tại.
/// </summary>
public class Portal : MonoBehaviour
{
    private SpriteRenderer portalRenderer;
    private Enemy keyGuardian;
    private KeyPickup levelKey;
    private bool guardianDefeated;
    private bool guardianWasAssigned;
    [SerializeField] private float npcObjectiveRange = 5.5f;
    private Transform playerTransform;
    private DialogueManager[] instructors;
    private ObjectivePanel objectivePanel;

    private void Awake()
    {
        // Mỗi scene có một chìa khóa riêng; không mang chìa khóa cũ sang màn mới.
        KeyPickup.ResetForNewLevel();
        portalRenderer = GetComponent<SpriteRenderer>();
        UpdatePortalAppearance();
    }

    private void Update()
    {
        CheckGuardianState();
        UpdatePortalAppearance();
        UpdateObjectivePanel();
    }

    private void Start()
    {
        SetupKeyGuardian();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = player != null ? player.transform : null;
        instructors = FindObjectsByType<DialogueManager>();
        foreach (ObjectivePanel panel in FindObjectsByType<ObjectivePanel>())
        {
            if (panel.gameObject.scene == gameObject.scene)
            {
                objectivePanel = panel;
                break;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D playerCollider)
    {
        if (!playerCollider.CompareTag("Player"))
        {
            return;
        }

        if (!KeyPickup.HasKey)
        {
            Debug.Log("Cổng đang khóa: hãy tìm Chìa Khóa Cổ.");
            return;
        }

        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        GameProgress.SaveCheckpoint(nextSceneIndex);
        SceneManager.LoadScene(nextSceneIndex);
    }

    private void UpdatePortalAppearance()
    {
        if (portalRenderer != null)
        {
            portalRenderer.color = KeyPickup.HasKey
                ? Color.white
                : new Color(0.42f, 0.42f, 0.42f, 1f);
        }
    }

    private void SetupKeyGuardian()
    {
        foreach (KeyPickup pickup in Resources.FindObjectsOfTypeAll<KeyPickup>())
        {
            if (pickup.gameObject.scene == gameObject.scene)
            {
                levelKey = pickup;
                levelKey.gameObject.SetActive(false);
                break;
            }
        }

        string guardianName = SceneManager.GetActiveScene().name switch
        {
            "Scene_1" => "Knight",
            "Scene_2" => "Bee",
            "Scene_3" => "Vampire",
            _ => string.Empty
        };

        if (string.IsNullOrEmpty(guardianName))
        {
            guardianDefeated = true;
            if (levelKey != null) levelKey.gameObject.SetActive(true);
            return;
        }

        Enemy[] enemies = FindObjectsByType<Enemy>();
        float nearestDistance = float.MaxValue;
        foreach (Enemy enemy in enemies)
        {
            if (!enemy.name.StartsWith(guardianName))
            {
                continue;
            }

            float distance = Vector2.Distance(enemy.transform.position, transform.position);
            if (distance < nearestDistance)
            {
                keyGuardian = enemy;
                guardianWasAssigned = true;
                nearestDistance = distance;
            }
        }

        if (keyGuardian == null)
        {
            guardianDefeated = true;
            if (levelKey != null) levelKey.gameObject.SetActive(true);
            return;
        }

        if (SceneManager.GetActiveScene().name == "Scene_2")
        {
            // Reuse the existing Bee and place it in the final arena, rather
            // than creating an extra enemy elsewhere in the level.
            Vector3 guardianPosition = keyGuardian.transform.position;
            guardianPosition.x = transform.position.x - 4f;
            guardianPosition.y = transform.position.y + 2.7f;
            keyGuardian.transform.position = guardianPosition;

            Patrol patrol = keyGuardian.GetComponent<Patrol>();
            if (patrol != null)
            {
                patrol.speed = 0.8f;
            }
        }

        // Tăng HP đủ rõ rệt để người chơi nhận ra đây là miniboss, nhưng vẫn phù hợp
        // với sát thương cơ bản 100 của nhân vật.
        // The Map 2 Bee is the first guardian that must clearly outscale the
        // Map 1 Knight. It has no clones, so its health pool and enraged
        // contact damage carry the extra difficulty instead.
        if (SceneManager.GetActiveScene().name == "Scene_2")
        {
            keyGuardian.health = Mathf.Max(keyGuardian.health * 8, 1800);
        }
        else
        {
            keyGuardian.health = Mathf.Max(keyGuardian.health * 3, 900);
        }
        MinibossController miniboss = keyGuardian.GetComponent<MinibossController>();
        if (miniboss == null)
        {
            miniboss = keyGuardian.gameObject.AddComponent<MinibossController>();
        }
        miniboss.ConfigureGuardian();
        SpriteRenderer guardianSprite = keyGuardian.GetComponent<SpriteRenderer>();
        if (guardianSprite != null)
        {
            guardianSprite.color = new Color(1f, 0.45f, 0.45f);
        }
    }

    private void CheckGuardianState()
    {
        if (guardianDefeated || (!guardianWasAssigned && keyGuardian == null) ||
            (keyGuardian != null && keyGuardian.health > 0))
        {
            return;
        }

        guardianDefeated = true;
        if (levelKey != null)
        {
            // Chìa khóa xuất hiện đúng tại nơi miniboss bị hạ, không ở vị trí đặt sẵn.
            if (keyGuardian != null)
            {
                levelKey.PlaceAt(keyGuardian.transform.position);
            }
            levelKey.gameObject.SetActive(true);
            Debug.Log("Kẻ Canh Giữ đã bị đánh bại. Chìa Khóa Cổ đã xuất hiện.");
        }
    }

    private void UpdateObjectivePanel()
    {
        if (objectivePanel == null || SceneManager.GetActiveScene().buildIndex == 0)
        {
            return;
        }

        if (!IsPlayerNearInstructor())
        {
            objectivePanel.SetVisible(false);
            return;
        }

        string objective = KeyPickup.HasKey
            ? "Chìa khóa đã có — đi đến cổng để qua màn"
            : !guardianDefeated && keyGuardian != null
                ? "Mục tiêu: Hạ gục Kẻ Canh Giữ để lấy chìa khóa"
                : "Mục tiêu: Tìm Chìa Khóa Cổ để mở cổng";
        objectivePanel.Show(objective);
    }

    private bool IsPlayerNearInstructor()
    {
        if (playerTransform == null || instructors == null)
        {
            return false;
        }

        foreach (DialogueManager instructor in instructors)
        {
            if (instructor != null && Vector2.Distance(playerTransform.position, instructor.transform.position) <= npcObjectiveRange)
            {
                return true;
            }
        }

        return false;
    }
}
