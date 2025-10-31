using UnityEngine;
using TMPro;

public class ReviewItemUI : MonoBehaviour
{
    public TextMeshProUGUI userNameText;
    public TextMeshProUGUI productNameText;
    public TextMeshProUGUI contentText;
    public TextMeshProUGUI ratingText;

    public void SetData(APIManager.ReviewObject review)
    {
        userNameText.text = review.userName ?? "Usuário desconhecido";
        productNameText.text = review.productName ?? "Produto não informado";
        contentText.text = review.content;
        ratingText.text = $"⭐ {review.rating}/5";
    }
}
