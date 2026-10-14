using System.Collections;
using UnityEngine;

/// <summary>
/// Connects the final encounter to the story/objective UI without changing the
/// Wizard's combat logic. This component is installed by FinalWizardBossAI.
/// </summary>
public class FinalLevelFlow : MonoBehaviour
{
    private const string FinalObjective =
        "MỤC TIÊU\nTiến vào điện phong ấn và đánh bại Malakar.";

    private FinalWizardBossAI bossAI;
    private Enemy boss;
    private Transform player;
    private Transform npc;
    private ObjectivePanel objectivePanel;
    private int lastPhase = 1;
    private bool preFightShown;
    private bool eventPlaying;

    private void Start()
    {
        bossAI = GetComponent<FinalWizardBossAI>();
        boss = GetComponent<Enemy>();
        FindSceneReferences();
    }

    private void Update()
    {
        if (player == null || objectivePanel == null)
        {
            FindSceneReferences();
            if (player == null || objectivePanel == null) return;
        }

        if (!preFightShown && boss != null && boss.health > 0 &&
            Vector2.Distance(player.position, transform.position) <= 10f)
        {
            preFightShown = true;
            StartCoroutine(ShowPreFightDialogue());
        }

        int phase = bossAI != null ? bossAI.CurrentPhase : 1;
        if (phase > lastPhase)
        {
            lastPhase = phase;
            if (phase == 2)
            {
                StartCoroutine(ShowEvent(
                    "PHONG ẤN ĐẦU TIÊN ĐÃ VỠ!\nMalakar bắt đầu dịch chuyển và tung chùm phép.",
                    4.2f));
            }
            else if (phase == 3)
            {
                StartCoroutine(ShowEvent(
                    "MALAKAR GIẢI PHÓNG TOÀN BỘ BÓNG TỐI!\n" +
                    "Hãy tìm khoảng trống giữa các dấu cảnh báo thiên thạch.",
                    4.8f));
            }
        }

        if (!eventPlaying)
        {
            bool nearNpc = npc != null && Vector2.Distance(player.position, npc.position) <= 4.5f;
            if (nearNpc) objectivePanel.Show(FinalObjective);
            else objectivePanel.SetVisible(false);
        }
    }

    private void FindSceneReferences()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            player = playerObject != null ? playerObject.transform : null;
        }

        if (objectivePanel == null)
            objectivePanel = FindAnyObjectByType<ObjectivePanel>(FindObjectsInactive.Include);

        if (npc == null)
        {
            DialogueManager dialogue = FindAnyObjectByType<DialogueManager>(FindObjectsInactive.Include);
            npc = dialogue != null ? dialogue.transform : null;
        }
    }

    private IEnumerator ShowEvent(string message, float duration)
    {
        while (eventPlaying) yield return null;
        eventPlaying = true;
        objectivePanel.ShowEvent(message, null, Color.white);
        yield return new WaitForSeconds(duration);
        objectivePanel.SetVisible(false);
        eventPlaying = false;
    }

    private IEnumerator ShowPreFightDialogue()
    {
        while (eventPlaying) yield return null;
        eventPlaying = true;

        PlayerController playerController = player != null ? player.GetComponent<PlayerController>() : null;
        PlayerCombat playerCombat = player != null ? player.GetComponent<PlayerCombat>() : null;
        if (playerController != null) playerController.enabled = false;
        if (playerCombat != null) playerCombat.enabled = false;

        float previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        objectivePanel.ShowCinematic(
            "MALAKAR\nBa chìa khóa chỉ đưa ngươi đến gần hơn với cái chết.");
        yield return new WaitForSecondsRealtime(3.7f);

        objectivePanel.ShowCinematic(
            "AREN\nTa sẽ kết thúc lời nguyền và đưa ánh sáng trở lại Eldoria.");
        yield return new WaitForSecondsRealtime(3.5f);

        objectivePanel.SetVisible(false);
        Time.timeScale = previousTimeScale <= 0f ? 1f : previousTimeScale;
        if (playerController != null) playerController.enabled = true;
        if (playerCombat != null) playerCombat.enabled = true;
        eventPlaying = false;
    }
}
