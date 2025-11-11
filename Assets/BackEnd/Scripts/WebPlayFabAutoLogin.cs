using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using Photon.Pun;
using Newtonsoft.Json;

public class WebPlayFabAutoLogin : MonoBehaviourPunCallbacks
{
    [System.Serializable]
    public class WebPlayFabData
    {
        public string playFabId;
        public string sessionTicket;
        public string entityId;
        public string entityType;
        public string displayName;
    }

    private WebPlayFabData data;

    private void Start()
    {
        Debug.Log("⏳ Aguardando dados do site...");
    }

    /// ✅ Recebido via postMessage do WebGL
    public void ReceivePlayFabSession(string json)
    {
        Debug.Log("📥 Dados PlayFab recebidos WebGL: " + json);

        data = JsonConvert.DeserializeObject<WebPlayFabData>(json);

        // ✅ Configurar sessão PlayFab (sem autenticação)
        ConfigurePlayFabSession();
    }

    private void ConfigurePlayFabSession()
    {
        Debug.Log("🔐 Configurando sessão PlayFab...");

        // Essas propriedades EXISTEM na sua versão do SDK
        PlayFabSettings.staticPlayer.PlayFabId = data.playFabId;
        PlayFabSettings.staticPlayer.ClientSessionTicket = data.sessionTicket;
        PlayFabSettings.staticPlayer.EntityId = data.entityId;
        PlayFabSettings.staticPlayer.EntityType = data.entityType;

        ValidatePlayFabSession();
    }

    private void ValidatePlayFabSession()
    {
        Debug.Log("✅ Validando sessão PlayFab...");

        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(),
        result =>
        {
            Debug.Log("✅ Sessão válida!");
            Debug.Log("Jogador: " + result.AccountInfo.TitleInfo.DisplayName);

            ConnectToPhoton(result.AccountInfo.TitleInfo.DisplayName);
        },
        error =>
        {
            Debug.LogError("❌ Sessão inválida: " + error.GenerateErrorReport());
        });
    }

    private void ConnectToPhoton(string nick)
    {
        Debug.Log("🚀 Conectando ao Photon...");
        PhotonNetwork.AuthValues = new Photon.Realtime.AuthenticationValues(data.playFabId);
        PhotonNetwork.NickName = nick;

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("✅ Conectado ao Photon Master — Carregando cena...");
        PhotonNetwork.LoadLevel("Game");
    }
}
