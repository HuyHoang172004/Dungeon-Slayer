using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>Epilogue pages followed by the final score and star summary.</summary>
public sealed class FinalVictoryUI : MonoBehaviour
{
    private readonly struct EndingPage
    {
        public readonly string Image;
        public readonly string Title;
        public readonly string Body;

        public EndingPage(string image, string title, string body)
        {
            Image = image;
            Title = title;
            Body = body;
        }
    }

    private static readonly EndingPage[] Pages =
    {
        new EndingPage(
            "StoryEnding/seal_restored",
            "PHONG ẤN ĐÃ ĐƯỢC HÓA GIẢI",
            "Ba chìa khóa hợp nhất cùng Trái Tim Ánh Sáng. Bóng tối rời khỏi Malakar và lời nguyền bao phủ Eldoria bắt đầu tan biến."),
        new EndingPage(
            "StoryEnding/guardians_freed",
            "HÀNH TRÌNH CỦA AREN",
            "Knight, Bee và Vampire được giải thoát khỏi lời nguyền. Aren hoàn thành lời hứa với những người canh giữ và đưa ánh sáng trở về vương quốc."),
        new EndingPage(
            "StoryEnding/eldoria_homecoming",
            "BÌNH MINH TRỞ LẠI ELDORIA",
            "Mặt trời một lần nữa soi sáng núi rừng, thành trì và những ngôi làng. Câu chuyện về Aren trở thành biểu tượng của lòng dũng cảm, sự sống và ý chí.")
    };

    private RawImage artwork;
    private TextMeshProUGUI title;
    private TextMeshProUGUI body;
    private TextMeshProUGUI pageNumber;
    private TextMeshProUGUI nextLabel;
    private RectTransform storyBox;
    private RectTransform resultBox;
    private Button nextButton;
    private Button skipButton;
    private int page;
    private int finalScore;
    private int earnedStars;
    private Sprite starSprite;
    private TMP_FontAsset font;

    public static void Show(TMP_FontAsset font, int score, int stars, Sprite star)
    {
        if (FindAnyObjectByType<FinalVictoryUI>(FindObjectsInactive.Include) != null) return;

        GameObject root = new GameObject(
            "FinalVictoryStory",
            typeof(RectTransform),
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster),
            typeof(FinalVictoryUI));

