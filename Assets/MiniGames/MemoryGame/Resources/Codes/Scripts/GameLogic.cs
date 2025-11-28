using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameLogic : MonoBehaviourPun
{
    public PointsManager pointsManager;
    public int pointsToWin = 5;
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


    public void Click(CardModel cardModel, Card card)
    {
        if (!CardsAnimator.isCardsClickable || card.isFlipping) return;

        var now = DateTime.UtcNow;

        if (now < _nextAllowedClick || _isComparing)
            return;

        _nextAllowedClick = now.AddMilliseconds(500);

        if (cardsAchados.Contains(cardModel) || cardsClicados.Contains(card) || cardsClicados.Count == 2)
            return;

        StartCoroutine(ClickRoutine(cardModel, card));
    }

    private IEnumerator ClickRoutine(CardModel cardModel, Card card)
    {
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
                    yield return new WaitForSeconds(0.5f);
                    matchAudio.Play();
                }
                else
                {
                    yield return new WaitForSeconds(0.5f);
                    _isComparing = true;
                    wrongAudio.Play();
                    yield return new WaitForSeconds(1.5f);

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
            yield return new WaitForSeconds(0.5f);
            winAudio.Play();
        }
    }


    private void YouWinCards()
    {
        pointsManager.AddPoints(PlayerPrefs.GetInt("UserId"), pointsToWin);
        cAnim.CallWinAnim();
    }

    private void ResetCards()
    {
        cAnim.ResetCardsToCenter();
    }

    private void EndGame()
    {
        PhotonNetwork.LoadLevel("Game");
    }
}
