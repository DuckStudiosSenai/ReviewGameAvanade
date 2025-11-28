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

    public GameObject suggestUi;

    public MakeReview makeReview;

    private APIManager api;
    private GameUIManager uiManager;
    private PlayerMovement player;

    private bool isAbleToOpen = false;

    private void Start()
    {
        api = GameObject.FindGameObjectWithTag("GameManager").GetComponent<APIManager>();
        uiManager = FindAnyObjectByType<GameUIManager>();

        foreach (var p in FindObjectsByType<PlayerMovement>(FindObjectsSortMode.None))
        {
            if (p.photonView.IsMine)
            {
                player = p;
                break;
            }
        }

        enabled = false;
    }

    private void Update()
    {
        if (!isAbleToOpen) return;

        if (Input.GetKeyDown(KeyCode.E) && !makeReview.isOpen)
            OpenMenu();
    }

    public void OpenMenu()
    {
        if (!player.photonView.IsMine) return;

        if (uiManager.isMenuOpen)
        {
            CloseAllMenus();
            suggestUi.SetActive(true);
            player.EnableMovement();
            return;
        }

        CloseAllMenus();

        api.DeleteChildren();
        menu.SetActive(true);
        uiManager.isMenuOpen = true;
        player.DisableMovement();
        suggestUi.SetActive(false);

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
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        PlayerMovement pm = collision.GetComponent<PlayerMovement>();
        if (pm != null && pm.photonView.IsMine)
        {
            player = pm;
            isAbleToOpen = true;
            enabled = true;
        }

        suggestUi = collision.gameObject.transform.Find("PlayerCanvas/Suggest/PressE").gameObject;
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
