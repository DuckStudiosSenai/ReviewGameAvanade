using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static APIManager;

public enum MiniGame
{
    CATCH_GAME,
    MEMORY_GAME,
    AI_GAME
}

public class GameSelector : MonoBehaviour
{
    [Header("Category")]
    public MiniGame minigame;

    private bool isAbleToOpen = false;

    private void Start()
    {
        enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log($"[{name}] pressionou E");
            OpenScene();
        }
    }

    private IEnumerator GetGames()
    {
        string category = null;
        int gameIndex = 0;
        switch (minigame)
        {
            case MiniGame.CATCH_GAME: category = "Duck Catch Game"; gameIndex = 1; break;
            case MiniGame.MEMORY_GAME: category = "Jogo da memória"; gameIndex = 2; break;
            case MiniGame.AI_GAME: category = "Jogo adivinhe a IA"; gameIndex = 3; break;
        }

        if (string.IsNullOrEmpty(category))
        {
            Debug.LogWarning("⚠️ Categoria nula ou inválida.");
            yield break;
        }

        Debug.Log($"🎮 Jogo {category} iniciado.");
        SceneManager.LoadScene(gameIndex);
    }

    public void OpenScene()
    {
        if (!isAbleToOpen)
        {
            Debug.LogWarning($"⚠️ [{name}] não pode abrir menu (fora da área).");
            return;
        }

        CloseAllMenus();

        foreach (var p in FindObjectsByType<PlayerMovement>(FindObjectsSortMode.None))
        {
            if (p.photonView.IsMine)
            {
                p.DisableMovement();
                break;
            }
        }

        StartCoroutine(GetGames());
    }

    private void CloseAllMenus()
    {
        foreach (var other in FindObjectsByType<CategoryMenu>(FindObjectsSortMode.None))
        {
            if (other.menu != null)
                other.menu.SetActive(false);
        }

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
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isAbleToOpen = false;
            enabled = false;
        }
    }
}
