using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour {
    public int coinValue = 1;
    public int scoreValue = 10;
    public GameObject sound;

    public void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.CompareTag("Player")) {
            ScoreManager.instance.ChangeScore(scoreValue);
            ScoreManager.instance.ChangeScoreCoin(coinValue);
            Instantiate(sound);
        }

        if(collision.gameObject.CompareTag("Water")) {
            Destroy(gameObject);
        }
    }
}

