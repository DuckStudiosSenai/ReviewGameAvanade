using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class ReviewItemUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Button removeButton;
    public TextMeshProUGUI userNameText;
    public TextMeshProUGUI enterpriseNameText;
    public TextMeshProUGUI productNameText;
    public TextMeshProUGUI qualityText;
    public TextMeshProUGUI ratingText;

    private int reviewId;
    private APIManager apiManager;

    public void SetData(APIManager.ReviewObject review)
    {
        reviewId = review.id;
        apiManager = FindAnyObjectByType<APIManager>();

        userNameText.text = "Usuário: " + (review.userName ?? "Desconhecido");
        //enterpriseNameText.text = "Empresa: " + (review.enterpriseName ?? "Não informada");
        productNameText.text = "Produto: " + (review.productName ?? "Não informado");
        qualityText.text = $"Qualidade: {review.rating}/5";
        ratingText.text = "Avaliação: " + review.content;

        removeButton.onClick.RemoveAllListeners();
        removeButton.onClick.AddListener(DeleteReview);
    }

    private void DeleteReview()
    {
        Debug.Log($"🗑️ Tentando deletar review ID: {reviewId}");

        if (apiManager != null)
            StartCoroutine(apiManager.DeleteReview(reviewId, success =>
            {
                if (success)
                {
                    Debug.Log("🧹 Removendo item da UI...");
                    Destroy(gameObject);
                }
                else
                {
                    Debug.LogWarning("⚠️ Falha ao deletar review, item não removido.");
                }
            }));
    }
}
