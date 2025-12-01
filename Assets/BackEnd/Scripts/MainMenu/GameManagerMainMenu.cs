using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class GameManagerMainMenu : MonoBehaviourPunCallbacks
{
    private string roomName = "SalaPrincipal";

    public override void OnConnectedToMaster()
    {
        Debug.Log("🌐 Conectado ao Master Server.");
        PhotonNetwork.JoinLobby();
        
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
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("🔌 Desconectado do Photon!");
    }

}
