using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;

public class CanvasSwitcher : MonoBehaviourPun
{

    [Header("Canvases")]
    public GameObject registerCanvas;
    public GameObject loginCanvas;

    [Header("Audio")]
    public AudioSource buttonSound;

    private void PlayButtonSound()
    {
        if (buttonSound != null)
            buttonSound.Play();
    }

    public void PlayGame()
    {
        EventSystem.current.SetSelectedGameObject(null);
        PlayButtonSound();
    }

    public void RegisterMenu()
    {
        EventSystem.current.SetSelectedGameObject(null);
        PlayButtonSound();
        if (loginCanvas != null) loginCanvas.SetActive(false);
        if (registerCanvas != null) registerCanvas.SetActive(true);
    }

    public void LoginMenu()
    {
        EventSystem.current.SetSelectedGameObject(null);
        PlayButtonSound();
        if (registerCanvas != null) registerCanvas.SetActive(false);
        if (loginCanvas != null) loginCanvas.SetActive(true);
    }

    public void ExitGame()
    {
        PlayButtonSound();
        Application.Quit();
        Debug.Log("Jogo fechado.");
    }
}
