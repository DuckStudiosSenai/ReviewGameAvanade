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
    }
}
