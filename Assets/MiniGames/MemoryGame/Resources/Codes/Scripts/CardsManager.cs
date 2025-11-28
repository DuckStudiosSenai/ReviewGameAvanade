using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CardsManager : MonoBehaviour
{
    public List<CardModel> cards;

    public GameObject cardPrefab;
    public Transform grid;

    public CardsAnimator cAnim;

    public GameLogic gameLogic;

    void Start()
    {
        cards = cards.Concat(cards).ToList();
        InstantiateCards();
        cAnim.AnimateCards();
    }

    private void InstantiateCards()
    {
        ListExtensions.Shuffle(cards);
        foreach(CardModel card in cards) 
        {
            GameObject newCard = Instantiate(cardPrefab, grid, false);
            var image = newCard.transform.Find("Image/Front").GetComponent<Image>();
            image.sprite = card.cardImage;  

            Button button = newCard.GetComponent<Button>();
            button.onClick.AddListener(() => gameLogic.Click(card, newCard.GetComponent<Card>()));
        }
    }

    public static void FlipAllCards()
    {
        GameObject[] cardsObjects = GameObject.FindGameObjectsWithTag("Card");

        foreach(GameObject cardObject in cardsObjects)
        {
            cardObject.GetComponent<Card>().Flip();
            
        }
    }
}
