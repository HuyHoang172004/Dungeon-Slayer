using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CameraWin : MonoBehaviour {

    public TextMeshProUGUI scoreTotal;
    public GameObject cameraWin;
    public GameObject stats;

    public Image Star1;
    public Image Star2;
    public Image Star3;
    public Sprite Star;

    public GameObject boss;

    public int stars;
    private bool victoryProcessed;
    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        if (!victoryProcessed && boss != null && boss.GetComponent<Enemy>().health <= 0) {
            victoryProcessed = true;
            if (SceneManager.GetActiveScene().name == "Scene_Final_1") {
                StartCoroutine(PlayFinalEnding());
            } else {
                ShowLegacyVictory();
            }
        }

    }

    private IEnumerator PlayFinalEnding() {
        FinalEndingCutscene cutscene = gameObject.AddComponent<FinalEndingCutscene>();
        yield return StartCoroutine(cutscene.Play(
            boss,
            stats,
            scoreTotal != null ? scoreTotal.font : null));

        int finalScore = ScoreManager.instance != null ? ScoreManager.instance.getScoreTotal() : 0;
        stars = ScoreManager.instance != null ? ScoreManager.instance.getScoreStars() : 0;
        Time.timeScale = 0f;
        FinalVictoryUI.Show(scoreTotal != null ? scoreTotal.font : null, finalScore, stars, Star);
        cutscene.FinishTransition();
    }

    private void ShowLegacyVictory() {
        cameraWin.SetActive(true);
        stats.SetActive(false);

        scoreTotal.text = "" + ScoreManager.instance.getScoreTotal();
        stars = ScoreManager.instance.getScoreStars();

        switch (stars) {
            case 1:
                Star1.sprite = Star;
                break;
            case 2:
                Star1.sprite = Star;
                Star2.sprite = Star;
                break;
            case 3:
                Star1.sprite = Star;
                Star2.sprite = Star;
                Star3.sprite = Star;
                break;
        }

        Time.timeScale = 0f;
    }
}
