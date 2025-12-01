using System.Collections;
using UnityEngine;

public class AvatarTrigger : MonoBehaviour
{
    public Animator targetAnimator;

    private void Start()
    {
        StartCoroutine(WaitAndTrigger());
    }

    IEnumerator WaitAndTrigger()
    {
        yield return new WaitForSeconds(2f);
        targetAnimator.SetTrigger("WalkDown");
    }
}
