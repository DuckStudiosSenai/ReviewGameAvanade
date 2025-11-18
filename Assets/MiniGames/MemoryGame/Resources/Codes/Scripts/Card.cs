using System.Collections;
using UnityEngine;

public class Card : MonoBehaviour
{
    [SerializeField] private GameObject front;
    [SerializeField] private GameObject back;
    private bool isFront = true;
    public bool isFlipping = false;

    [Header("Audios")]
    public AudioSource flipAudio;
    public AudioSource dragAudio;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) {
            Flip();
        }
    }

    public void Flip()
    {
        if (isFlipping) return;
        StartCoroutine(FlipRoutine());
    }

    private IEnumerator FlipRoutine()
    {
        isFlipping = true;

        float duration = 0.25f;
        float t = 0f;

        Quaternion baseRot = Quaternion.identity;
        Quaternion startRot = baseRot;
        Quaternion midRot = baseRot * Quaternion.Euler(0, 90, 0);
        Quaternion endRot = baseRot;

        while (t < duration)
        {
            t += Time.deltaTime;
            transform.rotation = Quaternion.Lerp(startRot, midRot, t / duration);
            yield return null;
        }

        isFront = !isFront;
        front.SetActive(isFront);
        back.SetActive(!isFront);

        t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            transform.rotation = Quaternion.Lerp(midRot, endRot, t / duration);
            yield return null;
        }

        transform.rotation = baseRot;

        isFlipping = false;
    }
}
