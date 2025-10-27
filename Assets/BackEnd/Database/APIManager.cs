using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class APIManager : MonoBehaviour
{
    private string baseUrl = "https://seuservidor.com/api";

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
        using (UnityWebRequest www = UnityWebRequest.Get(baseUrl + "/products"))
        {
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.Success)
                Debug.Log("📦 Produtos: " + www.downloadHandler.text);
            else
                Debug.LogError("❌ Erro ao buscar produtos: " + www.error);
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

    #endregion

}

[System.Serializable]
public class LoginRequest
{
    public string email;
    public string password;
}
