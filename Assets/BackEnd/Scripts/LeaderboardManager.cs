using System.Collections;
using TMPro;
using UnityEngine;
using static APIManager;

public class LeaderboardManager : MonoBehaviour
{
    [Header("Leaderboard UI")]
    public Transform contentParent;
    public GameObject prefab;
    public GameObject leaderboardCanvas;

    private APIManager api;
    private GameUIManager uiManager;

    private void Start()
    {
        api = GameObject.FindGameObjectWithTag("GameManager")
            .GetComponent<APIManager>();
        uiManager = FindAnyObjectByType<GameUIManager>();
    }

    private IEnumerator GetLeaderboardData()
    {
        yield return StartCoroutine(api.GetUsersWithReviews());
        Debug.Log("✅ [{name}] carregou a leaderboard com sucesso!");
    }

    public void ToggleLeaderboard()
    {
        if (uiManager.isMenuOpen)
        {
            CloseLeaderboard();
        }
        else
        {
            OpenLeaderboard();
        }
    }

    public void OpenLeaderboard()
    {
        if (uiManager.isMenuOpen)
        {
            Debug.LogWarning("⚠️ [{name}] outro menu já está aberto.");
            return;
        }

        CloseAllMenus();

        // Limpa leaderboard apenas se contentLeaderboardParent estiver atribuído
        if (api.contentLeaderboardParent != null)
            api.DeleteLeaderboardChildren();
        else
            Debug.LogWarning("⚠️ contentLeaderboardParent não atribuído no APIManager. UI abrirá mas sem lista.");

        leaderboardCanvas.SetActive(true);
        uiManager.isMenuOpen = true;
        
        if (api.contentLeaderboardParent != null)
            StartCoroutine(GetLeaderboardData());

        foreach (var p in FindObjectsByType<PlayerMovement>(FindObjectsSortMode.None))
        {
            if (p.photonView.IsMine)
            {
                p.isTyping = true;
                break;
            }
        }

        Debug.Log($"✅ [{name}] abriu a leaderboard com sucesso!");
    }

    public void CloseLeaderboard()
    {
        leaderboardCanvas.SetActive(false);
        uiManager.isMenuOpen = false;

        foreach (var p in FindObjectsByType<PlayerMovement>(FindObjectsSortMode.None))
        {
            if (p.photonView.IsMine)
            {
                p.isTyping = false;
                break;
            }
        }

        Debug.Log($"✅ [{name}] fechou a leaderboard com sucesso!");
    }

    private void CloseAllMenus()
    {
        foreach (var other in FindObjectsByType<LeaderboardManager>(FindObjectsSortMode.None))
        {
            if (other.leaderboardCanvas != null)
                other.leaderboardCanvas.SetActive(false);
        }

        foreach (var other in FindObjectsByType<ReviewsMenu>(FindObjectsSortMode.None))
        {
            if (other.menu != null)
                other.menu.SetActive(false);
        }

        foreach (var other in FindObjectsByType<CategoryMenu>(FindObjectsSortMode.None))
        {
            if (other.menu != null)
                other.menu.SetActive(false);
        }

        uiManager.isMenuOpen = false;
    }
}
