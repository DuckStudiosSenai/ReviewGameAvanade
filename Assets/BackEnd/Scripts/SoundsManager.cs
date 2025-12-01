using UnityEngine;

public class SoundsManager : MonoBehaviour
{
    [Header("Button")]
    public AudioClip buttonClickSound;
    public AudioSource buttonAudioSource;

    public void PlayButtonClickSound()
    {
        if (buttonAudioSource != null && buttonClickSound != null)
        {
            buttonAudioSource.PlayOneShot(buttonClickSound);
        }
    }
}
