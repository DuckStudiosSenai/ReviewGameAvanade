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
    [Header("Category")]
    public ProductCategory productCategory;
    public Transform contentParent;
    public GameObject prefab;
    public GameObject menu;
    public TextMeshProUGUI productName;

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

    private IEnumerator GetProducts()
    {
        string category = null;
        switch (productCategory)
        {
            case ProductCategory.DADOS_IA: category = "Dados e IA"; break;
            case ProductCategory.NUVEM_E_PLATAFORMAS: category = "Nuvem e Plataformas"; break;
            case ProductCategory.SEGURANCA: category = "Segurança"; break;
            case ProductCategory.TECNOLOGIA_INOVACAO: category = "Tecnologia e Inovação"; break;
            case ProductCategory.OUTROS: category = "Outros"; break;
        }

        if (string.IsNullOrEmpty(category))
        {
            Debug.LogWarning("⚠️ Categoria nula ou inválida.");
            yield break;
        }

        productName.text = category;
        yield return StartCoroutine(api.GetReviewsByCategory(category));
        Debug.Log($"📦 Produtos carregados para: {category}");
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
        StartCoroutine(GetProducts());

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
        foreach (var other in FindObjectsByType<CategoryMenu>(FindObjectsSortMode.None))
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
