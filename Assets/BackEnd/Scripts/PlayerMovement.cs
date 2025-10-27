using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    public float moveSpeed = 3f;
    public float tileSize = 1f; 

    [Header("States")]
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
    }

    void Update()
    {
        if (isTyping || isMoving) return;

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        if (Mathf.Abs(moveX) > 0)
            moveY = 0;

        input = new Vector2(moveX, moveY);

        if (input != Vector2.zero)
            StartCoroutine(Move());
    }

    System.Collections.IEnumerator Move()
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

    public void SetTypingState(bool typing)
    {
        isTyping = typing;
    }
}
