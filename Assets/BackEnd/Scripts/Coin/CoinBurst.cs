using UnityEngine;

public class CoinBurst : MonoBehaviour
{
    public GameObject coinPrefab;
    public float explosionForce = 5f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            SpawnCoins(5);
        }
    }

    public void SpawnCoins(int coinAmount)
    {
        for (int i = 0; i < coinAmount; i++)
        {
            GameObject coin = Instantiate(coinPrefab, transform.position, Quaternion.identity);

            Rigidbody2D rb = coin.GetComponent<Rigidbody2D>();

            Vector2 randomDir = Random.insideUnitCircle.normalized;
            rb.AddForce(randomDir * explosionForce, ForceMode2D.Impulse);
        }
    }
}
