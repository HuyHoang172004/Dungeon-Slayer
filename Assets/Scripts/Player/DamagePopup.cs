using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour {

    public float moveYSpeed = 2f;
    public float moveXSpeed = 1f;
    private float disappearTimer = 0.5f;
    private Color textColor;
    private TextMeshPro textMesh;

    private Vector3 moveVector;

    private void Awake() {
        textMesh = GetComponent<TextMeshPro>();
    }

    public void Setup(int damageAmount, bool isCriticalLevel = false) {
        if (textMesh != null) {
            textMesh.SetText(damageAmount.ToString());
            
            if (isCriticalLevel) {
                textMesh.fontSize = 7;
                textColor = Color.red;
            } else {
                textMesh.fontSize = 5;
                textColor = Color.yellow;
            }
            textMesh.color = textColor;
        }

        // Randomly burst left or right slightly
        float dirX = Random.Range(-1f, 1f) > 0 ? 1f : -1f;
        moveVector = new Vector3(moveXSpeed * dirX, moveYSpeed, 0);
    }

    private void Update() {
        transform.position += moveVector * Time.deltaTime;
        moveVector -= moveVector * 4f * Time.deltaTime; // friction effect

        disappearTimer -= Time.deltaTime;
        if (disappearTimer < 0) {
            float disappearSpeed = 3f;
            textColor.a -= disappearSpeed * Time.deltaTime;
            
            if (textMesh != null) {
                textMesh.color = textColor;
            }

            if (textColor.a <= 0) {
                Destroy(gameObject);
            }
        }
    }
}
