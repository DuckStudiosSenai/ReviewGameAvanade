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

    [System.Serializable]
    public class ReviewCreateDTO
    {
        public int userId;
        public int productId;
        public int rating;
        public string content;
    }


    [System.Serializable]
    public class ReviewResponseDto
    {
        public int id;
        public int userId;
        public int productId;
        public int rating;
        public string comment;
        public string createdAt;
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

    public IEnumerator CreateReview(ReviewCreateDTO newReview)
    {
        // ✅ Cria o JSON exatamente como a API espera
        string json = JsonConvert.SerializeObject(newReview);

        using (UnityWebRequest request = new UnityWebRequest(baseUrl + "/Reviews", "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("✅ Review criada com sucesso!");
                Debug.Log("Resposta: " + request.downloadHandler.text);

                try
                {
                    ReviewResponseDto response = JsonConvert.DeserializeObject<ReviewResponseDto>(request.downloadHandler.text);
                    Debug.Log($"🆔 ID: {response.id} | Produto: {response.productId} | Nota: {response.rating}");
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning($"⚠️ Não foi possível desserializar a resposta: {ex.Message}");
                }
            }
            else
            {
                Debug.LogError($"❌ Erro ao criar review: {request.responseCode} - {request.error}");
                Debug.LogError(request.downloadHandler.text);
            }
        }
    }




    public IEnumerator GetReviewsByCategory(string category)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(baseUrl + "/Products"))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("❌ Erro ao buscar produtos: " + www.error);
                PopulateReviews(new List<ReviewObject>()); // limpa caso erro de rede
                yield break;
            }

            string json = www.downloadHandler.text;
            List<ProductObject> products = JsonConvert.DeserializeObject<List<ProductObject>>(json);

            // filtra produtos pela categoria
            var filtered = products.FindAll(p => p.category != null &&
                p.category.Equals(category, System.StringComparison.OrdinalIgnoreCase));

            // caso não tenha nenhum produto, limpar reviews e sair
            if (filtered.Count == 0)
            {
                Debug.LogWarning("⚠️ Nenhum produto encontrado na categoria: " + category);
                PopulateReviews(new List<ReviewObject>()); // força limpar conteúdo
                yield break;
            }

            // busca os reviews de todos os produtos filtrados
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

            // popula reviews (mesmo se a lista estiver vazia)
            PopulateReviews(allReviews);
        }
    }

    public IEnumerator DeleteReview(int reviewId, System.Action<bool> onComplete = null)
    {
        string url = $"{baseUrl}/Reviews/{reviewId}";

        using (UnityWebRequest request = UnityWebRequest.Delete(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"✅ Review {reviewId} deletada com sucesso!");
                onComplete?.Invoke(true);
            }
            else
            {
                Debug.LogError($"❌ Erro ao deletar review {reviewId}: {request.responseCode} - {request.error}");
                Debug.LogError(request.downloadHandler.text);
                onComplete?.Invoke(false);
            }
        }
    }




    #endregion


    #endregion

    [Header("UI Elements")]
    public GameObject prefabProduct;
    public Transform contentParent;
    public MakeReview makeReview; // referência ao script MakeReview

    private void PopulateProducts(List<ProductObject> products)
    {
        // limpa tudo sempre
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        // cria apenas se houver produtos
        if (products != null && products.Count > 0)
        {
            foreach (var product in products)
            {
                GameObject item = Instantiate(prefabProduct, contentParent);

                // preenche dados visuais
                var ui = item.GetComponent<ProductItemUI>();
                if (ui != null)
                    ui.SetData(product);

                // adiciona o evento de clique no botão da prefab
                var button = item.GetComponentInChildren<Button>();
                if (button != null)
                {
                    int capturedId = product.id; // evita bug de closure em loop
                    string capturedName = product.name;
                    button.onClick.AddListener(() =>
                    {
                        makeReview.OpenReviewMenu(true, capturedId, 1, capturedName);
                    });
                }
                else
                {
                    Debug.LogWarning($"⚠️ Prefab {item.name} não possui Button configurado.");
                }
            }
        }
        else
        {
            Debug.Log("🟡 Nenhum produto encontrado. Lista vazia, mas conteúdo limpo.");
        }

        // força atualização do layout mesmo vazio
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentParent.GetComponent<RectTransform>());
    }



    [Header("UI Elements (Reviews)")]
    public GameObject prefabReview;
    public Transform contentReviewParent;

    private void PopulateReviews(List<ReviewObject> reviews)
    {
        StartCoroutine(DoPopulateReviews(reviews));
    }

    private IEnumerator DoPopulateReviews(List<ReviewObject> reviews)
    {
        Debug.Log($"🧱 Limpando {contentReviewParent.childCount} reviews antigos...");

        // Limpa os filhos sempre
        foreach (Transform child in contentReviewParent)
            Destroy(child.gameObject);

        // Espera 1 frame para o Unity processar as destruições
        yield return null;

        if (reviews != null && reviews.Count > 0)
        {
            Debug.Log($"🧩 Adicionando {reviews.Count} novos reviews...");
            foreach (var review in reviews)
            {
                GameObject item = Instantiate(prefabReview, contentReviewParent);
                var ui = item.GetComponent<ReviewItemUI>();
                if (ui != null)
                    ui.SetData(review);
            }
        }
        else
        {
            Debug.Log("🟡 Nenhum review recebido — lista esvaziada corretamente.");
        }

        // Espera mais um frame para garantir layout atualizado
        yield return null;

        LayoutRebuilder.ForceRebuildLayoutImmediate(contentReviewParent.GetComponent<RectTransform>());
    }

}

[System.Serializable]
public class LoginRequest
{
    public string email;
    public string password;
}
