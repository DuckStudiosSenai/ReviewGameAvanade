using UnityEngine;

public class MainMenuCanvasManager : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject avatarMenu;

    private void Start()
    {
        CloseAvatarMenu();
    }

    public void OpenAvatarMenu()
    {
        mainMenu.SetActive(false);
        avatarMenu.SetActive(true);
    }

    public void CloseAvatarMenu()
    {
        avatarMenu.SetActive(false);
        mainMenu.SetActive(true);
    }
}
