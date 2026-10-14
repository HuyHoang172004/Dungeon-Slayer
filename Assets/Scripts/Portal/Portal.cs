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
    [SerializeField] private float eventMessageDuration = 6f;
    private Transform playerTransform;
    private DialogueManager[] instructors;
    private ObjectivePanel objectivePanel;
    private string guardianDisplayName = "Kẻ Canh Giữ";
    private string keyDisplayName = "Chìa Khóa Cổ";
    private Color keyColor = Color.white;
    private Sprite keyIconSprite;
    private string eventMessage;
    private float eventMessageUntil;
    private bool keyPickupNotified;

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

        string guardianName;
        switch (SceneManager.GetActiveScene().name)
        {
            case "Scene_1":
                guardianName = "Knight";
                guardianDisplayName = "Knight";
                keyDisplayName = "Chìa khóa Lòng Dũng Cảm";
                keyColor = new Color(1f, 0.78f, 0.16f, 1f);
                break;
            case "Scene_2":
                guardianName = "Bee";
                guardianDisplayName = "Bee";
                keyDisplayName = "Chìa khóa Sự Sống";
                keyColor = new Color(0.3f, 1f, 0.38f, 1f);
                break;
            case "Scene_3":
                guardianName = "Vampire";
                guardianDisplayName = "Vampire";
                keyDisplayName = "Chìa khóa Ý Chí";
                keyColor = new Color(0.72f, 0.38f, 1f, 1f);
                break;
            default:
                guardianName = string.Empty;
                break;
        }

        if (levelKey != null)
        {
            levelKey.SetVisualColor(keyColor);
            keyIconSprite = levelKey.IconSprite;
        }

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
            ShowEventMessage(
                $"{guardianDisplayName} đã được giải thoát!\n{keyDisplayName} đã xuất hiện.");
            Debug.Log($"{guardianDisplayName} đã được giải thoát. {keyDisplayName} đã xuất hiện.");
        }
    }

    private void UpdateObjectivePanel()
    {
        if (objectivePanel == null || SceneManager.GetActiveScene().buildIndex == 0)
        {
            return;
        }

        if (KeyPickup.HasKey && !keyPickupNotified)
        {
            keyPickupNotified = true;
            ShowEventMessage($"Đã thu hồi {keyDisplayName}!\nHãy đến cổng để tiếp tục hành trình.");
        }

        if (Time.unscaledTime < eventMessageUntil)
        {
            objectivePanel.ShowEvent(eventMessage, keyIconSprite, keyColor);
            return;
        }

        if (!IsPlayerNearInstructor())
        {
            objectivePanel.SetVisible(false);
            return;
        }

        string objective = KeyPickup.HasKey
            ? $"ĐÃ THU HỒI {keyDisplayName}\nHãy đến cổng để qua màn."
            : !guardianDefeated && keyGuardian != null
                ? $"MỤC TIÊU\nGiải thoát {guardianDisplayName} và thu hồi {keyDisplayName}."
                : $"MỤC TIÊU\nThu hồi {keyDisplayName} vừa xuất hiện.";
        objectivePanel.Show(objective);
    }

    private void ShowEventMessage(string message)
    {
        eventMessage = message;
        eventMessageUntil = Time.unscaledTime + eventMessageDuration;
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
