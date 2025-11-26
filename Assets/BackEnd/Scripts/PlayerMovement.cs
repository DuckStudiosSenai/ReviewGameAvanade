using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviourPun, IPunObservable
{
    public float moveSpeed = 3f;
    public float tileSize = 1f;
    public bool isTyping = false;
    public bool movementEnabled = true;

    private bool isMoving = false;
    private Vector2 startPos;
    private Vector2 endPos;
    private float moveProgress = 0f;
    private Vector2 input;

    private Rigidbody2D rb;
    private Vector2 networkPosition;
    private bool hasInitialPosition = false;

    private bool blockInterpolation = false;
    private float interpBlockTimer = 0f;

    public Animator bodyAnimator;

    [Header("Audio")]
    public AudioSource stepsAudio;
    public AudioClip[] footstepClips;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (!bodyAnimator)
            Debug.LogError("PlayerMovement: bodyAnimator não foi atribuído no Inspector!");
    }

    void Start()
    {
        rb.gravityScale = 0;
        rb.freezeRotation = true;

        rb.interpolation = RigidbodyInterpolation2D.None;

        if (photonView.IsMine)
            photonView.RPC(nameof(SetInitialNetworkPosition), RpcTarget.OthersBuffered, rb.position);
    }

    void Update()
    {
        HandleInterpolationBlock();

        if (!photonView.IsMine)
        {
            if (!hasInitialPosition) return;

            if (blockInterpolation)
            {
                rb.position = networkPosition;
                return;
            }

            rb.position = Vector2.Lerp(rb.position, networkPosition, Time.deltaTime * 6f);
            return;
        }

        if (isTyping || !movementEnabled)
            return;

        HandleLocalMovement();
        HandleAnimations();
    }

    private void HandleLocalMovement()
    {
        if (isMoving)
        {
            moveProgress += Time.deltaTime * moveSpeed;
            rb.MovePosition(Vector2.Lerp(startPos, endPos, moveProgress));

            if (moveProgress >= 1f)
            {
                rb.MovePosition(endPos);
                isMoving = false;
            }
            return;
        }

        float mx = (Input.GetKey(KeyCode.A) ? -1 : Input.GetKey(KeyCode.D) ? 1 : 0);
        float my = (Input.GetKey(KeyCode.S) ? -1 : Input.GetKey(KeyCode.W) ? 1 : 0);

        if (Mathf.Abs(mx) > 0) my = 0;

        input = new Vector2(mx, my);
        if (input == Vector2.zero) return;

        Vector2 target = rb.position + input * tileSize;

        if (!Physics2D.OverlapCircle(target, 0.2f, LayerMask.GetMask("Default")))
        {
            startPos = rb.position;
            endPos = target;
            moveProgress = 0f;
            isMoving = true;
        }
    }

    // ======================================================
    // SINCRONIZAÇÃO DE ANIMAÇÕES (corpo é o MASTER)
    // ======================================================

    private void TriggerAnimation(string triggerName)
    {
        bodyAnimator.SetTrigger(triggerName);

        if (photonView.IsMine)
            photonView.RPC(nameof(RPC_TriggerAnimation), RpcTarget.Others, triggerName);
    }

    [PunRPC]
    private void RPC_TriggerAnimation(string triggerName)
    {
        bodyAnimator.SetTrigger(triggerName);
    }

    private void HandleAnimations()
    {
        if (!bodyAnimator) return;

        AnimatorStateInfo state = bodyAnimator.GetCurrentAnimatorStateInfo(0);

        // ---------- IDLE ----------
        if (input == Vector2.zero && !isMoving)
        {
            if (state.IsName("WalkUp"))
                TriggerAnimation("IdleUp");
            else if (state.IsName("WalkDown"))
                TriggerAnimation("IdleDown");
            else if (state.IsName("WalkLeft"))
                TriggerAnimation("IdleLeft");
            else if (state.IsName("WalkRight"))
                TriggerAnimation("IdleRight");

            return;
        }

        // ---------- WALK ----------
        if (input.y > 0 && !state.IsName("WalkUp"))
            TriggerAnimation("WalkUp");

        else if (input.y < 0 && !state.IsName("WalkDown"))
            TriggerAnimation("WalkDown");

        else if (input.x > 0 && !state.IsName("WalkRight"))
            TriggerAnimation("WalkRight");

        else if (input.x < 0 && !state.IsName("WalkLeft"))
            TriggerAnimation("WalkLeft");
    }

    // (todo o resto permanece igual)
    // INTERPOLAÇÃO, SERIALIZAÇÃO, TELEPORT, MOVIMENTO ON/OFF...

    private void HandleInterpolationBlock()
    {
        if (!blockInterpolation) return;

        interpBlockTimer -= Time.deltaTime;
        if (interpBlockTimer <= 0f)
            blockInterpolation = false;
    }

    public void TeleportTo(Vector2 newPos)
    {
        ResetMovementState(newPos);
        rb.position = newPos;
        transform.position = newPos;
    }

    private void ResetMovementState(Vector2 pos)
    {
        isMoving = false;
        moveProgress = 0f;
        input = Vector2.zero;

        startPos = pos;
        endPos = pos;
    }

    [PunRPC]
    public void RPC_Teleport(Vector3 newPos)
    {
        Vector2 pos = new Vector2(newPos.x, newPos.y);

        if (!hasInitialPosition)
        {
            rb.position = pos;
            networkPosition = pos;
            hasInitialPosition = true;
        }

        ResetMovementState(pos);

        rb.position = pos;
        transform.position = pos;
        networkPosition = pos;

        var npcs = FindObjectsByType<NPCController>(FindObjectsSortMode.None);
        foreach (var npc in npcs)
            if (npc.player != null && npc.player == this.transform)
                npc.TeleportAndContinueChase(new Vector2(pos.x - 5f, pos.y));

        blockInterpolation = true;
        interpBlockTimer = 0.2f;

        PhotonNetwork.SendAllOutgoingCommands();
    }

    [PunRPC]
    void SetInitialNetworkPosition(Vector2 pos)
    {
        rb.position = pos;
        networkPosition = pos;
        hasInitialPosition = true;
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

            if (!hasInitialPosition)
            {
                rb.position = networkPosition;
                transform.position = networkPosition;
            }

            hasInitialPosition = true;
        }
    }

    public void DisableMovement()
    {
        movementEnabled = false;
        isMoving = false;
        moveProgress = 0f;
        input = Vector2.zero;
    }

    public void EnableMovement()
    {
        movementEnabled = true;
    }

    public void PlayFootstep()
    {
        if (!photonView.IsMine) return;
        if (footstepClips.Length == 0) return;
        if (!movementEnabled || !isMoving) return;

        int index = Random.Range(0, footstepClips.Length);
        stepsAudio.PlayOneShot(footstepClips[index]);
    }

}