        FinalVictoryUI ui = root.GetComponent<FinalVictoryUI>();
        ui.font = font;
        ui.finalScore = score;
        ui.earnedStars = Mathf.Clamp(stars, 0, 3);
        ui.starSprite = star;
        ui.Build();
    }

    private void Build()
    {
        Canvas canvas = GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 300;

        CanvasScaler scaler = GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        GameObject artObject = new GameObject("EndingArtwork", typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage));
        RectTransform artRect = artObject.GetComponent<RectTransform>();
        artRect.SetParent(transform, false);
        Stretch(artRect);
        artwork = artObject.GetComponent<RawImage>();
        artwork.raycastTarget = false;

        RectTransform shade = CreatePanel("DarkShade", transform, new Color(0.01f, 0.02f, 0.06f, 0.42f));
        Stretch(shade);

        storyBox = CreatePanel("EndingStoryBox", transform, new Color(1f, 1f, 1f, 0.97f));
        SetRect(storyBox, new Vector2(0f, -340f), new Vector2(1420f, 290f));
        AddOutline(storyBox.gameObject, Color.black, 5f);

        title = CreateText("EndingTitle", storyBox, 43f, FontStyles.Bold);
        SetRect(title.rectTransform, new Vector2(0f, 88f), new Vector2(1300f, 58f));
        title.color = new Color(0.13f, 0.13f, 0.18f);
        title.alignment = TextAlignmentOptions.Center;

        body = CreateText("EndingBody", storyBox, 30f, FontStyles.Normal);
        SetRect(body.rectTransform, new Vector2(0f, -4f), new Vector2(1270f, 112f));
        body.color = Color.black;
        body.alignment = TextAlignmentOptions.TopLeft;
        body.textWrappingMode = TextWrappingModes.Normal;
        body.enableAutoSizing = true;
        body.fontSizeMin = 23f;
        body.fontSizeMax = 30f;

        pageNumber = CreateText("PageNumber", storyBox, 22f, FontStyles.Bold);
        SetRect(pageNumber.rectTransform, new Vector2(535f, -116f), new Vector2(150f, 34f));
        pageNumber.color = Color.black;
        pageNumber.alignment = TextAlignmentOptions.Right;

        skipButton = CreateButton("BTNSkipEnding", transform, "SKIP", new Vector2(-760f, -365f), new Vector2(210f, 76f));
        skipButton.onClick.AddListener(ShowResults);
        nextButton = CreateButton("BTNNextEnding", transform, "NEXT", new Vector2(760f, -365f), new Vector2(210f, 76f));
        nextButton.onClick.AddListener(NextPage);
        nextLabel = nextButton.GetComponentInChildren<TextMeshProUGUI>();

        page = 0;
        ShowPage();
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(nextButton.gameObject);
    }

    private void Update()
    {
        if (resultBox != null) return;
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)) NextPage();
        else if (Input.GetKeyDown(KeyCode.Escape)) ShowResults();
    }

    private void ShowPage()
    {
        EndingPage data = Pages[page];
        artwork.texture = Resources.Load<Texture2D>(data.Image);
        title.text = data.Title;
        body.text = data.Body;
        pageNumber.text = (page + 1) + " / " + Pages.Length;
        nextLabel.text = page == Pages.Length - 1 ? "VICTORY" : "NEXT";
    }

    private void NextPage()
    {
        if (resultBox != null) return;
        if (page < Pages.Length - 1)
        {
            page++;
            ShowPage();
        }
        else ShowResults();
    }

    private void ShowResults()
    {
        if (resultBox != null) return;

        storyBox.gameObject.SetActive(false);
        skipButton.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(false);
        artwork.texture = Resources.Load<Texture2D>("StoryEnding/eldoria_homecoming");

        resultBox = CreatePanel("VictoryResult", transform, new Color(1f, 1f, 1f, 0.97f));
        SetRect(resultBox, Vector2.zero, new Vector2(900f, 580f));
        AddOutline(resultBox.gameObject, new Color(0.91f, 0.69f, 0.2f), 6f);

        TextMeshProUGUI victory = CreateText("VictoryTitle", resultBox, 64f, FontStyles.Bold);
        SetRect(victory.rectTransform, new Vector2(0f, 205f), new Vector2(780f, 90f));
        victory.text = "VICTORY";
        victory.color = new Color(0.13f, 0.13f, 0.2f);
        victory.alignment = TextAlignmentOptions.Center;

        TextMeshProUGUI score = CreateText("FinalScore", resultBox, 42f, FontStyles.Bold);
        SetRect(score.rectTransform, new Vector2(0f, 105f), new Vector2(760f, 65f));
        score.text = "SCORE  " + finalScore;
        score.color = Color.black;
        score.alignment = TextAlignmentOptions.Center;

        for (int i = 0; i < 3; i++)
        {
            GameObject starObject = new GameObject("ResultStar" + (i + 1), typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            RectTransform rect = starObject.GetComponent<RectTransform>();
            rect.SetParent(resultBox, false);
            SetRect(rect, new Vector2((i - 1) * 145f, 5f), new Vector2(115f, 115f));
            Image image = starObject.GetComponent<Image>();
            image.sprite = starSprite;
            image.preserveAspect = true;
            image.color = i < earnedStars ? Color.white : new Color(0.25f, 0.25f, 0.3f, 0.55f);
        }

        Button menu = CreateButton("BTNFinalMenu", resultBox, "MAIN MENU", new Vector2(-185f, -190f), new Vector2(320f, 82f));
        menu.onClick.AddListener(OpenMenu);
        Button exit = CreateButton("BTNFinalExit", resultBox, "EXIT", new Vector2(185f, -190f), new Vector2(320f, 82f));
        exit.onClick.AddListener(ExitGame);
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(menu.gameObject);
    }

    public void OpenMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();
#if UNITY_EDITOR
        Debug.Log("EXIT pressed. Application.Quit only closes a built game.");
#endif
    }

    private RectTransform CreatePanel(string name, Transform parent, Color color)
    {
        GameObject panel = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panel.layer = 5;
        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        panel.GetComponent<Image>().color = color;
        return rect;
    }

    private TextMeshProUGUI CreateText(string name, Transform parent, float size, FontStyles style)
    {
        GameObject textObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        textObject.layer = 5;
        RectTransform rect = textObject.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
        text.font = font;
        text.fontSize = size;
        text.fontStyle = style;
        text.raycastTarget = false;
        return text;
    }

    private Button CreateButton(string name, Transform parent, string label, Vector2 position, Vector2 size)
    {
        RectTransform rect = CreatePanel(name, parent, new Color(0.12f, 0.2f, 0.34f, 1f));
        SetRect(rect, position, size);
        Button button = rect.gameObject.AddComponent<Button>();
        AddOutline(rect.gameObject, new Color(0.91f, 0.69f, 0.2f), 2f);
        TextMeshProUGUI text = CreateText("Label", rect, 29f, FontStyles.Bold);
        Stretch(text.rectTransform);
        text.text = label;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Center;
        return button;
    }

    private static void AddOutline(GameObject target, Color color, float distance)
    {
        Outline outline = target.AddComponent<Outline>();
        outline.effectColor = color;
        outline.effectDistance = new Vector2(distance, -distance);
    }

    private static void SetRect(RectTransform rect, Vector2 position, Vector2 size)
    {
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
