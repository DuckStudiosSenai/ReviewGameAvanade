using UnityEngine;

public class MenuStart : MonoBehaviour
{
    [SerializeField] private GameObject menuCanvas; // arraste o Canvas aqui no Inspector
    [SerializeField] private GameObject otherCanvas; // Canvas que deve ser desativado quando o menu abrir

    [SerializeField] private AudioClip buttonClick;
    [SerializeField] private AudioSource backgroundSong;

    private AudioSource audioSource;


    void Start()
    {
        // Quando o jogo iniciar, abre o menu e pausa tudo
        menuCanvas.SetActive(true);
        otherCanvas.SetActive(false); // desliga o outro canvas

        Time.timeScale = 0f;

        audioSource = GetComponent<AudioSource>(); // MOD
    }

    public void StartGame()
    {

        if (buttonClick != null)
        {
            audioSource.PlayOneShot(buttonClick);
        }

        // Fecha o menu
        menuCanvas.SetActive(false);

        otherCanvas.SetActive(true);

        // Despausa o jogo
        Time.timeScale = 1f;

        backgroundSong.Play();
    }
}
