using UnityEngine;

public class CoinBurst : MonoBehaviour
{
    public GameObject coinPrefab;
    public int coinAmount = 10;
    public float explosionForce = 5f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            SpawnCoins();
        }
    }

    public void SpawnCoins()
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
