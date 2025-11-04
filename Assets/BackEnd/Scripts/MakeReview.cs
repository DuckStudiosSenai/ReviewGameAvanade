using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Photon.Pun;

public class MakeReview : MonoBehaviour
{
    [Header("References")]
    public GameObject reviewMenu;
    public TextMeshProUGUI productName;
    public TMP_InputField reviewText;
    public TMP_Dropdown reviewDropdown;
    public Button submitButton;
    public Button closeButton;
    public APIManager apiManager;

    [Header("Configuração de IDs")]
    private int userId;
    private int productId;

    private int selectedRating = 1;
    private PhotonView localPlayerView;

    void Start()
    {
        // Começa fechado
        reviewMenu.SetActive(false);

        // Inicia a espera pelo player local
        StartCoroutine(WaitForLocalPlayer());
    }

    private IEnumerator WaitForLocalPlayer()
    {
        // Espera até o player ser instanciado pela rede
        while (localPlayerView == null)
        {
            foreach (var view in FindObjectsByType<PhotonView>(FindObjectsSortMode.None))
            {
                if (view.IsMine && view.CompareTag("Player"))
                {
                    localPlayerView = view;
                    break;
                }
            }

            yield return new WaitForSeconds(0.1f);
        }

        Debug.Log("✅ Player local encontrado: " + localPlayerView.name);

        // Agora que o player local existe, pode habilitar a UI e listeners
        InitializeUI();
    }

    private void InitializeUI()
    {
        reviewDropdown.onValueChanged.AddListener(delegate { OnDropdownChanged(); });
        submitButton.onClick.AddListener(SubmitReview);
        closeButton.onClick.AddListener(() => OpenReviewMenu(false, 0, 0, null));

        reviewMenu.SetActive(false);
    }

    private void OnDropdownChanged()
    {
        selectedRating = reviewDropdown.value + 1;
        Debug.Log("⭐ Nota selecionada: " + selectedRating);
    }

    public void OpenReviewMenu(bool open, int productId, int userId, string name)
    {
        reviewMenu.SetActive(open);

        if (open)
        {
            productName.text = "Produto: " + name;
            this.productId = productId;
            this.userId = userId;

            Debug.Log($"🛍️ Abrindo menu de review para produto ID: {productId}, usuário ID: {userId}");

            reviewDropdown.value = 0;
            reviewText.text = "";
            selectedRating = 1;
        }
    }

    private void SubmitReview()
    {
        string comment = reviewText.text.Trim();

        if (string.IsNullOrEmpty(comment))
        {
            Debug.LogWarning("⚠️ O campo de texto do review está vazio!");
            return;
        }

        Debug.Log($"📤 Enviando review: {comment} (Rating {selectedRating}) para produto {productId} pelo usuário {userId}");

        APIManager.ReviewCreateDTO reviewData = new APIManager.ReviewCreateDTO
        {
            userId = userId,
            productId = productId,
            rating = selectedRating,
            content = comment
        };

        StartCoroutine(apiManager.CreateReview(reviewData));

        OpenReviewMenu(false, 0, 0, null);
    }
}
