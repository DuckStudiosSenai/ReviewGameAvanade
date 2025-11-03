using UnityEngine;
using Photon.Pun;
using System.Collections;

public class CameraFollowPhoton2D : MonoBehaviour
{
    [Header("Configurações de Suavização")]
    public float smoothSpeed = 0.125f;
    public Vector3 offset = new Vector3(0, 0, -10);

    private Transform target;
    private bool following = false;

    void Start()
    {
        StartCoroutine(WaitForLocalPlayer());
    }

    IEnumerator WaitForLocalPlayer()
    {
        while (target == null)
        {
            foreach (var view in FindObjectsByType<PhotonView>(FindObjectsSortMode.None))
            {
                if (view.IsMine && view.CompareTag("Player"))
                {
                    target = view.transform;
                    following = true;
                    break;
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    void LateUpdate()
    {
        if (!following || target == null) return;

        Vector3 desiredPosition = target.position + offset;

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, smoothedPosition.z);
    }
}
