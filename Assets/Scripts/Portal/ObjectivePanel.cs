using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI mục tiêu đặt trực tiếp trong scene. Kéo RectTransform của ObjectiveLabel
/// để thay đổi vị trí riêng cho từng màn.
/// </summary>
public class ObjectivePanel : MonoBehaviour
{
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject background;
    [SerializeField] private TextMeshProUGUI objectiveLabel;

    private RectTransform labelRect;
    private RectTransform backgroundRect;
    private Image backgroundImage;
    private Image eventIconImage;
    private Outline eventOutline;
    private bool showEventIcon;
    private Vector2 defaultLabelAnchorMin;
    private Vector2 defaultLabelAnchorMax;
    private Vector2 defaultLabelPivot;
    private Vector2 defaultLabelPosition;
    private Vector2 defaultLabelSize;
    private Vector2 defaultBackgroundAnchorMin;
    private Vector2 defaultBackgroundAnchorMax;
    private Vector2 defaultBackgroundPivot;
    private Vector2 defaultBackgroundPosition;
    private Vector2 defaultBackgroundSize;
    private Color defaultLabelColor;
    private Color defaultBackgroundColor;
    private float defaultFontSize;
    private float defaultFontSizeMin;
    private float defaultFontSizeMax;
    private FontStyles defaultFontStyle;

    private void Awake()
    {
        CacheDefaultStyle();
        CreateEventIcon();
        SetVisible(false);
    }

    public void Show(string message)
    {
        showEventIcon = false;
        ApplyObjectiveStyle();
        SetMessage(message);
        SetVisible(true);
    }

    public void ShowEvent(string message, Sprite iconSprite, Color iconColor)
    {
        showEventIcon = eventIconImage != null && iconSprite != null;
        if (showEventIcon)
        {
            eventIconImage.sprite = iconSprite;
            eventIconImage.color = iconColor;
        }

        ApplyEventStyle();
        SetMessage(message);
        SetVisible(true);
    }

    public void ShowCinematic(string message)
    {
        showEventIcon = false;
        ApplyCinematicStyle();
        SetMessage(message);
        SetVisible(true);
    }

    private void SetMessage(string message)
    {
        if (objectiveLabel != null)
        {
            objectiveLabel.text = message;
        }
    }

    public void SetVisible(bool visible)
    {
        if (content != null)
        {
            content.SetActive(visible);
        }

        if (background != null)
        {
            background.SetActive(visible);
        }

        if (eventIconImage != null)
        {
            eventIconImage.gameObject.SetActive(visible && showEventIcon);
        }
    }

    private void CacheDefaultStyle()
    {
        if (objectiveLabel != null)
        {
            labelRect = objectiveLabel.rectTransform;
            defaultLabelAnchorMin = labelRect.anchorMin;
            defaultLabelAnchorMax = labelRect.anchorMax;
            defaultLabelPivot = labelRect.pivot;
            defaultLabelPosition = labelRect.anchoredPosition;
            defaultLabelSize = labelRect.sizeDelta;
            defaultLabelColor = objectiveLabel.color;
            defaultFontSize = objectiveLabel.fontSize;
            defaultFontSizeMin = objectiveLabel.fontSizeMin;
            defaultFontSizeMax = objectiveLabel.fontSizeMax;
            defaultFontStyle = objectiveLabel.fontStyle;
        }

        if (background != null)
        {
            backgroundRect = background.GetComponent<RectTransform>();
            backgroundImage = background.GetComponent<Image>();
            if (backgroundRect != null)
            {
                defaultBackgroundAnchorMin = backgroundRect.anchorMin;
                defaultBackgroundAnchorMax = backgroundRect.anchorMax;
                defaultBackgroundPivot = backgroundRect.pivot;
                defaultBackgroundPosition = backgroundRect.anchoredPosition;
                defaultBackgroundSize = backgroundRect.sizeDelta;
            }

            if (backgroundImage != null)
            {
                defaultBackgroundColor = backgroundImage.color;
                eventOutline = background.GetComponent<Outline>();
                if (eventOutline == null)
                {
                    eventOutline = background.AddComponent<Outline>();
                }

                eventOutline.effectColor = Color.black;
                eventOutline.effectDistance = new Vector2(3f, -3f);
                eventOutline.enabled = false;
            }
        }
    }

