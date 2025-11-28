using UnityEngine;

public class FoodDestroyArea : MonoBehaviour
{

    public Player player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Food"))
        {
            Destroy(collision.gameObject);
            player.DepleteLife();
            Debug.Log("Food Destroyed");
        }
    }
}
