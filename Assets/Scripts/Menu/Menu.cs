using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Menu : MonoBehaviour
{
    private const string ReplayIntroKey = "ReplayStoryIntro";

    private void Start()
    {
        // With an in-memory gameplay session, Menu is loaded additively and
        // does not become the active scene until after its Start methods run.
        if (gameObject.scene.name == "Menu")
        {
            CreateCheckpointButtons();

            if (PlayerPrefs.GetInt(ReplayIntroKey, 0) == 1)
            {
                PlayerPrefs.DeleteKey(ReplayIntroKey);
                PlayerPrefs.Save();
                NewGame();
            }
        }
    }

    // The original prefab already calls PlayGame. It now resumes the saved map.
    public void PlayGame()
    {
        ResumeGame();
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;

        // A gameplay scene kept in memory takes priority over the disk checkpoint.
        if (SuspendedGameSession.Resume())
        {
            return;
        }

        // Resume trong Pause Menu chi chi can bo pause; khong duoc tai lai man.
        if (SceneManager.GetActiveScene().name != "Menu")
        {
            return;
        }

        SceneManager.LoadScene(GameProgress.GetCheckpoint());
    }

    public void NewGame()
    {
        Time.timeScale = 1;
        SuspendedGameSession.Discard();
        GameProgress.StartNewGame();

        TextMeshProUGUI menuText = GetComponentInChildren<TextMeshProUGUI>(true);
        StoryIntroUI storyIntro = GetComponent<StoryIntroUI>();
        if (storyIntro == null)
        {
            storyIntro = gameObject.AddComponent<StoryIntroUI>();
        }

        storyIntro.Show(menuText != null ? menuText.font : null, LoadFirstLevel);
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        if (SceneManager.GetActiveScene().name != "Menu")
        {
            SuspendedGameSession.SuspendToMenu();
            return;
        }

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void PlayAgain()
    {
        Time.timeScale = 1f;
        SuspendedGameSession.Discard();
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (sceneIndex < 1)
            sceneIndex = GameProgress.GetCheckpoint();
        GameProgress.RestartLevel(sceneIndex);
        SceneManager.LoadScene(sceneIndex);
    }

    private void CreateCheckpointButtons()
    {
        Button resumeButton = FindButton("BTNPlay");
        if (resumeButton == null)
        {
            Debug.LogWarning("Khong tim thay nut Play de tao Resume/New Game.");
            return;
        }

        SetButtonLabel(resumeButton, "RESUME");
        SetButtonPosition(resumeButton, -35f);

        Button newGameButton = Instantiate(resumeButton, resumeButton.transform.parent);
        newGameButton.name = "BTNNewGame";
        newGameButton.onClick = new Button.ButtonClickedEvent();
        newGameButton.onClick.AddListener(NewGame);
        SetButtonLabel(newGameButton, "NEW GAME");
        SetButtonPosition(newGameButton, -75f);

        Button optionsButton = FindButton("BTNOption");
        if (optionsButton != null)
        {
            SetButtonLabel(optionsButton, "OPTIONS");
            SetButtonPosition(optionsButton, -115f);
        }

        Button exitButton = FindButton("BTNExit");
        if (exitButton != null)
        {
            SetButtonLabel(exitButton, "EXIT");
            SetButtonPosition(exitButton, -155f);
        }
    }

    private Button FindButton(string buttonName)
    {
        foreach (Button button in GetComponentsInChildren<Button>(true))
        {
            if (button.name.StartsWith(buttonName, System.StringComparison.OrdinalIgnoreCase))
            {
                return button;
            }
        }

        return null;
    }

    private static void SetButtonLabel(Button button, string label)
    {
        TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>(true);
        if (text != null)
        {
            text.text = label;
        }
    }

    private static void SetButtonPosition(Button button, float y)
    {
        RectTransform rect = button.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(0f, y);
    }
}
