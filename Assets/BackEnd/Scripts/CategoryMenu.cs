using System.Collections;
using TMPro;
using UnityEngine;
using static APIManager;

public enum ProductCategory
{
    DADOS_IA,
    NUVEM_E_PLATAFORMAS,
    SEGURANCA,
    TECNOLOGIA_INOVACAO,
    OUTROS
}

public class CategoryMenu : MonoBehaviour
{
    public ProductCategory productCategory;
    public GameObject menu;
    public TextMeshProUGUI productName;

    private APIManager api;
    private GameUIManager uiManager;
    private PlayerMovement localPlayer;

    private bool isAbleToOpen = false;

    private void Start()
    {
        api = GameObject.FindGameObjectWithTag("GameManager").GetComponent<APIManager>();
        uiManager = FindAnyObjectByType<GameUIManager>();

        // Encontre uma vez o player local
        foreach (var p in FindObjectsByType<PlayerMovement>(FindObjectsSortMode.None))
        {
            if (p.photonView.IsMine)
            {
                localPlayer = p;
                break;
            }
        }

        enabled = false;
    }

    private void Update()
    {
        if (!isAbleToOpen) return;

        if (Input.GetKeyDown(KeyCode.E))
            OpenMenu();
    }

    public void OpenMenu()
    {
        if (uiManager.isMenuOpen)
        {
            CloseAllMenus();
            return;
        }

        CloseAllMenus();

        api.DeleteChildren();
        menu.SetActive(true);
        uiManager.isMenuOpen = true;

        StartCoroutine(GetProducts());

    }

    private IEnumerator GetProducts()
    {
        string category = productCategory switch
        {
            ProductCategory.DADOS_IA => "Dados e IA",
            ProductCategory.NUVEM_E_PLATAFORMAS => "Nuvem e Plataformas",
            ProductCategory.SEGURANCA => "Segurança",
            ProductCategory.TECNOLOGIA_INOVACAO => "Tecnologia e Inovação",
            _ => "Outros"
        };

        productName.text = category;
        yield return StartCoroutine(api.GetProductsByCategory(category));
    }

    private void CloseAllMenus()
    {
        foreach (var other in FindObjectsByType<CategoryMenu>(FindObjectsSortMode.None))
            other.menu.SetActive(false);

        uiManager.isMenuOpen = false;

        if (localPlayer != null)
            localPlayer.isTyping = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        PlayerMovement pm = collision.GetComponent<PlayerMovement>();
        if (pm != null && pm.photonView.IsMine)
        {
            isAbleToOpen = true;
            enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        PlayerMovement pm = collision.GetComponent<PlayerMovement>();
        if (pm != null && pm.photonView.IsMine)
        {
            isAbleToOpen = false;
            enabled = false;
        }
    }
}
