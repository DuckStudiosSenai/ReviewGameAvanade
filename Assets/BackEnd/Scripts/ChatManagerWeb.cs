using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class ChatMessage
{
    public string sender;
    public string text;
}

[System.Serializable]
public class ChatHistory
{
    public List<ChatMessage> messages = new List<ChatMessage>();
}

public class ChatManagerWeb : MonoBehaviour
{
    [Header("Referências UI")]
    public TMP_InputField inputField;
    public Button sendButton;
    public Transform messageContainer;
    public GameObject messagePrefab;
    public ScrollRect scrollRect;

    private ChatHistory chatHistory = new ChatHistory();
    private AzureChatClient chatClient;

    private void Awake()
    {
        chatClient = FindAnyObjectByType<AzureChatClient>();
        if (chatClient == null) Debug.LogError("AzureChatClient não encontrado na cena.");
    }

    void Start()
    {
        uiManager = FindAnyObjectByType<GameUIManager>();

        DeleteHistory();
        LoadHistory();
        sendButton.onClick.AddListener(SendMessage);
    }

    void SendMessage()
    {
        string text = inputField.text.Trim();
        if (string.IsNullOrEmpty(text)) return;

        AddMessage("Você", text);
        inputField.text = "";
        StartCoroutine(BotResponse(text));
    }

    IEnumerator BotResponse(string userText)
    {
        yield return new WaitForSeconds(0.5f);
        chatClient.SendMessage(userText, (reply) =>
        {
            AddMessage("Bot", reply);
        });
    }

    void AddMessage(string sender, string text)
    {
        GameObject msgObj = Instantiate(messagePrefab, messageContainer);
        TMP_Text msgText = msgObj.GetComponentInChildren<TMP_Text>();
        msgText.text = $"<b>{sender}:</b> {text}";

        chatHistory.messages.Add(new ChatMessage { sender = sender, text = text });
        SaveHistory();

        StartCoroutine(ScrollToBottomNextFrame());
    }

    IEnumerator ScrollToBottomNextFrame()
    {
        yield return null;

        if (scrollRect != null)
            scrollRect.verticalNormalizedPosition = 0f;
    }

    void SaveHistory()
    {
        string json = JsonUtility.ToJson(chatHistory);
        PlayerPrefs.SetString("chat_history", json);
        PlayerPrefs.Save();
    }

    void LoadHistory()
    {
        if (PlayerPrefs.HasKey("chat_history"))
        {
            string json = PlayerPrefs.GetString("chat_history");
            chatHistory = JsonUtility.FromJson<ChatHistory>(json);

            foreach (var msg in chatHistory.messages)
            {
                GameObject msgObj = Instantiate(messagePrefab, messageContainer);
                TMP_Text msgText = msgObj.GetComponentInChildren<TMP_Text>();
                msgText.text = $"<b>{msg.sender}:</b> {msg.text}";
            }

            StartCoroutine(ScrollToBottomNextFrame());
        }
    }

    void DeleteHistory()
    {
        PlayerPrefs.DeleteKey("chat_history");
        PlayerPrefs.Save();
        chatHistory.messages.Clear();
    }

    private GameUIManager uiManager;
    public void SetPlayerTypingState(bool state)
    {
        foreach (var p in FindObjectsByType<PlayerMovement>(FindObjectsSortMode.None))
        {
            if (p.photonView.IsMine)
            {
                if (!uiManager.GetProductsMenuState() && !uiManager.isMenuOpen)
                    p.isTyping = state;
                break;
            }
        }
    }
}
