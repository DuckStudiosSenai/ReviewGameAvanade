using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Photon.Pun;

public class ChatSlideController : MonoBehaviour
{
    [Header("Referências")]
    public RectTransform chatPanel;
    public Button headerButton;
    public Button sendButton;

    [Header("Configuração de Deslize")]
    [Tooltip("Multiplicador da distância de deslize (1 = altura total do painel)")]
    [SerializeField] private float slideMultiplier = 1f;

    private bool isLocalPlayer = false;
    private NPCSpawner npc;
    private Vector2 visiblePosition;
    private Vector2 hiddenPosition;
    private bool isVisible = false;
    private Coroutine slideRoutine;

    void Start()
    {
        chatPanel.gameObject.SetActive(false);
        StartCoroutine(WaitForLocalPlayer());
    }

    IEnumerator WaitForLocalPlayer()
    {
        PhotonView localView = null;

        while (localView == null)
        {
            localView = FindLocalPlayerView();
            yield return new WaitForSeconds(0.1f);
        }

        Debug.Log("✅ Player local encontrado!");

        chatPanel.gameObject.SetActive(true);
        isLocalPlayer = true;

        npc = FindAnyObjectByType<NPCSpawner>();

        visiblePosition = chatPanel.anchoredPosition;
        float distance = chatPanel.rect.height * slideMultiplier;
        hiddenPosition = new Vector2(visiblePosition.x, visiblePosition.y - distance);
        chatPanel.anchoredPosition = hiddenPosition;

        headerButton.onClick.AddListener(ToggleChat);
    }

    PhotonView FindLocalPlayerView()
    {
        foreach (var view in FindObjectsByType<PhotonView>(FindObjectsSortMode.None))
        {
            if (view.IsMine && view.CompareTag("Player"))
                return view;
        }
        return null;
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        if (isVisible && Input.GetKeyDown(KeyCode.Return))
            sendButton.onClick.Invoke();
    }

    public void ToggleChat()
    {
        if (!isLocalPlayer) return;

        if (slideRoutine != null)
            StopCoroutine(slideRoutine);

        Vector2 target = isVisible ? hiddenPosition : visiblePosition;
        slideRoutine = StartCoroutine(SlideChat(target));

        if (npc != null)
        {
            if (!isVisible)
            {
                if (npc.GetActiveNPC() == null)
                    npc.SpawnAndChase();
                else
                    npc.GetActiveNPC().GetComponent<NPCController>().RestartChase();
            }
            else
            {
                npc.SendNPCAway();
            }
        }

        isVisible = !isVisible;
    }

    IEnumerator SlideChat(Vector2 target)
    {
        Vector2 start = chatPanel.anchoredPosition;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * 5f;
            float eased = Mathf.SmoothStep(0, 1, t);
            chatPanel.anchoredPosition = Vector2.Lerp(start, target, eased);
            yield return null;
        }

        chatPanel.anchoredPosition = target;
    }
}
