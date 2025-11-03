using TMPro;
using UnityEngine;

public class MakeReview : MonoBehaviour
{
    [Header("References")]
    public GameObject reviewMenu;
    public TMP_InputField reviewText;
    public TMP_Dropdown reviewDropdown;

    private APIManager.ReviewCreateDTO currentReview;

    private void OpenReviewMenu(bool open, APIManager.ReviewCreateDTO review)
    {
        currentReview = review;
        reviewMenu.SetActive(open);
    }



}
