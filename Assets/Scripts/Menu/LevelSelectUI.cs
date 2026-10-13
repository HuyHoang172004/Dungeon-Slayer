using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

[RequireComponent(typeof(Button))]
public class LevelSelectUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    
    [Header("Level Data")]
    public int levelIndex = 1;
    public string levelSceneName = "Scene_1";
    
    [Header("UI Visuals")]
    public GameObject lockIcon;
    public Vector3 hoverScale = new Vector3(1.1f, 1.1f, 1.1f);
    
    private Button button;
    private Vector3 originalScale;

    private void Awake() {
        button = GetComponent<Button>();
        originalScale = transform.localScale;
    }

    private void Start() {
        // Check if unlocked
        if (LevelManager.instance != null) {
            bool isUnlocked = LevelManager.instance.maxUnlockedLevel >= levelIndex;
            button.interactable = isUnlocked;

            if (lockIcon != null) {
                lockIcon.SetActive(!isUnlocked);
            }
        }

        button.onClick.AddListener(OnLevelClicked);
    }

    private void OnLevelClicked() {
        // Trigger bounce animation
        StartCoroutine(ClickBounceAnimation());

        // Load Scene
        if (SceneTransitionManager.instance != null) {
            SceneTransitionManager.instance.LoadScene(levelSceneName);
        } else {
            UnityEngine.SceneManagement.SceneManager.LoadScene(levelSceneName);
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (button.interactable) {
            StopAllCoroutines();
            StartCoroutine(ScaleTo(hoverScale, 0.15f));
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (button.interactable) {
            StopAllCoroutines();
            StartCoroutine(ScaleTo(originalScale, 0.15f));
        }
    }

    private IEnumerator ScaleTo(Vector3 targetScale, float duration) {
        float elapsed = 0f;
        Vector3 startScale = transform.localScale;

        while (elapsed < duration) {
            transform.localScale = Vector3.Lerp(startScale, targetScale, elapsed / duration);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
    }

    private IEnumerator ClickBounceAnimation() {
        yield return StartCoroutine(ScaleTo(originalScale * 0.9f, 0.05f));
        yield return StartCoroutine(ScaleTo(hoverScale, 0.1f));
    }
}
