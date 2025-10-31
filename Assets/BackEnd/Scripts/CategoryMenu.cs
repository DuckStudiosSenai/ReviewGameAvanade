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

    private APIManager api;

    void Start()
    {
        api = GameObject.FindGameObjectWithTag("GameManager")
            .GetComponent<APIManager>();

        StartCoroutine(GetProducts());
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
}
