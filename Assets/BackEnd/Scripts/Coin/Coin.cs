using UnityEngine;

public class Coin : MonoBehaviour
{
    private Rigidbody2D rb;
    private Transform target;
    private bool returning = false;

    public float returnDelay = 0.5f;
    public float returnSpeed = 10f;
    public float absorbDistance = 0.2f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        target = GameObject.FindWithTag("Player").transform;

        Invoke(nameof(StartReturning), returnDelay);
    }

    void StartReturning()
    {
        returning = true;

        // Zera física para não atrapalhar o retorno
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0;
        rb.gravityScale = 0;

        // Opcional — faz não colidir com paredes e evitar ficar presa
        GetComponent<Collider2D>().enabled = false;
    }

    void Update()
    {
        if (returning)
        {
            // Move diretamente
            transform.position = Vector3.MoveTowards(
                transform.position,
                target.position,
                returnSpeed * Time.deltaTime
            );

            // Absorve quando chega
            if (Vector2.Distance(transform.position, target.position) < absorbDistance)
            {
                Destroy(gameObject);
            }
        }
    }
}
