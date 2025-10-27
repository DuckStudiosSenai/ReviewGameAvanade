using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIChatBotManager : MonoBehaviour
{
    [Header("Buttons")]
    public GameObject teleportButton;

    [Header("Menus (objetos no mundo ou empties)")]
    public GameObject mainMenu;
    public GameObject teleportMenu;

    [Header("Animação")]
    [Range(0.1f, 1.5f)]
    public float animDuration = 0.3f;
    public AnimationCurve animCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private bool mainMenuOpen = false;
    private bool isAnimating = false;

    private Dictionary<GameObject, Coroutine> activeAnimations = new();

    private void Start()
    {
        CloseEveryMenuInstant();
    }

    #region ==== Menus ====
    public void OnClick_ChatBot()
    {
        if (isAnimating) return;

        if (mainMenuOpen)
            CloseEveryMenu();
        else
            OpenMainMenu();
    }

    private void OpenMainMenu()
    {
        CloseEveryMenu(); 
        AnimateMenu(mainMenu, true);
    }

    public void OpenTeleportMenu()
    {
        CloseEveryMenu();
        AnimateMenu(teleportMenu, true);
    }

    public void CloseEveryMenu()
    {
        AnimateMenu(mainMenu, false);
        AnimateMenu(teleportMenu, false);
    }

    private void CloseEveryMenuInstant()
    {
        SetMenuInstant(mainMenu, false);
        SetMenuInstant(teleportMenu, false);
    }

    #endregion

    #region ==== Animation Menu ====

    private void AnimateMenu(GameObject menu, bool show)
    {
        if (!menu) return;

        if (activeAnimations.ContainsKey(menu) && activeAnimations[menu] != null)
            StopCoroutine(activeAnimations[menu]);

        activeAnimations[menu] = StartCoroutine(AnimateScale(menu, show));
    }

    private IEnumerator AnimateScale(GameObject menu, bool show)
    {
        isAnimating = true;

        menu.SetActive(true);
        Vector3 start = menu.transform.localScale;
        Vector3 end = show ? Vector3.one : Vector3.zero;

        float t = 0f;

        while (t < animDuration)
        {
            t += Time.deltaTime;
            float normalized = Mathf.Clamp01(t / animDuration);
            float eval = animCurve.Evaluate(normalized);
            menu.transform.localScale = Vector3.Lerp(start, end, eval);
            yield return null;
        }

        menu.transform.localScale = end;

        if (!show)
            menu.SetActive(false);

        if (menu == mainMenu)
            mainMenuOpen = show;

        isAnimating = false;
    }

    private void SetMenuInstant(GameObject menu, bool show)
    {
        if (!menu) return;
        menu.SetActive(show);
        menu.transform.localScale = show ? Vector3.one : Vector3.zero;

        if (menu == mainMenu)
            mainMenuOpen = show;
    }

    #endregion
}
