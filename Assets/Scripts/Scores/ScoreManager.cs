using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour {
    public static ScoreManager instance;
    
    [Header("UI References")]
    public TextMeshProUGUI Score;
    public TextMeshProUGUI textScore;
    public TextMeshProUGUI textCoins;
    public TextMeshProUGUI textGems;
    public TextMeshProUGUI textStars;

    [Header("Max Collectibles")]
    public int maxCoins = 150;
    public int maxGems = 60;
    public int maxStars = 3;

    private int score;
    private int scoreCoins;
    private int scoreGems;
    private int scoreStars;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }
    }

    private void Start() {
        LoadScores();
        UpdateAllUI();
    }

    private void LoadScores() {
        score = PlayerPrefs.GetInt("Score", 0);
        scoreCoins = PlayerPrefs.GetInt("ScoreCoins", 0);
        scoreGems = PlayerPrefs.GetInt("ScoreGems", 0);
        scoreStars = PlayerPrefs.GetInt("ScoreStars", 0);
    }

    private void UpdateAllUI() {
        if (Score != null) Score.text = score.ToString();
        if (textScore != null) textScore.text = score.ToString();
        if (textCoins != null) textCoins.text = $"{maxCoins}/{scoreCoins}";
        if (textGems != null) textGems.text = $"{maxGems}/{scoreGems}";
        if (textStars != null) textStars.text = $"{maxStars}/{scoreStars}";
    }

    public void ChangeScore(int scoreValue) {
        score += scoreValue;
        PlayerPrefs.SetInt("Score", score);
        
        if (textScore != null) textScore.text = score.ToString();
        if (Score != null) Score.text = score.ToString();
    }

    public void ChangeScoreCoin(int coinValue) {
        scoreCoins += coinValue;
        PlayerPrefs.SetInt("ScoreCoins", scoreCoins);
        if (textCoins != null) textCoins.text = $"{maxCoins}/{scoreCoins}";
    }

    public void ChangeScoreGem(int gemValue) {
        scoreGems += gemValue;
        PlayerPrefs.SetInt("ScoreGems", scoreGems);
        if (textGems != null) textGems.text = $"{maxGems}/{scoreGems}";
    }

    public void ChangeScoreStar(int starsValue) {
        scoreStars += starsValue;
        PlayerPrefs.SetInt("ScoreStars", scoreStars);
        if (textStars != null) textStars.text = $"{maxStars}/{scoreStars}";
    }

    public int getScoreTotal() {
        return score;
    }

    public int getScoreStars() {
        return scoreStars;
    }
}
