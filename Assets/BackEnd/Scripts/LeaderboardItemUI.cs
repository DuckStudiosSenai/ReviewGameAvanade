using UnityEngine;
using TMPro;

public class LeaderboardItemUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI positionText;
    public TextMeshProUGUI userNameText;
    public TextMeshProUGUI pointsText;

    public void SetData(APIManager.LeaderboardUser user, int position)
    {
        positionText.text = position.ToString();
        userNameText.text = user.name ?? "Usuário Desconhecido";
        pointsText.text = user.reviewsCount.ToString();
    }
}
