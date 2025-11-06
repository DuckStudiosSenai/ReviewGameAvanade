using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    [Header("References")]
    public GameObject productsMenu;

    private bool isProductsMenuActive = false;

    private APIManager api;

    public bool isMenuOpen = false;

    private void Start()
    {
        api = GameObject.FindGameObjectWithTag("GameManager")
            .GetComponent<APIManager>();
    }

    public void ToggleProductsMenu()
    {
        isProductsMenuActive = !isProductsMenuActive;

        if (isProductsMenuActive)
            api.DeleteChildren();

        productsMenu.SetActive(isProductsMenuActive);

        if (isProductsMenuActive)
            StartCoroutine(api.GetAllProducts());

        foreach (var p in FindObjectsByType<PlayerMovement>(FindObjectsSortMode.None))
        {
            if (p.photonView.IsMine)
            {
                p.isTyping = isProductsMenuActive;
                RenameProductsName();
                break;
            }
        }
    }

    public bool GetProductsMenuState()
    {
        return isProductsMenuActive;
    }

    public void RenameProductsName()
    {
        foreach (var product in FindObjectsByType<CategoryMenu>(FindObjectsSortMode.None))
        {
            product.productName.text = "Produtos";
        }
    }
}
