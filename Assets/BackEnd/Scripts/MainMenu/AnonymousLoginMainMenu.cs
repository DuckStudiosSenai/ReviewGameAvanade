using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using Photon.Pun;

public class AnonymousLoginMainMenu : MonoBehaviour
{
    private bool isLogged = false;
    private PlayFabMainMenu playFab;

    private void Awake()
    {
        playFab = FindAnyObjectByType<PlayFabMainMenu>();
        playFab.userId = 1;
    }

    void Update()
    {
        if (!isLogged && Input.GetKeyDown(KeyCode.I))
        {
            Login();
        }
    }

    void Login()
    {
        Debug.Log("🔐 Logando PlayFab...");

        var request = new LoginWithCustomIDRequest
        {
            CustomId = "1",

            CreateAccount = false
        };
        PlayerPrefs.SetInt("UserId", 1);

        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginError);
    }

    void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("✅ PlayFab Login Sucesso!");
        isLogged = true;

        PhotonNetwork.NickName = "Player_" + Random.Range(1000, 9999);

        ConectarPhoton();
    }

    void OnLoginError(PlayFabError error)
    {
        Debug.LogError("❌ PlayFab Error: " + error.GenerateErrorReport());
    }

    void ConectarPhoton()
    {
        Debug.Log("🌐 Conectando Photon...");
        PhotonNetwork.ConnectUsingSettings();
    }
}
