using UnityEngine;

public class FoodSpin : MonoBehaviour
{
    private void Update()
    {
        transform.Rotate(0f, 0f, 90f * -Time.deltaTime);
    }
}
