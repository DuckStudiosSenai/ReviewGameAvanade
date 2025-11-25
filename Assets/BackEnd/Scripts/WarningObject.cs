using UnityEngine;
using Photon.Pun;

public class WarningObject : MonoBehaviour
{
    private Renderer[] renderers;
    private CanvasGroup[] canvasGroups;
    private CanvasRenderer[] canvasRenderers;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>(true);
        canvasGroups = GetComponentsInChildren<CanvasGroup>(true);
        canvasRenderers = GetComponentsInChildren<CanvasRenderer>(true);
    }

    private void Start()
    {
        foreach (var c in GetComponentsInChildren<Canvas>(true))
        {
            c.worldCamera = Camera.main;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        PlayerMovement pm = collision.GetComponent<PlayerMovement>();

        if (pm != null && pm.photonView.IsMine)
        {
            foreach (var r in renderers)
                r.enabled = false;

            foreach (var cg in canvasGroups)
            {
                cg.alpha = 0f;
                cg.blocksRaycasts = false;
                cg.interactable = false;
            }

            foreach (var cr in canvasRenderers)
                cr.SetAlpha(0f); 
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        PlayerMovement pm = collision.GetComponent<PlayerMovement>();

        if (pm != null && pm.photonView.IsMine)
        {
            foreach (var r in renderers)
                r.enabled = true;

            foreach (var cg in canvasGroups)
            {
                cg.alpha = 1f;
                cg.blocksRaycasts = true;
                cg.interactable = true;
            }

            foreach (var cr in canvasRenderers)
                cr.SetAlpha(1f);
        }
    }
}
