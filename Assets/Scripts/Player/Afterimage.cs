using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Afterimage : MonoBehaviour {

    public float fadeSpeed = 3f;
    private SpriteRenderer sr;

    private void Awake() {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update() {
        if (sr != null) {
            Color c = sr.color;
            c.a -= fadeSpeed * Time.deltaTime;
            sr.color = c;
            
            if (c.a <= 0) {
                Destroy(gameObject);
            }
        }
    }
}
