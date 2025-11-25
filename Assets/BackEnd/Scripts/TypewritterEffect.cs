using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class TypewritterEffect : MonoBehaviour
{
    private TMP_Text targetTextBox;
    public GameObject secretaryDialogue;

    [Header("Texts")]
    public string[] testText;
    private int currentTextIndex = 0;

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
    private AudioSource audioSource;

    private float lastSoundTime = 0f;
    private float soundCooldown = 0.07f;

    private PlayerMovement localPlayerMovement;
    public bool isTyping = false;
    public bool isFristTime = true;

    [Header("Background Music")]
    public GameObject bgmPrefab;
    private GameObject bgmInstance;

    void Awake()
    {
        targetTextBox = GetComponent<TMP_Text>();
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Start()
    {
        simpleDelay = new WaitForSeconds(1f / charactersPerSecond);
        interpunctuationDelay = new WaitForSeconds(interpunctuationDelayValue);
        eraseTextDelay = new WaitForSeconds(eraseDelayValue);

        localPlayerMovement = FindAnyObjectByType<PlayerMovement>();

        // Instancia BGM local para o jogador
        if (bgmPrefab != null)
        {
            bgmInstance = Instantiate(bgmPrefab);
            bgmInstance.SetActive(false);
            DontDestroyOnLoad(bgmInstance);
        }

        SetText(testText[currentTextIndex]);
    }

    private void OnNextTextRequested()
    {
        if (isTyping)
        {
            StopCoroutine(typewritterCoroutine);
            targetTextBox.maxVisibleCharacters = targetTextBox.text.Length;
            isTyping = false;
            return;
        }

        if (currentTextIndex < testText.Length - 1)
        {
            currentTextIndex++;
            SetText(testText[currentTextIndex]);
            return;
        }

        if (localPlayerMovement != null)
            localPlayerMovement.EnableMovement();

        if (secretaryDialogue != null)
            secretaryDialogue.SetActive(false);

        if (isFristTime)
        {
            if (bgmInstance != null)
                bgmInstance.SetActive(true);

            isFristTime = false;
        }
    }

    public void SetText(string text)
    {
        targetTextBox.text = text;
        currentlyVisibleCharacterIndex = 0;
        targetTextBox.maxVisibleCharacters = 0;

        if (typewritterCoroutine != null)
            StopCoroutine(typewritterCoroutine);

        typewritterCoroutine = StartCoroutine(TypewritterCoroutine(text));
    }

    private IEnumerator TypewritterCoroutine(string text)
    {
        isTyping = true;

        foreach (char c in text)
        {
            targetTextBox.maxVisibleCharacters++;
            currentlyVisibleCharacterIndex++;

            if (dialogueTypingAudioClips.Length > 0 && audioSource != null)
            {
                if (Time.time - lastSoundTime >= soundCooldown)
                {
                    audioSource.PlayOneShot(dialogueTypingAudioClips[Random.Range(0, dialogueTypingAudioClips.Length)]);
                    lastSoundTime = Time.time;
                }
            }

            if (c == '.' || c == ',' || c == '!' || c == '?')
                yield return interpunctuationDelay;
            else
                yield return simpleDelay;
        }

        isTyping = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            OnNextTextRequested();
    }
}
