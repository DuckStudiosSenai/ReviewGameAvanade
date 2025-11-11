using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using Newtonsoft.Json;

public class PlayFabReceiver : MonoBehaviour
{
    [System.Serializable]
    public class LoginPayload
    {
        public string entityId;
        public string entityType;
        public string sessionTicket;
        public string userName;
    }

    public void ReceiveLoginData(string json)
    {
        Debug.Log("📥 Recebendo dados do login do site pai...");
        Debug.Log(json);

        var login = JsonConvert.DeserializeObject<LoginPayload>(json);

        PlayFabSettings.staticPlayer.PlayFabId = login.entityId;
        PlayFabSettings.staticPlayer.ClientSessionTicket = login.sessionTicket;
        PlayFabSettings.staticPlayer.EntityId = login.entityId;
        PlayFabSettings.staticPlayer.EntityType = login.entityType;

        Debug.Log("✅ Sessão configurada com sucesso!");
        Debug.Log("EntityId: " + PlayFabSettings.staticPlayer.EntityId);
        Debug.Log("SessionTicket: " + PlayFabSettings.staticPlayer.ClientSessionTicket);

        ValidateSession();
    }

    private void ValidateSession()
    {
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(),
            result =>
            {
                Debug.Log("✅ Sessão válida!");
                Debug.Log("DisplayName: " + result.AccountInfo.TitleInfo.DisplayName);

                LoadUserData();
            },
            error =>
            {
                Debug.LogError("❌ Sessão inválida: " + error.GenerateErrorReport());
            }
        );
    }

    private void LoadUserData()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
            result =>
            {
                Debug.Log("📦 Dados carregados:");
                foreach (var item in result.Data)
                    Debug.Log(item.Key + ": " + item.Value.Value);
            },
            error =>
            {
                Debug.LogError("❌ Erro ao carregar dados: " + error.GenerateErrorReport());
            }
        );
    }
}
