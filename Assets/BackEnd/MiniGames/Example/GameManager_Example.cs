using Photon.Pun;
using UnityEngine;

public class GameManager_Example : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            FinishGame();
        }
    }

    private void FinishGame()
    {
        Debug.Log("🏁 Jogo Finalizado! Retornando ao jogo principal...");
        PhotonNetwork.LoadLevel("Game");
    }
}
