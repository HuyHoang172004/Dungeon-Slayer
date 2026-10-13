using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Menu : MonoBehaviour
{
    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "Menu")
        {
            CreateCheckpointButtons();
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
        GameProgress.StartNewGame();
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene("Menu");
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
