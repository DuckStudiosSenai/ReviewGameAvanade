using UnityEngine;

public class FoodDestroyArea : MonoBehaviour
{

    public CatchPlayer player;

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
