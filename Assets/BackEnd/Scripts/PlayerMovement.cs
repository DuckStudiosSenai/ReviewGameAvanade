using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviourPun, IPunObservable
{
    [Header("Movimentação")]
    public float moveSpeed = 3f;
    public float tileSize = 1f;
    public bool isMoving = false;
    public bool isTyping = false;

    [Header("Configuração de Física")]
    public LayerMask collisionMask;  // Camadas sólidas (ex: "Default", "Map", etc.)
    public float checkRadius = 0.2f; // Raio para checar colisão antes de mover

    private Rigidbody2D rb;
    private Vector2 startPos;
    private Vector2 endPos;
    private float moveProgress = 0f;
    private Vector2 input;
    private Vector2 networkPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

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
        if (!photonView.IsMine)
        {
            rb.position = Vector2.Lerp(rb.position, networkPosition, Time.deltaTime * 10f);
            return;
        }

        if (isTyping)
            return;

        if (isMoving)
        {
            moveProgress += Time.deltaTime * moveSpeed;
            Vector2 newPos = Vector2.Lerp(startPos, endPos, moveProgress);
            rb.MovePosition(newPos);

            if (moveProgress >= 1f)
            {
                rb.MovePosition(endPos);
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
            Vector2 desiredEnd = rb.position + input * tileSize;

            bool blocked = Physics2D.OverlapCircle(desiredEnd, checkRadius, collisionMask);

            if (!blocked)
            {
                startPos = rb.position;
                endPos = desiredEnd;
                moveProgress = 0f;
                isMoving = true;
            }
            else
            {
                Debug.Log("🚫 Movimento bloqueado por colisão em " + desiredEnd);
            }
        }
    }

    public void SetTypingState(bool typing)
    {
        isTyping = typing;
    }

    public void TeleportTo(Vector2 newPos)
    {
        isMoving = false;                
        moveProgress = 0f;
        startPos = newPos;              
        endPos = newPos;
        rb.position = newPos;          
        transform.position = newPos;    

        NPCController npc = FindAnyObjectByType<NPCController>();

        if (npc != null)
            npc.TeleportAndContinueChase(new Vector2(newPos.x - 5, newPos.y));
        
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(rb.position);
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

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying && photonView.IsMine)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(endPos, checkRadius);
        }
    }
#endif
}
