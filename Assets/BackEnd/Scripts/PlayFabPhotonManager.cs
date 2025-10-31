using PlayFab;
using PlayFab.ClientModels;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayFabPhotonManager : MonoBehaviourPunCallbacks
{
    [Header("Login")]
    [SerializeField] private string username;
    public LoginUI loginUi;
    public GameObject loginObj;
    

    private Dictionary<string, GameObject> roomButtons = new Dictionary<string, GameObject>();

    private static readonly Dictionary<PlayFabErrorCode, string> errorsTranslated = new()
    {
        { PlayFabErrorCode.InvalidEmailAddress, "Endereço de e-mail inválido." },
        { PlayFabErrorCode.AccountNotFound, "Conta não encontrada. Registre-se primeiro!" },
        { PlayFabErrorCode.InvalidUsernameOrPassword, "Usuário ou senha incorretos." },
        { PlayFabErrorCode.EmailAddressNotAvailable, "Este e-mail já está em uso." },
        { PlayFabErrorCode.InvalidParams, "Há parâmetros inválidos no envio." },
        { PlayFabErrorCode.ServiceUnavailable, "Serviço temporariamente indisponível. Tente mais tarde." },
        { PlayFabErrorCode.ConnectionError, "Não foi possível conectar ao servidor. Verifique sua internet." },
        { PlayFabErrorCode.AccountAlreadyExists, "Este usuário já foi registrado." },
        { PlayFabErrorCode.InvalidUsername, "Nome de usuário inválido. Use apenas letras e números." },
        { PlayFabErrorCode.UsernameNotAvailable, "Usuário já registrado." },
        { PlayFabErrorCode.APIClientRequestRateLimitExceeded, "Muitos usuários tentaram logar ao mesmo tempo. Aguarde um pouco." },

    };

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
    }

    private void Start()
    {
        PhotonNetwork.SendRate = 60;

        PhotonNetwork.SerializationRate = 30;

        if (string.IsNullOrEmpty(PlayFabSettings.TitleId))
            PlayFabSettings.TitleId = "17FF18";


        username = PlayerPrefs.GetString("USERNAME", "");
        
    }

    private float timeToKick = 10;
    private float elapsedTimeToKick = 0;
    private void Update()
    {
        if (PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.GetPing() > 500)
            {
                elapsedTimeToKick += Time.deltaTime;
                if (elapsedTimeToKick >= timeToKick)
                {
                    Debug.LogWarning("❌ Alto ping detectado. Desconectando...");
                    LogOut();
                }
            }
            else elapsedTimeToKick = 0;
        }
    }

    public static string Translate(PlayFabError error)
    {
        if (errorsTranslated.TryGetValue(error.Error, out string mensagem))
            return mensagem;

        return error.ErrorMessage;
    }

    #region === LOGIN PLAYFAB ===
    public void SetUsername(string name)
    {
        username = name;
        PlayerPrefs.SetString("USERNAME", username);
        PlayerPrefs.Save();
    }

    public void Login(string username, string password)
    {
        var request = new LoginWithPlayFabRequest
        {
            Username = username,
            Password = password,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
        };

        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnFailure);
    }

    public void LogOut()
    {
        PhotonNetwork.Disconnect();
        PlayFabClientAPI.ForgetAllCredentials();   
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("❌ Desconectado do Photon: " + cause.ToString());

        PhotonNetwork.LoadLevel("Login");
    }

    public void Register(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            Debug.LogError("❌ Usuário ou senha inválidos!");
            return;
        }

        if (password.Length < 6)
        {
            Debug.LogError("❌ A senha precisa ter pelo menos 6 caracteres!");
            return;
        }

        var request = new RegisterPlayFabUserRequest
        {
            Username = username,
            Password = password,
            RequireBothUsernameAndEmail = false
        };

        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFailure);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("✅ PlayFab login: " + username + " (PlayFabId: " + result.PlayFabId + ")");

        if (string.IsNullOrEmpty(result.InfoResultPayload?.PlayerProfile?.DisplayName) ||
            result.InfoResultPayload.PlayerProfile.DisplayName != username)
        {
            if (string.IsNullOrEmpty(result.InfoResultPayload?.PlayerProfile?.DisplayName))
            {
                UpdateDisplayName(username);
            }
        }

        PhotonNetwork.AuthValues = new AuthenticationValues(result.PlayFabId);
        PhotonNetwork.NickName = username;

        loginObj.SetActive(false);
        PhotonNetwork.LoadLevel("Game");
    }

    private void OnRegisterFailure(PlayFabError error)
    {
        Debug.LogError("❌ Erro no registro: " + error.GenerateErrorReport());
        FindFirstObjectByType<LoginUI>()?.ShowErrorReg(Translate(error));
        Debug.Log(error.ToString());

    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("✅ Usuário registrado com sucesso!");
        FindFirstObjectByType<LoginUI>()?.ShowSuccessReg("Usuário registrado!");
    }

    private void UpdateDisplayName(string username)
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = username
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(request,
            result => Debug.Log("✅ DisplayName atualizado: " + result.DisplayName),
            error => Debug.LogError("❌ Falha ao atualizar DisplayName: " + error.GenerateErrorReport()));
    }

    private void OnFailure(PlayFabError error)
    {
        Debug.LogError("❌ Erro PlayFab: " + error.GenerateErrorReport());
        loginUi.ShowError(Translate(error));
        Debug.Log(error.ToString());
    }
    #endregion

    #region === PHOTON ===
    public override void OnConnectedToMaster()
    {
        Debug.Log("✅ Conectado ao Photon Master");
        if (!PhotonNetwork.InLobby)
            PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("✅ Entrou no Lobby, aguardando lista de salas...");
    }

    

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"❌ Falha ao entrar na sala: {message}");
    }
    #endregion
}
