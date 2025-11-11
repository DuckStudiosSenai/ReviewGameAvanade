using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class UIChatBotManager : MonoBehaviour
{
    [Header("Buttons")]
    public GameObject teleportButton;
    public GameObject secButton;
    public GameObject techButton;
    public GameObject dataButton;
    public GameObject cloudButton;
    public GameObject othersButton;

    [Header("Locations")]
    public Transform secLocation;
    public Transform techLocation;
    public Transform dataLocation;
    public Transform cloudLocation;
    public Transform othersLocation;

    [Header("Menus (objetos no mundo ou empties)")]
    public GameObject mainMenu;
    public GameObject teleportMenu;

    [Header("Animação")]
    [Range(0.1f, 1.5f)]
    public float animDuration = 0.3f;
    public AnimationCurve animCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private bool mainMenuOpen = false;
    private bool isAnimating = false;

    private NPCController npc;

    private Dictionary<GameObject, Coroutine> activeAnimations = new();

    private void Start()
    {
        npc = GetComponentInParent<NPCController>();

        CloseEveryMenuInstant();
        StartCoroutine(WaitForLocalPlayerAndSetup());
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

    public void CloseEveryMenuInstant()
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

    #region ==== Buttons ====

    private IEnumerator WaitForLocalPlayerAndSetup()
    {
        PlayerMovement player = null;

        while (player == null)
        {
            foreach (var p in FindObjectsByType<PlayerMovement>(FindObjectsSortMode.None))
            {
                if (p.photonView.IsMine)
                {
                    player = p;
                    break;
                }
            }

            if (player == null)
                yield return null;
        }

        SetupButtons(player);
    }

    private void SetupButtons(PlayerMovement player)
    {
        if (player == null)
        {
            Debug.LogError("❌ Player local não encontrado! Botões não configurados.");
            return;
        }

        secLocation = GameObject.Find("TP_Sec_Location").transform;
        techLocation = GameObject.Find("TP_Tech_Location").transform;
        dataLocation = GameObject.Find("TP_Data_Location").transform;
        cloudLocation = GameObject.Find("TP_Cloud_Location").transform;
        //othersLocation = GameObject.Find("TP_Others_Location").transform;

        secButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => player.TeleportTo(secLocation.position));
        techButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => player.TeleportTo(techLocation.position));
        dataButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => player.TeleportTo(dataLocation.position));
        cloudButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => player.TeleportTo(cloudLocation.position));
        //othersButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => player.TeleportTo(othersLocation.position));

        //secButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => npc.TeleportAndContinueChase(new Vector2(secLocation.position.x - 5, secLocation.position.y)));
        //techButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => npc.TeleportAndContinueChase(new Vector2(techLocation.position.x - 5, techLocation.position.y)));
        //dataButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => npc.TeleportAndContinueChase(new Vector2(dataLocation.position.x - 5, dataLocation.position.y)));
        //cloudButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => npc.TeleportAndContinueChase(new Vector2(cloudLocation.position.x - 5, cloudLocation.position.y)));
        ////othersButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => npc.TeleportAndContinueChase(new Vector2(othersLocation.position.x - 5, othersLocation.position.y)));

        Debug.Log("✅ Botões configurados com o player local!");
    }


    #endregion

}