    private void ApplyEventStyle()
    {
        if (labelRect != null)
        {
            labelRect.anchorMin = labelRect.anchorMax = new Vector2(0.5f, 0.5f);
            labelRect.pivot = new Vector2(0.5f, 0.5f);
            labelRect.anchoredPosition = new Vector2(75f, 390f);
            labelRect.sizeDelta = new Vector2(720f, 125f);
            objectiveLabel.color = Color.black;
            objectiveLabel.alignment = TextAlignmentOptions.Center;
            objectiveLabel.fontSize = 38f;
            objectiveLabel.fontSizeMin = 24f;
            objectiveLabel.fontSizeMax = 38f;
            objectiveLabel.fontStyle = FontStyles.Bold;
        }

        if (backgroundRect != null)
        {
            backgroundRect.anchorMin = backgroundRect.anchorMax = new Vector2(0.5f, 0.5f);
            backgroundRect.pivot = new Vector2(0.5f, 0.5f);
            backgroundRect.anchoredPosition = new Vector2(0f, 390f);
            backgroundRect.sizeDelta = new Vector2(920f, 165f);
        }

        if (backgroundImage != null)
        {
            backgroundImage.color = new Color(1f, 1f, 1f, 0.97f);
        }

        if (eventOutline != null)
        {
            eventOutline.enabled = true;
        }
    }

    private void ApplyCinematicStyle()
    {
        if (labelRect != null)
        {
            labelRect.anchorMin = labelRect.anchorMax = new Vector2(0.5f, 0.5f);
            labelRect.pivot = new Vector2(0.5f, 0.5f);
            labelRect.anchoredPosition = Vector2.zero;
            labelRect.sizeDelta = new Vector2(1040f, 185f);
            objectiveLabel.color = Color.black;
            objectiveLabel.alignment = TextAlignmentOptions.Center;
            objectiveLabel.fontSize = 40f;
            objectiveLabel.fontSizeMin = 25f;
            objectiveLabel.fontSizeMax = 40f;
            objectiveLabel.fontStyle = FontStyles.Bold;
        }

        if (backgroundRect != null)
        {
            backgroundRect.anchorMin = backgroundRect.anchorMax = new Vector2(0.5f, 0.5f);
            backgroundRect.pivot = new Vector2(0.5f, 0.5f);
            backgroundRect.anchoredPosition = Vector2.zero;
            backgroundRect.sizeDelta = new Vector2(1190f, 245f);
        }

        if (backgroundImage != null)
            backgroundImage.color = new Color(1f, 1f, 1f, 0.98f);
        if (eventOutline != null)
            eventOutline.enabled = true;
    }

    private void ApplyObjectiveStyle()
    {
        if (labelRect != null)
        {
            labelRect.anchorMin = defaultLabelAnchorMin;
            labelRect.anchorMax = defaultLabelAnchorMax;
            labelRect.pivot = defaultLabelPivot;
            labelRect.anchoredPosition = defaultLabelPosition;
            labelRect.sizeDelta = defaultLabelSize;
            objectiveLabel.color = defaultLabelColor;
            // Không khôi phục giá trị alignment kết hợp từ scene vì một số scene cũ
            // lưu cờ căn dọc không hợp lệ, khiến chữ bị vẽ ra ngoài mép phải của hộp.
            objectiveLabel.alignment = TextAlignmentOptions.Center;
            objectiveLabel.fontSize = defaultFontSize;
            objectiveLabel.fontSizeMin = defaultFontSizeMin;
            objectiveLabel.fontSizeMax = defaultFontSizeMax;
            objectiveLabel.fontStyle = defaultFontStyle;
        }

        if (backgroundRect != null)
        {
            backgroundRect.anchorMin = defaultBackgroundAnchorMin;
            backgroundRect.anchorMax = defaultBackgroundAnchorMax;
            backgroundRect.pivot = defaultBackgroundPivot;
            backgroundRect.anchoredPosition = defaultBackgroundPosition;
            backgroundRect.sizeDelta = defaultBackgroundSize;
        }

        if (backgroundImage != null)
        {
            backgroundImage.color = defaultBackgroundColor;
        }

        if (eventOutline != null)
        {
            eventOutline.enabled = false;
        }
    }

    private void CreateEventIcon()
    {
        GameObject iconObject = new GameObject(
            "ObjectiveKeyIcon",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image));
        iconObject.layer = gameObject.layer;

        RectTransform iconRect = iconObject.GetComponent<RectTransform>();
        iconRect.SetParent(transform, false);
        iconRect.anchorMin = iconRect.anchorMax = new Vector2(0.5f, 0.5f);
        iconRect.pivot = new Vector2(0.5f, 0.5f);
        iconRect.anchoredPosition = new Vector2(-350f, 390f);
        iconRect.sizeDelta = new Vector2(125f, 105f);

        eventIconImage = iconObject.GetComponent<Image>();
        eventIconImage.preserveAspect = true;
        eventIconImage.raycastTarget = false;
        eventIconImage.gameObject.SetActive(false);
    }
}
