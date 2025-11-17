using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class DeathScreen : MonoBehaviour
{
    [SerializeField] private GameObject deathCanvas;
    [SerializeField] private GameObject[] otherCanvases;

    [SerializeField] private AudioClip backgroundSongs;
     private AudioSource AudioSource;

    void Start()
    {

        AudioSource = GetComponent<AudioSource>(); // MOD

        // Garante que começa desligado
        if (deathCanvas != null)
            deathCanvas.SetActive(false);

       
    }

    public void ShowDeathScreen()
    {
        // Inicia a espera de 10 segundos antes de mostrar a tela
        StartCoroutine(ShowAfterDelay(2f));

        if (backgroundSongs != null)
        {
            AudioSource.PlayOneShot(backgroundSongs);
        }
    }

    private IEnumerator ShowAfterDelay(float delay)
    {
        // Espera X segundos (mesmo se Time.timeScale = 0)
        yield return new WaitForSecondsRealtime(delay);

        // Ativa a tela de morte
        if (deathCanvas != null)
            deathCanvas.SetActive(true);

        // Desliga os outros canvas
        foreach (GameObject canvas in otherCanvases)
        {
            if (canvas != null)
                canvas.SetActive(false);
        }

        // Pausa o jogo (se não quiser pausar, pode remover)
        Time.timeScale = 0f;

       


    }

    public void RestartGame()
    {
        // Despausa o jogo
        Time.timeScale = 1f;

        // Recarrega a cena atual
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
