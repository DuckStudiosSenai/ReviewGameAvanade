using System.Collections;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Unity.VisualScripting;

[System.Serializable]
public class OwnsItemDto
{
    public int userId;
    public int itemId;
    public bool owns;
}


public class ShopManager : MonoBehaviour
{
    private const string baseUrl = "https://reviewgameapi.squareweb.app/api";

    public PointsManager pointsManager;
    public List<ItemShop> shopItems = new List<ItemShop>();
    public GameObject itemPrefab;
    public RectTransform shopContentPanel;
    public GameObject shopMenu;

    [Header("Preview")]
    public Image itemIcon;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemPriceText;
    public Button buyButton;

    private PlayFabManager api;

    void Start()
    {
        api = FindAnyObjectByType<PlayFabManager>();

        LoadShopItems();
        LoadPreview(shopItems[0]);
    }

    void LoadShopItems()
    {
        ResetItems();

        foreach (var item in shopItems)
        {
            PlayerHasShopItem(item.itemID, api.GetUserId(), owns =>
            {
                if (!owns)
                {
                    LoadItemPrefab(item);
                }
            });
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
        PlayerHasShopItem(item.itemID, api.GetUserId(), owns =>
        {
            if (owns)
            {
                Debug.LogWarning("O usuário já possui o item: " + item.itemName);
                return;
            } else
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
                                AddItemToUser(userId, item.itemID);
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
        });

        
    }

    void LoadPreview(ItemShop item)
    {
        itemIcon.sprite = item.itemIcon;
        itemIcon.transform.localScale = Vector3.one * item.itemIconScale * 9;
        itemNameText.text = item.itemName;
        itemPriceText.text = item.itemPrice.ToString();

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => BuyItem(item, api.GetUserId()));

        Debug.Log($"Preview carregado: {item.itemName}, Preço: {item.itemPrice}");
    }

    void ResetItems()
    {
        foreach (Transform child in shopContentPanel)
        {
            Destroy(child.gameObject);
        }
    }

    public static void UserHasItem(
    int userId,
    int itemId,
    Action<bool> onResult,
    Action<string> onError = null
)
    {
        CoroutineRunner.instance.StartCoroutine(
            UserHasItemCoroutine(
                userId,
                itemId,
                onResult,
                onError
            )
        );
    }


    public static IEnumerator UserHasItemCoroutine(
    int userId,
    int itemId,
    Action<bool> onSuccess,
    Action<string> onError
)
    {
        string url = $"{baseUrl}/Users/{userId}/owns/{itemId}";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                onError?.Invoke(request.error);
                yield break;
            }

            try
            {
                OwnsItemDto response =
                    JsonUtility.FromJson<OwnsItemDto>(request.downloadHandler.text);

                onSuccess?.Invoke(response.owns);
            }
            catch (System.Exception e)
            {
                onError?.Invoke("Erro ao converter JSON: " + e.Message);
            }
        }
    }

    public void AddItemToUser(int userId, int itemId)
    {
        StartCoroutine(AddItemCoroutine(userId, itemId));
    }

    public void RemoveItemFromUser(int userId, int itemId)
    {
        StartCoroutine(RemoveItemCoroutine(userId, itemId));
    }

    private IEnumerator AddItemCoroutine(int userId, int itemId)
    {
        string url = $"{baseUrl}/Users/{userId}/owns/{itemId}";

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(Array.Empty<byte>());
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            Debug.Log($"[AddItem] Status: {request.responseCode}");
            Debug.Log($"[AddItem] Response: {request.downloadHandler.text}");

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[AddItem] ERRO: {request.error}");
            }
        }
    }

    private IEnumerator RemoveItemCoroutine(int userId, int itemId)
    {
        string url = $"{baseUrl}/Users/{userId}/owns/{itemId}";

        using (UnityWebRequest request = new UnityWebRequest(url, "DELETE"))
        {
            request.uploadHandler = new UploadHandlerRaw(Array.Empty<byte>());
            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.SendWebRequest();

            Debug.Log($"[RemoveItem] Status: {request.responseCode}");
            Debug.Log($"[RemoveItem] Response: {request.downloadHandler.text}");

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[RemoveItem] ERRO: {request.error}");
            }
        }
    }


    private IEnumerator SendRequest(UnityWebRequest request)
    {
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(
                $"[HTTP ERROR]\n" +
                $"Status: {request.responseCode}\n" +
                $"URL: {request.url}\n" +
                $"Method: {request.method}\n" +
                $"Body:\n{request.downloadHandler?.text}"
            );
        }
        else
        {
            Debug.Log(
                $"[HTTP OK]\n" +
                $"Status: {request.responseCode}\n" +
                $"Body:\n{request.downloadHandler.text}"
            );
        }
    }


    public static void PlayerHasShopItem(int itemId, int userId, Action<bool> onResult)
    {
        UserHasItem(
            userId,
            itemId,
            owns =>
            {
                onResult?.Invoke(owns);
            },
            error =>
            {
                Debug.LogError(error);
                onResult?.Invoke(false);
            }
        );
    }

    public void ToggleShopMenu()
    {
        shopMenu.SetActive(!shopMenu.activeSelf);
    }
}
