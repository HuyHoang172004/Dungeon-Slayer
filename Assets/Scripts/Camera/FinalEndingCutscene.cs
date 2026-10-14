using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Plays the ending inside the actual final arena before the story epilogue.
/// </summary>
public sealed class FinalEndingCutscene : MonoBehaviour
{
    private Canvas dialogueCanvas;
    private TextMeshProUGUI dialogueText;
    private TMP_FontAsset font;
    private GameObject whiteTransition;

    public IEnumerator Play(GameObject bossObject, GameObject hud, TMP_FontAsset uiFont)
    {
        font = uiFont;
        FinalLevelFlow levelFlow = bossObject.GetComponent<FinalLevelFlow>();
        if (levelFlow != null) levelFlow.enabled = false;
        ObjectivePanel objective = FindAnyObjectByType<ObjectivePanel>(FindObjectsInactive.Include);
        if (objective != null) objective.SetVisible(false);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null || bossObject == null) yield break;

        // Detach the defeated boss from the NPC container and pin its world
        // position so no leftover animation/root motion can drag it onto Aren.
        bossObject.transform.SetParent(null, true);
        Vector3 defeatedBossPosition = bossObject.transform.position;

        LockPlayer(player);
        if (hud != null) hud.SetActive(false);
        BuildDialogueUI();

        yield return ShowLine("Malakar đã bị khuất phục...", 1.4f);
        yield return WalkArenToBoss(player, bossObject.transform, defeatedBossPosition);
        bossObject.transform.position = defeatedBossPosition;
        yield return ShowLine("AREN: Malakar, lời nguyền kết thúc ở đây.", 2.7f);
        yield return ShowLine("MALAKAR: Bóng tối đã trói buộc ta quá lâu... hãy giải phóng Trái Tim Ánh Sáng.", 3.4f);
        yield return PlaySealRitual(player.transform, bossObject, defeatedBossPosition);

