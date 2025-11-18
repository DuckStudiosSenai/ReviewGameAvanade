using UnityEngine;

public class MenuAmbiente : MonoBehaviour
{
    public AudioClip ambienteClip;

    void Start()
    {
        AudioSource audio = gameObject.AddComponent<AudioSource>();
        audio.clip = ambienteClip;
        audio.loop = true;
        audio.playOnAwake = true;
        audio.volume = 0.3f;
        audio.spatialBlend = 0f; // 2D
        audio.Play();
    }
}
