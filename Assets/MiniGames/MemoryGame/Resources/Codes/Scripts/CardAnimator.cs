using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class CardsAnimator : MonoBehaviour
{
    public Transform grid;
    public GridLayoutGroup layout;
    public GameLogic gl;

    private Vector3 center;

    public float animationSpeed = 0.8f;

    public static bool isCardsClickable = false;

    private GameObject[] cards;

    private void Awake()
    {
        // Garante que o DOTween está pronto no WebGL
        DOTween.Init(false, true, LogBehaviour.ErrorsOnly);
    }

    public void AnimateCards()
    {
        StartCoroutine(MainSequence());
    }

    /// <summary>
    /// Mantém a mesma ordem de execução do seu código original
    /// </summary>
    private IEnumerator MainSequence()
    {
        // Distribuir cartas ao grid
        yield return StartCoroutine(DistributeCards());

        // 8 segundos depois → juntar no centro
        yield return new WaitForSeconds(8f);
        ResetCardsToCenter();

        // Também aos 8s → virar cartas
        Flip();

        // 2s depois → distribuir novamente (igual ao Invoke de 10s)
        yield return new WaitForSeconds(2f);
        StartCoroutine(DistributeCards());

        // 1s depois → liberar clique (igual ao Invoke de 11s)
        yield return new WaitForSeconds(1f);
        StartGame();
    }

    /// <summary>
    /// Distribui as cartas a partir do centro
    /// </summary>
    private IEnumerator DistributeCards()
    {
        yield return null; // importante no WebGL para garantir que tudo existe

        cards = GameObject.FindGameObjectsWithTag("Card");

        // Calcula o centro corretamente para Canvas Overlay
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            grid as RectTransform,
            new Vector2(Screen.width / 2f, Screen.height / 2f),
            null,
            out center
        );

        layout.enabled = true;
        Canvas.ForceUpdateCanvases();

        // Salva as posições finais
        List<Vector3> finalPositions = new List<Vector3>();
        foreach (GameObject card in cards)
            finalPositions.Add(card.GetComponent<RectTransform>().position);

        // Desativa o grid para mover manualmente
        layout.enabled = false;

        // Move todas para o centro
        foreach (GameObject card in cards)
            card.GetComponent<RectTransform>().position = center;

        // Move uma a uma para a posição original
        for (int i = 0; i < cards.Length; i++)
        {
            RectTransform rect = cards[i].GetComponent<RectTransform>();

            rect.DOMove(finalPositions[i], animationSpeed)
                .SetEase(Ease.OutBack)
                .SetDelay(i * 0.05f);

            // Delay equivalente ao Task.Delay(100)
            yield return new WaitForSeconds(0.1f);

            cards[i].GetComponent<Card>().dragAudio.Play();
        }
    }

    /// <summary>
    /// Junta as cartas novamente ao centro
    /// </summary>
    public void ResetCardsToCenter()
    {
        GameObject[] cards = GameObject.FindGameObjectsWithTag("Card");
        Card cardComponent = null;

        foreach (GameObject card in cards)
        {
            if (cardComponent == null)
                cardComponent = card.GetComponent<Card>();

            card.GetComponent<RectTransform>()
                .DOMove(center, animationSpeed)
                .SetEase(Ease.InOutCubic);
        }

        cardComponent.dragAudio.Play();
    }

    /// <summary>
    /// Animação de vitória
    /// </summary>
    public void CallWinAnim()
    {
        StartCoroutine(YouWinCards());
    }

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
        foreach (GameObject card in cards)
            card.GetComponent<AudioSource>().Play();
    }

    private void StartGame()
    {
        isCardsClickable = true;
    }
}
