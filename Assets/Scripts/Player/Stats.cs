using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Stats : MonoBehaviour {
    
    [Header("Core Stats")]
    public int health = 200;
    public int power = 0;
    public int attackDamage = 100;
    public int defense = 0;

    [Header("Testing")]
    [SerializeField] private bool invincibleForBossTesting = false;

    [Header("References")]
    public GameObject camera;
    public GameObject stats;

    [Header("Hearts (Do Not Rename)")]
    public Image hearts;
    public Sprite fullHeart;
    public Sprite heart190, heart180, heart170, heart160, heart150, heart140, heart130, heart120, heart110;
    public Sprite heart100, heart90, heart80, heart70, heart60, heart50, heart40, heart30, heart20, heart10;
    public Sprite emptyHeart;

    [Header("Powers (Do Not Rename)")]
    public Image powers;
    public Sprite fullPower;
    public Sprite power75, power50, power25;
    public Sprite emptyPower;

    [Header("UI Text & Audio")]
    public TextMeshProUGUI textDamage;
    public TextMeshProUGUI textDefense;
    public GameObject deathSound;
    public GameObject damageSound;

    public static Stats instance;
    private bool isDeadFlag = false;
    private Coroutine hitFlashRoutine;

    private Animator anim;
    private PlayerController playerController;

    private void Awake() {
        if (instance == null) {
            instance = this;
        }

        anim = GetComponent<Animator>();
        playerController = GetComponentInParent<PlayerController>();
    }

    private void Start() {
        // Load saved data
        int savedAttack = PlayerPrefs.GetInt("AttackDamage", 0);
        if (savedAttack >= 100) {
            attackDamage = savedAttack;
        }
        defense = PlayerPrefs.GetInt("Defense", 0);

        UpdateStatTextUI();
        UpdateHealthUI();
        UpdatePowerUI();
    }

    [Header("Awakening (Limit Break)")]
    public float awakeningDuration = 10f;
    private bool isAwakened = false;

    private void Update() {
        if (!isAwakened && power >= 4) {
            if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.JoystickButton1)) {
                ActivateAwakening();
            }
        }
    }

    public void ActivateAwakeningButton() {
        if (!isAwakened && power >= 4) {
            ActivateAwakening();
        }
    }

    private void ActivateAwakening() {
        isAwakened = true;
        power = 0;
        UpdatePowerUI();

        StartCoroutine(AwakeningRoutine());
    }

    private System.Collections.IEnumerator AwakeningRoutine() {
        // Boost stats 
        int damageBuff = attackDamage;
        attackDamage += damageBuff; // double damage
        defense += 50; // extra defense
        
        // Aura glow
        SpriteRenderer sr = playerController != null ? playerController.GetComponent<SpriteRenderer>() : null;
        Color originalColor = Color.white;
        if (sr != null) {
            originalColor = sr.color;
            sr.color = new Color(1f, 0.4f, 0.4f); // Red aura
        }

        yield return new WaitForSeconds(awakeningDuration);

        // Turn off
        attackDamage -= damageBuff; // Revert strictly the buffed amount
        defense -= 50;
        if (sr != null) {
            sr.color = originalColor;
        }

        isAwakened = false;
    }

    private void HandleDeath() {
        if (isDeadFlag) return;
        isDeadFlag = true;

        if (anim != null) anim.SetBool("die", true);
        if (playerController != null) playerController.isDeadStatus();

        if (camera != null) camera.SetActive(true);
        if (stats != null) stats.SetActive(false);

        if (deathSound != null) {
            Instantiate(deathSound);
        }
    }

    private void UpdateHealthUI() {
        if (hearts == null) return;

        switch (health) {
            case int n when (n >= 200):            hearts.sprite = fullHeart; break;
            case int n when (n >= 190 && n < 200): hearts.sprite = heart190; break;
            case int n when (n >= 180 && n < 190): hearts.sprite = heart180; break;
            case int n when (n >= 170 && n < 180): hearts.sprite = heart170; break;
            case int n when (n >= 160 && n < 170): hearts.sprite = heart160; break;
            case int n when (n >= 150 && n < 160): hearts.sprite = heart150; break;
            case int n when (n >= 140 && n < 150): hearts.sprite = heart140; break;
            case int n when (n >= 130 && n < 140): hearts.sprite = heart130; break;
            case int n when (n >= 120 && n < 130): hearts.sprite = heart120; break;
            case int n when (n >= 110 && n < 120): hearts.sprite = heart110; break;
            case int n when (n >= 100 && n < 110): hearts.sprite = heart100; break;
            case int n when (n >= 90 && n < 100):  hearts.sprite = heart90; break;
            case int n when (n >= 80 && n < 90):   hearts.sprite = heart80; break;
            case int n when (n >= 70 && n < 80):   hearts.sprite = heart70; break;
            case int n when (n >= 60 && n < 70):   hearts.sprite = heart60; break;
            case int n when (n >= 50 && n < 60):   hearts.sprite = heart50; break;
            case int n when (n >= 40 && n < 50):   hearts.sprite = heart40; break;
            case int n when (n >= 30 && n < 40):   hearts.sprite = heart30; break;
            case int n when (n >= 20 && n < 30):   hearts.sprite = heart20; break;
            case int n when (n >= 10 && n < 20):   hearts.sprite = heart10; break;
            case int n when (n < 10):              hearts.sprite = emptyHeart; break;
        }
    }

    private void UpdatePowerUI() {
        if (powers == null) return;
        
        switch (power) {
            case 4: powers.sprite = fullPower; break;
            case 3: powers.sprite = power75; break;
            case 2: powers.sprite = power50; break;
            case 1: powers.sprite = power25; break;
            case 0: powers.sprite = emptyPower; break;
        }
    }

    private void UpdateStatTextUI() {
        if (textDamage != null) textDamage.text = "+" + attackDamage.ToString();
        if (textDefense != null) textDefense.text = "+" + defense.ToString();
    }

    public void takeDamage(int value) {
        if (invincibleForBossTesting) {
            return;
        }

        // Defense must mitigate damage, not make the player permanently immune.
        // At least 20% of every normal hit gets through, so collected shields
        // remain useful without disabling combat across all maps.
        int minimumDamage = Mathf.Max(1, Mathf.CeilToInt(value * 0.20f));
        int actualDamage = Mathf.Max(minimumDamage, value - defense);
        health -= actualDamage;

        if (hitFlashRoutine != null) StopCoroutine(hitFlashRoutine);
        hitFlashRoutine = StartCoroutine(HitFlash());
        
        if (damageSound != null) {
            Instantiate(damageSound);
        }

        if (health <= 0) {
            HandleDeath();
        } else {
            UpdateHealthUI();
        }
    }

    public void takeTrueDamage(int value) {
        if (invincibleForBossTesting) {
            return;
        }

        health -= value;
        if (hitFlashRoutine != null) StopCoroutine(hitFlashRoutine);
        hitFlashRoutine = StartCoroutine(HitFlash());
        if (health <= 0) {
            HandleDeath();
        } else {
            UpdateHealthUI();
        }
    }

    public void takePower(int value) {
        power += value;
        if (power > 4) power = 4;
        UpdatePowerUI();
    }

    public int getHealth() {
        return health;
    }

    public int getPower() {
        return power;
    }

    public void setHealth(int value) {
        health += value;
        if (health >= 200) {
            health = 200;
        }
        UpdateHealthUI();
    }

    public void addAttackDamage(int value) {
        attackDamage += value;
        PlayerPrefs.SetInt("AttackDamage", attackDamage);
        UpdateStatTextUI();
    }

    public int getAttackDamage() {
        return attackDamage;
    }

    public void addDefense(int value) {
        defense += value;
        PlayerPrefs.SetInt("Defense", defense);
        UpdateStatTextUI();
    }

    private System.Collections.IEnumerator HitFlash()
    {
        SpriteRenderer renderer = playerController != null ? playerController.GetComponent<SpriteRenderer>() : null;
        if (renderer == null) yield break;

        Color originalColor = renderer.color;
        renderer.color = new Color(1f, 0.3f, 0.3f, 1f);
        yield return new WaitForSeconds(0.12f);
        if (renderer != null) renderer.color = originalColor;
        hitFlashRoutine = null;
    }
}
