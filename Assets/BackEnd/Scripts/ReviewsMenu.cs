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

    private APIManager api;
    private GameUIManager uiManager;

    private bool isAbleToOpen = false; // só TRUE para o jogador local

    private void Start()
    {
        api = GameObject.FindGameObjectWithTag("GameManager")
            .GetComponent<APIManager>();

        uiManager = FindAnyObjectByType<GameUIManager>();

        enabled = false; // Update começa desativado
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            OpenMenu();
    }

    private IEnumerator GetReviews()
    {
        yield return StartCoroutine(api.GetAllReviews());
        Debug.Log($"✅ [{name}] reviews carregados!");
    }

    public void OpenMenu()
    {
        if (!isAbleToOpen)
        {
            Debug.LogWarning($"⚠️ [{name}] não pode abrir o menu (fora da área).");
            return;
        }

        if (uiManager.isMenuOpen)
        {
            CloseMenu();
            return;
        }

        // Fecha apenas o próprio menu antes de abrir
        CloseMenu();

        api.DeleteChildren();
        menu.SetActive(true);
        uiManager.isMenuOpen = true;

        StartCoroutine(GetReviews());

        // Travar movimento somente do player local
        SetLocalPlayerTyping(true);

        Debug.Log($"📂 [{name}] menu aberto!");
    }

    public void CloseMenu()
    {
        menu.SetActive(false);
        uiManager.isMenuOpen = false;

        SetLocalPlayerTyping(false);
    }

    private void SetLocalPlayerTyping(bool state)
    {
        foreach (var p in FindObjectsByType<PlayerMovement>(FindObjectsSortMode.None))
        {
            if (p.photonView != null && p.photonView.IsMine)
            {
                p.isTyping = state;
                return;
            }
        }
    }

    // Somente ativa o menu para o jogador local
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var pv = collision.GetComponentInParent<Photon.Pun.PhotonView>();

        if (pv != null && pv.IsMine)
        {
            isAbleToOpen = true;
            enabled = true; // habilita Update
            Debug.Log($"🔓 [{name}] (LOCAL PLAYER) pode abrir o menu.");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var pv = collision.GetComponentInParent<Photon.Pun.PhotonView>();

        if (pv != null && pv.IsMine)
        {
            isAbleToOpen = false;
            enabled = false; // desabilita Update
            CloseMenu();
            Debug.Log($"🔒 [{name}] (LOCAL PLAYER) saiu da área.");
        }
    }
}
