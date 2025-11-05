using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    [Header("References")]
    public GameObject productsMenu;

    private bool isProductsMenuActive = false;

    public void ToggleProductsMenu()
    {
        isProductsMenuActive = !isProductsMenuActive;

        productsMenu.SetActive(isProductsMenuActive);

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
