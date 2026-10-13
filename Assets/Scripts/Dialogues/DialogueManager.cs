using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour {

    public Dialogue dialogue;
    public GameObject dialoguePanel;
    public TextMeshProUGUI displayText;
    public float typingSpeed = 0.05f;
    [Min(0f)] public float postSentenceDelay = 1f;
    //public AudioClip speakSound; 

    Queue<string> sentences;
    string activeSentence;
    float sentenceFullyVisibleAt = float.PositiveInfinity;
    //AudioSource myAudio;

    // Start is called before the first frame update
    void Start() {
        sentences = new Queue<string>();
        //myAudio = GetComponent<AudioSource>();
    }

    void StartDialogue() {
        sentences.Clear();
        sentenceFullyVisibleAt = float.PositiveInfinity;

        foreach(string sentence in dialogue.sentenceList) {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }
    
    void DisplayNextSentence() {
        sentenceFullyVisibleAt = float.PositiveInfinity;

        if(sentences.Count <= 0) {
            displayText.text = activeSentence;
            return;
        }

        activeSentence = sentences.Dequeue();
        displayText.text = activeSentence;

        StopAllCoroutines();
        StartCoroutine(TypeTheSentence(activeSentence));
    }

    IEnumerator TypeTheSentence(string sentence) {
        displayText.text = "";

        foreach(char letter in sentence.ToCharArray()) {
            displayText.text += letter;

            if(displayText.text == activeSentence) {
                sentenceFullyVisibleAt = Time.unscaledTime;
            }

            //myAudio.PlayOneShot(speakSound);
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    bool CanAdvanceDialogue() {
        return displayText.text == activeSentence
            && Time.unscaledTime >= sentenceFullyVisibleAt + postSentenceDelay;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")) {
            dialoguePanel.SetActive(true);
            StartDialogue();
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if(collision.CompareTag("Player")) {
            if(Input.GetKeyDown(KeyCode.Return) && CanAdvanceDialogue()) {
                DisplayNextSentence();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if(collision.CompareTag("Player")) {
            dialoguePanel.SetActive(false);
            StopAllCoroutines();
            sentenceFullyVisibleAt = float.PositiveInfinity;
        }
    }

    public void DialogueMobile() {
        if (CanAdvanceDialogue()) {
            DisplayNextSentence();
        }
    }


}