        if (dialogueCanvas != null) Destroy(dialogueCanvas.gameObject);
    }

    public void FinishTransition()
    {
        if (whiteTransition != null) Destroy(whiteTransition);
    }

    private static void LockPlayer(GameObject player)
    {
        PlayerController controller = player.GetComponent<PlayerController>();
        if (controller != null)
        {
            if (controller.controllerMobile != null) controller.controllerMobile.SetActive(false);
            controller.enabled = false;
        }

        PlayerCombat combat = player.GetComponent<PlayerCombat>();
        if (combat != null) combat.enabled = false;

        Rigidbody2D body = player.GetComponent<Rigidbody2D>();
        if (body != null)
        {
            body.linearVelocity = Vector2.zero;
            body.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    private IEnumerator WalkArenToBoss(GameObject player, Transform boss, Vector3 fixedBossPosition)
    {
        Animator animator = player.GetComponent<Animator>();
        SpriteRenderer renderer = player.GetComponent<SpriteRenderer>();
        float side = player.transform.position.x <= fixedBossPosition.x ? -1f : 1f;
        float targetX = fixedBossPosition.x + side * 2.1f;
        if (renderer != null) renderer.flipX = targetX < player.transform.position.x;
        if (animator != null) animator.SetBool("moving", true);

        float timeout = 4f;
        while (Mathf.Abs(player.transform.position.x - targetX) > 0.08f && timeout > 0f)
        {
            Vector3 position = player.transform.position;
            position.x = Mathf.MoveTowards(position.x, targetX, 3.2f * Time.deltaTime);
            player.transform.position = position;
            boss.position = fixedBossPosition;
            timeout -= Time.deltaTime;
            yield return null;
        }

        Vector3 finalPosition = player.transform.position;
        finalPosition.x = targetX;
        player.transform.position = finalPosition;
        boss.position = fixedBossPosition;

        if (animator != null) animator.SetBool("moving", false);
        yield return new WaitForSeconds(0.45f);
    }

    private IEnumerator PlaySealRitual(Transform player, GameObject bossObject, Vector3 fixedBossPosition)
    {
        SpriteRenderer bossRenderer = bossObject.GetComponent<SpriteRenderer>();
        Vector3 ritualPosition = new Vector3(
            (player.position.x + fixedBossPosition.x) * 0.5f,
            player.position.y + 2.15f,
            player.position.z);

        int ritualSortingLayer = bossRenderer != null ? bossRenderer.sortingLayerID : 0;
        GameObject heart = CreateRitualSprite("Heart of Light", CreateHeartSprite(), ritualPosition, Color.white, ritualSortingLayer, 50);
        heart.transform.localScale = Vector3.zero;

        Color[] keyColors =
        {
            new Color(1f, 0.72f, 0.08f),
            new Color(0.2f, 1f, 0.35f),
            new Color(0.7f, 0.25f, 1f)
        };
        Vector3[] starts =
        {
            ritualPosition + new Vector3(-4f, 1.2f),
            ritualPosition + new Vector3(0f, 3.2f),
            ritualPosition + new Vector3(4f, 1.2f)
        };
        GameObject[] keys = new GameObject[3];
        Sprite keySprite = CreateKeySprite();
        for (int i = 0; i < keys.Length; i++)
        {
            keys[i] = CreateRitualSprite("Seal Key " + (i + 1), keySprite, starts[i], keyColors[i], ritualSortingLayer, 52);
            keys[i].transform.localScale = Vector3.one * 0.72f;
        }

        yield return ShowLine(
            "Ba chìa khóa đang hợp nhất với Trái Tim Ánh Sáng. Phong ấn bóng tối sắp bị phá vỡ!",
            2.8f);

        float duration = 2.1f;
        float elapsed = 0f;
        Color initialBossColor = bossRenderer != null ? bossRenderer.color : Color.white;
        Vector3 bossScale = bossObject.transform.localScale;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float smooth = Mathf.SmoothStep(0f, 1f, t);
            heart.transform.localScale = Vector3.one * Mathf.Lerp(0f, 1.35f, smooth);
            heart.transform.Rotate(0f, 0f, 90f * Time.deltaTime);

            for (int i = 0; i < keys.Length; i++)
            {
                float angle = (i - 1) * 24f * Mathf.Deg2Rad;
                Vector3 slot = ritualPosition + new Vector3(Mathf.Sin(angle) * 0.55f, Mathf.Cos(angle) * 0.45f);
                keys[i].transform.position = Vector3.Lerp(starts[i], slot, smooth);
                keys[i].transform.Rotate(0f, 0f, 220f * Time.deltaTime);
            }

            if (bossRenderer != null)
            {
                bossObject.transform.position = fixedBossPosition;
                Color purified = Color.Lerp(initialBossColor, Color.white, smooth);
                purified.a = 1f - Mathf.Clamp01((t - 0.58f) / 0.42f);
                bossRenderer.color = purified;
                bossObject.transform.localScale = bossScale * Mathf.Lerp(1f, 1.28f, smooth);
            }
            yield return null;
        }

        if (ImpactFX.instance != null) ImpactFX.instance.CameraShake();
        bossObject.SetActive(false);

        float pulseTime = 0f;
        while (pulseTime < 0.85f)
        {
            pulseTime += Time.deltaTime;
            float pulse = Mathf.Clamp01(pulseTime / 0.85f);
            heart.transform.localScale = Vector3.one * Mathf.Lerp(1.35f, 2.45f, pulse);
            heart.transform.Rotate(0f, 0f, 180f * Time.deltaTime);
            yield return null;
        }

        if (ImpactFX.instance != null) ImpactFX.instance.CameraShake();
        yield return FadeToWhite(1.25f);

        for (int i = 0; i < keys.Length; i++) Destroy(keys[i]);
        Destroy(heart);
    }

    private IEnumerator FadeToWhite(float duration)
    {
        whiteTransition = new GameObject(
            "FinalWhiteTransition",
            typeof(RectTransform),
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(CanvasGroup));
        Canvas canvas = whiteTransition.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 250;
        CanvasScaler scaler = whiteTransition.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);

        GameObject white = new GameObject("ExpandingWhiteLight", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        RectTransform rect = white.GetComponent<RectTransform>();
        rect.SetParent(whiteTransition.transform, false);
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        white.GetComponent<Image>().color = Color.white;

        CanvasGroup group = whiteTransition.GetComponent<CanvasGroup>();
        group.alpha = 0f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            group.alpha = t * t;
            if (t > 0.48f && t - Time.deltaTime / duration <= 0.48f && ImpactFX.instance != null)
                ImpactFX.instance.CameraShake();
            yield return null;
        }
        group.alpha = 1f;
        yield return new WaitForSeconds(0.35f);
    }

    private IEnumerator ShowLine(string message, float seconds)
    {
        if (dialogueText != null) dialogueText.text = message;
        yield return new WaitForSeconds(seconds);
    }

    private void BuildDialogueUI()
    {
        GameObject root = new GameObject("FinalCutsceneDialogue", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler));
        dialogueCanvas = root.GetComponent<Canvas>();
        dialogueCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        dialogueCanvas.sortingOrder = 120;
        CanvasScaler scaler = root.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);

        GameObject panel = new GameObject("DialogueBox", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.SetParent(root.transform, false);
        panelRect.anchorMin = panelRect.anchorMax = new Vector2(0.5f, 0f);
        panelRect.pivot = new Vector2(0.5f, 0f);
        panelRect.anchoredPosition = new Vector2(0f, 70f);
        panelRect.sizeDelta = new Vector2(1320f, 155f);
        panel.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.96f);
        Outline outline = panel.AddComponent<Outline>();
        outline.effectColor = Color.black;
        outline.effectDistance = new Vector2(5f, -5f);

        GameObject textObject = new GameObject("DialogueText", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.SetParent(panelRect, false);
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(55f, 24f);
        textRect.offsetMax = new Vector2(-55f, -24f);
        dialogueText = textObject.GetComponent<TextMeshProUGUI>();
        dialogueText.font = font;
        dialogueText.fontSize = 34f;
        dialogueText.enableAutoSizing = true;
        dialogueText.fontSizeMin = 24f;
        dialogueText.fontSizeMax = 34f;
        dialogueText.fontStyle = FontStyles.Bold;
        dialogueText.color = Color.black;
        dialogueText.alignment = TextAlignmentOptions.Center;
    }

    private static GameObject CreateRitualSprite(string name, Sprite sprite, Vector3 position, Color color, int sortingLayer, int order)
    {
        GameObject item = new GameObject(name, typeof(SpriteRenderer));
        item.transform.position = position;
        SpriteRenderer renderer = item.GetComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.color = color;
        renderer.sortingLayerID = sortingLayer;
        renderer.sortingOrder = order;
        return item;
    }

    private static Sprite CreateKeySprite()
    {
        Texture2D texture = new Texture2D(24, 12, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;
        Color clear = Color.clear;
        for (int y = 0; y < 12; y++)
        for (int x = 0; x < 24; x++)
        {
            float ring = Vector2.Distance(new Vector2(x, y), new Vector2(5f, 6f));
            bool pixel = (ring >= 2.4f && ring <= 4.2f) ||
                         (x >= 8 && x <= 20 && y >= 5 && y <= 7) ||
                         (x >= 17 && x <= 19 && y >= 3 && y <= 5) ||
                         (x >= 20 && x <= 22 && y >= 7 && y <= 9);
            texture.SetPixel(x, y, pixel ? Color.white : clear);
        }
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, 24, 12), new Vector2(0.5f, 0.5f), 12f);
    }

    private static Sprite CreateHeartSprite()
    {
        Texture2D texture = new Texture2D(20, 20, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;
        for (int y = 0; y < 20; y++)
        for (int x = 0; x < 20; x++)
        {
            float nx = (x - 9.5f) / 9.5f;
            float ny = (y - 9.5f) / 9.5f;
            bool diamond = Mathf.Abs(nx) + Mathf.Abs(ny) < 0.92f;
            bool core = Mathf.Abs(nx) + Mathf.Abs(ny) < 0.48f;
            texture.SetPixel(x, y, core ? Color.white : diamond ? new Color(1f, 0.85f, 0.2f, 0.82f) : Color.clear);
        }
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, 20, 20), new Vector2(0.5f, 0.5f), 10f);
    }
}
