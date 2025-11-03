using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    private APIManager api;

    private bool isMenuOpen = false;

    public bool isAbleToOpen = false;

    void Start()
    {
        api = GameObject.FindGameObjectWithTag("GameManager")
            .GetComponent<APIManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            OpenMenu();
        }
    }

    private IEnumerator GetProducts()
    {
        string category = null;

        switch (productCategory)
        {
            case ProductCategory.DADOS_IA:
                //category = "Dados e IA";
                category = "Hardware";
                break;
            case ProductCategory.NUVEM_E_PLATAFORMAS:
                category = "Nuvem e Plataformas";
                break;
            case ProductCategory.SEGURANCA:
                category = "Segurança";
                break;
            case ProductCategory.TECNOLOGIA_INOVACAO:
                category = "Tecnologia e Inovação";
                break;
            case ProductCategory.OUTROS:
                category = "Outros";
                Debug.Log($"📂 Categoria selecionada: {category}");
                break;

            default:
                Debug.LogWarning("⚠️ Categoria desconhecida.");
                break;
        }

        if (category != null)
            yield return StartCoroutine(api.GetReviewsByCategory(category));
        else
            Debug.LogWarning("⚠️ Categoria nula.");
    }


    public void OpenMenu()
    {

        if (isAbleToOpen) menu.SetActive(true);

        isMenuOpen = !isMenuOpen;

        if (isAbleToOpen)
        {
            StartCoroutine(GetProducts());
            Debug.Log("📂 Abrindo menu de categorias...");
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isAbleToOpen = true;
            Debug.Log("🔓 Você pode abrir o menu de categorias. Pressione 'E' para abrir.");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isAbleToOpen = false;
        }
    }
}
