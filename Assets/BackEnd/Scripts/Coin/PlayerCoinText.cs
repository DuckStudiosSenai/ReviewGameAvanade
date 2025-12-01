using TMPro;
using UnityEngine;

public class PlayerCoinText : MonoBehaviour
{
    private PointsManager pointsManager;

    private TextMeshProUGUI coinText;

    private void Start()
    {
        pointsManager = FindAnyObjectByType<PointsManager>();
        coinText = GetComponent<TextMeshProUGUI>();

        LoadCoins();
    }

    public void LoadCoins()
    {
        Debug.Log("Carregando pontos do usuário...");
        StartCoroutine(pointsManager.GetUserPoints(
            PlayerPrefs.GetInt("UserId"),
            onSuccess: (points, currentPoints) =>
            {
                Debug.Log($"Points totais: {points}, CurrentPoints: {currentPoints}");
                coinText.text = currentPoints.ToString();
            },
            onError: (err) =>
            {
                Debug.LogError("Erro ao buscar pontos: " + err);
            }
        ));
    }
}
