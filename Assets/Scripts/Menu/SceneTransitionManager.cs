using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransitionManager : MonoBehaviour {
    public static SceneTransitionManager instance;

    [Header("Transition Settings")]
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 0.5f;

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        if (fadeCanvasGroup != null) {
            fadeCanvasGroup.alpha = 1f;
            fadeCanvasGroup.blocksRaycasts = false;
            StartCoroutine(FadeIn());
        }
    }

    public void LoadScene(string sceneName) {
        StartCoroutine(FadeOutAndLoad(sceneName));
    }

    private IEnumerator FadeOutAndLoad(string sceneName) {
        if (fadeCanvasGroup != null) {
            fadeCanvasGroup.blocksRaycasts = true;
            float elapsed = 0;
            while (elapsed < fadeDuration) {
                fadeCanvasGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }
            fadeCanvasGroup.alpha = 1f;
        }

        // Freeze time scale resets just in case HitStop messed it up between scenes
        Time.timeScale = 1f; 
        
        SceneManager.LoadScene(sceneName);
        
        // Wait one frame to let it load before fading back in
        yield return null;
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn() {
        if (fadeCanvasGroup != null) {
            float elapsed = 0;
            while (elapsed < fadeDuration) {
                fadeCanvasGroup.alpha = 1f - Mathf.Clamp01(elapsed / fadeDuration);
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }
            fadeCanvasGroup.alpha = 0f;
            fadeCanvasGroup.blocksRaycasts = false;
        }
    }
}
