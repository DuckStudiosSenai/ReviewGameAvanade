using UnityEngine;
using TMPro;

public class ReviewItemUI : MonoBehaviour
{
    public TextMeshProUGUI userNameText;
    public TextMeshProUGUI enterpriseNameText;
    public TextMeshProUGUI productNameText;
    public TextMeshProUGUI qualityText;
    public TextMeshProUGUI ratingText;

    public void SetData(APIManager.ReviewObject review)
    {
        userNameText.text = "Usuário: " + review.userName ?? "Usuário desconhecido";
        //enterpriseNameText.text = "Empresa: " + review.enterpriseName ?? "Empresa não informada";
        productNameText.text = "Produto: " + review.productName ?? "Produto não informado";
        qualityText.text = $"Qualidade: {review.rating}/5";
        ratingText.text = "Avaliação: " + review.content;
    }
}
