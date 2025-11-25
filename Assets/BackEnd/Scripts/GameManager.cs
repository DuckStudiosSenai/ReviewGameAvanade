using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("Player")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform playerSpawnPos;

    public GameObject secretaryUi;
    public GameObject loadingMenu;

    private string roomName = "SalaPrincipal";

    private const string FIRST_TIME_KEY = "FirstTimePlayed";

    public override void OnConnectedToMaster()
    {
        Debug.Log("🌐 Conectado ao MasterServer!");
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
            CleanupCacheOnLeave = false,
            PlayerTtl = -1,
            EmptyRoomTtl = 0
        };

        PhotonNetwork.CreateRoom(roomName, options, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"🎮 Entrou na sala: {PhotonNetwork.CurrentRoom.Name}");

        // CHECK DE PRIMEIRA VEZ
        bool isFirstTime = true; //CheckFirstTime();
        if (isFirstTime)
        {
            Debug.Log("✨ PRIMEIRA VEZ DO JOGADOR!");
            
            playerSpawnPos = GameObject.Find("FirstTimeSpawnPos").transform;
            secretaryUi.SetActive(true);
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

            PhotonNetwork.LocalPlayer.TagObject = player;

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


    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        PhotonNetwork.DestroyPlayerObjects(otherPlayer);
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);
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
}
