using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public PointsManager pointsManager;
    public List<ItemShop> shopItems = new List<ItemShop>();
    public GameObject itemPrefab;
    public RectTransform shopContentPanel;

    [Header("Preview")]
    public Image itemIcon;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemPriceText;
    public Button buyButton;

    void Start()
    {
        LoadShopItems();
        LoadPreview(shopItems[0]);
    }

    void LoadShopItems()
    {
        ResetItems();
        foreach (var item in shopItems)
        {
            Debug.Log($"[ItemShop] Item: {item.itemName}, Price: {item.itemPrice}, Description: {item.itemDescription}");
            LoadItemPrefab(item);
        }
    }

    void SelectItem(ItemShop item)
    {
        LoadPreview(item);
    }

    void LoadItemPrefab(ItemShop item)
    {
        GameObject newItem = Instantiate(itemPrefab, shopContentPanel);

        Image itemIconRenderer = newItem.transform
            .Find("ContentContainer/Icon/Sprite")
            .GetComponent<Image>();

        TextMeshProUGUI itemNameText = newItem.transform
            .Find("ContentContainer/ItemName")
            .GetComponent<TextMeshProUGUI>();

        TextMeshProUGUI itemPriceText = newItem.transform
            .Find("ContentContainer/ItemPrice")
            .GetComponent<TextMeshProUGUI>();

        Button itemButton = newItem.GetComponent<Button>();

        itemIconRenderer.sprite = item.itemIcon;
        itemIconRenderer.transform.localScale = Vector3.one * item.itemIconScale * 3;
        itemNameText.text = item.itemName;
        itemPriceText.text = item.itemPrice.ToString();

        itemButton.onClick.AddListener(() => SelectItem(item));

        Debug.Log("Listener adicionado com sucesso");
    }


    void BuyItem(ItemShop item, int userId)
    {
        Debug.Log($"Iniciando compra do item: {item.itemName} por {item.itemPrice} pontos.");
        StartCoroutine(pointsManager.GetUserPoints(
            userId,
            onSuccess: (points, currentPoints) =>
            {
                if (currentPoints >= item.itemPrice)
                {
                    int newCurrentPoints = currentPoints - item.itemPrice;

                    StartCoroutine(pointsManager.UpdateUserPoints(
                    userId,
                    points: null,
                    currentPoints: newCurrentPoints,
                    onSuccess: () =>
                    {
                        Debug.Log($"✅ Item comprado: {item.itemName} por {item.itemPrice} pontos. Pontos restantes: {newCurrentPoints}");
                        LoadShopItems();
                    },
                    onError: (err) =>
                    {
                        Debug.LogError("Erro ao atualizar pontos: " + err);
                    }
                ));
                }
                else
                {
                    Debug.LogWarning("Pontos insuficientes para comprar o item: " + item.itemName);
                }
            },
            onError: (err) =>
            {
                Debug.LogError("Erro ao buscar pontos: " + err);
            }
        ));
    }

    void LoadPreview(ItemShop item)
    {
        itemIcon.sprite = item.itemIcon;
        itemIcon.transform.localScale = Vector3.one * item.itemIconScale * 9;
        itemNameText.text = item.itemName;
        itemPriceText.text = item.itemPrice.ToString();

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => BuyItem(item, 1));

        Debug.Log($"Preview carregado: {item.itemName}, Preço: {item.itemPrice}");
    }

    void ResetItems()
    {
        foreach (Transform child in shopContentPanel)
        {
            Destroy(child.gameObject);
        }
    }
}
