using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class NPCController : MonoBehaviour
{
    [Header("Referências")]
    public Transform player;

    [Header("Configurações")]
    public float speed = 2f;
    public float stopDistance = 0.5f;
    public float offScreenDespawnDelay = 5f;

    private Camera mainCam;
    private float offScreenTimer = 0f;
    private bool isChasing = true;
    private bool isLeaving = false;
    private Vector3 leaveDirection;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private Vector3 targetPosition;
    private Vector3 smoothedVelocity;

    private float nextDirectionCheck = 0f;
    private const float directionInterval = 0.1f; 

    private void Awake()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
                player = p.transform;
        }

        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        mainCam = Camera.main;
        targetPosition = transform.position;
    }

    void FixedUpdate() 
    {
        if (player == null) return;

        Vector3 moveDir = Vector3.zero;

        if (isLeaving)
        {
            moveDir = leaveDirection;
            targetPosition = transform.position + moveDir * speed * Time.fixedDeltaTime;
        }
        else if (isChasing)
        {
            if (Time.time >= nextDirectionCheck)
            {
                nextDirectionCheck = Time.time + directionInterval;

                Vector3 direction = player.position - transform.position;
                float distance = direction.magnitude;

                if (distance > stopDistance)
                {
                    if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                        moveDir = new Vector3(Mathf.Sign(direction.x), 0, 0);
                    else
                        moveDir = new Vector3(0, Mathf.Sign(direction.y), 0);

                    targetPosition = transform.position + moveDir * speed * Time.fixedDeltaTime;
                }
            }
        }

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref smoothedVelocity, 0.08f);

        UpdateDirectionAnimation(targetPosition - transform.position);
        CheckOffScreen();
    }

    void CheckOffScreen()
    {
        Vector3 viewPos = mainCam.WorldToViewportPoint(transform.position);
        bool inside = viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1;

        if (!inside)
        {
            offScreenTimer += Time.deltaTime;
            if (offScreenTimer >= offScreenDespawnDelay)
                Destroy(gameObject);
        }
        else
        {
            offScreenTimer = 0f;
        }
    }

    public void RestartChase()
    {
        isChasing = true;
        isLeaving = false;
        offScreenTimer = 0f;
    }

    public void SendAway()
    {
        isChasing = false;
        isLeaving = true;

        int side = Random.Range(0, 4);
        switch (side)
        {
            case 0: leaveDirection = Vector3.up; break;
            case 1: leaveDirection = Vector3.down; break;
            case 2: leaveDirection = Vector3.left; break;
            case 3: leaveDirection = Vector3.right; break;
        }
    }

    void UpdateDirectionAnimation(Vector3 moveDir)
    {
        if (animator == null) return;

        animator.SetFloat("moveX", moveDir.x);
        animator.SetFloat("moveY", moveDir.y);

        if (moveDir.y > 0.1f)
            animator.Play("Idle_Back");
        else if (moveDir.y < -0.1f)
            animator.Play("Idle_Front");
        // else if (Mathf.Abs(moveDir.x) > 0.1f)
        // {
        //     animator.Play("Idle_Side");
        //     spriteRenderer.flipX = moveDir.x < 0;
        // }
    }
}
