using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.UI;
using System;
using System.Linq;

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
    public class UserDto
    {
        public int id;
        public string name;
        public string email;
        public string enterprise;
        public int points;
        public int currentpoints;
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

    private void Awake()
    {
        playfab = FindAnyObjectByType<PlayFabManager>();
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

    public IEnumerator GetProductsByCategory(string category)
    { 
        string url = $"{baseUrl}/Products/category/{category}";

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            Debug.Log($"🔍 Buscando produtos na categoria: {category}");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string json = www.downloadHandler.text;

                Debug.Log("📦 Produtos filtrados: " + json);

                try
                {
                    List<ProductObject> products =
                        JsonConvert.DeserializeObject<List<ProductObject>>(json);

                    if (products == null)
                    {
                        Debug.LogWarning("⚠️ Nenhum produto retornado (lista nula).");
                        PopulateProducts(new List<ProductObject>());
                    }
                    else
                    {
                        PopulateProducts(products);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError("❌ Erro ao desserializar JSON de produtos: " + ex.Message);
                }
            }
            else
            {
                Debug.LogError("❌ Erro ao buscar produtos por categoria: " + www.error);

                if (www.responseCode == 404)
                {
                    Debug.LogWarning("⚠️ Nenhum produto encontrado para essa categoria.");
                    PopulateProducts(new List<ProductObject>());
                }
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

    [System.Serializable]
    public class LeaderboardUser
    {
        public int id;
        public string name;
        public int reviewsCount;
        public float averageRating;
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
                PopulateReviews(new List<ReviewObject>());
                yield break;
            }

            string json = www.downloadHandler.text;
            List<ProductObject> products = JsonConvert.DeserializeObject<List<ProductObject>>(json);

            var filtered = products.FindAll(p => p.category != null &&
                p.category.Equals(category, System.StringComparison.OrdinalIgnoreCase));

            if (filtered.Count == 0)
            {
                Debug.LogWarning("⚠️ Nenhum produto encontrado na categoria: " + category);
                PopulateReviews(new List<ReviewObject>()); 
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

    public IEnumerator GetAllReviews()
    {
        string url = $"{baseUrl}/Reviews";

        Debug.Log("🔍 Buscando todas as reviews em: " + url);

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                Debug.Log("📦 Reviews Recebidas: " + json);

                try
                {
                    List<ReviewObject> reviews =
                        JsonConvert.DeserializeObject<List<ReviewObject>>(json);

                    if (reviews == null)
                    {
                        Debug.LogWarning("⚠️ Lista de reviews veio nula. Exibindo vazio.");
                        PopulateReviews(new List<ReviewObject>());
                    }
                    else
                    {
                        Debug.Log($"📝 Total de reviews recebidas: {reviews.Count}");
                        PopulateReviews(reviews);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError("❌ Erro ao desserializar reviews: " + ex.Message);
                    PopulateReviews(new List<ReviewObject>());
                }
            }
            else
            {
                Debug.LogError($"❌ Erro ao buscar todas as reviews: {request.responseCode} - {request.error}");
                PopulateReviews(new List<ReviewObject>());
            }
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

    public IEnumerator GetUsersWithReviews()
    {
        string url = $"{baseUrl}/Users";

        Debug.Log("🔍 Buscando todos os usuários para leaderboard em: " + url);

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                Debug.Log("📦 Usuários para leaderboard: " + json);

                List<UserDto> users = null;
                bool hasError = false;

                try
                {
                    users = JsonConvert.DeserializeObject<List<UserDto>>(json);
                }
                catch (Exception ex)
                {
                    Debug.LogError("❌ Erro ao processar usuários para leaderboard: " + ex.Message);
                    hasError = true;
                }

                if (hasError || users == null)
                {
                    Debug.LogWarning("⚠️ Lista de usuários veio nula. Exibindo leaderboard vazia.");
                    PopulateLeaderboard(new List<LeaderboardUser>());
                }
                else
                {
                    // Buscar reviews para contar por usuário
                    yield return StartCoroutine(GetReviewsCountForUsers(users));
                }
            }
            else
            {
                Debug.LogError($"❌ Erro ao buscar usuários para leaderboard: {request.responseCode} - {request.error}");
                PopulateLeaderboard(new List<LeaderboardUser>());
            }
        }
    }

    private IEnumerator GetReviewsCountForUsers(List<UserDto> users)
    {
        string reviewsUrl = $"{baseUrl}/Reviews";
        
        using (UnityWebRequest request = UnityWebRequest.Get(reviewsUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                List<ReviewObject> reviews = JsonConvert.DeserializeObject<List<ReviewObject>>(json);

                List<LeaderboardUser> leaderboardUsers = ProcessUsersWithReviewsCount(users, reviews);
                Debug.Log($"🏆 Total de usuários na leaderboard: {leaderboardUsers.Count}");
                PopulateLeaderboard(leaderboardUsers);
            }
            else
            {
                Debug.LogError($"❌ Erro ao buscar reviews: {request.responseCode} - {request.error}");
                PopulateLeaderboard(new List<LeaderboardUser>());
            }
        }
    }

    private List<LeaderboardUser> ProcessUsersWithReviewsCount(List<UserDto> users, List<ReviewObject> reviews)
    {
        Dictionary<int, int> userReviewCounts = new Dictionary<int, int>();

        // Contar reviews por usuário
        foreach (var review in reviews)
        {
            if (userReviewCounts.ContainsKey(review.userId))
                userReviewCounts[review.userId]++;
            else
                userReviewCounts[review.userId] = 1;
        }

        List<LeaderboardUser> leaderboardUsers = new List<LeaderboardUser>();

        // Criar leaderboard com todos os usuários
        foreach (var user in users)
        {
            int reviewCount = userReviewCounts.ContainsKey(user.id) ? userReviewCounts[user.id] : 0;
            
            leaderboardUsers.Add(new LeaderboardUser
            {
                id = user.id,
                name = user.name ?? "Usuário Desconhecido",
                reviewsCount = reviewCount,
                averageRating = 0f
            });
        }

        // Ordenar: mais reviews primeiro, alfabético para empates, limite de 25
        var sortedUsers = leaderboardUsers
            .OrderByDescending(u => u.reviewsCount) // Mais reviews primeiro
            .ThenBy(u => u.name) // Alfabético para empates
            .Take(25) // Limite de 25 usuários
            .ToList();

        return sortedUsers;
    }

    #endregion


    #endregion

    [Header("UI Elements")]
    public GameObject prefabProduct;
    public Transform contentParent;
    public MakeReview makeReview;
    private PlayFabManager playfab;

    private void PopulateProducts(List<ProductObject> products)
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        if (products != null && products.Count > 0)
        {
            foreach (var product in products)
            {
                GameObject item = Instantiate(prefabProduct, contentParent);

                var ui = item.GetComponent<ProductItemUI>();
                if (ui != null)
                    ui.SetData(product);

                var button = item.GetComponentInChildren<Button>();
                if (button != null)
                {
                    int capturedId = product.id;
                    string capturedName = product.name;
                    button.onClick.AddListener(() =>
                    {
                        makeReview.OpenReviewMenu(true, capturedId, playfab.GetUserId(), capturedName);
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

        LayoutRebuilder.ForceRebuildLayoutImmediate(contentParent.GetComponent<RectTransform>());
    }



    [Header("UI Elements (Reviews)")]
    public GameObject prefabReview;
    public Transform contentReviewParent;

    [Header("UI Elements (Leaderboard)")]
    public GameObject prefabLeaderboard;
    public Transform contentLeaderboardParent;

    private void PopulateReviews(List<ReviewObject> reviews)
    {
        StartCoroutine(DoPopulateReviews(reviews));
    }

    public void DeleteChildren()
    {
        foreach (Transform child in contentReviewParent)
            Destroy(child.gameObject);
    }

    private IEnumerator DoPopulateReviews(List<ReviewObject> reviews)
    {
        Debug.Log($"🧱 Limpando {contentReviewParent.childCount} reviews antigos...");
        DeleteChildren();

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

        yield return null;

        LayoutRebuilder.ForceRebuildLayoutImmediate(contentReviewParent.GetComponent<RectTransform>());
    }

    private void PopulateLeaderboard(List<LeaderboardUser> users)
    {
        StartCoroutine(DoPopulateLeaderboard(users));
    }

    public void DeleteLeaderboardChildren()
    {
        if (contentLeaderboardParent == null)
        {
            Debug.LogWarning("⚠️ contentLeaderboardParent não atribuído no APIManager!");
            return;
        }

        foreach (Transform child in contentLeaderboardParent)
            Destroy(child.gameObject);
    }

    private IEnumerator DoPopulateLeaderboard(List<LeaderboardUser> users)
    {
        Debug.Log($"🧱 Limpando {contentLeaderboardParent.childCount} itens antigos da leaderboard...");
        DeleteLeaderboardChildren();

        yield return null;

        if (users != null && users.Count > 0)
        {
            Debug.Log($"🏆 Adicionando {users.Count} usuários na leaderboard...");
            for (int i = 0; i < users.Count; i++)
            {
                GameObject item = Instantiate(prefabLeaderboard, contentLeaderboardParent);
                var ui = item.GetComponent<LeaderboardItemUI>();
                if (ui != null)
                    ui.SetData(users[i], i + 1);
            }
        }
        else
        {
            Debug.Log("🟡 Nenhum usuário encontrado — leaderboard vazia.");
        }

        yield return null;

        LayoutRebuilder.ForceRebuildLayoutImmediate(contentLeaderboardParent.GetComponent<RectTransform>());
    }

}

[System.Serializable]
public class LoginRequest
{
    public string email;
    public string password;
}
