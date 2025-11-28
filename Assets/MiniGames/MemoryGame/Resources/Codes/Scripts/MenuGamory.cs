using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuGamory : MonoBehaviour
{
    public void LeaveGame()
    {
        PhotonNetwork.LoadLevel("Game");
    }
}
