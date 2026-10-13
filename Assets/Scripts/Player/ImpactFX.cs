using UnityEngine;
using System.Collections;

public class ImpactFX : MonoBehaviour {
    
    public static ImpactFX instance;

    [Header("Hit Stop Settings")]
    public float stopDuration = 0.05f;
    private bool isStopping;

    [Header("Camera Shake Settings")]
    public float shakeDuration = 0.1f;
    public float shakeMagnitude = 0.2f;
    
    [Header("Damage Popups")]
    public GameObject damagePopupPrefab;
    
    private void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }
    }

    public void TriggerHitImpact() {
        HitStop();
        CameraShake();
    }

    public void HitStop() {
        if (isStopping) return;
        StartCoroutine(HitStopRoutine());
    }

    private IEnumerator HitStopRoutine() {
        isStopping = true;
        Time.timeScale = 0f;
        
        // Wait using unscaled time because timeScale is 0
        yield return new WaitForSecondsRealtime(stopDuration);
        
        Time.timeScale = 1f;
        isStopping = false;
    }

    public void CameraShake() {
        if (Camera.main != null) {
            StartCoroutine(CameraShakeRoutine(Camera.main.transform));
        }
    }

    private IEnumerator CameraShakeRoutine(Transform camTransform) {
        float elapsed = 0.0f;
        Vector3 originalPos = camTransform.localPosition;

        while (elapsed < shakeDuration) {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            camTransform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);
            elapsed += Time.unscaledDeltaTime;

            yield return null;
        }

        camTransform.localPosition = originalPos;
    }

    public void SpawnDamagePopup(Vector3 position, int amount) {
        if (damagePopupPrefab != null) {
            Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(1f, 2f), 0);
            GameObject popup = Instantiate(damagePopupPrefab, position + randomOffset, Quaternion.identity);
            DamagePopup dp = popup.GetComponent<DamagePopup>();
            if (dp != null) {
                dp.Setup(amount, amount >= 150); // Custom coloring if damage is high
            }
        }
    }
}
