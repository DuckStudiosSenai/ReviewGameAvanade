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
            StartCoroutine(GetProducts());
            OpenMenu();
        }
    }

    private IEnumerator GetProducts()
    {
        string category = productCategory.ToString();

        switch (productCategory)
        {
            case ProductCategory.DADOS_IA:
            case ProductCategory.NUVEM_E_PLATAFORMAS:
            case ProductCategory.SEGURANCA:
            case ProductCategory.TECNOLOGIA_INOVACAO:
            case ProductCategory.OUTROS:
                Debug.Log($"📂 Categoria selecionada: {category}");
                yield return StartCoroutine(api.GetReviewsByCategory(category));
                break;

            default:
                Debug.LogWarning("⚠️ Categoria desconhecida.");
                break;
        }
    }

    
    public void OpenMenu()
    {
        if (isAbleToOpen) menu.SetActive(!menu.activeSelf);
        else menu.SetActive(false);

        isMenuOpen = !isMenuOpen;
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
