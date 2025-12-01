using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float zoomScale = 1.15f;   // Quanto aumenta no hover
    public float speed = 10f;         // Velocidade da animação

private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(ScaleTo(originalScale * zoomScale));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(ScaleTo(originalScale));
    }

    private System.Collections.IEnumerator ScaleTo(Vector3 target)
    {
        while (Vector3.Distance(transform.localScale, target) > 0.01f)
        {
            transform.localScale = Vector3.Lerp(
                transform.localScale,
                target,
                Time.deltaTime * speed
            );

            yield return null;
        }

        transform.localScale = target;
    }

}
