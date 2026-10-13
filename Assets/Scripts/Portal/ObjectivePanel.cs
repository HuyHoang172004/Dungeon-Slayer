using TMPro;
using UnityEngine;

/// <summary>
/// UI mục tiêu đặt trực tiếp trong scene. Kéo RectTransform của ObjectiveLabel
/// để thay đổi vị trí riêng cho từng màn.
/// </summary>
public class ObjectivePanel : MonoBehaviour
{
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject background;
    [SerializeField] private TextMeshProUGUI objectiveLabel;

    private void Awake()
    {
        SetVisible(false);
    }

    public void Show(string message)
    {
        if (objectiveLabel != null)
        {
            objectiveLabel.text = message;
        }

        SetVisible(true);
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
    }
}
