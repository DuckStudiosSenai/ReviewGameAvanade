using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Photon.Pun;

public class MakeReview : MonoBehaviour
{
    public bool isOpen = false;

    [Header("References")]
    public GameObject reviewMenu;
    public GameObject productsMenu;
    public TextMeshProUGUI productName;
    public TMP_InputField reviewText;
    public TMP_Dropdown reviewDropdown;
    public Button submitButton;
    public Button closeButton;
    public APIManager apiManager;
    
    [Header("Audio")]    
    public AudioSource audioSource;
    public AudioClip coinSound;
    public AudioClip submitReviewSound;

    [Header("Configuração de IDs")]
    private int userId;
    private int productId;

    private int selectedRating = 1;
    private PhotonView localPlayerView;
    private PointsManager pointsManager;

    private PlayerMovement localPlayerMovement;
    private CoinBurst coinBurst;
    private PlayerCoinText playerCoinText;
    private Animator animator;

    private PhotonView pv;

    void Start()
    {
        pv = GetComponent<PhotonView>();

        if (!pv.IsMine) return;

        reviewMenu.SetActive(false);

        pointsManager = FindAnyObjectByType<PointsManager>();

        StartCoroutine(WaitForLocalPlayer());
    }

    private IEnumerator WaitForLocalPlayer()
    {
        while (localPlayerView == null)
        {
            foreach (var view in FindObjectsByType<PhotonView>(FindObjectsSortMode.None))
            {
                if (view.IsMine && view.CompareTag("Player"))
                {
                    localPlayerView = view;
                    coinBurst = localPlayerView.GetComponent<CoinBurst>();
                    localPlayerMovement = localPlayerView.GetComponent<PlayerMovement>();
                    playerCoinText = localPlayerView.gameObject.transform.Find("PlayerCanvas/Coins/Counter").gameObject.GetComponent<PlayerCoinText>();
                    animator = localPlayerView.gameObject.transform.Find("PlayerCanvas/Coins/Image").gameObject.GetComponent<Animator>();
                    break;
                }
            }

            yield return new WaitForSeconds(0.1f);
        }

        Debug.Log("✅ Player local encontrado: " + localPlayerView.name);

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
        isOpen = open;

        if (open)
        {
            productName.text = name;
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

        OpenReviewMenu(false, 0, userId, null);

        audioSource.PlayOneShot(submitReviewSound);

        productsMenu.SetActive(false);

        localPlayerMovement.EnableMovement();

        if (localPlayerView.IsMine)
        {
            coinBurst.SpawnCoins(5);
        }

        pointsManager.AddPoints(userId, 5);
        
        StartCoroutine(LoadCoinsDelayed());
    }

    private IEnumerator LoadCoinsDelayed()
    {
        yield return new WaitForSeconds(3f);
        playerCoinText.LoadCoins();
        animator.SetTrigger("Flip");
        audioSource.PlayOneShot(coinSound);
    }
}
