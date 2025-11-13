using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using Photon.Pun;

public class AnonymousLogin : MonoBehaviour
{
    private bool isLogged = false;

    void Update()
    {
        if (!isLogged && Input.GetKeyDown(KeyCode.I))
        {
            Login();
        }
    }

    // ---------------- PLAYFAB ----------------
    void Login()
    {
        Debug.Log("🔐 Logando PlayFab...");

        var request = new LoginWithCustomIDRequest
        {
            CustomId = System.Guid.NewGuid().ToString(),
            CreateAccount = true
        };

        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginError);
    }

    void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("✅ PlayFab Login Sucesso!");
        isLogged = true;

        // (Opcional) define nickname no Photon
        PhotonNetwork.NickName = "Player_" + Random.Range(1000, 9999);

        ConectarPhoton();
    }

    void OnLoginError(PlayFabError error)
    {
        Debug.LogError("❌ PlayFab Error: " + error.GenerateErrorReport());
    }

    // ---------------- PHOTON ----------------
    void ConectarPhoton()
    {
        Debug.Log("🌐 Conectando Photon...");
        PhotonNetwork.ConnectUsingSettings();
    }
}
