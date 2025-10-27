using Photon.Realtime;
using UnityEngine;

public class UIChatBotManager : MonoBehaviour
{
    [Header("References")]

    [Header("Buttons")]
    public GameObject teleportButton;

    [Header("Menus")]
    public GameObject mainMenu;
    public GameObject teleportMenu;

    private void Start()
    {
        CloseEveryMenu();
    }


    #region ======= Menus =======
    private void OpenTeleportMenu()
    {
        CloseEveryMenu();
        teleportMenu.SetActive(true);
    }

    public void OpenMainMenu()
    {
        CloseEveryMenu();
        mainMenu.SetActive(true);
    }

    #endregion

    public void CloseEveryMenu()
    {
        mainMenu.SetActive(false);
        teleportMenu.SetActive(false);
    }

    public void OnClick_ChatBot()
    {
        if (isMainMenuOpen())
            CloseEveryMenu();
        else
            OpenMainMenu();
    }

    private bool isMainMenuOpen() => mainMenu.activeSelf;
    
}
