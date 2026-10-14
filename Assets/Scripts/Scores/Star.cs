using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour {
    public int starValue = 1;
    public int scoreValue = 50;
    public GameObject sound;

    private int sceneBuildIndex;

    private void Start() {
        sceneBuildIndex = gameObject.scene.buildIndex;
        if (GameProgress.IsLevelStarCollected(sceneBuildIndex)) {
            Destroy(gameObject);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            if (!GameProgress.TryCollectLevelStar(sceneBuildIndex)) {
                Destroy(gameObject);
                return;
            }

            if (ScoreManager.instance != null) {
                ScoreManager.instance.ChangeScore(scoreValue);
                ScoreManager.instance.ChangeScoreStar(starValue);
            }
            if (sound != null) Instantiate(sound);
            Destroy(gameObject);
        }
    }
}
