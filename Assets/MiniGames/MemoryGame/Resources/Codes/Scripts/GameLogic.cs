using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameLogic : MonoBehaviour
{
    public List<int> indiceClicados;
    public List<Card> cardsClicados;
    public List<CardModel> cardsAchados;

    public CardsManager cardsManager;
    public CardsAnimator cAnim;

    private DateTime _nextAllowedClick = DateTime.MinValue;
    private bool _isComparing = false;

    [Header("Audios")]
    public AudioSource matchAudio;
    public AudioSource wrongAudio;
    public AudioSource winAudio;


    public async void Click(CardModel cardModel, Card card)
    {

        if (!CardsAnimator.isCardsClickable || card.isFlipping) return;


        var now = DateTime.UtcNow;

        if (now < _nextAllowedClick || _isComparing)
            return;

        _nextAllowedClick = now.AddMilliseconds(500);

        if (cardsAchados.Contains(cardModel) || cardsClicados.Contains(card) || cardsClicados.Count == 2)
            return;

        if (indiceClicados.Count < 2)
        {
            cardsClicados.Add(card);
            indiceClicados.Add(cardModel.id);
            card.Flip();
            card.GetComponent<AudioSource>().Play();

            if (indiceClicados.Count == 2)
            {
                if (indiceClicados[0] == indiceClicados[1])
                {
                    Debug.Log("Match!");
                    cardsAchados.Add(cardModel);
                    await Task.Delay(500);
                    matchAudio.Play();
                }
                else
                {
                    
                    await Task.Delay(500);
                    _isComparing = true;
                    wrongAudio.Play();
                    await Task.Delay(1500);
                    foreach (var c in cardsClicados)
                    {
                        c.Flip();
                        c.GetComponent<AudioSource>().Play();
                    }
                        
                    _isComparing = false;
                }

                cardsClicados.Clear();
                indiceClicados.Clear();
            }
        }

        if (cardsAchados.Count == (cardsManager.cards.Count / 2))
        {
            Debug.Log("You win!");

            
            Invoke("YouWinCards", 2f);
            Invoke("ResetCards", 5f);
            Invoke("EndGame", 6.5f);
            await Task.Delay(500);
            winAudio.Play();
        }
    }

    private void YouWinCards()
    {
        cAnim.CallWinAnim();
    }

    private void ResetCards()
    {
        cAnim.ResetCardsToCenter();
    }

    private void EndGame()
    {
        SceneManager.LoadScene("Menu");
    }
}
