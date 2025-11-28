using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("Player")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform playerSpawnPos;

    public GameObject secretaryUi;
    public GameObject loadingMenu;

    private string roomName = "SalaPrincipal";

    private const string FIRST_TIME_KEY = "FirstTimePlayed";

    private PointsManager pointsManager;

    private void Start()
    {
        pointsManager = FindAnyObjectByType<PointsManager>();

        if (PlayerPrefs.HasKey("px"))
        {
            PlayerLastLocation();
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("🌐 Voltou ao master. Tentando ReconnectAndRejoin...");

        if (PhotonNetwork.ReconnectAndRejoin())
        {
            Debug.Log("🔄 Tentando voltar para a sala anterior...");
        }
        else
        {
            Debug.Log("⚠️ Não conseguiu ReconnectAndRejoin. Entrando no fluxo normal...");
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("✅ Entrou no Lobby com sucesso!");
        TryJoinOrCreateRoom();
    }


    public void TryJoinOrCreateRoom()
    {
        Debug.Log("🏠 Tentando entrar em sala existente...");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("❌ Nenhuma sala encontrada. Criando nova...");

        RoomOptions options = new RoomOptions
        {
            MaxPlayers = 10,
            IsVisible = true,
            IsOpen = true,
            CleanupCacheOnLeave = true,
            EmptyRoomTtl = 0
        };

        PhotonNetwork.CreateRoom(roomName, options, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"🎮 Entrou na sala: {PhotonNetwork.CurrentRoom.Name}");

        bool isFirstTime = true; //CheckFirstTime();
        if (isFirstTime)
        {
            Debug.Log("✨ PRIMEIRA VEZ DO JOGADOR!");
            
            if (!PlayerPrefs.HasKey("px"))
            {
                playerSpawnPos = GameObject.Find("FirstTimeSpawnPos").transform;
                secretaryUi.SetActive(true);
            }
            
        }
        else
        {
            Debug.Log("🔁 Jogador retornando ao jogo.");
        }

        if (PhotonNetwork.LocalPlayer.TagObject == null)
        {
            DisableLoadingMenu();
            GameObject player = PhotonNetwork.Instantiate(
                playerPrefab.name,
                playerSpawnPos.position,
                Quaternion.identity
            );

            Debug.Log($"[GameManager] Player instanciado: {PhotonNetwork.NickName}, dono: {player.GetComponent<PhotonView>().Owner.NickName}");

            TextMeshPro nameTag = player.GetComponentInChildren<TextMeshPro>();
            if (nameTag != null)
            {
                nameTag.text = PhotonNetwork.NickName;
            }

            if (isFirstTime)
            {
                Animator playerAnim = player.GetComponent<Animator>();
                if (playerAnim != null)
                {
                    playerAnim.SetTrigger("IdleUp");
                }
            }
        }
        else
        {
            Debug.LogWarning("⚠️ O jogador já possui um objeto instanciado. Ignorando duplicata.");
        }

        pointsManager.LoadPoints();

        PlayerPrefs.DeleteKey("px");
        PlayerPrefs.DeleteKey("py");
    }

    public void DisableLoadingMenu()
    {
        loadingMenu.SetActive(false);
    }

    private bool CheckFirstTime()
    {
        if (!PlayerPrefs.HasKey(FIRST_TIME_KEY))
        {
            PlayerPrefs.SetInt(FIRST_TIME_KEY, 1);
            PlayerPrefs.Save();  
            return true;        
        }

        return false;  
    }

    private void AlreadyInRoom()
    {
        secretaryUi.SetActive(false);
    }

    private void PlayerLastLocation()
    {
        playerSpawnPos.position = new Vector2(
            PlayerPrefs.GetFloat("px"),
            PlayerPrefs.GetFloat("py")
        );
        AlreadyInRoom();
    }
}
