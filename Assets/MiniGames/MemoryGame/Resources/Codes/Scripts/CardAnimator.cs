using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;
using System.Threading.Tasks;

public class CardsAnimator : MonoBehaviour
{
    public Transform grid;               
    public GridLayoutGroup layout;
    public GameLogic gl;

    private Vector3 center;

    public float animationSpeed = 0.8f;

    public static bool isCardsClickable = false;

    public void AnimateCards()
    {
        DistributeCards();
        Invoke("ResetCardsToCenter", 8f);
        cards = GameObject.FindGameObjectsWithTag("Card");
        Invoke("Flip", 8f);
        Invoke("DistributeCards", 10f);
        Invoke("StartGame", 11f);
    }

    public async Task DistributeCards()
    {
        GameObject[] cards = GameObject.FindGameObjectsWithTag("Card");

        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            grid as RectTransform,
            new Vector2(Screen.width / 2, Screen.height / 2),
            null,
            out center
        );

        layout.enabled = true;
        Canvas.ForceUpdateCanvases();

        List<Vector3> finalPositions = new List<Vector3>();
        foreach (GameObject card in cards)
            finalPositions.Add(card.GetComponent<RectTransform>().position);

        layout.enabled = false;

        foreach (GameObject card in cards)
            card.GetComponent<RectTransform>().position = center;

        for (int i = 0; i < cards.Length; i++)
        {
            
            RectTransform rect = cards[i].GetComponent<RectTransform>();
            rect.DOMove(finalPositions[i], animationSpeed)
                .SetEase(Ease.OutBack)
                .SetDelay(i * 0.05f);
            await Task.Delay(100);
            cards[i].GetComponent<Card>().dragAudio.Play();

        }
    }

    public void ResetCardsToCenter()
    {
        GameObject[] cards = GameObject.FindGameObjectsWithTag("Card");
        Card cardComponent = null;

        foreach (GameObject card in cards)
        {
            if (cardComponent == null)
                cardComponent = card.GetComponent<Card>();
            RectTransform rect = card.GetComponent<RectTransform>();

            rect.DOMove(center, animationSpeed)
                .SetEase(Ease.InOutCubic);
        }

        cardComponent.dragAudio.Play();
    }

    public void CallWinAnim()
    {
        StartCoroutine(YouWinCards());
    }

    GameObject[] cards;
    private IEnumerator YouWinCards()
    {
        
        foreach (GameObject card in cards)
        {
            card.GetComponent<Card>().Flip();
            card.GetComponent<AudioSource>().Play();
            yield return new WaitForSeconds(0.15f);
        }
    }

    private void Flip()
    {
        CardsManager.FlipAllCards();
        foreach(GameObject card in cards)
            card.GetComponent<AudioSource>().Play();
    }
    
    private void StartGame()
    {
        isCardsClickable = true;
    }
}
