using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviourPun, IPunObservable
{
    [Header("Movimentação")]
    public float moveSpeed = 3f;
    public float tileSize = 1f;
    public bool isMoving = false;
    public bool isTyping = false;

    private Vector2 startPos;
    private Vector2 endPos;
    private float moveProgress = 0f;
    private Vector2 input;
    private Vector2 networkPosition;

    void Start()
    {
        if (!photonView.IsMine)
            return;

        // 🧠 evita duplicação de player local
        if (PhotonNetwork.LocalPlayer.TagObject != null &&
            PhotonNetwork.LocalPlayer.TagObject != gameObject)
        {
            Debug.LogWarning("⚠️ Player duplicado detectado, destruindo o antigo.");
            PhotonNetwork.Destroy(gameObject);
            return;
        }

        PhotonNetwork.LocalPlayer.TagObject = gameObject;
    }

    void Update()
    {
        // Jogadores remotos só interpolam
        if (!photonView.IsMine)
        {
            transform.position = Vector2.Lerp(transform.position, networkPosition, Time.deltaTime * 10f);
            return;
        }

        if (isTyping)
            return;

        if (isMoving)
        {
            moveProgress += Time.deltaTime * moveSpeed;
            transform.position = Vector2.Lerp(startPos, endPos, moveProgress);

            if (moveProgress >= 1f)
            {
                transform.position = endPos;
                isMoving = false;
            }
            return;
        }

        float moveX = (Input.GetKey(KeyCode.A) ? -1 : Input.GetKey(KeyCode.D) ? 1 : 0);
        float moveY = (Input.GetKey(KeyCode.S) ? -1 : Input.GetKey(KeyCode.W) ? 1 : 0);

        if (Mathf.Abs(moveX) > 0) moveY = 0;
        input = new Vector2(moveX, moveY);

        if (input != Vector2.zero)
        {
            startPos = transform.position;
            endPos = startPos + input * tileSize;
            moveProgress = 0f;
            isMoving = true;
        }
    }

    public void SetTypingState(bool typing)
    {
        isTyping = typing;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
        }
        else
        {
            networkPosition = (Vector2)stream.ReceiveNext();
        }
    }

    void OnDisable()
    {
        if (PhotonNetwork.LocalPlayer != null && PhotonNetwork.LocalPlayer.TagObject == gameObject)
            PhotonNetwork.LocalPlayer.TagObject = null;
    }
}
