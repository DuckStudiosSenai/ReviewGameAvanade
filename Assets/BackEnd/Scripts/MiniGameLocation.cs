using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum MiniGameLocationType
{
    EXAMPLE_GAME
}

public class MiniGameLocation : MonoBehaviourPunCallbacks
{
    private bool isAbleToEnter = false;
    private PhotonView localPlayerPV;
    public MiniGameLocationType locationType;

    private void Update()
    {
        if (!isAbleToEnter || localPlayerPV == null || !localPlayerPV.IsMine)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            SavePlayerLocation();
            GoToPrivateScene();
        }
    }

    private void LoadGameScene()
    {
        switch (locationType)
        {
            case MiniGameLocationType.EXAMPLE_GAME:
                SceneManager.LoadScene("ExampleGame");
                break;

            default:
                Debug.LogError("MiniGameLocation: Tipo de mini-jogo desconhecido!");
                break;
        }
    }

    private void SavePlayerLocation()
    {
        if (localPlayerPV == null)
            return;

        Vector2 pos = localPlayerPV.transform.position;

        PlayerPrefs.SetFloat("px", pos.x);
        PlayerPrefs.SetFloat("py", pos.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        PhotonView pv = collision.GetComponent<PhotonView>();

        if (pv != null && pv.IsMine)
        {
            isAbleToEnter = true;
            localPlayerPV = pv;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        PhotonView pv = collision.GetComponent<PhotonView>();

        if (pv != null && pv.IsMine)
        {
            isAbleToEnter = false;
            localPlayerPV = null;
        }
    }

    public void GoToPrivateScene()
    {
        Debug.Log("➡️ Indo para o mini-jogo privado...");
        PhotonNetwork.Disconnect();
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LocalPlayer.TagObject = null;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("🔌 Desconectado do Photon. Carregando cena do mini-jogo...");
        LoadGameScene();
    }
}
