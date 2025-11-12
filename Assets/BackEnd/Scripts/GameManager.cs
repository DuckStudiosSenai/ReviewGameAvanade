using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("Player")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform playerSpawnPos;

    private string roomName = "SalaPrincipal";

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

        if (PhotonNetwork.LocalPlayer.TagObject == null)
        {
            GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, playerSpawnPos.position, Quaternion.identity);
            PhotonNetwork.LocalPlayer.TagObject = player;

            Debug.Log($"[GameManager] Player instanciado: {PhotonNetwork.NickName}, dono: {player.GetComponent<PhotonView>().Owner.NickName}");

            TextMeshPro nameTag = player.GetComponentInChildren<TextMeshPro>();
            if (nameTag != null)
            {
                nameTag.text = PhotonNetwork.NickName;
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
}
