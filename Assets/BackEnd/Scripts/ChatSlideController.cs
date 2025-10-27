using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Rendering.UI;

public class ChatSlideController : MonoBehaviour
{
    [Header("Referências")]
    public RectTransform chatPanel;    
    public Button headerButton; 
    public Button sendButton;

    [Header("Animação")]
    [Tooltip("Velocidade do movimento de deslize.")]
    public float slideSpeed = 5f;

    [Tooltip("Quanto o chat se move ao sair (em pixels).")]
    public float slideDistance = 500f;  

    [Tooltip("Se verdadeiro, usa a altura do painel como distância de saída.")]
    public bool usePanelHeight = true;

    private Vector2 visiblePosition;
    private Vector2 hiddenPosition;
    private bool isVisible = false;
    private Coroutine slideRoutine;
    private NPCSpawner npc;

    private void Update()
    {
        if (isVisible)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                sendButton.onClick.Invoke();
            }
        }
    }

    void Start()
    {
        npc = FindAnyObjectByType<NPCSpawner>();

        visiblePosition = chatPanel.anchoredPosition;

        float distance = usePanelHeight ? chatPanel.rect.height : slideDistance;
        hiddenPosition = new Vector2(visiblePosition.x, visiblePosition.y - distance);

        chatPanel.anchoredPosition = hiddenPosition;

        headerButton.onClick.AddListener(ToggleChat);
    }

    public void ToggleChat()
    {
        if (slideRoutine != null)
            StopCoroutine(slideRoutine);

        Vector2 target = isVisible ? hiddenPosition : visiblePosition;
        slideRoutine = StartCoroutine(SlideChat(target));

        if (!isVisible)
        { // Abriu

            if (npc.GetActiveNPC() == null)
            {
                npc.SpawnAndChase();
            }
            else
            {
                npc.GetActiveNPC().GetComponent<NPCController>().RestartChase();
            }

        } else
        { // Fechou 
            npc.SendNPCAway();
        }
        
        isVisible = !isVisible;
    }

    IEnumerator SlideChat(Vector2 target)
    {
        Vector2 start = chatPanel.anchoredPosition;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * slideSpeed;
            float eased = Mathf.SmoothStep(0, 1, t);
            chatPanel.anchoredPosition = Vector2.Lerp(start, target, eased);
            yield return null;
        }

        chatPanel.anchoredPosition = target;
    }
}
