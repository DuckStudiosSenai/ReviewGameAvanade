using System.Collections;
using TMPro;
using UnityEngine;
using static APIManager;

public class ReviewsMenu : MonoBehaviour
{
    [Header("Reviews")]
    public Transform contentParent;
    public GameObject prefab;
    public GameObject menu;
    //public TextMeshProUGUI productName;

    private APIManager api;
    private GameUIManager uiManager;

    private bool isAbleToOpen = false;

    private void Start()
    {
        api = GameObject.FindGameObjectWithTag("GameManager")
            .GetComponent<APIManager>();
        uiManager = FindAnyObjectByType<GameUIManager>();
        enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log($"🧠 [{name}] pressionou E (isAbleToOpen={isAbleToOpen}, menuOpen={uiManager.isMenuOpen})");
            OpenMenu();
        }
    }

    private IEnumerator GetReviews()
    {
        yield return StartCoroutine(api.GetAllReviews());
        Debug.Log("✅ [{name}] carregou os produtos da categoria com sucesso!");
    }

    public void OpenMenu()
    {
        if (!isAbleToOpen)
        {
            Debug.LogWarning($"⚠️ [{name}] não pode abrir menu (fora da área).");
            return;
        }

        if (uiManager.isMenuOpen)
        {

            CloseAllMenus();
            return;
        }

        CloseAllMenus();

        api.DeleteChildren();
        menu.SetActive(true);
        uiManager.isMenuOpen = true;
        StartCoroutine(GetReviews());

        foreach (var p in FindObjectsByType<PlayerMovement>(FindObjectsSortMode.None))
        {
            if (p.photonView.IsMine)
            {
                p.isTyping = true;
                break;
            }
        }

        Debug.Log($"✅ [{name}] abriu o menu com sucesso!");
    }

    private void CloseAllMenus()
    {
        foreach (var other in FindObjectsByType<ReviewsMenu>(FindObjectsSortMode.None))
        {
            if (other.menu != null)
                other.menu.SetActive(false);
        }

        uiManager.isMenuOpen = false;

        foreach (var p in FindObjectsByType<PlayerMovement>(FindObjectsSortMode.None))
        {
            if (p.photonView.IsMine)
            {
                p.isTyping = false;
                break;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isAbleToOpen = true;
            enabled = true;
            Debug.Log($"🔓 [{name}] pode abrir o menu (pressione E).");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isAbleToOpen = false;
            enabled = false;
            Debug.Log($"🔒 [{name}] saiu da área, desativando Update.");
        }
    }
}
