using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerMinigameManager : MonoBehaviourPunCallbacks
{
    public MiniGameLocation minigameLocation;

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("🔌 Desconectado do Photon. Carregando cena do mini-jogo...");
        minigameLocation.LoadGameScene();
    }
}
