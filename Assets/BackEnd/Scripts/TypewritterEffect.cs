using System.Collections;
using TMPro;
using UnityEngine;


[RequireComponent(typeof(TMP_Text))]
public class TypewritterEffect : MonoBehaviour
{
    private TMP_Text targetTextBox;

    [Header("Test Text")]
    public string[] testText;
    private int currentlyVisibleCharacterIndex;
    private Coroutine typewritterCoroutine;
    private WaitForSeconds simpleDelay;
    private WaitForSeconds interpunctuationDelay;
    private WaitForSeconds eraseTextDelay;

    [Header("Typewritter Settings")]
    [SerializeField] private float charactersPerSecond = 20f;
    [SerializeField] private float interpunctuationDelayValue = 0.5f;
    [SerializeField] private float eraseDelayValue = 1f;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip[] dialogueTypingAudioClips;
    [SerializeField] private bool stopAudioSource; /* Determina se o áudio anterior para no momento em que um áudio seguinte é tocado */
    private AudioSource audioSource;

    [Range(1,5)]
    [SerializeField] private int frequencyLevel;

    [Range(-3f, 3f)]
    [SerializeField] private float minPitch;

    [Range(-3f, 3f)]
    [SerializeField] private float maxPitch;

    private void Awake()
    {
        targetTextBox = GetComponent<TMP_Text>();
        simpleDelay = new WaitForSeconds(1 /  charactersPerSecond);
        interpunctuationDelay = new WaitForSeconds(interpunctuationDelayValue);
        eraseTextDelay = new WaitForSeconds(eraseDelayValue / 500);

        audioSource = this.gameObject.AddComponent<AudioSource>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targetTextBox.maxVisibleCharacters = 0;
        currentlyVisibleCharacterIndex = 0;
        //Tenha em mente que para criar uma linha nova, você terá que por "<br>" na váriavel testText, aonde você quiser criar uma nova linha.
        SetText(testText[0]);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            SetText(testText[0]);
        }
    }

    public void SetText(string text)
    {
        if (typewritterCoroutine != null)
        {
            StopCoroutine(typewritterCoroutine);
        }


        typewritterCoroutine = StartCoroutine(Typewritter());
    }

    private IEnumerator Typewritter()
    {
        TMP_TextInfo textInfo = targetTextBox.textInfo;

        while (currentlyVisibleCharacterIndex > 0 && targetTextBox.maxVisibleCharacters > 0)
        {
            currentlyVisibleCharacterIndex--;
            targetTextBox.maxVisibleCharacters--;
            yield return eraseTextDelay;
        }

        while (currentlyVisibleCharacterIndex < textInfo.characterCount + 1)
        {
            char character = textInfo.characterInfo[currentlyVisibleCharacterIndex].character;
            targetTextBox.maxVisibleCharacters++;
            if ("?.,:;!-".Contains(character))
            {
                yield return interpunctuationDelay;
            }
            else 
            {
                yield return simpleDelay;
            }
            PlayDialogueSound(currentlyVisibleCharacterIndex);
            currentlyVisibleCharacterIndex++;
        }

    }

    private void PlayDialogueSound(int currentlyDisplayedCharacterCount)
    {
        if (currentlyDisplayedCharacterCount % frequencyLevel == 0)
        {
            if (stopAudioSource)
            {
                audioSource.Stop();
            }
            int randomIndex = Random.Range(0, dialogueTypingAudioClips.Length);
            AudioClip soundClip = dialogueTypingAudioClips[randomIndex];
            audioSource.pitch = Random.Range(minPitch, maxPitch);
            audioSource.PlayOneShot(soundClip);
        }
    }
}
