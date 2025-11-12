using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using UnityEngine.Networking;
using TMPro;
using Photon.Pun; // integração com Photon

[Serializable]
public class UserFromAPI
{
    public int id;
    public string name;
    public string email;
    public string password;
    public string enterprise;
    public int points;
    public int currentpoints;
}

public class PlayFabManager : MonoBehaviour
{
    [Header("PlayFab Config")]
    public string titleId = "17FF18"; 

    [Header("API Config")]
    public string apiBaseUrl = "https://reviewgameapi.squareweb.app"; 

    [Header("UI")]
    public TextMeshProUGUI playerNameText; 

    private string cachedUserName;
    private GameManager gm;

    private void Awake()
    {
        if (!string.IsNullOrEmpty(titleId))
            PlayFabSettings.staticSettings.TitleId = titleId;
    }

    void Start()
    {
        gm = GetComponent<GameManager>();
    }

    public void OnSiteAuth(string userIdString)
    {
        Debug.Log("[PlayFabManager] Recebido ID: " + userIdString);

        if (int.TryParse(userIdString, out int userId))
        {
            StartCoroutine(GetUserAndLogin(userId));
        }
        else
        {
            Debug.LogError("❌ ID inválido recebido: " + userIdString);
            UpdateUI("❌ ID inválido!");
        }
    }

    IEnumerator GetUserAndLogin(int userId)
    {
        string url = $"{apiBaseUrl}/api/Users/{userId}";
        Debug.Log("🔍 Buscando usuário no banco: " + url);
        UpdateUI("🔍 Buscando usuário...");

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("❌ Erro ao buscar usuário: " + www.error);
                UpdateUI("❌ Erro ao buscar usuário!");
                yield break;
            }

            string json = www.downloadHandler.text;
            Debug.Log("📦 Resposta da API: " + json);

            UserFromAPI user = JsonUtility.FromJson<UserFromAPI>(json);
            if (user == null || string.IsNullOrEmpty(user.email))
            {
                Debug.LogError("⚠️ Usuário não encontrado ou sem senha válida.");
                UpdateUI("⚠️ Usuário inválido!");
                yield break;
            }

            cachedUserName = user.name;
            Debug.Log($"👤 Usuário encontrado: {cachedUserName} (ID: {user.id})");
            UpdateUI("👤 Conectando " + cachedUserName + "...");

            LoginOrRegister(user.id.ToString(), user.email);
        }
    }

    void LoginOrRegister(string username, string password)
    {
        var loginRequest = new LoginWithPlayFabRequest
        {
            Username = "userId" + username,
            Password = password
        };

        PlayFabClientAPI.LoginWithPlayFab(loginRequest,
            result =>
            {
                Debug.Log($"✅ Login PlayFab bem-sucedido! PlayFabId: {result.PlayFabId}");
                PhotonNetwork.NickName = cachedUserName;
                UpdateUI("✅ Bem-vindo, " + cachedUserName + "!");
                StartCoroutine(ActivateGameManagerDelayed());
            },
            error =>
            {
                Debug.LogWarning($"⚠️ Login falhou ({error.ErrorMessage}), tentando criar conta...");
                UpdateUI("⚙️ Criando conta no PlayFab...");

                var registerRequest = new RegisterPlayFabUserRequest
                {
                    Username = "userId" + username,
                    Password = password,
                    RequireBothUsernameAndEmail = false
                };

                PlayFabClientAPI.RegisterPlayFabUser(registerRequest,
                    registerResult =>
                    {
                        Debug.Log($"🟢 Conta criada com sucesso! PlayFabId: {registerResult.PlayFabId}");
                        PhotonNetwork.NickName = cachedUserName;
                        UpdateUI("🟢 Conta criada para " + cachedUserName + "!");
                        StartCoroutine(ActivateGameManagerDelayed());
                    },
                    registerError =>
                    {
                        Debug.LogError("❌ Falha ao criar conta: " + registerError.GenerateErrorReport());
                        UpdateUI("❌ Falha ao criar conta!");
                    }
                );
            }
        );
    }

    IEnumerator ActivateGameManagerDelayed()
    {
        yield return new WaitForSeconds(1f);

        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("🔌 Conectando ao Photon...");
            PhotonNetwork.ConnectUsingSettings();
        }

        yield return new WaitUntil(() => PhotonNetwork.IsConnectedAndReady);

        Debug.Log("✅ Photon conectado. Entrando na sala...");
        gm.TryJoinOrCreateRoom();
    }


    void UpdateUI(string message)
    {
        if (playerNameText != null)
            playerNameText.text = message;
        else
            Debug.Log("ℹ️ " + message);
    }
}
