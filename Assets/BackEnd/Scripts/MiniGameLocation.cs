using Photon.Pun;
using UnityEngine;

public enum MiniGameLocationType
{
    EXAMPLE_GAME
}

public class MiniGameLocation : MonoBehaviour
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
            LoadGameScene();
        }
    }

    private void LoadGameScene()
    {
        switch (locationType)
        {
            case MiniGameLocationType.EXAMPLE_GAME:
                PhotonNetwork.LoadLevel("ExampleGame");
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
}
