using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager_CatchGame : MonoBehaviour
{

    public static float delayToSpawnFood = 2f;
    public static float delayToDestroyFood = 5f;
    public static float lives = 100f;
    public static int score = 0;

    public TextMeshProUGUI timeText;
    public Slider lifeSlider;

    private float timeElapsed = 0f;
    private string formattedTime;

    public RandomFood randomFood;

    public Player player;

    private void Update()
    {
        timeElapsed += Time.deltaTime;
        formattedTime = TimeSpan.FromSeconds(timeElapsed).ToString(@"mm\:ss");
        timeText.text = formattedTime;
        lifeSlider.value = lives;
        TimeEvents();
        lives -= Time.deltaTime * 2;
    }

    private void Start()
    {
        StartGame();
        lives = 100;


    }

    public void StartGame()
    {
        randomFood.StartFoodGeneration(); // agora guarda referência e pode ser parado

    }

    public void TimeEvents()
    {
        if (timeElapsed >= 180f)
        {
            score = 10;
        }
        else if (timeElapsed >= 150f)
        {
            score = 6;
        }
        else if (timeElapsed >= 120f)
        {
            score = 4;
        }
        else if (timeElapsed >= 90f)
        {
            player.moveSpeed = 11f;
            delayToSpawnFood = 0.5f;
        }
        else if (timeElapsed >= 60f)
        {
            player.moveSpeed = 9f;
            delayToSpawnFood = 1f;
        }
        else if (timeElapsed >= 30f)
        {
            player.moveSpeed = 7f;
            delayToSpawnFood = 1.5f;
        }
        else if (timeElapsed >= 0f)
        {
            player.moveSpeed = 5f;
            delayToSpawnFood = 2f;
            score = 2;
        }
    }
}
