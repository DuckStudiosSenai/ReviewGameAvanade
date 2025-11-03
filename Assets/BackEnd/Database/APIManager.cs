using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.UI;

public class APIManager : MonoBehaviour
{
    [System.Serializable]
    public class ApiKeyResponse
    {
        public string apiKey;
    }

    [System.Serializable]
    public class ProductObject
    {
        public int id;
        public string name;
        public string description;
        public string enterprisename;
        public string category;
    }

    [System.Serializable]
    public class ProductList
    {
        public List<ProductObject> products;
    }

    private string baseUrl = "https://reviewgameapi.squareweb.app/api";

    public static string apiKey;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(GetAllProducts());
            Debug.Log("🔄 Recarregando produtos...");
        }
    }

    #region =====User=====
    public IEnumerator CreateUser(User newUser)
    {
        string json = JsonUtility.ToJson(newUser);
        using (UnityWebRequest www = new UnityWebRequest(baseUrl + "/users/register", "POST"))
        {
            byte[] jsonToSend = new UTF8Encoding().GetBytes(json);
            www.uploadHandler = new UploadHandlerRaw(jsonToSend);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.Success)
                Debug.Log("✅ Usuário registrado com sucesso!");
            else
                Debug.LogError("❌ Erro ao registrar: " + www.error);
        }
    }

    #endregion

    #region ====Auth=====

    public IEnumerator Login(string email, string password)
    {
        LoginRequest loginData = new LoginRequest { email = email, password = password };
        string json = JsonUtility.ToJson(loginData);

        using (UnityWebRequest www = new UnityWebRequest(baseUrl + "/users/login", "POST"))
        {
            byte[] jsonToSend = new UTF8Encoding().GetBytes(json);
            www.uploadHandler = new UploadHandlerRaw(jsonToSend);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.Success)
                Debug.Log("✅ Login realizado: " + www.downloadHandler.text);
            else
                Debug.LogError("❌ Erro no login: " + www.error);
        }
    }

    #endregion

    #region ====Products=====
    public IEnumerator GetAllProducts()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(baseUrl + "/Products"))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string json = www.downloadHandler.text;
                Debug.Log("📦 Produtos: " + json);

                List<ProductObject> products = JsonConvert.DeserializeObject<List<ProductObject>>(json);
                PopulateProducts(products);

            }
            else
            {
                Debug.LogError("❌ Erro ao buscar produtos: " + www.error);
            }
        }
    }

    public IEnumerator GetProductById(int id)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(baseUrl + "/products/" + id))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string json = www.downloadHandler.text;
                Product product = JsonUtility.FromJson<Product>(json);

                Debug.Log("🆔 Produto encontrado: " + product.name);
            }
            else
            {
                Debug.LogError("❌ Erro ao buscar produto por ID: " + www.error);
            }
        }
    }


    public IEnumerator CreateProduct(Product product)
    {
        string json = JsonUtility.ToJson(product);
        using (UnityWebRequest www = new UnityWebRequest(baseUrl + "/products", "POST"))
        {
            byte[] jsonToSend = new UTF8Encoding().GetBytes(json);
            www.uploadHandler = new UploadHandlerRaw(jsonToSend);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.Success)
                Debug.Log("✅ Produto criado com sucesso!");
            else
                Debug.LogError("❌ Erro ao criar produto: " + www.error);
        }
    }

    #region ====Reviews=====

    [System.Serializable]
    public class ReviewObject
    {
        public int id;
        public string content;
        public int rating;
        public string createdAt;
        public int userId;
        public string userName;
        public int? productId;
        public string productName;
    }

    //public IEnumerator GetReviewsByCategory(string category)
    //{
    //    string url = baseUrl + "/category/" + UnityWebRequest.EscapeURL(category);

    //    using (UnityWebRequest www = UnityWebRequest.Get(url))
    //    {
    //        yield return www.SendWebRequest();

    //        if (www.result == UnityWebRequest.Result.Success)
    //        {
    //            string json = www.downloadHandler.text;
    //            Debug.Log($"🗂️ Reviews da categoria '{category}': " + json);

    //            List<ReviewObject> reviews = JsonConvert.DeserializeObject<List<ReviewObject>>(json);
    //            PopulateReviews(reviews);
    //        }
    //        else
    //        {
    //            Debug.LogError("❌ Erro ao buscar reviews por categoria: " + www.error);
    //        }
    //    }
    //}

    public IEnumerator GetReviewsByCategory(string category)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(baseUrl + "/Products"))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("❌ Erro ao buscar produtos: " + www.error);
                yield break;
            }

            string json = www.downloadHandler.text;
            List<ProductObject> products = JsonConvert.DeserializeObject<List<ProductObject>>(json);

            var filtered = products.FindAll(p => p.category != null && p.category.Equals(category, System.StringComparison.OrdinalIgnoreCase));

            if (filtered.Count == 0)
            {
                Debug.LogWarning("⚠️ Nenhum produto encontrado na categoria: " + category);
                yield break;
            }

            List<ReviewObject> allReviews = new List<ReviewObject>();

            foreach (var product in filtered)
            {
                string reviewUrl = $"{baseUrl}/Reviews/product/{product.id}";
                using (UnityWebRequest wwwReviews = UnityWebRequest.Get(reviewUrl))
                {
                    yield return wwwReviews.SendWebRequest();

                    if (wwwReviews.result == UnityWebRequest.Result.Success)
                    {
                        string reviewsJson = wwwReviews.downloadHandler.text;
                        List<ReviewObject> reviews = JsonConvert.DeserializeObject<List<ReviewObject>>(reviewsJson);
                        allReviews.AddRange(reviews);
                    }
                    else
                    {
                        Debug.LogError($"❌ Erro ao buscar reviews do produto '{product.name}': {wwwReviews.error}");
                    }
                }
            }

            PopulateReviews(allReviews);
        }
    }


    #endregion


    #endregion

    [Header("UI Elements")]
    public GameObject prefabProduct;
    public Transform contentParent;
    private void PopulateProducts(List<ProductObject> products)
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        foreach (var product in products)
        {
            GameObject item = Instantiate(prefabProduct, contentParent);
            var ui = item.GetComponent<ProductItemUI>();
            ui.SetData(product);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(contentParent.GetComponent<RectTransform>());
    }

    [Header("UI Elements (Reviews)")]
    public GameObject prefabReview;
    public Transform contentReviewParent;

    private void PopulateReviews(List<ReviewObject> reviews)
    {
        Debug.Log($"🧱 Populando {reviews.Count} reviews...");

        foreach (Transform child in contentReviewParent)
            Destroy(child.gameObject);

        foreach (var review in reviews)
        {
            Debug.Log($"➡️ Criando review de {review.userName} - {review.productName}");
            GameObject item = Instantiate(prefabReview, contentReviewParent);
            var ui = item.GetComponent<ReviewItemUI>();

            if (ui != null)
                ui.SetData(review);
            else
                Debug.LogWarning("⚠️ ReviewItemUI não encontrado no prefab!");
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(contentReviewParent.GetComponent<RectTransform>());
    }


}

[System.Serializable]
public class LoginRequest
{
    public string email;
    public string password;
}
