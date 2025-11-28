using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class RandomFood : MonoBehaviour
{
    public List<Food> foods;
    public Collider2D area;
    public static int foodEaten = 0;


    private Coroutine foodRoutine;

    public Food GetRandomFood()
    {
        if (foods == null || foods.Count == 0)
        {
            Debug.LogWarning("A lista está vazia ou não existe.");
            return null;
        }
        int index = Random.Range(0, foods.Count);
        return foods[index];
    }

    public void SpawnFood()
    {
        Food randomFood = GetRandomFood();
        if (randomFood == null) return;
        Vector2 spawnPosition = new Vector2(
            Random.Range(area.bounds.min.x, area.bounds.max.x),
            area.bounds.max.y
        );

        GameObject foodInstance = Instantiate(randomFood.foodPrefab, spawnPosition, Quaternion.identity);

        Destroy(foodInstance, GameManager_CatchGame.delayToDestroyFood);

        if (foodInstance.tag == "Trash") return;     
    }

    

    public IEnumerator StartGeneratingFood()
    {
        while (true)
        {
            yield return new WaitForSeconds(GameManager_CatchGame.delayToSpawnFood);
            SpawnFood();
        }
    }


    public void StartFoodGeneration()
    {
        if (foodRoutine == null)
        {
            foodRoutine = StartCoroutine(StartGeneratingFood());
        }
    }

    public void StopFoodGeneration()
    {
        if (foodRoutine != null)
        {
            StopCoroutine(foodRoutine);
            foodRoutine = null;
        }
    }

}
