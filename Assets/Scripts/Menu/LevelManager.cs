using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {
    public static LevelManager instance;

    [Header("Meta Progression")]
    public int maxUnlockedLevel = 1;

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadProgress();
        } else {
            Destroy(gameObject);
        }
    }

    public void LoadProgress() {
        maxUnlockedLevel = PlayerPrefs.GetInt("MaxUnlockedLevel", 1);
    }

    public void UnlockNextLevel(int completedLevelIndex) {
        int nextLevel = completedLevelIndex + 1;
        if (nextLevel > maxUnlockedLevel) {
            maxUnlockedLevel = nextLevel;
            PlayerPrefs.SetInt("MaxUnlockedLevel", maxUnlockedLevel);
            PlayerPrefs.Save();
        }
    }
}

/// <summary>
/// Checkpoint theo man: luu man gan nhat nguoi choi da vao duoc, khong luu
/// vi tri hay trang thai quai vat dang dang do trong man.
/// </summary>
public static class GameProgress {
    private const string CheckpointSceneKey = "CheckpointScene";
    private const string PositionSceneKey = "CheckpointPositionScene";
    private const string PositionXKey = "CheckpointPositionX";
    private const string PositionYKey = "CheckpointPositionY";
    private const int FirstLevelBuildIndex = 1;

    public static void SaveCheckpoint(int sceneBuildIndex) {
        if (sceneBuildIndex < FirstLevelBuildIndex ||
            sceneBuildIndex >= SceneManager.sceneCountInBuildSettings) {
            return;
        }

        PlayerPrefs.SetInt(CheckpointSceneKey, sceneBuildIndex);
        ClearSavedPosition();
        PlayerPrefs.Save();
    }

    public static int GetCheckpoint() {
        int savedScene = PlayerPrefs.GetInt(CheckpointSceneKey, FirstLevelBuildIndex);
        return savedScene >= FirstLevelBuildIndex && savedScene < SceneManager.sceneCountInBuildSettings
            ? savedScene
            : FirstLevelBuildIndex;
    }

    public static void StartNewGame() {
        PlayerPrefs.DeleteKey(CheckpointSceneKey);
        ClearSavedPosition();
        PlayerPrefs.DeleteKey("MaxUnlockedLevel");
        PlayerPrefs.SetInt("AttackDamage", 0);
        PlayerPrefs.SetInt("Defense", 0);
        PlayerPrefs.SetInt("Score", 0);
        PlayerPrefs.SetInt("ScoreCoins", 0);
        PlayerPrefs.SetInt("ScoreGems", 0);
        PlayerPrefs.SetInt("ScoreStars", 0);
        SaveCheckpoint(FirstLevelBuildIndex);
    }

    public static void SavePlayerPosition(Vector3 position) {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (sceneIndex < FirstLevelBuildIndex) {
            return;
        }

        PlayerPrefs.SetInt(CheckpointSceneKey, sceneIndex);
        PlayerPrefs.SetInt(PositionSceneKey, sceneIndex);
        PlayerPrefs.SetFloat(PositionXKey, position.x);
        PlayerPrefs.SetFloat(PositionYKey, position.y);
        PlayerPrefs.Save();
    }

    public static bool TryGetSavedPlayerPosition(out Vector3 position) {
        int activeScene = SceneManager.GetActiveScene().buildIndex;
        bool hasMatchingCheckpoint = PlayerPrefs.GetInt(CheckpointSceneKey, -1) == activeScene &&
                                     PlayerPrefs.GetInt(PositionSceneKey, -1) == activeScene;
        if (hasMatchingCheckpoint) {
            position = new Vector3(
                PlayerPrefs.GetFloat(PositionXKey),
                PlayerPrefs.GetFloat(PositionYKey),
                0f);
            return true;
        }

        position = default;
        return false;
    }

    private static void ClearSavedPosition() {
        PlayerPrefs.DeleteKey(PositionSceneKey);
        PlayerPrefs.DeleteKey(PositionXKey);
        PlayerPrefs.DeleteKey(PositionYKey);
    }
}
