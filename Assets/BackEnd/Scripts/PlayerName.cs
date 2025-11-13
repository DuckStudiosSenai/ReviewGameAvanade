using TMPro;
using UnityEngine;
using Photon.Pun;

public class PlayerName : MonoBehaviourPun
{
    public TextMeshProUGUI playerNameTxt;
    private Canvas canvas;

    private void Start()
    {
        canvas = GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;

        if (photonView.IsMine)
        {
            canvas.enabled = false;
            return;
        }

        if (playerNameTxt != null)
        {
            playerNameTxt.text = photonView.Owner.NickName;
        }
    }
}
