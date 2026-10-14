using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Opening story shown only when the player chooses New Game.
/// The UI is built at runtime so it scales consistently on desktop and mobile.
/// </summary>
public class StoryIntroUI : MonoBehaviour
{
    private readonly struct StoryPage
    {
        public readonly string Title;
        public readonly string Body;

        public StoryPage(string title, string body)
        {
            Title = title;
            Body = body;
        }
    }

    private static readonly StoryPage[] Pages =
    {
        new StoryPage(
            "VƯƠNG QUỐC ELDORIA",
            "Từ xa xưa, Eldoria được bảo vệ bởi Trái Tim Ánh Sáng — nguồn sức mạnh duy trì sự sống và đẩy lùi bóng tối khỏi vương quốc."),
        new StoryPage(
            "SỰ SA NGÃ CỦA MALAKAR",
            "Malakar, pháp sư hoàng gia, đã dùng nghi lễ cấm để tìm kiếm sự bất tử. Nghi lễ thất bại, biến ông thành chúa tể bóng tối và gieo lời nguyền lên khắp Eldoria."),
        new StoryPage(
            "BA CHÌA KHÓA PHONG ẤN",
            "Để ngăn Malakar trở lại, phong ấn của Trái Tim Ánh Sáng được chia thành ba chìa khóa. Nhưng lời nguyền đã xâm chiếm những người canh giữ chúng: Knight, Bee và Vampire."),
        new StoryPage(
            "SỨ MỆNH CỦA AREN",
            "Aren, hãy giải thoát ba người canh giữ, thu hồi Chìa khóa Lòng Dũng Cảm, Chìa khóa Sự Sống và Chìa khóa Ý Chí. Chỉ khi đó bạn mới có thể đối mặt Malakar và cứu Eldoria.")
    };

    private static readonly string[] ImageResourcePaths =
    {
        "StoryIntro/eldoria",
        "StoryIntro/malakar",
        "StoryIntro/guardians",
        "StoryIntro/aren"
    };

    private GameObject introRoot;
    private RawImage storyImage;
    private TextMeshProUGUI titleText;
    private TextMeshProUGUI bodyText;
    private TextMeshProUGUI pageText;
    private TextMeshProUGUI nextButtonText;
    private Action onCompleted;
    private int currentPage;
    private bool isClosing;

    public void Show(TMP_FontAsset font, Action completed)
    {
        if (introRoot != null)
        {
            return;
        }

        onCompleted = completed;
        currentPage = 0;
        isClosing = false;
        BuildInterface(font);
        ShowCurrentPage();
    }

