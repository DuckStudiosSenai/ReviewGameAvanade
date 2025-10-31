using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class MenuButtonEffect : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    ISelectHandler, IDeselectHandler
{
    public TextMeshProUGUI buttonText;
    public GameObject underline;
    public GameObject background;

    private Vector3 originalScale;
    private bool isHovered = false;

    void Start()
    {
        if (buttonText == null)
            buttonText = GetComponentInChildren<TextMeshProUGUI>();

        originalScale = buttonText.transform.localScale;

        if (underline != null)
            underline.SetActive(false);

        if (background != null)
            background.SetActive(false);
    }

    void Update()
    {
        // Escala suave
        Vector3 targetScale = isHovered ? originalScale * 1.1f : originalScale;
        buttonText.transform.localScale = Vector3.Lerp(buttonText.transform.localScale, targetScale, Time.deltaTime * 10f);
    }

    // -------- Mouse --------
    public void OnPointerEnter(PointerEventData eventData)
    {
        SetHover(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetHover(false);
    }

    // -------- Controle / Teclado --------
    public void OnSelect(BaseEventData eventData)
    {
        SetHover(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        SetHover(false);
    }

    // -------- Lógica de hover centralizada --------
    private void SetHover(bool active)
    {
        isHovered = active;

        if (underline != null)
            underline.SetActive(active);

        if (background != null)
            background.SetActive(active);
    }
}
