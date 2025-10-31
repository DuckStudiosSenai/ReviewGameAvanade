using UnityEngine;
using Photon.Pun;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviourPun, IPunObservable
{
    [Header("Movimentação")]
    public float moveSpeed = 3f;
    public float tileSize = 1f;
    public bool isMoving = false;
    public bool isTyping = false;

    private Rigidbody2D rb;
    private Vector2 input;
    private Vector2 startPos;
    private Vector2 endPos;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;

        // 🔒 Desativa controle nos jogadores remotos
        if (!photonView.IsMine)
        {
            enabled = false;
            return;
        }

        // 🧠 Protege caso a instância do player tenha vindo duplicada
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
        if (!photonView.IsMine || isTyping || isMoving)
            return;

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        if (Mathf.Abs(moveX) > 0) moveY = 0; // impede diagonais
        input = new Vector2(moveX, moveY);

        if (input != Vector2.zero)
            StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        isMoving = true;
        startPos = rb.position;
        endPos = startPos + input * tileSize;

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed;
            rb.MovePosition(Vector2.Lerp(startPos, endPos, t));
            yield return null;
        }

        rb.MovePosition(endPos);
        isMoving = false;
    }

    public void SetTypingState(bool typing) => isTyping = typing;

    // 🔄 Sincronização de posição entre jogadores
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (!this || rb == null || photonView == null)
            return; // proteção contra sincronização prematura

        try
        {
            if (stream.IsWriting)
            {
                stream.SendNext(rb.position);
            }
            else
            {
                Vector2 targetPos = (Vector2)stream.ReceiveNext();
                rb.position = Vector2.Lerp(rb.position, targetPos, Time.deltaTime * 10);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"[SerializeView] Exceção ignorada: {e.Message}");
        }
    }

    void OnDisable()
    {
        // limpa referência ao sair ou ser destruído
        if (PhotonNetwork.LocalPlayer != null && PhotonNetwork.LocalPlayer.TagObject == gameObject)
            PhotonNetwork.LocalPlayer.TagObject = null;
    }
}