    private void Update()
    {
        if (introRoot == null || isClosing)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            NextPage();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            FinishIntro();
        }
    }

    private void BuildInterface(TMP_FontAsset font)
    {
        introRoot = new GameObject(
            "NewGameStoryIntro",
            typeof(RectTransform),
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster));

        Canvas canvas = introRoot.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        CanvasScaler scaler = introRoot.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        RectTransform backdrop = CreatePanel("Backdrop", introRoot.transform, Color.black);
        StretchToParent(backdrop);
        backdrop.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.88f);

        RectTransform artFrame = CreatePanel("StoryArtworkFrame", backdrop, Color.black);
        SetRect(artFrame, new Vector2(0f, 20f), new Vector2(1720f, 968f));

        Outline artOutline = artFrame.gameObject.AddComponent<Outline>();
        artOutline.effectColor = new Color(0.91f, 0.69f, 0.2f, 1f);
        artOutline.effectDistance = new Vector2(3f, -3f);

        GameObject artworkObject = new GameObject(
            "StoryArtwork",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(RawImage));
        artworkObject.layer = 5;
        RectTransform artworkRect = artworkObject.GetComponent<RectTransform>();
        artworkRect.SetParent(artFrame, false);
        StretchToParent(artworkRect);
        storyImage = artworkObject.GetComponent<RawImage>();
        storyImage.color = Color.white;
        storyImage.raycastTarget = false;

        RectTransform dialoguePanel = CreatePanel("DialogueBox", backdrop, new Color(1f, 1f, 1f, 0.97f));
        SetRect(dialoguePanel, new Vector2(0f, -350f), new Vector2(1260f, 260f));

        Outline dialogueOutline = dialoguePanel.gameObject.AddComponent<Outline>();
        dialogueOutline.effectColor = Color.black;
        dialogueOutline.effectDistance = new Vector2(5f, -5f);

        titleText = CreateText("StoryTitle", dialoguePanel, font, 40f, FontStyles.Bold);
        SetRect(titleText.rectTransform, new Vector2(0f, 76f), new Vector2(1150f, 58f));
        titleText.alignment = TextAlignmentOptions.Left;
        titleText.color = Color.black;
        titleText.enableAutoSizing = true;
        titleText.fontSizeMin = 30f;
        titleText.fontSizeMax = 40f;

        bodyText = CreateText("StoryBody", dialoguePanel, font, 31f, FontStyles.Normal);
        SetRect(bodyText.rectTransform, new Vector2(0f, -23f), new Vector2(1150f, 135f));
        bodyText.alignment = TextAlignmentOptions.TopLeft;
        bodyText.color = Color.black;
        bodyText.textWrappingMode = TextWrappingModes.Normal;
        bodyText.overflowMode = TextOverflowModes.Truncate;
        bodyText.enableAutoSizing = true;
        bodyText.fontSizeMin = 23f;
        bodyText.fontSizeMax = 31f;
        bodyText.margin = new Vector4(0f, 7f, 0f, 5f);

        pageText = CreateText("PageIndicator", dialoguePanel, font, 23f, FontStyles.Bold);
        SetRect(pageText.rectTransform, new Vector2(520f, -105f), new Vector2(140f, 38f));
        pageText.alignment = TextAlignmentOptions.Right;
        pageText.color = new Color(0.17f, 0.17f, 0.17f, 1f);

        Button skipButton = CreateButton(
            "BTNSkipStory",
            backdrop,
            font,
            "SKIP",
            new Vector2(-770f, -350f));
        skipButton.onClick.AddListener(FinishIntro);

        Button nextButton = CreateButton(
            "BTNNextStory",
            backdrop,
            font,
            "NEXT",
            new Vector2(770f, -350f));
        nextButton.onClick.AddListener(NextPage);
        nextButtonText = nextButton.GetComponentInChildren<TextMeshProUGUI>();

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(nextButton.gameObject);
        }
    }

    private void ShowCurrentPage()
    {
        StoryPage page = Pages[currentPage];
        titleText.text = page.Title;
        bodyText.text = page.Body;
        pageText.text = $"{currentPage + 1} / {Pages.Length}";
        storyImage.texture = Resources.Load<Texture2D>(ImageResourcePaths[currentPage]);
        nextButtonText.text = currentPage == Pages.Length - 1 ? "BEGIN" : "NEXT";
    }

    private void NextPage()
    {
        if (isClosing)
        {
            return;
        }

        if (currentPage < Pages.Length - 1)
        {
            currentPage++;
            ShowCurrentPage();
            return;
        }

        FinishIntro();
    }

    private void FinishIntro()
    {
        if (isClosing)
        {
            return;
        }

        isClosing = true;
        Action completed = onCompleted;
        onCompleted = null;

        if (introRoot != null)
        {
            Destroy(introRoot);
            introRoot = null;
        }

        completed?.Invoke();
    }

    private static RectTransform CreatePanel(string name, Transform parent, Color color)
    {
        GameObject panel = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panel.layer = 5;
        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        panel.GetComponent<Image>().color = color;
        return rect;
    }

    private static TextMeshProUGUI CreateText(
        string name,
        Transform parent,
        TMP_FontAsset font,
        float fontSize,
        FontStyles style)
    {
        GameObject textObject = new GameObject(
            name,
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(TextMeshProUGUI));
        textObject.layer = 5;
        RectTransform rect = textObject.GetComponent<RectTransform>();
        rect.SetParent(parent, false);

        TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
        text.font = font;
        text.fontSize = fontSize;
        text.fontStyle = style;
        text.raycastTarget = false;
        return text;
    }

    private static Button CreateButton(
        string name,
        Transform parent,
        TMP_FontAsset font,
        string label,
        Vector2 position)
    {
        GameObject buttonObject = new GameObject(
            name,
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image),
            typeof(Button));
        buttonObject.layer = 5;
        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(220f, 88f);
        rect.anchoredPosition = position;

        Image image = buttonObject.GetComponent<Image>();
        image.color = new Color(0.12f, 0.2f, 0.34f, 1f);

        Button button = buttonObject.GetComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1f, 0.85f, 0.4f, 1f);
        colors.pressedColor = new Color(0.75f, 0.62f, 0.25f, 1f);
        colors.selectedColor = colors.highlightedColor;
        button.colors = colors;

        Outline outline = buttonObject.AddComponent<Outline>();
        outline.effectColor = new Color(0.91f, 0.69f, 0.2f, 1f);
        outline.effectDistance = new Vector2(2f, -2f);

        TextMeshProUGUI text = CreateText("Label", rect, font, 30f, FontStyles.Bold);
        StretchToParent(text.rectTransform);
        text.text = label;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;

        return button;
    }

    private static void SetRect(RectTransform rect, Vector2 position, Vector2 size)
    {
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
    }

    private static void StretchToParent(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
