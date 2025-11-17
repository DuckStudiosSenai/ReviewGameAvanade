using UnityEngine;

public class CloudMovement : MonoBehaviour
{
    public float speed = 1f; // velocidade da nuvem
    public float resetX = -20f; // posição onde a nuvem reaparece
    public float limitX = 20f;  // limite da tela pra direita

    void Update()
    {
        // Move a nuvem pra direita
        transform.Translate(Vector2.right * speed * Time.deltaTime);

        // Se passar do limite, volta pra esquerda
        if (transform.position.x > limitX)
        {
            transform.position = new Vector2(resetX, transform.position.y);
        }
    }
}
